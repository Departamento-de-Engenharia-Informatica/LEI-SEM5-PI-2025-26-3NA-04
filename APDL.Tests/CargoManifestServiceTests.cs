using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.ManifestAggregate.DTO;
using APDL.API.Domain.ManifestAggregate.Repos;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.DTO;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure;
using APDL.API.Infrastructure.ManifestInfrastructure;
using APDL.API.Infrastructure.Shared;
using APDL.API.Infrastructure.VesselVisitInfrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace APDL.Tests.Integration
{
    public class CargoManifestServiceTests : IDisposable
    {
        private readonly DDDSample1DbContext _context;
        private readonly CargoManifestService _service;
        private readonly VesselVisitNotificationService _vesselVisitService;
        private readonly IUnitOfWork _unitOfWork;

        public CargoManifestServiceTests()
        {
            var options = new DbContextOptionsBuilder<DDDSample1DbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + Guid.NewGuid())
                .Options;

            _context = new DDDSample1DbContext(options);
            _unitOfWork = new UnitOfWork(_context);

            var cargoRepo = new CargoManifestRepository(_context);
            var vesselVisitRepo = new VesselVisitNotificationRepository(_context);

            _service = new CargoManifestService(_unitOfWork, cargoRepo, vesselVisitRepo);
            _vesselVisitService = new VesselVisitNotificationService(_unitOfWork, vesselVisitRepo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region Create Tests

        [Fact]
        public async Task AddAsync_WithValidData_ShouldCreateManifest()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 100,
                VesselVisitNotificationId = vesselVisitId,
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(100, result.CargoVolume);
            Assert.True(result.IsLoadingManifest);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = false,
                CargoVolume = 250,
                VesselVisitNotificationId = vesselVisitId,
            };

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            var dbManifest = await _context.CargoManifests.FirstOrDefaultAsync(m =>
                m.Id.AsGuid() == result.Id
            );
            Assert.NotNull(dbManifest);
            Assert.Equal(250, dbManifest.CargoVolume.Value);
            Assert.False(dbManifest.IsLoadingManifest);
        }

        [Fact]
        public async Task AddAsync_WithNonExistentVesselVisit_ShouldThrowException()
        {
            // Arrange
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 100,
                VesselVisitNotificationId = Guid.NewGuid(),
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenManifestAlreadyExists_ShouldThrowException()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var dto1 = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 100,
                VesselVisitNotificationId = vesselVisitId,
            };
            await _service.AddAsync(dto1);

            var dto2 = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 150,
                VesselVisitNotificationId = vesselVisitId,
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddAsync(dto2)
            );
        }

        #endregion

        #region Read Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllManifests()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            await CreateTestManifest(vesselVisitId, true, 100);
            await CreateTestManifest(vesselVisitId, false, 200);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnManifest()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var created = await CreateTestManifest(vesselVisitId, true, 150);

            // Act
            var result = await _service.GetByIdAsync(new CargoManifestId(created.Id));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(created.Id, result.Id);
            Assert.Equal(150, result.CargoVolume);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.GetByIdAsync(new CargoManifestId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByVesselVisitAsync_ShouldReturnManifestsForVessel()
        {
            // Arrange
            var vesselVisit1 = await CreateTestVesselVisit();
            var vesselVisit2 = await CreateTestVesselVisit();

            await CreateTestManifest(vesselVisit1, true, 100);
            await CreateTestManifest(vesselVisit1, false, 200);
            await CreateTestManifest(vesselVisit2, true, 150);

            // Act
            var result = await _service.GetByVesselVisitAsync(vesselVisit1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Equal(vesselVisit1, m.VesselVisitNotificationId));
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateManifest()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var manifest = await CreateTestManifest(vesselVisitId, true, 100);

            var updateDto = new UpdateCargoManifestDto { Id = manifest.Id, CargoVolume = 250 };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(250, result.CargoVolume);
        }

        [Fact]
        public async Task UpdateAsync_ShouldPersistChanges()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var manifest = await CreateTestManifest(vesselVisitId, true, 100);

            var updateDto = new UpdateCargoManifestDto { Id = manifest.Id, CargoVolume = 300 };

            // Act
            await _service.UpdateAsync(updateDto);

            // Assert
            var dbManifest = await _context.CargoManifests.FirstOrDefaultAsync(m =>
                m.Id.AsGuid() == manifest.Id
            );
            Assert.Equal(300, dbManifest.CargoVolume.Value);
        }

        [Fact]
        public async Task UpdateAsync_WhenNotExists_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new UpdateCargoManifestDto { Id = Guid.NewGuid(), CargoVolume = 250 };

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Container Management Tests

        [Fact]
        public async Task AddContainerAsync_ShouldAddContainer()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var manifest = await CreateTestManifest(vesselVisitId, true, 100);
            var containerId = Guid.NewGuid();

            // Act
            await _service.AddContainerAsync(manifest.Id, containerId);

            // Assert
            var dbManifest = await _context.CargoManifests.FirstOrDefaultAsync(m =>
                m.Id.AsGuid() == manifest.Id
            );
            Assert.Single(dbManifest.ContainerIds);
            Assert.Contains(dbManifest.ContainerIds, c => c.AsGuid() == containerId);
        }

        [Fact]
        public async Task AddContainerAsync_WhenDuplicate_ShouldThrowException()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var manifest = await CreateTestManifest(vesselVisitId, true, 100);
            var containerId = Guid.NewGuid();
            await _service.AddContainerAsync(manifest.Id, containerId);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() =>
                _service.AddContainerAsync(manifest.Id, containerId)
            );
        }

        [Fact]
        public async Task RemoveContainerAsync_ShouldRemoveContainer()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var manifest = await CreateTestManifest(vesselVisitId, true, 100);
            var containerId = Guid.NewGuid();
            await _service.AddContainerAsync(manifest.Id, containerId);

            // Act
            await _service.RemoveContainerAsync(manifest.Id, containerId);

            // Assert
            var dbManifest = await _context.CargoManifests.FirstOrDefaultAsync(m =>
                m.Id.AsGuid() == manifest.Id
            );
            Assert.Empty(dbManifest.ContainerIds);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task DeleteAsync_WhenExists_ShouldDeleteManifest()
        {
            // Arrange
            var vesselVisitId = await CreateTestVesselVisit();
            var manifest = await CreateTestManifest(vesselVisitId, true, 100);

            // Act
            var result = await _service.DeleteAsync(new CargoManifestId(manifest.Id));

            // Assert
            Assert.NotNull(result);
            var dbManifest = await _context.CargoManifests.FirstOrDefaultAsync(m =>
                m.Id.AsGuid() == manifest.Id
            );
            Assert.Null(dbManifest);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _service.DeleteAsync(new CargoManifestId(Guid.NewGuid()));

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Helper Methods

        private async Task<Guid> CreateTestVesselVisit()
        {
            var dto = new CreateVesselVisitNotificationDto
            {
                VesselId = Guid.NewGuid(),
                ShippingAgentId = Guid.NewGuid(),
                ExpectedArrival = DateTime.UtcNow.AddDays(1),
                ExpectedDeparture = DateTime.UtcNow.AddDays(3),
                CargoType = "Container Cargo",
                CargoVolume = 1000,
                SpecialHandlingRequirements = null,
                CaptainName = "Captain Test",
                TotalCrewCount = 20,
                SafetyCrewOfficers = null,
            };

            var result = await _vesselVisitService.AddAsync(dto);
            return result.Id;
        }

        private async Task<CargoManifestDto> CreateTestManifest(
            Guid vesselVisitId,
            bool isLoading,
            int volume
        )
        {
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = isLoading,
                CargoVolume = volume,
                VesselVisitNotificationId = vesselVisitId,
            };

            return await _service.AddAsync(dto);
        }

        #endregion
    }
}
