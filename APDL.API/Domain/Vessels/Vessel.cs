
using System;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Vessels.ValueObjects;

namespace APDL.API.Domain.Vessels;

public class Vessel : Entity<VesselId>, IAggregateRoot
{
    public Name Name { get; private set; }
    public ImoNumber ImoNumber { get; private set; }
    public VesselTypeId VesselTypeId { get; private set; }
    public VesselType VesselType { get; private set; }
    public Operator Operator { get; private set; }

    protected Vessel() { }

    public Vessel(string name, string imoNumber, VesselTypeId vesselTypeId, VesselType vesselType, string operatorName)
    {
        Id = new VesselId(Guid.NewGuid());
        Name = new Name(name);
        ImoNumber = new ImoNumber(imoNumber);
        VesselTypeId = vesselTypeId;
        VesselType = vesselType;
        Operator = new Operator(operatorName);
    }
}
