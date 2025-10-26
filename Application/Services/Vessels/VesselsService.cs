using System.Collections.Generic;
using System.Threading.Tasks;
using DDDSample1.Application.DTOs;
using DDDSample1.Domain.Vessels;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.VesselTypes;

namespace DDDSample1.Application.Services
{
    public class VesselService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVesselRepository _repo;
        private readonly IVesselTypeRepository _vesselTypeRepo;

        public VesselService(IUnitOfWork unitOfWork, IVesselRepository repo, IVesselTypeRepository _vesselTypeRepo) 
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._vesselTypeRepo = _vesselTypeRepo;
        }

        public async Task<List<VesselDto>> GetAllAsync()
        {
            var vessels = await _repo.GetAllAsync();


            if (vessels == null)
                return new List<VesselDto>();

            List<VesselDto> listDto = vessels.ConvertAll<VesselDto>(vessel =>
                new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator));

            return listDto;
        }

        public async Task<VesselDto> GetByIdAsync(VesselId id)
        {
            var vessel = await _repo.GetByIdAsync(id);
            if (vessel == null) return null;

            return new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator);
        }

        public async Task<VesselDto> AddAsync(CreatingVesselDto dto)
        {
            // Validação do número IMO
            if (!IsValidImoNumber(dto.ImoNumber))
                throw new BusinessRuleValidationException("Invalid IMO number format or check digit.");

            var vesselTypeId = new VesselTypeId(dto.VesselTypeId);
            var vesselType = await _vesselTypeRepo.GetByIdAsync(vesselTypeId);
            if (vesselType == null)
                throw new BusinessRuleValidationException("Vessel type not found.");


            var vessel = new Vessel(dto.Name, dto.ImoNumber, dto.VesselTypeId, vesselType, dto.Operator);

            await _repo.AddAsync(vessel);
            await _unitOfWork.CommitAsync();

            return new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator);
        }

        public async Task<VesselDto> DeleteAsync(VesselId id)
        {
            var vessel = await _repo.GetByIdAsync(id);
            if (vessel == null) return null;

            _repo.Remove(vessel);
            await _unitOfWork.CommitAsync();

            return new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator);
        }

        
        public async Task<List<VesselDto>> SearchByNameAsync(string name)
        {
            var vessels = await _repo.SearchByNameAsync(name);

            if (vessels == null)
                return new List<VesselDto>();

            List<VesselDto> listDto = vessels.ConvertAll<VesselDto>(vessel =>
                new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator));

            return listDto;
        }

        public async Task<List<VesselDto>> SearchByOperatorAsync(string operatorName)
        {
            var vessels = await _repo.SearchByOperatorAsync(operatorName);

            if (vessels == null)
                return new List<VesselDto>();

            List<VesselDto> listDto = vessels.ConvertAll<VesselDto>(vessel =>
                new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator));

            return listDto;
        }

        
        public async Task<VesselDto> GetByImoAsync(string imoNumber)
        {
            var vessel = await _repo.GetByImoAsync(imoNumber);

           if (vessel == null) return null;

            return new VesselDto(vessel.Id.AsGuid(), vessel.Name, vessel.ImoNumber, vessel.VesselTypeId, vessel.Operator);
        }

        private bool IsValidImoNumber(string imo)
        {
            if (imo.Length != 7 || !int.TryParse(imo, out _)) return false;

            int sum = 0;
            for (int i = 0; i < 6; i++)
            {
                sum += (imo[i] - '0') * (7 - i);
            }

            int checkDigit = sum % 10;
            return checkDigit == (imo[6] - '0');
        }
    }
}