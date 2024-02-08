using Microsoft.AspNetCore.Authorization;
using Retail.Branch.Services.PermissionModule;
using retail_teams_management_api.Exceptions;
using System.Security.Claims;

namespace retail_teams_management_api.Middlewares
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var token = context?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
            bool isSuperAdmin = bool.Parse( context?.User?.Claims?.FirstOrDefault(c => c.Type == "is_super")?.Value??"False");
            if(string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("You must be logged in to perform this action");
            }
            var haspermisson = await _permissionService.HasPermission(requirement.Permission, token, userId,isSuperAdmin);
            if (haspermisson)
            {
                context?.Succeed(requirement);
            }
            else
            {            
                throw new PermissionException("You are not authorized to perform this action");
            }
           
        }
    }
}
