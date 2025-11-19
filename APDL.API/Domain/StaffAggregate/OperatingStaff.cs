using System;
using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.OperatingStaffAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;
using APDL.API.Domain.StaffAggregate; // For OperationalWindow

namespace APDL.API.Domain.OperatingStaffAggregate
{
    public class OperatingStaff : Entity<OperatingStaffId>, IAggregateRoot
    {
        public MecanographicNumber MecanographicNumber { get; private set; }
        public string ShortName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }

        public OperationalWindow OperationalWindow { get; private set; }
        public StaffStatus Status { get; private set; }

        private List<QualificationType> _qualifications;
        public IReadOnlyList<QualificationType> Qualifications => _qualifications.AsReadOnly();

        protected OperatingStaff()
        {
            _qualifications = new List<QualificationType>();
        }

        public OperatingStaff(
            OperatingStaffId id,
            MecanographicNumber mecanographicNumber,
            string shortName,
            string email,
            string phone,
            OperationalWindow operationalWindow
        )
        {
            if (mecanographicNumber == null)
                throw new BusinessRuleValidationException(
                    "Mecanographic number cannot be null.",
                    nameof(mecanographicNumber)
                );

            if (string.IsNullOrWhiteSpace(shortName))
                throw new BusinessRuleValidationException(
                    "Short name cannot be empty.",
                    nameof(shortName)
                );

            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleValidationException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new BusinessRuleValidationException("Phone cannot be empty.", nameof(phone));

            if (operationalWindow == null)
                throw new BusinessRuleValidationException(
                    "Operational window cannot be null.",
                    nameof(operationalWindow)
                );

            this.Id = id;
            this.MecanographicNumber = mecanographicNumber;
            this.ShortName = shortName;
            this.Email = email;
            this.Phone = phone;
            this.OperationalWindow = operationalWindow;
            this.Status = StaffStatus.Available;
            this._qualifications = new List<QualificationType>();
        }

        public void AddQualification(QualificationType qualification)
        {
            if (qualification == null)
                throw new BusinessRuleValidationException(
                    "Qualification cannot be null.",
                    nameof(qualification)
                );

            if (_qualifications.Any(q => q.Equals(qualification)))
                throw new BusinessRuleValidationException(
                    "Staff member already has this qualification.",
                    nameof(qualification)
                );

            _qualifications.Add(qualification);
        }

        public void RemoveQualification(QualificationType qualification)
        {
            if (qualification == null)
                throw new BusinessRuleValidationException(
                    "Qualification cannot be null.",
                    nameof(qualification)
                );

            var existing = _qualifications.FirstOrDefault(q => q.Equals(qualification));
            if (existing == null)
                throw new BusinessRuleValidationException(
                    "Staff member does not have this qualification.",
                    nameof(qualification)
                );

            _qualifications.Remove(existing);
        }

        public bool HasQualification(QualificationType qualification)
        {
            return _qualifications.Any(q => q.Equals(qualification));
        }

        public void MarkAsAvailable()
        {
            this.Status = StaffStatus.Available;
        }

        public void MarkAsUnavailable()
        {
            this.Status = StaffStatus.Unavailable;
        }

        public void MarkAsOnLeave()
        {
            this.Status = StaffStatus.OnLeave;
        }

        public void MarkAsInTraining()
        {
            this.Status = StaffStatus.InTraining;
        }

        public void UpdateContactInfo(string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleValidationException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new BusinessRuleValidationException("Phone cannot be empty.", nameof(phone));

            this.Email = email;
            this.Phone = phone;
        }

        public void UpdateOperationalWindow(OperationalWindow newWindow)
        {
            if (newWindow == null)
                throw new BusinessRuleValidationException(
                    "Operational window cannot be null.",
                    nameof(newWindow)
                );

            this.OperationalWindow = newWindow;
        }

        public bool IsAvailableAt(DateTime dateTime)
        {
            return Status.IsAvailable() && OperationalWindow.IsAvailableAt(dateTime);
        }

        public bool IsAvailableForDuration(DateTime start, TimeSpan duration)
        {
            return Status.IsAvailable()
                && OperationalWindow.IsAvailableForDuration(start, duration);
        }
    }
}
