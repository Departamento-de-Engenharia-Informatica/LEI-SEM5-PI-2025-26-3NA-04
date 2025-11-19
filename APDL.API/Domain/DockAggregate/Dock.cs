using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared;

public class Dock : Entity<DockId>, IAggregateRoot
{
    private List<StsCrane> _stsCranes;
    public IReadOnlyList<StsCrane> STSCranes => _stsCranes?.AsReadOnly();

    public int NumberOfSTSCranes => _stsCranes?.Count ?? 0;

    public string DockName { get; private set; }
    public int DockLength { get; private set; }
    public int DockDraft { get; private set; }

    public UpcomingMaintenances UpcomingMaintenances { get; private set; }

    protected Dock()
    {
        _stsCranes = new List<StsCrane>();
        UpcomingMaintenances = new UpcomingMaintenances();
    }

    public Dock(string name, int length, int draft)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleValidationException("Dock name cannot be empty.", nameof(name));

        if (length <= 0)
            throw new BusinessRuleValidationException(
                "Dock length must be greater than zero.",
                nameof(length)
            );

        if (draft <= 0)
            throw new BusinessRuleValidationException(
                "Dock draft must be greater than zero.",
                nameof(draft)
            );

        this.DockName = name;
        this.DockLength = length;
        this.DockDraft = draft;

        this._stsCranes = new List<StsCrane>();
        this.UpcomingMaintenances = new UpcomingMaintenances();
    }

    public void AddSTSCrane(StsCrane crane)
    {
        if (crane == null)
            throw new BusinessRuleValidationException("Crane cannot be null.", nameof(crane));

        if (crane.DockId != this.Id)
            throw new BusinessRuleValidationException(
                "Crane must belong to this dock.",
                nameof(crane)
            );

        _stsCranes.Add(crane);
    }

    public void RemoveSTSCrane(StsCraneId craneId)
    {
        var crane = _stsCranes.FirstOrDefault(c => c.Id == craneId);

        if (crane == null)
            throw new BusinessRuleValidationException("Crane not found.", nameof(craneId));

        _stsCranes.Remove(crane);
    }

    public bool CanAccommodateVessel(int vesselLength, int vesselDraft)
    {
        return vesselLength <= this.DockLength && vesselDraft <= this.DockDraft;
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
