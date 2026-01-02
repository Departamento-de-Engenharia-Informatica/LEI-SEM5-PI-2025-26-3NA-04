using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using APDL.API.Domain.PrivacyPolicyAggregate;

namespace APDL.API.Infrastructure.PrivacyPolicyInfrastructure
{
    public class PrivacyPolicyEntityTypeConfiguration : IEntityTypeConfiguration<PrivacyPolicy>
    {
        public void Configure(EntityTypeBuilder<PrivacyPolicy> builder)
        {
            builder.ToTable("PrivacyPolicies");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Version)
                .IsRequired();

            builder.HasIndex(p => p.Version)
                .IsUnique();

            builder.Property(p => p.Content)
                .IsRequired();

            builder.Property(p => p.EffectiveDate)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(p => p.IsActive)
                .HasFilter("[IsActive] = 1");

            builder.Property(p => p.CreatedBy)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();
        }
    }
}

