namespace ApiTemplate.Application.Common.Constants;

public static class Permissions
{
    // These are assigned directly to User by Company (CompanyUser)

    #region Customer/Vendor Permissions

    public static string[] CustomerVendorPermissions { get; } =
    {
        CompanyProfileAdmin,
        CompanySettingsAdmin,
        CompanyUsersAdmin,
    };

    public static string[] CustomerOnlyPermissions { get; } =
    {
        CompanyRequestsWrite,
        CompanyAwardQuote,
        CompanyApprovePurchase,
    };

    public static string[] VendorOnlyPermissions { get; } =
    {
        CompanyQuotesWrite,
    };

    #region Shared

    public const string CompanyProfileAdmin = nameof(CompanyProfileAdmin);
    public const string CompanySettingsAdmin = nameof(CompanySettingsAdmin);
    public const string CompanyUsersAdmin = nameof(CompanyUsersAdmin);

    #endregion

    #region Customer Only

    public const string CompanyRequestsWrite = nameof(CompanyRequestsWrite);
    public const string CompanyAwardQuote = nameof(CompanyAwardQuote);
    public const string CompanyApprovePurchase = nameof(CompanyApprovePurchase);

    #endregion

    #region Vendor Only

    public const string CompanyQuotesWrite = nameof(CompanyQuotesWrite);

    #endregion

    #endregion


    // These are assigned to roles which are assigned to admin users

    #region Admin Permissions

    public const string RolesRead = nameof(RolesRead);
    public const string RolesWrite = nameof(RolesWrite);
    public const string CouponsRead = nameof(CouponsRead);
    public const string CouponsWrite = nameof(CouponsWrite);
    public const string UsersRead = nameof(UsersRead);
    public const string UsersWrite = nameof(UsersWrite);

    #endregion
}
