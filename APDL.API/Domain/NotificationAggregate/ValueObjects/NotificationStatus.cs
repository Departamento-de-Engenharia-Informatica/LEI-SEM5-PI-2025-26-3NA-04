using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.NotificationAggregate.ValueObjects
{
    public class NotificationStatus : IValueObject
    {
        public string Value { get; private set; }

        public static readonly NotificationStatus InProgress = new NotificationStatus("InProgress");
        public static readonly NotificationStatus Submitted = new NotificationStatus("Submitted");
        public static readonly NotificationStatus Approved = new NotificationStatus("Approved");
        public static readonly NotificationStatus Rejected = new NotificationStatus("Rejected");

        private NotificationStatus(string value)
        {
            Value = value;
        }

        public NotificationStatus(string value, bool validate = true)
        {
            if (!validate)
            {
                Value = value;
                return;
            }

            var normalizedValue = value?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedValue))
                throw new BusinessRuleValidationException(
                    nameof(NotificationStatus),
                    "Status cannot be empty."
                );

            if (
                !normalizedValue.Equals("InProgress", StringComparison.OrdinalIgnoreCase)
                && !normalizedValue.Equals("Submitted", StringComparison.OrdinalIgnoreCase)
                && !normalizedValue.Equals("Approved", StringComparison.OrdinalIgnoreCase)
                && !normalizedValue.Equals("Rejected", StringComparison.OrdinalIgnoreCase)
            )
            {
                throw new BusinessRuleValidationException(
                    nameof(NotificationStatus),
                    "Status must be 'InProgress', 'Submitted', 'Approved', or 'Rejected'."
                );
            }

            Value = normalizedValue switch
            {
                var s when s.Equals("InProgress", StringComparison.OrdinalIgnoreCase) =>
                    "InProgress",
                var s when s.Equals("Submitted", StringComparison.OrdinalIgnoreCase) => "Submitted",
                var s when s.Equals("Approved", StringComparison.OrdinalIgnoreCase) => "Approved",
                var s when s.Equals("Rejected", StringComparison.OrdinalIgnoreCase) => "Rejected",
                _ => normalizedValue,
            };
        }

        protected bool Equals(NotificationStatus other) =>
            Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is NotificationStatus other && Equals(other);

        public override int GetHashCode() => Value.ToLower().GetHashCode();

        public override string ToString() => Value;
    }
}
