using APDL.API.Domain.OperatingStaffAggregate;
using APDL.API.Domain.StaffAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APDL.API.Infrastructure.OperatingStaffInfrastructure
{
    internal class OperatingStaffEntityTypeConfiguration : IEntityTypeConfiguration<OperatingStaff>
    {
        public void Configure(EntityTypeBuilder<OperatingStaff> builder)
        {
            builder.ToTable("OperatingStaff", SchemaNames.port);

            builder.HasKey(s => s.Id);

            builder
                .Property(s => s.Id)
                .HasConversion(id => id.Value, value => new OperatingStaffId(value))
                .IsRequired();

            builder.OwnsOne(
                s => s.MecanographicNumber,
                mn =>
                {
                    mn.Property(m => m.Value)
                        .HasColumnName("MecanographicNumber")
                        .HasMaxLength(20)
                        .IsRequired();

                    mn.HasIndex(m => m.Value).IsUnique();
                }
            );

            builder
                .Property(s => s.ShortName)
                .HasColumnName("ShortName")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Email).HasColumnName("Email").HasMaxLength(200).IsRequired();

            builder.Property(s => s.Phone).HasColumnName("Phone").HasMaxLength(20).IsRequired();

            builder.OwnsOne(
                s => s.OperationalWindow,
                ow =>
                {
                    ow.Property(o => o.StartDay).HasColumnName("OperationalStartDay").IsRequired();

                    ow.Property(o => o.StartTime)
                        .HasColumnName("OperationalStartTime")
                        .IsRequired();

                    ow.Property(o => o.EndDay).HasColumnName("OperationalEndDay").IsRequired();

                    ow.Property(o => o.EndTime).HasColumnName("OperationalEndTime").IsRequired();

                    ow.Property(o => o.Is24x7).HasColumnName("Is24x7").IsRequired();
                }
            );

            builder.OwnsOne(
                s => s.Status,
                st =>
                {
                    st.Property(status => status.Value)
                        .HasColumnName("Status")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );

            builder.OwnsMany(
                s => s.Qualifications,
                qualifications =>
                {
                    qualifications.ToTable("OperatingStaffQualifications", SchemaNames.port);

                    qualifications.WithOwner().HasForeignKey("OperatingStaffId");

                    qualifications.Property<int>("Id");
                    qualifications.HasKey("Id");

                    qualifications
                        .Property(q => q.Value)
                        .HasColumnName("QualificationType")
                        .HasConversion<string>()
                        .IsRequired();
                }
            );
        }
    }
}
