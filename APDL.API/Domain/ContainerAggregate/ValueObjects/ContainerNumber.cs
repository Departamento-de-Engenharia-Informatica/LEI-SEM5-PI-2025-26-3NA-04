using System;
using System.Linq;
using System.Text.RegularExpressions;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ContainerAggregate.ValueObjects
{
    public class ContainerNumber : IValueObject
    {
        public string Value { get; }

        public ContainerNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(nameof(ContainerNumber), "Container number cannot be empty.");

            value = value.Trim().ToUpperInvariant();

            if (!IsValidContainerFormat(value))
                throw new BusinessRuleValidationException(nameof(ContainerNumber),
                    "Container number must follow ISO 6346 format: 3 letters (owner code), 1 category letter (U/J/Z), 6-digit serial number, and 1 check digit.");

            Value = value;
        }

        private static bool IsValidContainerFormat(string containerNumber)
        {
            if (string.IsNullOrWhiteSpace(containerNumber))
                return false;

            if (containerNumber.Length != 11)
                return false;

            string ownerCode = containerNumber.Substring(0, 3);
            char categoryId = containerNumber[3];
            string serialNumber = containerNumber.Substring(4, 6);
            char checkDigit = containerNumber[10];

            if (!ownerCode.All(char.IsLetter))
                return false;

            if (!char.IsLetter(categoryId))
                return false;

            if (!serialNumber.All(char.IsDigit))
                return false;

            if (!char.IsDigit(checkDigit))
                return false;

            return true;
        }


        protected bool Equals(ContainerNumber other) => Value == other.Value;
        public override bool Equals(object obj) => obj is ContainerNumber other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
    }
}
