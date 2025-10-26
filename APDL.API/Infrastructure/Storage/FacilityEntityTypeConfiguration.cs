
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.Storage;
using APDL.API.Domain.Dock;

namespace APDL.API.Infrastructure.Storage
{
    

public class FacilityEntityTypeConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facilities", SchemaNames.port);
        
        builder.HasKey(f => f.Id);

        builder.OwnsOne(f => f.Location, loc =>
        {
            loc.Property(l => l.Value)
                .HasColumnName("Location")
                .IsRequired();
        });

        builder.OwnsOne(f => f.MaxCapacityTEU, cap =>
        {
            cap.Property(c => c.Value)
                .HasColumnName("MaxCapacityTEU")
                .IsRequired();
        });

        builder.Property(f => f.CurrentOccupancyTEU)
            .IsRequired();

        builder.HasDiscriminator<string>("FacilityType")
            .HasValue<Yard>("Yard")
            .HasValue<Warehouse>("Warehouse");

        builder.OwnsMany(f => f.DockAssignments, da =>
        {
            da.WithOwner().HasForeignKey("FacilityId");

            da.Property(d => d.DockId)
                .HasConversion(
                    id => id.AsGuid(),
                    guid => new DockId(guid))
                .HasColumnName("DockId")
                .IsRequired();

            da.OwnsOne(d => d.DistanceToDock, d =>
            {
                d.Property(p => p.Meters)
                    .HasColumnName("DistanceToDock")
                    .IsRequired();
            });

            da.ToTable("FacilityDockAssignments", SchemaNames.port);
        });
    }
}


}
