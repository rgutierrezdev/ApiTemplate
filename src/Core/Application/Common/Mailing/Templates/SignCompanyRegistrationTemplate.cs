namespace ApiTemplate.Application.Common.Mailing.Templates;

public class SignCompanyRegistrationTemplate : IEmailTemplate<SignCompanyRegistrationTemplate.Variables>
{
    public string Name { get; }
    public string Subject { get; }
    public Variables Vars { get; set; }
    public IEnumerable<string> Tags { get; }

    public SignCompanyRegistrationTemplate(Variables variables)
    {
        Name = "sign-company-registration";
        Subject = "Firma de documentos de Registro - ApiTemplate";
        Tags = new[] { "sign-onboarding" };
        Vars = variables;
    }

    public class Variables
    {
        public string RepFirstName { get; set; } = default!;
        public string RepLastName { get; set; } = default!;
        public string CompanyName { get; set; } = default!;
        public AppLink SignDocumentLink { get; set; } = default!;
    }
}
