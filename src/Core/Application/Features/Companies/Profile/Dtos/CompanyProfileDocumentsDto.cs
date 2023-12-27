using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileDocumentsDto : BaseProfileResponse
{
    public List<CompanyProfileDocumentDto> Documents { get; set; } = default!;
}

public class CompanyProfileDocumentDto : BaseCompanyDocumentDto
{
    public List<CompanyProfileDocumentFileDto> Files { get; set; } = default!;
}

public class CompanyProfileDocumentFileDto
{
    public CurrentChange Current { get; set; } = default!;
    public CurrentChange? Change { get; set; } = default!;

    public class CurrentChange : BaseCompanyDocumentFileDto
    {
        public ReviewStatus Status { get; set; }
        public string? ReviewMessage { get; set; }
    }
}
