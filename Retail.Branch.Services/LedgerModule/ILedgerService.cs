using Retail.Branch.Services.Common;
using Retail.Branch.Services.LedgerModule.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.LedgerModule
{
    public interface ILedgerService
    {
        Task<PagedResponse<List<LedgerResult>>> GetAll(LedgerListFilter filter, string token);

    }
}
