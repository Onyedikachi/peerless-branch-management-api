using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class DeactivationRequestModel
    {
        public List<DeactivateLedgerModel> Ledgers { get; set; } = new();
        public List<DiactivateUserModel> Users { get; set; } = new();
        public string? Reason { get; set; }
        public bool Draft { get; set; }
        public Guid BranchId { get; set; }
        public string? Url { get; set; }
   
    }

}
