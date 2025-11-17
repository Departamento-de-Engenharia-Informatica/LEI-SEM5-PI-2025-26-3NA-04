using System;

namespace APDL.API.Domain.DockAggregate.DTO
{
    public class MaintenanceScheduleDto
    {
        public DateTime ScheduledDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
