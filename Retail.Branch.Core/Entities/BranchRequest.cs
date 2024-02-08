using Retail.Branch.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Entities
{

    public class BranchRequest : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public string? Treated_By { get; set; }
        public string? Treated_By_Id { get; set; }
        public string? Treated_By_Branch_Id { get; set; }
        public Guid? CreatedByBranchId { get; set; }
        public string Request_Type { get; set; }
        public string Status { get; set; }
        public string? Reason { get; set; }
        public string? Meta { get; set; } = null;
        public string? Alt_Name { get; set; }
        public string? Alt_Description { get; set; }
       //  [JsonIgnore]
        public ICollection<Branch> Branches { get; set; }= new List<Branch>();

    }
}
