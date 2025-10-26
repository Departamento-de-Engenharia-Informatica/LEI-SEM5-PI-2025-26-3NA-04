using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.Vessels;
public class VesselService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVesselRepository _repo;
    private readonly IVesselTypeRepository _vesselTypeRepo;

    public VesselService(IUnitOfWork unitOfWork, IVesselRepository repo, IVesselTypeRepository vesselTypeRepo)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
        _vesselTypeRepo = vesselTypeRepo;
    }

    public async Task<List<VesselDto>> GetAllAsync()
    {
        var vessels = await _repo.GetAllAsync();
        if (vessels == null) return new List<VesselDto>();

        return vessels.ConvertAll(v => new VesselDto(
            v.Id.AsGuid(),
            v.Name.Value,
            v.ImoNumber.Value,
            v.VesselTypeId.AsGuid(),
            v.Operator.Value
        ));
    }

    public async Task<VesselDto> GetByIdAsync(VesselId id)
    {
        var vessel = await _repo.GetByIdAsync(id);
        if (vessel == null) return null;

        return new VesselDto(
            vessel.Id.AsGuid(),
            vessel.Name.Value,
            vessel.ImoNumber.Value,
            vessel.VesselTypeId.AsGuid(),
            vessel.Operator.Value
        );
    }

    public async Task<VesselDto> AddAsync(CreatingVesselDto dto)
    {
        var vesselTypeId = new VesselTypeId(dto.VesselTypeId);
        var vesselType = await _vesselTypeRepo.GetByIdAsync(vesselTypeId);
        if (vesselType == null)
            throw new BusinessRuleValidationException("Vessel type not found.");

        var vessel = new Vessel(
            dto.Name,
            dto.ImoNumber,
            vesselTypeId,
            vesselType,
            dto.Operator
        );

        await _repo.AddAsync(vessel);
        await _unitOfWork.CommitAsync();

        return new VesselDto(
            vessel.Id.AsGuid(),
            vessel.Name.Value,
            vessel.ImoNumber.Value,
            vessel.VesselTypeId.AsGuid(),
            vessel.Operator.Value
        );
    }

    public async Task<VesselDto> DeleteAsync(VesselId id)
    {
        var vessel = await _repo.GetByIdAsync(id);
        if (vessel == null) return null;

        _repo.Remove(vessel);
        await _unitOfWork.CommitAsync();

        return new VesselDto(
            vessel.Id.AsGuid(),
            vessel.Name.Value,
            vessel.ImoNumber.Value,
            vessel.VesselTypeId.AsGuid(),
            vessel.Operator.Value
        );
    }

    public async Task<List<VesselDto>> SearchByNameAsync(string name)
    {
        var vessels = await _repo.SearchByNameAsync(name);
        if (vessels == null) return new List<VesselDto>();

        return vessels.ConvertAll(v => new VesselDto(
            v.Id.AsGuid(),
            v.Name.Value,
            v.ImoNumber.Value,
            v.VesselTypeId.AsGuid(),
            v.Operator.Value
        ));
    }

    public async Task<List<VesselDto>> SearchByOperatorAsync(string operatorName)
    {
        var vessels = await _repo.SearchByOperatorAsync(operatorName);
        if (vessels == null) return new List<VesselDto>();

        return vessels.ConvertAll(v => new VesselDto(
            v.Id.AsGuid(),
            v.Name.Value,
            v.ImoNumber.Value,
            v.VesselTypeId.AsGuid(),
            v.Operator.Value
        ));
    }

    public async Task<VesselDto> GetByImoAsync(string imoNumber)
    {
        var vessel = await _repo.GetByImoAsync(imoNumber);
        if (vessel == null) return null;

        return new VesselDto(
            vessel.Id.AsGuid(),
            vessel.Name.Value,
            vessel.ImoNumber.Value,
            vessel.VesselTypeId.AsGuid(),
            vessel.Operator.Value
        );
    }
}