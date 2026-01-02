using System;
using APDL.API.Domain.Shared;
using APDL.API.Domain.UserAggregate;

namespace APDL.API.Domain.PrivacyPolicyAggregate
{
    public class PrivacyPolicy : Entity<PrivacyPolicyId>, IAggregateRoot
    {
        public new Guid Id { get; set; }
        public int Version { get; private set; }
        public string Content { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        public bool IsActive { get; private set; }
        public Guid CreatedBy { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private PrivacyPolicy() { }

        public PrivacyPolicy(int version, string content, DateTime effectiveDate, Guid createdBy)
        {
            var guid = Guid.NewGuid();
            Id = guid;
            base.Id = new PrivacyPolicyId(guid);
            Version = version;
            Content = content;
            EffectiveDate = effectiveDate;
            IsActive = false;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContent(string content)
        {
            if (IsActive)
            {
                throw new InvalidOperationException("Cannot update an active privacy policy. Create a new version instead.");
            }
            Content = content;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

