using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Services.BranchRequestModule.Models;

namespace Retail.Branch.Services.BranchModule.Models
{
    public class BranchModel
    {
        public Guid Id { get; set; }
        public string? Created_By { get; set; }
        public string? Created_By_Id { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public DateTime Updated_At { get; set; }

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
        public string? Number { get; set; }
        public string? StreetName { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Lga { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }

        public ICollection<BranchRequestModel> BranchRequests { get; set; } = new List<BranchRequestModel>();

        public static implicit operator BranchModel(Retail.Branch.Core.Entities.Branch v)
        {
            BranchModel model = new();
            model.Id = v.Id;
            model.Created_By = v.Created_By;
            model.Created_By_Id = v.Created_By_Id;
            model.Created_At = v.Created_At;
            model.Created_By_BranchId = v.Created_By_BranchId;
            model.Updated_At = v.Updated_At;
            model.Treated_By_Branch_Id = v.Treated_By_Branch_Id;
            model.Treated_By_Id = v.Treated_By_Id;
            model.Treated_By = v.Treated_By;
            model.Name = v.Name;
            model.Code = v.Code;
            model.Description = v.Description;
            model.Status = v.Status;
            model.Updated_At = v.Updated_At;
            model.Number = v.Number;
            model.StreetName= v.StreetName;
            model.City = v.City;
            model.State= v.State;
            model.Lga= v.Lga;   
            model.PostalCode= v.PostalCode;
            model.Country = v.Country;
            return model;
        }
    }

    public class BranchRequestModel
    {
        public Guid Id { get; set; }
        public string? Created_By { get; set; }
        public string? Created_By_Id { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public DateTime Updated_At { get; set; }

        public string Description { get; set; } = string.Empty;
        public string? Treated_By { get; set; }
        public string? Treated_By_Id { get; set; }
        public string? Treated_By_Branch_Id { get; set; }
        public Guid? CreatedByBranchId { get; set; }
        public string Request_Type { get; set; }
        public string Status { get; set; }
        public string? Reason { get; set; }

        public string? Alt_Name { get; set; }
        public string? Alt_Description { get; set; }
        public object? Meta { get; set; } 

        public ICollection<BranchModel> Branches { get; set; } = new List<BranchModel>();


        public static implicit operator BranchRequestModel(BranchRequest v)
        {
            BranchRequestModel model = new();
            model.Id = v.Id;
            model.CreatedByBranchId = v.CreatedByBranchId;
            model.Treated_By_Branch_Id = v.Treated_By_Branch_Id;
            model.Treated_By_Id = v.Treated_By_Id;
            model.Created_By_Id = v.Created_By_Id;
            model.Created_By = v.Created_By;
            model.Treated_By = v.Treated_By;
            model.Alt_Description = v.Alt_Description;
            model.Alt_Name = v.Alt_Name;
            model.Reason = v.Reason;
            model.Request_Type = v.Request_Type;
            model.Status = v.Status;
            model.Updated_At = v.Updated_At;
            model.Meta = v.Meta;
            model.Description = v.Description;

            switch (v.Request_Type)
            {
                case AppContants.CHANGE_REQUEST_TYPE: 
                    model.Meta = JsonConvert.DeserializeObject<EditRequestModel?>(v.Meta) ;
                    break;
                case AppContants.DEACTIVATE_REQUEST_TYPE:
                    model.Meta = JsonConvert.DeserializeObject<DeactivationRequestModel?>(v.Meta);
                        break;
                case AppContants.CREATE_REQUEST_TYPE:
                    if(v.Meta != null)
                    {
                        model.Meta = JsonConvert.DeserializeObject<EditRequestModel?>(v.Meta);
                    }
                    
                    break;
                default:
                    break;
            }
            model.Branches = v.Branches.Select(c => (BranchModel)c).ToList();
            return model;
        }
    }
}
