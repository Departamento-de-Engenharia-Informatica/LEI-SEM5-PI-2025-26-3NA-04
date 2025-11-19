using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.StaffQualificationAggregate.DTO
{
    public class CreateStaffQualificationDto
    {
        public string QualificationType { get; set; }
        public string QualificationName { get; set; }
        public string Description { get; set; }
    }
}
