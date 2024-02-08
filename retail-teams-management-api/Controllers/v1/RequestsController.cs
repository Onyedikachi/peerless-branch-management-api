using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchRequestModule;
using Retail.Branch.Services.BranchRequestModule.Models;
using Retail.Branch.Services.BranchRequestModule.QueryFilter;
using Retail.Branch.Services.Common;
using retail_teams_management_api.Attributes;

namespace retail_teams_management_api.Controllers.v1
{
    [Authorize]
    public class RequestsController : BaseApiController
    {
        private readonly IBranchRequestService _service;
        public RequestsController(IBranchRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] BranchRequestFilter filter)
        {
            var result = await _service.GetBranchRequests(filter, CurrentUser);
            return Ok(new SuccessApiResponse<object>("", result));
        }

        [HttpGet("activities/{request_id}")]
        [ProducesResponseType(typeof(SuccessApiResponse<PagedResponse<BranchRequestLog>>), 200)]
        public async Task<IActionResult> Activities([FromRoute] Guid request_id)
        {
            var result = _service.GetBranchRequestLogs(request_id);
            return Ok(new SuccessApiResponse<PagedResponse<BranchRequestLog>>("", result));
        }

        [HttpGet("analytics")]
        public IActionResult Analytics([FromQuery] string? filterBy = "created_by_me")
        {
            var result = _service.GetAnalytics(filterBy, CurrentUser);
            return Ok(result);
        }


        [BranchPermission($"{BranchPermissions.AUTHORIZE_CREATION_OR_MODIFICATION_REQUESTS},{ BranchPermissions.AUTHORIZE_REACTIVATION_OR_DEACTIVATION_REQUESTS}")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve([FromRoute] Guid id)
        {
            var result = await _service.ApproveRequest(id, CurrentUser);
            return Ok(result);
        }



        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var result = await _service.DeleteRequest(id, CurrentUser);
            return Ok(result);
        }

        [HttpPut("edit-bulk/{id}")]
        public async Task<IActionResult> EditBulk([FromRoute] Guid id, [FromBody]SaveBulkRequest model)
        {
            var result = await _service.UpdateBulkReques(id, model, CurrentUser);
            return Ok(result);
        }



        [HttpPut("edit/{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromBody] EditRequestModel model)
        {
            var result = await _service.UpdateRequest(model, CurrentUser);
            return Ok(result);
        }


        [BranchPermission(BranchPermissions.AUTHORIZE_CREATION_OR_MODIFICATION_REQUESTS)]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject([FromRoute] Guid id, [FromBody] RejectBranchRequest model)
        {
            var result = await _service.RejectRequest(id, CurrentUser, model);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetbyId([FromRoute] Guid id)
        {
            var result = await _service.GetDetails(id);
            return Ok(result);
        }



    }


}
