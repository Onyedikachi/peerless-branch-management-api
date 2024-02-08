using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Extensions.Caching;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Retail.Branch.Infrastructure;
using System.Diagnostics;

namespace retail_teams_management_api.Controllers
{
    public class MetaController : ControllerBase
    {
        private readonly ILogger<MetaController> _logger;
        public MetaController(ILogger<MetaController> logger)
        {
            _logger = logger;
        }
        [HttpGet("/info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(Program).Assembly;

            var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            _logger.LogWarning($"Version: {version}, Last Updated: {lastUpdate}");
            return Ok($"Version: {version}, Last Updated: {lastUpdate}");
        }

        [HttpGet("/test")]
        public async Task<IActionResult> testit()
        {
            SecretCacheConfiguration cacheConfiguration = new SecretCacheConfiguration
            {
                CacheItemTTL = 86400000
            };
            SecretsManagerCache cache = new SecretsManagerCache(cacheConfiguration);
            var set = cache.GetSecretString("dev-retailcore-teams-management-api-3tnqza").Result;
            AwsSetting aw = JsonConvert.DeserializeObject<AwsSetting>(set);
            var connectionstring = aw.ConnectionStrings__DefaultConnection;

            return Ok(connectionstring);
        }

        [HttpGet("/test3")]
        public async Task<IActionResult> testit3()
        {
            var constring = Environment.GetEnvironmentVariable("dbserver");
            return Ok(constring);

        }
    }
}
