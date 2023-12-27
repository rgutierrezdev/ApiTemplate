using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyDocumentFileConfiguration : EntityTypeConfigurationDependency<CompanyDocumentFile>
{
    public override void Configure(EntityTypeBuilder<CompanyDocumentFile> builder)
    {
        builder
            .ToTable("CompanyDocumentFile")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.ReviewMessage)
            .HasMaxLength(500);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyDocumentsFiles)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.CompanyDocument)
            .WithMany(e => e.CompanyDocumentsFiles)
            .HasForeignKey(e => e.CompanyDocumentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.File)
            .WithOne(e => e.CompanyDocumentFile)
            .HasForeignKey<CompanyDocumentFile>(e => e.Id)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.ChangeCompanyDocumentFile)
            .WithOne(e => e.ChangedByCompanyDocumentFile)
            .HasForeignKey<CompanyDocumentFile>(e => e.ChangeCompanyDocumentFileId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
