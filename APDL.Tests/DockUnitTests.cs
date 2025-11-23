using System;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.Shared.ValueObjects;
using Xunit;

namespace APDL.Tests.Unit.Domain
{
    public class DockUnitTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidData_ShouldCreateDock()
        {
            // Act
            var dock = new Dock("Dock A", 350, 15);

            // Assert
            Assert.NotNull(dock);
            Assert.Equal("Dock A", dock.DockName);
            Assert.Equal(350, dock.DockLength);
            Assert.Equal(15, dock.DockDraft);
            Assert.Empty(dock.STSCranes);
        }

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() => new Dock("", 350, 15));
            Assert.Contains("Dock name cannot be empty", ex.Message);
        }

        [Fact]
        public void Constructor_WithZeroLength_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new Dock("Dock A", 0, 15)
            );
            Assert.Contains("Dock length must be greater than zero", ex.Message);
        }

        [Fact]
        public void Constructor_WithNegativeLength_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new Dock("Dock A", -100, 15)
            );
            Assert.Contains("Dock length must be greater than zero", ex.Message);
        }

        [Fact]
        public void Constructor_WithZeroDraft_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                new Dock("Dock A", 350, 0)
            );
            Assert.Contains("Dock draft must be greater than zero", ex.Message);
        }

        #endregion

        #region CanAccommodateVessel Tests

        [Fact]
        public void CanAccommodateVessel_WithSmallerVessel_ShouldReturnTrue()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act
            var result = dock.CanAccommodateVessel(300, 12);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanAccommodateVessel_WithExactSize_ShouldReturnTrue()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act
            var result = dock.CanAccommodateVessel(350, 15);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanAccommodateVessel_WithLongerVessel_ShouldReturnFalse()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act
            var result = dock.CanAccommodateVessel(400, 12);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanAccommodateVessel_WithDeeperDraft_ShouldReturnFalse()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act
            var result = dock.CanAccommodateVessel(300, 20);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CanAccommodateVessel_WithBothExceeding_ShouldReturnFalse()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act
            var result = dock.CanAccommodateVessel(400, 20);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region AddSTSCrane Tests

        [Fact]
        public void AddSTSCrane_WithValidCrane_ShouldAddToDock()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);
            var crane = CreateValidCrane(dock.Id);

            // Act
            dock.AddSTSCrane(crane);

            // Assert
            Assert.Single(dock.STSCranes);
            Assert.Equal(1, dock.NumberOfSTSCranes);
        }

        [Fact]
        public void AddSTSCrane_WithNullCrane_ShouldThrowException()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() => dock.AddSTSCrane(null));
            Assert.Contains("Crane cannot be null", ex.Message);
        }

        [Fact]
        public void AddSTSCrane_WithWrongDockId_ShouldThrowException()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);
            var wrongDockId = new DockId(Guid.NewGuid());
            var crane = CreateValidCrane(wrongDockId);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() => dock.AddSTSCrane(crane));
            Assert.Contains("Crane must belong to this dock", ex.Message);
        }

        [Fact]
        public void AddSTSCrane_MultiipleCranes_ShouldAddAll()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            var crane1 = CreateValidCrane(dock.Id);
            var crane2 = CreateValidCrane(dock.Id);

            // Act
            dock.AddSTSCrane(crane1);
            dock.AddSTSCrane(crane2);

            // Assert
            Assert.Equal(2, dock.NumberOfSTSCranes);
        }

        #endregion

        #region RemoveSTSCrane Tests

        [Fact]
        public void RemoveSTSCrane_WithExistingCrane_ShouldRemove()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);
            var crane = CreateValidCrane(dock.Id);
            dock.AddSTSCrane(crane);

            // Act
            dock.RemoveSTSCrane(crane.Id);

            // Assert
            Assert.Empty(dock.STSCranes);
            Assert.Equal(0, dock.NumberOfSTSCranes);
        }

        [Fact]
        public void RemoveSTSCrane_WithNonExistentCrane_ShouldThrowException()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);
            var craneId = new StsCraneId(Guid.NewGuid());

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                dock.RemoveSTSCrane(craneId)
            );
            Assert.Contains("Crane not found", ex.Message);
        }

        #endregion

        #region ScheduleMaintenance Tests

        [Fact]
        public void ScheduleMaintenance_WithValidSchedule_ShouldAdd()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);
            var schedule = new MaintenanceSchedule(
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                "Routine inspection",
                MaintenanceType.ROUTINE
            );

            // Act
            dock.ScheduleMaintenance(schedule);

            // Assert
            Assert.Single(dock.UpcomingMaintenances.Schedules);
        }

        [Fact]
        public void ScheduleMaintenance_WithNullSchedule_ShouldThrowException()
        {
            // Arrange
            var dock = new Dock("Dock A", 350, 15);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                dock.ScheduleMaintenance(null)
            );
            Assert.Contains("Maintenance schedule cannot be null", ex.Message);
        }

        #endregion

        #region Helper Methods

        private StsCrane CreateValidCrane(DockId dockId)
        {
            var operationalWindow = OperationalWindow.CreateWeekdays(
                TimeSpan.FromHours(8),
                TimeSpan.FromHours(18)
            );

            return new StsCrane(
                new StsCraneId(Guid.NewGuid()),
                "Crane Test",
                dockId,
                operationalWindow,
                30,
                TimeSpan.FromMinutes(15)
            );
        }

        #endregion
    }
}
