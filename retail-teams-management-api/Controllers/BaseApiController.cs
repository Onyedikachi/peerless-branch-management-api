using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Services;
using System.Security.Claims;

namespace retail_teams_management_api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class BaseApiController : ControllerBase
    {

        public BranchUser CurrentUser
        {
            get
            {
                return new BranchUser(User as ClaimsPrincipal);
            }
        }

        protected string GetBearerToken()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(token)) throw new UnauthorizedAccessException("No token was passed!");
            return token.ToString().Trim();
        }

        protected string GetOrigin()
        {
            var origin = Request.HttpContext.Request.Host.Value;
            var result = $"{Request.HttpContext.Request.Scheme}://{origin}";
            return result;
        }
    }
}
