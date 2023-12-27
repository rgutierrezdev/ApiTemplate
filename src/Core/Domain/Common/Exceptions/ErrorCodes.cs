namespace ApiTemplate.Domain.Common.Exceptions;

public static class ErrorCodes
{
    public const string ParamsMissMatch = nameof(ParamsMissMatch);
    public const string AuthenticationFailed = nameof(AuthenticationFailed);
    public const string InvalidAccessToken = nameof(InvalidAccessToken);
    public const string ValidationError = nameof(ValidationError);
    public const string NoHttpContext = nameof(NoHttpContext);
    public const string MissingRefreshCookie = nameof(MissingRefreshCookie);
    public const string LoginFailed = nameof(LoginFailed);
    public const string InvalidRefreshToken = nameof(InvalidRefreshToken);
    public const string DuplicatedEmail = nameof(DuplicatedEmail);
    public const string NoPermission = nameof(NoPermission);
    public const string ResetTokenExpired = nameof(ResetTokenExpired);
    public const string InvalidResetToken = nameof(InvalidResetToken);
    public const string UserNotFound = nameof(UserNotFound);
    public const string RoleNotFound = nameof(RoleNotFound);
    public const string CouponNotFound = nameof(CouponNotFound);
    public const string CouponAlreadyApplied = nameof(CouponAlreadyApplied);
    public const string CompanyNotFound = nameof(CompanyNotFound);
    public const string NoAccessToCompany = nameof(NoAccessToCompany);
    public const string CityNotFound = nameof(CityNotFound);
    public const string ContextualDocumentNotFound = nameof(ContextualDocumentNotFound);
    public const string FileNotFound = nameof(FileNotFound);
    public const string CompanyDocumentNotFound = nameof(CompanyDocumentNotFound);
    public const string CompanyDocumentFileNotFound = nameof(CompanyDocumentFileNotFound);
    public const string InvalidCreditAvailability = nameof(InvalidCreditAvailability);
    public const string MaxCompanyDocumentFiles = nameof(MaxCompanyDocumentFiles);
    public const string InvalidSignToken = nameof(InvalidSignToken);
    public const string CompanyRegistrationAlreadySigned = nameof(CompanyRegistrationAlreadySigned);
    public const string CompanyHasCoupon = nameof(CompanyHasCoupon);
    public const string CouponExpired = nameof(CouponExpired);
    public const string CompanyInReview = nameof(CompanyInReview);
    public const string CompanyAddressNotFound = nameof(CompanyAddressNotFound);
    public const string CompanyInvalidStatus = nameof(CompanyInvalidStatus);
    public const string CompanyHasNoChanges = nameof(CompanyHasNoChanges);
    public const string CompanyOwnerNotFound = nameof(CompanyOwnerNotFound);
}
