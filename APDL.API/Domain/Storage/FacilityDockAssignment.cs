using System;
using System.Collections.Generic;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Dock;
using APDL.API.Domain.Storage.ValueObjects;

namespace APDL.API.Domain.Storage;


public class FacilityDockAssignment
{
    public string DebugColumn1 { get; private set; } = "debug";
    public FacilityId FacilityId { get; private set; }
    public DockId DockId { get; private set; }
    public Distance DistanceToDock { get; private set; }

    protected FacilityDockAssignment() { }
    public FacilityDockAssignment(FacilityId facilityId, DockId dockId, Distance distance)
    {
        FacilityId = facilityId;
        DockId = dockId;
        DistanceToDock = distance;
    }
}

