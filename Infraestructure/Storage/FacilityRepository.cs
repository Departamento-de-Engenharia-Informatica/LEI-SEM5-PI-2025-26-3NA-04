
using DDDSample1.Domain.Storage;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.Storage
{
    public class FacilityRepository : BaseRepository<Facility, FacilityId>, IFacilityRepository
    {
        public FacilityRepository(DDDSample1DbContext context) : base(context.Facilities)
        {
        }
    }
}
