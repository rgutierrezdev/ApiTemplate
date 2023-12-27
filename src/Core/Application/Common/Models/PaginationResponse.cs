namespace ApiTemplate.Application.Common.Models;

public class PaginationResponse<T>
{
    public List<T> Results { get; set; }
    public int Total { get; set; }

    public PaginationResponse(List<T> results, int total)
    {
        Results = results;
        Total = total;
    }
}
