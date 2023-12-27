using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class LoginAttemptConfiguration : EntityTypeConfigurationDependency<LoginAttempt>
{
    public override void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder
            .ToTable("LoginAttempt")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.User)
            .WithMany(e => e.LoginAttempts)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.Email)
            .HasMaxLength(320);
    }
}
