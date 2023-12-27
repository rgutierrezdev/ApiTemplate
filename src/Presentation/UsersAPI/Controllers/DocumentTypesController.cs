using ApiTemplate.Application.Features.DocumentTypes;
using ApiTemplate.Application.Features.DocumentTypes.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class DocumentTypesController : BaseApiController
{
    [HttpGet("")]
    public Task<List<DocumentTypeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetDocumentTypes.Request(), cancellationToken);
    }
}
