namespace ApiTemplate.Application.Common.Mailing.Templates;

public class AdminUserInvitationTemplate : IEmailTemplate<AdminUserInvitationTemplate.Variables>
{
    public string Name { get; }
    public string Subject { get; }
    public Variables Vars { get; }
    public IEnumerable<string> Tags { get; }

    public AdminUserInvitationTemplate(Variables variables)
    {
        Name = "admin-user-invitation";
        Subject = "Termina la configuración de tu cuenta - ApiTemplate";
        Tags = new[] { "admin-user-invitation" };
        Vars = variables;
    }

    public class Variables
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public AppLink NewPasswordLink { get; set; } = default!;
    }
}
