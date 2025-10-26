using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Vessels.ValueObjects;
public class Operator
{
    public string Value { get; }

    public Operator(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("Operator cannot be empty.");
        Value = value;
    }
}
