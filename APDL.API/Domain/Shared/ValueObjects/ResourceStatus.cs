using System;

namespace APDL.API.Domain.Shared.ValueObjects
{
    public class ResourceStatus : IValueObject
    {
        public ResourceStatusEnum Value { get; }

        private ResourceStatus(ResourceStatusEnum value)
        {
            Value = value;
        }

        private ResourceStatus() { }


        public static ResourceStatus Available => new ResourceStatus(ResourceStatusEnum.AVAILABLE);
        public static ResourceStatus Unavailable =>
            new ResourceStatus(ResourceStatusEnum.UNAVAILABLE);
        public static ResourceStatus OutOfService =>
            new ResourceStatus(ResourceStatusEnum.OUT_OF_SERVICE);
        public static ResourceStatus UnderMaintenance =>
            new ResourceStatus(ResourceStatusEnum.UNDER_MAINTENANCE);

        public static ResourceStatus FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(
                    "Resource status cannot be empty.",
                    nameof(value)
                );

            if (
                !Enum.TryParse<ResourceStatusEnum>(
                    value.ToUpper().Replace(" ", "_"),
                    out var status
                )
            )
                throw new BusinessRuleValidationException(
                    $"Invalid Resource Status: '{value}'. Valid values are: AVAILABLE, UNAVAILABLE, OUT_OF_SERVICE, UNDER_MAINTENANCE.",
                    nameof(value)
                );

            return new ResourceStatus(status);
        }

        public static ResourceStatus FromEnum(ResourceStatusEnum status)
        {
            return new ResourceStatus(status);
        }

        public bool IsAvailable() => Value == ResourceStatusEnum.AVAILABLE;

        public bool IsUnavailable() => Value == ResourceStatusEnum.UNAVAILABLE;

        public bool IsOutOfService() => Value == ResourceStatusEnum.OUT_OF_SERVICE;

        public bool IsUnderMaintenance() => Value == ResourceStatusEnum.UNDER_MAINTENANCE;

        public override bool Equals(object obj)
        {
            return obj is ResourceStatus other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public enum ResourceStatusEnum
    {
        AVAILABLE,
        UNAVAILABLE,
        OUT_OF_SERVICE,
        UNDER_MAINTENANCE,
    }
}
