namespace ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

public class CompanySummaryDto
{
    public string LegalRepresentativeFirstName { get; set; } = default!;
    public string LegalRepresentativeLastName { get; set; } = default!;
    public string LegalRepresentativeEmail { get; set; } = default!;
    public string LegalRepresentativeDocumentTypeShortName { get; set; } = default!;
    public string LegalRepresentativeDocumentTypeName { get; set; } = default!;
    public string LegalRepresentativeDocument { get; set; } = default!;
}
