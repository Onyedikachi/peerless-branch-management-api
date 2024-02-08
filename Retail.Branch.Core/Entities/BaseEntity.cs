using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string? Created_By { get; set; }
        public string? Created_By_Id { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public DateTime Updated_At { get; set; }

        public bool Deleted { get; set; } = false;
        public string? Deleted_By_Id { get; set; }
        public DateTime? Deleted_At { get; set; }
    }
}
