using ApiTemplate.Application.Features.Cities;
using ApiTemplate.Application.Features.Cities.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class CitiesController : BaseApiController
{
    [HttpGet("search")]
    public Task<List<CityDto>> SearchAsync(
        [FromQuery] SearchCities.Request request,
        CancellationToken cancellationToken
    )
    {
        return Mediator.Send(request, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [OperationErrors(ErrorCodes.CityNotFound)]
    public Task<CityDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetCity.Request(id), cancellationToken);
    }
}
