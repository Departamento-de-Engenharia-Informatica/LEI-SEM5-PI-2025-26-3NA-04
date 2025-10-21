using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DDDSample1.Domain.VesselTypes;
using DDDSample1.Infrastructure.Shared;

namespace DDDSample1.Infrastructure.VesselTypes
{
    public class VesselTypeRepository : BaseRepository<VesselType, VesselTypeId>, IVesselTypeRepository
    {

        private readonly DDDSample1DbContext _context;

        public VesselTypeRepository(DDDSample1DbContext context) : base(context.VesselTypes)
        {
            _context = context;
        }

        public async Task<List<VesselType>> GetByNameOrDescriptionAsync(string searchTerm)
        {
            return await _context.VesselTypes
                .Where(v => v.Name.Contains(searchTerm) || v.Description.Contains(searchTerm))
                .ToListAsync();
        }


        
    }
}
