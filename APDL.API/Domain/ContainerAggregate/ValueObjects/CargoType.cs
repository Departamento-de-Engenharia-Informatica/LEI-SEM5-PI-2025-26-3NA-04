using APDL.API.Domain.Shared;


namespace APDL.API.Domain.ContainerAggregate.ValueObjects
{
    public class CargoType : IValueObject
    {
        public string Value { get; }

        public CargoType(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Cargo Type cannot be empty.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is CargoType other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}