using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Domain.Storage;
using DDDSample1.Application.DTOs;
using DDDSample1.Domain.Shared;

namespace DDDSample1.Application.Services
{
    public class FacilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFacilityRepository _repo;

        public FacilityService(IFacilityRepository repo, IUnitOfWork unitOfWork)
        {
            this._repo = repo;
            this._unitOfWork = unitOfWork;
        }

        public async Task<List<FacilityDto>> GetAllAsync()
        {
            var facilities = await _repo.GetAllAsync();

            List<FacilityDto> listDto = facilities.ConvertAll<FacilityDto>(facility => 
                new FacilityDto(facility.Id.AsGuid(), facility.Type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU));

            return listDto;
        }

        public async Task<FacilityDto> GetByIdAsync(FacilityId id)
        {
            var facility = await _repo.GetByIdAsync(id);
            if (facility == null) return null;
            return new FacilityDto(facility.Id.AsGuid(), facility.Type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }

        public async Task<FacilityDto> AddAsync(CreatingFacilityDto dto)
        {
            var facility = new Facility(dto.Type, dto.Location, dto.MaxCapacityTEU);
            await _repo.AddAsync(facility);
            await this._unitOfWork.CommitAsync();
            return new FacilityDto(facility.Id.AsGuid(), facility.Type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }

        public async Task<FacilityDto> UpdateAsync(int currentOccupancyTEU, FacilityDto dto)
        {
            var facility = await _repo.GetByIdAsync(new FacilityId(dto.Id));
            if (facility == null) return null;

            facility.ChangeCurrentOccupancyTEU(currentOccupancyTEU);

            await this._unitOfWork.CommitAsync();
            return new FacilityDto(facility.Id.AsGuid(), facility.Type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }

        public async Task<FacilityDto> DeleteAsync(FacilityId id)
        {
            var facility = await _repo.GetByIdAsync(id);
            if (facility == null) return null;
            this._repo.Remove(facility);
            await this._unitOfWork.CommitAsync();
            return new FacilityDto(facility.Id.AsGuid(), facility.Type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }
    }
}
