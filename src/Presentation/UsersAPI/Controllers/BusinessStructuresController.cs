using ApiTemplate.Application.Features.BusinessStructures;
using ApiTemplate.Application.Features.BusinessStructures.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class BusinessStructuresController : BaseApiController
{
    [HttpGet("")]
    public Task<List<BusinessStructureDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetBusinessStructures.Request(), cancellationToken);
    }
}
