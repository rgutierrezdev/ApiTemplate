namespace ApiTemplate.Application.Common.Models;

public class Difference : Difference<Guid>
{
}

public class Difference<T>
{
    public T[] Added { get; set; } = default!;
    public T[] Removed { get; set; } = default!;
}
