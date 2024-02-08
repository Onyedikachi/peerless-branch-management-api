using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.QueryFilters
{
    public class GetBranchFilter
    {
        public string? Filter_by { get; set; }
        public char[]? Status__In { get; set; }
        public string? Q { get; set; }
        public DateTime? Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1, int.MaxValue)]
        public int Page_Size { get; set; } = 10;
    }

   
}
