using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class StateConfiguration : EntityTypeConfigurationDependency<State>
{
    public override void Configure(EntityTypeBuilder<State> builder)
    {
        builder
            .ToTable("State")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(60);

        builder.Property(e => e.DaneCode)
            .HasMaxLength(2);

        builder.HasOne(e => e.Country)
            .WithMany(e => e.States)
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
