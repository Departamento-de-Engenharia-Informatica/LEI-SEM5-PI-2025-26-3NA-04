using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage.ValueObjects;


public class Location
{
    public string Value { get; }

    public Location(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("Location cannot be empty.");
        Value = value;
    }

    public override string ToString() => Value;
}
