using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APDL.API.Domain.VesselTypes
{
    public interface IVesselTypeRepository : IRepository<VesselType, VesselTypeId>
    {
        Task<List<VesselType>> GetByNameOrDescriptionAsync(string searchTerm);
    }
}
