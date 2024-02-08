namespace Retail.Branch.Services.BranchRequestModule.Models
{

    public class EditRequestModel
    {
        public string name { get; set; }
        public string description { get; set; }
        public bool draft { get; set; }
        public string code { get; set; }
        public string type { get; set; }
        public Guid id { get; set; }

        public string? number { get; set; }
        public string? streetname { get; set; }
        public string? city { get; set; }
        public string? State { get; set; }
        public string? lga { get; set; }
        public string? postalCode { get; set; }
        public string? country { get; set; }
    }

}
