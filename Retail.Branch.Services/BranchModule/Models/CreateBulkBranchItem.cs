using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class CreateBulkBranchItem
    {
        [Column(1)]
        public string Name { get; set; }
        [Column(3)]
        public string? Number { get; set; }
        [Column(4)]
        public string? Street { get; set; }
        [Column(5)]
        public string? City { get; set; }
        [Column(6)]
        public string? State { get; set; }
        [Column(7)]
        public string? Lga { get; set; }
        [Column(8)]
        public string? Zip { get; set; }
        [Column(9)]
        public string? Country { get; set; }
        [Column(10)]
        public string? Description { get; set; }
       

    }

    public class SaveBulkRequest
    {
        public bool Draft { get; set; }
        public List<CreateBulkBranchItemResponse> Items { get; set; } = new();
    }

    public class CreateBulkResponse
    {
        public List<CreateBulkBranchItemResponse> Items { get; set; } = new();
        public int Failed { get; set; }
        public int Success { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }
        public List<string> Errors { get; set; } = new();

    }
    public class CreateBulkBranchItemResponse
    {       
        public string Name { get; set; }       
        public string? Description { get; set; }
        public string Number { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string? State { get; set; }
        public string? Lga { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public bool Status { get; set; }
        public string? Status_Description { get; set; }

    }
}
