using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Services.BranchModule;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchModule.QueryFilters;
using Retail.Branch.Services.BranchRequestModule;
using Retail.Branch.Services.Common;
using retail_teams_management_api.Attributes;
using retail_teams_management_api.Models;
using System.ComponentModel.DataAnnotations;

namespace retail_teams_management_api.Controllers.v1
{

    [Authorize]
    public class BranchController : BaseApiController
    {
        private readonly IBranchService _service;
        private readonly IBranchRequestService _branchRequestService;
        public BranchController(IBranchService service, IBranchRequestService branchRequestService)
        {
            _service = service;
            _branchRequestService = branchRequestService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(SuccessApiResponse<PagedResponse<BranchModel>>), 200)]
        public async Task<IActionResult> Post([FromBody] GetBranchFilter filter)
        {
            var result = await _service.GetBranches(filter, CurrentUser?.UserId, CurrentUser?.BranchId);
            return Ok(new SuccessApiResponse<object>("", result));
        }


        [HttpGet("analytics")]
        [ProducesResponseType(typeof(SuccessApiResponse<object>), 200)]
        public IActionResult Analytics([FromQuery] string? filterBy = "created_by_me")
        {
            var result = _service.BranchAnalytics(filterBy, CurrentUser);
            return Ok(result);
        }
        [BranchPermission(BranchPermissions.RE_OR_DE_ACTIVATE_TEAM)]
        [HttpPut("deactivate")]
        public async Task<IActionResult> Deactivate([FromBody] DeactivationRequestModel model)
        {
            var result = await _branchRequestService.SaveDiactivationRequest(model, CurrentUser);
            return Ok(result);
        }


        [HttpPut("modify/{id}")]
        public async Task<IActionResult> Modify([FromRoute] Guid id, [FromBody] ModifyBranch model)
        {
            var result = await _service.Update(id, model, CurrentUser);
            return Ok(result);
        }


        [HttpPut("reactivate")]
        public async Task<IActionResult> Reactivate([FromBody] ReactivateModel model)
        {
            var result = await _service.ActivateBranch(model.BranchId, CurrentUser);
            return Ok(result);
        }


        [HttpGet("{code}")]
        public async Task<IActionResult> Code([FromRoute] string code)
        {
            var result = await _service.GetBranchByCode(code);
            return Ok(result);
        }



        [HttpGet("activities/{request_id}")]
        [ProducesResponseType(typeof(SuccessApiResponse<PagedResponse<BranchRequestLog>>), 200)]
        public IActionResult Activities([FromRoute] Guid request_id)
        {
            var result = _service.GetBranchRequestLogs(request_id);
            return Ok(new SuccessApiResponse<PagedResponse<BranchRequestLog>>("", result));
        }

        [HttpPost("UploadSingleDocument")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]

        public IActionResult UploadSingleDocument([FromBody] UploadDocument document)
        {
            var uploads = GetSingleDocumentUrl(document);
            return Ok(new SuccessApiResponse<string>("file path", uploads));
        }

        [AllowAnonymous]
        [HttpGet("template")]
        public async Task<IActionResult> GetTemplate()
        {
            var template = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot", "data", "bulktemplate.xlsx"));

            var filebytes = System.IO.File.ReadAllBytes(template);
            return File(filebytes, "application/octet-stream", "bulktemplate.xlsx");

        }

        [BranchPermission(BranchPermissions.FAKE)]
        [HttpGet("atest")]
        public IActionResult JustTest()
        {
            return Ok();
        }

        private string GetSingleDocumentUrl(UploadDocument doc)
        {
            var origin = GetOrigin();
            try
            {
                byte[] imageBtyes = Convert.FromBase64String(doc.Base64);
                var filename = Guid.NewGuid() + doc.ext;
                var filepath = Path.Combine("wwwroot", "uploads\\" + filename);
                System.IO.File.WriteAllBytes(filepath, imageBtyes);

                return $"{origin}/uploads/{filename}";

            }
            catch (Exception ex)
            {
                throw new ValidationException($"A file upload error occured,{ex.Message}");
            }
        }

    }
}
