using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Dock;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.DTO;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;

namespace APDL.API.Domain.VesselVisitAggregate
{
    public class VesselVisitNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVesselVisitNotificationRepository _repo;

        public VesselVisitNotificationService(
            IUnitOfWork unitOfWork,
            IVesselVisitNotificationRepository repo
        )
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        public async Task<List<VesselVisitNotificationDto>> GetAllAsync()
        {
            var notifications = await _repo.GetAllAsync();

            return notifications
                .Select(notification => new VesselVisitNotificationDto
                {
                    Id = notification.Id.AsGuid(),
                    VesselId = notification.VesselId.AsGuid(),
                    ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                    ExpectedArrival = notification.ExpectedArrival.Value,
                    ExpectedDeparture = notification.ExpectedDeparture.Value,
                    CargoType = notification.CargoType,
                    CargoVolume = notification.CargoVolume,
                    SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                    CaptainName = notification.CaptainName,
                    TotalCrewCount = notification.TotalCrewCount,
                    SafetyCrewOfficers = notification
                        .SafetyCrewOfficers.Select(o => o.Name)
                        .ToList(),
                    Status = notification.Status.Value,
                    AssignedDockId = notification.AssignedDockId?.AsGuid(),
                    RejectionReason = notification.RejectionReason,
                    CreatedAt = notification.CreatedAt,
                    SubmittedAt = notification.SubmittedAt,
                    ReviewedAt = notification.ReviewedAt,
                })
                .ToList();
        }

        public async Task<VesselVisitNotificationDto> GetByIdAsync(VesselVisitNotificationId id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null)
                return null;

            return new VesselVisitNotificationDto
            {
                Id = notification.Id.AsGuid(),
                VesselId = notification.VesselId.AsGuid(),
                ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                ExpectedArrival = notification.ExpectedArrival.Value,
                ExpectedDeparture = notification.ExpectedDeparture.Value,
                CargoType = notification.CargoType,
                CargoVolume = notification.CargoVolume,
                SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                CaptainName = notification.CaptainName,
                TotalCrewCount = notification.TotalCrewCount,
                SafetyCrewOfficers = notification.SafetyCrewOfficers.Select(o => o.Name).ToList(),
                Status = notification.Status.Value,
                AssignedDockId = notification.AssignedDockId?.AsGuid(),
                RejectionReason = notification.RejectionReason,
                CreatedAt = notification.CreatedAt,
                SubmittedAt = notification.SubmittedAt,
                ReviewedAt = notification.ReviewedAt,
            };
        }

        public async Task<List<VesselVisitNotificationDto>> GetByStatusAsync(string status)
        {
            var notifications = await _repo.GetByStatusAsync(status);

            return notifications
                .Select(notification => new VesselVisitNotificationDto
                {
                    Id = notification.Id.AsGuid(),
                    VesselId = notification.VesselId.AsGuid(),
                    ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                    ExpectedArrival = notification.ExpectedArrival.Value,
                    ExpectedDeparture = notification.ExpectedDeparture.Value,
                    CargoType = notification.CargoType,
                    CargoVolume = notification.CargoVolume,
                    SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                    CaptainName = notification.CaptainName,
                    TotalCrewCount = notification.TotalCrewCount,
                    SafetyCrewOfficers = notification
                        .SafetyCrewOfficers.Select(o => o.Name)
                        .ToList(),
                    Status = notification.Status.Value,
                    AssignedDockId = notification.AssignedDockId?.AsGuid(),
                    RejectionReason = notification.RejectionReason,
                    CreatedAt = notification.CreatedAt,
                    SubmittedAt = notification.SubmittedAt,
                    ReviewedAt = notification.ReviewedAt,
                })
                .ToList();
        }

        public async Task<List<VesselVisitNotificationDto>> GetPendingNotificationsAsync()
        {
            var notifications = await _repo.GetPendingNotificationsAsync();

            return notifications
                .Select(notification => new VesselVisitNotificationDto
                {
                    Id = notification.Id.AsGuid(),
                    VesselId = notification.VesselId.AsGuid(),
                    ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                    ExpectedArrival = notification.ExpectedArrival.Value,
                    ExpectedDeparture = notification.ExpectedDeparture.Value,
                    CargoType = notification.CargoType,
                    CargoVolume = notification.CargoVolume,
                    SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                    CaptainName = notification.CaptainName,
                    TotalCrewCount = notification.TotalCrewCount,
                    SafetyCrewOfficers = notification
                        .SafetyCrewOfficers.Select(o => o.Name)
                        .ToList(),
                    Status = notification.Status.Value,
                    AssignedDockId = notification.AssignedDockId?.AsGuid(),
                    RejectionReason = notification.RejectionReason,
                    CreatedAt = notification.CreatedAt,
                    SubmittedAt = notification.SubmittedAt,
                    ReviewedAt = notification.ReviewedAt,
                })
                .ToList();
        }

        public async Task<VesselVisitNotificationDto> AddAsync(CreateVesselVisitNotificationDto dto)
        {
            var notification = VesselVisitNotification.Create(
                new VesselId(dto.VesselId),
                new ShippingAgentId(dto.ShippingAgentId),
                dto.ExpectedArrival,
                dto.ExpectedDeparture,
                dto.CargoType,
                dto.CargoVolume,
                dto.SpecialHandlingRequirements,
                dto.CaptainName,
                dto.TotalCrewCount,
                dto.SafetyCrewOfficers
            );

            await _repo.AddAsync(notification);
            await _unitOfWork.CommitAsync();

            return new VesselVisitNotificationDto
            {
                Id = notification.Id.AsGuid(),
                VesselId = notification.VesselId.AsGuid(),
                ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                ExpectedArrival = notification.ExpectedArrival.Value,
                ExpectedDeparture = notification.ExpectedDeparture.Value,
                CargoType = notification.CargoType,
                CargoVolume = notification.CargoVolume,
                SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                CaptainName = notification.CaptainName,
                TotalCrewCount = notification.TotalCrewCount,
                SafetyCrewOfficers = notification.SafetyCrewOfficers.Select(o => o.Name).ToList(),
                Status = notification.Status.Value,
                AssignedDockId = notification.AssignedDockId?.AsGuid(),
                RejectionReason = notification.RejectionReason,
                CreatedAt = notification.CreatedAt,
                SubmittedAt = notification.SubmittedAt,
                ReviewedAt = notification.ReviewedAt,
            };
        }

        public async Task<VesselVisitNotificationDto> UpdateAsync(
            UpdateVesselVisitNotificationDto dto
        )
        {
            var notification = await _repo.GetByIdAsync(new VesselVisitNotificationId(dto.Id));
            if (notification == null)
                return null;

            notification.UpdateExpectedArrival(dto.ExpectedArrival);
            notification.UpdateExpectedDeparture(dto.ExpectedDeparture);
            notification.UpdateCargoInformation(
                dto.CargoType,
                dto.CargoVolume,
                dto.SpecialHandlingRequirements
            );
            notification.UpdateCrewInformation(dto.CaptainName, dto.TotalCrewCount);
            notification.UpdateSafetyCrew(dto.SafetyCrewOfficers);

            await _unitOfWork.CommitAsync();

            return new VesselVisitNotificationDto
            {
                Id = notification.Id.AsGuid(),
                VesselId = notification.VesselId.AsGuid(),
                ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                ExpectedArrival = notification.ExpectedArrival.Value,
                ExpectedDeparture = notification.ExpectedDeparture.Value,
                CargoType = notification.CargoType,
                CargoVolume = notification.CargoVolume,
                SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                CaptainName = notification.CaptainName,
                TotalCrewCount = notification.TotalCrewCount,
                SafetyCrewOfficers = notification.SafetyCrewOfficers.Select(o => o.Name).ToList(),
                Status = notification.Status.Value,
                AssignedDockId = notification.AssignedDockId?.AsGuid(),
                RejectionReason = notification.RejectionReason,
                CreatedAt = notification.CreatedAt,
                SubmittedAt = notification.SubmittedAt,
                ReviewedAt = notification.ReviewedAt,
            };
        }

        public async Task SubmitAsync(Guid id)
        {
            var notification = await _repo.GetByIdAsync(new VesselVisitNotificationId(id));
            if (notification == null)
                throw new BusinessRuleValidationException(
                    "Notification",
                    $"Notification with ID {id} not found."
                );

            notification.Submit();
            await _unitOfWork.CommitAsync();
        }

        public async Task ApproveAsync(Guid id, Guid assignedDockId)
        {
            var notification = await _repo.GetByIdAsync(new VesselVisitNotificationId(id));
            if (notification == null)
                throw new BusinessRuleValidationException(
                    "Notification",
                    $"Notification with ID {id} not found."
                );

            notification.Approve(new DockId(assignedDockId));
            await _unitOfWork.CommitAsync();
        }

        public async Task RejectAsync(Guid id, string reason)
        {
            var notification = await _repo.GetByIdAsync(new VesselVisitNotificationId(id));
            if (notification == null)
                throw new BusinessRuleValidationException(
                    "Notification",
                    $"Notification with ID {id} not found."
                );

            notification.Reject(reason);
            await _unitOfWork.CommitAsync();
        }

        public async Task<VesselVisitNotificationDto> DeleteAsync(VesselVisitNotificationId id)
        {
            var notification = await _repo.GetByIdAsync(id);
            if (notification == null)
                return null;

            _repo.Remove(notification);
            await _unitOfWork.CommitAsync();

            return new VesselVisitNotificationDto
            {
                Id = notification.Id.AsGuid(),
                VesselId = notification.VesselId.AsGuid(),
                ShippingAgentId = notification.ShippingAgentId.AsGuid(),
                ExpectedArrival = notification.ExpectedArrival.Value,
                ExpectedDeparture = notification.ExpectedDeparture.Value,
                CargoType = notification.CargoType,
                CargoVolume = notification.CargoVolume,
                SpecialHandlingRequirements = notification.SpecialHandlingRequirements,
                CaptainName = notification.CaptainName,
                TotalCrewCount = notification.TotalCrewCount,
                SafetyCrewOfficers = notification.SafetyCrewOfficers.Select(o => o.Name).ToList(),
                Status = notification.Status.Value,
                AssignedDockId = notification.AssignedDockId?.AsGuid(),
                RejectionReason = notification.RejectionReason,
                CreatedAt = notification.CreatedAt,
                SubmittedAt = notification.SubmittedAt,
                ReviewedAt = notification.ReviewedAt,
            };
        }

        public async Task AddSafetyOfficerAsync(Guid notificationId, string officerName)
        {
            var notification = await _repo.GetByIdAsync(
                new VesselVisitNotificationId(notificationId)
            );
            if (notification == null)
                throw new BusinessRuleValidationException(
                    "Notification",
                    $"Notification with ID {notificationId} not found."
                );

            notification.AddSafetyOfficer(officerName);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveSafetyOfficerAsync(Guid notificationId, string officerName)
        {
            var notification = await _repo.GetByIdAsync(
                new VesselVisitNotificationId(notificationId)
            );
            if (notification == null)
                throw new BusinessRuleValidationException(
                    "Notification",
                    $"Notification with ID {notificationId} not found."
                );

            notification.RemoveSafetyOfficer(officerName);
            await _unitOfWork.CommitAsync();
        }
    }
}
