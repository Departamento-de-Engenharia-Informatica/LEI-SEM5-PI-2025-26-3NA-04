using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.VesselTypes
{
    public class VesselType : Entity<VesselTypeId>, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Capacity { get; private set; }
        public int MaxRows { get; private set; }
        public int MaxBays { get; private set; }
        public int MaxTiers { get; private set; }

        public VesselType(string name, string description, int capacity, int maxRows, int maxBays, int maxTiers)
        {
            Id = new VesselTypeId(Guid.NewGuid());
            Name = name;
            Description = description;
            Capacity = capacity;
            MaxRows = maxRows;
            MaxBays = maxBays;
            MaxTiers = maxTiers;
        }

        public void Update(string name, string description, int capacity, int maxRows, int maxBays, int maxTiers)
        {
            Name = name;
            Description = description;
            Capacity = capacity;
            MaxRows = maxRows;
            MaxBays = maxBays;
            MaxTiers = maxTiers;
        }
    }
}
