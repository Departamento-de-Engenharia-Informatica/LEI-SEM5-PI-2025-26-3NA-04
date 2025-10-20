using System.Text.RegularExpressions;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ShippingAgentAggregate.ValueObjects
{
    public class Phone : IValueObject
    {
        public string Value { get; }

        public Phone(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Phone number cannot be empty.", nameof(value));

            if (!Regex.IsMatch(value, @"^9\d{8}$"))
                throw new BusinessRuleValidationException("Invalid phone number format.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Phone other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}