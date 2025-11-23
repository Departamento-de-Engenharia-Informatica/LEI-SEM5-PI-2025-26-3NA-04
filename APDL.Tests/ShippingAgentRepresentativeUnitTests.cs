using System;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.ShippingAgentAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using Xunit;

namespace APDL.Tests.Unit.Domain
{
    public class ShippingAgentRepresentativeUnitTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidData_ShouldCreateRepresentative()
        {
            // Act
            var rep = new ShippingAgentRepresentative(
                new Name("John Doe"),
                new CitizenId("12345678"),
                new Nationality("Portuguese"),
                new Email("john@example.com"),
                new Phone("912345678")
            );

            // Assert
            Assert.NotNull(rep);
            Assert.NotNull(rep.Id);
            Assert.Equal("John Doe", rep.Name.Value);
            Assert.Equal("12345678", rep.CitizenId.Value);
            Assert.Equal("Portuguese", rep.Nationality.Value);
            Assert.Equal("john@example.com", rep.Email.Value);
            Assert.Equal("912345678", rep.Phone.Value);
            Assert.True(rep.IsActive);
        }

        [Fact]
        public void Constructor_WithNullName_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgentRepresentative(
                    null,
                    new CitizenId("12345678"),
                    new Nationality("Portuguese"),
                    new Email("john@example.com"),
                    new Phone("912345678")
                )
            );
            Assert.Contains("name", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullCitizenId_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgentRepresentative(
                    new Name("John Doe"),
                    null,
                    new Nationality("Portuguese"),
                    new Email("john@example.com"),
                    new Phone("912345678")
                )
            );
            Assert.Contains("citizenId", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullNationality_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgentRepresentative(
                    new Name("John Doe"),
                    new CitizenId("12345678"),
                    null,
                    new Email("john@example.com"),
                    new Phone("912345678")
                )
            );
            Assert.Contains("nationality", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullEmail_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgentRepresentative(
                    new Name("John Doe"),
                    new CitizenId("12345678"),
                    new Nationality("Portuguese"),
                    null,
                    new Phone("912345678")
                )
            );
            Assert.Contains("email", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullPhone_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new ShippingAgentRepresentative(
                    new Name("John Doe"),
                    new CitizenId("12345678"),
                    new Nationality("Portuguese"),
                    new Email("john@example.com"),
                    null
                )
            );
            Assert.Contains("phone", ex.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdate()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act
            rep.UpdateName(new Name("Jane Smith"));

            // Assert
            Assert.Equal("Jane Smith", rep.Name.Value);
        }

        [Fact]
        public void UpdateName_WithNull_ShouldThrowException()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                rep.UpdateName(null)
            );
            Assert.Contains("name", ex.Message);
        }

        [Fact]
        public void UpdateCitizenId_WithValidId_ShouldUpdate()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act
            rep.UpdateCitizenId(new CitizenId("87654321"));

            // Assert
            Assert.Equal("87654321", rep.CitizenId.Value);
        }

        [Fact]
        public void UpdateCitizenId_WithNull_ShouldThrowException()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                rep.UpdateCitizenId(null)
            );
            Assert.Contains("citizenId", ex.Message);
        }

        [Fact]
        public void UpdateNationality_WithValidNationality_ShouldUpdate()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act
            rep.UpdateNationality(new Nationality("Spanish"));

            // Assert
            Assert.Equal("Spanish", rep.Nationality.Value);
        }

        [Fact]
        public void UpdateNationality_WithNull_ShouldThrowException()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                rep.UpdateNationality(null)
            );
            Assert.Contains("nationality", ex.Message);
        }

        [Fact]
        public void UpdateEmail_WithValidEmail_ShouldUpdate()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act
            rep.UpdateEmail(new Email("newemail@example.com"));

            // Assert
            Assert.Equal("newemail@example.com", rep.Email.Value);
        }

        [Fact]
        public void UpdateEmail_WithNull_ShouldThrowException()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                rep.UpdateEmail(null)
            );
            Assert.Contains("email", ex.Message);
        }

        [Fact]
        public void UpdatePhone_WithValidPhone_ShouldUpdate()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act
            rep.UpdatePhone(new Phone("987654321"));

            // Assert
            Assert.Equal("987654321", rep.Phone.Value);
        }

        [Fact]
        public void UpdatePhone_WithNull_ShouldThrowException()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                rep.UpdatePhone(null)
            );
            Assert.Contains("phone", ex.Message);
        }

        #endregion

        #region Activation Tests

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var rep = CreateValidRepresentative();

            // Act
            rep.Deactivate();

            // Assert
            Assert.False(rep.IsActive);
        }

        [Fact]
        public void Reactivate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var rep = CreateValidRepresentative();
            rep.Deactivate();

            // Act
            rep.Reactivate();

            // Assert
            Assert.True(rep.IsActive);
        }

        #endregion

        #region Value Object Tests

        [Fact]
        public void CitizenId_WithValidId_ShouldCreate()
        {
            // Act
            var citizenId = new CitizenId("12345678");

            // Assert
            Assert.Equal("12345678", citizenId.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("1234567")]
        public void CitizenId_WithInvalidId_ShouldThrowException(string invalid)
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
                new CitizenId(invalid)
            );
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("first+last@company.org")]
        public void Email_WithValidFormat_ShouldCreate(string validEmail)
        {
            // Act
            var email = new Email(validEmail);

            // Assert
            Assert.Equal(validEmail, email.Value);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test @example.com")]
        public void Email_WithInvalidFormat_ShouldThrowException(string invalidEmail)
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
                new Email(invalidEmail)
            );
        }

        [Theory]
        [InlineData("912345678")]
        [InlineData("987654321")]
        [InlineData("900000000")]
        public void Phone_WithValidFormat_ShouldCreate(string validPhone)
        {
            // Act
            var phone = new Phone(validPhone);

            // Assert
            Assert.Equal(validPhone, phone.Value);
        }

        [Theory]
        [InlineData("812345678")]
        [InlineData("9123456789")]
        [InlineData("91234567")]
        [InlineData("abc123456")]
        public void Phone_WithInvalidFormat_ShouldThrowException(string invalidPhone)
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
                new Phone(invalidPhone)
            );
        }

        [Fact]
        public void Name_WithEmptyValue_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
                new Name("")
            );
        }

        [Fact]
        public void Nationality_WithEmptyValue_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
                new Nationality("")
            );
        }

        #endregion

        #region Helper Methods

        private ShippingAgentRepresentative CreateValidRepresentative()
        {
            return new ShippingAgentRepresentative(
                new Name("John Doe"),
                new CitizenId("12345678"),
                new Nationality("Portuguese"),
                new Email("john@example.com"),
                new Phone("912345678")
            );
        }

        #endregion
    }
}