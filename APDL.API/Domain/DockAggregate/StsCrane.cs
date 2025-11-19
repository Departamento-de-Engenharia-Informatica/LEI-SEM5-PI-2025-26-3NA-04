using System;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;

namespace APDL.API.Domain.DockAggregate
{
    public class StsCrane : Entity<StsCraneId>
    {
        public string CraneName { get; private set; }
        public DockId DockId { get; private set; }
        public OperationalWindow OperationalWindow { get; private set; }
        public int CapacityContainersPerHour { get; private set; }
        public ResourceStatus Status { get; private set; }
        public int RequiredOperators { get; private set; }
        public QualificationType RequiredQualification { get; private set; }
        public TimeSpan SetupTime { get; private set; }
        public UpcomingMaintenances UpcomingMaintenances { get; private set; }

        protected StsCrane() { }

        public StsCrane(
            StsCraneId id,
            string craneName,
            DockId dockId,
            OperationalWindow operationalWindow,
            int capacityContainersPerHour,
            TimeSpan setupTime,
            int requiredOperators = 1
        )
        {
            if (string.IsNullOrWhiteSpace(craneName))
                throw new BusinessRuleValidationException(
                    "Crane name cannot be empty.",
                    nameof(craneName)
                );

            if (dockId == null)
                throw new BusinessRuleValidationException(
                    "Crane must belong to a dock.",
                    nameof(dockId)
                );

            if (operationalWindow == null)
                throw new BusinessRuleValidationException(
                    "Operational window cannot be null.",
                    nameof(operationalWindow)
                );

            if (capacityContainersPerHour <= 0)
                throw new BusinessRuleValidationException(
                    "Capacity must be greater than zero.",
                    nameof(capacityContainersPerHour)
                );

            if (setupTime < TimeSpan.Zero)
                throw new BusinessRuleValidationException(
                    "Setup time cannot be negative.",
                    nameof(setupTime)
                );

            if (requiredOperators <= 0)
                throw new BusinessRuleValidationException(
                    "Required operators must be at least 1.",
                    nameof(requiredOperators)
                );

            this.Id = id;
            this.CraneName = craneName;
            this.DockId = dockId;
            this.OperationalWindow = operationalWindow;
            this.CapacityContainersPerHour = capacityContainersPerHour;
            this.SetupTime = setupTime;
            this.RequiredOperators = requiredOperators;
            this.RequiredQualification = QualificationType.STSCraneOperator;
            this.Status = ResourceStatus.Available;
            this.UpcomingMaintenances = new UpcomingMaintenances();
        }
    }
}
