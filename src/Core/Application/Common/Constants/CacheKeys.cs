namespace ApiTemplate.Application.Common.Constants;

public static class CacheKeys
{
    public const string User = "User_[userId]";
    public const string UserRoleIds = "UserRoleIds_[userId]";
    public const string RolePermissions = "RolePermissions_[roleId]";
    public const string UserCompanies = "UserCompanies_[userId]";
    public const string Company = "Company_[companyId]";
    public const string CompanyCostCenters = "CompanyCostCenters_[companyId]";
    public const string CompanyUserCostCenterIds = "CompanyUserCostCenterIds_[companyUserId]";
    public const string CompanyUserPermissions = "CompanyUserPermissions_[companyUserId]";
}
