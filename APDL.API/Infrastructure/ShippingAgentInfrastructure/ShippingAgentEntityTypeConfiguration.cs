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

            builder.OwnsOne(sa => sa.LegalName, ln =>
            {
                ln.Property(l => l.Value).HasColumnName("LegalName").IsRequired();
            });

            builder.OwnsOne(sa => sa.AlternativeName, an =>
            {
                an.Property(a => a.Value).HasColumnName("AlternativeName");
            });

            builder.OwnsOne(sa => sa.Address, ad =>
            {
                ad.Property(a => a.Value).HasColumnName("Address").IsRequired();
            });

            builder.OwnsOne(sa => sa.TaxNumber, tn =>
            {
                tn.Property(t => t.Value).HasColumnName("TaxNumber").IsRequired();
            });

            builder.Metadata.FindNavigation(nameof(ShippingAgent.Representatives))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
