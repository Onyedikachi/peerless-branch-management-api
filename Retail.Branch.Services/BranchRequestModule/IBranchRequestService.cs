using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchRequestModule.Models;
using Retail.Branch.Services.BranchRequestModule.QueryFilter;
using Retail.Branch.Services.Common;

namespace Retail.Branch.Services.BranchRequestModule
{
    public interface IBranchRequestService
    {
        ApiResponse<object> GetAnalytics(string? filter_By, BranchUser user);
        Task<PagedResponse<BranchRequestModel>> GetBranchRequests(BranchRequestFilter filter, BranchUser user);
        Task<ApiResponse<object>> GetDetails(Guid Id);
        Task<ApiResponse<object>> ApproveRequest(Guid Id, BranchUser user);
        Task<ApiResponse<object>> RejectRequest(Guid Id, BranchUser user, RejectBranchRequest model);

        PagedResponse<BranchRequestLog> GetBranchRequestLogs(Guid request_id);
        Task<ApiResponse<object>> DeleteRequest(Guid Id, BranchUser user);

        Task<ApiResponse<object>> UpdateRequest(EditRequestModel model, BranchUser user);

        Task<ApiResponse<object>> SaveDiactivationRequest(DeactivationRequestModel model, BranchUser user);
        Task<ApiResponse<object>> UpdateBulkReques(Guid requestId, SaveBulkRequest model, BranchUser user);
    }
}