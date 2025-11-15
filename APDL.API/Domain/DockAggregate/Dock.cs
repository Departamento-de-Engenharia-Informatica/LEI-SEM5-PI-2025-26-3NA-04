using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.DockAggregate
{
    public class Dock : Entity<DockId>, IAggregateRoot
    {
        public string DockName { get; private set; }
        public int DockLength { get; private set; }
        public int DockDraft { get; private set; }
        public UpcomingMaintenances UpcomingMaintenances { get; private set; }

        protected Dock() { }

        public Dock(string Name, int Length, int Draft)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new BusinessRuleValidationException(
                    "Dock name cannot be empty.",
                    nameof(Name)
                );

            if (Length <= 0)
                throw new BusinessRuleValidationException(
                    "Dock length must be greater than zero.",
                    nameof(Length)
                );

            if (Draft <= 0)
                throw new BusinessRuleValidationException(
                    "Dock draft must be greater than zero.",
                    nameof(Draft)
                );

            this.DockName = Name;
            this.DockLength = Length;
            this.DockDraft = Draft;
        }

         public void ScheduleMaintenance(MaintenanceSchedule maintenance)
        {
            if (maintenance == null)
                throw new BusinessRuleValidationException(
                    "Maintenance schedule cannot be null.",
                    nameof(maintenance)
                );

            UpcomingMaintenances = UpcomingMaintenances.AddMaintenance(maintenance);
        }
    }
}
