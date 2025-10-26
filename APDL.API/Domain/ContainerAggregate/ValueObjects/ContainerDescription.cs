using APDL.API.Domain.Shared;


namespace APDL.API.Domain.ContainerAggregate.ValueObjects
{
    public class ContainerDescription : IValueObject
    {
        public string Value { get; }

        public ContainerDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Description cannot be empty.", nameof(value));

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is ContainerDescription other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}