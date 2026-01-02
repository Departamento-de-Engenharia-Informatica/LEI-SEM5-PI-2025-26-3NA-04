using System;

namespace APDL.API.Domain.PrivacyPolicyAggregate.DTO
{
    public class PrivacyPolicyDto
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string Content { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

