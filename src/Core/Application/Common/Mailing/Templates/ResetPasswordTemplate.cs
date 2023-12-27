namespace ApiTemplate.Application.Common.Mailing.Templates;

public class ResetPasswordTemplate : IEmailTemplate<ResetPasswordTemplate.Variables>
{
    public string Name { get; }
    public string Subject { get; }
    public Variables Vars { get; }
    public IEnumerable<string> Tags { get; }

    public ResetPasswordTemplate(Variables variables)
    {
        Name = "reset-password";
        Subject = "Restablece tu contraseña - ApiTemplate";
        Tags = new[] { "reset-password" };
        Vars = variables;
    }

    public class Variables
    {
        public string Username { get; set; } = default!;
        public AppLink ResetPasswordLink { get; set; } = default!;
    }
}
