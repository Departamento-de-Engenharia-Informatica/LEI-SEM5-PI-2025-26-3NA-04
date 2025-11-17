using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.OperatingStaffAggregate.DTO;
using APDL.API.Domain.OperatingStaffAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Domain.StaffAggregate;

namespace APDL.API.Domain.OperatingStaffAggregate
{
    public class OperatingStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperatingStaffRepository _repo;

        public OperatingStaffService(IUnitOfWork unitOfWork, IOperatingStaffRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<OperatingStaffDto>> GetAllAsync()
        {
            var staff = await _repo.GetAllAsync();

            return staff
                .Select(s => new OperatingStaffDto
                {
                    Id = s.Id.AsGuid(),
                    MecanographicNumber = s.MecanographicNumber.Value,
                    ShortName = s.ShortName,
                    Email = s.Email,
                    Phone = s.Phone,
                    OperationalWindow = s.OperationalWindow.ToString(),
                    Status = s.Status.ToString(),
                    Qualifications = s.Qualifications.Select(q => q.ToString()).ToList(),
                })
                .ToList();
        }

        public async Task<OperatingStaffDto> GetByIdAsync(OperatingStaffId id)
        {
            var staff = await _repo.GetByIdAsync(id);
            if (staff == null)
                return null;

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }

        public async Task<OperatingStaffDto> GetByMecanographicNumberAsync(
            string mecanographicNumber
        )
        {
            var staff = await _repo.GetByMecanographicNumberAsync(mecanographicNumber);
            if (staff == null)
                return null;

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }

        public async Task<List<OperatingStaffDto>> GetAvailableStaffAsync()
        {
            var staff = await _repo.GetAvailableStaffAsync();

            return staff
                .Select(s => new OperatingStaffDto
                {
                    Id = s.Id.AsGuid(),
                    MecanographicNumber = s.MecanographicNumber.Value,
                    ShortName = s.ShortName,
                    Email = s.Email,
                    Phone = s.Phone,
                    OperationalWindow = s.OperationalWindow.ToString(),
                    Status = s.Status.ToString(),
                    Qualifications = s.Qualifications.Select(q => q.ToString()).ToList(),
                })
                .ToList();
        }

        public async Task<List<OperatingStaffDto>> GetStaffByQualificationAsync(
            string qualificationType
        )
        {
            var qualification = QualificationType.FromString(qualificationType);
            var staff = await _repo.GetStaffByQualificationAsync(qualification);

            return staff
                .Select(s => new OperatingStaffDto
                {
                    Id = s.Id.AsGuid(),
                    MecanographicNumber = s.MecanographicNumber.Value,
                    ShortName = s.ShortName,
                    Email = s.Email,
                    Phone = s.Phone,
                    OperationalWindow = s.OperationalWindow.ToString(),
                    Status = s.Status.ToString(),
                    Qualifications = s.Qualifications.Select(q => q.ToString()).ToList(),
                })
                .ToList();
        }

        public async Task<OperatingStaffDto> AddAsync(CreateOperatingStaffDto dto)
        {
            var exists = await _repo.MecanographicNumberExistsAsync(dto.MecanographicNumber);
            if (exists)
                throw new BusinessRuleValidationException(
                    nameof(dto.MecanographicNumber),
                    $"Staff with mecanographic number '{dto.MecanographicNumber}' already exists."
                );

            var operationalWindow = OperationalWindow.FromString(dto.OperationalWindow);
            var mecanographicNumber = new MecanographicNumber(dto.MecanographicNumber);

            var staff = new OperatingStaff(
                new OperatingStaffId(Guid.NewGuid()),
                mecanographicNumber,
                dto.ShortName,
                dto.Email,
                dto.Phone,
                operationalWindow
            );

            await _repo.AddAsync(staff);
            await _unitOfWork.CommitAsync();

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }

        public async Task<OperatingStaffDto> UpdateAsync(UpdateOperatingStaffDto dto)
        {
            var staff = await _repo.GetByIdAsync(new OperatingStaffId(dto.Id));
            if (staff == null)
                return null;

            var operationalWindow = OperationalWindow.FromString(dto.OperationalWindow);

            staff.UpdateContactInfo(dto.Email, dto.Phone);
            staff.UpdateOperationalWindow(operationalWindow);

            await _unitOfWork.CommitAsync();

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }

        public async Task<OperatingStaffDto> AddQualificationAsync(
            OperatingStaffId id,
            string qualificationType
        )
        {
            var staff = await _repo.GetByIdAsync(id);
            if (staff == null)
                return null;

            var qualification = QualificationType.FromString(qualificationType);
            staff.AddQualification(qualification);

            await _unitOfWork.CommitAsync();

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }

        public async Task<OperatingStaffDto> RemoveQualificationAsync(
            OperatingStaffId id,
            string qualificationType
        )
        {
            var staff = await _repo.GetByIdAsync(id);
            if (staff == null)
                return null;

            var qualification = QualificationType.FromString(qualificationType);
            staff.RemoveQualification(qualification);

            await _unitOfWork.CommitAsync();

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }

        public async Task<OperatingStaffDto> UpdateStatusAsync(OperatingStaffId id, string status)
        {
            var staff = await _repo.GetByIdAsync(id);
            if (staff == null)
                return null;

            var staffStatus = StaffStatus.FromString(status);

            if (staffStatus.IsAvailable())
                staff.MarkAsAvailable();
            else if (staffStatus.IsUnavailable())
                staff.MarkAsUnavailable();
            else if (staffStatus.IsOnLeave())
                staff.MarkAsOnLeave();
            else if (staffStatus.IsInTraining())
                staff.MarkAsInTraining();

            await _unitOfWork.CommitAsync();

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
            };
        }

        public async Task<OperatingStaffDto> DeleteAsync(OperatingStaffId id)
        {
            var staff = await _repo.GetByIdAsync(id);
            if (staff == null)
                return null;

            _repo.Remove(staff);
            await _unitOfWork.CommitAsync();

            return new OperatingStaffDto
            {
                Id = staff.Id.AsGuid(),
                MecanographicNumber = staff.MecanographicNumber.Value,
                ShortName = staff.ShortName,
                Email = staff.Email,
                Phone = staff.Phone,
                OperationalWindow = staff.OperationalWindow.ToString(),
                Status = staff.Status.ToString(),
                Qualifications = staff.Qualifications.Select(q => q.ToString()).ToList(),
            };
        }
    }
}
