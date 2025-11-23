using System;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using Xunit;

namespace APDL.Tests.Unit.Domain
{
    public class ContainerUnitTests
    {
        #region Create Tests

        [Fact]
        public void Create_WithValidData_ShouldCreateContainer()
        {
            // Act
            var container = Container.Create(
                "MSCU1234567",
                "Dry Goods",
                "Electronics and appliances",
                "Handle with care",
                1,
                2,
                3
            );

            // Assert
            Assert.NotNull(container);
            Assert.NotNull(container.Id);
            Assert.Equal("MSCU1234567", container.ContainerNumber.Value);
            Assert.Equal("Dry Goods", container.CargoType.Value);
            Assert.Equal("Electronics and appliances", container.Description.Value);
            Assert.Equal("Handle with care", container.SpecialRequirements.Value);
            Assert.Equal(1, container.Position.Bay);
            Assert.Equal(2, container.Position.Row);
            Assert.Equal(3, container.Position.Tier);
        }

        [Fact]
        public void Create_WithEmptyContainerNumber_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("", "Dry Goods", "Test description", "None", 1, 2, 3)
            );
            Assert.Contains("ContainerNumber", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidContainerNumber_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("INVALID", "Dry Goods", "Test description", "None", 1, 2, 3)
            );
            Assert.Contains("ContainerNumber", ex.Message);
        }

        [Fact]
        public void Create_WithEmptyCargoType_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("MSCU1234567", "", "Test description", "None", 1, 2, 3)
            );
            Assert.Contains("Cargo Type cannot be empty", ex.Message);
        }

        [Fact]
        public void Create_WithEmptyDescription_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("MSCU1234567", "Dry Goods", "", "None", 1, 2, 3)
            );
            Assert.Contains("Description cannot be empty", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidBay_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("MSCU1234567", "Dry Goods", "Test description", "None", 0, 2, 3)
            );
            Assert.Contains("VesselPosition", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidRow_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("MSCU1234567", "Dry Goods", "Test description", "None", 1, 0, 3)
            );
            Assert.Contains("VesselPosition", ex.Message);
        }

        [Fact]
        public void Create_WithInvalidTier_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                Container.Create("MSCU1234567", "Dry Goods", "Test description", "None", 1, 2, 0)
            );
            Assert.Contains("VesselPosition", ex.Message);
        }

        #endregion

        #region UpdateCargoType Tests

        [Fact]
        public void UpdateCargoType_WithValidType_ShouldUpdate()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act
            container.UpdateCargoType("Refrigerated");

            // Assert
            Assert.Equal("Refrigerated", container.CargoType.Value);
        }

        [Fact]
        public void UpdateCargoType_WithEmptyType_ShouldThrowException()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                container.UpdateCargoType("")
            );
            Assert.Contains("Cargo Type cannot be empty", ex.Message);
        }

        #endregion

        #region UpdateDescription Tests

        [Fact]
        public void UpdateDescription_WithValidDescription_ShouldUpdate()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act
            container.UpdateDescription("Updated description");

            // Assert
            Assert.Equal("Updated description", container.Description.Value);
        }

        [Fact]
        public void UpdateDescription_WithEmptyDescription_ShouldThrowException()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                container.UpdateDescription("")
            );
            Assert.Contains("Description cannot be empty", ex.Message);
        }

        #endregion

        #region UpdateSpecialRequirements Tests

        [Fact]
        public void UpdateSpecialRequirements_WithValidRequirements_ShouldUpdate()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act
            container.UpdateSpecialRequirements("Temperature controlled");

            // Assert
            Assert.Equal("Temperature controlled", container.SpecialRequirements.Value);
        }

        [Fact]
        public void UpdateSpecialRequirements_WithNullRequirements_ShouldAccept()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act
            container.UpdateSpecialRequirements(null);

            // Assert
            Assert.Null(container.SpecialRequirements.Value);
        }

        #endregion

        #region UpdatePosition Tests

        [Fact]
        public void UpdatePosition_WithValidPosition_ShouldUpdate()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act
            container.UpdatePosition(5, 6, 7);

            // Assert
            Assert.Equal(5, container.Position.Bay);
            Assert.Equal(6, container.Position.Row);
            Assert.Equal(7, container.Position.Tier);
        }

        [Fact]
        public void UpdatePosition_WithInvalidBay_ShouldThrowException()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                container.UpdatePosition(0, 2, 3)
            );
            Assert.Contains("VesselPosition", ex.Message);
        }

        [Fact]
        public void UpdatePosition_WithInvalidRow_ShouldThrowException()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                container.UpdatePosition(1, -1, 3)
            );
            Assert.Contains("VesselPosition", ex.Message);
        }

        [Fact]
        public void UpdatePosition_WithInvalidTier_ShouldThrowException()
        {
            // Arrange
            var container = CreateValidContainer();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                container.UpdatePosition(1, 2, 0)
            );
            Assert.Contains("VesselPosition", ex.Message);
        }

        #endregion

        #region ContainerNumber Value Object Tests

        [Theory]
        [InlineData("MSCU1234567")]
        [InlineData("ABCU9876543")]
        [InlineData("XYZJ0001112")]
        public void ContainerNumber_WithValidFormat_ShouldCreate(string validNumber)
        {
            // Act
            var containerNumber = new ContainerNumber(validNumber);

            // Assert
            Assert.Equal(validNumber.ToUpperInvariant(), containerNumber.Value);
        }

        [Theory]
        [InlineData("ABC123")]
        [InlineData("TOOLONG12345")]
        [InlineData("SHORT")]
        [InlineData("123U1234567")]
        public void ContainerNumber_WithInvalidFormat_ShouldThrowException(string invalidNumber)
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
                new ContainerNumber(invalidNumber)
            );
        }

        [Fact]
        public void ContainerNumber_ShouldConvertToUpperCase()
        {
            // Act
            var containerNumber = new ContainerNumber("mscu1234567");

            // Assert
            Assert.Equal("MSCU1234567", containerNumber.Value);
        }

        #endregion

        #region VesselPosition Value Object Tests

        [Fact]
        public void VesselPosition_WithValidCoordinates_ShouldCreate()
        {
            // Act
            var position = new VesselPosition(10, 20, 30);

            // Assert
            Assert.Equal(10, position.Bay);
            Assert.Equal(20, position.Row);
            Assert.Equal(30, position.Tier);
        }

        [Fact]
        public void VesselPosition_Equals_WithSameValues_ShouldReturnTrue()
        {
            // Arrange
            var position1 = new VesselPosition(1, 2, 3);
            var position2 = new VesselPosition(1, 2, 3);

            // Act & Assert
            Assert.Equal(position1, position2);
        }

        [Fact]
        public void VesselPosition_Equals_WithDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var position1 = new VesselPosition(1, 2, 3);
            var position2 = new VesselPosition(4, 5, 6);

            // Act & Assert
            Assert.NotEqual(position1, position2);
        }

        [Fact]
        public void VesselPosition_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var position = new VesselPosition(1, 2, 3);

            // Act
            var result = position.ToString();

            // Assert
            Assert.Equal("Bay: 1, Row: 2, Tier: 3", result);
        }

        #endregion

        #region Helper Methods

        private Container CreateValidContainer()
        {
            return Container.Create(
                "MSCU1234567",
                "Dry Goods",
                "Electronics and appliances",
                "Handle with care",
                1,
                2,
                3
            );
        }

        #endregion
    }
}
