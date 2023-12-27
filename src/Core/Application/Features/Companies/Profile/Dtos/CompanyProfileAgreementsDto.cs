namespace ApiTemplate.Application.Features.Companies.Profile.Dtos;

public class CompanyProfileAgreementsDto : BaseProfileResponse
{
    public CurrenChange Current { get; set; } = default!;

    public class CurrenChange
    {
        public Registration? Registration { get; set; }
    }

    public class Registration
    {
        public string Name { get; set; } = default!;
        public Guid FileId { get; set; }
    }
}
