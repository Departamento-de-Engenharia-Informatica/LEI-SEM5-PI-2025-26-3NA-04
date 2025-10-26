using APDL.API.Domain.Shared;

namespace APDL.API.Domain.CargoManifestAggregate.ValueObjects
{
    public class CargoVolume : IValueObject
    {
        public int Value { get; }

        public CargoVolume(int value)
        {
            if (value < 0)
                throw new BusinessRuleValidationException("Cargo volume cannot be negative.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is CargoVolume other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"{Value} m³";
    }
}
