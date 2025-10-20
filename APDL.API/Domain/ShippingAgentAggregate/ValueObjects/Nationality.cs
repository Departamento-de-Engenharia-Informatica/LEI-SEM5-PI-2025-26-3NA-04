using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ShippingAgentAggregate.ValueObjects
{
    public class Nationality : IValueObject
    {
        public string Value { get; }

        public Nationality(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Nationality cannot be empty.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Nationality other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}