using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Vessels.ValueObjects;


public class Name
{
    public string Value { get; }

    public Name(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("Vessel name cannot be empty.");
        Value = value;
    }
}
