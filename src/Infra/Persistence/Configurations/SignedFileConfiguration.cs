using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class SignedFileConfiguration : EntityTypeConfigurationDependency<SignedFile>
{
    public override void Configure(EntityTypeBuilder<SignedFile> builder)
    {
        builder
            .ToTable("SignedFile")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(100);

        builder.Property(e => e.Hash)
            .HasMaxLength(200);

        builder.HasOne(e => e.File)
            .WithOne(e => e.SignedFile)
            .HasForeignKey<SignedFile>(e => e.Id)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
