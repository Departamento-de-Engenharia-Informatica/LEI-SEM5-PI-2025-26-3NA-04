
using System;

namespace APDL.API.Domain.Vessels
{
    public class CreatingVesselDto
    {
        public string Name { get; set; } // compatível com VesselName VO
        public string ImoNumber { get; set; } // compatível com ImoNumber VO
        public Guid VesselTypeId { get; set; }
        public string Operator { get; set; } // compatível com Operator VO
    }
}
