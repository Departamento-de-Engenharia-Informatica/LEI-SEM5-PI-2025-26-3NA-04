using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate.DTO;
using APDL.API.Domain.ShippingAgentAggregate.Repos;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;

namespace APDL.API.Domain.ShippingAgentAggregate
{
    public class ShippingAgentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShippingAgentRepository _repo;

        public ShippingAgentService(IUnitOfWork unitOfWork, IShippingAgentRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<ShippingAgentDto>> GetAllAsync()
        {
            var agents = await _repo.GetAllAsync();

            return agents.Select(agent => new ShippingAgentDto
            {
                Id = agent.Id.AsGuid(),
                LegalName = agent.LegalName.Value,
                AlternativeName = agent.AlternativeName?.Value,
                Address = agent.Address.Value,
                TaxNumber = agent.TaxNumber.Value,
                Representatives = agent.Representatives
                    .Select(rep => new RepresentativeDto
                    {
                        Id = rep.Id.AsGuid(),
                        Name = rep.Name.Value,
                        Email = rep.Email.Value,
                        CitizenId = rep.CitizenId.Value,
                        Nationality = rep.Nationality.Value,
                        Phone = rep.Phone.Value
                    }).ToList()
            }).ToList();
        }

        public async Task<ShippingAgentDto> GetByIdAsync(ShippingAgentId id)
        {
            Console.WriteLine($"SERVICE: Received ID = {id.Value}");
            var agent = await _repo.GetByIdAsync(id);
            if (agent == null){
                Console.WriteLine("DEBUG: Agent is NULL inside GetByIdAsync!");
                return null;
            } 

            return new ShippingAgentDto
            {
                Id = agent.Id.AsGuid(),
                LegalName = agent.LegalName.Value,
                AlternativeName = agent.AlternativeName?.Value,
                Address = agent.Address.Value,
                TaxNumber = agent.TaxNumber.Value,
                Representatives = agent.Representatives
                    .Select(rep => new RepresentativeDto
                    {
                        Id = rep.Id.AsGuid(),
                        Name = rep.Name.Value,
                        Email = rep.Email.Value,
                        CitizenId = rep.CitizenId.Value,
                        Nationality = rep.Nationality.Value,
                        Phone = rep.Phone.Value
                    }).ToList()
            };
        }

        public async Task<ShippingAgentDto> AddAsync(CreateShippingAgentDto dto)
        {
            if (dto.Representatives == null || !dto.Representatives.Any())
                throw new BusinessRuleValidationException(nameof(dto.Representatives), "At least one representative is required.");

            var reps = dto.Representatives.Select(r => new ShippingAgentRepresentative(
                new Name(r.Name),
                new CitizenId(r.CitizenId),
                new Nationality(r.Nationality),
                new Email(r.Email),
                new Phone(r.Phone)
            )).ToList();

            Name? altName = string.IsNullOrWhiteSpace(dto.AlternativeName) ? null : new Name(dto.AlternativeName);

            var agent = new ShippingAgent(
                dto.LegalName,
                dto.AlternativeName,
                dto.Address,
                dto.TaxNumber,
                reps
            );

            await _repo.AddAsync(agent);
            await _unitOfWork.CommitAsync();

            return new ShippingAgentDto
            {
                Id = agent.Id.AsGuid(),
                LegalName = agent.LegalName.Value,
                AlternativeName = agent.AlternativeName?.Value,
                Address = agent.Address.Value,
                TaxNumber = agent.TaxNumber.Value,
                Representatives = agent.Representatives
                    .Select(r => new RepresentativeDto
                    {
                        Id = r.Id.AsGuid(),
                        Name = r.Name.Value,
                        CitizenId = r.CitizenId.Value,
                        Nationality = r.Nationality.Value,
                        Email = r.Email.Value,
                        Phone = r.Phone.Value
                    }).ToList()
            };
        }


        public async Task<ShippingAgentDto> UpdateAsync(ShippingAgentDto dto)
        {
            var agent = await _repo.GetByIdAsync(new ShippingAgentId(dto.Id));
            if (agent == null) return null;

            agent.UpdateLegalName(dto.LegalName);
            agent.UpdateAlternativeName(dto.AlternativeName);
            agent.UpdateAddress(dto.Address);
            agent.UpdateTaxNumber(dto.TaxNumber);

            await _unitOfWork.CommitAsync();

            return new ShippingAgentDto
            {
                Id = agent.Id.AsGuid(),
                LegalName = agent.LegalName.Value,
                AlternativeName = agent.AlternativeName?.Value,
                Address = agent.Address.Value,
                TaxNumber = agent.TaxNumber.Value,
                Representatives = agent.Representatives
                    .Select(rep => new RepresentativeDto
                    {
                        Id = rep.Id.AsGuid(),
                        Name = rep.Name.Value,
                        CitizenId = rep.CitizenId.Value,
                        Nationality = rep.Nationality.Value,
                        Email = rep.Email.Value,
                        Phone = rep.Phone.Value
                    }).ToList()
            };
        }


        public async Task<ShippingAgentDto> DeleteAsync(ShippingAgentId id)
        {
            var agent = await _repo.GetByIdAsync(id);
            if (agent == null) return null;

            _repo.Remove(agent);
            await _unitOfWork.CommitAsync();

            return new ShippingAgentDto
            {
                Id = agent.Id.AsGuid(),
                LegalName = agent.LegalName.Value,
                AlternativeName = agent.AlternativeName?.Value,
                Address = agent.Address.Value,
                TaxNumber = agent.TaxNumber.Value,
                Representatives = agent.Representatives
                    .Select(rep => new RepresentativeDto
                    {
                        Id = rep.Id.AsGuid(),
                        Name = rep.Name.Value,
                        CitizenId = rep.CitizenId.Value,
                        Nationality = rep.Nationality.Value,
                        Email = rep.Email.Value,
                        Phone = rep.Phone.Value
                    }).ToList()
            };
        }
    }
}
