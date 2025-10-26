using System;

namespace APDL.API.Domain.ManifestAggregate.DTO
{
    public class CreateCargoManifestDto
    {
        public bool IsLoadingManifest { get; set; }
        public int CargoVolume { get; set; }
        public Guid VesselVisitNotificationId { get; set; }
    }
}