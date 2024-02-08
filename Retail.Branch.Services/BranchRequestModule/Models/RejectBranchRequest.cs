using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchRequestModule.Models
{
    public class RejectBranchRequest
    {
        public RejectBranchRequest(string reason)
        {
            Reason = reason;
        }
        public string Reason { get; set; }
        public BasicUserInfo? RouteTo { get; set; }
    }

    public class BasicUserInfo
    {
        public string? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}
