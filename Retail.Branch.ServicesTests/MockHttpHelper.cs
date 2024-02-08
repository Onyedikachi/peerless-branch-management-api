using Newtonsoft.Json;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.LedgerModule.Model;
using Retail.Branch.Services.UsersModule.Models;
using Retail.Branch.Services.Util;

namespace Retail.Branch.ServicesTests
{
    public class MockLegerHttpHelper : IHttpHelper
    {
        public async Task<T?> GetData<T>(string url, string token)
        {
            var pagedData = new PagedResponse<List<LedgerResult>>("", TestData.LedgerData(), 1, 1, 1);

            var datastring = JsonConvert.SerializeObject(pagedData);
            var data = JsonConvert.DeserializeObject<T>(datastring, new JsonSerializerSettings()
            {
                Error = (sender, error) => error.ErrorContext.Handled = true

            }) ?? default;

            return data;
        }

        public async Task<(bool status, string message, T? data)> PostData<T>(object model, string url, string token)
        {
            throw new NotImplementedException();
        }
    }

    public class MockUsersListHttpHelper : IHttpHelper
    {
        public async Task<T?> GetData<T>(string url, string token)
        {
            if (token == "all")
            {


                var pagedData = new PagedResponse<List<UserInfoModel>>("", TestData.GetUsers(), 1, 1, 1);

                var datastring = JsonConvert.SerializeObject(pagedData);
                var data = JsonConvert.DeserializeObject<T>(datastring, new JsonSerializerSettings()
                {
                    Error = (sender, error) => error.ErrorContext.Handled = true

                }) ?? default;

                return data;
            }
            else
            {
                var user = TestData.GetUser();
                var datastring = JsonConvert.SerializeObject(user);
                var data = JsonConvert.DeserializeObject<T>(datastring, new JsonSerializerSettings()
                {
                    Error = (sender, error) => error.ErrorContext.Handled = true

                }) ?? default;

                return data;

            }
        }

        public async Task<(bool status, string message, T? data)> PostData<T>(object model, string url, string token)
        {
            throw new NotImplementedException();
        }
    }
}
