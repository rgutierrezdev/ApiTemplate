using ApiTemplate.Application.Features.Categories.Dtos;

namespace ApiTemplate.Application.Features.Categories;

public class GetCategories
{
    public class Request : IRequest<List<CategoryDto>>
    {
    }

    internal class Handler : IRequestHandler<Request, List<CategoryDto>>
    {
        private readonly IRepository<Category> _repository;

        public Handler(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var categories = await _repository.ListAsync<CategoryDto>(query => query
                    .OrderBy(dt => dt.Name),
                cancellationToken
            );

            // TODO cache this response
            return categories;
        }
    }
}
