using Retail.Branch.Core.Common;
using Retail.Branch.Services.AuditLogModule.Models;
using Retail.Branch.Services.AuditLogModule.QueryFilters;
using Retail.Branch.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.AuditLogModule
{
    public interface IAuditLogService
    {
        Task<PagedResponse<AuditLogModel>> GetAuditLog(string user_id);
        Task<PagedResponse<AuditLogModel>> GetAuditLogs(AuditLogFilters filter);

        Task<ApiResponse<object>> CreateAuditLog(AuditLogModel auditLog);
    }
}
