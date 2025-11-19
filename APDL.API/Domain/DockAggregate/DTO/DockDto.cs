using System;
using System.Collections.Generic;

namespace APDL.API.Domain.DockAggregate.DTO
{
    public class DockDto
    {
        public Guid Id { get; set; }
        public string DockName { get; set; }
        public int DockLength { get; set; }
        public int DockDraft { get; set; }

        public List<StsCraneDto> StsCranes { get; set; }
        public List<MaintenanceScheduleDto> UpcomingMaintenances { get; set; }
    }
}
