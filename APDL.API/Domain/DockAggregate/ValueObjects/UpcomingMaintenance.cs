using System;
using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.DockAggregate.ValueObjects
{
    public class UpcomingMaintenances : IValueObject
    {
        private readonly List<MaintenanceSchedule> _schedules;

        public IReadOnlyList<MaintenanceSchedule> Schedules => _schedules.AsReadOnly();

        public UpcomingMaintenances()
        {
            _schedules = new List<MaintenanceSchedule>();
        }

        public UpcomingMaintenances(IEnumerable<MaintenanceSchedule> schedules)
        {
            if (schedules == null)
                throw new BusinessRuleValidationException(
                    "Schedules cannot be null.",
                    nameof(schedules)
                );

            _schedules = schedules.OrderBy(s => s.ScheduledDate).ToList();
            ValidateNoOverlaps();
        }

        public UpcomingMaintenances AddMaintenance(MaintenanceSchedule maintenance)
        {
            if (maintenance == null)
                throw new BusinessRuleValidationException(
                    "Maintenance schedule cannot be null.",
                    nameof(maintenance)
                );

            var newSchedules = new List<MaintenanceSchedule>(_schedules) { maintenance };
            return new UpcomingMaintenances(newSchedules);
        }

        public UpcomingMaintenances RemoveMaintenance(MaintenanceSchedule maintenance)
        {
            var newSchedules = _schedules.Where(s => !s.Equals(maintenance)).ToList();
            return new UpcomingMaintenances(newSchedules);
        }

        public bool HasMaintenanceOn(DateTime date) => _schedules.Any(s => s.IsScheduledFor(date));

        public bool HasUpcomingMaintenance(DateTime currentDate) =>
            _schedules.Any(s => s.IsUpcoming(currentDate));

        public bool IsInMaintenance(DateTime currentDate) =>
            _schedules.Any(s => s.IsInProgress(currentDate));

        public MaintenanceSchedule GetNextMaintenance(DateTime currentDate) =>
            _schedules
                .Where(s => s.IsUpcoming(currentDate))
                .OrderBy(s => s.ScheduledDate)
                .FirstOrDefault();

        public MaintenanceSchedule GetCurrentMaintenance(DateTime currentDate) =>
            _schedules.FirstOrDefault(s => s.IsInProgress(currentDate));

        public IEnumerable<MaintenanceSchedule> GetMaintenancesInRange(
            DateTime start,
            DateTime end
        ) => _schedules.Where(s => s.ScheduledDate <= end && s.EstimatedEndDate >= start);

        private void ValidateNoOverlaps()
        {
            for (int i = 0; i < _schedules.Count; i++)
            {
                for (int j = i + 1; j < _schedules.Count; j++)
                {
                    if (_schedules[i].OverlapsWith(_schedules[j]))
                    {
                        throw new BusinessRuleValidationException(
                            $"Maintenance schedules cannot overlap: {_schedules[i]} and {_schedules[j]}",
                            nameof(_schedules)
                        );
                    }
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is UpcomingMaintenances other && _schedules.SequenceEqual(other._schedules);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var schedule in _schedules)
                hash.Add(schedule);
            return hash.ToHashCode();
        }

        public override string ToString() => $"{_schedules.Count} scheduled maintenance(s)";
    }
}
