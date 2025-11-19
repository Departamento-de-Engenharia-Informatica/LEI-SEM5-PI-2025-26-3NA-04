using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.OperatingStaffAggregate.DTO
{
    public class UpdateStaffStatusDto
    {
        [Required]
        public string Status { get; set; }
    }
}
