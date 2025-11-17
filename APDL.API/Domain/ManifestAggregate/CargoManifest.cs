using System;
using System.Collections.Generic;
using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ManifestAggregate
{
    public class CargoManifest : Entity<CargoManifestId>, IAggregateRoot
    {
        public bool IsLoadingManifest { get; private set; }
        public CargoVolume CargoVolume { get; private set; }
        public VesselVisitNotificationId VesselVisitNotificationId { get; private set; }

        private readonly List<ContainerId> _containerIds = new();
        public IReadOnlyCollection<ContainerId> ContainerIds => _containerIds.AsReadOnly();

        protected CargoManifest() { }

        private CargoManifest(
            CargoManifestId id,
            bool isLoadingManifest,
            CargoVolume cargoVolume,
            VesselVisitNotificationId vesselVisitNotificationId
        )
        {
            Id = id;
            IsLoadingManifest = isLoadingManifest;
            CargoVolume = cargoVolume;
            VesselVisitNotificationId = vesselVisitNotificationId;
        }

        public static CargoManifest Create(
            bool isLoadingManifest,
            int cargoVolume,
            VesselVisitNotificationId vesselVisitNotificationId,
            bool vesselVisitExists,
            bool manifestAlreadyExists
        )
        {
            if (!vesselVisitExists)
            {
                throw new BusinessRuleValidationException(
                    $"Cannot create manifest. Vessel visit notification with ID {vesselVisitNotificationId.Value} does not exist."
                );
            }

            if (manifestAlreadyExists)
            {
                var manifestType = isLoadingManifest ? "loading" : "unloading";
                throw new BusinessRuleValidationException(
                    $"Cannot create manifest. A {manifestType} manifest already exists for vessel visit {vesselVisitNotificationId.Value}."
                );
            }

            var cargoVolumeVO = new CargoVolume(cargoVolume);

            return new CargoManifest(
                new CargoManifestId(Guid.NewGuid()),
                isLoadingManifest,
                cargoVolumeVO,
                vesselVisitNotificationId
            );
        }

        public void AddContainer(ContainerId containerId)
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));

            if (_containerIds.Contains(containerId))
                throw new BusinessRuleValidationException(
                    $"Container {containerId.Value} is already in this manifest."
                );

            _containerIds.Add(containerId);
        }

        public void RemoveContainer(ContainerId containerId)
        {
            if (containerId == null)
                throw new ArgumentNullException(nameof(containerId));

            if (!_containerIds.Contains(containerId))
                throw new BusinessRuleValidationException(
                    $"Container {containerId.Value} is not in this manifest."
                );

            _containerIds.Remove(containerId);
        }

        public void updateCargoVolume(int cargoVolume)
        {
            this.CargoVolume = new CargoVolume(cargoVolume);
        }
    }
}
