using APDL.API.Domain.Shared;


namespace APDL.API.Domain.ContainerAggregate.ValueObjects
{
    public class SpecialRequirements : IValueObject
    {
        public string Value { get; }

        public SpecialRequirements(string value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is SpecialRequirements other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}