using System;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.StaffQualificationAggregate.DTO
{
    public class UpdateStaffQualificationDto
    {
        public Guid Id { get; set; }

        public string QualificationName { get; set; }

        public string Description { get; set; }
    }
}
