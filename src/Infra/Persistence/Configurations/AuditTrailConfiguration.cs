using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Persistence.Configurations;

public class AuditTrailConfiguration : EntityTypeConfigurationDependency<AuditTrail>
{
    public override void Configure(EntityTypeBuilder<AuditTrail> builder)
    {
        builder
            .ToTable("AuditTrail")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.User)
            .WithMany(e => e.AuditTrails)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.Type)
            .HasMaxLength(10);

        builder.Property(e => e.TableName)
            .HasMaxLength(100);
    }
}
