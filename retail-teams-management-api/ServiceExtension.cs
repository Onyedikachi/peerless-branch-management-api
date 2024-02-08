using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Retail.Branch.Services;
using retail_teams_management_api.Middlewares;
using System.Security.Claims;

namespace retail_teams_management_api
{
    public static class ServiceExtension
    {


        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddSwaggerGen
                (g =>
                {
                    g.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Retailcore Branch Management API",
                        Description = "Documentation for Retailcore Branch Management API",
                        Contact = new OpenApiContact
                        {
                            Name = "AppQuest",
                            Email = "info@appquest.com.ng",
                            Url = new Uri("http://appquest.com.ng"),
                        },

                    });
                    g.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",

                    });
                    g.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                            Scheme = "Bearer",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    },
                });


                });

            services.AddApiVersioning();
        }


        public static void AddCurrentUser(this IServiceCollection services)
        {
            services.AddAuthentication(MyAuthenticationOptions.DefaultScheme)
                                        .AddScheme<MyAuthenticationOptions, MyAuthenticationHandler>
                                        (MyAuthenticationOptions.DefaultScheme,
                                                                              options =>
                                                                              {

                                                                              });

            services.AddHttpContextAccessor();
            services.AddScoped<BranchUser>((serviceProvider) =>
            {
                var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                return new BranchUser(httpContext?.HttpContext?.User as ClaimsPrincipal);
            });

            services.AddAuthorization();
            services.AddSingleton<IAuthorizationHandler,PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        }
    }
}
