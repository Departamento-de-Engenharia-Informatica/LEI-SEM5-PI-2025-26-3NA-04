using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Storage.ValueObjects;

public class Distance
{
    public double Meters { get; }

    public Distance(double meters)
    {
        if (meters < 0)
            throw new BusinessRuleValidationException("Distance cannot be negative.");
        Meters = meters;
    }

    public override string ToString() => $"{Meters} m";
}
