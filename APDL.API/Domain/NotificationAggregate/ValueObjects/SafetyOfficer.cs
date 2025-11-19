using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.NotificationAggregate.ValueObjects
{
    public class SafetyOfficer : IValueObject
    {
        public string Name { get; private set; }

        protected SafetyOfficer() { }

        public SafetyOfficer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleValidationException(nameof(SafetyOfficer), "Safety officer name cannot be empty.");

            Name = name.Trim();
        }

        protected bool Equals(SafetyOfficer other) => Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        public override bool Equals(object obj) => obj is SafetyOfficer other && Equals(other);
        public override int GetHashCode() => Name.ToLower().GetHashCode();
    }
}