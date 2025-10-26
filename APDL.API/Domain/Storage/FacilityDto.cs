using System;
using APDL.API.Domain.Dock;
using System.Collections.Generic;


namespace APDL.API.Domain.Storage
{
    
    public class FacilityDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public int MaxCapacityTEU { get; set; }
        public int CurrentOccupancyTEU { get; set; }
        public List<DockAssignmentDto> DockAssignments { get; set; }

        public FacilityDto(Guid id, string type, string location, int maxCapacityTEU, int currentOccupancyTEU, List<DockAssignmentDto> dockAssignments)
        {
            Id = id;
            Type = type;
            Location = location;
            MaxCapacityTEU = maxCapacityTEU;
            CurrentOccupancyTEU = currentOccupancyTEU;
            DockAssignments = dockAssignments;
        }
    }

}
