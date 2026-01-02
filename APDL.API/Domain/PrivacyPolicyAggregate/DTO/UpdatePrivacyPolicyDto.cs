using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.PrivacyPolicyAggregate.DTO
{
    public class UpdatePrivacyPolicyDto
    {
        [Required]
        public string Content { get; set; }
    }
}

