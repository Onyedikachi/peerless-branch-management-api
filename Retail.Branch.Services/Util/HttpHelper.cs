using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Retail.Branch.Services.Util
{
    public class HttpHelper : IHttpHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpHelper> _logger;

        public HttpHelper(IHttpClientFactory httpClientFactory, ILogger<HttpHelper> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<(bool status, string message, T? data)> PostData<T>(object model, string url, string token)
        {
            try
            {
                var _client = _httpClientFactory.CreateClient();
                if (_client.DefaultRequestHeaders.Authorization == null)
                {
                    _client.DefaultRequestHeaders.Add("Authorization", $"{token}");

                }

                var response = await _client.PostAsJsonAsync(url, model);
                var datastring = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("path: {path}, payload: {@payload}, response:{response}", url, model, datastring);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new ValidationException("You are not authorised");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ValidationException("Could not locate endpoint");

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return (false, "Invalid Data", default);

                }

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<T>(datastring, new JsonSerializerSettings()
                    {
                        Error = (sender, error) => error.ErrorContext.Handled = true

                    }) ?? default;
                    return (true, "Success", data!);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "path: {path}, payload: {payload}", url, model);
                throw new ValidationException(ex.Message);
            }

            throw new ValidationException("Remote server returned an error occured");
        }

        public async Task<T?> GetData<T>(string url, string token)
        {
            var _client = _httpClientFactory.CreateClient();
            if (_client.DefaultRequestHeaders.Authorization == null)
            {
               // _client.DefaultRequestHeaders.add
                _client.DefaultRequestHeaders.Add("Authorization", $"{token}");

            }
            var response = await _client.GetAsync(url);
            var datastring = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new ValidationException("No matching record found");

           

            var data = JsonConvert.DeserializeObject<T>(datastring, new JsonSerializerSettings()
            {
                Error = (sender, error) => error.ErrorContext.Handled = true

            }) ?? default;

            return data;
        }
    }
}
