using System.Linq.Expressions;

namespace ApiTemplate.Application.Common.Specifications;

public class PaginationSpecification<TFrom, TTo, TFilters, TOrderBy> : Specification<TFrom, TTo> where TOrderBy : struct
{
    private readonly PaginationRequest<TFilters, TOrderBy> _request;
    private readonly Dictionary<TOrderBy, Expression<Func<TFrom, object?>>>? _fixOrders;

    public PaginationSpecification(
        PaginationRequest<TFilters, TOrderBy> request,
        Action<ISpecificationBuilder<TFrom, TTo>>? predicate = null,
        Dictionary<TOrderBy, Expression<Func<TFrom, object?>>>? fixOrders = null
    )
    {
        request.Page ??= QueryExtensions.DefaultPage;
        request.Size ??= QueryExtensions.DefaultSize;

        _request = request;
        _fixOrders = fixOrders;

        if (request.Page <= 0)
            request.Page = QueryExtensions.DefaultPage;

        if (request.Size <= 0)
            request.Size = QueryExtensions.DefaultSize;

        ApplyPagination(request);

        predicate?.Invoke(Query);

        ApplyOrderBy();
    }

    private void ApplyPagination(PaginationRequest<TFilters, TOrderBy> request)
    {
        if (request.Page > 1)
            Query.Skip(((int)request.Page - 1) * (int)request.Size!);

        Query.Take((int)request.Size!);
    }

    private void ApplyOrderBy()
    {
        if (!_request.HasOrderBy()) return;

        var orderField = _request.OrderBy!.Field;
        var orderDirType = _request.OrderBy!.Dir == OrderByDir.Desc
            ? OrderTypeEnum.OrderByDescending
            : OrderTypeEnum.OrderBy;

        var keySelector = _fixOrders?.GetValueOrDefault(orderField);

        if (keySelector == null)
        {
            var paramExpr = Expression.Parameter(typeof(TFrom));

            Expression propertyExpr = paramExpr;

            propertyExpr = Expression.PropertyOrField(propertyExpr, orderField.ToString()!);

            keySelector = Expression.Lambda<Func<TFrom, object?>>(
                Expression.Convert(propertyExpr, typeof(object)),
                paramExpr);
        }

        ((List<OrderExpressionInfo<TFrom>>)Query.Specification.OrderExpressions)
            .Add(new OrderExpressionInfo<TFrom>(keySelector, orderDirType));
    }
}
