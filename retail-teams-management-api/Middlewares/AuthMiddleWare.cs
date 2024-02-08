using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Retail.Branch.Core.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace retail_teams_management_api.Middlewares
{
    public class MyAuthenticationOptions :
     AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "Bearer";
        public string TokenHeaderName { get; set; } = "Authorization";
    }

    public class MyAuthenticationHandler : AuthenticationHandler<MyAuthenticationOptions>
    {
        // private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MyAuthenticationHandler> _logger;
        private readonly IConfiguration _configuration;
        public MyAuthenticationHandler
            (IOptionsMonitor<MyAuthenticationOptions> options,
            ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock //, IHttpClientFactory httpClientFactory
, IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
            //   _httpClientFactory = httpClientFactory;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //check header first
            if (!Request.Headers
                .ContainsKey(Options.TokenHeaderName))
            {
                return AuthenticateResult.Fail($"Missing header: {Options.TokenHeaderName}");
            }

            string beartokentoken = Request
                .Headers[Options.TokenHeaderName]!;
            var token = beartokentoken.Split(' ')[1];

            var tokenPayload = new TokenPayload() { Token = token };

            DecodeTokenResponse? tokenResponse = null;
#if DEBUG
           tokenResponse = DemoToken();

#else
               var url = _configuration.GetSection("Token:Url").Value;
                tokenResponse = await PostData(tokenPayload, $"{url}", token);
#endif


            if (tokenResponse is null)
            {
                return AuthenticateResult
                    .Fail($"Invalid token.");
            }
          
            var claims = new List<Claim>()
                     {

                         new Claim(ClaimTypes.Name, tokenResponse.firstname + " " + tokenResponse.lastname),
                         new Claim(ClaimTypes.Email,tokenResponse.email),
                         new Claim(ClaimTypes.NameIdentifier,tokenResponse.id),
                         new Claim(ClaimTypes.MobilePhone,tokenResponse.phone),
                         new Claim("is_super", tokenResponse.is_superuser.ToString()),
                         new Claim(ClaimTypes.UserData, beartokentoken)
                     };

            var claimsIdentity = new ClaimsIdentity
                (claims, this.Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal
                (claimsIdentity);

            return AuthenticateResult.Success
                (new AuthenticationTicket(claimsPrincipal,
                this.Scheme.Name));
        }

        private async Task<DecodeTokenResponse?> PostData(object model, string url, string key)
        {
            try
            {
                var _client = new HttpClient();// _httpClientFactory.CreateClient();
                _client.DefaultRequestHeaders.Add("Authorization", key);

                var response = await _client.PostAsJsonAsync(url, model);
                var datastring = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException("You are not authorized");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new ValidationException("Could not locate endpoint");

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return null;

                }


                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<DecodeTokenResponse>(datastring, new JsonSerializerSettings()
                    {
                        Error = (sender, error) => error.ErrorContext.Handled = true

                    }) ?? default;
                    return data;
                }
            }
            catch (Exception ex)
            {
                //  _logger.LogError(ex, "path: {path}, payload: {payload}", url, model);
                throw;
            }

            throw new UnauthorizedAccessException("Auth Service unavailable, try again");
        }

        private DecodeTokenResponse DemoToken()
        {
            return new DecodeTokenResponse()
            {
                firstname = "Demo",
                lastname = "admin",
                id = Guid.Empty.ToString(),
                phone = "080",
                email = "ojunix@hotmail.com",
                is_superuser = true,
            };
        }
    }
    public class TokenPayload
    {
        public string Token { get; set; }
    }
}



