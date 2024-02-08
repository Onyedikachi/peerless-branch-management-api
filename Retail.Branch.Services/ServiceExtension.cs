using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Retail.Branch.Services.BranchModule;
using Retail.Branch.Services.BranchRequestModule;
using Retail.Branch.Services.LedgerModule;
using Retail.Branch.Services.PermissionModule;
using Retail.Branch.Services.UsersModule;
using Retail.Branch.Services.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services
{
    public static class ServiceExtension
    {
        public static void AddLogicServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddTransient<IBranchService, BranchService>(); 
            services.AddTransient<IBranchRequestService,BranchRequestService>();
            services.AddTransient<IUserService,UserServices>();
            services.AddTransient<ILedgerService,LedgerServices>();
            services.AddTransient<IPermissionService,PermissionService>();
            services.AddTransient<IHttpHelper,HttpHelper>();
        }
    }
}
