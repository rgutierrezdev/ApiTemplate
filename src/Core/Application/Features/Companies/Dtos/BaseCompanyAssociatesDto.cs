namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseCompanyAssociatesDto
{
    public bool? HasPepRelative { get; set; }
    public bool? UnderOath { get; set; }
    public IEnumerable<CompanyAssociateDto> Associates { get; set; } = default!;
}

public record CompanyAssociateDto(
    Guid Id,
    string Name,
    Guid DocumentTypeId,
    string DocumentTypeShortName,
    string DocumentTypeName,
    string Document,
    int ParticipationPercent,
    bool Pep
);
