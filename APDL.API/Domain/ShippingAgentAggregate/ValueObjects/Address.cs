using APDL.API.Domain.Shared;


namespace APDL.API.Domain.ShippingAgentAggregate.ValueObjects
{
    public class Address : IValueObject
    {
        public string Value { get; }

        public Address(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Address name cannot be empty.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Address other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}