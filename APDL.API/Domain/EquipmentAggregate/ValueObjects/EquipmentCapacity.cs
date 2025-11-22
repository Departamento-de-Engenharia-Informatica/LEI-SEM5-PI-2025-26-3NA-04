using APDL.API.Domain.Shared;

namespace APDL.API.Domain.MobileEquipmentAggregate.ValueObjects
{
    public class EquipmentCapacity : IValueObject
    {
        public int? ContainersPerTrip { get; }

        public double? AverageSpeedPerHour { get; }

        public int? ContainersPerHour { get; }

        private EquipmentCapacity() { }

        private EquipmentCapacity(int containersPerTrip, double averageSpeedPerHour)
        {
            ContainersPerTrip = containersPerTrip;
            AverageSpeedPerHour = averageSpeedPerHour;
            ContainersPerHour = null;
        }

        private EquipmentCapacity(int containersPerHour)
        {
            ContainersPerHour = containersPerHour;
            ContainersPerTrip = null;
            AverageSpeedPerHour = null;
        }

        public static EquipmentCapacity ForTruck(int containersPerTrip, double averageSpeedPerHour)
        {
            if (containersPerTrip <= 0)
                throw new BusinessRuleValidationException(
                    "Containers per trip must be greater than zero.",
                    nameof(containersPerTrip)
                );

            if (averageSpeedPerHour <= 0)
                throw new BusinessRuleValidationException(
                    "Average speed must be greater than zero.",
                    nameof(averageSpeedPerHour)
                );

            return new EquipmentCapacity(containersPerTrip, averageSpeedPerHour);
        }

        public static EquipmentCapacity ForCrane(int containersPerHour)
        {
            if (containersPerHour <= 0)
                throw new BusinessRuleValidationException(
                    "Containers per hour must be greater than zero.",
                    nameof(containersPerHour)
                );

            return new EquipmentCapacity(containersPerHour);
        }

        public override bool Equals(object obj)
        {
            return obj is EquipmentCapacity other
                && ContainersPerTrip == other.ContainersPerTrip
                && AverageSpeedPerHour == other.AverageSpeedPerHour
                && ContainersPerHour == other.ContainersPerHour;
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(
                ContainersPerTrip,
                AverageSpeedPerHour,
                ContainersPerHour
            );
        }

        public override string ToString()
        {
            if (ContainersPerHour.HasValue)
                return $"{ContainersPerHour} containers/hour";

            return $"{ContainersPerTrip} containers/trip @ {AverageSpeedPerHour} km/h";
        }
    }
}
