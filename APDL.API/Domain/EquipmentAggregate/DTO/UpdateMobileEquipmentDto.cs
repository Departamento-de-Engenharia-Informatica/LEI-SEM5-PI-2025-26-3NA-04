using System;
using System.ComponentModel.DataAnnotations;

namespace APDL.API.Domain.MobileEquipmentAggregate.DTO
{
    public class UpdateMobileEquipmentDto
    {
        public Guid Id { get; set; }

        public string EquipmentName { get; set; }

        public string OperationalWindow { get; set; }

        public int? ContainersPerTrip { get; set; }
        public double? AverageSpeedPerHour { get; set; }

        public int? ContainersPerHour { get; set; }

        public TimeSpan SetupTime { get; set; }
    }
}
