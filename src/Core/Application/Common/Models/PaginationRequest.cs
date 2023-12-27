using System.Linq.Expressions;

namespace ApiTemplate.Application.Common.Models;

public enum Like
{
    Starts,
    Ends,
    Contains,
    Scape
}

public enum OrderByDir
{
    Asc,
    Desc,
}

public record OrderBy<TOrderBy>(TOrderBy Field, OrderByDir Dir);

public class PaginationRequest<TFilters, TOrderBy> where TOrderBy : struct
{
    public string? Filter { get; set; }
    public int? Page { get; set; }
    public int? Size { get; set; }
    public OrderBy<TOrderBy>? OrderBy { get; set; }
    public TFilters? Filters { get; set; }

    public bool HasOrderBy()
    {
        return OrderBy != null;
    }

    public PaginationRequest<TFilters, TOrderBy> HandleFilter(Like like, Action<string> fn)
    {
        if (!string.IsNullOrWhiteSpace(Filter))
            fn(GetLikeFilterValue(Filter, like));

        return this;
    }

    public PaginationRequest<TFilters, TOrderBy> HandleFilter<TProperty>(
        Expression<Func<TFilters, TProperty>> propertyExpression,
        Action<TProperty> fn
    )
    {
        if (Filters == null)
            return this;

        var func = propertyExpression.Compile();
        object? value = func(Filters);

        if (value == null ||
            (value is string stringValue && string.IsNullOrWhiteSpace(stringValue)) ||
            (value is Array arrayValue && arrayValue.Length == 0))
            return this;

        fn((TProperty)value);

        return this;
    }

    public PaginationRequest<TFilters, TOrderBy> HandleFilter(
        Expression<Func<TFilters, string?>> propertyExpression,
        Like like,
        Action<string> fn
    )
    {
        if (Filters == null)
            return this;

        var func = propertyExpression.Compile();
        var value = func(Filters);

        if (string.IsNullOrWhiteSpace(value))
            return this;

        fn(GetLikeFilterValue(value, like));

        return this;
    }

    private string GetLikeFilterValue(string value, Like like)
    {
        value = like switch
        {
            Like.Starts => $"{value}%",
            Like.Ends => $"%{value}",
            Like.Contains => $"%{value}%",
            Like.Scape => $"%{value.Replace(' ', '%')}%",
            _ => value
        };

        return value;
    }
}
