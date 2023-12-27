namespace ApiTemplate.Application.Common.Models;

public class BaseFileRequest
{
    public string Base64 { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Mime { get; set; } = default!;
}

public class BaseFileRequestValidator : AbstractValidator<BaseFileRequest>
{
    public BaseFileRequestValidator()
    {
        RuleFor(x => x.Base64)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Mime)
            .NotEmpty()
            .MaximumLength(100);
    }
}
