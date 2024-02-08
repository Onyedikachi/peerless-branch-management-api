using System.ComponentModel.DataAnnotations;

namespace Retail.Branch.Services.BranchModule.Models
{

    public class CreateSingleBranch
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? LocationRef { get; set; }
        public bool Draft { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string StreetName { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string? Lga { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
    }
}
