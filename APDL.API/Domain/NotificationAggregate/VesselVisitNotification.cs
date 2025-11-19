using System;
using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;

namespace APDL.API.Domain.VesselVisitAggregate
{
    public class VesselVisitNotification : Entity<VesselVisitNotificationId>, IAggregateRoot
    {
        public VesselId VesselId { get; private set; }
        public ShippingAgentId ShippingAgentId { get; private set; }

        public ExpectedArrival ExpectedArrival { get; private set; }
        public ExpectedDeparture ExpectedDeparture { get; private set; }

        public string CargoType { get; private set; }
        public int CargoVolume { get; private set; }
        public string SpecialHandlingRequirements { get; private set; }

        public string CaptainName { get; private set; }
        public int TotalCrewCount { get; private set; }

        private readonly List<SafetyOfficer> _safetyCrewOfficers = new();
        public IReadOnlyCollection<SafetyOfficer> SafetyCrewOfficers =>
            _safetyCrewOfficers.AsReadOnly();

        private readonly List<CargoManifestId> _cargoManifestIds = new();
        public IReadOnlyCollection<CargoManifestId> CargoManifestIds =>
            _cargoManifestIds.AsReadOnly();

        public NotificationStatus Status { get; private set; }
        public DockId? AssignedDockId { get; private set; }
        public string RejectionReason { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? SubmittedAt { get; private set; }
        public DateTime? ReviewedAt { get; private set; }

        protected VesselVisitNotification() { }

        private VesselVisitNotification(
            VesselVisitNotificationId id,
            VesselId vesselId,
            ShippingAgentId shippingAgentId,
            ExpectedArrival expectedArrival,
            ExpectedDeparture expectedDeparture,
            string cargoType,
            int cargoVolume,
            string specialHandlingRequirements,
            string captainName,
            int totalCrewCount,
            List<SafetyOfficer> safetyCrewOfficers,
            NotificationStatus status
        )
        {
            Id = id;
            VesselId = vesselId;
            ShippingAgentId = shippingAgentId;
            ExpectedArrival = expectedArrival;
            ExpectedDeparture = expectedDeparture;
            CargoType = cargoType;
            CargoVolume = cargoVolume;
            SpecialHandlingRequirements = specialHandlingRequirements;
            CaptainName = captainName;
            TotalCrewCount = totalCrewCount;
            Status = status;
            CreatedAt = DateTime.UtcNow;

            if (safetyCrewOfficers != null && safetyCrewOfficers.Any())
            {
                _safetyCrewOfficers.AddRange(safetyCrewOfficers);
            }
        }

        public static VesselVisitNotification Create(
            VesselId vesselId,
            ShippingAgentId shippingAgentId,
            DateTime expectedArrival,
            DateTime expectedDeparture,
            string cargoType,
            int cargoVolume,
            string specialHandlingRequirements,
            string captainName,
            int totalCrewCount,
            List<string> safetyCrewOfficerNames = null
        )
        {
            if (vesselId == null)
                throw new BusinessRuleValidationException(
                    "Vessel ID is required.",
                    nameof(vesselId)
                );

            if (shippingAgentId == null)
                throw new BusinessRuleValidationException(
                    "Shipping Agent ID is required.",
                    nameof(shippingAgentId)
                );

            if (expectedDeparture <= expectedArrival)
                throw new BusinessRuleValidationException(
                    "Expected departure must be after expected arrival."
                );

            if (string.IsNullOrWhiteSpace(cargoType))
                throw new BusinessRuleValidationException(
                    "Cargo type is required.",
                    nameof(cargoType)
                );

            if (cargoVolume < 0)
                throw new BusinessRuleValidationException(
                    "Cargo volume cannot be negative.",
                    nameof(cargoVolume)
                );

            if (string.IsNullOrWhiteSpace(captainName))
                throw new BusinessRuleValidationException(
                    "Captain name is required.",
                    nameof(captainName)
                );

            if (totalCrewCount <= 0)
                throw new BusinessRuleValidationException(
                    "Total crew count must be at least 1.",
                    nameof(totalCrewCount)
                );

            var safetyOfficers = new List<SafetyOfficer>();
            if (safetyCrewOfficerNames != null && safetyCrewOfficerNames.Any())
            {
                safetyOfficers = safetyCrewOfficerNames
                    .Select(name => new SafetyOfficer(name))
                    .ToList();
            }

            return new VesselVisitNotification(
                new VesselVisitNotificationId(Guid.NewGuid()),
                vesselId,
                shippingAgentId,
                new ExpectedArrival(expectedArrival),
                new ExpectedDeparture(expectedDeparture),
                cargoType,
                cargoVolume,
                specialHandlingRequirements,
                captainName,
                totalCrewCount,
                safetyOfficers,
                NotificationStatus.InProgress
            );
        }

        public void UpdateCargoInformation(
            string cargoType,
            int cargoVolume,
            string specialHandlingRequirements
        )
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            if (string.IsNullOrWhiteSpace(cargoType))
                throw new BusinessRuleValidationException(
                    "Cargo type is required.",
                    nameof(cargoType)
                );

            if (cargoVolume < 0)
                throw new BusinessRuleValidationException(
                    "Cargo volume cannot be negative.",
                    nameof(cargoVolume)
                );

            CargoType = cargoType;
            CargoVolume = cargoVolume;
            SpecialHandlingRequirements = specialHandlingRequirements;
        }

        public void UpdateCrewInformation(string captainName, int totalCrewCount)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            if (string.IsNullOrWhiteSpace(captainName))
                throw new BusinessRuleValidationException(
                    "Captain name is required.",
                    nameof(captainName)
                );

            if (totalCrewCount <= 0)
                throw new BusinessRuleValidationException(
                    "Total crew count must be at least 1.",
                    nameof(totalCrewCount)
                );

            CaptainName = captainName;
            TotalCrewCount = totalCrewCount;
        }

        public void UpdateExpectedArrival(DateTime expectedArrival)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            if (ExpectedDeparture.Value <= expectedArrival)
            {
                throw new BusinessRuleValidationException(
                    nameof(expectedArrival),
                    "Expected departure must be after expected arrival."
                );
            }

            ExpectedArrival = new ExpectedArrival(expectedArrival);
        }

        public void UpdateExpectedDeparture(DateTime expectedDeparture)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            if (expectedDeparture <= ExpectedArrival.Value)
            {
                throw new BusinessRuleValidationException(
                    "Expected departure must be after expected arrival."
                );
            }

            ExpectedDeparture = new ExpectedDeparture(expectedDeparture);
        }

        public void UpdateSafetyCrew(List<string> safetyCrewOfficerNames)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            _safetyCrewOfficers.Clear();

            if (safetyCrewOfficerNames != null && safetyCrewOfficerNames.Any())
            {
                var officers = safetyCrewOfficerNames
                    .Select(name => new SafetyOfficer(name))
                    .ToList();

                _safetyCrewOfficers.AddRange(officers);
            }
        }

        public void AddSafetyOfficer(string officerName)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            var officer = new SafetyOfficer(officerName);

            if (
                _safetyCrewOfficers.Any(o =>
                    o.Name.Equals(officer.Name, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                throw new BusinessRuleValidationException(
                    "Safety officer is already in the crew list."
                );
            }

            _safetyCrewOfficers.Add(officer);
        }

        public void RemoveSafetyOfficer(string officerName)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            var officer = _safetyCrewOfficers.FirstOrDefault(o =>
                o.Name.Equals(officerName, StringComparison.OrdinalIgnoreCase)
            );

            if (officer == null)
            {
                throw new BusinessRuleValidationException("Safety officer not found in crew list.");
            }

            _safetyCrewOfficers.Remove(officer);
        }

        public void AttachCargoManifest(CargoManifestId manifestId)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            if (_cargoManifestIds.Count >= 2)
            {
                throw new BusinessRuleValidationException(
                    "Cannot attach more than 2 cargo manifests (unloading and loading)."
                );
            }

            if (_cargoManifestIds.Contains(manifestId))
            {
                throw new BusinessRuleValidationException("This manifest is already attached.");
            }

            _cargoManifestIds.Add(manifestId);
        }

        public void DetachCargoManifest(CargoManifestId manifestId)
        {
            if (
                Status == NotificationStatus.Approved
                || Status == NotificationStatus.Rejected
                || Status == NotificationStatus.Submitted
            )
            {
                throw new BusinessRuleValidationException(
                    "Cannot update notification with current status."
                );
            }

            if (!_cargoManifestIds.Remove(manifestId))
            {
                throw new BusinessRuleValidationException("Manifest not found in notification.");
            }
        }

        public void Submit()
        {
            if (!Status.Equals(NotificationStatus.InProgress))
            {
                throw new BusinessRuleValidationException(
                    "Only notifications in progress can be submitted."
                );
            }

            Status = NotificationStatus.Submitted;
            SubmittedAt = DateTime.UtcNow;
        }

        public void Approve(DockId assignedDockId)
        {
            if (!Status.Equals(NotificationStatus.Submitted))
            {
                throw new BusinessRuleValidationException(
                    "Only submitted notifications can be approved."
                );
            }

            if (assignedDockId == null)
            {
                throw new BusinessRuleValidationException(
                    "Dock must be assigned when approving.",
                    nameof(assignedDockId)
                );
            }

            Status = NotificationStatus.Approved;
            AssignedDockId = assignedDockId;
            ReviewedAt = DateTime.UtcNow;
        }

        public void Reject(string reason)
        {
            if (!Status.Equals(NotificationStatus.Submitted))
            {
                throw new BusinessRuleValidationException(
                    "Only submitted notifications can be rejected."
                );
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                throw new BusinessRuleValidationException(
                    "Rejection reason is required.",
                    nameof(reason)
                );
            }

            Status = NotificationStatus.Rejected;
            RejectionReason = reason;
            ReviewedAt = DateTime.UtcNow;
        }

        public void ReassignDock(DockId newDockId)
        {
            if (!Status.Equals(NotificationStatus.Approved))
            {
                throw new BusinessRuleValidationException(
                    "Can only reassign dock for approved notifications."
                );
            }

            if (newDockId == null)
            {
                throw new BusinessRuleValidationException(
                    "New dock ID is required.",
                    nameof(newDockId)
                );
            }

            AssignedDockId = newDockId;
        }
    }
}
