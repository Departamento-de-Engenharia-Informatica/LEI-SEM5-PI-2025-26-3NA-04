using System;
using System.Collections.Generic;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;
using Xunit;

namespace APDL.Tests.Unit.Domain
{
    public class ShippingAgentUnitTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidData_ShouldCreateShippingAgent()
        {
            // Arrange
            var representatives = new List<ShippingAgentRepresentative>
            {
                CreateValidRepresentative(),
            };

            // Act
            var agent = new ShippingAgent(
                "Maersk Line",
                "Maersk",
                "123 Harbor Street, Copenhagen",
                "123456789",
                representatives
            );

            // Assert
            Assert.NotNull(agent);
            Assert.NotNull(agent.Id);
            Assert.Equal("Maersk Line", agent.LegalName.Value);
            Assert.Equal("Maersk", agent.AlternativeName.Value);
            Assert.Equal("123 Harbor Street, Copenhagen", agent.Address.Value);
            Assert.Equal("123456789", agent.TaxNumber.Value);
            Assert.Single(agent.Representatives);
        }

        [Fact]
        public void Constructor_WithNullAlternativeName_ShouldCreateWithNullAlternativeName()
        {
            // Arrange
            var representatives = new List<ShippingAgentRepresentative>
            {
                CreateValidRepresentative(),
            };

            // Act
            var agent = new ShippingAgent(
                "MSC Mediterranean Shipping",
                null,
                "456 Port Avenue, Geneva",
                "987654321",
                representatives
            );

            // Assert
            Assert.NotNull(agent);
            Assert.Null(agent.AlternativeName);
        }

        [Fact]
        public void Constructor_WithEmptyAlternativeName_ShouldCreateWithNullAlternativeName()
        {
            // Arrange
            var representatives = new List<ShippingAgentRepresentative>
            {
                CreateValidRepresentative(),
            };

            // Act
            var agent = new ShippingAgent(
                "CMA CGM",
                "   ",
                "789 Ocean Road, Marseille",
                "111222333",
                representatives
            );

            // Assert
            Assert.NotNull(agent);
            Assert.Null(agent.AlternativeName);
        }

        [Fact]
        public void Constructor_WithNoRepresentatives_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgent(
                    "Test Company",
                    "Test",
                    "Test Address",
                    "123456789",
                    new List<ShippingAgentRepresentative>()
                )
            );
            Assert.Contains("At least one representative is required", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullRepresentatives_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgent("Test Company", "Test", "Test Address", "123456789", null)
            );
            Assert.Contains("At least one representative is required", ex.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void UpdateLegalName_WithValidName_ShouldUpdate()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act
            agent.UpdateLegalName("New Legal Name");

            // Assert
            Assert.Equal("New Legal Name", agent.LegalName.Value);
        }

        [Fact]
        public void UpdateAlternativeName_WithValidName_ShouldUpdate()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act
            agent.UpdateAlternativeName("New Alternative");

            // Assert
            Assert.Equal("New Alternative", agent.AlternativeName.Value);
        }

        [Fact]
        public void UpdateAddress_WithValidAddress_ShouldUpdate()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act
            agent.UpdateAddress("999 New Address Street");

            // Assert
            Assert.Equal("999 New Address Street", agent.Address.Value);
        }

        [Fact]
        public void UpdateTaxNumber_WithValidTaxNumber_ShouldUpdate()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act
            agent.UpdateTaxNumber("999888777");

            // Assert
            Assert.Equal("999888777", agent.TaxNumber.Value);
        }

        #endregion

        #region AddRepresentative Tests

        [Fact]
        public void AddRepresentative_WithValidRepresentative_ShouldAdd()
        {
            // Arrange
            var agent = CreateValidAgent();
            var newRep = CreateValidRepresentative("Jane Smith", "jane@example.com");

            // Act
            agent.AddRepresentative(newRep);

            // Assert
            Assert.Equal(2, agent.Representatives.Count);
        }

        [Fact]
        public void AddRepresentative_WithNullRepresentative_ShouldThrowException()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                agent.AddRepresentative(null)
            );
            Assert.Contains("Representative is empty", ex.Message);
        }

        [Fact]
        public void AddRepresentative_WithDuplicateRepresentative_ShouldThrowException()
        {
            // Arrange
            var rep = CreateValidRepresentative();
            var agent = new ShippingAgent(
                "Test Company",
                "Test",
                "Test Address",
                "123456789",
                new List<ShippingAgentRepresentative> { rep }
            );

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                agent.AddRepresentative(rep)
            );
            Assert.Contains("Representative already part of this organization", ex.Message);
        }

        #endregion

        #region RemoveRepresentative Tests

        [Fact]
        public void RemoveRepresentative_WithMultipleRepresentatives_ShouldRemove()
        {
            // Arrange
            var rep1 = CreateValidRepresentative("John Doe", "john@example.com");
            var rep2 = CreateValidRepresentative("Jane Smith", "jane@example.com");
            var agent = new ShippingAgent(
                "Test Company",
                "Test",
                "Test Address",
                "123456789",
                new List<ShippingAgentRepresentative> { rep1, rep2 }
            );

            // Act
            agent.RemoveRepresentative(rep1);

            // Assert
            Assert.Single(agent.Representatives);
        }

        [Fact]
        public void RemoveRepresentative_WithOnlyOneRepresentative_ShouldThrowException()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                agent.RemoveRepresentative(agent.Representatives.ToArray()[0])
            );
            Assert.Contains("A shipping agent must have at least one representative", ex.Message);
        }

        [Fact]
        public void RemoveRepresentative_WithNullRepresentative_ShouldThrowException()
        {
            // Arrange
            var agent = CreateValidAgent();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                agent.RemoveRepresentative(null)
            );
            Assert.Contains("Provide a representative", ex.Message);
        }

        #endregion

        #region Value Object Tests

        [Fact]
        public void TaxNumber_WithValidFormat_ShouldCreate()
        {
            // Act
            var taxNumber = new TaxNumber("123456789");

            // Assert
            Assert.Equal("123456789", taxNumber.Value);
        }

        [Theory]
        [InlineData("12345678")]
        [InlineData("1234567890")]
        [InlineData("ABC123456")]
        public void TaxNumber_WithInvalidFormat_ShouldThrowException(string invalid)
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new TaxNumber(invalid));
        }

        [Fact]
        public void Address_WithEmptyValue_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Address(""));
        }

        #endregion

        #region Helper Methods

        private ShippingAgent CreateValidAgent()
        {
            var representatives = new List<ShippingAgentRepresentative>
            {
                CreateValidRepresentative(),
            };

            return new ShippingAgent(
                "Test Shipping Company",
                "Test Co",
                "123 Test Street",
                "123456789",
                representatives
            );
        }

        private ShippingAgentRepresentative CreateValidRepresentative(
            string name = "John Doe",
            string email = "john.doe@example.com"
        )
        {
            return new ShippingAgentRepresentative(
                new Name(name),
                new CitizenId("12345678"),
                new Nationality("Portuguese"),
                new Email(email),
                new Phone("912345678")
            );
        }

        #endregion
    }
}
