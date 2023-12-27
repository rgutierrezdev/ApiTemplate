using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CostCenterUserConfiguration : EntityTypeConfigurationDependency<CostCenterUser>
{
    public override void Configure(EntityTypeBuilder<CostCenterUser> builder)
    {
        builder
            .ToTable("CostCenterUser")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.CostCenter)
            .WithMany(e => e.CostCenterUsers)
            .HasForeignKey(e => e.CostCenterId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.CompanyUser)
            .WithMany(e => e.CostCenterUsers)
            .HasForeignKey(e => e.CompanyUserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
