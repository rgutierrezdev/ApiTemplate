namespace ApiTemplate.Application.Features.Auth.Dtos;

public class TokensExpirationDto
{
    public DateTime AccessTokenExpiryDate { get; set; }
    public DateTime RefreshTokenExpiryDate { get; set; }
}
