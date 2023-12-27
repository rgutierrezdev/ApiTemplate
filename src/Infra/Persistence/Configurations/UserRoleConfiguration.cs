using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class UserRoleConfiguration : EntityTypeConfigurationDependency<UserRole>
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder
            .ToTable("UserRole")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserRoles)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.Role)
            .WithMany(e => e.UserRoles)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
