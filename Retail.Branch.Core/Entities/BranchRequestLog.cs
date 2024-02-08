namespace Retail.Branch.Core.Entities
{
#nullable disable
    public class BranchRequestLog : BaseEntity
    {
        public string Description { get; set; } = string.Empty;
        public Guid? BranchRequestId { get; set; }
        public  BranchRequest BranchRequest { get; set; }
        public Guid? BranchId { get; set; }
    }
}
