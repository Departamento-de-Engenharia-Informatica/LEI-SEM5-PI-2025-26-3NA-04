using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.EquipmentAggregate;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.OperatingStaffAggregate;
using APDL.API.Domain.PrivacyPolicyAggregate;
using APDL.API.Domain.QualificationsAggregate;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.UserAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselTypes;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Infrastructure.ContainerInfrastructure;
using APDL.API.Infrastructure.DockInfrastructure;
using APDL.API.Infrastructure.ManifestInfrastructure;
using APDL.API.Infrastructure.MobileEquipmentInfrastructure;
using APDL.API.Infrastructure.OperatingStaffInfrastructure;
using APDL.API.Infrastructure.PrivacyPolicyInfrastructure;
using APDL.API.Infrastructure.ShippingAgentInfrastructure;
using APDL.API.Infrastructure.StaffQualificationInfrastructure;
using APDL.API.Infrastructure.UserInfrastructure;
using APDL.API.Infrastructure.Vessels;
using APDL.API.Infrastructure.VesselTypes;
using APDL.API.Infrastructure.VesselVisitInfrastructure;
using Microsoft.EntityFrameworkCore;

namespace APDL.API.Infrastructure
{
    public class DDDSample1DbContext : DbContext
    {
        public DbSet<ShippingAgent> ShippingAgents { get; set; }
        public DbSet<ShippingAgentRepresentative> ShippingAgentRepresentatives { get; set; }
        public DbSet<CargoManifest> CargoManifests { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<VesselVisitNotification> VesselVisitNotifications { get; set; }
        // public DbSet<Facility> Facilities { get; set; }
        public DbSet<VesselType> VesselTypes { get; set; }
        public DbSet<Dock> Docks { get; set; }
        public DbSet<StaffQualification> StaffQualifications { get; set; }
        public DbSet<OperatingStaff> OperatingStaff { get; set; }
        public DbSet<MobileEquipment> MobileEquipment { get; set; }
        public DbSet<Vessel> Vessels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }

        public DDDSample1DbContext(DbContextOptions<DDDSample1DbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ShippingAgentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(
                new ShippingAgentRepresentativeEntityTypeConfiguration()
            );
            modelBuilder.ApplyConfiguration(new CargoManifestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ContainerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VesselVisitNotificationEntityTypeConfiguration());
            // modelBuilder.ApplyConfiguration(new FacilityEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VesselTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StsCraneEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new VesselEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DockEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StaffQualificationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperatingStaffEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MobileEquipmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PrivacyPolicyEntityTypeConfiguration());
        }
    }
}
