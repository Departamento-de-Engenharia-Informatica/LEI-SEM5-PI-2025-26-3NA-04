using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;
using APDL.API.Domain.ShippingAgentAggregate.DTO;

namespace APDL.API.Domain.ShippingAgentAggregate
{
    public class ShippingAgentRepresentativeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShippingAgentRepresentativeRepository _repo;

        public ShippingAgentRepresentativeService(IUnitOfWork unitOfWork, IShippingAgentRepresentativeRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<RepresentativeDto>> GetAllAsync()
        {
            var reps = await _repo.GetAllAsync();

            return reps.Select(rep => new RepresentativeDto
            {
                Id = rep.Id.AsGuid(),
                Name = rep.Name.Value,
                CitizenId = rep.CitizenId.Value,
                Nationality = rep.Nationality.Value,
                Email = rep.Email.Value,
                Phone = rep.Phone.Value,
                IsActive = rep.IsActive
            }).ToList();
        }

        public async Task<RepresentativeDto> GetByIdAsync(ShippingAgentRepresentativeId id)
        {
            var rep = await _repo.GetByIdAsync(id);

            if (rep == null)
                return null;

            return new RepresentativeDto
            {
                Id = rep.Id.AsGuid(),
                Name = rep.Name.Value,
                CitizenId = rep.CitizenId.Value,
                Nationality = rep.Nationality.Value,
                Email = rep.Email.Value,
                Phone = rep.Phone.Value,
                IsActive = rep.IsActive
            };
        }

        public async Task<RepresentativeDto> AddAsync(CreateRepresentativeDto dto)
        {
            var rep = new ShippingAgentRepresentative(
                new Name(dto.Name),
                new CitizenId(dto.CitizenId),
                new Nationality(dto.Nationality),
                new Email(dto.Email),
                new Phone(dto.Phone)
            );

            await _repo.AddAsync(rep);
            await _unitOfWork.CommitAsync();

            return new RepresentativeDto
            {
                Id = rep.Id.AsGuid(),
                Name = rep.Name.Value,
                CitizenId = rep.CitizenId.Value,
                Nationality = rep.Nationality.Value,
                Email = rep.Email.Value,
                Phone = rep.Phone.Value,
                IsActive = rep.IsActive
            };
        }

        public async Task<RepresentativeDto> UpdateAsync(RepresentativeDto dto)
        {
            var rep = await _repo.GetByIdAsync(new ShippingAgentRepresentativeId(dto.Id));

            if (rep == null)
                return null;

            rep.UpdateName(new Name(dto.Name));
            rep.UpdateCitizenId(new CitizenId(dto.CitizenId));
            rep.UpdateNationality(new Nationality(dto.Nationality));
            rep.UpdateEmail(new Email(dto.Email));
            rep.UpdatePhone(new Phone(dto.Phone));

            await _unitOfWork.CommitAsync();

            return new RepresentativeDto
            {
                Id = rep.Id.AsGuid(),
                Name = rep.Name.Value,
                CitizenId = rep.CitizenId.Value,
                Nationality = rep.Nationality.Value,
                Email = rep.Email.Value,
                Phone = rep.Phone.Value,
                IsActive = rep.IsActive
            };
        }

        public async Task<RepresentativeDto> DeleteAsync(ShippingAgentRepresentativeId id)
        {
            var rep = await _repo.GetByIdAsync(id);

            if (rep == null)
                return null;

            _repo.Remove(rep);
            await _unitOfWork.CommitAsync();

            return new RepresentativeDto
            {
                Id = rep.Id.AsGuid(),
                Name = rep.Name.Value,
                CitizenId = rep.CitizenId.Value,
                Nationality = rep.Nationality.Value,
                Email = rep.Email.Value,
                Phone = rep.Phone.Value,
                IsActive = rep.IsActive
            };
        }

        public async Task DeactivateAsync(ShippingAgentRepresentativeId id)
        {
            var rep = await _repo.GetByIdAsync(id);
            if (rep == null) throw new KeyNotFoundException($"Representative {id} not found");

            rep.Deactivate();
            await _unitOfWork.CommitAsync();
        }

        public async Task ReactivateAsync(ShippingAgentRepresentativeId id)
        {
            var rep = await _repo.GetByIdAsync(id);
            if (rep == null) throw new KeyNotFoundException($"Representative {id} not found");

            rep.Reactivate();
            await _unitOfWork.CommitAsync();
        }
    
    }
}
