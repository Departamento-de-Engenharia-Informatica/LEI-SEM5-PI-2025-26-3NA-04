using System;
using System.Collections.Generic;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselTypes.ValueObjects;

namespace APDL.API.Domain.VesselTypes
{
    
    public class VesselType : Entity<VesselTypeId>, IAggregateRoot
    {
        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public Capacity Capacity { get; private set; }
        public Dimension MaxRows { get; private set; }
        public Dimension MaxBays { get; private set; }
        public Dimension MaxTiers { get; private set; }

        public VesselType() { }
        public VesselType(string name, string description, int capacity, int maxRows, int maxBays, int maxTiers)
        {
            Id = new VesselTypeId(Guid.NewGuid());
            Name = new Name(name);
            Description = new Description(description);
            Capacity = new Capacity(capacity);
            MaxRows = new Dimension(maxRows);
            MaxBays = new Dimension(maxBays);
            MaxTiers = new Dimension(maxTiers);
        }

        public void Update(string name, string description, int capacity, int maxRows, int maxBays, int maxTiers)
        {
            Name = new Name(name);
            Description = new Description(description);
            Capacity = new Capacity(capacity);
            MaxRows = new Dimension(maxRows);
            MaxBays = new Dimension(maxBays);
            MaxTiers = new Dimension(maxTiers);
        }
    }

}
