using System;

namespace DDDSample1.Application.DTOs
{
    public class FacilityDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } // "Yard" ou "Warehouse"
        public string Location { get; set; }
        public int MaxCapacityTEU { get; set; }
        public int CurrentOccupancyTEU { get; set; }

        public FacilityDto(Guid id, string type, string Location, int maxCapacityTEU, int currentOccupancyTeu)
        {
            this.Id = id;
            this.Location = Location;
            this.MaxCapacityTEU = maxCapacityTEU;
            this.Type = type;
            this.CurrentOccupancyTEU = currentOccupancyTeu;
        }
    }
}
