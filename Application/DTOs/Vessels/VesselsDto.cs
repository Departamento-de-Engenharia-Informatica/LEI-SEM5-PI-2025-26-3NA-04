
using System;

namespace DDDSample1.Application.DTOs
{
    public class VesselDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImoNumber { get; set; }
        public Guid VesselTypeId { get; set; }
        public string Operator { get; set; }

        public VesselDto(Guid id, string name, string imoNumber, Guid vesselTypeId, string operatorName)
        {
            this.Id = id;
            Name = name;
            ImoNumber = imoNumber;
            VesselTypeId = vesselTypeId;
            Operator = operatorName;
        }
    }
}
