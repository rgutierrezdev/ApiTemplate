namespace ApiTemplate.Application.Common.Interfaces;

public interface ICookieService : IScopedService
{
    void AttachTokenCookies(string accessToken, string refreshToken);

    public void ClearTokenCookies();

    string GetRefreshToken();
}
