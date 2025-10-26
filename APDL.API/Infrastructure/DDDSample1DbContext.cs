using Microsoft.EntityFrameworkCore;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Storage;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.Vessels;
using APDL.API.Infrastructure.ShippingAgentInfrastructure;
using APDL.API.Infrastructure.Storage; 
using APDL.API.Infrastructure.VesselTypes;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Infrastructure.ManifestInfrastructure;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Infrastructure.ContainerInfrastructure;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure.VesselVisitInfrastructure;
using APDL.API.Infrastructure.Vessels;


namespace APDL.API.Infrastructure
{
    public class DDDSample1DbContext : DbContext
    {
        public DbSet<ShippingAgent> ShippingAgents { get; set; }
        public DbSet<ShippingAgentRepresentative> ShippingAgentRepresentatives { get; set; }
        public DbSet<CargoManifest> CargoManifests { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<VesselVisitNotification> VesselVisitNotifications { get; set; }
        public DbSet<Facility> Facilities { get; set; }

        public DbSet<VesselType> VesselTypes { get; set; }

        public DbSet<Vessel> Vessels { get; set; }

        public DDDSample1DbContext(DbContextOptions<DDDSample1DbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ShippingAgentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShippingAgentRepresentativeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CargoManifestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContainerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VesselVisitNotificationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FacilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VesselTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VesselEntityTypeConfiguration());
        }
    }
}