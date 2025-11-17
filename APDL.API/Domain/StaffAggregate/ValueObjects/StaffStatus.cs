using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.OperatingStaffAggregate.ValueObjects
{
    public class StaffStatus : IValueObject
    {
        public StaffStatusEnum Value { get; }

        private StaffStatus(StaffStatusEnum value)
        {
            Value = value;
        }

        public static StaffStatus Available => new StaffStatus(StaffStatusEnum.AVAILABLE);
        public static StaffStatus Unavailable => new StaffStatus(StaffStatusEnum.UNAVAILABLE);
        public static StaffStatus OnLeave => new StaffStatus(StaffStatusEnum.ON_LEAVE);
        public static StaffStatus InTraining => new StaffStatus(StaffStatusEnum.IN_TRAINING);

        public static StaffStatus FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(
                    "Staff status cannot be empty.",
                    nameof(value)
                );

            if (!Enum.TryParse<StaffStatusEnum>(value.ToUpper().Replace(" ", "_"), out var status))
                throw new BusinessRuleValidationException(
                    $"Invalid Staff Status: '{value}'. Valid values are: AVAILABLE, UNAVAILABLE, ON_LEAVE, IN_TRAINING, TEMPORARILY_REASSIGNED.",
                    nameof(value)
                );

            return new StaffStatus(status);
        }

        public bool IsAvailable() => Value == StaffStatusEnum.AVAILABLE;

        public bool IsUnavailable() => Value == StaffStatusEnum.UNAVAILABLE;

        public bool IsOnLeave() => Value == StaffStatusEnum.ON_LEAVE;

        public bool IsInTraining() => Value == StaffStatusEnum.IN_TRAINING;

        public override bool Equals(object obj)
        {
            return obj is StaffStatus other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public enum StaffStatusEnum
    {
        AVAILABLE,
        UNAVAILABLE,
        ON_LEAVE,
        IN_TRAINING,
    }
}
