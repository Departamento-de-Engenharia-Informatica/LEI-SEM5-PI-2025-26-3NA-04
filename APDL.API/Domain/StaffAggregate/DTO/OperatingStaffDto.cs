using System;
using System.Collections.Generic;

namespace APDL.API.Domain.OperatingStaffAggregate.DTO
{
    public class OperatingStaffDto
    {
        public Guid Id { get; set; }
        public string MecanographicNumber { get; set; }
        public string ShortName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string OperationalWindow { get; set; }
        public string Status { get; set; }
        public List<string> Qualifications { get; set; }
    }
}
