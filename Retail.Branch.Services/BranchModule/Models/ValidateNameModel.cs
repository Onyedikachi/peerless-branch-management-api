using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
#nullable disable
    public class ValidateNameModel
    {
        [Required]
        public string Name { get; set; }
    }
}
