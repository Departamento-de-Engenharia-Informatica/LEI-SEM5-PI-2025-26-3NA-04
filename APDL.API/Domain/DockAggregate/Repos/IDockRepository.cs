using System.Collections.Generic;
using System.Threading.Tasks;
using APDL.API.Domain.Shared;

namespace APDL.API.Domain.DockAggregate.Repos
{
    public interface IDockRepository : IRepository<Dock, DockId>
    {
        Task<Dock> GetByNameAsync(string dockName);
        Task<bool> DockNameExistsAsync(string dockName);
        Task<List<Dock>> GetDocksCapableOfVesselAsync(int vesselLength, int vesselDraft);
    }
}
