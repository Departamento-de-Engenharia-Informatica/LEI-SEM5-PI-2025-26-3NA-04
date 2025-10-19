using System;

namespace DDDSample1.Application.DTOs
{
    public class CreatingVesselDto
    {
        public string Name { get; set; }
        public string ImoNumber { get; set; }
        public Guid VesselTypeId { get; set; }
        public string Operator { get; set; }
    }
}
