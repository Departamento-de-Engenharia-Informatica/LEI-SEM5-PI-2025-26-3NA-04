using APDL.API.Domain.ShippingAgentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.ShippingAgentInfrastructure
{
    internal class ShippingAgentRepresentativeEntityTypeConfiguration
        : IEntityTypeConfiguration<ShippingAgentRepresentative>
    {
        public void Configure(EntityTypeBuilder<ShippingAgentRepresentative> builder)
        {
            builder.ToTable("ShippingAgentRepresentatives", SchemaNames.port);

            builder.HasKey(r => r.Id);

            builder
                .Property(sa => sa.Id)
                .HasConversion(id => id.AsGuid(), value => new ShippingAgentRepresentativeId(value))
            .IsRequired();

            builder.OwnsOne(
                r => r.Name,
                n => n.Property(nv => nv.Value).HasColumnName("Name").IsRequired()
            );
            builder.OwnsOne(
                r => r.CitizenId,
                c => c.Property(cv => cv.Value).HasColumnName("CitizenId").IsRequired()
            );
            builder.OwnsOne(
                r => r.Nationality,
                n => n.Property(nv => nv.Value).HasColumnName("Nationality").IsRequired()
            );
            builder.OwnsOne(
                r => r.Email,
                e => e.Property(ev => ev.Value).HasColumnName("Email").IsRequired()
            );
            builder.OwnsOne(
                r => r.Phone,
                p => p.Property(pv => pv.Value).HasColumnName("Phone").IsRequired()
            );

            builder.Property(r => r.IsActive).IsRequired();
        }
    }
}
