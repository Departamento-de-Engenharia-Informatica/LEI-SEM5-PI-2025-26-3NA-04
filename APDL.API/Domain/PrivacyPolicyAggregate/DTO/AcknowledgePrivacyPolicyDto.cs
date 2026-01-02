using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.PrivacyPolicyAggregate.DTO
{
    public class AcknowledgePrivacyPolicyDto
    {
        [Required]
        public int Version { get; set; }
    }
}

