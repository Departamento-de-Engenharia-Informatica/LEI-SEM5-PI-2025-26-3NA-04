% First support for IARTI project 2025/2026
% Scheduling Vessels Unload/Load

:-dynamic shortest_delay/2.
:-dynamic dock_occupation/2.
:-dynamic staff_occupation/2.
:-dynamic storage_level/1.

vessel(va, 6, 63, 10, 16).
vessel(vb, 23, 50, 9, 7).
vessel(vc, 8, 40, 5, 12).
vessel(vd, 27, 40, 0, 8).
vessel(ve, 36, 70, 12, 0).
vessel(vf, 40, 60, 8, 6).
vessel(vg, 52, 80, 9, 10).
vessel(vi, 61, 90, 13, 8).
vessel(vj, 61, 100, 7, 7).
vessel(vk, 81, 110, 6, 8).
%vessel(vl, 90, 140, 22, 18).
%vessel(vm, 112, 140, 8, 7).
%vessel(vn, 82, 135, 13, 12).

dock(dock_a, crane01).
dock(dock_b, crane02).
dock(dock_c, crane03).

crane(crane01, 10).
crane(crane02, 0.8).
crane(crane03, 1).

staff(staff01, 36, 84).
staff(staff02, 36, 84).
staff(staff03, 36, 84).

staff(staff04, 84, 132).
staff(staff05, 84, 132).
staff(staff06, 84, 132).

staff(staff07, 132, 180).
staff(staff08, 132, 180).
staff(staff09, 0, 36).
staff(staff10, 0, 36). 

storage(storage_a, 100, 500).

initialize_docks :-
    retractall(dock_occupation(_,_)),
    findall(DockId, dock(DockId, _), Docks),
    forall(member(D, Docks), asserta(dock_occupation(D, 0))).

initialize_staff :-
    retractall(staff_occupation(_, _)),
    findall(StaffId, staff(StaffId, _, _), StaffList),
    forall(member(S, StaffList), asserta(staff_occupation(S, 0))).

initialize_storage :-
    retractall(storage_level(_)),
    storage(_, InitialLevel, _),
    asserta(storage_level(InitialLevel)).

sequence_temporization(LV, SeqTriplets) :-
    initialize_docks,
    initialize_staff,
    initialize_storage,
    sequence_temporization1(LV, SeqTriplets).

sequence_temporization1([], []).

sequence_temporization1([V|LV], [(V, TInUnload, TEndLoad, DockID, CraneID, StaffID, StorageID, TimeFactor, VesselDelay)|SeqTriplets]) :-
    vessel(V, TIn, TDep, TUnload, TLoad),
    
    findall(
        (EndTime, VesselDelay, Factor, Dock, Crane, Staff, Storage, StartTime),
        (
            dock(Dock, Crane),
            
            crane(Crane, Factor),
            
            dock_occupation(Dock, DockFreeAt),
            
            staff(Staff, ShiftStart, ShiftEnd),
            
            staff_occupation(Staff, StaffFreeAt),
            
            storage(Storage, _, MaxCap),
            
            storage_level(CurrentLevel),
            
            StorageChange is TUnload - TLoad,
            NewStorageLevel is CurrentLevel + StorageChange,
            NewStorageLevel >= 0,
            NewStorageLevel =< MaxCap,
            
            StartTime is max(max(TIn, DockFreeAt), max(ShiftStart, StaffFreeAt)),
            
            TotalTime is (TUnload + TLoad) * Factor,
            EndTime is StartTime + TotalTime - 1,
            
            StartTime >= ShiftStart,
            StartTime < ShiftEnd,
            
            TPossibleDep is EndTime + 1,
            (TPossibleDep > TDep -> VesselDelay is TPossibleDep - TDep ; VesselDelay = 0)        
        ),
        AvailableOptions
    ),

    sort(AvailableOptions, [(TEndLoad, VesselDelay, TimeFactor, DockID, CraneID, StaffID, StorageID, TInUnload)|_]),
    
    retract(dock_occupation(DockID, _)),
    NewDockFree is TEndLoad + 1,
    asserta(dock_occupation(DockID, NewDockFree)),
    
    retract(staff_occupation(StaffID, _)),
    NewStaffFree is TEndLoad + 1,
    asserta(staff_occupation(StaffID, NewStaffFree)),
    
    vessel(V, _, _, TUnload, TLoad),
    retract(storage_level(OldLevel)),
    StorageChange is TUnload - TLoad,
    NewLevel is OldLevel + StorageChange,
    asserta(storage_level(NewLevel)),
    
    sequence_temporization1(LV, SeqTriplets).

sum_delays([], 0).
sum_delays([(_, _, _, _, _, _, _, _, VesselDelay)|LV], S) :-
    sum_delays(LV, SLV),
    S is VesselDelay + SLV.

obtain_seq_shortest_delay(SeqBetterTriplets, SShortestDelay) :-
    get_time(Ti),
    (obtain_seq_shortest_delay1; true),
    retract(shortest_delay(SeqBetterTriplets, SShortestDelay)),
    write('Better Sequence: '), write(SeqBetterTriplets), nl,
    write('Shortest Delay: '), write(SShortestDelay), nl,
    get_time(Tf),
    T is Tf - Ti,
    write('Time to generate the shortest delay solution: '), write(T), nl.

obtain_seq_shortest_delay1 :-
    asserta(shortest_delay(_, 100000)),
    findall(V, vessel(V, _, _, _, _), LV), !,
    permutation(LV, SeqV),
    sequence_temporization(SeqV, SeqTriplets),
    sum_delays(SeqTriplets, S),
    compare_shortest_delay(SeqTriplets, S),
    fail.

compare_shortest_delay(SeqTriplets, S) :-
    shortest_delay(_, SLower),
    ((S < SLower, !, retract(shortest_delay(_, _)), asserta(shortest_delay(SeqTriplets, S))); true).