
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.VesselTypes;

namespace DDDSample1.Infrastructure.VesselTypes
{
    public class VesselTypeEntityTypeConfiguration : IEntityTypeConfiguration<VesselType>
    {
        public void Configure(EntityTypeBuilder<VesselType> builder)
        {
            builder.ToTable("VesselTypes", SchemaNames.DDDSample1);
            builder.HasKey(v => v.Id);
        }
    }
}
