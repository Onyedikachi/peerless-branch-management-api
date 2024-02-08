using Microsoft.AspNetCore.Authorization;

namespace retail_teams_management_api.Middlewares
{
    public class PermissionRequirement:IAuthorizationRequirement
    {
       
        public PermissionRequirement(string permission)
        {
            Permission = permission;    
        }

        public string Permission { get; }
    }
}
