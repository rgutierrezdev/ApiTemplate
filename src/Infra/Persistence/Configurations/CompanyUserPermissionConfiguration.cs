using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyUserPermissionConfiguration : EntityTypeConfigurationDependency<CompanyUserPermission>
{
    public override void Configure(EntityTypeBuilder<CompanyUserPermission> builder)
    {
        builder
            .ToTable("CompanyUserPermission")
            .HasKey(e => new { e.CompanyUserId, e.PermissionId });

        builder.HasOne(e => e.CompanyUser)
            .WithMany(e => e.CompanyUserPermissions)
            .HasForeignKey(e => e.CompanyUserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.Permission)
            .WithMany(e => e.CompanyUserPermissions)
            .HasForeignKey(e => e.PermissionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
