using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Domain.Vessels;
using Microsoft.EntityFrameworkCore;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.Vessels
{
    public class VesselRepository : BaseRepository<Vessel, VesselId>, IVesselRepository
    {

        private readonly DDDSample1DbContext _context;
        public VesselRepository(DDDSample1DbContext context) : base(context.Vessel)
        {
            _context = context;
        }

        
        public async Task<List<Vessel>> SearchByNameAsync(string name)
        {
            return await _context.Set<Vessel>()
                .Where(v => v.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<List<Vessel>> SearchByOperatorAsync(string operatorName)
        {
            return await _context.Set<Vessel>()
                .Where(v => v.Operator.Contains(operatorName))
                .ToListAsync();
        }


        public async Task<Vessel> GetByImoAsync(string imoNumber)
        {
            return await _context.Set<Vessel>()
                .FirstOrDefaultAsync(v => v.ImoNumber == imoNumber);
        }
    }
}