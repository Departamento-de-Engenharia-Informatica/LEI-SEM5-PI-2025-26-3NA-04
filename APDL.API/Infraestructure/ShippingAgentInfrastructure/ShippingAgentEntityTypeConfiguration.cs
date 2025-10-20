using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.ShippingAgentAggregate;

namespace APDL.API.Infrastructure.ShippingAgentInfrastructure
{
    internal class ShippingAgentEntityTypeConfiguration : IEntityTypeConfiguration<ShippingAgent>
    {
        public void Configure(EntityTypeBuilder<ShippingAgent> builder)
        {
            builder.ToTable("ShippingAgents", SchemaNames.port);

            builder.HasKey(sa => sa.Id);

            builder.Property(sa => sa.Name)
                   .IsRequired();

            builder.HasMany<ShippingAgentRepresentative>("_representatives")
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
