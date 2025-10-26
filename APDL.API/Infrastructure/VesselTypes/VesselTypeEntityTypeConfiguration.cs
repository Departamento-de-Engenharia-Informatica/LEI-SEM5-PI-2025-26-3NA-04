
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.VesselTypes;
namespace APDL.API.Infrastructure.VesselTypes
{
    
    public class VesselTypeEntityTypeConfiguration : IEntityTypeConfiguration<VesselType>
    {
        public void Configure(EntityTypeBuilder<VesselType> builder)
        {
            builder.ToTable("VesselTypes", SchemaNames.port);

            builder.HasKey(v => v.Id);

            builder.OwnsOne(v => v.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.Description, desc =>
            {
                desc.Property(d => d.Value)
                    .HasColumnName("Description")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.Capacity, cap =>
            {
                cap.Property(c => c.Value)
                    .HasColumnName("Capacity")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.MaxRows, rows =>
            {
                rows.Property(r => r.Value)
                    .HasColumnName("MaxRows")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.MaxBays, bays =>
            {
                bays.Property(b => b.Value)
                    .HasColumnName("MaxBays")
                    .IsRequired();
            });

            builder.OwnsOne(v => v.MaxTiers, tiers =>
            {
                tiers.Property(t => t.Value)
                    .HasColumnName("MaxTiers")
                    .IsRequired();
            });
        }
    }

}
