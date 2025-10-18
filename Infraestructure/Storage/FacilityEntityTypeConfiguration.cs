
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DDDSample1.Domain.Storage;

namespace DDDSample1.Infrastructure.Storage
{
    internal class FacilityEntityTypeConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> builder)
        {
            builder.ToTable("Facilities", SchemaNames.DDDSample1);
            builder.HasKey(f => f.Id);

            
            builder.HasDiscriminator<string>("FacilityType")
                   .HasValue<Yard>("Yard")
                   .HasValue<Warehouse>("Warehouse");

        }
    }
}
