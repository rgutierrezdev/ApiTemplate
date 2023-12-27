namespace ApiTemplate.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid Id { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    bool IsAuthenticated { get; }
    List<UserCompany>? Companies { get; }

    string? IpAddress { get; }
    string? Client { get; }

    Task SetCurrentAsync(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default
    );

    void ValidateCompanyAccess(Guid companyId);

    void ValidateCompanyAndPermissionAccess(Guid companyId, string permission);

    void SetClient(string? ipAddress, string? client);

    Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken = default);

    Task<List<string>> GetPermissionsAsync(
        Guid userId,
        bool applyUnderscoreCase = false,
        CancellationToken cancellationToken = default
    );
}
