using System;
using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Shared.ValueObjects
{
    public class OperationalWindow : IValueObject
    {
        public DayOfWeek StartDay { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public DayOfWeek EndDay { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public bool Is24x7 { get; private set; }

        private OperationalWindow() { }

        private OperationalWindow(
            DayOfWeek startDay,
            TimeSpan startTime,
            DayOfWeek endDay,
            TimeSpan endTime,
            bool is24x7 = false
        )
        {
            if (startTime < TimeSpan.Zero || startTime >= TimeSpan.FromDays(1))
                throw new BusinessRuleValidationException(
                    "Start time must be between 00:00 and 23:59.",
                    nameof(startTime)
                );

            if (endTime < TimeSpan.Zero || endTime >= TimeSpan.FromDays(1))
                throw new BusinessRuleValidationException(
                    "End time must be between 00:00 and 23:59.",
                    nameof(endTime)
                );

            StartDay = startDay;
            StartTime = startTime;
            EndDay = endDay;
            EndTime = endTime;
            Is24x7 = is24x7;
        }


        public static OperationalWindow Create24x7()
        {
            return new OperationalWindow(
                DayOfWeek.Monday,
                TimeSpan.Zero,
                DayOfWeek.Sunday,
                new TimeSpan(23, 59, 59),
                is24x7: true
            );
        }

        public static OperationalWindow Create(
            DayOfWeek startDay,
            TimeSpan startTime,
            DayOfWeek endDay,
            TimeSpan endTime
        )
        {
            return new OperationalWindow(startDay, startTime, endDay, endTime);
        }

        public static OperationalWindow CreateWeekdays(TimeSpan startTime, TimeSpan endTime)
        {
            return new OperationalWindow(DayOfWeek.Monday, startTime, DayOfWeek.Friday, endTime);
        }

        public static OperationalWindow FromString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException(
                    "Operational window cannot be empty.",
                    nameof(value)
                );

            if (
                value.Trim().Equals("24/7", StringComparison.OrdinalIgnoreCase)
                || value.Trim().Equals("24x7", StringComparison.OrdinalIgnoreCase)
            )
            {
                return Create24x7();
            }

            try
            {
                var parts = value.Split('-');
                if (parts.Length != 2)
                    throw new FormatException(
                        "Invalid format. Expected: 'DayOfWeek HH:mm - DayOfWeek HH:mm'"
                    );

                var startParts = parts[0].Trim().Split(' ');
                var endParts = parts[1].Trim().Split(' ');

                if (!Enum.TryParse<DayOfWeek>(startParts[0], true, out var startDay))
                    throw new FormatException($"Invalid start day: {startParts[0]}");

                if (!Enum.TryParse<DayOfWeek>(endParts[0], true, out var endDay))
                    throw new FormatException($"Invalid end day: {endParts[0]}");

                if (!TimeSpan.TryParse(startParts[1], out var startTime))
                    throw new FormatException($"Invalid start time: {startParts[1]}");

                if (!TimeSpan.TryParse(endParts[1], out var endTime))
                    throw new FormatException($"Invalid end time: {endParts[1]}");

                return new OperationalWindow(startDay, startTime, endDay, endTime);
            }
            catch (Exception ex)
            {
                throw new BusinessRuleValidationException(
                    $"Invalid operational window format: '{value}'. Expected format: 'Monday 08:00 - Friday 17:00' or '24/7'",
                    nameof(value)
                );
            }
        }

        public bool IsAvailableAt(DateTime dateTime)
        {
            if (Is24x7)
                return true;

            var day = dateTime.DayOfWeek;
            var time = dateTime.TimeOfDay;

            if (StartDay <= EndDay)
            {
                if (day < StartDay || day > EndDay)
                    return false;

                if (day == StartDay && time < StartTime)
                    return false;

                if (day == EndDay && time > EndTime)
                    return false;

                return true;
            }
            else
            {
                if (day >= StartDay)
                {
                    if (day == StartDay && time < StartTime)
                        return false;
                    return true;
                }
                else if (day <= EndDay)
                {
                    if (day == EndDay && time > EndTime)
                        return false;
                    return true;
                }

                return false;
            }
        }

        public bool IsAvailableForDuration(DateTime start, TimeSpan duration)
        {
            if (Is24x7)
                return true;

            var end = start.Add(duration);
            var current = start;

            while (current <= end)
            {
                if (!IsAvailableAt(current))
                    return false;

                current = current.AddHours(1);
            }

            return IsAvailableAt(end);
        }

        public bool OverlapsWith(OperationalWindow other)
        {
            if (Is24x7 || other.Is24x7)
                return true;

            bool daysOverlap = !(EndDay < other.StartDay || StartDay > other.EndDay);

            if (!daysOverlap)
                return false;

            return !(EndTime < other.StartTime || StartTime > other.EndTime);
        }

        public double GetTotalHoursPerWeek()
        {
            if (Is24x7)
                return 168.0;

            int dayDifference;
            if (EndDay >= StartDay)
            {
                dayDifference = (int)EndDay - (int)StartDay;
            }
            else
            {
                dayDifference = 7 - (int)StartDay + (int)EndDay;
            }

            double totalHours = (dayDifference * 24.0) + (EndTime - StartTime).TotalHours;
            return totalHours;
        }

        public override bool Equals(object obj)
        {
            return obj is OperationalWindow other
                && StartDay == other.StartDay
                && StartTime == other.StartTime
                && EndDay == other.EndDay
                && EndTime == other.EndTime
                && Is24x7 == other.Is24x7;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartDay, StartTime, EndDay, EndTime, Is24x7);
        }

        public override string ToString()
        {
            if (Is24x7)
                return "24/7";

            return $"{StartDay} {StartTime:hh\\:mm} - {EndDay} {EndTime:hh\\:mm}";
        }
    }
}
