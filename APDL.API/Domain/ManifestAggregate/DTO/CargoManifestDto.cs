using System;
using System.Collections.Generic;

namespace APDL.API.Domain.ManifestAggregate.DTO
{
    public class CargoManifestDto
    {
        public Guid Id { get; set; }
        public bool IsLoadingManifest { get; set; }
        public int CargoVolume { get; set; }
        public Guid VesselVisitNotificationId { get; set; }
        public List<Guid> ContainerIds { get; set; } = new();
    }
}