using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.ManifestAggregate.Repos;
using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using APDL.API.Infrastructure.Shared;

namespace APDL.API.Infrastructure.ManifestInfrastructure
{
    public class CargoManifestRepository : BaseRepository<CargoManifest, CargoManifestId>, ICargoManifestRepository
    {
        private readonly DDDSample1DbContext _context;

        public CargoManifestRepository(DDDSample1DbContext context) : base(context.CargoManifests)
        {
            _context = context;
        }

        public async Task<bool> HasManifestForVesselVisitAsync(
            Guid vesselVisitNotificationId, 
            bool isLoadingManifest)
        {
            return await _context.CargoManifests
                .AnyAsync(m => 
                    m.VesselVisitNotificationId.AsGuid() == vesselVisitNotificationId && 
                    m.IsLoadingManifest == isLoadingManifest);
        }

        public async Task<List<CargoManifest>> GetByVesselVisitAsync(Guid vesselVisitNotificationId)
        {
            return await _context.CargoManifests
                .Where(m => m.VesselVisitNotificationId.AsGuid() == vesselVisitNotificationId)
                .ToListAsync();
        }
    }
}