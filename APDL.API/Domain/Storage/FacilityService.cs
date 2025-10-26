
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Storage;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Storage.ValueObjects;
using APDL.API.Domain.Dock;

namespace APDL.API.Domain.Storage
{
    public class FacilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFacilityRepository _repo;

        public FacilityService(IUnitOfWork unitOfWork, IFacilityRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<FacilityDto>> GetAllAsync()
        {
            var facilities = await _repo.GetAllAsync();
            if (facilities == null)
                return new List<FacilityDto>();

            return facilities.ConvertAll(facility =>
            {
                string type = facility is Yard ? "Yard" : "Warehouse";
                return new FacilityDto(facility.Id.AsGuid(), type, facility.Location.Value, facility.MaxCapacityTEU.Value, facility.CurrentOccupancyTEU, 
                facility.DockAssignments.ConvertAll(da => new DockAssignmentDto
                    {
                        DockId = da.DockId.AsGuid(),
                        DistanceMeters = da.DistanceToDock.Meters
                    })
                );
            });
        }

        public async Task<FacilityDto> GetByIdAsync(FacilityId id)
        {
            var facility = await _repo.GetByIdAsync(id);
            if (facility == null) return null;

            string type = facility is Yard ? "Yard" : "Warehouse";
            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location.Value, facility.MaxCapacityTEU.Value, facility.CurrentOccupancyTEU, 
            facility.DockAssignments.ConvertAll(da => new DockAssignmentDto
                {
                    DockId = da.DockId.AsGuid(),
                    DistanceMeters = da.DistanceToDock.Meters
                })
            );
        }

        public async Task<FacilityDto> AddAsync(CreatingFacilityDto dto)
        {
            var location = new Location(dto.Location);
            var maxCapacity = new TEUCapacity(dto.MaxCapacityTEU);

            Facility facility = dto.Type switch
            {
                "Yard" => new Yard(location, maxCapacity),
                "Warehouse" => new Warehouse(location, maxCapacity),
                _ => throw new BusinessRuleValidationException("Invalid facility type. Must be 'Yard' or 'Warehouse'.")
            };

            if (dto.DockAssignments != null)
            {
                foreach (var assignment in dto.DockAssignments)
                {
                    var dockId = new DockId(assignment.DockId);
                    var distance = new Distance(assignment.DistanceMeters);
                    facility.AssignDock(dockId, distance);
                }
            }

            await _repo.AddAsync(facility);
            await _unitOfWork.CommitAsync();

            string type = facility is Yard ? "Yard" : "Warehouse";
            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location.Value, facility.MaxCapacityTEU.Value, facility.CurrentOccupancyTEU, 
            facility.DockAssignments.ConvertAll(da => new DockAssignmentDto
                {
                    DockId = da.DockId.AsGuid(),
                    DistanceMeters = da.DistanceToDock.Meters
                })
            );
        }

        public async Task<FacilityDto> UpdateAsync(int currentOccupancyTEU, FacilityDto dto)
        {
            var facility = await _repo.GetByIdAsync(new FacilityId(dto.Id));
            if (facility == null) return null;

            facility.ChangeCurrentOccupancyTEU(currentOccupancyTEU);
            await _unitOfWork.CommitAsync();

            string type = facility is Yard ? "Yard" : "Warehouse";
            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location.Value, facility.MaxCapacityTEU.Value, facility.CurrentOccupancyTEU, 
            facility.DockAssignments.ConvertAll(da => new DockAssignmentDto
                {
                    DockId = da.DockId.AsGuid(),
                    DistanceMeters = da.DistanceToDock.Meters
                })
            );
        }

        public async Task<FacilityDto> DeleteAsync(FacilityId id)
        {
            var facility = await _repo.GetByIdAsync(id);
            if (facility == null) return null;

            _repo.Remove(facility);
            await _unitOfWork.CommitAsync();

            string type = facility is Yard ? "Yard" : "Warehouse";
            return new FacilityDto(facility.Id.AsGuid(), type, facility.Location.Value, facility.MaxCapacityTEU.Value, facility.CurrentOccupancyTEU, 
            facility.DockAssignments.ConvertAll(da => new DockAssignmentDto
                {
                    DockId = da.DockId.AsGuid(),
                    DistanceMeters = da.DistanceToDock.Meters
                })
            );
        }
    }
}

