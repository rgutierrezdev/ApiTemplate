using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyCategoryConfiguration : EntityTypeConfigurationDependency<CompanyCategory>
{
    public override void Configure(EntityTypeBuilder<CompanyCategory> builder)
    {
        builder
            .ToTable("CompanyCategory")
            .HasKey(e => new { e.CompanyId, e.CategoryId });

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyCategories)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Category)
            .WithMany(e => e.CompanyCategories)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
