using APDL.API.Domain.Shared;
using System.Text.RegularExpressions;

namespace APDL.API.Domain.ShippingAgentAggregate.ValueObjects
{
    public class TaxNumber : IValueObject
    {
        public string Value { get; }

        public TaxNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Tax number cannot be empty.", nameof(value));

            if (!Regex.IsMatch(value, @"^\d{9}$"))
                throw new BusinessRuleValidationException("Tax number must be 9 digits.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is TaxNumber other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}