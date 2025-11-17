using System;
using APDL.API.Domain.DockAggregate;
using APDL.API.Domain.DockAggregate.ValueObjects;
using APDL.API.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.DockInfrastructure
{
    internal class StsCraneEntityTypeConfiguration : IEntityTypeConfiguration<StsCrane>
    {
        public void Configure(EntityTypeBuilder<StsCrane> builder)
        {
            builder.ToTable("StsCranes", SchemaNames.port);

            builder.HasKey(c => c.Id);

            builder
                .Property(c => c.Id)
                .HasConversion(id => id.Value, value => new StsCraneId(value))
                .IsRequired();

            builder
                .Property(c => c.DockId)
                .HasConversion(id => id.Value, value => new DockId(value))
                .IsRequired();

            builder
                .Property(c => c.CraneName)
                .HasColumnName("CraneName")
                .HasMaxLength(100)
                .IsRequired();

            builder.OwnsOne(
                c => c.OperationalWindow,
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

            builder
                .Property(c => c.CapacityContainersPerHour)
                .HasColumnName("CapacityContainersPerHour")
                .IsRequired();

            builder.OwnsOne(
                c => c.Status,
                s =>
                {
                    s.Property(status => status.Value)
                        .HasColumnName("Status")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );

            builder
                .Property(c => c.RequiredOperators)
                .HasColumnName("RequiredOperators")
                .IsRequired();

            builder.OwnsOne(
                c => c.RequiredQualification,
                q =>
                {
                    q.Property(qual => qual.Value)
                        .HasColumnName("RequiredQualification")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );

            builder.Property(c => c.SetupTime).HasColumnName("SetupTime").IsRequired();

            builder.Ignore(c => c.UpcomingMaintenances);

            builder
                .HasOne<Dock>()
                .WithMany(d => d.STSCranes)
                .HasForeignKey(c => c.DockId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.DockId);
        }
    }
}
