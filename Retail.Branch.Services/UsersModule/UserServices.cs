using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.UsersModule.Models;
using Retail.Branch.Services.Util;

namespace Retail.Branch.Services.UsersModule
{
    public class UserServices : IUserService
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IHttpHelper _client;
        private readonly IConfiguration _config;
        public UserServices(ILogger<UserServices> logger, IHttpHelper httpClientFactory, IConfiguration config)
        {
            _logger = logger;
            _client = httpClientFactory;
            _config = config;
        }
        public async Task<PagedResponse<List<UserInfoModel>>> GetUsers(UserListFilter filter, string token)
        {
            var baseUrl = _config.GetSection("Endpoints:UsersService").Value;
            var url = $"{baseUrl}/api/v1/users/?page={filter.Page}&page_size={filter.Page_Size}&state={filter.state}&search={filter.Search}";

            var data = await _client.GetData<UsersListModel>(url, token);


            return new PagedResponse<List<UserInfoModel>>("", data!.results, data.total_pages, filter.Page, filter.Page_Size);

        }


        public async Task<UserInfoModel> GetUser(Guid userid, string token)
        {
            var baseUrl = _config.GetSection("Endpoints:UsersService").Value;
            var url = $"{baseUrl}/api/v1/users/{userid}/";

            var data = await _client.GetData<UserInfoModel>(url, token);
            return data;

        }


    }
}
