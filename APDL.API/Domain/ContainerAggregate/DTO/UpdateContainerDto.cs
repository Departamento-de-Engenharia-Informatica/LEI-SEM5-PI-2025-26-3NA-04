using System;

namespace APDL.API.Domain.ContainerAggregate.DTO
{
    public class UpdateContainerDto
    {
        public Guid Id { get; set; }
        public string CargoType { get; set; }
        public string Description { get; set; }
        public string SpecialRequirements { get; set; }
        public int Bay { get; set; }
        public int Row { get; set; }
        public int Tier { get; set; }
    }
}