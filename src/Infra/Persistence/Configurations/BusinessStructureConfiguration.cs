using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class BusinessStructureConfiguration : EntityTypeConfigurationDependency<BusinessStructure>
{
    public override void Configure(EntityTypeBuilder<BusinessStructure> builder)
    {
        builder
            .ToTable("BusinessStructure")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);

        builder.HasOne(e => e.Country)
            .WithMany(e => e.BusinessStructures)
            .HasForeignKey(e => e.CountryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
