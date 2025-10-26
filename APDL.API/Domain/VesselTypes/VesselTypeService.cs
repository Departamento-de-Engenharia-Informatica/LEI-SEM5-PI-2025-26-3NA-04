
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.VesselTypes
{
    public class VesselTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVesselTypeRepository _repo;

        public VesselTypeService(IUnitOfWork unitOfWork, IVesselTypeRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<VesselTypeDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.ConvertAll(v => new VesselTypeDto
            {
                Id = v.Id.AsString(),
                Name = v.Name.Value,
                Description = v.Description.Value,
                Capacity = v.Capacity.Value,
                MaxRows = v.MaxRows.Value,
                MaxBays = v.MaxBays.Value,
                MaxTiers = v.MaxTiers.Value
            });
        }

        public async Task<VesselTypeDto> GetByIdAsync(string id)
        {
            var v = await _repo.GetByIdAsync(new VesselTypeId(id));
            if (v == null) return null;
            return new VesselTypeDto
            {
                Id = v.Id.AsString(),
                Name = v.Name.Value,
                Description = v.Description.Value,
                Capacity = v.Capacity.Value,
                MaxRows = v.MaxRows.Value,
                MaxBays = v.MaxBays.Value,
                MaxTiers = v.MaxTiers.Value
            };
        }


        public async Task<List<VesselTypeDto>> SearchAsyncName(string name)
        {
            var vesselTypes = await _repo.GetByNameAsync(name);

            return vesselTypes.ConvertAll(v => new VesselTypeDto
            {
                Id = v.Id.AsString(),
                Name = v.Name.Value,
                Description = v.Description.Value,
                Capacity = v.Capacity.Value,
                MaxRows = v.MaxRows.Value,
                MaxBays = v.MaxBays.Value,
                MaxTiers = v.MaxTiers.Value
            });
        }
        
        
        public async Task<List<VesselTypeDto>> SearchAsyncDescription(string description)
        {
            var vesselTypes = await _repo.GetByDescriptionAsync(description);

            return vesselTypes.ConvertAll(v => new VesselTypeDto
            {
                Id = v.Id.AsString(),
                Name = v.Name.Value,
                Description = v.Description.Value,
                Capacity = v.Capacity.Value,
                MaxRows = v.MaxRows.Value,
                MaxBays = v.MaxBays.Value,
                MaxTiers = v.MaxTiers.Value
            });
        }



        public async Task<VesselTypeDto> AddAsync(CreatingVesselTypeDto dto)
        {
            var vesselType = new VesselType(dto.Name, dto.Description, dto.Capacity, dto.MaxRows, dto.MaxBays, dto.MaxTiers);
            await _repo.AddAsync(vesselType);
            await _unitOfWork.CommitAsync();
            return new VesselTypeDto
            {
                Id = vesselType.Id.AsString(),
                Name = vesselType.Name.Value,
                Description = vesselType.Description.Value,
                Capacity = vesselType.Capacity.Value,
                MaxRows = vesselType.MaxRows.Value,
                MaxBays = vesselType.MaxBays.Value,
                MaxTiers = vesselType.MaxTiers.Value
            };
        }

        public async Task<VesselTypeDto> UpdateAsync(string id, CreatingVesselTypeDto dto)
        {
            var vesselType = await _repo.GetByIdAsync(new VesselTypeId(id));
            if (vesselType == null) return null;
            vesselType.Update(dto.Name, dto.Description, dto.Capacity, dto.MaxRows, dto.MaxBays, dto.MaxTiers);
            await _unitOfWork.CommitAsync();
            return new VesselTypeDto
            {
                Id = vesselType.Id.AsString(),
                Name = vesselType.Name.Value,
                Description = vesselType.Description.Value,
                Capacity = vesselType.Capacity.Value,
                MaxRows = vesselType.MaxRows.Value,
                MaxBays = vesselType.MaxBays.Value,
                MaxTiers = vesselType.MaxTiers.Value
            };
        }

        public async Task<VesselTypeDto> DeleteAsync(string id)
        {
            var vesselType = await _repo.GetByIdAsync(new VesselTypeId(id));
            if (vesselType == null) return null;
            _repo.Remove(vesselType);
            await _unitOfWork.CommitAsync();
            return new VesselTypeDto
            {
                Id = vesselType.Id.AsString(),
                Name = vesselType.Name.Value,
                Description = vesselType.Description.Value,
                Capacity = vesselType.Capacity.Value,
                MaxRows = vesselType.MaxRows.Value,
                MaxBays = vesselType.MaxBays.Value,
                MaxTiers = vesselType.MaxTiers.Value
            };
        }
    }
}
