using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;

namespace APDL.API.Domain.QualificationsAggregate
{
    public class StaffQualification : Entity<StaffQualificationId>, IAggregateRoot
    {
        public QualificationType QualificationType { get; private set; }
        public string QualificationName { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }

        protected StaffQualification() { }

        public StaffQualification(
            StaffQualificationId id,
            QualificationType qualificationType,
            string qualificationName,
            string description
        )
        {
            if (qualificationType == null)
                throw new BusinessRuleValidationException(
                    "Qualification type cannot be null.",
                    nameof(qualificationType)
                );

            if (string.IsNullOrWhiteSpace(qualificationName))
                throw new BusinessRuleValidationException(
                    "Qualification name cannot be empty.",
                    nameof(qualificationName)
                );

            if (string.IsNullOrWhiteSpace(description))
                throw new BusinessRuleValidationException(
                    "Description cannot be empty.",
                    nameof(description)
                );

            this.Id = id;
            this.QualificationType = qualificationType;
            this.QualificationName = qualificationName;
            this.Description = description;
            this.IsActive = true;
        }

        public void UpdateQualificationName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new BusinessRuleValidationException(
                    "Qualification name cannot be empty.",
                    nameof(newName)
                );

            this.QualificationName = newName;
        }

        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new BusinessRuleValidationException(
                    "Description cannot be empty.",
                    nameof(newDescription)
                );

            this.Description = newDescription;
        }

        public void Activate()
        {
            this.IsActive = true;
        }

        public void Deactivate()
        {
            this.IsActive = false;
        }
    }
}
