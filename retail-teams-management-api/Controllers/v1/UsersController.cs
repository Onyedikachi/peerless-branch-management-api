using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Core.Common;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.PermissionModule;
using Retail.Branch.Services.PermissionModule.Model;
using Retail.Branch.Services.UsersModule;
using Retail.Branch.Services.UsersModule.Models;

namespace retail_teams_management_api.Controllers.v1
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _service;
        private readonly IPermissionService _permissionService;
        public UsersController(IUserService service, IPermissionService permissionService)
        {
            _service = service;
            _permissionService = permissionService;
        }
        [HttpGet]
        [ProducesResponseType( typeof(PagedResponse<List<UserInfoModel>>),200)]
        public async Task<IActionResult> GetUsers([FromQuery] UserListFilter filter)
        {
            var token = GetBearerToken();
            var result = await _service.GetUsers(filter, token);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(UserInfoModel), 200)]
        public async Task<IActionResult> GetUser([FromRoute] Guid userId)
        {
            var token = GetBearerToken();
            var result = await _service.GetUser(userId, token);
            return Ok(new SuccessApiResponse<UserInfoModel>("User found",result));
        }

        [HttpPost("permissions")]
        [ProducesResponseType(typeof(List<PermissionModel>), 200)]
        public async Task<IActionResult> GetPermissions()
        {
            var token = GetBearerToken();
            var result = await _permissionService.GetUsersPermissions(token, CurrentUser.UserId,bool.Parse( CurrentUser.Is_Super));
            return Ok(new SuccessApiResponse<List<PermissionModel>>("Permissions", result));
        }

        [HttpPost("has-branch-permission/{permission}")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> HasPermissions([FromRoute]string permission)
        {
            var permissionlist = permission.Split(',');
            foreach (var perm in permissionlist)
            {
                var exist = BranchPermissions.GetBranchPermissions().Contains(perm);
                if (exist is false)
                {
                    return BadRequest(new FailureApiResponse($"branch permission {perm}does not exist", false));
                }
            }
            var token = GetBearerToken();
            var result = await _permissionService.HasPermission(permission, token, CurrentUser.UserId, bool.Parse( CurrentUser.Is_Super));
            return Ok(new SuccessApiResponse<bool>("Permission", result));
        }
    }
}
