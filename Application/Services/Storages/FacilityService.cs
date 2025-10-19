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

        public FacilityService(IUnitOfWork unitOfWork, IFacilityRepository repo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<List<FacilityDto>> GetAllAsync()
        {
            var facilities = await _repo.GetAllAsync();


            if (facilities == null)
                return new List<FacilityDto>();


            List<FacilityDto> listDto = facilities.ConvertAll<FacilityDto>(facility =>
            {
                string type = facility is Yard ? "Yard" : "Warehouse";
                return new FacilityDto(facility.Id.AsGuid(), type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
            });

            return listDto;

        }

        public async Task<FacilityDto> GetByIdAsync(FacilityId id)
        {
            var facility = await _repo.GetByIdAsync(id);
            if (facility == null) return null;

            string type = facility is Yard ? "Yard" : "Warehouse";

            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }

        public async Task<FacilityDto> AddAsync(CreatingFacilityDto dto)
        {
            
            Facility facility;

            if (dto.Type == "Yard")
            {
                facility = new Yard(dto.Location, dto.MaxCapacityTEU);
            }
            else if (dto.Type == "Warehouse")
            {
                facility = new Warehouse(dto.Location, dto.MaxCapacityTEU);
            }
            else
            {
                throw new BusinessRuleValidationException("Invalid facility type. Must be 'Yard' or 'Warehouse'.");
            }

            await _repo.AddAsync(facility);
            await this._unitOfWork.CommitAsync();

            string type = facility is Yard ? "Yard" : "Warehouse";

            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }

        public async Task<FacilityDto> UpdateAsync(int currentOccupancyTEU, FacilityDto dto)
        {
            var facility = await _repo.GetByIdAsync(new FacilityId(dto.Id));
            if (facility == null) return null;

            facility.ChangeCurrentOccupancyTEU(currentOccupancyTEU);

            await this._unitOfWork.CommitAsync();

            string type = facility is Yard ? "Yard" : "Warehouse";

            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }

        public async Task<FacilityDto> DeleteAsync(FacilityId id)
        {
            var facility = await _repo.GetByIdAsync(id);
            if (facility == null) return null;
            this._repo.Remove(facility);
            await this._unitOfWork.CommitAsync();

            string type = facility is Yard ? "Yard" : "Warehouse";

            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location, facility.MaxCapacityTEU, facility.CurrentOccupancyTEU);
        }
    }
}
