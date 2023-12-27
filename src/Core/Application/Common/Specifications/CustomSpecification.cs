namespace ApiTemplate.Application.Common.Specifications;

public class CustomSpecification<TFrom> : Specification<TFrom>
{
    public CustomSpecification(Action<ISpecificationBuilder<TFrom>>? predicate = null)
    {
        predicate?.Invoke(Query);
    }
}

public class CustomSpecification<TFrom, TTo> : Specification<TFrom, TTo>
{
    public CustomSpecification(Action<ISpecificationBuilder<TFrom, TTo>>? predicate = null)
    {
        predicate?.Invoke(Query);
    }
}
