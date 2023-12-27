using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyAddressConfiguration : EntityTypeConfigurationDependency<CompanyAddress>
{
    public override void Configure(EntityTypeBuilder<CompanyAddress> builder)
    {
        builder
            .ToTable("CompanyAddress")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(50);

        builder.Property(e => e.Address)
            .HasMaxLength(100);

        builder.Property(e => e.AdditionalInfo)
            .HasMaxLength(300);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyAddresses)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.City)
            .WithMany(e => e.CompanyAddresses)
            .HasForeignKey(e => e.CityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
