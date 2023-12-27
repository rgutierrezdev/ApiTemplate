namespace ApiTemplate.Application.Features.Auth.Dtos;

public class AuthCostCenterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public bool Default { get; set; }
    public bool Enabled { get; set; }
}
