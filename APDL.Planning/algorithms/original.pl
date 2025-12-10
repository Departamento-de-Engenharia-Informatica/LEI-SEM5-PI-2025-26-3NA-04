% First support for IARTI project 2025/2026
% Scheduling Vessels Unload/Load

:-dynamic shortest_delay/2.


vessel(va, 6, 63, 10, 16).
vessel(vb, 23, 50, 9, 7).
vessel(vc, 8, 40, 5, 12).
vessel(vd, 27, 40, 0, 8).
vessel(ve, 36, 70, 12, 0).
vessel(vf, 40, 60, 8, 6).
vessel(vg, 52, 80, 9, 10).
vessel(vi, 61, 90, 13, 8).
%vessel(vj, 74, 100, 7, 7).
%vessel(vk, 81, 110, 6, 8).
%vessel(vl, 90, 140, 22, 18).
%vessel(vm, 112, 140, 8, 7).
%vessel(vn, 82, 135, 13, 12).

sequence_temporization(LV,SeqTriplets):-
		sequence_temporization1(0,LV,SeqTriplets).


sequence_temporization1(EndPrevSeq,[V|LV],[(V,TInUnload,TEndLoad)|SeqTriplets]):-
			vessel(V,TIn,_,TUnload,TLoad),
			 ( (TIn> EndPrevSeq,!, TInUnload is TIn); TInUnload is EndPrevSeq+1),
		TEndLoad is TInUnload + TUnload+TLoad -1,
		sequence_temporization1(TEndLoad,LV,SeqTriplets).

sequence_temporization1(_,[],[]).


sum_delays([],0).

sum_delays([(V,_,TEndLoad)|LV],S):-
		vessel(V,_,TDep,_,_),TPossibleDep is TEndLoad+1,
		( (TPossibleDep>TDep,!,SV is TPossibleDep-TDep);SV is 0),
		sum_delays(LV,SLV),
		S is SV+SLV.


obtain_seq_shortest_delay(SeqBetterTriplets, SShortestDelay):-
    get_time(Ti),
    (obtain_seq_shortest_delay1;true),
    retract(shortest_delay(SeqBetterTriplets, SShortestDelay)),
    write('Better Sequence: '),write(SeqBetterTriplets),nl,
    write('Shortest Delay: '),write(SShortestDelay),nl,
    get_time(Tf),
    T is Tf-Ti,
    write('Time to generate the shortest delay solution: '),write(T),nl.


obtain_seq_shortest_delay1:-
    asserta(shortest_delay(_,100000)),
    findall(V,vessel(V,_,_,_,_),LV),!,
    permutation(LV,SeqV),
    sequence_temporization(SeqV,SeqTriplets),
    sum_delays(SeqTriplets,S),
    compare_shortest_delay(SeqTriplets,S),
    fail.

compare_shortest_delay(SeqTriplets,S):-
 shortest_delay(_,SLower),
    ((S<SLower,!,retract(shortest_delay(_,_)),asserta(shortest_delay(SeqTriplets,S)));true).