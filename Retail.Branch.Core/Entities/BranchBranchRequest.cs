using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Entities
{
    public class BranchBranchRequest
    {
        public Guid BranchRequestsId { get; set; }
        public Guid BranchesId { get; set; }

        public Branch Branches { get; set; } = null!;
        public BranchRequest BranchRequest { get; set; } = null!;
        public string? Meta { get; set; }
        public string? Document { get; set; }
    }
}
