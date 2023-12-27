using System.Reflection;
using FluentValidation.Validators;

namespace ApiTemplate.Application.Common.Validators;

public class InValidator<T, TProperty> : PropertyValidator<T, TProperty>, IInValidator
{
    public override bool IsValid(ValidationContext<T> context, TProperty value)
    {
        var success = ValuesToCompare.Contains(value);

        if (success) return true;

        context.MessageFormatter.AppendArgument("ComparisonValue", ValuesToCompare);

        return false;
    }

    public override string Name => "InValidator";

    public InValidator(TProperty[] valuesToCompare)
    {
        ValuesToCompare = valuesToCompare;
    }

    public Comparison Comparison => Comparison.Equal;
    public MemberInfo? MemberToCompare { get; private set; }
    private TProperty[] ValuesToCompare { get; set; }

    object IComparisonValidator.ValueToCompare => ValuesToCompare;

    protected override string GetDefaultMessageTemplate(string errorCode)
    {
        return "Invalid given value";
    }
}

public interface IInValidator : IComparisonValidator
{
}
