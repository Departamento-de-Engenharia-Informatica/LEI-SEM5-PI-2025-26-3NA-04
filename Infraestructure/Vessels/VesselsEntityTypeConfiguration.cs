using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.Vessels;

namespace DDDSample1.Infrastructure.Vessels
{
    internal class VesselEntityTypeConfiguration : IEntityTypeConfiguration<Vessel>
    {
        public void Configure(EntityTypeBuilder<Vessel> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .HasConversion(
                    id => id.AsGuid(),
                    guid => new VesselId(guid)
                );

            builder.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.ImoNumber)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(v => v.Operator)
                .IsRequired()
                .HasMaxLength(50);


            builder.Property(v => v.VesselTypeId)
                .IsRequired();
        }
    }
}
