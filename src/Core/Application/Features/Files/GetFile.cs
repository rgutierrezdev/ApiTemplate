using ApiTemplate.Application.Features.Files.Dtos;
using File = ApiTemplate.Domain.Entities.File;

namespace ApiTemplate.Application.Features.Files;

public class GetFile
{
    public class Request : IRequest<FileDto>
    {
        public Guid Id { get; set; }

        public Request(Guid id) => Id = id;
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(p => p.Id)
                .NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, FileDto>
    {
        private readonly IValidator<Request> _validator;
        private readonly IRepository<File> _fileRepository;
        private readonly IStorageService _storageService;

        public Handler(IValidator<Request> validator, IRepository<File> fileRepository, IStorageService storageService)
        {
            _validator = validator;
            _fileRepository = fileRepository;
            _storageService = storageService;
        }

        public async Task<FileDto> Handle(Request request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var file = await _fileRepository.GetByIdAsync(request.Id, cancellationToken)
                       ?? throw new NotFoundException(
                           ErrorCodes.FileNotFound,
                           $"File with id '{request.Id}' was not found"
                       );

            var base64 = await _storageService.GetFileAsync(file.Public, file.Src, cancellationToken);

            return new FileDto()
            {
                Name = file.Name,
                Mime = file.Mime,
                Base64 = base64,
                Size = file.Size,
            };
        }
    }
}
