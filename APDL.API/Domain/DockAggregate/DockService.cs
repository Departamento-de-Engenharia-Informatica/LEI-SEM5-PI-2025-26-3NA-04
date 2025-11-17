using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.DTO;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.DockAggregate
{
    public class DockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDockRepository _repo;

        public DockService(IUnitOfWork unitOfWork, IDockRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<DockDto>> GetAllAsync()
        {
            var docks = await _repo.GetAllAsync();

            return docks
                .Select(dock => new DockDto
                {
                    Id = dock.Id.AsGuid(),
                    DockName = dock.DockName,
                    DockLength = dock.DockLength,
                    DockDraft = dock.DockDraft,
                    StsCranes = dock
                        .STSCranes.Select(crane => new StsCraneDto
                        {
                            Id = crane.Id.AsGuid(),
                            CraneName = crane.CraneName,
                            DockId = crane.DockId.AsGuid(),
                            OperationalWindow = crane.OperationalWindow.ToString(),
                            CapacityContainersPerHour = crane.CapacityContainersPerHour,
                            Status = crane.Status.ToString(),
                            RequiredOperators = crane.RequiredOperators,
                            RequiredQualification = crane.RequiredQualification.ToString(),
                            SetupTime = crane.SetupTime,
                            UpcomingMaintenances = crane
                                .UpcomingMaintenances.Schedules.Select(
                                    m => new MaintenanceScheduleDto
                                    {
                                        ScheduledDate = m.ScheduledDate,
                                        EstimatedEndDate = m.EstimatedEndDate,
                                        Description = m.Description,
                                        Type = m.Type.ToString(),
                                    }
                                )
                                .ToList(),
                        })
                        .ToList(),
                    UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
                })
                .ToList();
        }

        public async Task<DockDto> GetByIdAsync(DockId id)
        {
            var dock = await _repo.GetByIdAsync(id);
            if (dock == null)
                return null;

            return new DockDto
            {
                Id = dock.Id.AsGuid(),
                DockName = dock.DockName,
                DockLength = dock.DockLength,
                DockDraft = dock.DockDraft,
                StsCranes = dock
                    .STSCranes.Select(crane => new StsCraneDto
                    {
                        Id = crane.Id.AsGuid(),
                        CraneName = crane.CraneName,
                        DockId = crane.DockId.AsGuid(),
                        OperationalWindow = crane.OperationalWindow.ToString(),
                        CapacityContainersPerHour = crane.CapacityContainersPerHour,
                        Status = crane.Status.ToString(),
                        RequiredOperators = crane.RequiredOperators,
                        RequiredQualification = crane.RequiredQualification.ToString(),
                        SetupTime = crane.SetupTime,
                        UpcomingMaintenances = crane
                            .UpcomingMaintenances.Schedules.Select(m => new MaintenanceScheduleDto
                            {
                                ScheduledDate = m.ScheduledDate,
                                EstimatedEndDate = m.EstimatedEndDate,
                                Description = m.Description,
                                Type = m.Type.ToString(),
                            })
                            .ToList(),
                    })
                    .ToList(),
                UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
            };
        }

        public async Task<DockDto> GetByNameAsync(string dockName)
        {
            var dock = await _repo.GetByNameAsync(dockName);
            if (dock == null)
                return null;

            return new DockDto
            {
                Id = dock.Id.AsGuid(),
                DockName = dock.DockName,
                DockLength = dock.DockLength,
                DockDraft = dock.DockDraft,
                StsCranes = dock
                    .STSCranes.Select(crane => new StsCraneDto
                    {
                        Id = crane.Id.AsGuid(),
                        CraneName = crane.CraneName,
                        DockId = crane.DockId.AsGuid(),
                        OperationalWindow = crane.OperationalWindow.ToString(),
                        CapacityContainersPerHour = crane.CapacityContainersPerHour,
                        Status = crane.Status.ToString(),
                        RequiredOperators = crane.RequiredOperators,
                        RequiredQualification = crane.RequiredQualification.ToString(),
                        SetupTime = crane.SetupTime,
                        UpcomingMaintenances = crane
                            .UpcomingMaintenances.Schedules.Select(m => new MaintenanceScheduleDto
                            {
                                ScheduledDate = m.ScheduledDate,
                                EstimatedEndDate = m.EstimatedEndDate,
                                Description = m.Description,
                                Type = m.Type.ToString(),
                            })
                            .ToList(),
                    })
                    .ToList(),
                UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
            };
        }

        public async Task<List<DockDto>> GetDocksCapableOfVesselAsync(
            int vesselLength,
            int vesselDraft
        )
        {
            var docks = await _repo.GetDocksCapableOfVesselAsync(vesselLength, vesselDraft);

            return docks
                .Select(dock => new DockDto
                {
                    Id = dock.Id.AsGuid(),
                    DockName = dock.DockName,
                    DockLength = dock.DockLength,
                    DockDraft = dock.DockDraft,
                    StsCranes = dock
                        .STSCranes.Select(crane => new StsCraneDto
                        {
                            Id = crane.Id.AsGuid(),
                            CraneName = crane.CraneName,
                            DockId = crane.DockId.AsGuid(),
                            OperationalWindow = crane.OperationalWindow.ToString(),
                            CapacityContainersPerHour = crane.CapacityContainersPerHour,
                            Status = crane.Status.ToString(),
                            RequiredOperators = crane.RequiredOperators,
                            RequiredQualification = crane.RequiredQualification.ToString(),
                            SetupTime = crane.SetupTime,
                            UpcomingMaintenances = crane
                                .UpcomingMaintenances.Schedules.Select(
                                    m => new MaintenanceScheduleDto
                                    {
                                        ScheduledDate = m.ScheduledDate,
                                        EstimatedEndDate = m.EstimatedEndDate,
                                        Description = m.Description,
                                        Type = m.Type.ToString(),
                                    }
                                )
                                .ToList(),
                        })
                        .ToList(),
                    UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
                })
                .ToList();
        }

        public async Task<DockDto> AddAsync(CreateDockDto dto)
        {
            var exists = await _repo.DockNameExistsAsync(dto.DockName);
            if (exists)
                throw new BusinessRuleValidationException(
                    nameof(dto.DockName),
                    $"Dock with name '{dto.DockName}' already exists."
                );

            var dock = new Dock(dto.DockName, dto.DockLength, dto.DockDraft);

            await _repo.AddAsync(dock);
            await _unitOfWork.CommitAsync();

            return new DockDto
            {
                Id = dock.Id.AsGuid(),
                DockName = dock.DockName,
                DockLength = dock.DockLength,
                DockDraft = dock.DockDraft,
                StsCranes = new List<StsCraneDto>(),
                UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
            };
        }

        public async Task<DockDto> UpdateAsync(UpdateDockDto dto)
        {
            var dock = await _repo.GetByIdAsync(new DockId(dto.Id));
            if (dock == null)
                return null;

            await _unitOfWork.CommitAsync();

            return new DockDto
            {
                Id = dock.Id.AsGuid(),
                DockName = dock.DockName,
                DockLength = dock.DockLength,
                DockDraft = dock.DockDraft,
                StsCranes = dock
                    .STSCranes.Select(crane => new StsCraneDto
                    {
                        Id = crane.Id.AsGuid(),
                        CraneName = crane.CraneName,
                        DockId = crane.DockId.AsGuid(),
                        OperationalWindow = crane.OperationalWindow.ToString(),
                        CapacityContainersPerHour = crane.CapacityContainersPerHour,
                        Status = crane.Status.ToString(),
                        RequiredOperators = crane.RequiredOperators,
                        RequiredQualification = crane.RequiredQualification.ToString(),
                        SetupTime = crane.SetupTime,
                        UpcomingMaintenances = crane
                            .UpcomingMaintenances.Schedules.Select(m => new MaintenanceScheduleDto
                            {
                                ScheduledDate = m.ScheduledDate,
                                EstimatedEndDate = m.EstimatedEndDate,
                                Description = m.Description,
                                Type = m.Type.ToString(),
                            })
                            .ToList(),
                    })
                    .ToList(),
                UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
            };
        }

        public async Task<DockDto> DeleteAsync(DockId id)
        {
            var dock = await _repo.GetByIdAsync(id);
            if (dock == null)
                return null;

            _repo.Remove(dock);
            await _unitOfWork.CommitAsync();

            return new DockDto
            {
                Id = dock.Id.AsGuid(),
                DockName = dock.DockName,
                DockLength = dock.DockLength,
                DockDraft = dock.DockDraft,
                StsCranes = dock
                    .STSCranes.Select(crane => new StsCraneDto
                    {
                        Id = crane.Id.AsGuid(),
                        CraneName = crane.CraneName,
                        DockId = crane.DockId.AsGuid(),
                        OperationalWindow = crane.OperationalWindow.ToString(),
                        CapacityContainersPerHour = crane.CapacityContainersPerHour,
                        Status = crane.Status.ToString(),
                        RequiredOperators = crane.RequiredOperators,
                        RequiredQualification = crane.RequiredQualification.ToString(),
                        SetupTime = crane.SetupTime,
                        UpcomingMaintenances = crane
                            .UpcomingMaintenances.Schedules.Select(m => new MaintenanceScheduleDto
                            {
                                ScheduledDate = m.ScheduledDate,
                                EstimatedEndDate = m.EstimatedEndDate,
                                Description = m.Description,
                                Type = m.Type.ToString(),
                            })
                            .ToList(),
                    })
                    .ToList(),
                UpcomingMaintenances = new List<MaintenanceScheduleDto>(),
            };
        }
    }
}
