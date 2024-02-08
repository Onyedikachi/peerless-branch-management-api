using Retail.Branch.Core.Common;
using System.Text.Json.Serialization;

namespace Retail.Branch.Core.Entities
{
    public class Branch : BaseEntity
    {
        public Branch(string name, string code)
        {
            Name = name;
            Code = code;
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; }
        public char Status { get; set; } = AppContants.PENDING_BRANCH_STATUS;
        public string? LocationRef { get; set; }
        public bool IsLocked { get; set; } = false;
        public Guid? Created_By_BranchId { get; set; }
        public string? Treated_By { get; set; }
        public string? Treated_By_Id { get; set; }
        public string? Treated_By_Branch_Id { get; set; }
        [JsonIgnore]
        public ICollection<BranchRequest> BranchRequests { get; set; } = new List<BranchRequest>();

        public string Number { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }="";
        public string? State { get; set; }
        public string? Lga { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
}
