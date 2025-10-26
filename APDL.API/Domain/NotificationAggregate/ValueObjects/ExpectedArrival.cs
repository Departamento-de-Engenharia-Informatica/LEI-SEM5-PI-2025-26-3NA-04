using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.NotificationAggregate.ValueObjects
{
    public class ExpectedArrival : IValueObject
    {
        public DateTime Value { get; private set; }

        public ExpectedArrival(DateTime value)
        {
            if (value == default)
                throw new BusinessRuleValidationException(nameof(ExpectedArrival), "Expected arrival date cannot be empty.");

            if (value.Date < DateTime.UtcNow.Date)
                throw new BusinessRuleValidationException(nameof(ExpectedArrival), "Expected arrival cannot be in the past.");

            Value = value;
        }

        protected bool Equals(ExpectedArrival other) => Value.Equals(other.Value);
        public override bool Equals(object obj) => obj is ExpectedArrival other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
    }
}