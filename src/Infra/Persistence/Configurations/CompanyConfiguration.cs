using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.Persistence.Configurations;

public class CompanyConfiguration : EntityTypeConfigurationDependency<Company>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .ToTable("Company")
            .HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(200);

        builder.Property(e => e.ContactPhoneNumber)
            .HasMaxLength(30);

        builder.Property(e => e.LegalName)
            .HasMaxLength(300);

        builder.Property(e => e.CiiuCode)
            .HasMaxLength(4);

        builder.Property(e => e.Document)
            .HasMaxLength(30);

        builder.Property(e => e.VerificationDigit)
            .HasMaxLength(1);

        builder.Property(e => e.Address)
            .HasMaxLength(100);

        builder.Property(e => e.LegalRepresentativeFirstName)
            .HasMaxLength(70);

        builder.Property(e => e.LegalRepresentativeLastName)
            .HasMaxLength(70);

        builder.Property(e => e.LegalRepresentativeEmail)
            .HasMaxLength(320);

        builder.Property(e => e.LegalRepresentativeDocument)
            .HasMaxLength(30);

        builder.Property(e => e.SignOnboardingToken)
            .HasMaxLength(20);

        builder.Property(e => e.EInvoiceFullName)
            .HasMaxLength(140);

        builder.Property(e => e.EInvoicePhoneNumber)
            .HasMaxLength(30);

        builder.Property(e => e.EInvoiceEmail)
            .HasMaxLength(320);

        builder.Property(e => e.DianGreatContributorRes)
            .HasMaxLength(30);

        builder.Property(e => e.SalesRetentionAgentRes)
            .HasMaxLength(30);

        builder.Property(e => e.IncomeSelfRetainerRes)
            .HasMaxLength(30);

        builder.Property(e => e.IcaActivity)
            .HasMaxLength(4);

        builder.Property(e => e.IcaAutoRetainerRes)
            .HasMaxLength(30);

        builder.Property(e => e.AboutUs)
            .HasMaxLength(500);

        builder.Property(e => e.ContactEmail)
            .HasMaxLength(320);

        builder.Property(e => e.WebsiteUrl)
            .HasMaxLength(100);

        builder.Property(e => e.CompanyInfoReviewMessage)
            .HasMaxLength(500);

        builder.Property(e => e.BillingTaxesReviewMessage)
            .HasMaxLength(500);

        builder.Property(e => e.CreditReviewMessage)
            .HasMaxLength(500);

        builder.Property(e => e.AssociatesReviewMessage)
            .HasMaxLength(500);

        builder.Property(e => e.CreditLimit)
            .HasPrecision(18, 2);

        builder.HasOne(e => e.BusinessStructure)
            .WithMany(e => e.Companies)
            .HasForeignKey(e => e.BusinessStructureId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.DocumentType)
            .WithMany(e => e.Companies)
            .HasForeignKey(e => e.DocumentTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.City)
            .WithMany(e => e.Companies)
            .HasForeignKey(e => e.CityId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.LegalRepresentativeDocumentType)
            .WithMany(e => e.LegalRepresentativeCompanies)
            .HasForeignKey(e => e.LegalRepresentativeDocumentTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.LogoFile)
            .WithMany(e => e.LogoCompanies)
            .HasForeignKey(e => e.LogoFileId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
