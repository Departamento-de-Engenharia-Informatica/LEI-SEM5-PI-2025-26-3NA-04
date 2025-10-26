using System;
using System.Collections.Generic;

namespace APDL.API.Domain.ManifestAggregate.DTO
{
    public class UpdateCargoManifestDto
    {
        public Guid Id { get; set; }
        public int CargoVolume { get; set; }
    }

}