using APDL.API.Domain.Shared;

namespace APDL.API.Domain.VesselTypes.ValueObjects;

public class Dimension
{
    public int Value { get; }

    public Dimension(int value)
    {
        if (value <= 0)
            throw new BusinessRuleValidationException("Dimension must be greater than zero.");
        Value = value;
    }
}
