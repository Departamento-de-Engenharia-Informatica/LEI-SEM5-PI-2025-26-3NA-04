using System;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.OperatingStaffAggregate.DTO
{
    public class UpdateOperatingStaffDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string OperationalWindow { get; set; }
    }
}
