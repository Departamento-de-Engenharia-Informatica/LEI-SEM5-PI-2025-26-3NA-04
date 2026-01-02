using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.PrivacyPolicyAggregate.DTO;
using APDL.API.Domain.PrivacyPolicyAggregate.Repos;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.PrivacyPolicyAggregate
{
    public class PrivacyPolicyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPrivacyPolicyRepository _repo;

        public PrivacyPolicyService(IUnitOfWork unitOfWork, IPrivacyPolicyRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<PrivacyPolicyDto> GetActivePolicyAsync()
        {
            var policy = await _repo.GetActiveAsync();
            if (policy == null) return null;

            return new PrivacyPolicyDto
            {
                Id = policy.Id.ToString(),
                Version = policy.Version,
                Content = policy.Content,
                EffectiveDate = policy.EffectiveDate,
                IsActive = policy.IsActive,
                CreatedBy = policy.CreatedBy.ToString(),
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt
            };
        }

        public async Task<List<PrivacyPolicyDto>> GetAllVersionsAsync()
        {
            var policies = await _repo.GetAllVersionsAsync();
            return policies.OrderByDescending(p => p.Version).Select(p => new PrivacyPolicyDto
            {
                Id = p.Id.ToString(),
                Version = p.Version,
                Content = p.Content,
                EffectiveDate = p.EffectiveDate,
                IsActive = p.IsActive,
                CreatedBy = p.CreatedBy.ToString(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }

        public async Task<PrivacyPolicyDto> GetByVersionAsync(int version)
        {
            var policy = await _repo.GetByVersionAsync(version);
            if (policy == null) return null;

            return new PrivacyPolicyDto
            {
                Id = policy.Id.ToString(),
                Version = policy.Version,
                Content = policy.Content,
                EffectiveDate = policy.EffectiveDate,
                IsActive = policy.IsActive,
                CreatedBy = policy.CreatedBy.ToString(),
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt
            };
        }

        public async Task<PrivacyPolicyDto> PublishNewVersionAsync(CreatePrivacyPolicyDto dto, Guid createdBy)
        {
            var allVersions = await _repo.GetAllVersionsAsync();
            var nextVersion = allVersions != null && allVersions.Any() ? allVersions.Max(p => p.Version) + 1 : 1;

            var activePolicy = await _repo.GetActiveAsync();
            if (activePolicy != null)
            {
                activePolicy.Deactivate();
            }

            var newPolicy = new PrivacyPolicy(nextVersion, dto.Content, dto.EffectiveDate, createdBy);
            newPolicy.Activate();

            await _repo.AddAsync(newPolicy);
            await _unitOfWork.CommitAsync();

            return new PrivacyPolicyDto
            {
                Id = newPolicy.Id.ToString(),
                Version = newPolicy.Version,
                Content = newPolicy.Content,
                EffectiveDate = newPolicy.EffectiveDate,
                IsActive = newPolicy.IsActive,
                CreatedBy = newPolicy.CreatedBy.ToString(),
                CreatedAt = newPolicy.CreatedAt,
                UpdatedAt = newPolicy.UpdatedAt
            };
        }

        public async Task<PrivacyPolicyDto> UpdatePolicyAsync(string id, UpdatePrivacyPolicyDto dto)
        {
            var policy = await _repo.GetByIdAsync(new PrivacyPolicyId(id));
            if (policy == null) return null;

            policy.UpdateContent(dto.Content);
            await _unitOfWork.CommitAsync();

            return new PrivacyPolicyDto
            {
                Id = policy.Id.ToString(),
                Version = policy.Version,
                Content = policy.Content,
                EffectiveDate = policy.EffectiveDate,
                IsActive = policy.IsActive,
                CreatedBy = policy.CreatedBy.ToString(),
                CreatedAt = policy.CreatedAt,
                UpdatedAt = policy.UpdatedAt
            };
        }
    }
}

