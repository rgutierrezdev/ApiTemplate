using ApiTemplate.Application.Features.Files;
using ApiTemplate.Application.Features.Files.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class FilesController : BaseApiController
{
    [HttpGet("{id:guid}")]
    [OperationErrors(ErrorCodes.FileNotFound)]
    public Task<FileDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetFile.Request(id), cancellationToken);
    }
}
