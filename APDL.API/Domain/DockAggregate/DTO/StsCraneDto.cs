using System;
using System.Collections.Generic;

namespace APDL.API.Domain.DockAggregate.DTO
{
    public class StsCraneDto
    {
        public Guid Id { get; set; }
        public string CraneName { get; set; }
        public Guid DockId { get; set; }
        public string OperationalWindow { get; set; }
        public int CapacityContainersPerHour { get; set; }
        public string Status { get; set; }

        public int RequiredOperators { get; set; }
        public string RequiredQualification { get; set; }
        public TimeSpan SetupTime { get; set; }
        public List<MaintenanceScheduleDto> UpcomingMaintenances { get; set; }
    }
}
