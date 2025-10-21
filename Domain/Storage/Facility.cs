using System;
using System.Collections.Generic;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Storage
{
    public abstract class Facility : Entity<FacilityId>, IAggregateRoot
    {
        public string Location { get; private set; }
        public int MaxCapacityTEU { get; private set; }
        public int CurrentOccupancyTEU { get; private set; }
        //public List<Container> Containers { get; private set; }
        //public List<DockId> AssignedDocks { get; private set; }

        public Facility(string type, string location, int maxCapacityTEU)
        {
            Id = new FacilityId(Guid.NewGuid());
            Location = location;
            MaxCapacityTEU = maxCapacityTEU;
            CurrentOccupancyTEU = 0;
            //Containers = new List<Container>();
            //AssignedDocks = new List<DockId>();
        }

        public void ChangeCurrentOccupancyTEU(int currentOccupancyTEU)
        {
            if (currentOccupancyTEU > this.MaxCapacityTEU)
                throw new BusinessRuleValidationException("It is not possible for current capacity to exceed maximum capacity.");

            this.CurrentOccupancyTEU = currentOccupancyTEU;
        }

        /*public void AddContainer(Container container)
        {
            if (CurrentOccupancyTEU + container.TEU > MaxCapacityTEU)
                throw new BusinessRuleValidationException("Exceeds max capacity.");
            Containers.Add(container);
            CurrentOccupancyTEU += container.TEU;
        }

        public void RemoveContainer(Container container)
        {
            if (Containers.Remove(container))
                CurrentOccupancyTEU -= container.TEU;
        }*/
    }
}
