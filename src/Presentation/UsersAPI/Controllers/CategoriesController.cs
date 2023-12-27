using ApiTemplate.Application.Features.Categories;
using ApiTemplate.Application.Features.Categories.Dtos;

namespace ApiTemplate.UsersAPI.Controllers;

public class CategoriesController : BaseApiController
{
    [HttpGet("")]
    public Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Mediator.Send(new GetCategories.Request(), cancellationToken);
    }
}
