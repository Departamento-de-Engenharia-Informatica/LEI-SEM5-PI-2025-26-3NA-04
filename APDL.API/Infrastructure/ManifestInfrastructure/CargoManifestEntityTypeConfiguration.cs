using APDL.API.Domain.CargoManifestAggregate.ValueObjects;
using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ManifestAggregate;
using APDL.API.Domain.NotificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.ManifestInfrastructure
{
    internal class CargoManifestEntityTypeConfiguration : IEntityTypeConfiguration<CargoManifest>
    {
        public void Configure(EntityTypeBuilder<CargoManifest> builder)
        {
            builder.ToTable("CargoManifests", SchemaNames.port);

            builder.HasKey(cm => cm.Id);

            builder
                .Property(cm => cm.Id)
                .HasConversion(id => id.Value, value => new CargoManifestId(value))
                .IsRequired();

            builder
                .Property(cm => cm.IsLoadingManifest)
                .HasColumnName("IsLoadingManifest")
                .IsRequired();

            builder
                .Property(cm => cm.VesselVisitNotificationId)
                .HasConversion(id => id.Value, value => new VesselVisitNotificationId(value))
                .HasColumnName("VesselVisitNotificationId")
                .IsRequired();

            builder.OwnsOne(
                cm => cm.CargoVolume,
                cv =>
                {
                    cv.Property(c => c.Value).HasColumnName("CargoVolume").IsRequired();
                }
            );

            builder.Ignore(cm => cm.ContainerIds);

            builder.OwnsMany<ContainerId>(
                "_containerIds",
                containerIds =>
                {
                    containerIds.ToTable("CargoManifestContainers", SchemaNames.port);

                    containerIds.WithOwner().HasForeignKey("CargoManifestId");

                    containerIds.Property<int>("Id").ValueGeneratedOnAdd();

                    containerIds.HasKey("Id");

                    containerIds.Property(c => c.Value).HasColumnName("ContainerId").IsRequired();
                }
            );
        }
    }
}
