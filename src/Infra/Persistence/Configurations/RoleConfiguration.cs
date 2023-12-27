using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class RoleConfiguration : EntityTypeConfigurationDependency<Role>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .ToTable("Role")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);
    }
}
