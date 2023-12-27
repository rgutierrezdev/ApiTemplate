namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseSaveElectronicInvoice
{
    public class Request
    {
        public string FullName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int AccountingCloseDay { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.FullName)
                .NotEmpty()
                .MaximumLength(140);

            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(r => r.Email)
                .NotEmpty()
                .MaximumLength(320);

            RuleFor(r => r.AccountingCloseDay)
                .ExclusiveBetween(1, 31);
        }
    }
}
