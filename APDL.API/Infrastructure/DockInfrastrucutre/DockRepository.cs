using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure.DockInfrastructure
{
    public class DockRepository : BaseRepository<Dock, DockId>, IDockRepository
    {
        private readonly DDDSample1DbContext _context;

        public DockRepository(DDDSample1DbContext context)
            : base(context.Docks)
        {
            _context = context;
        }

        public async Task<Dock> GetByNameAsync(string dockName)
        {
            return await _context
                .Docks.Include(d => d.STSCranes)
                .FirstOrDefaultAsync(d => d.DockName == dockName.Trim());
        }

        public async Task<bool> DockNameExistsAsync(string dockName)
        {
            return await _context.Docks.AnyAsync(d => d.DockName == dockName.Trim());
        }

        public async Task<List<Dock>> GetDocksCapableOfVesselAsync(
            int vesselLength,
            int vesselDraft
        )
        {
            return await _context
                .Docks.Include(d => d.STSCranes)
                .Where(d => d.DockLength >= vesselLength && d.DockDraft >= vesselDraft)
                .ToListAsync();
        }

        public async Task<Dock> GetDockByCraneIdAsync(StsCraneId craneId)
        {
            return await _context
                .Docks.Include(d => d.STSCranes)
                .FirstOrDefaultAsync(d => d.STSCranes.Any(c => c.Id == craneId));
        }
    }
}
