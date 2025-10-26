using System;
using System.Linq;
using Xunit;
using APDL.API.Domain.Shared;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.NotificationAggregate;

namespace APDL.Tests.Domain.ManifestAggregate
{
    public class CargoManifestDomainTests
    {
        private VesselVisitNotificationId CreateVesselVisitId()
        {
            return new VesselVisitNotificationId(Guid.NewGuid());
        }

        [Fact]
        public void CreateManifest_WithValidData_ShouldSucceed()
        {
            // Arrange
            var vesselVisitId = CreateVesselVisitId();

            // Act
            var manifest = CargoManifest.Create(
                isLoadingManifest: true,
                cargoVolume: 1000,
                vesselVisitId,
                vesselVisitExists: true,
                manifestAlreadyExists: false
            );

            // Assert
            Assert.NotNull(manifest);
            Assert.True(manifest.IsLoadingManifest);
            Assert.Equal(1000, manifest.CargoVolume.Value);
            Assert.Equal(vesselVisitId, manifest.VesselVisitNotificationId);
            Assert.Empty(manifest.ContainerIds);
        }

        [Fact]
        public void CreateManifest_WhenVesselVisitDoesNotExist_ShouldThrow()
        {
            // Arrange
            var vesselVisitId = CreateVesselVisitId();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                CargoManifest.Create(
                    isLoadingManifest: true,
                    cargoVolume: 1000,
                    vesselVisitId,
                    vesselVisitExists: false,
                    manifestAlreadyExists: false
                );
            });

            Assert.Contains("does not exist", ex.Message);
        }

        [Fact]
        public void CreateManifest_WhenLoadingManifestAlreadyExists_ShouldThrow()
        {
            // Arrange
            var vesselVisitId = CreateVesselVisitId();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                CargoManifest.Create(
                    isLoadingManifest: true,
                    cargoVolume: 1000,
                    vesselVisitId,
                    vesselVisitExists: true,
                    manifestAlreadyExists: true
                );
            });

            Assert.Contains("loading manifest already exists", ex.Message);
        }

        [Fact]
        public void CreateManifest_WhenUnloadingManifestAlreadyExists_ShouldThrow()
        {
            // Arrange
            var vesselVisitId = CreateVesselVisitId();

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                CargoManifest.Create(
                    isLoadingManifest: false,
                    cargoVolume: 1000,
                    vesselVisitId,
                    vesselVisitExists: true,
                    manifestAlreadyExists: true
                );
            });

            Assert.Contains("unloading manifest already exists", ex.Message);
        }

        [Fact]
        public void AddContainer_WithValidContainerId_ShouldSucceed()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );
            var containerId = new ContainerId(Guid.NewGuid());

            // Act
            manifest.AddContainer(containerId);

            // Assert
            Assert.Single(manifest.ContainerIds);
            Assert.Contains(containerId, manifest.ContainerIds);
        }

        [Fact]
        public void AddContainer_WithNullContainerId_ShouldThrow()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manifest.AddContainer(null));
        }

        [Fact]
        public void AddContainer_DuplicateContainer_ShouldThrow()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );
            var containerId = new ContainerId(Guid.NewGuid());
            manifest.AddContainer(containerId);

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                manifest.AddContainer(containerId);
            });

            Assert.Contains("already in this manifest", ex.Message);
        }

        [Fact]
        public void RemoveContainer_WhenContainerExists_ShouldSucceed()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );
            var containerId = new ContainerId(Guid.NewGuid());
            manifest.AddContainer(containerId);

            // Act
            manifest.RemoveContainer(containerId);

            // Assert
            Assert.Empty(manifest.ContainerIds);
        }

        [Fact]
        public void RemoveContainer_WhenContainerDoesNotExist_ShouldThrow()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );
            var containerId = new ContainerId(Guid.NewGuid());

            // Act & Assert
            var ex = Assert.Throws<BusinessRuleValidationException>(() =>
            {
                manifest.RemoveContainer(containerId);
            });

            Assert.Contains("not in this manifest", ex.Message);
        }

        [Fact]
        public void RemoveContainer_WithNullContainerId_ShouldThrow()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manifest.RemoveContainer(null));
        }

        [Fact]
        public void AddMultipleContainers_ShouldSucceed()
        {
            // Arrange
            var manifest = CargoManifest.Create(
                true, 1000, CreateVesselVisitId(), true, false
            );
            var container1 = new ContainerId(Guid.NewGuid());
            var container2 = new ContainerId(Guid.NewGuid());
            var container3 = new ContainerId(Guid.NewGuid());

            // Act
            manifest.AddContainer(container1);
            manifest.AddContainer(container2);
            manifest.AddContainer(container3);

            // Assert
            Assert.Equal(3, manifest.ContainerIds.Count);
        }
    }
}