using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.NotificationAggregate.ValueObjects
{
    public class ExpectedDeparture : IValueObject
    {
        public DateTime Value { get; private set; }

        public ExpectedDeparture(DateTime value)
        {
            if (value == default)
                throw new BusinessRuleValidationException(nameof(ExpectedDeparture), "Expected departure date cannot be empty.");

            if (value.Date < DateTime.UtcNow.Date)
                throw new BusinessRuleValidationException(nameof(ExpectedDeparture), "Expected departure cannot be in the past.");

            Value = value;
        }

        protected bool Equals(ExpectedDeparture other) => Value.Equals(other.Value);
        public override bool Equals(object obj) => obj is ExpectedDeparture other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
    }
}

