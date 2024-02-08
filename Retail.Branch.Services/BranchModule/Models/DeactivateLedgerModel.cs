using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class DeactivateLedgerModel
    {
        public string LegerId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Currency { get; set; }
        public float NetBalance { get; set; }
        public string DestinationLedgerId { get; set; }
        public string DestinationLedgerName { get; set; }

    }
}
