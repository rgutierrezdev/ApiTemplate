using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class PermissionConfiguration : EntityTypeConfigurationDependency<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder
            .ToTable("Permission")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);

        builder.Property(e => e.DisplayName)
            .HasMaxLength(150);

        builder
            .HasIndex(e => e.Name)
            .IsUnique();
    }
}
