using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class DocumentTypeConfiguration : EntityTypeConfigurationDependency<DocumentType>
{
    public override void Configure(EntityTypeBuilder<DocumentType> builder)
    {
        builder
            .ToTable("DocumentType")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.ShortName)
            .HasMaxLength(4);

        builder.Property(e => e.Name)
            .HasMaxLength(50);

        builder
            .HasIndex(e => e.ShortName)
            .IsUnique();
    }
}
