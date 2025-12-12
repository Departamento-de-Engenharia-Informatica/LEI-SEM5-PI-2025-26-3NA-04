:-dynamic generations/1.
:-dynamic population/1.
:-dynamic prob_crossover/1.
:-dynamic prob_mutation/1.
:-dynamic elitism_percentage/1.
:-dynamic target_fitness/1.
:-dynamic time_limit/1.
:-dynamic shortest_delay/2.
:-dynamic dock/2.

% Vessel definitions - vessel(Id, ArrivalTime, DepartureTime, UnloadTime, LoadTime)
vessel(va, 6, 63, 10, 16).
vessel(vb, 23, 50, 9, 7).
vessel(vc, 8, 40, 5, 12).
vessel(vd, 27, 40, 0, 8).
vessel(ve, 36, 70, 12, 0).
vessel(vf, 40, 60, 8, 6).
vessel(vg, 52, 80, 9, 10).
vessel(vi, 61, 90, 13, 8).
vessel(vj, 74, 100, 7, 7).
vessel(vk, 81, 110, 6, 8).
vessel(vl, 90, 140, 22, 18).

dock(dock1, 1).
dock(dock2, 1).
dock(dock3, 2).
dock(dock4, 4).

% vessels(NVessels) - counts total vessels
vessels(N):- findall(V, vessel(V,_,_,_,_), L), length(L, N).

% parameters initialization
initialize:-
    write('Number of new generations: '), read(NG),
    (retract(generations(_));true), asserta(generations(NG)),
    write('Population size: '), read(PS),
    (retract(population(_));true), asserta(population(PS)),
    write('Elitism percentage: '), read(EP),
    (retract(elitism_percentage(_));true), asserta(elitism_percentage(EP)),
    write('Probability of crossover (%): '), read(P1),
    PC is P1/100,
    (retract(prob_crossover(_));true), asserta(prob_crossover(PC)),
    write('Probability of mutation (%): '), read(P2),
    PM is P2/100,
    (retract(prob_mutation(_));true), asserta(prob_mutation(PM)),
    write('Target fitness (0 for no target): '), read(TF),
    (retract(target_fitness(_));true), asserta(target_fitness(TF)),
    write('Time limit in seconds (0 for no limit): '), read(TL),
    (retract(time_limit(_));true), asserta(time_limit(TL)).

% Main generation predicate
generate:-
    initialize,
    generate_population(Pop),
    write('Initial Population='), write(Pop), nl,
    evaluate_population(Pop, PopValue),
    write('Initial PopValue='), write(PopValue), nl,
    order_population(PopValue, PopOrd),
    generations(NG),
    get_time(StartTime),
    generate_generation(0, NG, PopOrd, StartTime).

% Generate initial population
generate_population(Pop):-
    population(PopSize),
    vessels(NumV),
    findall(Vessel, vessel(Vessel,_,_,_,_), VesselsList),
    generate_population(PopSize, VesselsList, NumV, Pop).

generate_population(0, _, _, []):-!.
generate_population(PopSize, VesselsList, NumV, [Ind|Rest]):-
    PopSize1 is PopSize-1,
    generate_population(PopSize1, VesselsList, NumV, Rest),
    generate_individual(VesselsList, NumV, Ind),
    not(member(Ind, Rest)).
generate_population(PopSize, VesselsList, NumV, L):-
    generate_population(PopSize, VesselsList, NumV, L).

% Generate individual (random permutation of vessels)
generate_individual([G], 1, [G]):-!.
generate_individual(VesselsList, NumV, [G|Rest]):-
    NumTemp is NumV + 1,
    random(1, NumTemp, N),
    remove(N, VesselsList, G, NewList),
    NumV1 is NumV-1,
    generate_individual(NewList, NumV1, Rest).

remove(1, [G|Rest], G, Rest).
remove(N, [G1|Rest], G, [G1|Rest1]):-
    N1 is N-1,
    remove(N1, Rest, G, Rest1).

% Evaluate population - calculate delays for each sequence
evaluate_population([], []).
evaluate_population([Ind|Rest], [Ind*V|Rest1]):-
    evaluate(Ind, V),
    evaluate_population(Rest, Rest1).

% Evaluate a single vessel sequence
evaluate(SeqV, SumDelays):-
    sequence_temporization(SeqV, SeqTriplets),
    sum_delays(SeqTriplets, SumDelays).

% Calculate timing for vessel sequence
sequence_temporization(LV, SeqTriplets):-
    sequence_temporization1(0, LV, SeqTriplets).

sequence_temporization1(EndPrevSeq, [V|LV], [(V,TInUnload,TEndLoad)|SeqTriplets]):-
    vessel(V, TIn, _, TUnload, TLoad),
    ((TIn > EndPrevSeq, !, TInUnload is TIn); TInUnload is EndPrevSeq+1),
    TEndLoad is TInUnload + TUnload + TLoad - 1,
    sequence_temporization1(TEndLoad, LV, SeqTriplets).
sequence_temporization1(_, [], []).

% Calculate sum of delays
sum_delays([], 0).
sum_delays([(V,_,TEndLoad)|LV], S):-
    vessel(V, _, TDep, _, _),
    TPossibleDep is TEndLoad + 1,
    ((TPossibleDep > TDep, !, SV is TPossibleDep - TDep); SV is 0),
    sum_delays(LV, SLV),
    S is SV + SLV.

% Order population by fitness (ascending - lower delay is better)
order_population(PopValue, PopValueOrd):-
    bsort(PopValue, PopValueOrd).

bsort([X], [X]):-!.
bsort([X|Xs], Ys):-
    bsort(Xs, Zs),
    bchange([X|Zs], Ys).

bchange([X], [X]):-!.
bchange([X*VX,Y*VY|L1], [Y*VY|L2]):-
    VX > VY, !,
    bchange([X*VX|L1], L2).
bchange([X|L1], [X|L2]):-
    bchange(L1, L2).

% Selection with elitism from combined population
select_population(SortedCombinedPop, NewPop):-
    population(PopSize),
    elitism_percentage(ElitismPct),
    NumElite is ceiling((PopSize * ElitismPct) / 100),
    NumLottery is PopSize - NumElite,
    
    take_n(NumElite, SortedCombinedPop, EliteIndividuals),
    remove_elite(NumElite, SortedCombinedPop, NonElitePop),

    fitness_lottery(NumLottery, NonElitePop, LotteryWinners),
    append(EliteIndividuals, LotteryWinners, NewPop).


% Remove first N elements from list (the elite individuals)
remove_elite(0, L, L):-!.
remove_elite(_, [], []):-!.
remove_elite(N, [_|Rest], Result):-
    N > 0,
    N1 is N - 1,
    remove_elite(N1, Rest, Result).

% Take first N elements from list
take_n(0, _, []):-!.
take_n(_, [], []):-!.
take_n(N, [X|Rest], [X|Result]):-
    N > 0,
    N1 is N - 1,
    take_n(N1, Rest, Result).

% Fitness-based lottery: each individual gets fitness * random(0,1), select best scores
fitness_lottery(0, _, []):-!.
fitness_lottery(NumToSelect, CombinedPop, [Winner|Rest]):-
    NumToSelect > 0,
    apply_random_multipliers(CombinedPop, MultipliedPop),
    order_population(MultipliedPop, [BestMultiplied|_]),
    BestMultiplied = Ind*_,
    find_original_fitness(Ind, CombinedPop, Winner),
    NumToSelect1 is NumToSelect - 1,
    fitness_lottery(NumToSelect1, CombinedPop, Rest).

% Find the original individual*fitness pair in the population
find_original_fitness(_, [], _):-!, fail.
find_original_fitness(TargetInd, [Ind*Fitness|_], Ind*Fitness):-
    TargetInd = Ind, !.
find_original_fitness(TargetInd, [_|Rest], Result):-
    find_original_fitness(TargetInd, Rest, Result).

% Apply random multiplier between 0 and 1 to each individuals fitness
apply_random_multipliers([], []).
apply_random_multipliers([Ind*Fitness|Rest], [Ind*NewFitness|Result]):-
    random(0.0, 1.0, Multiplier),
    NewFitness is Fitness * Multiplier,
    apply_random_multipliers(Rest, Result).

% Generate successive generations
generate_generation(G, G, Pop, _):-!,
    write('Generation '), write(G), write(':'), nl, write(Pop), nl,
    write('Stopping: Maximum generations reached'), nl.

generate_generation(N, G, Pop, StartTime):-
    Pop = [_*BestFitness|_],
    
    (check_target_fitness(BestFitness), !,
        write('Generation '), write(N), write(':'), nl, write(Pop), nl,
        write('Stopping: Target fitness reached'), nl
    ;
    check_time_limit(StartTime), !,
        write('Generation '), write(N), write(':'), nl, write(Pop), nl,
        write('Stopping: Time limit exceeded'), nl
    ;
        write('Generation '), write(N), write(':'), nl, write(Pop), nl,
        
        random_permutation(Pop, ShuffledPop),
        
        crossover(ShuffledPop, NPop1),
        mutation(NPop1, NPop),
        
        evaluate_population(NPop, NPopValue),
        
        append(Pop, NPopValue, CombinedPop),
        
        remove_duplicates(CombinedPop, UniquePop),
        
        order_population(UniquePop, CombinedSorted),
        
        select_population(CombinedSorted, NPopOrd),
        
        N1 is N + 1,
        generate_generation(N1, G, NPopOrd, StartTime)
    ).

% Check if target fitness has been reached
check_target_fitness(BestFitness):-
    target_fitness(TF),
    TF > 0,
    BestFitness =< TF.

% Check if time limit has been exceeded
check_time_limit(StartTime):-
    time_limit(TL),
    TL > 0,
    get_time(CurrentTime),
    ElapsedTime is CurrentTime - StartTime,
    ElapsedTime >= TL.

% Generate crossover points
generate_crossover_points(P1, P2):-
    generate_crossover_points1(P1, P2).

generate_crossover_points1(P1, P2):-
    vessels(N),
    NTemp is N + 1,
    random(1, NTemp, P11),
    random(1, NTemp, P21),
    P11 \== P21, !,
    ((P11 < P21, !, P1=P11, P2=P21); P1=P21, P2=P11).
generate_crossover_points1(P1, P2):-
    generate_crossover_points1(P1, P2).

% Crossover operation
crossover([], []).
crossover([Ind*_], [Ind]).
crossover([Ind1*_,Ind2*_|Rest], [NInd1,NInd2|Rest1]):-
    generate_crossover_points(P1, P2),
    prob_crossover(Pcruz),
    random(0.0, 1.0, Pc),
    ((Pc =< Pcruz, !,
        cross(Ind1, Ind2, P1, P2, NInd1),
        cross(Ind2, Ind1, P1, P2, NInd2))
    ;
    (NInd1=Ind1, NInd2=Ind2)),
    crossover(Rest, Rest1).

% Order-1 crossover implementation
fillh([], []).
fillh([_|R1], [h|R2]):-
    fillh(R1, R2).

sublist(L1, I1, I2, L):-
    I1 < I2, !,
    sublist1(L1, I1, I2, L).
sublist(L1, I1, I2, L):-
    sublist1(L1, I2, I1, L).

sublist1([X|R1], 1, 1, [X|H]):-!, fillh(R1, H).
sublist1([X|R1], 1, N2, [X|R2]):-!,
    N3 is N2 - 1,
    sublist1(R1, 1, N3, R2).
sublist1([_|R1], N1, N2, [h|R2]):-
    N3 is N1 - 1,
    N4 is N2 - 1,
    sublist1(R1, N3, N4, R2).

rotate_right(L, K, L1):-
    vessels(N),
    T is N - K,
    rr(T, L, L1).

rr(0, L, L):-!.
rr(N, [X|R], R2):-
    N1 is N - 1,
    append(R, [X], R1),
    rr(N1, R1, R2).

remove([], _, []):-!.
remove([X|R1], L, [X|R2]):-
    not(member(X, L)), !,
    remove(R1, L, R2).
remove([_|R1], L, R2):-
    remove(R1, L, R2).

remove_duplicates([], []).
remove_duplicates([Ind*Fit|Rest], [Ind*Fit|Result]):-
    not(member_individual(Ind, Rest)),
    !,
    remove_duplicates(Rest, Result).

remove_duplicates([Ind*Fit|Rest], Result):-
    member_individual(Ind, Rest),
    !,
    remove_duplicates(Rest, Result).

member_individual(_, []):- fail.
member_individual(Target, [Ind*_|_]):-
    Target = Ind,
    !.
member_individual(Target, [_|Rest]):-
    member_individual(Target, Rest).

insert([], L, _, L):-!.
insert([X|R], L, N, L2):-
    vessels(T),
    ((N > T, !, N1 is N mod T); N1 = N),
    insert1(X, N1, L, L1),
    N2 is N + 1,
    insert(R, L1, N2, L2).

insert1(X, 1, L, [X|L]):-!.
insert1(X, N, [Y|L], [Y|L1]):-
    N1 is N - 1,
    insert1(X, N1, L, L1).

cross(Ind1, Ind2, P1, P2, NInd11):-
    sublist(Ind1, P1, P2, Sub1),
    vessels(NumV),
    R is NumV - P2,
    rotate_right(Ind2, R, Ind21),
    remove(Ind21, Sub1, Sub2),
    P3 is P2 + 1,
    insert(Sub2, Sub1, P3, NInd1),
    removeh(NInd1, NInd11).

removeh([], []).
removeh([h|R1], R2):-!,
    removeh(R1, R2).
removeh([X|R1], [X|R2]):-
    removeh(R1, R2).

% Mutation operation
mutation([], []).
mutation([Ind|Rest], [NInd|Rest1]):-
    prob_mutation(Pmut),
    random(0.0, 1.0, Pm),
    ((Pm < Pmut, !, mutacao1(Ind, NInd)); NInd = Ind),
    mutation(Rest, Rest1).

mutacao1(Ind, NInd):-
    generate_crossover_points(P1, P2),
    mutacao22(Ind, P1, P2, NInd).

mutacao22([G1|Ind], 1, P2, [G2|NInd]):-!,
    P21 is P2 - 1,
    mutacao23(G1, P21, Ind, G2, NInd).
mutacao22([G|Ind], P1, P2, [G|NInd]):-
    P11 is P1 - 1,
    P21 is P2 - 1,
    mutacao22(Ind, P11, P21, NInd).

mutacao23(G1, 1, [G2|Ind], G2, [G1|Ind]):-!.
mutacao23(G1, P, [G|Ind], G2, [G|NInd]):-
    P1 is P - 1,
    mutacao23(G1, P1, Ind, G2, NInd).

% Distribute vessels across docks
distribute_vessels_to_docks(BestSequence, Assignments):-
    findall(dock(DockId, NumCranes), dock(DockId, NumCranes), Docks),
    initialize_dock_loads(Docks, DockLoads),
    distribute_vessels(BestSequence, DockLoads, Assignments).

% Initialize all docks with 0 load
initialize_dock_loads([], []).
initialize_dock_loads([dock(DockId, NumCranes)|Rest], [dock(DockId, NumCranes, 0, [])|Rest1]):-
    initialize_dock_loads(Rest, Rest1).

% Distribute each vessel to dock with lowest load
distribute_vessels([], DockLoads, DockLoads):-!.
distribute_vessels([Vessel|RestVessels], DockLoads, FinalAssignments):-
    find_min_load_dock(DockLoads, MinDock),
    MinDock = dock(DockId, NumCranes, CurrentLoad, VesselList),
    
    vessel(Vessel, _, _, Unload, Load),
    VesselTime is Unload + Load,
    AddedLoad is ceiling(VesselTime / NumCranes),
    NewLoad is CurrentLoad + AddedLoad,
    
    append(VesselList, [Vessel], NewVesselList),
    UpdatedDock = dock(DockId, NumCranes, NewLoad, NewVesselList),
    
    replace_dock(DockId, DockLoads, UpdatedDock, NewDockLoads),
    
    distribute_vessels(RestVessels, NewDockLoads, FinalAssignments).

% Find dock with minimum load
find_min_load_dock([Dock], Dock):-!.
find_min_load_dock([Dock1|Rest], MinDock):-
    find_min_load_dock(Rest, Dock2),
    Dock1 = dock(_, _, Load1, _),
    Dock2 = dock(_, _, Load2, _),
    ((Load1 =< Load2, !, MinDock = Dock1); MinDock = Dock2).

% Replace dock in list
replace_dock(_, [], _, []).
replace_dock(DockId, [dock(DockId, NC, _, _)|Rest], NewDock, [NewDock|Rest]):-!.
replace_dock(DockId, [Dock|Rest], NewDock, [Dock|Rest1]):-
    replace_dock(DockId, Rest, NewDock, Rest1).