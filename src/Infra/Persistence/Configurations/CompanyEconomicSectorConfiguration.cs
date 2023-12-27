using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyEconomicSectorConfiguration : EntityTypeConfigurationDependency<CompanyEconomicSector>
{
    public override void Configure(EntityTypeBuilder<CompanyEconomicSector> builder)
    {
        builder
            .ToTable("CompanyEconomicSector")
            .HasKey(e => new { e.CompanyId, e.EconomicSectorId });

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyEconomicSectors)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.EconomicSector)
            .WithMany(e => e.CompanyEconomicSectors)
            .HasForeignKey(e => e.EconomicSectorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
