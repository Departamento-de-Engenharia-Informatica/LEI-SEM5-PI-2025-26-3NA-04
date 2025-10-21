
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.Storage;

namespace APDL.API.Infrastructure.Storage
{
    internal class FacilityEntityTypeConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> builder)
        {
            builder.ToTable("Facilities", SchemaNames.port);
            builder.HasKey(f => f.Id);

            
            builder.HasDiscriminator<string>("FacilityType")
                   .HasValue<Yard>("Yard")
                   .HasValue<Warehouse>("Warehouse");

        }
    }
}
