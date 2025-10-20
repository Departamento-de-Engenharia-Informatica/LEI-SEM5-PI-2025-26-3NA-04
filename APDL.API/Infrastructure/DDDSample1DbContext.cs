using Microsoft.EntityFrameworkCore;
using APDL.API.Domain.ShippingAgentAggregate; 
using APDL.API.Infrastructure.ShippingAgentInfrastructure; 

namespace APDL.API.Infrastructure
{
    public class DDDSample1DbContext : DbContext
    {
        public DbSet<ShippingAgent> ShippingAgents { get; set; }
        public DbSet<ShippingAgentRepresentative> ShippingAgentRepresentatives { get; set; }

        public DDDSample1DbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ShippingAgentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShippingAgentRepresentativeEntityTypeConfiguration());
        }
    }
}