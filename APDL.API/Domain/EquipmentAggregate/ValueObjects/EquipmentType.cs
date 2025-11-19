using System;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.MobileEquipmentAggregate.ValueObjects
{
    public class EquipmentType : IValueObject
    {
        public EquipmentTypeEnum Value { get; }

        private EquipmentType(EquipmentTypeEnum value)
        {
            Value = value;
        }

        private EquipmentType() {}

        public static EquipmentType Truck => new EquipmentType(EquipmentTypeEnum.TRUCK);
        public static EquipmentType YardGantryCrane =>
            new EquipmentType(EquipmentTypeEnum.YARD_GANTRY_CRANE);
        public static EquipmentType TerminalTractor =>
            new EquipmentType(EquipmentTypeEnum.TERMINAL_TRACTOR);

        public static EquipmentType FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(
                    "Equipment type cannot be empty.",
                    nameof(value)
                );

            if (!Enum.TryParse<EquipmentTypeEnum>(value.ToUpper().Replace(" ", "_"), out var type))
                throw new BusinessRuleValidationException(
                    $"Invalid Equipment Type: '{value}'. Valid values are: TRUCK, YARD_GANTRY_CRANE, TERMINAL_TRACTOR.",
                    nameof(value)
                );

            return new EquipmentType(type);
        }

        public bool IsTruck() =>
            Value == EquipmentTypeEnum.TRUCK || Value == EquipmentTypeEnum.TERMINAL_TRACTOR;

        public bool IsYardGantryCrane() => Value == EquipmentTypeEnum.YARD_GANTRY_CRANE;

        public override bool Equals(object obj)
        {
            return obj is EquipmentType other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public enum EquipmentTypeEnum
    {
        TRUCK,
        YARD_GANTRY_CRANE,
        TERMINAL_TRACTOR,
    }
}
