using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.DTO;
using APDL.API.Domain.Shared;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure.VesselVisitInfrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APDL.Tests.Integration
{
    public class VesselVisitNotificationServiceTests : IDisposable
    {
        private readonly DDDSample1DbContext _context;
        private readonly VesselVisitNotificationService _service;
        private readonly IUnitOfWork _unitOfWork;

        public VesselVisitNotificationServiceTests()
        {
            var options = new DbContextOptionsBuilder<DDDSample1DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            _context = new DDDSample1DbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            var repo = new VesselVisitNotificationRepository(_context);
            _service = new VesselVisitNotificationService(_unitOfWork, repo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateNotification()
        {
            // Arrange
            var dto = new CreateVesselVisitNotificationDto
            {
                VesselId = Guid.NewGuid(),
                ShippingAgentId = Guid.NewGuid(),
                ExpectedArrival = DateTime.UtcNow.AddDays(1),
                ExpectedDeparture = DateTime.UtcNow.AddDays(3),
                CargoType = "Container Cargo",
                CargoVolume = 1000,
                SpecialHandlingRequirements = "Handle with care",
                CaptainName = "Captain Smith",
                TotalCrewCount = 25,
                SafetyCrewOfficers = new List<string> { "Officer A", "Officer B" },
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Container Cargo", result.CargoType);
            Assert.Equal("InProgress", result.Status);
            Assert.Equal(2, result.SafetyCrewOfficers.Count);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var dto = CreateValidDto();

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            var dbNotification = await _context.VesselVisitNotifications.FirstOrDefaultAsync(n =>
                n.Id.AsGuid() == result.Id
            );
            Assert.NotNull(dbNotification);
            Assert.Equal(dto.CargoType, dbNotification.CargoType);
        }

        [Fact]
        public async Task AddAsync_WithInvalidData_ShouldThrowException()
        {
            // Arrange - departure before arrival
            var dto = new CreateVesselVisitNotificationDto
            {
                VesselId = Guid.NewGuid(),
                ShippingAgentId = Guid.NewGuid(),
                ExpectedArrival = DateTime.UtcNow.AddDays(3),
                ExpectedDeparture = DateTime.UtcNow.AddDays(1),
                CargoType = "Cargo",
                CargoVolume = 100,
                CaptainName = "Captain",
                TotalCrewCount = 10,
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllNotifications()
        {
            // Arrange
            await CreateTestNotification();
            await CreateTestNotification();

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnNotification()
        {
            // Arrange
            var created = await CreateTestNotification();

            // Act
            var result = await _service.GetByIdAsync(new VesselVisitNotificationId(created.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(new VesselVisitNotificationId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetPendingNotificationsAsync_ShouldReturnOnlySubmitted()
        {
            // Arrange
            var notification1 = await CreateTestNotification();
            var notification2 = await CreateTestNotification();

            await _service.SubmitAsync(notification1.Id);
            // notification2 stays InProgress

            // Act
            var result = await _service.GetPendingNotificationsAsync();

            // Assert
            Assert.Single(result);
            Assert.All(result, n => Assert.Equal("Submitted", n.Status));
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateNotification()
        {
            // Arrange
            var notification = await CreateTestNotification();
            var updateDto = new UpdateVesselVisitNotificationDto
            {
                Id = notification.Id,
                ExpectedArrival = DateTime.UtcNow.AddDays(2),
                ExpectedDeparture = DateTime.UtcNow.AddDays(4),
                CargoType = "Bulk Cargo",
                CargoVolume = 2000,
                SpecialHandlingRequirements = "Fragile",
                CaptainName = "Captain Jones",
                TotalCrewCount = 30,
                SafetyCrewOfficers = new List<string> { "Officer C" },
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Bulk Cargo", result.CargoType);
            Assert.Equal(2000, result.CargoVolume);
            Assert.Equal("Captain Jones", result.CaptainName);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new UpdateVesselVisitNotificationDto
            {
                Id = Guid.NewGuid(),
                ExpectedArrival = DateTime.UtcNow.AddDays(1),
                ExpectedDeparture = DateTime.UtcNow.AddDays(3),
                CargoType = "Cargo",
                CargoVolume = 100,
                CaptainName = "Captain",
                TotalCrewCount = 10,
            };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WhenSubmitted_ShouldThrowException()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.SubmitAsync(notification.Id);

            var updateDto = new UpdateVesselVisitNotificationDto
            {
                Id = notification.Id,
                ExpectedArrival = DateTime.UtcNow.AddDays(1),
                ExpectedDeparture = DateTime.UtcNow.AddDays(3),
                CargoType = "New Cargo",
                CargoVolume = 500,
                CaptainName = "Captain",
                TotalCrewCount = 10,
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.UpdateAsync(updateDto)
            );
        }

        #endregion

        #region Workflow Tests

        [Fact]
        public async Task SubmitAsync_WhenInProgress_ShouldChangeStatus()
        {
            // Arrange
            var notification = await CreateTestNotification();

            // Act
            await _service.SubmitAsync(notification.Id);

            // Assert
            var updated = await _service.GetByIdAsync(
                new VesselVisitNotificationId(notification.Id)
            );
            Assert.Equal("Submitted", updated.Status);
            Assert.NotNull(updated.SubmittedAt);
        }

        [Fact]
        public async Task SubmitAsync_WhenAlreadySubmitted_ShouldThrowException()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.SubmitAsync(notification.Id);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.SubmitAsync(notification.Id)
            );
        }

        [Fact]
        public async Task ApproveAsync_WhenSubmitted_ShouldApprove()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.SubmitAsync(notification.Id);
            var dockId = Guid.NewGuid();

            // Act
            await _service.ApproveAsync(notification.Id, dockId);

            // Assert
            var updated = await _service.GetByIdAsync(
                new VesselVisitNotificationId(notification.Id)
            );
            Assert.Equal("Approved", updated.Status);
            Assert.Equal(dockId, updated.AssignedDockId);
            Assert.NotNull(updated.ReviewedAt);
        }

        [Fact]
        public async Task ApproveAsync_WhenNotSubmitted_ShouldThrowException()
        {
            // Arrange
            var notification = await CreateTestNotification();

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.ApproveAsync(notification.Id, Guid.NewGuid())
            );
        }

        [Fact]
        public async Task RejectAsync_WhenSubmitted_ShouldReject()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.SubmitAsync(notification.Id);

            // Act
            await _service.RejectAsync(notification.Id, "Insufficient documentation");

            // Assert
            var updated = await _service.GetByIdAsync(
                new VesselVisitNotificationId(notification.Id)
            );
            Assert.Equal("Rejected", updated.Status);
            Assert.Equal("Insufficient documentation", updated.RejectionReason);
            Assert.NotNull(updated.ReviewedAt);
        }

        [Fact]
        public async Task RejectAsync_WithoutReason_ShouldThrowException()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.SubmitAsync(notification.Id);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.RejectAsync(notification.Id, "")
            );
        }

        #endregion

        #region Safety Officer Tests

        [Fact]
        public async Task AddSafetyOfficerAsync_ShouldAddOfficer()
        {
            // Arrange
            var notification = await CreateTestNotification();

            // Act
            await _service.AddSafetyOfficerAsync(notification.Id, "Officer C");

            // Assert
            var updated = await _service.GetByIdAsync(
                new VesselVisitNotificationId(notification.Id)
            );
            Assert.Contains("Officer C", updated.SafetyCrewOfficers);
        }

        [Fact]
        public async Task AddSafetyOfficerAsync_WhenDuplicate_ShouldThrowException()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.AddSafetyOfficerAsync(notification.Id, "Officer Duplicate");

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddSafetyOfficerAsync(notification.Id, "Officer Duplicate")
            );
        }

        [Fact]
        public async Task RemoveSafetyOfficerAsync_ShouldRemoveOfficer()
        {
            // Arrange
            var notification = await CreateTestNotification();
            await _service.AddSafetyOfficerAsync(notification.Id, "Officer ToRemove");

            // Act
            await _service.RemoveSafetyOfficerAsync(notification.Id, "Officer ToRemove");

            // Assert
            var updated = await _service.GetByIdAsync(
                new VesselVisitNotificationId(notification.Id)
            );
            Assert.DoesNotContain("Officer ToRemove", updated.SafetyCrewOfficers);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldDelete()
        {
            // Arrange
            var notification = await CreateTestNotification();

            // Act
            var result = await _service.DeleteAsync(new VesselVisitNotificationId(notification.Id));

            // Assert
            Assert.NotNull(result);
            var dbNotification = await _context.VesselVisitNotifications.FirstOrDefaultAsync(n =>
                n.Id.AsGuid() == notification.Id
            );
            Assert.Null(dbNotification);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.DeleteAsync(new VesselVisitNotificationId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Helper Methods

        private CreateVesselVisitNotificationDto CreateValidDto()
        {
            return new CreateVesselVisitNotificationDto
            {
                VesselId = Guid.NewGuid(),
                ShippingAgentId = Guid.NewGuid(),
                ExpectedArrival = DateTime.UtcNow.AddDays(1),
                ExpectedDeparture = DateTime.UtcNow.AddDays(3),
                CargoType = "Container Cargo",
                CargoVolume = 1000,
                SpecialHandlingRequirements = "Standard handling",
                CaptainName = "Captain Smith",
                TotalCrewCount = 25,
                SafetyCrewOfficers = new List<string> { "Officer A", "Officer B" },
            };
        }

        private async Task<VesselVisitNotificationDto> CreateTestNotification()
        {
            var dto = CreateValidDto();
            return await _service.AddAsync(dto);
        }

        #endregion
    }
}
