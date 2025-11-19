using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.OperatingStaffAggregate.DTO
{
    public class CreateOperatingStaffDto
    {
        public string MecanographicNumber { get; set; }

        public string ShortName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string OperationalWindow { get; set; }
    }
}
