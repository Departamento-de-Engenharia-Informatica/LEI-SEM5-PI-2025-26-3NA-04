using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage.ValueObjects;


public class TEUCapacity
{
    public int Value { get; }

    public TEUCapacity(int value)
    {
        if (value <= 0)
            throw new BusinessRuleValidationException("Capacity must be greater than zero.");
        Value = value;
    }
}
