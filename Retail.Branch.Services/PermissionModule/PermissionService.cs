using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Retail.Branch.Core.Common;
using Retail.Branch.Services.PermissionModule.Model;
using Retail.Branch.Services.UsersModule;
using Retail.Branch.Services.UsersModule.Models;
using Retail.Branch.Services.Util;

namespace Retail.Branch.Services.PermissionModule
{
    public class PermissionService : IPermissionService
    {
        private readonly IHttpHelper _http;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserService _userService;
        public PermissionService(IHttpHelper http, IConfiguration config, IMemoryCache memoryCache, IUserService userService)
        {
            _http = http;
            _config = config;
            _memoryCache = memoryCache;
            _userService = userService;
        }
        public async Task<List<PermissionModel>> GetUsersPermissions(string token, string userId, bool isSuperAdmin = false)
        {
            List<PermissionModel>? permissions;
            var cacheKey = $"BranchUser-{userId}";

            if (!_memoryCache.TryGetValue(cacheKey, out permissions))
            {
                permissions = await FetchPermissions(token, userId, isSuperAdmin);
                _memoryCache.Set(cacheKey, permissions, TimeSpan.FromMinutes(5));
            }
            return permissions!;

        }

        private async Task<List<PermissionModel>?> FetchPermissions(string token, string userId, bool isSuperadmin = false)
        {
            var userInfo = await _userService.GetUser(Guid.Parse(userId), token);
            var usersPermission = new List<UserInfoPermission>();
            foreach (var role in userInfo.roles)
            {
                usersPermission.AddRange(role.permissions);
            }

            var response = (from m in usersPermission
                            select new PermissionModel()
                            {
                                id = m.id,
                                name = m.name,
                                created_at = m.created_at,
                                description = m.description,
                                updated_at = m.updated_at
                            }).ToList();
            if (isSuperadmin)
            {
                response.AddRange(AddPermissionToSuperAdmin());
            }
            return response;
        }

        public async Task<bool> HasPermission(string permission, string token, string userId, bool isSuperAdmin = false)
        {
            var passedpermissions = permission.Split(',');
          
            var permissions = await GetUsersPermissions(token, userId,isSuperAdmin);

            foreach (var perm in passedpermissions)
            {
                var exist = permissions.Exists(c => c.name == perm);
                if (exist)
                    return true;
            }

           return false;

        

        }

        private List<PermissionModel> AddPermissionToSuperAdmin()
        {
            var permissionList = new List<PermissionModel>();
            var permissions = BranchPermissions.GetBranchPermissions();
            permissions.Remove("FAKE");
            foreach (var permission in permissions)
            {
                permissionList.Add(new PermissionModel()
                {
                    created_at = DateTime.UtcNow,
                    description = "",
                    id = Guid.NewGuid().ToString(),
                    name = permission

                });
            }

            return permissionList;
        }
    }
}
