using System;
using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.Shared;
using APDL.API.Domain.NotificationAggregate.ValueObjects;
using APDL.API.Domain.NotificationAggregate;

namespace APDL.API.Domain.VesselVisitAggregate
{
    public class VesselVisitNotification : Entity<VesselVisitNotificationId>, IAggregateRoot
    {
        public ExpectedArrival ExpectedArrival { get; private set; }
        public ExpectedDeparture ExpectedDeparture { get; private set; }
        public NotificationStatus Status { get; private set; }
        
        private readonly List<SafetyOfficer> _safetyCrewOfficers = new();
        public IReadOnlyCollection<SafetyOfficer> SafetyCrewOfficers => _safetyCrewOfficers.AsReadOnly();

        protected VesselVisitNotification() { }

        private VesselVisitNotification(
            VesselVisitNotificationId id,
            ExpectedArrival expectedArrival,
            ExpectedDeparture expectedDeparture,
            List<SafetyOfficer> safetyCrewOfficers,
            NotificationStatus status)
        {
            Id = id;
            ExpectedArrival = expectedArrival;
            ExpectedDeparture = expectedDeparture;
            Status = status;
            
            if (safetyCrewOfficers != null && safetyCrewOfficers.Any())
            {
                _safetyCrewOfficers.AddRange(safetyCrewOfficers);
            }
        }

        public static VesselVisitNotification Create(
            DateTime expectedArrival,
            DateTime expectedDeparture,
            List<string> safetyCrewOfficerNames = null)
        {
            if (expectedDeparture <= expectedArrival)
            {
                throw new BusinessRuleValidationException("Expected departure must be after expected arrival.");
            }

            var safetyOfficers = new List<SafetyOfficer>();
            if (safetyCrewOfficerNames != null && safetyCrewOfficerNames.Any())
            {
                safetyOfficers = safetyCrewOfficerNames
                    .Select(name => new SafetyOfficer(name))
                    .ToList();
            }

            return new VesselVisitNotification(
                new VesselVisitNotificationId(Guid.NewGuid()),
                new ExpectedArrival(expectedArrival),
                new ExpectedDeparture(expectedDeparture),
                safetyOfficers,
                NotificationStatus.InProgress
            );
        }

        public void UpdateExpectedArrival(DateTime expectedArrival)
        {
            if (Status == NotificationStatus.Approved || Status == NotificationStatus.Rejected || Status == NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Cannot update notification with current status.");
            }

            if (ExpectedDeparture.Value <= expectedArrival)
            {
                throw new BusinessRuleValidationException(
                    nameof(expectedArrival), "Expected departure must be after expected arrival.");
            }

            ExpectedArrival = new ExpectedArrival(expectedArrival);
        }

        public void UpdateExpectedDeparture(DateTime expectedDeparture)
        {
            if (Status == NotificationStatus.Approved || Status == NotificationStatus.Rejected || Status == NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Cannot update notification with current status.");
            }

            if (expectedDeparture <= ExpectedArrival.Value)
            {
                throw new BusinessRuleValidationException("Expected departure must be after expected arrival.");
            }

            ExpectedDeparture = new ExpectedDeparture(expectedDeparture);
        }

        public void UpdateSafetyCrew(List<string> safetyCrewOfficerNames)
        {
            if (Status == NotificationStatus.Approved || Status == NotificationStatus.Rejected || Status == NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Cannot update notification with current status.");
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
            if (Status == NotificationStatus.Approved || Status == NotificationStatus.Rejected || Status == NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Cannot update notification with current status.");
            }

            var officer = new SafetyOfficer(officerName);
            
            if (_safetyCrewOfficers.Any(o => o.Name.Equals(officer.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessRuleValidationException( "Safety officer is already in the crew list.");
            }

            _safetyCrewOfficers.Add(officer);
        }

        public void RemoveSafetyOfficer(string officerName)
        {
            if (Status == NotificationStatus.Approved || Status == NotificationStatus.Rejected || Status == NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Cannot update notification with current status.");
            }

            var officer = _safetyCrewOfficers.FirstOrDefault(o => 
                o.Name.Equals(officerName, StringComparison.OrdinalIgnoreCase));

            if (officer == null)
            {
                throw new BusinessRuleValidationException("Safety officer not found in crew list.");
            }

            _safetyCrewOfficers.Remove(officer);
        }

        public void Submit()
        {
            if (Status != NotificationStatus.InProgress)
            {
                throw new BusinessRuleValidationException("Only notifications in progress can be submitted.");
            }

            Status = NotificationStatus.Submitted;
        }

        public void Approve()
        {
            if (Status != NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Only submitted notifications can be approved.");
            }

            Status = NotificationStatus.Approved;
        }

        public void Reject()
        {
            if (Status != NotificationStatus.Submitted)
            {
                throw new BusinessRuleValidationException("Only submitted notifications can be rejected.");
            }

            Status = NotificationStatus.Rejected;
        }
    }
}