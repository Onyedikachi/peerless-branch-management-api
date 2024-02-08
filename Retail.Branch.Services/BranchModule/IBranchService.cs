using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchModule.QueryFilters;
using Retail.Branch.Services.Common;

namespace Retail.Branch.Services.BranchModule
{
    public interface IBranchService
    {
        Task<ApiResponse<object>> CreateBranch(CreateSingleBranch model, BranchUser user);
     
        Task<ApiResponse<object>> VerifyName(ValidateNameModel model);
        Task<ApiResponse<object>> VerifyAddress(ValidateBranchAddressModel model);

        Task<PagedResponse<BranchModel>> GetBranches(GetBranchFilter filter,string? userId, Guid? branchId);


        Task<ApiResponse<object>> GetBranchByCode(string code);

        ApiResponse<object> BranchAnalytics(string? filterBy, BranchUser user);

        Task<ApiResponse<object>> Update(Guid id, ModifyBranch model, BranchUser user);

        Task<ApiResponse<object>> BulkUpload(List<CreateBulkBranchItem> model);

        Task<ApiResponse<object>> CreateBulkReques(SaveBulkRequest model, BranchUser user);
        Task<ApiResponse<object>> ActivateBranch(Guid Id, BranchUser user);
        PagedResponse<BranchRequestLog> GetBranchRequestLogs(Guid request_id);
    }
}