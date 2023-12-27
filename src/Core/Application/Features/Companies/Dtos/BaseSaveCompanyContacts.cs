namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseSaveCompanyContacts
{
    public class Request
    {
        public MainContactRequest MainContact { get; set; } = default!;
        public OptionalContactRequest TreasuryContact { get; set; } = default!;
        public OptionalContactRequest OtherContact { get; set; } = default!;
    }

    public record MainContactRequest(
        string Name,
        string Email,
        string PhoneNumber
    );

    public record OptionalContactRequest(
        string? Name,
        string? Email,
        string? PhoneNumber
    );

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.MainContact)
                .NotNull()
                .SetValidator(new RequiredContactRequestValidator());

            RuleFor(r => r.TreasuryContact)
                .NotNull()
                .SetValidator(new OptionalContactRequestValidator());

            RuleFor(r => r.OtherContact)
                .NotNull()
                .SetValidator(new OptionalContactRequestValidator());
        }

        public class RequiredContactRequestValidator : AbstractValidator<MainContactRequest>
        {
            public RequiredContactRequestValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(140);

                RuleFor(x => x.Email)
                    .NotEmpty()
                    .MaximumLength(320);

                RuleFor(x => x.PhoneNumber)
                    .NotEmpty()
                    .MaximumLength(30);
            }
        }

        public class OptionalContactRequestValidator : AbstractValidator<OptionalContactRequest>
        {
            public OptionalContactRequestValidator()
            {
                RuleFor(x => x.Name)
                    .MaximumLength(140);

                RuleFor(x => x.Email)
                    .MaximumLength(320);

                RuleFor(x => x.PhoneNumber)
                    .MaximumLength(30);
            }
        }
    }
}
