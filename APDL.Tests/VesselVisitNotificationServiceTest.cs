using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.NotificationAggregate.DTO;

namespace APDL.Tests.Domain.VesselVisitAggregate
{
    public class VesselVisitNotificationServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IVesselVisitNotificationRepository> _mockRepo;
        private readonly VesselVisitNotificationService _service;

        public VesselVisitNotificationServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<IVesselVisitNotificationRepository>();
            _service = new VesselVisitNotificationService(_mockUnitOfWork.Object, _mockRepo.Object);
        }

        private DateTime GetFutureDate(int daysFromNow) => DateTime.UtcNow.AddDays(daysFromNow);

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfNotifications()
        {
            // Arrange
            var notifications = CreateTestNotifications();
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(notifications);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("InProgress", result[0].Status);
            Assert.Equal(2, result[0].SafetyCrewOfficers.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoNotifications()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<VesselVisitNotification>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNotification_WhenExists()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(notification.Id)).ReturnsAsync(notification);

            // Act
            var result = await _service.GetByIdAsync(notification.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(notification.Id.AsGuid(), result.Id);
            Assert.Equal(notification.Status.Value, result.Status);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var notificationId = new VesselVisitNotificationId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(notificationId)).ReturnsAsync((VesselVisitNotification)null);

            // Act
            var result = await _service.GetByIdAsync(notificationId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByStatusAsync Tests

        [Fact]
        public async Task GetByStatusAsync_ShouldReturnNotifications()
        {
            // Arrange
            var notifications = CreateTestNotifications();
            _mockRepo.Setup(r => r.GetByStatusAsync("Submitted")).ReturnsAsync(notifications);

            // Act
            var result = await _service.GetByStatusAsync("Submitted");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region GetPendingNotificationsAsync Tests

        [Fact]
        public async Task GetPendingNotificationsAsync_ShouldReturnSubmittedNotifications()
        {
            // Arrange
            var notifications = CreateTestNotifications();
            _mockRepo.Setup(r => r.GetPendingNotificationsAsync()).ReturnsAsync(notifications);

            // Act
            var result = await _service.GetPendingNotificationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCreateNotification_WhenValid()
        {
            // Arrange
            var dto = new CreateVesselVisitNotificationDto
            {
                ExpectedArrival = GetFutureDate(1),
                ExpectedDeparture = GetFutureDate(3),
                SafetyCrewOfficers = new List<string> { "John Smith", "Maria Garcia" }
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<VesselVisitNotification>()))
                .ReturnsAsync((VesselVisitNotification n) => n);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ExpectedArrival, result.ExpectedArrival);
            Assert.Equal(dto.ExpectedDeparture, result.ExpectedDeparture);
            Assert.Equal(2, result.SafetyCrewOfficers.Count);
            Assert.Equal("InProgress", result.Status);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<VesselVisitNotification>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldCreateNotification_WithoutSafetyOfficers()
        {
            // Arrange
            var dto = new CreateVesselVisitNotificationDto
            {
                ExpectedArrival = GetFutureDate(1),
                ExpectedDeparture = GetFutureDate(3),
                SafetyCrewOfficers = new List<string>()
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<VesselVisitNotification>()))
                .ReturnsAsync((VesselVisitNotification n) => n);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.SafetyCrewOfficers);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenDepartureBeforeArrival()
        {
            // Arrange
            var dto = new CreateVesselVisitNotificationDto
            {
                ExpectedArrival = GetFutureDate(5),
                ExpectedDeparture = GetFutureDate(1),
                SafetyCrewOfficers = new List<string>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<VesselVisitNotification>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ShouldUpdateNotification_WhenInProgress()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            var dto = new UpdateVesselVisitNotificationDto
            {
                Id = notification.Id.AsGuid(),
                ExpectedArrival = GetFutureDate(2),
                ExpectedDeparture = GetFutureDate(6),
                SafetyCrewOfficers = new List<string> { "New Officer" }
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ExpectedArrival, result.ExpectedArrival);
            Assert.Equal(dto.ExpectedDeparture, result.ExpectedDeparture);
            Assert.Single(result.SafetyCrewOfficers);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var dto = new UpdateVesselVisitNotificationDto
            {
                Id = Guid.NewGuid(),
                ExpectedArrival = GetFutureDate(1),
                ExpectedDeparture = GetFutureDate(3),
                SafetyCrewOfficers = new List<string>()
            };

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>()))
                .ReturnsAsync((VesselVisitNotification)null);

            // Act
            var result = await _service.UpdateAsync(dto);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        #endregion

        #region SubmitAsync Tests

        [Fact]
        public async Task SubmitAsync_ShouldChangeStatusToSubmitted_WhenInProgress()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.SubmitAsync(notification.Id.AsGuid());

            // Assert
            Assert.Equal("Submitted", notification.Status.Value);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task SubmitAsync_ShouldThrow_WhenNotExists()
        {
            // Arrange
            var notificationId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>()))
                .ReturnsAsync((VesselVisitNotification)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => 
                _service.SubmitAsync(notificationId));
        }

        #endregion

        #region ApproveAsync Tests

        [Fact]
        public async Task ApproveAsync_ShouldChangeStatusToApproved_WhenSubmitted()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            notification.Submit();
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.ApproveAsync(notification.Id.AsGuid());

            // Assert
            Assert.Equal("Approved", notification.Status.Value);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_ShouldThrow_WhenNotSubmitted()
        {
            // Arrange
            var notification = CreateTestNotifications()[0]; // Status: InProgress
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.ApproveAsync(notification.Id.AsGuid()));
        }

        #endregion

        #region RejectAsync Tests

        [Fact]
        public async Task RejectAsync_ShouldChangeStatusToRejected_WhenSubmitted()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            notification.Submit();
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.RejectAsync(notification.Id.AsGuid());

            // Assert
            Assert.Equal("Rejected", notification.Status.Value);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_ShouldThrow_WhenNotSubmitted()
        {
            // Arrange
            var notification = CreateTestNotifications()[0]; // Status: InProgress
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.RejectAsync(notification.Id.AsGuid()));
        }

        #endregion

        #region AddSafetyOfficerAsync Tests

        [Fact]
        public async Task AddSafetyOfficerAsync_ShouldAddOfficer_WhenValid()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.AddSafetyOfficerAsync(notification.Id.AsGuid(), "New Officer");

            // Assert
            Assert.Equal(3, notification.SafetyCrewOfficers.Count);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddSafetyOfficerAsync_ShouldThrow_WhenDuplicate()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddSafetyOfficerAsync(notification.Id.AsGuid(), "John Smith"));
        }

        #endregion

        #region RemoveSafetyOfficerAsync Tests

        [Fact]
        public async Task RemoveSafetyOfficerAsync_ShouldRemoveOfficer_WhenExists()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.RemoveSafetyOfficerAsync(notification.Id.AsGuid(), "John Smith");

            // Assert
            Assert.Single(notification.SafetyCrewOfficers);
            Assert.DoesNotContain(notification.SafetyCrewOfficers, o => o.Name == "John Smith");
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveSafetyOfficerAsync_ShouldThrow_WhenNotExists()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<VesselVisitNotificationId>())).ReturnsAsync(notification);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.RemoveSafetyOfficerAsync(notification.Id.AsGuid(), "Non Existent"));
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveNotification_WhenExists()
        {
            // Arrange
            var notification = CreateTestNotifications()[0];

            _mockRepo.Setup(r => r.GetByIdAsync(notification.Id)).ReturnsAsync(notification);
            _mockRepo.Setup(r => r.Remove(notification)).Verifiable();
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(notification.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(notification.Id.AsGuid(), result.Id);
            _mockRepo.Verify(r => r.Remove(notification), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var notificationId = new VesselVisitNotificationId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(notificationId)).ReturnsAsync((VesselVisitNotification)null);

            // Act
            var result = await _service.DeleteAsync(notificationId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<VesselVisitNotification>()), Times.Never);
        }

        #endregion

        #region Helper Methods

        private List<VesselVisitNotification> CreateTestNotifications()
        {
            var notification1 = VesselVisitNotification.Create(
                GetFutureDate(1),
                GetFutureDate(3),
                new List<string> { "John Smith", "Maria Garcia" }
            );

            var notification2 = VesselVisitNotification.Create(
                GetFutureDate(5),
                GetFutureDate(10),
                new List<string> { "Carlos Rodriguez" }
            );

            return new List<VesselVisitNotification> { notification1, notification2 };
        }

        #endregion
    }
}