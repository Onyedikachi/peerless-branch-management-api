using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchRequestModule.QueryFilter
{
    public class BranchRequestFilter
    {
        public string? Q { get; set; }
        public string? Filter_By { get; set; }
        public string[]? Status__In { get; set; }

        public string[]? Request_Type__In { get; set; }

        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string[]? Initiator { get; set; }
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1, int.MaxValue)]
        public int Page_Size { get; set; } = 10;
    }
}
