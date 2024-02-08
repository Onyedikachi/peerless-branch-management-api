using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Entities
{
    public class BranchMember : BaseEntity
    {
        public string User_Id { get; set; }
        public string User_Name { get; set; }
        public Branch Branch { get; set; }
        public Guid BranchId { get; set; }
    }
}
