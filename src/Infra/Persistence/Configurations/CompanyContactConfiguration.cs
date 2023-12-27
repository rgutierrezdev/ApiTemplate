using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyContactConfiguration : EntityTypeConfigurationDependency<CompanyContact>
{
    public override void Configure(EntityTypeBuilder<CompanyContact> builder)
    {
        builder
            .ToTable("CompanyContact")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(140);

        builder.Property(e => e.Email)
            .HasMaxLength(320);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(30);

        builder.HasIndex(e => new { e.CompanyId, e.Type })
            .IsUnique();

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyContacts)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
