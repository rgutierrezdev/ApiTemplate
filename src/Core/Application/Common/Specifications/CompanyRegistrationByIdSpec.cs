namespace ApiTemplate.Application.Common.Specifications;

public sealed class CompanyRegistrationByIdSpec : SingleResultSpecification<Company, CompanyRegistrationPdfModel>
{
    public CompanyRegistrationByIdSpec(Guid companyId)
    {
        Query
            .Select(c => new CompanyRegistrationPdfModel()
            {
                Type = c.Type,
                LegalName = c.CompanyChange!.LegalName,
                PersonType = c.CompanyChange.PersonType,
                DocumentTypeName = c.CompanyChange.DocumentType!.Name,
                DocumentTypeShortName = c.CompanyChange.DocumentType!.ShortName,
                Document = c.CompanyChange.Document,
                VerificationDigit = c.CompanyChange.VerificationDigit,
                LegalRepresentativeFirstName = c.CompanyChange.LegalRepresentativeFirstName,
                LegalRepresentativeLastName = c.CompanyChange.LegalRepresentativeLastName,
                LegalRepresentativeEmail = c.CompanyChange.LegalRepresentativeEmail,
                LegalRepresentativeDocumentTypeId = c.CompanyChange.LegalRepresentativeDocumentTypeId,
                LegalRepresentativeDocumentTypeShortName = c.CompanyChange.LegalRepresentativeDocumentType!.ShortName,
                LegalRepresentativeDocumentTypeName = c.CompanyChange.LegalRepresentativeDocumentType!.Name,
                LegalRepresentativeDocument = c.CompanyChange.LegalRepresentativeDocument,
                SignOnboardingToken = c.SignOnboardingToken,
                Associates = c.CompanyAssociates
                    .Where(ca => ca.IsChange)
                    .Select(ca => new OnboardingSummaryAssociate(
                        ca.Name,
                        ca.DocumentType.ShortName,
                        ca.Document,
                        ca.ParticipationPercent,
                        ca.Pep
                    ))
                    .ToList(),
                Contacts = c.CompanyContacts.Select(cc => new OnboardingSummaryContact(
                    cc.Type,
                    cc.Name,
                    cc.Email,
                    cc.PhoneNumber
                )).ToList(),
                EInvoiceFullName = c.EInvoiceFullName,
                EInvoicePhoneNumber = c.EInvoicePhoneNumber,
                EInvoiceEmail = c.EInvoiceEmail,
                EInvoiceAccountingCloseDay = c.EInvoiceAccountingCloseDay,
                HasPepRelative = c.CompanyChange.HasPepRelative,
                UnderOath = c.CompanyChange.UnderOath,
                AuthorizesFinancialInformation = c.CompanyChange.AuthorizesFinancialInformation,
                AgreesTermsAndConditions = c.AgreesTermsAndConditions,
                RetentionSubject = c.CompanyChange.RetentionSubject,
                RequiredToDeclareIncome = c.CompanyChange.RequiredToDeclareIncome,
                VatResponsible = c.CompanyChange.VatResponsible,
                DianGreatContributor = c.CompanyChange.DianGreatContributor,
                SalesRetentionAgent = c.CompanyChange.SalesRetentionAgent,
                IncomeSelfRetainer = c.CompanyChange.IncomeSelfRetainer,
                IcaAutoRetainer = c.CompanyChange.IcaAutoRetainer,
                Regime = c.CompanyChange.Regime,
                IcaActivity = c.CompanyChange.IcaActivity,
                IsRegistrationSigned = c.CompanySignedFiles.Any(csf =>
                    csf.Type == CompanySignedFileType.Registration
                )
            })
            .Where(c => c.Id == companyId);
    }
}
