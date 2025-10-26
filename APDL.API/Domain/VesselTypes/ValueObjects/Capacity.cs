using APDL.API.Domain.Shared;

namespace APDL.API.Domain.VesselTypes.ValueObjects;


public class Capacity
{
    public int Value { get; }

    public Capacity(int value)
    {
        if (value <= 0)
            throw new BusinessRuleValidationException("Capacity must be greater than zero.");
        Value = value;
    }
}
