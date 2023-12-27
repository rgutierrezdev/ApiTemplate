using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class ContextualDocumentConfiguration : EntityTypeConfigurationDependency<ContextualDocument>
{
    public override void Configure(EntityTypeBuilder<ContextualDocument> builder)
    {
        builder
            .ToTable("ContextualDocument")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasIndex(e => new { e.Type, e.Version })
            .IsUnique();
    }
}
