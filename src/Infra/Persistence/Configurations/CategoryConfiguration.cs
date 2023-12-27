using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CategoryConfiguration : EntityTypeConfigurationDependency<Category>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .ToTable("Category")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);
    }
}
