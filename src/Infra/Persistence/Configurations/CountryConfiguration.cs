using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CountryConfiguration : EntityTypeConfigurationDependency<Country>
{
    public override void Configure(EntityTypeBuilder<Country> builder)
    {
        builder
            .ToTable("Country")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(60);

        builder.Property(e => e.IsoCode)
            .HasMaxLength(3);
    }
}
