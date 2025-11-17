using APDL.API.Domain.ContainerAggregate;
using APDL.API.Domain.ContainerAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.ContainerInfrastructure
{
    internal class ContainerEntityTypeConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.ToTable("Containers", SchemaNames.port);

            builder.HasKey(c => c.Id);

            builder
                .Property(c => c.Id)
                .HasConversion(id => id.Value, value => new ContainerId(value))
                .IsRequired();

            builder.OwnsOne(
                c => c.ContainerNumber,
                cn =>
                {
                    cn.Property(n => n.Value)
                        .HasColumnName("ContainerNumber")
                        .HasMaxLength(20)
                        .IsRequired();

                    cn.HasIndex(n => n.Value).IsUnique();
                }
            );

            builder.OwnsOne(
                c => c.CargoType,
                ct =>
                {
                    ct.Property(t => t.Value)
                        .HasColumnName("CargoType")
                        .HasMaxLength(50)
                        .IsRequired();
                }
            );

            builder.OwnsOne(
                c => c.Description,
                d =>
                {
                    d.Property(desc => desc.Value)
                        .HasColumnName("Description")
                        .HasMaxLength(500)
                        .IsRequired();
                }
            );

            builder.OwnsOne(
                c => c.SpecialRequirements,
                sr =>
                {
                    sr.Property(s => s.Value)
                        .HasColumnName("SpecialRequirements")
                        .HasMaxLength(500);
                }
            );

            builder.OwnsOne(
                c => c.Position,
                p =>
                {
                    p.Property(pos => pos.Bay).HasColumnName("Bay").IsRequired();

                    p.Property(pos => pos.Row).HasColumnName("Row").IsRequired();

                    p.Property(pos => pos.Tier).HasColumnName("Tier").IsRequired();
                }
            );
        }
    }
}
