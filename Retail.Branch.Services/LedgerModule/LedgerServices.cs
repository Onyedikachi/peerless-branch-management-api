using Microsoft.Extensions.Configuration;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.LedgerModule.Model;
using Retail.Branch.Services.Util;

namespace Retail.Branch.Services.LedgerModule
{
    public class LedgerServices : ILedgerService
    {
        private readonly IHttpHelper _httpClient;
        private readonly IConfiguration _config;

        public LedgerServices(IHttpHelper httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory;
            _config = config;
        }


        public async Task<PagedResponse<List<LedgerResult>>> GetAll(LedgerListFilter filter, string token)
        {
            var baseUrl = _config.GetSection("Endpoints:AccountsService").Value;
            var url = $"{baseUrl}/api/v1/accounting/gl/?page={filter.Page}&page_size={filter.Page_Size}&state={filter.state}&search={filter.Search}";

            var response = await _httpClient.GetData<LedgerResponseModel>(url, token);


            return new PagedResponse<List<LedgerResult>>("", response!.results, response.total, filter.Page, filter.Page_Size);

        }
    }
}
