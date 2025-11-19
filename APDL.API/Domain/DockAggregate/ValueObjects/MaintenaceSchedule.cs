using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.DockAggregate.ValueObjects
{
    public class MaintenanceSchedule : IValueObject
    {
        public DateTime ScheduledDate { get; }
        public DateTime EstimatedEndDate { get; }
        public string Description { get; }
        public MaintenanceType Type { get; }

        public MaintenanceSchedule(
            DateTime scheduledDate,
            DateTime estimatedEndDate,
            string description,
            MaintenanceType type
        )
        {
            if (scheduledDate == default)
                throw new BusinessRuleValidationException(
                    "Scheduled date cannot be empty.",
                    nameof(scheduledDate)
                );

            if (estimatedEndDate < scheduledDate)
                throw new BusinessRuleValidationException(
                    "Estimated end date cannot be before scheduled date.",
                    nameof(estimatedEndDate)
                );

            if (string.IsNullOrWhiteSpace(description))
                throw new BusinessRuleValidationException(
                    "Description cannot be empty.",
                    nameof(description)
                );

            ScheduledDate = scheduledDate;
            EstimatedEndDate = estimatedEndDate;
            Description = description;
            Type = type;
        }

        public TimeSpan EstimatedDuration => EstimatedEndDate - ScheduledDate;

        public bool IsScheduledFor(DateTime date) =>
            date.Date >= ScheduledDate.Date && date.Date <= EstimatedEndDate.Date;

        public bool OverlapsWith(MaintenanceSchedule other) =>
            ScheduledDate < other.EstimatedEndDate && EstimatedEndDate > other.ScheduledDate;

        public bool IsUpcoming(DateTime currentDate) => ScheduledDate > currentDate;

        public bool IsInProgress(DateTime currentDate) =>
            currentDate >= ScheduledDate && currentDate <= EstimatedEndDate;

        public override bool Equals(object obj)
        {
            return obj is MaintenanceSchedule other
                && ScheduledDate == other.ScheduledDate
                && EstimatedEndDate == other.EstimatedEndDate
                && Description == other.Description
                && Type == other.Type;
        }

        public override int GetHashCode() =>
            HashCode.Combine(ScheduledDate, EstimatedEndDate, Description, Type);

        public override string ToString() =>
            $"{Type} maintenance from {ScheduledDate:yyyy-MM-dd} to {EstimatedEndDate:yyyy-MM-dd}: {Description}";
    }

    public enum MaintenanceType
    {
        ROUTINE,
        EMERGENCY,
        INSPECTION,
        REPAIR,
        UPGRADE,
    }
}
