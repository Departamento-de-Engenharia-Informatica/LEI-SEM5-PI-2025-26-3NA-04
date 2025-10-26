using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;

namespace APDL.Tests
{
    public class ShippingAgentDomainTests
    {
        private ShippingAgentRepresentative CreateRep(string name = "Daniel")
        {
            return new ShippingAgentRepresentative(
                new Name(name),
                new CitizenId("123456789"),
                new Nationality("PT"),
                new Email("daniel@test.com"),
                new Phone("912345678")
            );
        }

        [Fact]
        public void CreatingShippingAgent_WithoutRepresentatives_ShouldThrow()
        {
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                var agent = new ShippingAgent("LegalName", null, "Address", "123456789", new List<ShippingAgentRepresentative>());
            });

            Assert.Contains("At least one representative", ex.Message);
        }

        [Fact]
        public void CreatingShippingAgent_WithRepresentative_ShouldSucceed()
        {
            var rep = CreateRep();
            var agent = new ShippingAgent("LegalName", "AltName", "Address", "123456789", new List<ShippingAgentRepresentative> { rep });

            Assert.NotNull(agent);
            Assert.Single(agent.Representatives);
            Assert.Equal("Daniel", agent.Representatives.First().Name.Value);
        }

        [Fact]
        public void AddingDuplicateRepresentative_ShouldThrowException()
        {
            var rep = CreateRep();
            var agent = new ShippingAgent("LegalName", null, "Address", "123456789", new List<ShippingAgentRepresentative> { rep });

            var ex = Assert.Throws<BusinessRuleValidationException>(() => agent.AddRepresentative(rep));
            Assert.Contains("Representative already part of this organization", ex.Message);
        } 

        [Fact]
        public void RemovingLastRepresentative_ShouldThrow()
        {
            var rep = CreateRep();
            var agent = new ShippingAgent("LegalName", null, "Address", "123456789", new List<ShippingAgentRepresentative> { rep });

            var ex = Assert.Throws<BusinessRuleValidationException>(() => agent.RemoveRepresentative(rep));
            Assert.Contains("must have at least one representative", ex.Message);
        }

        [Fact]
        public void DeactivateAndReactivateRepresentative_ShouldWork()
        {
            var rep = CreateRep();

            rep.Deactivate();
            Assert.False(rep.IsActive);

            rep.Reactivate();
            Assert.True(rep.IsActive);
        }

        [Fact]
        public void UpdatingRepresentativeDetails_ShouldWork()
        {
            var rep = CreateRep();

            rep.UpdateName(new Name("Tobias"));
            rep.UpdateCitizenId(new CitizenId("987654321"));
            rep.UpdateNationality(new Nationality("ES"));
            rep.UpdateEmail(new Email("Tobias@test.com"));
            rep.UpdatePhone(new Phone("987654321"));

            Assert.Equal("Tobias", rep.Name.Value);
            Assert.Equal("987654321", rep.CitizenId.Value);
            Assert.Equal("ES", rep.Nationality.Value);
            Assert.Equal("Tobias@test.com", rep.Email.Value);
            Assert.Equal("987654321", rep.Phone.Value);
        }
    }
}
