using Amazon.Runtime.Internal.Util;
using Amazon.SecretsManager.Extensions.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Retail.Branch.Infrastructure
{
    public static class ServiceRegistration
    {
       
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            SecretCacheConfiguration cacheConfiguration = new SecretCacheConfiguration
            {
                CacheItemTTL = 86400000
            };
            //SecretsManagerCache cache = new SecretsManagerCache(cacheConfiguration);
            //var set = cache.GetSecretString("qa-retail-teams-management-api-b9dxsm").Result;
            //AwsSetting aw = JsonConvert.DeserializeObject<AwsSetting>(set);
            //var connectionstring = aw.ConnectionStrings__DefaultConnection;
//#if !DEBUG
     
//#endif



            services.AddDbContext<BranchDataContext>(options =>
           options.UseNpgsql(
              configuration.GetConnectionString("DefaultConnection"),
               b => b.MigrationsAssembly(typeof(BranchDataContext).Assembly.FullName)),ServiceLifetime.Transient);
            #region Repositories
            #endregion

           

        }

        public static IConfigurationBuilder AddSecretsFromAwsSecretsManager(this IConfigurationBuilder builder, IConfiguration config)
        {
            var secretsManagerHelper = new AwsSecretsManagerHelper();
            var secretJson = secretsManagerHelper.GetSecret("dev-retailcore-teams-management-api-3tnqza");

            if (!string.IsNullOrEmpty(secretJson))
            {
                builder.AddSecretsFromAwsSecretsManager(config);
              //  builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(secretJson)));
            }

            return builder;
        }
    }

    public class AwsSecretsManagerHelper
    {
        private readonly IAmazonSecretsManager _secretsManager;

        public AwsSecretsManagerHelper()
        {
            _secretsManager = new AmazonSecretsManagerClient(RegionEndpoint.EUWest3);
        }

        public string GetSecret(string secretName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = _secretsManager.GetSecretValueAsync(request).Result;

            if (response.SecretString != null)
            {
                return response.SecretString;
            }

            return null;
        }


    }


    //Using a configuration provider
    public class AmazonSecretsManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly string _region;
        private readonly string _secretName;

        public AmazonSecretsManagerConfigurationProvider(string region, string secretName)
        {
            _region = region;
            _secretName = secretName;
        }

        public override void Load()
        {
            var secret = GetSecret();

            Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(secret);
        }

        private string GetSecret()
        {
            var request = new GetSecretValueRequest
            {
                SecretId = _secretName,
                VersionStage = "AWSCURRENT" // VersionStage defaults to AWSCURRENT if unspecified.
            };

            using (var client =
            new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region)))
            {
                var response = client.GetSecretValueAsync(request).Result;

                string secretString;
                if (response.SecretString != null)
                {
                    secretString = response.SecretString;
                }
                else
                {
                    var memoryStream = response.SecretBinary;
                    var reader = new StreamReader(memoryStream);
                    secretString =
            System.Text.Encoding.UTF8
                .GetString(Convert.FromBase64String(reader.ReadToEnd()));
                }

                return secretString;
            }
        }
    }
    public class AmazonSecretsManagerConfigurationSource : IConfigurationSource
{
    private readonly string _region;
    private readonly string _secretName;

    public AmazonSecretsManagerConfigurationSource(string region, string secretName)
    {
        _region = region;
        _secretName = secretName;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AmazonSecretsManagerConfigurationProvider(_region, _secretName);
    }
}
}
