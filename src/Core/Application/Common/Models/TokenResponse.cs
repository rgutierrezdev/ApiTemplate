namespace ApiTemplate.Application.Common.Models;

public class TokenResponse
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime AccessTokenExpiryDate { get; set; }
    public DateTime RefreshTokenExpiryDate { get; set; }
};
