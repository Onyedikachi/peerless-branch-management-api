
using Retail.Branch.Services.Common;
using Retail.Branch.Services.UsersModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.UsersModule
{
    public  interface IUserService
    {
        Task<PagedResponse<List<UserInfoModel>>> GetUsers(UserListFilter filter, string token);
        Task<UserInfoModel> GetUser(Guid userid, string token);

    }
}
