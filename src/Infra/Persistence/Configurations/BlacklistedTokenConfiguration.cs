using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class BlacklistedTokenConfiguration : EntityTypeConfigurationDependency<BlacklistedToken>
{
    public override void Configure(EntityTypeBuilder<BlacklistedToken> builder)
    {
        builder
            .ToTable("BlacklistedToken")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.User)
            .WithMany(e => e.BlacklistedTokens)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
