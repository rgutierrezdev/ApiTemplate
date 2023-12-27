using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class UserConfiguration : EntityTypeConfigurationDependency<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .ToTable("User")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.FirstName)
            .HasMaxLength(70);

        builder.Property(e => e.LastName)
            .HasMaxLength(70);

        builder.Property(e => e.Email)
            .HasMaxLength(320);

        builder.Property(e => e.Password)
            .HasMaxLength(100);

        builder.Property(e => e.RecoveryCode)
            .HasMaxLength(300);

        builder
            .HasIndex(e => e.Email)
            .IsUnique();
    }
}
