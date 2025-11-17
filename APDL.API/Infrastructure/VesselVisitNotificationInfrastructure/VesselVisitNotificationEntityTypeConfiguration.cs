using APDL.API.Domain.Dock;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.NotificationAggregate;
using APDL.API.Domain.NotificationAggregate.ValueObjects;
using APDL.API.Domain.ShippingAgentAggregate;
using APDL.API.Domain.Vessels;
using APDL.API.Domain.VesselVisitAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.VesselVisitInfrastructure
{
    internal class VesselVisitNotificationEntityTypeConfiguration
        : IEntityTypeConfiguration<VesselVisitNotification>
    {
        public void Configure(EntityTypeBuilder<VesselVisitNotification> builder)
        {
            builder.ToTable("VesselVisitNotifications", SchemaNames.port);

            builder.HasKey(v => v.Id);

            builder
                .Property(v => v.Id)
                .HasConversion(id => id.Value, value => new VesselVisitNotificationId(value))
                .IsRequired();

            builder
                .Property(v => v.VesselId)
                .HasConversion(id => id.Value, value => new VesselId(value))
                .IsRequired();

            builder
                .Property(v => v.ShippingAgentId)
                .HasConversion(id => id.Value, value => new ShippingAgentId(value))
                .IsRequired();

            builder
                .Property(v => v.AssignedDockId)
                .HasConversion(id => id.Value, value => new DockId(value));

            builder.OwnsOne(
                v => v.ExpectedArrival,
                ea =>
                {
                    ea.Property(a => a.Value).HasColumnName("ExpectedArrival").IsRequired();
                }
            );

            builder.OwnsOne(
                v => v.ExpectedDeparture,
                ed =>
                {
                    ed.Property(d => d.Value).HasColumnName("ExpectedDeparture").IsRequired();
                }
            );

            builder.OwnsOne(
                v => v.Status,
                s =>
                {
                    s.Property(st => st.Value)
                        .HasColumnName("Status")
                        .HasMaxLength(20)
                        .IsRequired();
                }
            );

            builder
                .Property(v => v.CargoType)
                .HasColumnName("CargoType")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(v => v.CargoVolume).HasColumnName("CargoVolume").IsRequired();

            builder
                .Property(v => v.SpecialHandlingRequirements)
                .HasColumnName("SpecialHandlingRequirements")
                .HasMaxLength(500);

            builder
                .Property(v => v.CaptainName)
                .HasColumnName("CaptainName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(v => v.TotalCrewCount).HasColumnName("TotalCrewCount").IsRequired();

            builder
                .Property(v => v.RejectionReason)
                .HasColumnName("RejectionReason")
                .HasMaxLength(1000);

            builder.Property(v => v.CreatedAt).HasColumnName("CreatedAt").IsRequired();

            builder.Property(v => v.SubmittedAt).HasColumnName("SubmittedAt");

            builder.Property(v => v.ReviewedAt).HasColumnName("ReviewedAt");

            builder.Ignore(v => v.SafetyCrewOfficers);

            builder.OwnsMany<SafetyOfficer>(
                "_safetyCrewOfficers",
                officers =>
                {
                    officers.ToTable("VesselVisitSafetyOfficers", SchemaNames.port);

                    officers.WithOwner().HasForeignKey("VesselVisitNotificationId");

                    officers.Property<int>("Id").ValueGeneratedOnAdd();

                    officers.HasKey("Id");

                    officers
                        .Property(o => o.Name)
                        .HasColumnName("OfficerName")
                        .HasMaxLength(100)
                        .IsRequired();
                }
            );

            builder.Ignore(v => v.CargoManifestIds);

            builder.OwnsMany<CargoManifestId>(
                "_cargoManifestIds",
                manifests =>
                {
                    manifests.ToTable("VesselVisitCargoManifests", SchemaNames.port);

                    manifests.WithOwner().HasForeignKey("VesselVisitNotificationId");

                    manifests.Property<int>("Id").ValueGeneratedOnAdd();

                    manifests.HasKey("Id");

                    manifests.Property(m => m.Value).HasColumnName("CargoManifestId").IsRequired();
                }
            );

            builder.HasIndex(v => v.VesselId);
            builder.HasIndex(v => v.ShippingAgentId);
            builder.HasIndex(v => v.AssignedDockId);
            builder.HasIndex(v => v.Status);
        }
    }
}
