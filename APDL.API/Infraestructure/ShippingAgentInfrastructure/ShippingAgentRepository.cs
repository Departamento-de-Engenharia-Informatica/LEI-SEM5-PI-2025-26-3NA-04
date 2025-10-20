using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Infrastructure.Shared;

namespace APDL.API.Infrastructure.ShippingAgentInfrastructure
{
    public class ShippingAgentRepository : BaseRepository<ShippingAgent, ShippingAgentId>, IShippingAgentRepository
    {
        public ShippingAgentRepository(APDLDbContext context):base(context.ShippingAgents) {
            modelBuilder.ApplyConfiguration(new ShippingAgentEntityTypeConfiguration());
        }
    }
}
