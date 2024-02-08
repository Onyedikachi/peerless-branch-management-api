using Retail.Branch.Services.PermissionModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.PermissionModule
{
    public interface IPermissionService
    {
        Task<List<PermissionModel>> GetUsersPermissions(string token, string userId, bool isSuperAdmin = false);
        Task<bool> HasPermission(string permission, string token, string userId, bool isSuperAdmin=false);
    }
}
