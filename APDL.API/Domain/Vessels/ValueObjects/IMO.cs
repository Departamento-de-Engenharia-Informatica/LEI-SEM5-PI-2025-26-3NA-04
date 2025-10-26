using APDL.API.Domain.Shared;
using System.Text.RegularExpressions;
using System.Linq;

namespace APDL.API.Domain.Vessels.ValueObjects;

public class ImoNumber
{
    public string Value { get; }

    public ImoNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new BusinessRuleValidationException("IMO number cannot be null or empty.");

        if (!IsValidImoNumber(value))
            throw new BusinessRuleValidationException("Invalid IMO number format or check digit.");

        Value = value;
    }

    private bool IsValidImoNumber(string imo)
    {
        if (imo.Length != 7 || !imo.All(char.IsDigit)) return false;

        int sum = 0;
        for (int i = 0; i < 6; i++)
        {
            sum += (imo[i] - '0') * (7 - i);
        }

        int checkDigit = sum % 10;
        return checkDigit == (imo[6] - '0');
    }
    public override string ToString() => Value;
}
