using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.VesselVisitAggregate;
using APDL.API.Domain.NotificationAggregate.ValueObjects;
using APDL.API.Domain.NotificationAggregate;

namespace APDL.API.Infrastructure.VesselVisitInfrastructure
{
    internal class VesselVisitNotificationEntityTypeConfiguration : IEntityTypeConfiguration<VesselVisitNotification>
    {
        public void Configure(EntityTypeBuilder<VesselVisitNotification> builder)
        {
            builder.ToTable("VesselVisitNotifications", SchemaNames.port);

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .HasConversion(
                    id => id.Value,
                    value => new VesselVisitNotificationId(value))
                .IsRequired();

            builder.OwnsOne(v => v.ExpectedArrival, ea =>
            {
                ea.Property(a => a.Value)
                    .HasColumnName("ExpectedArrival")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.ExpectedDeparture, ed =>
            {
                ed.Property(d => d.Value)
                    .HasColumnName("ExpectedDeparture")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.Status, s =>
            {
                s.Property(st => st.Value)
                    .HasColumnName("Status")
                    .HasMaxLength(20)
                    .IsRequired();
            });

            builder.OwnsMany<SafetyOfficer>("_safetyCrewOfficers", officers =>
            {
                officers.ToTable("VesselVisitSafetyOfficers", SchemaNames.port);
                
                officers.WithOwner()
                    .HasForeignKey("VesselVisitNotificationId");
                
                officers.Property<int>("Id")
                    .ValueGeneratedOnAdd();
                
                officers.HasKey("Id");
                
                officers.Property(o => o.Name)
                    .HasColumnName("OfficerName")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Metadata.FindNavigation(nameof(VesselVisitNotification.SafetyCrewOfficers))
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex("Status");
        }
    }
}