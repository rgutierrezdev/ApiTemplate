namespace ApiTemplate.Application.Features.Companies.Dtos;

public class BaseSaveCompanyAssociates
{
    public class Request
    {
        public bool HasPepRelative { get; set; }
        public bool UnderOath { get; set; }
        public AssociateRequest[] Associates { get; set; } = default!;
        public Guid[] RemovedAssociateIds { get; set; } = default!;
    }

    public record AssociateRequest(
        Guid? Id,
        string Name,
        Guid DocumentTypeId,
        string Document,
        int ParticipationPercent,
        bool Pep
    );

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Associates)
                .NotNull()
                .ForEach(collection => collection.SetValidator(new AssociateRequestValidator()));

            RuleFor(r => r.RemovedAssociateIds)
                .NotNull();
        }

        public class AssociateRequestValidator : AbstractValidator<AssociateRequest>
        {
            public AssociateRequestValidator()
            {
                RuleFor(r => r.Name)
                    .NotEmpty()
                    .MaximumLength(140);

                RuleFor(r => r.DocumentTypeId)
                    .NotEmpty();

                RuleFor(r => r.Document)
                    .NotEmpty()
                    .MaximumLength(30);

                RuleFor(r => r.ParticipationPercent)
                    .NotEmpty()
                    .InclusiveBetween(1, 100);
            }
        }
    }
}
