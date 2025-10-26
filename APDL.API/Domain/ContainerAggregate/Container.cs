using System;
using APDL.API.Domain.ContainerAggregate.ValueObjects;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ContainerAggregate
{
    public class Container : Entity<ContainerId>, IAggregateRoot
    {
        public ContainerNumber ContainerNumber { get; private set; }
        public CargoType CargoType { get; private set; }
        public ContainerDescription Description { get; private set; }
        public SpecialRequirements SpecialRequirements { get; private set; }
        public VesselPosition Position { get; private set; }

        protected Container() { }

        private Container(
            ContainerId id,
            ContainerNumber containerNumber,
            CargoType cargoType,
            ContainerDescription description,
            SpecialRequirements specialRequirements,
            VesselPosition position)
        {
            Id = id;
            ContainerNumber = containerNumber;
            CargoType = cargoType;
            Description = description;
            SpecialRequirements = specialRequirements;
            Position = position;
        }

        public static Container Create(
            string containerNumber,
            string cargoType,
            string description,
            string specialRequirements,
            int bay,
            int row,
            int tier)
        {
            return new Container(
                new ContainerId(Guid.NewGuid()),
                new ContainerNumber(containerNumber),
                new CargoType(cargoType),
                new ContainerDescription(description),
                new SpecialRequirements(specialRequirements),
                new VesselPosition(bay, row, tier)
            );
        }

        public void UpdateCargoType(string cargoType)
        {
            CargoType = new CargoType(cargoType);
        }

        public void UpdateDescription(string description)
        {
            Description = new ContainerDescription(description);
        }

        public void UpdateSpecialRequirements(string specialRequirements)
        {
            SpecialRequirements = new SpecialRequirements(specialRequirements);
        }

        public void UpdatePosition(int bay, int row, int tier)
        {
            Position = new VesselPosition(bay, row, tier);
        }
    }
}