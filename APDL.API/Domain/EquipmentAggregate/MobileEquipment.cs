using System;
using APDL.API.Domain.MobileEquipmentAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;

namespace APDL.API.Domain.EquipmentAggregate
{
    public class MobileEquipment : Entity<MobileEquipmentId>, IAggregateRoot
    {
        public string EquipmentName { get; private set; }
        public EquipmentType EquipmentType { get; private set; }

        public OperationalWindow OperationalWindow { get; private set; }
        public EquipmentCapacity Capacity { get; private set; }
        public ResourceStatus Status { get; private set; }

        public int RequiredOperators { get; private set; }
        public QualificationType RequiredQualification { get; private set; }

        public TimeSpan SetupTime { get; private set; }

        protected MobileEquipment() { }

        public MobileEquipment(
            MobileEquipmentId id,
            string equipmentName,
            EquipmentType equipmentType,
            OperationalWindow operationalWindow,
            EquipmentCapacity capacity,
            TimeSpan setupTime,
            int requiredOperators,
            QualificationType requiredQualification
        )
        {
            if (string.IsNullOrWhiteSpace(equipmentName))
                throw new BusinessRuleValidationException(
                    "Equipment name cannot be empty.",
                    nameof(equipmentName)
                );

            if (equipmentType == null)
                throw new BusinessRuleValidationException(
                    "Equipment type cannot be null.",
                    nameof(equipmentType)
                );

            if (operationalWindow == null)
                throw new BusinessRuleValidationException(
                    "Operational window cannot be null.",
                    nameof(operationalWindow)
                );

            if (capacity == null)
                throw new BusinessRuleValidationException(
                    "Capacity cannot be null.",
                    nameof(capacity)
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

            if (requiredQualification == null)
                throw new BusinessRuleValidationException(
                    "Required qualification cannot be null.",
                    nameof(requiredQualification)
                );

            this.Id = id;
            this.EquipmentName = equipmentName;
            this.EquipmentType = equipmentType;
            this.OperationalWindow = operationalWindow;
            this.Capacity = capacity;
            this.SetupTime = setupTime;
            this.RequiredOperators = requiredOperators;
            this.RequiredQualification = requiredQualification;
            this.Status = ResourceStatus.Available;
        }

        public void MarkAsAvailable()
        {
            this.Status = ResourceStatus.Available;
        }

        public void MarkAsUnavailable()
        {
            this.Status = ResourceStatus.Unavailable;
        }

        public void MarkAsOutOfService()
        {
            this.Status = ResourceStatus.OutOfService;
        }

        public void MarkAsUnderMaintenance()
        {
            this.Status = ResourceStatus.UnderMaintenance;
        }

        public void UpdateEquipmentName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new BusinessRuleValidationException(
                    "Equipment name cannot be empty.",
                    nameof(newName)
                );

            this.EquipmentName = newName;
        }

        public void UpdateOperationalWindow(OperationalWindow newWindow)
        {
            if (newWindow == null)
                throw new BusinessRuleValidationException(
                    "Operational window cannot be null.",
                    nameof(newWindow)
                );

            this.OperationalWindow = newWindow;
        }

        public void UpdateCapacity(EquipmentCapacity newCapacity)
        {
            if (newCapacity == null)
                throw new BusinessRuleValidationException(
                    "Capacity cannot be null.",
                    nameof(newCapacity)
                );

            this.Capacity = newCapacity;
        }

        public void UpdateSetupTime(TimeSpan newSetupTime)
        {
            if (newSetupTime < TimeSpan.Zero)
                throw new BusinessRuleValidationException(
                    "Setup time cannot be negative.",
                    nameof(newSetupTime)
                );

            this.SetupTime = newSetupTime;
        }

        public bool IsAvailable()
        {
            return Status.IsAvailable();
        }

        public bool IsAvailableAt(DateTime dateTime)
        {
            return Status.IsAvailable() && OperationalWindow.IsAvailableAt(dateTime);
        }

        public bool IsAvailableForDuration(DateTime start, TimeSpan duration)
        {
            return Status.IsAvailable()
                && OperationalWindow.IsAvailableForDuration(start, duration);
        }
    }
}
