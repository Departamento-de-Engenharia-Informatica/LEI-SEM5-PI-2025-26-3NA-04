
namespace APDL.API.Domain.ShippingAgentAggregate.ValueObjects
{
    public class Name : IValueObject
    {
        public string Value { get; }

        public Name(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Legal name cannot be empty.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is LegalName other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}