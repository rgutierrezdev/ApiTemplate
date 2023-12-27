namespace ApiTemplate.Application.Features.Auth.Dtos;

public class AuthUserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsAdmin { get; set; }
    public IEnumerable<string> Permissions { get; set; } = default!;
    public IEnumerable<AuthCompanyDto> Companies { get; set; } = default!;
}
