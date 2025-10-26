using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.ManifestAggregate.DTO;
using APDL.API.Domain.ManifestAggregate.Repos;
using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using APDL.API.Domain.NotificationAggregate.Repos;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.NotificationAggregate.Repos;

namespace APDL.Tests.Domain.ManifestAggregate
{
    public class CargoManifestServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICargoManifestRepository> _mockRepo;
        private readonly Mock<IVesselVisitNotificationRepository> _mockVesselVisitRepo;
        private readonly CargoManifestService _service;

    public CargoManifestServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockRepo = new Mock<ICargoManifestRepository>();
            _mockVesselVisitRepo = new Mock<IVesselVisitNotificationRepository>();
            
            _service = new CargoManifestService(
                _mockUnitOfWork.Object,
                _mockRepo.Object,
                _mockVesselVisitRepo.Object
            );
        }

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ShouldReturnListOfManifests()
        {
            // Arrange
            var manifests = CreateTestManifests();
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(manifests);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.True(result[0].IsLoadingManifest);
            Assert.False(result[1].IsLoadingManifest);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoManifests()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CargoManifest>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ShouldReturnManifest_WhenExists()
        {
            // Arrange
            var manifest = CreateTestManifests()[0];
            _mockRepo.Setup(r => r.GetByIdAsync(manifest.Id)).ReturnsAsync(manifest);

            // Act
            var result = await _service.GetByIdAsync(manifest.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(manifest.Id.AsGuid(), result.Id);
            Assert.Equal(manifest.CargoVolume.Value, result.CargoVolume);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var manifestId = new CargoManifestId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(manifestId)).ReturnsAsync((CargoManifest)null);

            // Act
            var result = await _service.GetByIdAsync(manifestId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region GetByVesselVisitAsync Tests

        [Fact]
        public async Task GetByVesselVisitAsync_ShouldReturnManifests()
        {
            // Arrange
            var vesselVisitId = Guid.NewGuid();
            var manifests = CreateTestManifests();
            _mockRepo.Setup(r => r.GetByVesselVisitAsync(vesselVisitId)).ReturnsAsync(manifests);

            // Act
            var result = await _service.GetByVesselVisitAsync(vesselVisitId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        #endregion

        #region AddAsync Tests

        [Fact]
        public async Task AddAsync_ShouldCreateManifest_WhenValidAndVesselVisitExists()
        {
            // Arrange
            var vesselVisitId = Guid.NewGuid();
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 1000,
                VesselVisitNotificationId = vesselVisitId
            };

            _mockVesselVisitRepo.Setup(r => r.ExistsAsync(It.IsAny<VesselVisitNotificationId>()))
                .ReturnsAsync(true);
            _mockRepo.Setup(r => r.HasManifestForVesselVisitAsync(vesselVisitId, true))
                .ReturnsAsync(false);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<CargoManifest>()))
                .ReturnsAsync((CargoManifest m) => m);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.IsLoadingManifest, result.IsLoadingManifest);
            Assert.Equal(dto.CargoVolume, result.CargoVolume);
            Assert.Equal(dto.VesselVisitNotificationId, result.VesselVisitNotificationId);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<CargoManifest>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenVesselVisitDoesNotExist()
        {
            // Arrange
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 1000,
                VesselVisitNotificationId = Guid.NewGuid()
            };

            _mockVesselVisitRepo.Setup(r => r.ExistsAsync(It.IsAny<VesselVisitNotificationId>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<CargoManifest>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenLoadingManifestAlreadyExists()
        {
            // Arrange
            var vesselVisitId = Guid.NewGuid();
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = true,
                CargoVolume = 1000,
                VesselVisitNotificationId = vesselVisitId
            };

            _mockVesselVisitRepo.Setup(r => r.ExistsAsync(It.IsAny<VesselVisitNotificationId>()))
                .ReturnsAsync(true);
            _mockRepo.Setup(r => r.HasManifestForVesselVisitAsync(vesselVisitId, true))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<CargoManifest>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenUnloadingManifestAlreadyExists()
        {
            // Arrange
            var vesselVisitId = Guid.NewGuid();
            var dto = new CreateCargoManifestDto
            {
                IsLoadingManifest = false,
                CargoVolume = 1000,
                VesselVisitNotificationId = vesselVisitId
            };

            _mockVesselVisitRepo.Setup(r => r.ExistsAsync(It.IsAny<VesselVisitNotificationId>()))
                .ReturnsAsync(true);
            _mockRepo.Setup(r => r.HasManifestForVesselVisitAsync(vesselVisitId, false))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _service.AddAsync(dto));
        }

        #endregion

        #region AddContainerAsync Tests

        [Fact]
        public async Task AddContainerAsync_ShouldAddContainer_WhenManifestExists()
        {
            // Arrange
            var manifest = CreateTestManifests()[0];
            var containerId = Guid.NewGuid();

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<CargoManifestId>())).ReturnsAsync(manifest);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.AddContainerAsync(manifest.Id.AsGuid(), containerId);

            // Assert
            Assert.Single(manifest.ContainerIds);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddContainerAsync_ShouldThrow_WhenManifestDoesNotExist()
        {
            // Arrange
            var manifestId = Guid.NewGuid();
            var containerId = Guid.NewGuid();

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<CargoManifestId>()))
                .ReturnsAsync((CargoManifest)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddContainerAsync(manifestId, containerId));
        }

        #endregion

        #region RemoveContainerAsync Tests

        [Fact]
        public async Task RemoveContainerAsync_ShouldRemoveContainer_WhenExists()
        {
            // Arrange
            var manifest = CreateTestManifests()[0];
            var containerId = new ContainerId(Guid.NewGuid());
            manifest.AddContainer(containerId);

            _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<CargoManifestId>())).ReturnsAsync(manifest);
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await _service.RemoveContainerAsync(manifest.Id.AsGuid(), containerId.AsGuid());

            // Assert
            Assert.Empty(manifest.ContainerIds);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ShouldRemoveManifest_WhenExists()
        {
            // Arrange
            var manifest = CreateTestManifests()[0];

            _mockRepo.Setup(r => r.GetByIdAsync(manifest.Id)).ReturnsAsync(manifest);
            _mockRepo.Setup(r => r.Remove(manifest)).Verifiable();
            _mockUnitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(manifest.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(manifest.Id.AsGuid(), result.Id);
            _mockRepo.Verify(r => r.Remove(manifest), Times.Once);
            _mockUnitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var manifestId = new CargoManifestId(Guid.NewGuid());
            _mockRepo.Setup(r => r.GetByIdAsync(manifestId)).ReturnsAsync((CargoManifest)null);

            // Act
            var result = await _service.DeleteAsync(manifestId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(r => r.Remove(It.IsAny<CargoManifest>()), Times.Never);
        }

        #endregion

        #region Helper Methods

        private List<CargoManifest> CreateTestManifests()
        {
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            var loadingManifest = CargoManifest.Create(
                isLoadingManifest: true,
                cargoVolume: 1000,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            var unloadingManifest = CargoManifest.Create(
                isLoadingManifest: false,
                cargoVolume: 800,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            return new List<CargoManifest> { loadingManifest, unloadingManifest };
        }

        #endregion
    }
}