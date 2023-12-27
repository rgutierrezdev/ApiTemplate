using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyUserConfiguration : EntityTypeConfigurationDependency<CompanyUser>
{
    public override void Configure(EntityTypeBuilder<CompanyUser> builder)
    {
        builder
            .ToTable("CompanyUser")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder
            .HasIndex(e => new { e.CompanyId, e.UserId })
            .IsUnique();

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyUsers)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.User)
            .WithMany(e => e.CompanyUsers)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
