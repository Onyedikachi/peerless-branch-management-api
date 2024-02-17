using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Infrastructure;
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
    public class AuditLogService : IAuditLogService
    {
        private readonly BranchDataContext _Db;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(BranchDataContext db, ILogger<AuditLogService> logger)
        {
            _Db = db;
            _logger = logger;
        }


        public Task<PagedResponse<AuditLogModel>> GetAuditLog(string user_id)
        {
            var records = _Db.AuditLogs.AsNoTracking().Where(c => c.User_Id == user_id).AsQueryable();
            var count = records.Count();

            var pagedData = records.GetPaginatedReponseAsync(1, 10);

            var result = (from m in pagedData
                          select new AuditLogModel()
                          {
                              Created_At = m.Created_At,
                              LoginTime = m.LoginTime,
                              User_Id = m.User_Id,
                              User_Name = m.User_Name,
                              Clicks = m.Clicks,
                              status = m.status,
                              BranchCode = m.BranchCode,
                              SourceIPs = m.SourceIPs,
                          });
            return Task.FromResult(new PagedResponse<AuditLogModel>("Success", pagedData, count, 1, 10));


        }

        public async Task<PagedResponse<AuditLogModel>> GetAuditLogs(AuditLogFilters filters)
        {
            var records = _Db.AuditLogs.OrderByDescending(c => c.Created_At).ThenByDescending(c => c.LoginTime).AsQueryable();

            var count = await records.CountAsync();

            //if(!string.IsNullOrEmpty(filters.Filter_by))
            //{
            //    switch (filters.Filter_by)
            //    {
            //        case: $"branch_name";
            //            records = records.Where(c => c.BranchName = )
            //    }
            //}
            var pagedData = records.GetPaginatedReponseAsync(filters.Page, filters.Page_Size);
            var result = (from m in pagedData
                          select new AuditLogModel()
                          {
                              Created_At = m.Created_At,
                              LoginTime = m.LoginTime,
                              User_Id = m.User_Id,
                              User_Name = m.User_Name,
                              Clicks = m.Clicks,
                              status = m.status,
                              BranchCode = m.BranchCode,
                              SourceIPs = m.SourceIPs,
                              Action_Type = m.Action_Type,
                          });

            return new PagedResponse<AuditLogModel>("Success", result, count, filters.Page, filters.Page_Size);
        }

        public async Task<ApiResponse<object>> CreateAuditLog(AuditLogModel auditLog)
        {
            var log = new AuditLogs();

            if (auditLog != null)
            {
                log.SourceIPs = auditLog.SourceIPs;
                log.BranchCode = auditLog.BranchCode;
                log.User_Id = Guid.NewGuid().ToString();
                log.User_Name = auditLog.User_Name;
                log.LoginTime = auditLog.LoginTime;
                log.Clicks = auditLog.Clicks;
                log.Created_At = auditLog.Created_At;
                log.Action_Type = auditLog.Action_Type;


                _Db.AuditLogs.Add(log);
                await _Db.SaveChangesAsync();


            }
            return new SuccessApiResponse<AuditLogs>("Branch Created", log);

        }
    }
}
