using System;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;

namespace APDL.API.Domain.ShippingAgentAggregate
{
    public class ShippingAgentRepresentative : Entity<ShippingAgentRepresentativeId>
    {
        public Name Name { get; private set; }
        public CitizenId CitizenId { get; private set; }
        public Nationality Nationality { get; private set; }
        public Email Email { get; private set; }
        public Phone Phone { get; private set; }
        public bool IsActive { get; private set; }

        private ShippingAgentRepresentative() { }

        public ShippingAgentRepresentative(
            Name name,
            CitizenId citizenId,
            Nationality nationality,
            Email email,
            Phone phone)
        {
            Name = name ?? throw new BusinessRuleValidationException(nameof(name), "Name must be provided.");
            CitizenId = citizenId ?? throw new BusinessRuleValidationException(nameof(citizenId), "Citizen id must be provided.");
            Nationality = nationality ?? throw new BusinessRuleValidationException(nameof(nationality), "Nationality must be provided.");
            Email = email ?? throw new BusinessRuleValidationException(nameof(email), "Email must be provided.");
            Phone = phone ?? throw new BusinessRuleValidationException(nameof(phone), "Phone must be provided.");
            IsActive = true;
        }

        public void UpdateName(Name name)
        {
            Name = name ?? throw new BusinessRuleValidationException(nameof(name), "Name must be provided.");
        }

        public void UpdateCitizenId(CitizenId citizenId)
        {
            CitizenId = citizenId ?? throw new BusinessRuleValidationException(nameof(citizenId), "Citizen id must be provided.");
        }

        public void UpdateNationality(Nationality nationality)
        {
            Nationality = nationality ?? throw new BusinessRuleValidationException(nameof(nationality), "Nationality must be provided.");
        }

        public void UpdateEmail(Email email)
        {
            Email = email ?? throw new BusinessRuleValidationException(nameof(email), "Email must be provided.");
        }

        public void UpdatePhone(Phone phone)
        {
            Phone = phone ?? throw new BusinessRuleValidationException(nameof(phone), "Phone must be provided.");
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Reactivate()
        {
            IsActive = true;
        }
    }
}
