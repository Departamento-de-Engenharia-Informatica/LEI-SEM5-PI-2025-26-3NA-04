using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.Vessels;

namespace APDL.API.Infrastructure.Vessels
{
    internal class VesselEntityTypeConfiguration : IEntityTypeConfiguration<Vessel>
    {
        public void Configure(EntityTypeBuilder<Vessel> builder)
        {
            
            builder.ToTable("Vessels", SchemaNames.port);

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .HasConversion(
                    id => id.AsGuid(),
                    guid => new VesselId(guid)
                );

            builder.OwnsOne(v => v.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(v => v.ImoNumber, imo =>
            {
                imo.Property(i => i.Value)
                    .HasColumnName("ImoNumber")
                    .HasMaxLength(7)
                    .IsRequired();
            });

            builder.OwnsOne(v => v.Operator, op =>
            {
                op.Property(o => o.Value)
                    .HasColumnName("Operator")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            builder.Property(v => v.VesselTypeId)
                .IsRequired();
        }
    }
}