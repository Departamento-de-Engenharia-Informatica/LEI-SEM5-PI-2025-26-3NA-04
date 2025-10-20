using APDL.API.Domain.Shared;

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
                throw new BusinessRuleValidationException(nameof(representatives), "At least one representative is required.");

            LegalName = new Name(legalName);
            AlternativeName = string.IsNullOrWhiteSpace(alternativeName) ? null : new Name(alternativeName);
            Address = new Address(address);
            TaxNumber = new TaxNumber(taxNumber);

            _representatives.AddRange(representatives);
        }

        public void UpdateLegalName(string legalName)
        {
            if (string.IsNullOrWhiteSpace(legalName))
                throw new ArgumentException("LegalName cannot be empty.", nameof(legalName));
            LegalName = legalName;
        }

        public void UpdateAlternativeName(string alternativeName)
        {
            AlternativeName = alternativeName ?? string.Empty;
        }

        public void UpdateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty.", nameof(address));
            Address = address;
        }

        public void UpdateTaxNumber(string taxNumber)
        {
            if (string.IsNullOrWhiteSpace(taxNumber))
                throw new ArgumentException("TaxNumber cannot be empty.", nameof(taxNumber));
            TaxNumber = taxNumber;
        }

        // Representative management
        public void AddRepresentative(ShippingAgentRepresentative rep)
        {
            if (rep == null)
                throw new ArgumentNullException(nameof(rep));

            if (!_representatives.Contains(rep))
                _representatives.Add(rep);
        }

        public void RemoveRepresentative(ShippingAgentRepresentative rep)
        {
            if (rep == null)
                throw new ArgumentNullException(nameof(rep));

            if (_representatives.Count <= 1)
                throw new InvalidOperationException("A shipping agent must have at least one representative.");

            _representatives.Remove(rep);
        }
    }
}