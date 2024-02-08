using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.LedgerModule.Model
{
    public class LedgerListFilter
    {
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int Page_Size { get; set; } = 10;
        public string? state { get; set; } = "OPEN";
    }
}
