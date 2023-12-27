using SqlKata;

namespace ApiTemplate.Application.Common.Extensions;

public static class QueryExtensions
{
    public const int DefaultPage = 1;
    public const int DefaultSize = 15;

    public static Query ApplyPager<T, TOrderBy>(
        this Query query,
        PaginationRequest<T, TOrderBy> request,
        string defaultOrderBy = "Id",
        OrderByDir defaultOrder = OrderByDir.Desc
    ) where TOrderBy : struct
    {
        request.Page ??= DefaultPage;
        request.Size ??= DefaultSize;

        if (request.Page <= 0)
            request.Page = DefaultPage;

        if (request.Size <= 0)
            request.Size = DefaultSize;

        if (request.Page > 1)
            query.Skip(((int)request.Page - 1) * (int)request.Size!);

        query.Take((int)request.Size!);

        if (request.HasOrderBy())
        {
            var field = request.OrderBy!.Field.ToString();

            if (request.OrderBy!.Dir == OrderByDir.Asc)
                query.OrderBy(field);
            else
                query.OrderByDesc(field);
        }
        else
        {
            if (defaultOrder == OrderByDir.Asc)
                query.OrderBy(defaultOrderBy);
            else
                query.OrderByDesc(defaultOrderBy);
        }

        return query;
    }
}
