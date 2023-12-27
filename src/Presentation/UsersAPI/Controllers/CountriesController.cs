using ApiTemplate.Application.Features.Countries;
using ApiTemplate.Application.Features.Countries.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class CountriesController : BaseApiController
{
    [HttpGet("")]
    public Task<List<CountryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetCountries.Request(), cancellationToken);
    }
}
