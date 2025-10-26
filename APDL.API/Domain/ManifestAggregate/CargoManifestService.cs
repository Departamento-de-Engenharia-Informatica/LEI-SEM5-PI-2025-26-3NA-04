using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ManifestAggregate.DTO;
using APDL.API.Domain.ManifestAggregate.Repos;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.ContainerAggregate;
namespace APDL.API.Domain.ManifestAggregate
{
    public class CargoManifestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICargoManifestRepository _repo;
        private readonly IVesselVisitNotificationRepository _vesselVisitRepo;

        public CargoManifestService(
            IUnitOfWork unitOfWork, 
            ICargoManifestRepository repo,
            IVesselVisitNotificationRepository vesselVisitRepo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _vesselVisitRepo = vesselVisitRepo;
        }

        public async Task<List<CargoManifestDto>> GetAllAsync()
        {
            var manifests = await _repo.GetAllAsync();

            return manifests.Select(manifest => new CargoManifestDto
            {
                Id = manifest.Id.AsGuid(),
                IsLoadingManifest = manifest.IsLoadingManifest,
                CargoVolume = manifest.CargoVolume.Value,
                VesselVisitNotificationId = manifest.VesselVisitNotificationId.AsGuid(),
                ContainerIds = manifest.ContainerIds.Select(c => c.AsGuid()).ToList() 
            }).ToList();
        }

        public async Task<CargoManifestDto> GetByIdAsync(CargoManifestId id)
        {
            var manifest = await _repo.GetByIdAsync(id);
            if (manifest == null) return null;

            return new CargoManifestDto
            {
                Id = manifest.Id.AsGuid(),
                IsLoadingManifest = manifest.IsLoadingManifest,
                CargoVolume = manifest.CargoVolume.Value,
                VesselVisitNotificationId = manifest.VesselVisitNotificationId.AsGuid(),
                ContainerIds = manifest.ContainerIds.Select(c => c.AsGuid()).ToList() 
            };
        }

        public async Task<List<CargoManifestDto>> GetByVesselVisitAsync(Guid vesselVisitNotificationId)
        {
            var manifests = await _repo.GetByVesselVisitAsync(vesselVisitNotificationId);

            return manifests.Select(manifest => new CargoManifestDto
            {
                Id = manifest.Id.AsGuid(),
                IsLoadingManifest = manifest.IsLoadingManifest,
                CargoVolume = manifest.CargoVolume.Value,
                VesselVisitNotificationId = manifest.VesselVisitNotificationId.AsGuid(),
                ContainerIds = manifest.ContainerIds.Select(c => c.AsGuid()).ToList() 
            }).ToList();
        }

        public async Task<CargoManifestDto> AddAsync(CreateCargoManifestDto dto)
        {
            var vesselVisitId = new VesselVisitNotificationId(dto.VesselVisitNotificationId);

            var vesselVisitExists = await _vesselVisitRepo.ExistsAsync(vesselVisitId);
            
            var manifestAlreadyExists = await _repo
                .HasManifestForVesselVisitAsync(dto.VesselVisitNotificationId, dto.IsLoadingManifest);

            var manifest = CargoManifest.Create(
                dto.IsLoadingManifest,
                dto.CargoVolume,
                vesselVisitId,
                vesselVisitExists,
                manifestAlreadyExists
            );

            await _repo.AddAsync(manifest);
            await _unitOfWork.CommitAsync();

            return new CargoManifestDto
            {
                Id = manifest.Id.AsGuid(),
                IsLoadingManifest = manifest.IsLoadingManifest,
                CargoVolume = manifest.CargoVolume.Value,
                VesselVisitNotificationId = manifest.VesselVisitNotificationId.AsGuid(),
                ContainerIds = manifest.ContainerIds.Select(c => c.AsGuid()).ToList()
            };
        }

        public async Task<CargoManifestDto> UpdateAsync(UpdateCargoManifestDto dto)
        {
            var manifest = await _repo.GetByIdAsync(new CargoManifestId(dto.Id));
            if (manifest == null) return null;

            manifest.updateCargoVolume(dto.CargoVolume);

            await _unitOfWork.CommitAsync();

            return new CargoManifestDto
            {
                Id = manifest.Id.AsGuid(),
                IsLoadingManifest = manifest.IsLoadingManifest,
                CargoVolume = manifest.CargoVolume.Value,
                VesselVisitNotificationId = manifest.VesselVisitNotificationId.AsGuid(),
                ContainerIds = manifest.ContainerIds.Select(c => c.AsGuid()).ToList() 
            };
        }

        public async Task AddContainerAsync(Guid manifestId, Guid containerId)
        {
            var manifest = await _repo.GetByIdAsync(new CargoManifestId(manifestId));
            if (manifest == null)
                throw new KeyNotFoundException($"Manifest with ID {manifestId} not found.");

            manifest.AddContainer(new ContainerId(containerId));
            
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveContainerAsync(Guid manifestId, Guid containerId)
        {
            var manifest = await _repo.GetByIdAsync(new CargoManifestId(manifestId));
            if (manifest == null)
                throw new KeyNotFoundException($"Manifest with ID {manifestId} not found.");

            manifest.RemoveContainer(new ContainerId(containerId));
            
            await _unitOfWork.CommitAsync();
        }

        public async Task<CargoManifestDto> DeleteAsync(CargoManifestId id)
        {
            var manifest = await _repo.GetByIdAsync(id);
            if (manifest == null) return null;

            _repo.Remove(manifest);
            await _unitOfWork.CommitAsync();

            return new CargoManifestDto
            {
                Id = manifest.Id.AsGuid(),
                IsLoadingManifest = manifest.IsLoadingManifest,
                CargoVolume = manifest.CargoVolume.Value,
                VesselVisitNotificationId = manifest.VesselVisitNotificationId.AsGuid(),
                ContainerIds = manifest.ContainerIds.Select(c => c.AsGuid()).ToList() 
            };
        }
    }
}