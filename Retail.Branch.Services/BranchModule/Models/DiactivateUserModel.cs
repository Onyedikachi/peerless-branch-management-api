using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class DiactivateUserModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string? ReassignBranchId { get; set; }
        public DeactivateUserAction DictivateAction { get; set; }
    }

    public  enum DeactivateUserAction
    {
        Deactivate = 0,
        Reassign =1,
       
    }
}
