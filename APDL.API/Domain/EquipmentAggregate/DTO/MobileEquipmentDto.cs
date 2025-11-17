using System;

namespace APDL.API.Domain.MobileEquipmentAggregate.DTO
{
    public class MobileEquipmentDto
    {
        public Guid Id { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentType { get; set; }
        public string OperationalWindow { get; set; }
        public string Status { get; set; }

        public int? ContainersPerTrip { get; set; }
        public double? AverageSpeedPerHour { get; set; }
        public int? ContainersPerHour { get; set; }

        public int RequiredOperators { get; set; }
        public string RequiredQualification { get; set; }
        public TimeSpan SetupTime { get; set; }
    }
}
