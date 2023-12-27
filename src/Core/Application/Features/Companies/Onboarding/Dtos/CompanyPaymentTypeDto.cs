namespace ApiTemplate.Application.Features.Companies.Onboarding.Dtos;

public class CompanyPaymentTypeDto
{
    public bool CreditEnabled { get; set; }
    public bool? AuthorizesFinancialInformation { get; set; }
}
