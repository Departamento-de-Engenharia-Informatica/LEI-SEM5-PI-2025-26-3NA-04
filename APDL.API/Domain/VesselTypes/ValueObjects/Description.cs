using APDL.API.Domain.Shared;

namespace APDL.API.Domain.VesselTypes.ValueObjects;

public class Description
{
    public string Value { get; }

    public Description(string value)
    {
        Value = value;
    }
}
