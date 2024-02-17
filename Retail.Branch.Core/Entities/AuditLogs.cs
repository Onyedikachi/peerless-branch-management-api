using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Entities
{
    public class AuditLogs
    {
        [Key]
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public string Action_Type { get; set; }
        public int Clicks { get; set; }
        public string status { get; set; } = "false";
        public string SourceIPs { get; set; }
        public string BranchCode { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
    }
}
