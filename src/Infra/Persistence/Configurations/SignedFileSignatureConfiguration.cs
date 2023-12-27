using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class SignedFileSignatureConfiguration : EntityTypeConfigurationDependency<SignedFileSignature>
{
    public override void Configure(EntityTypeBuilder<SignedFileSignature> builder)
    {
        builder
            .ToTable("SignedFileSignature")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.FullName)
            .HasMaxLength(140);

        builder.Property(e => e.Email)
            .HasMaxLength(320);

        builder.Property(e => e.Client)
            .HasMaxLength(300);

        builder.Property(e => e.Token)
            .HasMaxLength(10);

        builder.HasOne(e => e.SignedFile)
            .WithMany(e => e.SignedFileSignatures)
            .HasForeignKey(e => e.SignedFileId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.DocumentType)
            .WithMany(e => e.SignedFileSignatures)
            .HasForeignKey(e => e.DocumentTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
