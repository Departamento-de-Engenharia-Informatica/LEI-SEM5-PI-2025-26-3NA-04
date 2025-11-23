using System;
using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.Shared;
using Xunit;

namespace APDL.Tests.Unit.Domain
{
    public class CargoManifestUnitTests
    {
        #region Create Tests

        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());
            bool isLoadingManifest = true;
            int cargoVolume = 100;

            // Act
            var manifest = CargoManifest.Create(
                isLoadingManifest,
                cargoVolume,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            // Assert
            Assert.NotNull(manifest);
            Assert.NotEqual(Guid.Empty, manifest.Id.AsGuid());
            Assert.Equal(isLoadingManifest, manifest.IsLoadingManifest);
            Assert.Equal(cargoVolume, manifest.CargoVolume.Value);
            Assert.Equal(vesselVisitId, manifest.VesselVisitNotificationId);
            Assert.Empty(manifest.ContainerIds);
        }

        [Fact]
        public void Create_AsLoadingManifest_ShouldSetIsLoadingManifestToTrue()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act
            var manifest = CargoManifest.Create(
                isLoadingManifest: true,
                cargoVolume: 100,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            // Assert
            Assert.True(manifest.IsLoadingManifest);
        }

        [Fact]
        public void Create_AsUnloadingManifest_ShouldSetIsLoadingManifestToFalse()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act
            var manifest = CargoManifest.Create(
                isLoadingManifest: false,
                cargoVolume: 50,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            // Assert
            Assert.False(manifest.IsLoadingManifest);
        }

        [Fact]
        public void Create_WithZeroCargoVolume_ShouldSucceed()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act
            var manifest = CargoManifest.Create(
                isLoadingManifest: true,
                cargoVolume: 0,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            // Assert
            Assert.NotNull(manifest);
            Assert.Equal(0, manifest.CargoVolume.Value);
        }

        [Fact]
        public void Create_WithNegativeCargoVolume_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                CargoManifest.Create(
                    isLoadingManifest: true,
                    cargoVolume: -10,
                    vesselVisitId,
                    vesselVisitExists: true,
                    manifestAlreadyExists: false
                )
            );

            Assert.Contains("Cargo volume cannot be negative", exception.Message);
        }

        [Fact]
        public void Create_WhenVesselVisitDoesNotExist_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                CargoManifest.Create(
                    isLoadingManifest: true,
                    cargoVolume: 100,
                    vesselVisitId,
                    vesselVisitExists: false,
                    manifestAlreadyExists: false
                )
            );

            Assert.Contains("does not exist", exception.Message);
            Assert.Contains(vesselVisitId.Value.ToString(), exception.Message);
        }

        [Fact]
        public void Create_WhenLoadingManifestAlreadyExists_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                CargoManifest.Create(
                    isLoadingManifest: true,
                    cargoVolume: 100,
                    vesselVisitId,
                    vesselVisitExists: true,
                    manifestAlreadyExists: true
                )
            );

            Assert.Contains("loading manifest already exists", exception.Message);
        }

        [Fact]
        public void Create_WhenUnloadingManifestAlreadyExists_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                CargoManifest.Create(
                    isLoadingManifest: false,
                    cargoVolume: 100,
                    vesselVisitId,
                    vesselVisitExists: true,
                    manifestAlreadyExists: true
                )
            );

            Assert.Contains("unloading manifest already exists", exception.Message);
        }

        #endregion

        #region AddContainer Tests

        [Fact]
        public void AddContainer_WithValidContainerId_ShouldSucceed()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var containerId = new ContainerId(Guid.NewGuid());

            // Act
            manifest.AddContainer(containerId);

            // Assert
            Assert.Single(manifest.ContainerIds);
            Assert.Contains(containerId, manifest.ContainerIds);
        }

        [Fact]
        public void AddContainer_MultipleContainers_ShouldAddAll()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var container1 = new ContainerId(Guid.NewGuid());
            var container2 = new ContainerId(Guid.NewGuid());
            var container3 = new ContainerId(Guid.NewGuid());

            // Act
            manifest.AddContainer(container1);
            manifest.AddContainer(container2);
            manifest.AddContainer(container3);

            // Assert
            Assert.Equal(3, manifest.ContainerIds.Count);
            Assert.Contains(container1, manifest.ContainerIds);
            Assert.Contains(container2, manifest.ContainerIds);
            Assert.Contains(container3, manifest.ContainerIds);
        }

        [Fact]
        public void AddContainer_WithNullContainerId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var manifest = CreateValidManifest();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manifest.AddContainer(null));
        }

        [Fact]
        public void AddContainer_WhenContainerAlreadyExists_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var containerId = new ContainerId(Guid.NewGuid());
            manifest.AddContainer(containerId);

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                manifest.AddContainer(containerId)
            );

            Assert.Contains("already in this manifest", exception.Message);
            Assert.Contains(containerId.Value.ToString(), exception.Message);
        }

        #endregion

        #region RemoveContainer Tests

        [Fact]
        public void RemoveContainer_WithExistingContainer_ShouldSucceed()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var containerId = new ContainerId(Guid.NewGuid());
            manifest.AddContainer(containerId);

            // Act
            manifest.RemoveContainer(containerId);

            // Assert
            Assert.Empty(manifest.ContainerIds);
            Assert.DoesNotContain(containerId, manifest.ContainerIds);
        }

        [Fact]
        public void RemoveContainer_FromMultipleContainers_ShouldRemoveOnlySpecified()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var container1 = new ContainerId(Guid.NewGuid());
            var container2 = new ContainerId(Guid.NewGuid());
            var container3 = new ContainerId(Guid.NewGuid());
            manifest.AddContainer(container1);
            manifest.AddContainer(container2);
            manifest.AddContainer(container3);

            // Act
            manifest.RemoveContainer(container2);

            // Assert
            Assert.Equal(2, manifest.ContainerIds.Count);
            Assert.Contains(container1, manifest.ContainerIds);
            Assert.DoesNotContain(container2, manifest.ContainerIds);
            Assert.Contains(container3, manifest.ContainerIds);
        }

        [Fact]
        public void RemoveContainer_WithNullContainerId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var manifest = CreateValidManifest();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manifest.RemoveContainer(null));
        }

        [Fact]
        public void RemoveContainer_WhenContainerDoesNotExist_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var containerId = new ContainerId(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                manifest.RemoveContainer(containerId)
            );

            Assert.Contains("not in this manifest", exception.Message);
            Assert.Contains(containerId.Value.ToString(), exception.Message);
        }

        [Fact]
        public void RemoveContainer_FromEmptyManifest_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var manifest = CreateValidManifest();
            var containerId = new ContainerId(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                manifest.RemoveContainer(containerId)
            );

            Assert.Contains("not in this manifest", exception.Message);
        }

        #endregion

        #region UpdateCargoVolume Tests

        [Fact]
        public void UpdateCargoVolume_WithValidVolume_ShouldSucceed()
        {
            // Arrange
            var manifest = CreateValidManifest();
            int newVolume = 250;

            // Act
            manifest.updateCargoVolume(newVolume);

            // Assert
            Assert.Equal(newVolume, manifest.CargoVolume.Value);
        }

        [Fact]
        public void UpdateCargoVolume_ToZero_ShouldSucceed()
        {
            // Arrange
            var manifest = CreateValidManifest();

            // Act
            manifest.updateCargoVolume(0);

            // Assert
            Assert.Equal(0, manifest.CargoVolume.Value);
        }

        [Fact]
        public void UpdateCargoVolume_WithNegativeVolume_ShouldThrowBusinessRuleValidationException()
        {
            // Arrange
            var manifest = CreateValidManifest();

            // Act & Assert
            var exception = Assert.Throws<BusinessRuleValidationException>(() =>
                manifest.updateCargoVolume(-50)
            );

            Assert.Contains("Cargo volume cannot be negative", exception.Message);
        }

        [Fact]
        public void UpdateCargoVolume_MultipleTimes_ShouldKeepLatestValue()
        {
            // Arrange
            var manifest = CreateValidManifest();

            // Act
            manifest.updateCargoVolume(100);
            manifest.updateCargoVolume(200);
            manifest.updateCargoVolume(150);

            // Assert
            Assert.Equal(150, manifest.CargoVolume.Value);
        }

        #endregion

        #region Helper Methods

        private CargoManifest CreateValidManifest()
        {
            var vesselVisitId = new VesselVisitNotificationId(Guid.NewGuid());
            return CargoManifest.Create(
                isLoadingManifest: true,
                cargoVolume: 100,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );
        }

        #endregion
    }
}
