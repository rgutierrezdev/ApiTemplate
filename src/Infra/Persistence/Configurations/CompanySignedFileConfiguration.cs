using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanySignedFileConfiguration : EntityTypeConfigurationDependency<CompanySignedFile>
{
    public override void Configure(EntityTypeBuilder<CompanySignedFile> builder)
    {
        builder
            .ToTable("CompanySignedFile")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanySignedFiles)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.SignedFile)
            .WithOne(e => e.CompanySignedFile)
            .HasForeignKey<CompanySignedFile>()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
