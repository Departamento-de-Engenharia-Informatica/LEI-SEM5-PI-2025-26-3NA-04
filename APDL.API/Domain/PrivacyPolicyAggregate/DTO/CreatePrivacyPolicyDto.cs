using System;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.PrivacyPolicyAggregate.DTO
{
    public class CreatePrivacyPolicyDto
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }
}

