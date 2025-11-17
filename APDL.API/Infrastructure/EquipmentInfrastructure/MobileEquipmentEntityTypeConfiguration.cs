using APDL.API.Domain.EquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate;
using APDL.API.Domain.MobileEquipmentAggregate.ValueObjects;
using APDL.API.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.MobileEquipmentInfrastructure
{
    internal class MobileEquipmentEntityTypeConfiguration
        : IEntityTypeConfiguration<MobileEquipment>
    {
        public void Configure(EntityTypeBuilder<MobileEquipment> builder)
        {
            builder.ToTable("MobileEquipment", SchemaNames.port);

            builder.HasKey(e => e.Id);

            builder
                .Property(e => e.Id)
                .HasConversion(id => id.Value, value => new MobileEquipmentId(value))
                .IsRequired();

            builder
                .Property(e => e.EquipmentName)
                .HasColumnName("EquipmentName")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(
                e => e.EquipmentType,
                et =>
                {
                    et.Property(t => t.Value)
                        .HasColumnName("EquipmentType")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );

            builder.OwnsOne(
                e => e.OperationalWindow,
                ow =>
                {
                    ow.Property(o => o.StartDay).HasColumnName("OperationalStartDay").IsRequired();

                    ow.Property(o => o.StartTime)
                        .HasColumnName("OperationalStartTime")
                        .IsRequired();

                    ow.Property(o => o.EndDay).HasColumnName("OperationalEndDay").IsRequired();

                    ow.Property(o => o.EndTime).HasColumnName("OperationalEndTime").IsRequired();

                    ow.Property(o => o.Is24x7).HasColumnName("Is24x7").IsRequired();
                }
            );

            builder.OwnsOne(
                e => e.Capacity,
                c =>
                {
                    c.Property(cap => cap.ContainersPerTrip).HasColumnName("ContainersPerTrip");

                    c.Property(cap => cap.AverageSpeedPerHour).HasColumnName("AverageSpeedPerHour");

                    c.Property(cap => cap.ContainersPerHour).HasColumnName("ContainersPerHour");
                }
            );

            builder.OwnsOne(
                e => e.Status,
                s =>
                {
                    s.Property(status => status.Value)
                        .HasColumnName("Status")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );

            builder
                .Property(e => e.RequiredOperators)
                .HasColumnName("RequiredOperators")
                .IsRequired();

            builder.OwnsOne(
                e => e.RequiredQualification,
                q =>
                {
                    q.Property(qual => qual.Value)
                        .HasColumnName("RequiredQualification")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );

            builder.Property(e => e.SetupTime).HasColumnName("SetupTime").IsRequired();

            builder.HasIndex(e => e.EquipmentName);
        }
    }
}
