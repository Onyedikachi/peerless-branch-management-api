using Ganss.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Core.Common;
using Retail.Branch.Services;
using Retail.Branch.Services.BranchModule;
using Retail.Branch.Services.BranchModule.Models;
using retail_teams_management_api.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace retail_teams_management_api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("/v{version:apiVersion}")]
    public class CreateController : ControllerBase
    {
        private readonly IBranchService _service;
        public CreateController(IBranchService service)
        {
            _service = service;
        }

        [BranchPermission(BranchPermissions.CREATE_BULK_TEAM)]
        [Route("bulk-create")]
        [HttpPost]
        public async Task<IActionResult> BulkCreate([FromBody] SaveBulkRequest model)
        {
            var result = await _service.CreateBulkReques(model, CurrentUser);
            return Ok(result);
        }

        [BranchPermission(BranchPermissions.CREATE_BULK_TEAM)]
        [HttpPost("bulk-create/upload")]
        public async Task<IActionResult> CreateUpload(IFormFile file)
        {

            // Specify the number of rows to skip

            IEnumerable<CreateBulkBranchItem> branches = new List<CreateBulkBranchItem>();
            var filstream = file.OpenReadStream();
           
            var mapper = new ExcelMapper(filstream)
            {
                 HeaderRowNumber=1
            };
            var head = mapper.HeaderRow;
            branches = mapper.Fetch<CreateBulkBranchItem>();
          

            if (branches.Count() == 0)
                throw new ValidationException(" Please upload a valid file with branches.");

            var firstrow = branches.FirstOrDefault();
            if (firstrow?.Number != "Branch Location")
                throw new ValidationException(" Please upload a valid file with branches.");


            var result = await _service.BulkUpload(branches.Skip(1).ToList());
            return Ok(result);
        }

        [Authorize]
        [BranchPermission(BranchPermissions.CREATE_SINGLE_TEAM)]

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateSingleBranch model)
        {

            var result = await _service.CreateBranch(model, CurrentUser);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("validate-name")]
        public async Task<IActionResult> Validate([FromBody] ValidateNameModel model)
        {
            var result = await _service.VerifyName(model);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("validate-address")]
        public async Task<IActionResult> ValidateAddress([FromBody] ValidateBranchAddressModel model)
        {
            var result = await _service.VerifyAddress(model);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

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
    }
}
