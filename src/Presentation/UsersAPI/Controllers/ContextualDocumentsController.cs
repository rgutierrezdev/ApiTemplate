using ApiTemplate.Application.Features.ContextualDocuments;
using ApiTemplate.Application.Features.ContextualDocuments.Dtos;
using ApiTemplate.Domain.Entities;

namespace ApiTemplate.UsersAPI.Controllers;

public class ContextualDocumentsController : BaseApiController
{
    [HttpGet("{type}")]
    [AllowAnonymous]
    [OperationErrors(ErrorCodes.ContextualDocumentNotFound)]
    public Task<ContextualDocumentDto> GetAllAsync(
        ContextualDocumentType type,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(new GetContextualDocument.Request(type), cancellationToken);
    }
}
