using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Infrastructure.Shared;

namespace APDL.API.Infrastructure.ShippingAgentInfrastructure
{
    public class ShippingAgentRepresentativeRepository 
        : BaseRepository<ShippingAgentRepresentative, ShippingAgentRepresentativeId>, IShippingAgentRepresentativeRepository
    {

         private readonly DDDSample1DbContext _context;

        public ShippingAgentRepresentativeRepository(DDDSample1DbContext context)
            : base(context.ShippingAgentRepresentatives)
        {
            _context = context;
        }

        public async Task<ShippingAgentRepresentative?> GetByCitizenIdAsync(string citizenId)
        {
            return await _context.Set<ShippingAgentRepresentative>()
                .FirstOrDefaultAsync(r => r.CitizenId.Value == citizenId);
        }

        public async Task<ShippingAgentRepresentative?> GetByEmailAsync(string email)
        {
            return await _context.Set<ShippingAgentRepresentative>()
                .FirstOrDefaultAsync(r => r.Email.Value == email);
        }

    }
}
