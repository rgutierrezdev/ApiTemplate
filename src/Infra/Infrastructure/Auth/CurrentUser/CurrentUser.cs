using Ardalis.Specification;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using ApiTemplate.Application.Common.Cache;
using ApiTemplate.Application.Common.Constants;
using ApiTemplate.Application.Common.Extensions;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Application.Common.Models;
using ApiTemplate.Application.Common.Persistence;
using ApiTemplate.Application.Features.Auth.Dtos;
using ApiTemplate.Domain.Common.Exceptions;
using ApiTemplate.Domain.Entities;
using ApiTemplate.Domain.Entities.Identity;

namespace ApiTemplate.Infrastructure.Auth.CurrentUser;

public class CurrentUser : ICurrentUser
{
    private IRepository<Company>? _companyRepository;
    private IRepository<CompanyUser>? _companyUserRepository;
    private IRepository<CompanyUserPermission>? _companyUserPermissionRepository;
    private IRepository<CostCenter>? _costCenterRepository;
    private IRepository<CostCenterUser>? _costCenterUserRepository;
    private IRepository<UserRole>? _userRoleRepository;
    private IRepository<RolePermission>? _rolePermissionsRepository;
    private IStorageService? _storageService;

    private IRepository<Company> CompanyRepository =>
        _companyRepository ??= _serviceProvider.GetRequiredService<IRepository<Company>>();

    private IRepository<CompanyUser> CompanyUserRepository =>
        _companyUserRepository ??= _serviceProvider.GetRequiredService<IRepository<CompanyUser>>();

    private IRepository<CompanyUserPermission> CompanyUserPermissionRepository =>
        _companyUserPermissionRepository ??= _serviceProvider.GetRequiredService<IRepository<CompanyUserPermission>>();

    private IRepository<CostCenter> CostCenterRepository =>
        _costCenterRepository ??= _serviceProvider.GetRequiredService<IRepository<CostCenter>>();

    private IRepository<CostCenterUser> CostCenterUserRepository => _costCenterUserRepository ??=
        _serviceProvider.GetRequiredService<IRepository<CostCenterUser>>();

    private IRepository<UserRole> UserRoleRepository => _userRoleRepository ??=
        _serviceProvider.GetRequiredService<IRepository<UserRole>>();

    private IRepository<RolePermission> RolePermissionsRepository => _rolePermissionsRepository ??=
        _serviceProvider.GetRequiredService<IRepository<RolePermission>>();

    private IStorageService StorageService => _storageService ??=
        _serviceProvider.GetRequiredService<IStorageService>();

    private readonly ICacheService _cacheService;
    private readonly ICacheKeyService _cacheKeyService;
    private readonly IServiceProvider _serviceProvider;

    public Guid Id { get; private set; }
    public string? Email { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public List<UserCompany>? Companies { get; private set; }

    public string? IpAddress { get; private set; }
    public string? Client { get; private set; }

    /**
     * Repositories are not injected through constructor to avoid a circular reference dependency
     */
    public CurrentUser(
        ICacheService cacheService,
        ICacheKeyService cacheKeyService,
        IServiceProvider serviceProvider
    )
    {
        _cacheService = cacheService;
        _cacheKeyService = cacheKeyService;
        _serviceProvider = serviceProvider;
    }

    private class CostCenterData : AuthCostCenterDto
    {
        public Guid CompanyId { get; set; }
    }

    private record CompanyUserData(Guid Id, Guid CompanyId);

    private record RolePermissionData(Guid RoleId, string PermissionName);

    public void ValidateCompanyAccess(Guid companyId)
    {
        var hasAccess = Companies?.Any(c => c.Id == companyId) ?? false;

        if (!hasAccess)
        {
            throw new ForbiddenException(
                ErrorCodes.NoAccessToCompany,
                $"You dont have access to Company with id '{companyId}'"
            );
        }
    }

    public void ValidateCompanyAndPermissionAccess(Guid companyId, string permission)
    {
        ValidateCompanyAccess(companyId);

        var company = Companies?.First(c => c.Id == companyId);
        var hasPermission = company!.Permissions.Contains(permission);

        if (!hasPermission)
        {
            throw new ForbiddenException(
                ErrorCodes.NoPermission,
                $"You dont have the permission {permission} for company with id '{companyId}'"
            );
        }
    }

    public void SetClient(string? ipAddress, string? client)
    {
        IpAddress = ipAddress;
        Client = client;
    }

    public async Task SetCurrentAsync(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default
    )
    {
        Id = userId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsAuthenticated = true;
        Companies = await GetUserCompaniesAsync(userId, cancellationToken);
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permission, CancellationToken cancellationToken)
    {
        var rolesPermissions = await GetRolePermissionsAsync(userId, cancellationToken);
        var hasPermission = rolesPermissions.Any(rp => rp.PermissionName == permission);

        return hasPermission;
    }

    public async Task<List<string>> GetPermissionsAsync(
        Guid userId,
        bool applyUnderscoreCase = false,
        CancellationToken cancellationToken = default
    )
    {
        var rolesPermissions = await GetRolePermissionsAsync(userId, cancellationToken);

        return rolesPermissions
            .Select(rp => applyUnderscoreCase ? rp.PermissionName.ToUnderScoreCase() : rp.PermissionName)
            .Distinct()
            .ToList();
    }

    private async Task<List<UserCompany>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = _cacheKeyService.GetKey(CacheKeys.UserCompanies, userId.ToString());
        var userCompanies = await _cacheService.GetAsync<List<CompanyUserData>>(cacheKey, cancellationToken);

        if (userCompanies == null)
        {
            userCompanies = await CompanyUserRepository.ListAsync<CompanyUserData>(query => query
                    .Where(cu => cu.UserId == userId && cu.DeletedDate == null),
                cancellationToken
            );

            await _cacheService.SetAsync(cacheKey, userCompanies, null, cancellationToken);
        }

        var companies = await GetCompaniesAsync(userCompanies, cancellationToken);

        return companies;
    }

    private async Task<List<UserCompany>> GetCompaniesAsync(
        List<CompanyUserData> userCompanies,
        CancellationToken cancellationToken
    )
    {
        var companies = new List<UserCompany>();
        var nonCachedCompanyIds = new List<Guid>();

        foreach (var userCompany in userCompanies)
        {
            var cacheKey = _cacheKeyService.GetKey(CacheKeys.Company, userCompany.CompanyId.ToString());
            var company = await _cacheService.GetAsync<UserCompany>(cacheKey, cancellationToken);

            if (company == null)
            {
                nonCachedCompanyIds.Add(userCompany.CompanyId);
                continue;
            }

            companies.Add(company);
        }

        if (nonCachedCompanyIds.Count > 0)
        {
            var nonCachedCompanies = await CompanyRepository.ListAsync<UserCompany>(query => query
                    .Select(c => new UserCompany()
                    {
                        Id = c.Id,
                        Type = c.Type,
                        Name = c.Name,
                        UsesPurchaseOrder = c.UsesPurchaseOrder,
                        Status = c.Status,
                        LastOnboardingStep = c.LastOnboardingStep,
                        LogoFileId = c.LogoFileId,
                        LogoPublic = c.LogoFile!.Public,
                        LogoUrl = c.LogoFile.Src,
                        LegalName = c.LegalName,
                        CountryName = c.City!.State.Country.Name,
                        CountryIsoCode = c.City.State.Country.IsoCode,
                        StateName = c.City.State.Name,
                        CityName = c.City.Name,
                        Address = c.Address,
                    })
                    .Where(c => nonCachedCompanyIds.Contains(c.Id)),
                cancellationToken
            );

            foreach (var company in nonCachedCompanies)
            {
                var userCompany = userCompanies.First(uc => uc.CompanyId == company.Id);
                company.CompanyUserId = userCompany.Id;

                if (company.LogoPublic != null && company.LogoUrl != null)
                {
                    company.LogoUrl = StorageService.GetBaseUrlBucket((bool)company.LogoPublic) + "/" +
                                      company.LogoUrl;
                }

                var cacheKey = _cacheKeyService.GetKey(CacheKeys.Company, company.Id.ToString());
                await _cacheService.SetAsync(cacheKey, company, null, cancellationToken);

                companies.Add(company);
            }
        }

        await SetCompaniesPermissionsAsync(companies, userCompanies, cancellationToken);
        await SetCompaniesCostCentersAsync(companies, cancellationToken);
        await FilterCostCenterUsersAsync(companies, userCompanies, cancellationToken);

        return companies;
    }

    private async Task SetCompaniesPermissionsAsync(
        IReadOnlyCollection<UserCompany> companies,
        List<CompanyUserData> userCompanies,
        CancellationToken cancellationToken = default
    )
    {
        var nonCachedUserCompanyIds = new List<Guid>();

        foreach (var userCompany in userCompanies)
        {
            var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyUserPermissions, userCompany.Id.ToString());
            var permissions = await _cacheService.GetAsync<List<string>>(cacheKey, cancellationToken);

            if (permissions == null)
            {
                nonCachedUserCompanyIds.Add(userCompany.Id);
                continue;
            }

            var company = companies.First(c => c.Id == userCompany.CompanyId);
            company.Permissions = permissions;
        }

        if (nonCachedUserCompanyIds.Count > 0)
        {
            var nonCachedUserPermissions = await CompanyUserPermissionRepository.ListAsync(query => query
                    .Include(p => p.Permission)
                    .Where(cup => nonCachedUserCompanyIds.Contains(cup.CompanyUserId)),
                cancellationToken
            );

            foreach (var group in nonCachedUserPermissions.GroupBy(csu => csu.CompanyUserId))
            {
                var permissions = group.Select(cup => cup.Permission.Name).ToList();

                var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyUserPermissions, group.Key.ToString());
                await _cacheService.SetAsync(cacheKey, permissions, null, cancellationToken);

                var companyId = userCompanies.First(uc => uc.Id == group.Key).CompanyId;
                var company = companies.First(c => c.Id == companyId);

                company.Permissions = permissions;
            }
        }
    }

    private async Task SetCompaniesCostCentersAsync(
        List<UserCompany> companies,
        CancellationToken cancellationToken = default
    )
    {
        var nonCachedCompanyIds = new List<Guid>();

        foreach (var company in companies)
        {
            var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyCostCenters, company.Id.ToString());
            var companyCostCenters = await _cacheService.GetAsync<List<AuthCostCenterDto>>(cacheKey, cancellationToken);

            if (companyCostCenters == null)
            {
                nonCachedCompanyIds.Add(company.Id);
                continue;
            }

            company.CostCenters = companyCostCenters;
        }

        if (nonCachedCompanyIds.Count > 0)
        {
            var nonCachedCompanyCostCenters = await CostCenterRepository.ListAsync<CostCenterData>(query => query
                    .Where(cs => nonCachedCompanyIds.Contains(cs.CompanyId)),
                cancellationToken
            );

            foreach (var companyId in nonCachedCompanyIds)
            {
                var companyCostCenters = nonCachedCompanyCostCenters
                    .Where(cs => cs.CompanyId == companyId)
                    .Select(cs => cs.Adapt<AuthCostCenterDto>())
                    .ToList();

                var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyCostCenters, companyId.ToString());
                await _cacheService.SetAsync(cacheKey, companyCostCenters, null, cancellationToken);

                var company = companies.First(c => c.Id == companyId);

                company.CostCenters = companyCostCenters;
            }
        }
    }

    private async Task FilterCostCenterUsersAsync(
        List<UserCompany> companies,
        IEnumerable<CompanyUserData> userCompanies,
        CancellationToken cancellationToken
    )
    {
        var nonCachedUserCompanyIds = new List<Guid>();

        foreach (var userCompany in userCompanies)
        {
            var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyUserCostCenterIds, userCompany.Id.ToString());
            var costCenterIds = await _cacheService.GetAsync<List<Guid>>(cacheKey, cancellationToken);

            if (costCenterIds == null)
            {
                nonCachedUserCompanyIds.Add(userCompany.Id);
                continue;
            }

            // filter out cost centers not assigned to the user
            foreach (var company in companies)
            {
                company.CostCenters = company.CostCenters
                    .Where(cs => costCenterIds.Contains(cs.Id))
                    .ToList();
            }
        }

        if (nonCachedUserCompanyIds.Count > 0)
        {
            var nonCachedUserCostCenters = await CostCenterUserRepository.ListAsync(query => query
                    .Where(csu => nonCachedUserCompanyIds.Contains(csu.CompanyUserId)),
                cancellationToken
            );

            foreach (var group in nonCachedUserCostCenters.GroupBy(csu => csu.CompanyUserId))
            {
                var costCenterIds = group.Select(csu => csu.CostCenterId).ToList();

                var cacheKey = _cacheKeyService.GetKey(CacheKeys.CompanyUserCostCenterIds, group.Key.ToString());
                await _cacheService.SetAsync(cacheKey, costCenterIds, null, cancellationToken);

                // filter out cost centers not assigned to the user
                foreach (var company in companies)
                {
                    company.CostCenters = company.CostCenters
                        .Where(cs => costCenterIds.Contains(cs.Id))
                        .ToList();
                }
            }
        }
    }

    private async Task<List<Guid>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roleIdsCacheKey = _cacheKeyService.GetKey(CacheKeys.UserRoleIds, userId.ToString());
        var roleIds = await _cacheService.GetAsync<List<Guid>>(roleIdsCacheKey, cancellationToken);

        if (roleIds != null)
            return roleIds;

        roleIds = await UserRoleRepository.ListAsync<Guid>(q =>
                q.Select(ur => ur.RoleId)
                    .Where(ur => ur.UserId == userId)
                    .Where(ur => ur.StartDate == null || ur.StartDate <= DateTime.UtcNow)
                    .Where(ur => ur.EndDate == null || ur.EndDate >= DateTime.UtcNow),
            cancellationToken
        );

        roleIds = roleIds.Distinct().ToList();

        await _cacheService.SetAsync(roleIdsCacheKey, roleIds, null, cancellationToken);

        return roleIds;
    }

    private async Task<List<RolePermissionData>> GetRolePermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var roleIds = await GetUserRoleIdsAsync(userId, cancellationToken);
        var rolesPermissions = new List<RolePermissionData>();
        var nonCachedRoleIds = new List<Guid>();

        foreach (var roleId in roleIds)
        {
            var cacheKey = _cacheKeyService.GetKey(CacheKeys.RolePermissions, roleId.ToString());
            var rolePermissions = await _cacheService.GetAsync<List<RolePermissionData>>(cacheKey, cancellationToken);

            if (rolePermissions == null)
            {
                nonCachedRoleIds.Add(roleId);
                continue;
            }

            rolesPermissions.AddRange(rolePermissions);
        }

        if (nonCachedRoleIds.Count > 0)
        {
            var nonCachedRolesPermissions = await RolePermissionsRepository.ListAsync<RolePermissionData>(q => q
                    .Where(rp => nonCachedRoleIds.Contains(rp.RoleId)),
                cancellationToken
            );

            foreach (var roleId in nonCachedRoleIds)
            {
                var rolePermissions = nonCachedRolesPermissions.Where(rp => rp.RoleId == roleId).ToList();

                var cacheKey = _cacheKeyService.GetKey(CacheKeys.RolePermissions, roleId.ToString());
                await _cacheService.SetAsync(cacheKey, rolePermissions, null, cancellationToken);
            }

            rolesPermissions.AddRange(nonCachedRolesPermissions);
        }

        return rolesPermissions;
    }
}
