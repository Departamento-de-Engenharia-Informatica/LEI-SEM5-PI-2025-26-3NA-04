using DDDSample1.Domain.VesselTypes;
using DDDSample1.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DDDSample1.Domain.VesselTypes
{
    public interface IVesselTypeRepository : IRepository<VesselType, VesselTypeId>
    {
        Task<List<VesselType>> GetByNameOrDescriptionAsync(string searchTerm);
    }
}
