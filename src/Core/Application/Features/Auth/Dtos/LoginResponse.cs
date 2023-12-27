namespace ApiTemplate.Application.Features.Auth.Dtos;

public class LoginResponse : CurrentResponse
{
    public TokensExpirationDto Tokens { get; set; } = default!;
}
