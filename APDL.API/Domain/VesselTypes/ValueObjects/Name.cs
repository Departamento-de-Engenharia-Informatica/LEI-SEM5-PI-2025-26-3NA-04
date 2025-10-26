using APDL.API.Domain.Shared;
using System;
using System.Collections.Generic;

namespace APDL.API.Domain.VesselTypes.ValueObjects;

public class Name
{
    public string Value { get; }

    private static readonly HashSet<string> AllowedNames = new()
    {
        "Feeder", "Panamax", "PostPanamax", "ULCV"
    };

    public Name(string value)
    {
        if (!AllowedNames.Contains(value))
            throw new BusinessRuleValidationException($"Invalid vessel type name: {value}. Must be one of {string.Join(", ", AllowedNames)}");

        Value = value;
    }

    public override string ToString() => Value;
}
