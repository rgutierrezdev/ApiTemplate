using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class EconomicSectorConfiguration : EntityTypeConfigurationDependency<EconomicSector>
{
    public override void Configure(EntityTypeBuilder<EconomicSector> builder)
    {
        builder
            .ToTable("EconomicSector")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);
    }
}
