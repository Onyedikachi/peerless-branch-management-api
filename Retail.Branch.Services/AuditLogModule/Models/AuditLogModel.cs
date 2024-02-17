using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.AuditLogModule.Models
{
    public class AuditLogModel
    {
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public string Action_Type { get; set; }
        public int Clicks { get; set; }
        public string status { get; set; }
        public string SourceIPs { get; set; }
        public string BranchCode { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
    }
}
