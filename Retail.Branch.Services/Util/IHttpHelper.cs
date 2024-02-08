using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.Util
{
    public interface IHttpHelper
    {
        Task<(bool status, string message, T? data)> PostData<T>(object model, string url, string token);
        Task<T?> GetData<T>(string url, string token);
    }
}
