using System;
using APDL.API.Domain.Shared;
using System.Text.RegularExpressions;

namespace APDL.API.Domain.ShippingAgentAggregate
{
    public class Email : IValueObject
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Email cannot be empty.");

            if (!Regex.IsMatch(value,@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new BusinessRuleValidationException($"Invalid email format: {value}");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Email))
                return false;

            var other = (Email)obj;
            return Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
