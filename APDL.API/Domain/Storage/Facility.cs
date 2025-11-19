// using System;
// using System.Collections.Generic;
// using APDL.API.Domain.DockAggregate;
// using APDL.API.Domain.Shared;
// using APDL.API.Domain.Storage.ValueObjects;


// namespace APDL.API.Domain.Storage;
    
// public abstract class Facility : Entity<FacilityId>, IAggregateRoot
// {
//     public Location Location { get; private set; }
//     public TEUCapacity MaxCapacityTEU { get; private set; }
//     public int CurrentOccupancyTEU { get; private set; }

//     public List<FacilityDockAssignment> DockAssignments { get; private set; } = new();

//     protected Facility() { }

//     protected Facility(string type, Location location, TEUCapacity maxCapacityTEU)
//     {
//         Id = new FacilityId(Guid.NewGuid());
//         Location = location;
//         MaxCapacityTEU = maxCapacityTEU;
//         CurrentOccupancyTEU = 0;
//     }

//     public void ChangeCurrentOccupancyTEU(int currentOccupancyTEU)
//     {
//         if (currentOccupancyTEU > MaxCapacityTEU.Value)
//             throw new BusinessRuleValidationException("Current occupancy cannot exceed max capacity.");
//         CurrentOccupancyTEU = currentOccupancyTEU;
//     }

//     public void AssignDock(DockId dockId, Distance distance)
//     {
//         DockAssignments.Add(new FacilityDockAssignment(this.Id, dockId, distance));
//     }
// }
