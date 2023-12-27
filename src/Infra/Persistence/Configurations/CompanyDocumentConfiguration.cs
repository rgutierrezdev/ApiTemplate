using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyDocumentConfiguration : EntityTypeConfigurationDependency<CompanyDocument>
{
    public override void Configure(EntityTypeBuilder<CompanyDocument> builder)
    {
        builder
            .ToTable("CompanyDocument")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(300);

        builder.Property(e => e.Description)
            .HasMaxLength(500);
    }
}
