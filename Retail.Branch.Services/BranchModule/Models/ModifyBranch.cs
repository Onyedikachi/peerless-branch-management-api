using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class ModifyBranch
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Draft { get; set; }
        public string? Number { get; set; }
        public string? StreetName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Lga { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
