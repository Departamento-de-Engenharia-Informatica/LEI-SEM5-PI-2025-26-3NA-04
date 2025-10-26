using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Domain.NotificationAggregate.ValueObjects;

namespace APDL.Tests.Domain.VesselVisitAggregate
{
    public class VesselVisitNotificationDomainTests
    {
        private DateTime GetFutureDate(int daysFromNow = 1)
        {
            return DateTime.UtcNow.AddDays(daysFromNow);
        }

        [Fact]
        public void CreateNotification_WithValidData_ShouldSucceed()
        {
            // Arrange
            var arrival = GetFutureDate(1);
            var departure = GetFutureDate(3);
            var officers = new List<string> { "John Smith", "Maria Garcia" };

            // Act
            var notification = VesselVisitNotification.Create(arrival, departure, officers);

            // Assert
            Assert.NotNull(notification);
            Assert.Equal(arrival, notification.ExpectedArrival.Value);
            Assert.Equal(departure, notification.ExpectedDeparture.Value);
            Assert.Equal(2, notification.SafetyCrewOfficers.Count);
            Assert.Equal("InProgress", notification.Status.Value);
        }

        [Fact]
        public void CreateNotification_WithoutSafetyOfficers_ShouldSucceed()
        {
            // Arrange
            var arrival = GetFutureDate(1);
            var departure = GetFutureDate(3);

            // Act
            var notification = VesselVisitNotification.Create(arrival, departure, null);

            // Assert
            Assert.NotNull(notification);
            Assert.Empty(notification.SafetyCrewOfficers);
        }

        [Fact]
        public void CreateNotification_WhenDepartureBeforeArrival_ShouldThrow()
        {
            // Arrange
            var arrival = GetFutureDate(3);
            var departure = GetFutureDate(1);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                VesselVisitNotification.Create(arrival, departure, null);
            });

            Assert.Contains("must be after expected arrival", ex.Message);
        }

        [Fact]
        public void CreateNotification_WhenDepartureEqualsArrival_ShouldThrow()
        {
            // Arrange
            var date = GetFutureDate(1);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                VesselVisitNotification.Create(date, date, null);
            });

            Assert.Contains("Expected departure must be after expected arrival.", ex.Message);
        }

        [Fact]
        public void UpdateExpectedArrival_WithValidDate_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(5), null
            );
            var newArrival = GetFutureDate(2);

            // Act
            notification.UpdateExpectedArrival(newArrival);

            // Assert
            Assert.Equal(newArrival, notification.ExpectedArrival.Value);
        }

        [Fact]
        public void UpdateExpectedArrival_WhenApproved_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );
            notification.Submit();
            notification.Approve();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.UpdateExpectedArrival(GetFutureDate(2));
            });

            Assert.Contains("Cannot update notification with current status", ex.Message);
        }

        [Fact]
        public void UpdateExpectedDeparture_WithValidDate_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );
            var newDeparture = GetFutureDate(5);

            // Act
            notification.UpdateExpectedDeparture(newDeparture);

            // Assert
            Assert.Equal(newDeparture, notification.ExpectedDeparture.Value);
        }

        [Fact]
        public void UpdateExpectedDeparture_BeforeArrival_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(5), GetFutureDate(10), null
            );

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.UpdateExpectedDeparture(GetFutureDate(2));
            });

            Assert.Contains("Expected departure must be after expected arrival.", ex.Message);
        }

        [Fact]
        public void AddSafetyOfficer_WithValidName_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act
            notification.AddSafetyOfficer("John Smith");

            // Assert
            Assert.Single(notification.SafetyCrewOfficers);
            Assert.Equal("John Smith", notification.SafetyCrewOfficers.First().Name);
        }

        [Fact]
        public void AddSafetyOfficer_Duplicate_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), new List<string> { "John Smith" }
            );

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.AddSafetyOfficer("John Smith");
            });

            Assert.Contains("Safety officer is already in the crew list.", ex.Message);
        }

        [Fact]
        public void AddSafetyOfficer_WhenApproved_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );
            notification.Submit();
            notification.Approve();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.AddSafetyOfficer("John Smith");
            });

            Assert.Contains("Cannot update notification", ex.Message);
        }

        [Fact]
        public void RemoveSafetyOfficer_WhenExists_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), 
                new List<string> { "John Smith", "Maria Garcia" }
            );

            // Act
            notification.RemoveSafetyOfficer("John Smith");

            // Assert
            Assert.Single(notification.SafetyCrewOfficers);
            Assert.Equal("Maria Garcia", notification.SafetyCrewOfficers.First().Name);
        }

        [Fact]
        public void RemoveSafetyOfficer_WhenNotFound_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.RemoveSafetyOfficer("John Smith");
            });

            Assert.Contains("Safety officer not found in crew list.", ex.Message);
        }

        [Fact]
        public void UpdateSafetyCrew_WithNewList_ShouldReplaceOldList()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3),
                new List<string> { "John Smith", "Maria Garcia" }
            );

            // Act
            notification.UpdateSafetyCrew(new List<string> { "Carlos Rodriguez", "Ahmed Hassan" });

            // Assert
            Assert.Equal(2, notification.SafetyCrewOfficers.Count);
            Assert.Contains(notification.SafetyCrewOfficers, o => o.Name == "Carlos Rodriguez");
            Assert.Contains(notification.SafetyCrewOfficers, o => o.Name == "Ahmed Hassan");
            Assert.DoesNotContain(notification.SafetyCrewOfficers, o => o.Name == "John Smith");
        }

        [Fact]
        public void Submit_FromInProgress_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act
            notification.Submit();

            // Assert
            Assert.Equal("Submitted", notification.Status.Value);
        }

        [Fact]
        public void Submit_FromNonInProgress_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );
            notification.Submit();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.Submit();
            });

            Assert.Contains("in progress can be submitted", ex.Message);
        }

        [Fact]
        public void Approve_FromSubmitted_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );
            notification.Submit();

            // Act
            notification.Approve();

            // Assert
            Assert.Equal("Approved", notification.Status.Value);
        }

        [Fact]
        public void Approve_FromNonSubmitted_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.Approve();
            });

            Assert.Contains("submitted notifications can be approved", ex.Message);
        }

        [Fact]
        public void Reject_FromSubmitted_ShouldSucceed()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );
            notification.Submit();

            // Act
            notification.Reject();

            // Assert
            Assert.Equal("Rejected", notification.Status.Value);
        }

        [Fact]
        public void Reject_FromNonSubmitted_ShouldThrow()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                notification.Reject();
            });

            Assert.Contains("submitted notifications can be rejected", ex.Message);
        }

        [Fact]
        public void StatusWorkflow_CompleteApprovalFlow_ShouldWork()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act & Assert - InProgress
            Assert.Equal("InProgress", notification.Status.Value);

            // Act & Assert - Submit
            notification.Submit();
            Assert.Equal("Submitted", notification.Status.Value);

            // Act & Assert - Approve
            notification.Approve();
            Assert.Equal("Approved", notification.Status.Value);
        }

        [Fact]
        public void StatusWorkflow_CompleteRejectionFlow_ShouldWork()
        {
            // Arrange
            var notification = VesselVisitNotification.Create(
                GetFutureDate(1), GetFutureDate(3), null
            );

            // Act & Assert - InProgress
            Assert.Equal("InProgress", notification.Status.Value);

            // Act & Assert - Submit
            notification.Submit();
            Assert.Equal("Submitted", notification.Status.Value);

            // Act & Assert - Reject
            notification.Reject();
            Assert.Equal("Rejected", notification.Status.Value);
        }

        #region Value Object Tests

        [Fact]
        public void SafetyOfficer_WithValidName_ShouldSucceed()
        {
            // Act
            var officer = new SafetyOfficer("John Smith");

            // Assert
            Assert.Equal("John Smith", officer.Name);
        }

        [Fact]
        public void SafetyOfficer_WithEmptyName_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
            {
                new SafetyOfficer("");
            });
        }

        [Theory]
        [InlineData("InProgress")]
        [InlineData("Submitted")]
        [InlineData("Approved")]
        [InlineData("Rejected")]
        public void NotificationStatus_WithValidStatus_ShouldSucceed(string status)
        {
            // Act
            var notificationStatus = new NotificationStatus(status);

            // Assert
            Assert.Equal(status, notificationStatus.Value);
        }

        [Fact]
        public void NotificationStatus_WithInvalidStatus_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() =>
            {
                new NotificationStatus("InvalidStatus");
            });
        }

        #endregion
    }
}