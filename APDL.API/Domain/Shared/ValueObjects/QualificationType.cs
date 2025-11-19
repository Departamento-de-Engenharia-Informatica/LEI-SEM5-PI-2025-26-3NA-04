using System;

namespace APDL.API.Domain.Shared.ValueObjects
{
    public class QualificationType : IValueObject
    {
        public QualificationTypeEnum Value { get; }

        private QualificationType(QualificationTypeEnum value)
        {
            Value = value;
        }

        public static QualificationType STSCraneOperator =>
            new QualificationType(QualificationTypeEnum.STS_CRANE_OPERATOR);

        public static QualificationType YardGantryCraneOperator =>
            new QualificationType(QualificationTypeEnum.YARD_GANTRY_CRANE_OPERATOR);

        public static QualificationType TruckDriver =>
            new QualificationType(QualificationTypeEnum.TRUCK_DRIVER);

        public static QualificationType YardPlanner =>
            new QualificationType(QualificationTypeEnum.YARD_PLANNER);

        public static QualificationType FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(
                    "Qualification type cannot be empty.",
                    nameof(value)
                );

            if (
                !Enum.TryParse<QualificationTypeEnum>(
                    value.ToUpper().Replace(" ", "_"),
                    out var type
                )
            )
                throw new BusinessRuleValidationException(
                    $"Invalid Qualification Type: '{value}'. Valid values are: STS_CRANE_OPERATOR, YARD_GANTRY_CRANE_OPERATOR, TRUCK_DRIVER, YARD_PLANNER.",
                    nameof(value)
                );

            return new QualificationType(type);
        }

        public static QualificationType FromEnum(QualificationTypeEnum type)
        {
            return new QualificationType(type);
        }

        public bool IsSTSCraneOperator() => Value == QualificationTypeEnum.STS_CRANE_OPERATOR;

        public bool IsYardGantryCraneOperator() =>
            Value == QualificationTypeEnum.YARD_GANTRY_CRANE_OPERATOR;

        public bool IsTruckDriver() => Value == QualificationTypeEnum.TRUCK_DRIVER;

        public bool IsYardPlanner() => Value == QualificationTypeEnum.YARD_PLANNER;

        public override bool Equals(object obj)
        {
            return obj is QualificationType other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();
    }

    public enum QualificationTypeEnum
    {
        STS_CRANE_OPERATOR,
        YARD_GANTRY_CRANE_OPERATOR,
        TRUCK_DRIVER,
        YARD_PLANNER,
    }
}
