# Pagination

There are 2 ways to handle pagination data for requests, using ([Ardalis](https://specification.ardalis.com/)) or
using a query builder for complex queries ([SqlKata](https://sqlkata.com/)).

A pagination request will be always composed of some parameters like:

- Page: page to retrieve
- Size: number of items per page
- Order by: Field/Direction to order the elements
- Filter: general filter
- Filters: Custom filters to apply, i.e. column filters

There are some common things you'll need to do regardless of the option you choose. Let's make an example of a
pagination request for a "Document Types" feature.

Define your feature including

1. Model class for available custom filters
2. Enum for available fields to order by
3. Request model inheriting `PaginationRequest<TFilers, TOrderBy>` class and passing both model and enum types

```csharp
public abstract class GetDocumentTypesPage
{
    // 1. available custom filters    
    public class Filters
    {
        public string? ShortName { get; set; }
        public string? Name { get; set; }
    }

    // 2. available fields to order by
    public enum OrderByFields
    {
        Id,
        Name,
        ShortName
    }

    // 3. request model
    public class Request : PaginationRequest<Filters, OrderByFields>, IRequest<PaginationResponse<DocumentTypeDto>>
    {
    }
```

## Specification

Use a repository along with `PaginationSpecification<TFrom, TTo, TFilters, TOrderBy>` by passing the request object, by
doing this the specification will handle the page, size and order parameters.

But you have to manually handle the filter, custom filters and specify what order to apply when there is no order in the
request.

```csharp
public abstract class GetDocumentTypesPage
{
    // code from steps 1 to 3 remains the same

    // 4. handle the request
    internal class Handler : IRequestHandler<Request, PaginationResponse<DocumentTypeDto>>
    {
        private readonly IRepository<DocumentType> _repository;

        public Handler(IRepository<DocumentType> repository)
        {
            _repository = repository;
        }

        public async Task<PaginationResponse<DocumentTypeDto>> Handle(
            Request request,
            CancellationToken cancellationToken
        )
        {
            var spec = new PaginationSpecification<DocumentType, DocumentTypeDto, Filters, OrderByFields>(
                request,
                query =>
                {
                    query.OrderByDescending(dt => dt.Id, !request.HasOrderBy());
                    
                    request
                        .HandleFilter(Like.Contains, value =>
                        {
                            query
                                .Search(dt => dt.ShortName, value)
                                .Search(dt => dt.Name, value);
                        })
                        .HandleFilter(f => f.ShortName, Like.Contains,
                            value => query.Search(dt => dt.ShortName, value)
                        )
                        .HandleFilter(f => f.Name, Like.Contains,
                            value => query.Search(dt => dt.Name, value)
                        );
                }
            );
            
            var list = await _repository.ListAsync(spec, cancellationToken);
            var total = await _repository.CountAsync(spec, cancellationToken);

            return new PaginationResponse<DocumentTypeDto>(list.ToList(), total);
        }
    }
}
```

## Query Builder

Use the `IQueryRepository` to build the query and apply the pager by passing the request object and the it will handle
the page, size and order parameters.

But you have to manually handle the filter, custom filters and specify what order to apply when there is no order in the
request.

```csharp
public abstract class GetDocumentTypesPage
{
    // code from steps 1 to 3 remains the same

    // 4. handle the request
    internal class Handler : IRequestHandler<Request, PaginationResponse<DocumentTypeDto>>
    {
        private readonly IQueryRepository _queryRepository;

        public Handler(IQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<PaginationResponse<DocumentTypeDto>> Handle(
            Request request,
            CancellationToken cancellationToken
        )
        {
            var query = new Query("DocumentType as dt");

            request
                .HandleFilter(Like.Contains, value =>
                {
                    query.Where(q => q
                        .WhereLike("dt.ShortName", value)
                        .OrWhereLike("dt.name", value)
                    );
                })
                .HandleFilter(f => f.ShortName, Like.Contains,
                    value => query.WhereLike("dt.ShortName", value)
                )
                .HandleFilter(f => f.Name, Like.Contains,
                    value => query.WhereLike("dt.Name", value)
                );

            var total = await _queryRepository.ScalarAsync<int>(query.Clone().AsCount(), cancellationToken);

            query.Select("dt.{Id, ShortName, Name}")
                .ApplyPager(request, "dt.Id");

            var list = await _queryRepository.GetAsync<DocumentTypeDto>(
                query,
                cancellationToken
            );

            return new PaginationResponse<DocumentTypeDto>(list.ToList(), total);
        }
    }
}
```

---

The main objective of hard type available Filters and available Order fields is to ensure that pagination request are
mapped correctly when executed and also make the frontend development more fluent by simply looking at the swagger
document:

| Swagger UI                  | Redoc                       |
|-----------------------------|-----------------------------|
| ![](assets/pagination1.png) | ![](assets/pagination2.png) |
