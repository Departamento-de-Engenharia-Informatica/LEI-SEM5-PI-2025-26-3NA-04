using System;
using Xunit;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.ValueObjects;

namespace APDL.Tests.Domain.ContainerAggregate
{
    public class ContainerDomainTests
    {
        [Fact]
        public void CreateContainer_WithValidData_ShouldSucceed()
        {
            // Act
            var container = Container.Create(
                containerNumber: "MSCU1234567",
                cargoType: "Reefer",
                description: "Refrigerated goods at -18C",
                specialRequirements: "Temperature controlled",
                bay: 10,
                row: 5,
                tier: 3
            );

            // Assert
            Assert.NotNull(container);
            Assert.Equal("MSCU1234567", container.ContainerNumber.Value);
            Assert.Equal("Reefer", container.CargoType.Value);
            Assert.Equal("Refrigerated goods at -18C", container.Description.Value);
            Assert.Equal("Temperature controlled", container.SpecialRequirements.Value);
            Assert.Equal(10, container.Position.Bay);
            Assert.Equal(5, container.Position.Row);
            Assert.Equal(3, container.Position.Tier);
        }

        [Fact]
        public void CreateContainer_WithEmptySpecialRequirements_ShouldSucceed()
        {
            // Act
            var container = Container.Create(
                "MSCU1234567",
                "General",
                "Consumer electronics",
                "",
                10, 5, 3
            );

            // Assert
            Assert.NotNull(container);
            Assert.Equal(string.Empty, container.SpecialRequirements.Value);
        }

        [Fact]
        public void UpdateCargoType_WithValidType_ShouldSucceed()
        {
            // Arrange
            var container = Container.Create("MSCU1234567", "General", "Test", "", 10, 5, 3);

            // Act
            container.UpdateCargoType("HAZMAT");

            // Assert
            Assert.Equal("HAZMAT", container.CargoType.Value);
        }

        [Fact]
        public void UpdateDescription_WithValidDescription_ShouldSucceed()
        {
            // Arrange
            var container = Container.Create("MSCU1234567", "General", "Old desc", "", 10, 5, 3);

            // Act
            container.UpdateDescription("New description");

            // Assert
            Assert.Equal("New description", container.Description.Value);
        }

        [Fact]
        public void UpdateSpecialRequirements_ShouldSucceed()
        {
            // Arrange
            var container = Container.Create("MSCU1234567", "Reefer", "Test", "", 10, 5, 3);

            // Act
            container.UpdateSpecialRequirements("Keep at -18C");

            // Assert
            Assert.Equal("Keep at -18C", container.SpecialRequirements.Value);
        }

        [Fact]
        public void UpdatePosition_WithValidPosition_ShouldSucceed()
        {
            // Arrange
            var container = Container.Create("MSCU1234567", "General", "Test", "", 10, 5, 3);

            // Act
            container.UpdatePosition(20, 10, 6);

            // Assert
            Assert.Equal(20, container.Position.Bay);
            Assert.Equal(10, container.Position.Row);
            Assert.Equal(6, container.Position.Tier);
        }

        [Fact]
        public void UpdatePosition_WithInvalidBay_ShouldThrow()
        {
            // Arrange
            var container = Container.Create("MSCU1234567", "General", "Test", "", 10, 5, 3);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
            {
                container.UpdatePosition(0, 10, 6); // Bay must be >= 1
            });
        }

        #region Value Object Tests

        [Fact]
        public void ContainerNumber_WithValidNumber_ShouldSucceed()
        {
            // Act
            var containerNumber = new ContainerNumber("MSCU1234567");

            // Assert
            Assert.Equal("MSCU1234567", containerNumber.Value);
        }

        [Fact]
        public void ContainerNumber_ShouldConvertToUpperCase()
        {
            // Act
            var containerNumber = new ContainerNumber("mscu1234567");

            // Assert
            Assert.Equal("MSCU1234567", containerNumber.Value);
        }

        [Fact]
        public void ContainerNumber_TooShort_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
            {
                new ContainerNumber("ABC");
            });
        }

        [Fact]
        public void ContainerNumber_TooLong_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
            {
                new ContainerNumber("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            });
        }

        [Fact]
        public void Empty_CargoType_ShouldThrow()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                new CargoType("");
            });

            Assert.Contains("Cargo Type cannot be empty.", ex.Message);
        }

        [Fact]
        public void VesselPosition_WithValidValues_ShouldSucceed()
        {
            // Act
            var position = new VesselPosition(10, 20, 30);

            // Assert
            Assert.Equal(10, position.Bay);
            Assert.Equal(20, position.Row);
            Assert.Equal(30, position.Tier);
        }

        [Theory]
        [InlineData(0, 20, 30)]    // Bay too low
        [InlineData(10, 0, 30)]    // Row too low
        [InlineData(10, 20, 0)]    // Tier too low
        public void VesselPosition_WithInvalidValues_ShouldThrow(int bay, int row, int tier)
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
            {
                new VesselPosition(bay, row, tier);
            });
        }

        #endregion
    }
}