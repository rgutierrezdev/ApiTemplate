using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyAssociateConfiguration : EntityTypeConfigurationDependency<CompanyAssociate>
{
    public override void Configure(EntityTypeBuilder<CompanyAssociate> builder)
    {
        builder
            .ToTable("CompanyAssociate")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(140);

        builder.Property(e => e.Document)
            .HasMaxLength(30);

        builder.HasOne(e => e.Company)
            .WithMany(e => e.CompanyAssociates)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasOne(e => e.DocumentType)
            .WithMany(e => e.CompanyAssociates)
            .HasForeignKey(e => e.DocumentTypeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
