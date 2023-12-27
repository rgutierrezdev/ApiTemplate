using ApiTemplate.Application.Features.EconomicSectors;
using ApiTemplate.Application.Features.EconomicSectors.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class EconomicSectorsController : BaseApiController
{
    [HttpGet("")]
    public Task<List<EconomicSectorDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetEconomicSectors.Request(), cancellationToken);
    }
}
