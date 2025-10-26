using System;
using APDL.API.Domain.Dock;

namespace APDL.API.Domain.Dock;

public class DockAssignmentDto
{
    public Guid DockId { get; set; }
    public double DistanceMeters { get; set; }
}
