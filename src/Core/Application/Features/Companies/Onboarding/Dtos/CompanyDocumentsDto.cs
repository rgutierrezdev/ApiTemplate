using ApiTemplate.Application.Features.Companies.Dtos;

namespace ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

public class CompanyDocumentDto : BaseCompanyDocumentDto
{
    public IEnumerable<BaseCompanyDocumentFileDto> Files { get; set; } = default!;
}
