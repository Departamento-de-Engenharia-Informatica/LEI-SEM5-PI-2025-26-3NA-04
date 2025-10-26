using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.ManifestAggregate.Repos {
    public interface ICargoManifestRepository : IRepository<CargoManifest, CargoManifestId> {
        Task<bool> HasManifestForVesselVisitAsync(Guid vesselVisitNotificationId, bool isLoadingManifest);
        
        Task<List<CargoManifest>> GetByVesselVisitAsync(Guid vesselVisitNotificationId);
    }
}