using Microsoft.AspNetCore.Mvc;
using Retail.Branch.Core.Common;
using Retail.Branch.Infrastructure;
using Retail.Branch.Services;
using retail_teams_management_api.Middlewares;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace retail_teams_management_api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers().AddJsonOptions(options =>
            {
               options.JsonSerializerOptions.PropertyNamingPolicy = JsonLowerCase.Instance;
            });
            builder.Services.AddHealthChecks();
           
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithMethods("GET", "PUT", "DELETE", "POST")
                    );
            });
            builder.Services.AddCurrentUser();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = (int)HttpStatusCode.BadRequest,
                        Detail = "Please refer to the errors property for additional details"
                    };

                    return new BadRequestObjectResult(new FailureApiResponse("Bad request", problemDetails.Errors, 400))
                    {

                    };
                };

            });

            builder.Services.AddSwaggerServices();
           // builder.Configuration.AddSecretsFromAwsSecretsManager(builder.Configuration);
            builder.Services.AddMemoryCache();
            builder.Services.AddLogicServices(builder.Configuration);
            builder.Services.AddPersistenceInfrastructure(builder.Configuration);
            builder.Services.AddHostedService<DeleteDraftBackgroundService>();


            var app = builder.Build();
            app.MapHealthChecks("/health");
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<ErrorHandlerMiddleware>();           
            app.UseHttpsRedirection();
            app.UseAuthentication();
           

           
            app.UseRouting();

            app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

           
            app.UseAuthorization();
            app.UseStaticFiles();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                try
                {
                    Log.Information("Connecting to Db");
                    if (scope.ServiceProvider.GetRequiredService<BranchDataContext>().Database.CanConnect())
                    {
                        Log.Information("Starting Db Migration");
                        scope.ServiceProvider.GetRequiredService<BranchDataContext>().Database.Migrate();
                        Log.Information("Db Migration Completed");
                    }
                    else
                    {
                        Log.Error("Could not connect to Database");
                    }

                    Log.Information("Application Starting");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "An error occurred seeding the DB");
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }


            app.Run();
           
          

        }


    }
}