using APDL.API.Domain.DockAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.DockInfrastructure
{
    internal class DockEntityTypeConfiguration : IEntityTypeConfiguration<Dock>
    {
        public void Configure(EntityTypeBuilder<Dock> builder)
        {
            builder.ToTable("Docks", SchemaNames.port);

            builder.HasKey(d => d.Id);

            builder
                .Property(d => d.Id)
                .HasConversion(id => id.Value, value => new DockId(value))
                .IsRequired();

            builder
                .Property(d => d.DockName)
                .HasColumnName("DockName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(d => d.DockLength).HasColumnName("DockLength").IsRequired();

            builder.Property(d => d.DockDraft).HasColumnName("DockDraft").IsRequired();

            builder.OwnsOne(
                d => d.UpcomingMaintenances,
                um =>
                {
                    um.OwnsMany(
                        m => m.Schedules,
                        schedules =>
                        {
                            schedules.ToTable("DockMaintenanceSchedules", SchemaNames.port);

                            schedules.WithOwner();

                            schedules
                                .Property(s => s.ScheduledDate)
                                .HasColumnName("ScheduledDate")
                                .IsRequired();

                            schedules
                                .Property(s => s.EstimatedEndDate)
                                .HasColumnName("EstimatedEndDate")
                                .IsRequired();

                            schedules
                                .Property(s => s.Description)
                                .HasColumnName("Description")
                                .HasMaxLength(500)
                                .IsRequired();

                            schedules
                                .Property(s => s.Type)
                                .HasColumnName("Type")
                                .HasConversion<string>()
                                .IsRequired();
                        }
                    );
                }
            );

            builder.Navigation(d => d.STSCranes).AutoInclude();

            builder.HasIndex(d => d.DockName).IsUnique();
        }
    }
}
