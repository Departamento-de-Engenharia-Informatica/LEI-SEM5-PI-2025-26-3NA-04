using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.EquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate.DTO;
using APDL.API.Domain.MobileEquipmentAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;

namespace APDL.API.Domain.MobileEquipmentAggregate
{
    public class MobileEquipmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMobileEquipmentRepository _repo;

        public MobileEquipmentService(IUnitOfWork unitOfWork, IMobileEquipmentRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<MobileEquipmentDto>> GetAllAsync()
        {
            var equipment = await _repo.GetAllAsync();

            return equipment
                .Select(e => new MobileEquipmentDto
                {
                    Id = e.Id.AsGuid(),
                    EquipmentName = e.EquipmentName,
                    EquipmentType = e.EquipmentType.ToString(),
                    OperationalWindow = e.OperationalWindow.ToString(),
                    Status = e.Status.ToString(),
                    ContainersPerTrip = e.Capacity.ContainersPerTrip,
                    AverageSpeedPerHour = e.Capacity.AverageSpeedPerHour,
                    ContainersPerHour = e.Capacity.ContainersPerHour,
                    RequiredOperators = e.RequiredOperators,
                    RequiredQualification = e.RequiredQualification.ToString(),
                    SetupTime = e.SetupTime,
                })
                .ToList();
        }

        public async Task<MobileEquipmentDto> GetByIdAsync(MobileEquipmentId id)
        {
            var equipment = await _repo.GetByIdAsync(id);
            if (equipment == null)
                return null;

            return new MobileEquipmentDto
            {
                Id = equipment.Id.AsGuid(),
                EquipmentName = equipment.EquipmentName,
                EquipmentType = equipment.EquipmentType.ToString(),
                OperationalWindow = equipment.OperationalWindow.ToString(),
                Status = equipment.Status.ToString(),
                ContainersPerTrip = equipment.Capacity.ContainersPerTrip,
                AverageSpeedPerHour = equipment.Capacity.AverageSpeedPerHour,
                ContainersPerHour = equipment.Capacity.ContainersPerHour,
                RequiredOperators = equipment.RequiredOperators,
                RequiredQualification = equipment.RequiredQualification.ToString(),
                SetupTime = equipment.SetupTime,
            };
        }

        public async Task<List<MobileEquipmentDto>> GetAvailableEquipmentAsync()
        {
            var equipment = await _repo.GetAvailableEquipmentAsync();

            return equipment
                .Select(e => new MobileEquipmentDto
                {
                    Id = e.Id.AsGuid(),
                    EquipmentName = e.EquipmentName,
                    EquipmentType = e.EquipmentType.ToString(),
                    OperationalWindow = e.OperationalWindow.ToString(),
                    Status = e.Status.ToString(),
                    ContainersPerTrip = e.Capacity.ContainersPerTrip,
                    AverageSpeedPerHour = e.Capacity.AverageSpeedPerHour,
                    ContainersPerHour = e.Capacity.ContainersPerHour,
                    RequiredOperators = e.RequiredOperators,
                    RequiredQualification = e.RequiredQualification.ToString(),
                    SetupTime = e.SetupTime,
                })
                .ToList();
        }

        public async Task<List<MobileEquipmentDto>> GetEquipmentByTypeAsync(string equipmentType)
        {
            var type = EquipmentType.FromString(equipmentType);
            var equipment = await _repo.GetEquipmentByTypeAsync(type);

            return equipment
                .Select(e => new MobileEquipmentDto
                {
                    Id = e.Id.AsGuid(),
                    EquipmentName = e.EquipmentName,
                    EquipmentType = e.EquipmentType.ToString(),
                    OperationalWindow = e.OperationalWindow.ToString(),
                    Status = e.Status.ToString(),
                    ContainersPerTrip = e.Capacity.ContainersPerTrip,
                    AverageSpeedPerHour = e.Capacity.AverageSpeedPerHour,
                    ContainersPerHour = e.Capacity.ContainersPerHour,
                    RequiredOperators = e.RequiredOperators,
                    RequiredQualification = e.RequiredQualification.ToString(),
                    SetupTime = e.SetupTime,
                })
                .ToList();
        }

        public async Task<MobileEquipmentDto> AddAsync(CreateMobileEquipmentDto dto)
        {
            var exists = await _repo.EquipmentNameExistsAsync(dto.EquipmentName);
            if (exists)
                throw new BusinessRuleValidationException(
                    nameof(dto.EquipmentName),
                    $"Equipment with name '{dto.EquipmentName}' already exists."
                );

            var equipmentType = EquipmentType.FromString(dto.EquipmentType);
            var operationalWindow = OperationalWindow.FromString(dto.OperationalWindow);
            var requiredQualification = QualificationType.FromString(dto.RequiredQualification);

            EquipmentCapacity capacity;
            if (equipmentType.IsTruck())
            {
                if (!dto.ContainersPerTrip.HasValue || !dto.AverageSpeedPerHour.HasValue)
                    throw new BusinessRuleValidationException(
                        "Capacity",
                        "Trucks require ContainersPerTrip and AverageSpeedPerHour."
                    );

                capacity = EquipmentCapacity.ForTruck(
                    dto.ContainersPerTrip.Value,
                    dto.AverageSpeedPerHour.Value
                );
            }
            else if (equipmentType.IsYardGantryCrane())
            {
                if (!dto.ContainersPerHour.HasValue)
                    throw new BusinessRuleValidationException(
                        "Capacity",
                        "Cranes require ContainersPerHour."
                    );

                capacity = EquipmentCapacity.ForCrane(dto.ContainersPerHour.Value);
            }
            else
            {
                throw new BusinessRuleValidationException(
                    "EquipmentType",
                    "Invalid equipment type."
                );
            }

            var equipment = new MobileEquipment(
                new MobileEquipmentId(Guid.NewGuid()),
                dto.EquipmentName,
                equipmentType,
                operationalWindow,
                capacity,
                dto.SetupTime,
                dto.RequiredOperators,
                requiredQualification
            );

            await _repo.AddAsync(equipment);
            await _unitOfWork.CommitAsync();

            return new MobileEquipmentDto
            {
                Id = equipment.Id.AsGuid(),
                EquipmentName = equipment.EquipmentName,
                EquipmentType = equipment.EquipmentType.ToString(),
                OperationalWindow = equipment.OperationalWindow.ToString(),
                Status = equipment.Status.ToString(),
                ContainersPerTrip = equipment.Capacity.ContainersPerTrip,
                AverageSpeedPerHour = equipment.Capacity.AverageSpeedPerHour,
                ContainersPerHour = equipment.Capacity.ContainersPerHour,
                RequiredOperators = equipment.RequiredOperators,
                RequiredQualification = equipment.RequiredQualification.ToString(),
                SetupTime = equipment.SetupTime,
            };
        }

        public async Task<MobileEquipmentDto> UpdateAsync(UpdateMobileEquipmentDto dto)
        {
            var equipment = await _repo.GetByIdAsync(new MobileEquipmentId(dto.Id));
            if (equipment == null)
                return null;

            var operationalWindow = OperationalWindow.FromString(dto.OperationalWindow);

            EquipmentCapacity capacity;
            if (equipment.EquipmentType.IsTruck())
            {
                if (!dto.ContainersPerTrip.HasValue || !dto.AverageSpeedPerHour.HasValue)
                    throw new BusinessRuleValidationException(
                        "Capacity",
                        "Trucks require ContainersPerTrip and AverageSpeedPerHour."
                    );

                capacity = EquipmentCapacity.ForTruck(
                    dto.ContainersPerTrip.Value,
                    dto.AverageSpeedPerHour.Value
                );
            }
            else if (equipment.EquipmentType.IsYardGantryCrane())
            {
                if (!dto.ContainersPerHour.HasValue)
                    throw new BusinessRuleValidationException(
                        "Capacity",
                        "Cranes require ContainersPerHour."
                    );

                capacity = EquipmentCapacity.ForCrane(dto.ContainersPerHour.Value);
            }
            else
            {
                throw new BusinessRuleValidationException(
                    "EquipmentType",
                    "Invalid equipment type."
                );
            }

            equipment.UpdateEquipmentName(dto.EquipmentName);
            equipment.UpdateOperationalWindow(operationalWindow);
            equipment.UpdateCapacity(capacity);
            equipment.UpdateSetupTime(dto.SetupTime);

            await _unitOfWork.CommitAsync();

            return new MobileEquipmentDto
            {
                Id = equipment.Id.AsGuid(),
                EquipmentName = equipment.EquipmentName,
                EquipmentType = equipment.EquipmentType.ToString(),
                OperationalWindow = equipment.OperationalWindow.ToString(),
                Status = equipment.Status.ToString(),
                ContainersPerTrip = equipment.Capacity.ContainersPerTrip,
                AverageSpeedPerHour = equipment.Capacity.AverageSpeedPerHour,
                ContainersPerHour = equipment.Capacity.ContainersPerHour,
                RequiredOperators = equipment.RequiredOperators,
                RequiredQualification = equipment.RequiredQualification.ToString(),
                SetupTime = equipment.SetupTime,
            };
        }

        public async Task<MobileEquipmentDto> UpdateStatusAsync(MobileEquipmentId id, string status)
        {
            var equipment = await _repo.GetByIdAsync(id);
            if (equipment == null)
                return null;

            var resourceStatus = ResourceStatus.FromString(status);

            if (resourceStatus.IsAvailable())
                equipment.MarkAsAvailable();
            else if (resourceStatus.IsUnavailable())
                equipment.MarkAsUnavailable();
            else if (resourceStatus.IsOutOfService())
                equipment.MarkAsOutOfService();
            else if (resourceStatus.IsUnderMaintenance())
                equipment.MarkAsUnderMaintenance();

            await _unitOfWork.CommitAsync();

            return new MobileEquipmentDto
            {
                Id = equipment.Id.AsGuid(),
                EquipmentName = equipment.EquipmentName,
                EquipmentType = equipment.EquipmentType.ToString(),
                OperationalWindow = equipment.OperationalWindow.ToString(),
                Status = equipment.Status.ToString(),
                ContainersPerTrip = equipment.Capacity.ContainersPerTrip,
                AverageSpeedPerHour = equipment.Capacity.AverageSpeedPerHour,
                ContainersPerHour = equipment.Capacity.ContainersPerHour,
                RequiredOperators = equipment.RequiredOperators,
                RequiredQualification = equipment.RequiredQualification.ToString(),
                SetupTime = equipment.SetupTime,
            };
        }

        public async Task<MobileEquipmentDto> DeleteAsync(MobileEquipmentId id)
        {
            var equipment = await _repo.GetByIdAsync(id);
            if (equipment == null)
                return null;

            _repo.Remove(equipment);
            await _unitOfWork.CommitAsync();

            return new MobileEquipmentDto
            {
                Id = equipment.Id.AsGuid(),
                EquipmentName = equipment.EquipmentName,
                EquipmentType = equipment.EquipmentType.ToString(),
                OperationalWindow = equipment.OperationalWindow.ToString(),
                Status = equipment.Status.ToString(),
                ContainersPerTrip = equipment.Capacity.ContainersPerTrip,
                AverageSpeedPerHour = equipment.Capacity.AverageSpeedPerHour,
                ContainersPerHour = equipment.Capacity.ContainersPerHour,
                RequiredOperators = equipment.RequiredOperators,
                RequiredQualification = equipment.RequiredQualification.ToString(),
                SetupTime = equipment.SetupTime,
            };
        }
    }
}
