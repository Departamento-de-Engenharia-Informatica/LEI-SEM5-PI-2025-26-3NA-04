
namespace APDL.API.Domain.Storage
{
    public class CreatingFacilityDto
    {
        public string Type { get; set; } // "Yard" or "Warehouse"
        public string Location { get; set; }
        public int MaxCapacityTEU { get; set; }
    }
}
