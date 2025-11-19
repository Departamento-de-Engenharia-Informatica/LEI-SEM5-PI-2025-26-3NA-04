using APDL.API.Domain.QualificationsAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.StaffQualificationInfrastructure
{
    internal class StaffQualificationEntityTypeConfiguration
        : IEntityTypeConfiguration<StaffQualification>
    {
        public void Configure(EntityTypeBuilder<StaffQualification> builder)
        {
            builder.ToTable("StaffQualifications", SchemaNames.port);

            builder.HasKey(q => q.Id);

            builder
                .Property(q => q.Id)
                .HasConversion(id => id.Value, value => new StaffQualificationId(value))
                .IsRequired();

            builder.OwnsOne(
                q => q.QualificationType,
                qt =>
                {
                    qt.Property(t => t.Value)
                        .HasColumnName("QualificationType")
                        .HasConversion<string>()
                        .IsRequired();

                    qt.HasIndex(t => t.Value).IsUnique();
                }
            );

            builder
                .Property(q => q.QualificationName)
                .HasColumnName("QualificationName")
                .HasMaxLength(200)
                .IsRequired();

            builder
                .Property(q => q.Description)
                .HasColumnName("Description")
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(q => q.IsActive).HasColumnName("IsActive").IsRequired();
        }
    }
}
