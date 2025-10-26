using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ContainerAggregate.ValueObjects
{
    public class VesselPosition : IValueObject
    {
        public int Bay { get; private set; }
        public int Row { get; private set; }
        public int Tier { get; private set; }

        public VesselPosition(int bay, int row, int tier)
        {
            if (bay < 1)
                throw new BusinessRuleValidationException(nameof(VesselPosition), "Bay must be above 1.");

            if (row < 1)
                throw new BusinessRuleValidationException(nameof(VesselPosition), "Row must be above 1..");

            if (tier < 1)
                throw new BusinessRuleValidationException(nameof(VesselPosition), "Tier must be above 1..");

            Bay = bay;
            Row = row;
            Tier = tier;
        }

        protected bool Equals(VesselPosition other) => 
            Bay == other.Bay && Row == other.Row && Tier == other.Tier;
        
        public override bool Equals(object obj) => obj is VesselPosition other && Equals(other);
        
        public override int GetHashCode() => HashCode.Combine(Bay, Row, Tier);

        public override string ToString() => $"Bay: {Bay}, Row: {Row}, Tier: {Tier}";
 
   }
}