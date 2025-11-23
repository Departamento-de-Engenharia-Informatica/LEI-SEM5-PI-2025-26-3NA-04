using System;
using System.Collections.Generic;
using System.Linq;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.ValueObjects;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselVisitAggregate;
using Xunit;

namespace APDL.Tests.Unit.Domain
{
    public class VesselVisitNotificationUnitTests
    {
        #region Create Tests

        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            var vesselId = new VesselId(Guid.NewGuid());
            var agentId = new ShippingAgentId(Guid.NewGuid());
            var arrival = DateTime.UtcNow.AddDays(1);
            var departure = DateTime.UtcNow.AddDays(3);

            // Act
            var notification = VesselVisitNotification.Create(
                vesselId,
                agentId,
                arrival,
                departure,
                "Container Cargo",
                1000,
                "Handle with care",
                "Captain Smith",
                25,
                new List<string> { "Officer A", "Officer B" }
            );

            // Assert
            Assert.NotNull(notification);
            Assert.Equal(vesselId, notification.VesselId);
            Assert.Equal(agentId, notification.ShippingAgentId);
            Assert.Equal("Container Cargo", notification.CargoType);
            Assert.Equal(1000, notification.CargoVolume);
            Assert.Equal("Captain Smith", notification.CaptainName);
            Assert.Equal(25, notification.TotalCrewCount);
            Assert.Equal(2, notification.SafetyCrewOfficers.Count);
            Assert.Equal("InProgress", notification.Status.Value);
        }

        [Fact]
        public void Create_WithNullVesselId_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                VesselVisitNotification.Create(
                    null,
                    new ShippingAgentId(Guid.NewGuid()),
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(3),
                    "Cargo",
                    100,
                    null,
                    "Captain",
                    10
                )
            );
            Assert.Contains("Vessel ID is required", ex.Message);
        }

        [Fact]
        public void Create_WithNullShippingAgentId_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                VesselVisitNotification.Create(
                    new VesselId(Guid.NewGuid()),
                    null,
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(3),
                    "Cargo",
                    100,
                    null,
                    "Captain",
                    10
                )
            );
            Assert.Contains("Shipping Agent ID is required", ex.Message);
        }

        [Fact]
        public void Create_WithDepartureBeforeArrival_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                VesselVisitNotification.Create(
                    new VesselId(Guid.NewGuid()),
                    new ShippingAgentId(Guid.NewGuid()),
                    DateTime.UtcNow.AddDays(3),
                    DateTime.UtcNow.AddDays(1),
                    "Cargo",
                    100,
                    null,
                    "Captain",
                    10
                )
            );
            Assert.Contains("Expected departure must be after expected arrival", ex.Message);
        }

        [Fact]
        public void Create_WithEmptyCargoType_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                VesselVisitNotification.Create(
                    new VesselId(Guid.NewGuid()),
                    new ShippingAgentId(Guid.NewGuid()),
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(3),
                    "",
                    100,
                    null,
                    "Captain",
                    10
                )
            );
            Assert.Contains("Cargo type is required", ex.Message);
        }

        [Fact]
        public void Create_WithNegativeCargoVolume_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                VesselVisitNotification.Create(
                    new VesselId(Guid.NewGuid()),
                    new ShippingAgentId(Guid.NewGuid()),
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(3),
                    "Cargo",
                    -100,
                    null,
                    "Captain",
                    10
                )
            );
            Assert.Contains("Cargo volume cannot be negative", ex.Message);
        }

        [Fact]
        public void Create_WithZeroCrewCount_ShouldThrowException()
        {
            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                VesselVisitNotification.Create(
                    new VesselId(Guid.NewGuid()),
                    new ShippingAgentId(Guid.NewGuid()),
                    DateTime.UtcNow.AddDays(1),
                    DateTime.UtcNow.AddDays(3),
                    "Cargo",
                    100,
                    null,
                    "Captain",
                    0
                )
            );
            Assert.Contains("Total crew count must be at least 1", ex.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void UpdateCargoInformation_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act
            notification.UpdateCargoInformation("Bulk Cargo", 2000, "Fragile");

            // Assert
            Assert.Equal("Bulk Cargo", notification.CargoType);
            Assert.Equal(2000, notification.CargoVolume);
            Assert.Equal("Fragile", notification.SpecialHandlingRequirements);
        }

        [Fact]
        public void UpdateCargoInformation_WhenSubmitted_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.Submit();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.UpdateCargoInformation("Cargo", 100, null)
            );
            Assert.Contains("Cannot update notification with current status", ex.Message);
        }

        [Fact]
        public void UpdateCrewInformation_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act
            notification.UpdateCrewInformation("Captain Jones", 30);

            // Assert
            Assert.Equal("Captain Jones", notification.CaptainName);
            Assert.Equal(30, notification.TotalCrewCount);
        }

        [Fact]
        public void UpdateExpectedArrival_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            var newArrival = DateTime.UtcNow.AddDays(2);

            // Act
            notification.UpdateExpectedArrival(newArrival);

            // Assert
            Assert.Equal(newArrival, notification.ExpectedArrival.Value);
        }

        [Fact]
        public void UpdateExpectedDeparture_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            var newDeparture = DateTime.UtcNow.AddDays(5);

            // Act
            notification.UpdateExpectedDeparture(newDeparture);

            // Assert
            Assert.Equal(newDeparture, notification.ExpectedDeparture.Value);
        }

        #endregion

        #region Safety Officer Tests

        [Fact]
        public void AddSafetyOfficer_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act
            notification.AddSafetyOfficer("Officer C");

            // Assert
            Assert.Contains(notification.SafetyCrewOfficers, o => o.Name == "Officer C");
        }

        [Fact]
        public void AddSafetyOfficer_WhenDuplicate_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.AddSafetyOfficer("Officer Duplicate");

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.AddSafetyOfficer("Officer Duplicate")
            );
            Assert.Contains("already in the crew list", ex.Message);
        }

        [Fact]
        public void RemoveSafetyOfficer_WhenExists_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.AddSafetyOfficer("Officer To Remove");

            // Act
            notification.RemoveSafetyOfficer("Officer To Remove");

            // Assert
            Assert.DoesNotContain(
                notification.SafetyCrewOfficers,
                o => o.Name == "Officer To Remove"
            );
        }

        [Fact]
        public void RemoveSafetyOfficer_WhenNotExists_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.RemoveSafetyOfficer("NonExistent")
            );
            Assert.Contains("not found in crew list", ex.Message);
        }

        #endregion

        #region Cargo Manifest Tests

        [Fact]
        public void AttachCargoManifest_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            var manifestId = new CargoManifestId(Guid.NewGuid());

            // Act
            notification.AttachCargoManifest(manifestId);

            // Assert
            Assert.Single(notification.CargoManifestIds);
            Assert.Contains(manifestId, notification.CargoManifestIds);
        }

        [Fact]
        public void AttachCargoManifest_WhenAlreadyAttached_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();
            var manifestId = new CargoManifestId(Guid.NewGuid());
            notification.AttachCargoManifest(manifestId);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.AttachCargoManifest(manifestId)
            );
            Assert.Contains("already attached", ex.Message);
        }

        [Fact]
        public void AttachCargoManifest_WhenMoreThanTwo_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.AttachCargoManifest(new CargoManifestId(Guid.NewGuid()));
            notification.AttachCargoManifest(new CargoManifestId(Guid.NewGuid()));

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.AttachCargoManifest(new CargoManifestId(Guid.NewGuid()))
            );
            Assert.Contains("Cannot attach more than 2 cargo manifests", ex.Message);
        }

        [Fact]
        public void DetachCargoManifest_WhenExists_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            var manifestId = new CargoManifestId(Guid.NewGuid());
            notification.AttachCargoManifest(manifestId);

            // Act
            notification.DetachCargoManifest(manifestId);

            // Assert
            Assert.Empty(notification.CargoManifestIds);
        }

        #endregion

        #region Status Workflow Tests

        [Fact]
        public void Submit_WhenInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act
            notification.Submit();

            // Assert
            Assert.Equal("Submitted", notification.Status.Value);
            Assert.NotNull(notification.SubmittedAt);
        }

        [Fact]
        public void Submit_WhenAlreadySubmitted_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.Submit();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() => notification.Submit());
            Assert.Contains("Only notifications in progress can be submitted", ex.Message);
        }

        [Fact]
        public void Approve_WhenSubmitted_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.Submit();
            var dockId = new DockId(Guid.NewGuid());

            // Act
            notification.Approve(dockId);

            // Assert
            Assert.Equal("Approved", notification.Status.Value);
            Assert.Equal(dockId, notification.AssignedDockId);
            Assert.NotNull(notification.ReviewedAt);
        }

        [Fact]
        public void Approve_WhenNotSubmitted_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.Approve(new DockId(Guid.NewGuid()))
            );
            Assert.Contains("Only submitted notifications can be approved", ex.Message);
        }

        [Fact]
        public void Reject_WhenSubmitted_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.Submit();

            // Act
            notification.Reject("Insufficient documentation");

            // Assert
            Assert.Equal("Rejected", notification.Status.Value);
            Assert.Equal("Insufficient documentation", notification.RejectionReason);
            Assert.NotNull(notification.ReviewedAt);
        }

        [Fact]
        public void Reject_WithoutReason_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.Submit();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() => notification.Reject(""));
            Assert.Contains("Rejection reason is required", ex.Message);
        }

        [Fact]
        public void ReassignDock_WhenApproved_ShouldSucceed()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.Submit();
            var oldDock = new DockId(Guid.NewGuid());
            notification.Approve(oldDock);
            var newDock = new DockId(Guid.NewGuid());

            // Act
            notification.ReassignDock(newDock);

            // Assert
            Assert.Equal(newDock, notification.AssignedDockId);
        }

        [Fact]
        public void ReassignDock_WhenNotApproved_ShouldThrowException()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
                notification.ReassignDock(new DockId(Guid.NewGuid()))
            );
            Assert.Contains("Can only reassign dock for approved notifications", ex.Message);
        }

        #endregion

        #region Helper Methods

        private VesselVisitNotification CreateValidNotification()
        {
            return VesselVisitNotification.Create(
                new VesselId(Guid.NewGuid()),
                new ShippingAgentId(Guid.NewGuid()),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(3),
                "Container Cargo",
                1000,
                "Standard handling",
                "Captain Smith",
                25,
                new List<string> { "Officer A", "Officer B" }
            );
        }

        #endregion
    }
}
