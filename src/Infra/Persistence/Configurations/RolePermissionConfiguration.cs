using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class RolePermissionConfiguration : EntityTypeConfigurationDependency<RolePermission>
{
    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder
            .ToTable("RolePermission")
            .HasKey(e => new { e.RoleId, e.PermissionId });

        builder.HasOne(e => e.Role)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.Permission)
            .WithMany(e => e.RolePermissions)
            .HasForeignKey(e => e.PermissionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
