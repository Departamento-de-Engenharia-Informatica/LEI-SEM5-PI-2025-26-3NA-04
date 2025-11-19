using System;

namespace APDL.API.Domain.StaffQualificationAggregate.DTO
{
    public class StaffQualificationDto
    {
        public Guid Id { get; set; }
        public string QualificationType { get; set; }
        public string QualificationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
