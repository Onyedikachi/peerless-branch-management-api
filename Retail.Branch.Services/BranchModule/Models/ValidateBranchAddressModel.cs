using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class ValidateBranchAddressModel
    {
        public ValidateBranchAddressModel(string number, string street, string city)
        {
            this.Number = number;
            this.Street = street;
            this.City = city;
        }
        [Required]
        public string Number { get; set; }
        [Required]
        public string Street { get; set; }
        public string City { get; set; }
    }
}
