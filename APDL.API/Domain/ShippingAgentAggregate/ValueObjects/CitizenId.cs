using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ShippingAgentAggregate.ValueObjects
{
    public class CitizenId : IValueObject
    {
        public string Value { get; }

        public CitizenId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Citizen ID cannot be empty.", nameof(value));

            if (value.Length < 8)
                throw new BusinessRuleValidationException("Citizen ID must be at least 8 characters long.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is CitizenId other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}