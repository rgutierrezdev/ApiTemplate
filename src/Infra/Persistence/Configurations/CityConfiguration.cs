using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CityConfiguration : EntityTypeConfigurationDependency<City>
{
    public override void Configure(EntityTypeBuilder<City> builder)
    {
        builder
            .ToTable("City")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(60);

        builder.Property(e => e.DaneCode)
            .HasMaxLength(3);

        builder.HasOne(e => e.State)
            .WithMany(e => e.Cities)
            .HasForeignKey(e => e.StateId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
