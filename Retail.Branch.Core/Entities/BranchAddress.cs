using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Entities
{
    public class BranchAddress:BaseEntity
    {
        public string? Number { get; set; }
        public string? StreetName { get; set; }
        public string? City { get; set; }
        public string?   State  { get; set; }
        public string? Lga { get; set; }
        public string? PostalCode { get; set; }
    }
}
