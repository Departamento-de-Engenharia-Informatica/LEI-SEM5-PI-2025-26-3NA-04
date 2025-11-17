using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.QualificationsAggregate;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Domain.StaffQualificationAggregate.DTO;

namespace APDL.API.Domain.StaffQualificationAggregate
{
    public class StaffQualificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffQualificationRepository _repo;

        public StaffQualificationService(IUnitOfWork unitOfWork, IStaffQualificationRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<StaffQualificationDto>> GetAllAsync()
        {
            var qualifications = await _repo.GetAllAsync();

            return qualifications
                .Select(q => new StaffQualificationDto
                {
                    Id = q.Id.AsGuid(),
                    QualificationType = q.QualificationType.ToString(),
                    QualificationName = q.QualificationName,
                    Description = q.Description,
                    IsActive = q.IsActive,
                })
                .ToList();
        }

        public async Task<StaffQualificationDto> GetByIdAsync(StaffQualificationId id)
        {
            var qualification = await _repo.GetByIdAsync(id);
            if (qualification == null)
                return null;

            return new StaffQualificationDto
            {
                Id = qualification.Id.AsGuid(),
                QualificationType = qualification.QualificationType.ToString(),
                QualificationName = qualification.QualificationName,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
            };
        }

        public async Task<List<StaffQualificationDto>> GetActiveQualificationsAsync()
        {
            var qualifications = await _repo.GetActiveQualificationsAsync();

            return qualifications
                .Select(q => new StaffQualificationDto
                {
                    Id = q.Id.AsGuid(),
                    QualificationType = q.QualificationType.ToString(),
                    QualificationName = q.QualificationName,
                    Description = q.Description,
                    IsActive = q.IsActive,
                })
                .ToList();
        }

        public async Task<StaffQualificationDto> AddAsync(CreateStaffQualificationDto dto)
        {
            var qualificationType = QualificationType.FromString(dto.QualificationType);

            var exists = await _repo.QualificationTypeExistsAsync(qualificationType);
            if (exists)
                throw new BusinessRuleValidationException(
                    nameof(dto.QualificationType),
                    $"Qualification type '{dto.QualificationType}' already exists."
                );

            var qualification = new StaffQualification(
                new StaffQualificationId(Guid.NewGuid()),
                qualificationType,
                dto.QualificationName,
                dto.Description
            );

            await _repo.AddAsync(qualification);
            await _unitOfWork.CommitAsync();

            return new StaffQualificationDto
            {
                Id = qualification.Id.AsGuid(),
                QualificationType = qualification.QualificationType.ToString(),
                QualificationName = qualification.QualificationName,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
            };
        }

        public async Task<StaffQualificationDto> UpdateAsync(UpdateStaffQualificationDto dto)
        {
            var qualification = await _repo.GetByIdAsync(new StaffQualificationId(dto.Id));
            if (qualification == null)
                return null;

            qualification.UpdateQualificationName(dto.QualificationName);
            qualification.UpdateDescription(dto.Description);

            await _unitOfWork.CommitAsync();

            return new StaffQualificationDto
            {
                Id = qualification.Id.AsGuid(),
                QualificationType = qualification.QualificationType.ToString(),
                QualificationName = qualification.QualificationName,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
            };
        }

        public async Task<StaffQualificationDto> ActivateAsync(StaffQualificationId id)
        {
            var qualification = await _repo.GetByIdAsync(id);
            if (qualification == null)
                return null;

            qualification.Activate();
            await _unitOfWork.CommitAsync();

            return new StaffQualificationDto
            {
                Id = qualification.Id.AsGuid(),
                QualificationType = qualification.QualificationType.ToString(),
                QualificationName = qualification.QualificationName,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
            };
        }

        public async Task<StaffQualificationDto> DeactivateAsync(StaffQualificationId id)
        {
            var qualification = await _repo.GetByIdAsync(id);
            if (qualification == null)
                return null;

            qualification.Deactivate();
            await _unitOfWork.CommitAsync();

            return new StaffQualificationDto
            {
                Id = qualification.Id.AsGuid(),
                QualificationType = qualification.QualificationType.ToString(),
                QualificationName = qualification.QualificationName,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
            };
        }

        public async Task<StaffQualificationDto> DeleteAsync(StaffQualificationId id)
        {
            var qualification = await _repo.GetByIdAsync(id);
            if (qualification == null)
                return null;

            _repo.Remove(qualification);
            await _unitOfWork.CommitAsync();

            return new StaffQualificationDto
            {
                Id = qualification.Id.AsGuid(),
                QualificationType = qualification.QualificationType.ToString(),
                QualificationName = qualification.QualificationName,
                Description = qualification.Description,
                IsActive = qualification.IsActive,
            };
        }
    }
}
