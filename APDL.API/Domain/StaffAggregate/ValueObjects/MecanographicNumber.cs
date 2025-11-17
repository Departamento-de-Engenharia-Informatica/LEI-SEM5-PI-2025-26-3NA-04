using APDL.API.Domain.Shared;

namespace APDL.API.Domain.OperatingStaffAggregate.ValueObjects
{
    public class MecanographicNumber : IValueObject
    {
        public string Value { get; }

        public MecanographicNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(
                    "Mecanographic number cannot be empty.",
                    nameof(value)
                );

            if (value.Length < 3 || value.Length > 20)
                throw new BusinessRuleValidationException(
                    "Mecanographic number must be between 3 and 20 characters.",
                    nameof(value)
                );

            Value = value.Trim().ToUpper();
        }

        public override bool Equals(object obj)
        {
            return obj is MecanographicNumber other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
