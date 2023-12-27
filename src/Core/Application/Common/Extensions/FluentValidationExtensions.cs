using ApiTemplate.Application.Common.Validators;

namespace ApiTemplate.Application.Common.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(
        this IRuleBuilder<T, TProperty> ruleBuilder,
        TProperty[] toCompare
    )
        => ruleBuilder.SetValidator(new InValidator<T, TProperty>(toCompare));
}
