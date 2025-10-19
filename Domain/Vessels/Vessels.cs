using System;
using System.ComponentModel.DataAnnotations;
using DDDSample1.Domain.VesselTypes;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Domain.Vessels;

public class Vessel : Entity<VesselId>, IAggregateRoot
{
    public string Name { get; set; }

    public string ImoNumber { get; set; }

    public Guid VesselTypeId { get; set; }
    public VesselType VesselType { get; set; }

    public string Operator { get; set; }

    public Vessel(string name, string imoNumber, Guid VesselTypeId, VesselType vesselType, string operatorName)
    {
        Id = new VesselId(Guid.NewGuid());
        this.Name = name;
        this.ImoNumber = imoNumber;
        this.VesselTypeId = VesselTypeId;
        this.VesselType = vesselType;
        this.Operator = operatorName;
    }

    protected Vessel() { }
}