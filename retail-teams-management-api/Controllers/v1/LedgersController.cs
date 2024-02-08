using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Core.Common;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.LedgerModule;
using Retail.Branch.Services.LedgerModule.Model;

namespace retail_teams_management_api.Controllers.v1
{
    [Authorize]
    public class LedgersController : BaseApiController
    {
        private readonly ILedgerService _service;

        public LedgersController(ILedgerService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<List<LedgerResult>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] LedgerListFilter filter)
        {
            var token = GetBearerToken();
            var result = await _service.GetAll(filter, token);
            return Ok(result);
        }
    }
}
