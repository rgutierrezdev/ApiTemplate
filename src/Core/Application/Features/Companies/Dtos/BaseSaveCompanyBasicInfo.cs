namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseSaveCompanyBasicInfo
{
    public class Request
    {
        public CompanyLegalType LegalType { get; set; }
        public string LegalName { get; set; } = default!;
        public string CiiuCode { get; set; } = default!;
        public PersonType PersonType { get; set; }
        public Guid? BusinessStructureId { get; set; }
        public Guid DocumentTypeId { get; set; }
        public string Document { get; set; } = default!;
        public string VerificationDigit { get; set; } = default!;

        public Guid CityId { get; set; }
        public string Address { get; set; } = default!;
        public LegalRepresentativeRequest? LegalRepresentative { get; set; }
    }

    public record LegalRepresentativeRequest(
        string FirstName,
        string LastName,
        string Email,
        Guid DocumentTypeId,
        string Document
    );

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.LegalType)
                .NotEmpty();

            RuleFor(x => x.LegalName)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.CiiuCode)
                .NotEmpty()
                .MaximumLength(4);

            RuleFor(x => x.PersonType)
                .NotEmpty();

            RuleFor(x => x.DocumentTypeId)
                .NotEmpty();

            RuleFor(x => x.Document)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.VerificationDigit)
                .NotEmpty()
                .MaximumLength(1);

            RuleFor(x => x.CityId)
                .NotEmpty();

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(100);

            When(x => x.PersonType == PersonType.Legal, () =>
            {
                RuleFor(x => x.BusinessStructureId)
                    .NotEmpty();

                RuleFor(x => x.LegalRepresentative)
                    .NotNull()
                    .SetValidator(new LegalRepresentativeRequestValidator()!);
            });
        }
    }

    public class LegalRepresentativeRequestValidator : AbstractValidator<LegalRepresentativeRequest>
    {
        public LegalRepresentativeRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(70);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(70);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(320);

            RuleFor(x => x.DocumentTypeId)
                .NotEmpty();

            RuleFor(x => x.Document)
                .NotEmpty()
                .MaximumLength(30);
        }
    }
}
