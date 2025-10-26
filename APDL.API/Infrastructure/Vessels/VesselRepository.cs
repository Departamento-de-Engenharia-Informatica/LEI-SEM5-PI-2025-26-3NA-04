using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APDL.API.Domain.Vessels;
using Microsoft.EntityFrameworkCore;
using APDL.API.Infrastructure.Shared;
using System;

namespace APDL.API.Infrastructure.Vessels
{
    public class VesselRepository : BaseRepository<Vessel, VesselId>, IVesselRepository
    {
        private readonly DDDSample1DbContext _context;

        public VesselRepository(DDDSample1DbContext context) : base(context.Vessels)
        {
            _context = context;
        }

        public async Task<List<Vessel>> SearchByNameAsync(string name)
        {
            return await _context.Set<Vessel>()
                .Where(v => v.Name.Value.ToLower() == name.ToLower())
                .ToListAsync();
        }

        public async Task<List<Vessel>> SearchByOperatorAsync(string operatorName)
        {
            return await _context.Set<Vessel>()
                .Where(v => v.Operator.Value.ToLower() == operatorName.ToLower())
                .ToListAsync();
        }

        public async Task<Vessel> GetByImoAsync(string imoNumber)
        {
            return await _context.Set<Vessel>()
                .FirstOrDefaultAsync(v => v.ImoNumber.Value == imoNumber);
        }
    }
}