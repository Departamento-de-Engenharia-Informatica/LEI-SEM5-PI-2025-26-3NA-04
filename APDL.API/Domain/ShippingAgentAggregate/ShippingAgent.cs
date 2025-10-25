using System;
using System.Collections.Generic;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;
using System.Linq;

namespace APDL.API.Domain.ShippingAgentAggregate
{
    public class ShippingAgent : Entity<ShippingAgentId>, IAggregateRoot
    {
        public Name LegalName { get; private set; }
        public Name? AlternativeName { get; private set; }
        public Address Address { get; private set; }
        public TaxNumber TaxNumber { get; private set; }

        private readonly List<ShippingAgentRepresentative> _representatives = new();
        public IReadOnlyCollection<ShippingAgentRepresentative> Representatives => _representatives.AsReadOnly();

        private ShippingAgent() { }

        public ShippingAgent(
            string legalName,
            string? alternativeName,
            string address,
            string taxNumber,
            List<ShippingAgentRepresentative> representatives)
        {
            if (representatives == null || !representatives.Any())
                throw new BusinessRuleValidationException("At least one representative is required.");

            Id = new ShippingAgentId(Guid.NewGuid());
            LegalName = new Name(legalName);
            AlternativeName = string.IsNullOrWhiteSpace(alternativeName) ? null : new Name(alternativeName);
            Address = new Address(address);
            TaxNumber = new TaxNumber(taxNumber);

            _representatives.AddRange(representatives);
        }

        public void UpdateLegalName(string legalName)
        {
            LegalName = new Name(legalName);
        }

        public void UpdateAlternativeName(string alternativeName)
        {
            AlternativeName = new Name(alternativeName);
        }

        public void UpdateAddress(string address)
        {
            Address = new Address(address);
        }

        public void UpdateTaxNumber(string taxNumber)
        {
            TaxNumber = new TaxNumber(taxNumber);
        }

        // Representative management
        public void AddRepresentative(ShippingAgentRepresentative rep)
        {
            if (rep == null)
                throw new BusinessRuleValidationException(nameof(rep));

            if (!_representatives.Contains(rep))
                _representatives.Add(rep);
        }

        public void RemoveRepresentative(ShippingAgentRepresentative rep)
        {
            if (rep == null)
                throw new BusinessRuleValidationException("Provide a representative");

            if (_representatives.Count <= 1)
                throw new BusinessRuleValidationException("A shipping agent must have at least one representative.");

            _representatives.Remove(rep);
        }
    }
}