using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CostCenterConfiguration : EntityTypeConfigurationDependency<CostCenter>
{
    public override void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder
            .ToTable("CostCenter")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CostCenters)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
