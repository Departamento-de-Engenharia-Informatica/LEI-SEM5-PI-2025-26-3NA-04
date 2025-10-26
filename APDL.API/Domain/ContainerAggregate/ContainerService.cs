using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.DTO;
using APDL.API.Domain.ContainerAggregate.Repos;
using APDL.API.Domain.ContainerAggregate.ValueObjects;

namespace APDL.API.Domain.ContainerAggregate
{
    public class ContainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IContainerRepository _repo;

        public ContainerService(IUnitOfWork unitOfWork, IContainerRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<ContainerDto>> GetAllAsync()
        {
            var containers = await _repo.GetAllAsync();

            return containers.Select(container => new ContainerDto
            {
                Id = container.Id.AsGuid(),
                ContainerNumber = container.ContainerNumber.Value,
                CargoType = container.CargoType.Value,
                Description = container.Description.Value,
                SpecialRequirements = container.SpecialRequirements.Value,
                Bay = container.Position.Bay,
                Row = container.Position.Row,
                Tier = container.Position.Tier
            }).ToList();
        }

        public async Task<ContainerDto> GetByIdAsync(ContainerId id)
        {
            var container = await _repo.GetByIdAsync(id);
            if (container == null) return null;

            return new ContainerDto
            {
                Id = container.Id.AsGuid(),
                ContainerNumber = container.ContainerNumber.Value,
                CargoType = container.CargoType.Value,
                Description = container.Description.Value,
                SpecialRequirements = container.SpecialRequirements.Value,
                Bay = container.Position.Bay,
                Row = container.Position.Row,
                Tier = container.Position.Tier
            };
        }

        public async Task<ContainerDto> GetByContainerNumberAsync(string containerNumber)
        {
            var container = await _repo.GetByContainerNumberAsync(containerNumber);
            if (container == null) return null;

            return new ContainerDto
            {
                Id = container.Id.AsGuid(),
                ContainerNumber = container.ContainerNumber.Value,
                CargoType = container.CargoType.Value,
                Description = container.Description.Value,
                SpecialRequirements = container.SpecialRequirements.Value,
                Bay = container.Position.Bay,
                Row = container.Position.Row,
                Tier = container.Position.Tier
            };
        }

        public async Task<ContainerDto> AddAsync(CreateContainerDto dto)
        {
            var exists = await _repo.ContainerNumberExistsAsync(dto.ContainerNumber);
            if (exists)
                throw new BusinessRuleValidationException(nameof(dto.ContainerNumber), 
                    $"Container with number '{dto.ContainerNumber}' already exists.");

            var container = Container.Create(
                dto.ContainerNumber,
                dto.CargoType,
                dto.Description,
                dto.SpecialRequirements,
                dto.Bay,
                dto.Row,
                dto.Tier
            );

            await _repo.AddAsync(container);
            await _unitOfWork.CommitAsync();

            return new ContainerDto
            {
                Id = container.Id.AsGuid(),
                ContainerNumber = container.ContainerNumber.Value,
                CargoType = container.CargoType.Value,
                Description = container.Description.Value,
                SpecialRequirements = container.SpecialRequirements.Value,
                Bay = container.Position.Bay,
                Row = container.Position.Row,
                Tier = container.Position.Tier
            };
        }

        public async Task<ContainerDto> UpdateAsync(UpdateContainerDto dto)
        {
            var container = await _repo.GetByIdAsync(new ContainerId(dto.Id));
            if (container == null) return null;

            container.UpdateCargoType(dto.CargoType);
            container.UpdateDescription(dto.Description);
            container.UpdateSpecialRequirements(dto.SpecialRequirements);
            container.UpdatePosition(dto.Bay, dto.Row, dto.Tier);

            await _unitOfWork.CommitAsync();

            return new ContainerDto
            {
                Id = container.Id.AsGuid(),
                ContainerNumber = container.ContainerNumber.Value,
                CargoType = container.CargoType.Value,
                Description = container.Description.Value,
                SpecialRequirements = container.SpecialRequirements.Value,
                Bay = container.Position.Bay,
                Row = container.Position.Row,
                Tier = container.Position.Tier
            };
        }

        public async Task<ContainerDto> DeleteAsync(ContainerId id)
        {
            var container = await _repo.GetByIdAsync(id);
            if (container == null) return null;

            _repo.Remove(container);
            await _unitOfWork.CommitAsync();

            return new ContainerDto
            {
                Id = container.Id.AsGuid(),
                ContainerNumber = container.ContainerNumber.Value,
                CargoType = container.CargoType.Value,
                Description = container.Description.Value,
                SpecialRequirements = container.SpecialRequirements.Value,
                Bay = container.Position.Bay,
                Row = container.Position.Row,
                Tier = container.Position.Tier
            };
        }
    }
}