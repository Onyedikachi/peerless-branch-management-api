using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Infrastructure;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchRequestModule.Models;
using Retail.Branch.Services.BranchRequestModule.QueryFilter;
using Retail.Branch.Services.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Retail.Branch.Services.BranchRequestModule
{
    public class BranchRequestService : IBranchRequestService
    {
        private readonly BranchDataContext _Db;
        private readonly ILogger<BranchRequestService> _Logger;

        public BranchRequestService(BranchDataContext db, ILogger<BranchRequestService> logger)
        {
            _Db = db;
            _Logger = logger;
            _Db.ChangeTracker.AutoDetectChangesEnabled = false;
            _Db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public ApiResponse<object> GetAnalytics(string? filter_By, BranchUser user)
        {
            var recordsquery = _Db.BranchRequests.Where(c => c.Deleted == false).AsQueryable();

            if (!string.IsNullOrEmpty(filter_By))
            {
                switch (filter_By)
                {
                    case "created_by_anyone":
                        break;

                    case "created_by_me":
                        recordsquery = recordsquery.Where(c => c.Created_By_Id == user.UserId);
                        break;
                    case $"created_by_my_branch":
                        recordsquery = recordsquery.Where(c => c.CreatedByBranchId == user.BranchId);
                        break;
                    case "sent_to_me":
                        recordsquery = recordsquery.Where(c => c.Treated_By_Id == user.UserId);
                        break;

                    case "sent_to_my_branch":
                        recordsquery = recordsquery.Where(c => c.CreatedByBranchId == user.BranchId);
                        break;
                    default:
                        break;

                }
            }

            var allrecords = recordsquery.ToList();

            var anaytics = new Dictionary<string, int>();
            anaytics.Add("All", allrecords.Count());
            anaytics.Add("P", allrecords.Count(c => c.Status == AppContants.PENDING_REQUEST_STATUS.ToString()));
            anaytics.Add("D", allrecords.Count(c => c.Status == AppContants.DRAFT_REQUEST_STATUS.ToString()));
            anaytics.Add("A", allrecords.Count(c => c.Status == AppContants.APPROVED_REQUEST_STATUS.ToString()));
            anaytics.Add("R", allrecords.Count(c => c.Status == AppContants.REJECTED_REQUEST_STATUS.ToString()));

            return new SuccessApiResponse<object>("success", anaytics);
        }


        public async Task<PagedResponse<BranchRequestModel>> GetBranchRequests(BranchRequestFilter filter, BranchUser user)
        {
            var requestrecords = _Db.BranchRequests.AsNoTracking().AsQueryable().Include(c => c.Branches).AsQueryable();
            requestrecords = requestrecords.OrderByDescending(c => c.Updated_At).Where(c => c.Deleted == false);


            if (!string.IsNullOrEmpty(filter.Q))
            {
                requestrecords = requestrecords.Where(
                            c => c.Description.ToLower().Contains(filter.Q.ToLower())
                            || c.Created_By!.Contains(filter.Q.ToLower())
                            ).AsQueryable();
            }

            if (!string.IsNullOrEmpty(filter.Filter_By))
            {
                switch (filter.Filter_By)
                {
                    case $"created_system_wide":

                        break;

                    case $"created_by_me":
                        requestrecords = requestrecords.Where(c => c.Created_By_Id == user.UserId);
                        break;

                    case $"created_by_my_branch":
                        requestrecords = requestrecords.Where(c => c.CreatedByBranchId == user.BranchId);
                        break;

                    case $"sent_to_me":
                        requestrecords = requestrecords.Where(c => c.Treated_By_Id == user.UserId);
                        break;

                    case $"sent_to_my_branch":
                        requestrecords = requestrecords.Where(c => c.CreatedByBranchId == user.BranchId);
                        break;

                    case $"sent_system_wide":

                        break;
                }
            }
            if (filter.Initiator is not null)
            {
                requestrecords = requestrecords.Where(c => filter.Initiator.Contains(c.Created_By_Id));
            }

            if (filter.Status__In is not null)
            {
                requestrecords = requestrecords.Where(c => filter.Status__In.Contains(c.Status));
            }

            if (filter.Request_Type__In is not null)
            {
                requestrecords = requestrecords.Where(c => filter.Request_Type__In.Contains(c.Request_Type));
            }

            if (filter.Start_Date is not null)
            {
                var from = new DateTime(filter.Start_Date.Value.Year, filter.Start_Date.Value.Month, filter.Start_Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);
                requestrecords = requestrecords.Where(c => c.Updated_At >= from);
            }

            if (filter.End_Date is not null)
            {
                var to = new DateTime(filter.End_Date.Value.Year, filter.End_Date.Value.Month, filter.End_Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);

                requestrecords = requestrecords.Where(c => c.Updated_At <= to);
            }
            var count = requestrecords.Count();

            var pagedData = requestrecords.GetPaginatedReponseAsync(filter.Page, filter.Page_Size);

            var result = pagedData.Select(c => (BranchRequestModel)c).ToList();

            return new PagedResponse<BranchRequestModel>("Success", result, count, filter.Page, filter.Page_Size);
        }


        public async Task<ApiResponse<object>> GetDetails(Guid Id)
        {
            var record = await _Db.BranchRequests.Include(c => c.Branches).FirstOrDefaultAsync(c => c.Id == Id);

            if (record is null)
            {
                throw new ValidationException("Branch not found");
            }
            var result = (BranchRequestModel)record;

            switch (result.Request_Type)
            {
                case AppContants.CHANGE_REQUEST_TYPE:
                    result.Branches = record.Branches.Select(c => (BranchModel)c).ToList();
                    break;
                case AppContants.DEACTIVATE_REQUEST_TYPE:

                    var MetaInfo = JsonConvert.DeserializeObject<DeactivationRequestModel?>(record?.Meta);

                    result.Branches = _Db.Branches.Where(c => c.Id == MetaInfo.BranchId).Select(c => (BranchModel)c).ToList();
                    break;
                case AppContants.CREATE_REQUEST_TYPE:
                    if (result.Meta is null)
                    {
                        result.Branches = record.Branches.Select(c => (BranchModel)c).ToList();
                    }
                    break;


                default:
                    break;
            }


            return new SuccessApiResponse<BranchRequestModel>("Branch Record", result);
        }

        public async Task<ApiResponse<object>> ApproveRequest(Guid Id, BranchUser user)
        {

            var record = _Db.BranchRequests.AsNoTracking().FirstOrDefault(c => c.Id == Id);

            if (record is null)
            {
                throw new ValidationException("Branch not found");
            }

            record.Status = AppContants.APPROVED_REQUEST_STATUS.ToString();
            record.Treated_By_Id = user.UserId;
            record.Treated_By = user.FullName;
            record.Treated_By_Branch_Id = user.BranchId.ToString();
            record.Reason = "";
            record.Updated_At = DateTime.UtcNow;

            switch (record.Request_Type)
            {
                case AppContants.CREATE_REQUEST_TYPE:
                    await ApproveCreateBranch(user, record);
                    break;
                case AppContants.CHANGE_REQUEST_TYPE:
                    await ApproveModifyBranch(user, record);
                    break;


                case AppContants.BULK_CREATE_REQUEST_TYPE:
                    await ApproveBulkCreateBranch(user, record.Id);
                    break;

                case AppContants.DEACTIVATE_REQUEST_TYPE:
                    await ApproveDeactivate(user, record);
                    break;

                default:
                    break;
            }


            return new SuccessApiResponse<bool>($"Branch {record.Request_Type} approved", true);
        }

        private async Task ApproveCreateBranch(BranchUser user, BranchRequest record)
        {

            var jointable = _Db.BranchBranchRequest.AsNoTracking().FirstOrDefault(c => c.BranchRequestsId == record.Id);
            if (jointable is null)
            {
                throw new ValidationException("No joint record found between banch and request");
            }
            var firstbranch = _Db.Branches.AsNoTracking().FirstOrDefault(c => c.Id == jointable.BranchesId);

            if (firstbranch is null)
            {
                throw new ValidationException("No branch associated with this request");
            }


            firstbranch.Status = AppContants.APPROVED_BRANCH_STATUS;
            firstbranch.Treated_By_Id = user.UserId;
            firstbranch.Treated_By = user.FullName;
            firstbranch.Treated_By_Branch_Id = user.BranchId.ToString();

            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Branch creation approved for {firstbranch.Name} by {user.FullName}";
            log.BranchRequestId = record.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            log.BranchId = firstbranch.Id;
            _Db.BranchRequestLogs.Add(log);
            _Db.Branches.Update(firstbranch);
            _Db.BranchRequests.Update(record);

            try
            {
                _Db.SaveChanges();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error saving Approve create branch", ex);
                throw;
            }
        }

        private async Task ApproveBulkCreateBranch(BranchUser user, Guid Id)
        {
            _Db.ChangeTracker.Clear();

            List<Retail.Branch.Core.Entities.Branch> toupdate = new();

            var jointable = _Db.BranchBranchRequest.Where(c => c.BranchRequestsId == Id).ToList();
            if (jointable.Count() == 0)
            {
                throw new ValidationException("No joint record found between banch and request");
            }
            foreach (var branch in jointable)
            {
                var firstbranch = _Db.Branches.FirstOrDefault(c => c.Id == branch.BranchesId);

                if (firstbranch is null)
                {
                    throw new ValidationException("No branch associated with this request");
                }

                firstbranch.Status = AppContants.APPROVED_BRANCH_STATUS;
                firstbranch.Treated_By_Id = user.UserId;
                firstbranch.Treated_By = user.FullName;
                firstbranch.Treated_By_Branch_Id = user.BranchId.ToString();
                toupdate.Add(firstbranch);
            }

            var request = _Db.BranchRequests.Find(Id);

            request.Status = AppContants.APPROVED_REQUEST_STATUS.ToString();

            _Db.BranchRequests.Update(request);
            await _Db.SaveChangesAsync();
            _Db.Branches.UpdateRange(toupdate);

            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Branch creation approved for all";
            log.BranchRequestId = Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;

            _Db.BranchRequestLogs.Add(log);


            try
            {
                await _Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Db error occured while saving Approve Bulk Create Action", ex);
                throw;
            }

        }
        private async Task ApproveModifyBranch(BranchUser user, BranchRequest record)
        {
            _Db.ChangeTracker.AutoDetectChangesEnabled = false;
            _Db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            record.Reason = "";
            record.Updated_At = DateTime.UtcNow;


            var jointable = _Db.BranchBranchRequest.FirstOrDefault(c => c.BranchRequestsId == record.Id);
            var firstbranch = _Db.Branches.FirstOrDefault(c => c.Id == jointable.BranchesId);

            if (firstbranch is null)
            {
                throw new ValidationException("No branch associated with this request");
            }

            firstbranch.Status = AppContants.APPROVED_BRANCH_STATUS;
            firstbranch.Treated_By_Id = user.UserId;
            firstbranch.Treated_By = user.FullName;
            firstbranch.Treated_By_Branch_Id = user.BranchId.ToString();
            firstbranch.Name = record.Alt_Name ?? "";

            var branchUpdate = JsonConvert.DeserializeObject<EditRequestModel>(record.Meta);
            if (branchUpdate != null)
            {
                firstbranch.Name = branchUpdate.name;
                firstbranch.Description = branchUpdate.description;
                firstbranch.Number = branchUpdate.number;
                firstbranch.StreetName = branchUpdate.streetname;
                firstbranch.State = branchUpdate.State;
                firstbranch.City = branchUpdate.city;
                firstbranch.Country = branchUpdate.country;

            }

            _Db.Branches.Update(firstbranch);
            _Db.BranchRequests.Update(record);

            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Branch modification approved for {firstbranch.Name} by {user.FullName}";
            log.BranchRequestId = record.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            log.BranchId = firstbranch.Id;

            _Db.BranchRequestLogs.Add(log);
            await _Db.SaveChangesAsync();
        }


        private async Task ApproveDeactivate(BranchUser user, BranchRequest record)
        {

            var MetaInfo = JsonConvert.DeserializeObject<DeactivationRequestModel?>(record.Meta);

            var branch = _Db.Branches.AsNoTracking().FirstOrDefault(c => c.Id == MetaInfo.BranchId);

            if (branch is null)
            {
                throw new ValidationException("No branch associated with this request");
            }

            branch.Status = AppContants.INACTIVE_BRANCH_STATUS;
            branch.Treated_By_Id = user.UserId;
            branch.Treated_By = user.FullName;
            branch.Treated_By_Branch_Id = user.BranchId.ToString();


            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Branch deactivation approved for {branch.Name} by {user.FullName}";
            log.BranchRequestId = record.Id;
            log.BranchId = branch.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            _Db.BranchRequestLogs.Add(log);

            _Db.Branches.Update(branch);
            _Db.BranchRequests.Update(record);

            await _Db.SaveChangesAsync();
        }

        private async Task ApproveDeactivateAdmin(BranchUser user, BranchRequest record)
        {

            var MetaInfo = JsonConvert.DeserializeObject<DeactivationRequestModel?>(record.Meta);

            var firstbranch = _Db.Branches.AsNoTracking().FirstOrDefault(c => c.Id == MetaInfo.BranchId);

            if (firstbranch is null)
            {
                throw new ValidationException("No branch associated with this request");
            }

            firstbranch.Status = AppContants.INACTIVE_BRANCH_STATUS;
            firstbranch.Treated_By_Id = user.UserId;
            firstbranch.Treated_By = user.FullName;
            firstbranch.Treated_By_Branch_Id = user.BranchId.ToString();


            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Branch deactivation approved for {firstbranch.Name} by {user.FullName}";
            log.BranchRequestId = record.Id;
            log.BranchId = firstbranch.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            _Db.BranchRequestLogs.Add(log);
            _Db.Branches.Update(firstbranch);

            await _Db.SaveChangesAsync();
        }

        public async Task<ApiResponse<object>> RejectRequest(Guid Id, BranchUser user, RejectBranchRequest model)
        {
            Guid? branchId = null;
            var record = await _Db.BranchRequests.AsNoTracking().FirstOrDefaultAsync(c => c.Id == Id);
            var rejectmsg = "";

            if (record is null)
            {
                throw new ValidationException("Branch not found");
            }

            record.Status = AppContants.REJECTED_REQUEST_STATUS.ToString();
            record.Treated_By_Id = user.UserId;
            record.Treated_By = user.FullName;
            record.Reason = model.Reason;
            record.Updated_At = DateTime.UtcNow;
            record.Treated_By_Branch_Id = user.BranchId.ToString();

            if (record.Request_Type == AppContants.CREATE_REQUEST_TYPE || record.Request_Type == AppContants.BULK_CREATE_REQUEST_TYPE)
            {
                var jointable = _Db.BranchBranchRequest.FirstOrDefault(c => c.BranchRequestsId == record.Id);
                var firstbranch = _Db.Branches.FirstOrDefault(c => c.Id == jointable.BranchesId);

                if (firstbranch is null)
                {
                    throw new ValidationException("No branch associated with this request");
                }
                rejectmsg = $"Branch creation rejected ";
                firstbranch.Status = AppContants.REJECTED_BRANCH_STATUS;
                firstbranch.Treated_By_Branch_Id = user.BranchId.ToString();
                _Db.Branches.Update(firstbranch);
                branchId = firstbranch.Id;

                if(model?.RouteTo?.UserId !=null)
                {
                    record.Treated_By_Id = model.RouteTo.UserId;
                    record.Treated_By = model.RouteTo.FullName ?? "";
                }
            }

            BranchRequestLog log = new BranchRequestLog();
            log.Description = rejectmsg;
            log.BranchRequestId = record.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            log.BranchId = branchId;
            _Db.BranchRequestLogs.Add(log);
            _Db.BranchRequests.Update(record);
            await _Db.SaveChangesAsync();

            return new SuccessApiResponse<bool>($"Branch {record.Request_Type.ToLowerInvariant()}  request Rejected", true);
        }

        public PagedResponse<BranchRequestLog> GetBranchRequestLogs(Guid request_id)
        {
            var records = _Db.BranchRequestLogs.Where(c => c.BranchRequestId == request_id).AsQueryable();

            var count = records.Count();

            var pagedData = records.GetPaginatedReponseAsync(1, 10);

            return new PagedResponse<BranchRequestLog>("Success", pagedData, count, 1, 10);
        }

        public async Task<ApiResponse<object>> DeleteRequest(Guid Id, BranchUser user)
        {
            var record = await _Db.BranchRequests.FirstOrDefaultAsync(c => c.Id == Id);

            if (record is null)
            {
                throw new ValidationException("Branch not found");
            }
            record.Deleted = true;
            record.Deleted_By_Id = user.UserId;
            record.Deleted_At = DateTime.UtcNow;
            record.Updated_At = DateTime.UtcNow;

            _Db.Update(record);
            await _Db.SaveChangesAsync();

            if (record.Request_Type == AppContants.BULK_CREATE_REQUEST_TYPE || record.Request_Type == AppContants.CREATE_REQUEST_TYPE)
            {


                var jointable = _Db.BranchBranchRequest.FirstOrDefault(c => c.BranchRequestsId == record.Id);
                if (jointable is null && record.Status == AppContants.DRAFT_REQUEST_STATUS.ToString())
                {
                    return new SuccessApiResponse<bool>("Request withdrawn and deleted successfully", true);
                }

                var firstbranch = _Db.Branches.FirstOrDefault(c => c.Id == jointable.BranchesId);

                if (firstbranch is null)
                {
                    throw new ValidationException("No branch associated with this request");
                }


                var branch = await _Db.Branches.FirstOrDefaultAsync(c => c.Id == firstbranch.Id);
                if (branch is null)
                {
                    throw new ValidationException("No branch associated with this request");

                }
                branch.Deleted_By_Id = user.BranchId.ToString();
                branch.Deleted_At = DateTime.UtcNow;

                _Db.Branches.Update(branch);
            }

            await _Db.SaveChangesAsync();

            return new SuccessApiResponse<bool>("Request withdrawn and deleted successfully", true);
        }

        public async Task<ApiResponse<object>> UpdateRequest(EditRequestModel model, BranchUser user)
        {
            var record = await _Db.BranchRequests.FirstOrDefaultAsync(c => c.Id == model.id);

            if (record is null)
            {
                throw new ValidationException("Branch not found");
            }

            if (record.Status == AppContants.APPROVED_REQUEST_STATUS.ToString())
            {
                throw new ValidationException("This request has already been approved");
            }

            record.Updated_At = DateTime.UtcNow;
            record.Meta = JsonConvert.SerializeObject(model);
            record.Description = $"Creation of {model.name}";

            if (model.draft == true)
            {
                record.Status = AppContants.DRAFT_REQUEST_STATUS.ToString();
            }
            else
            {
                record.Status = AppContants.PENDING_REQUEST_STATUS.ToString();
            }

            if (record.Request_Type == AppContants.CREATE_REQUEST_TYPE)
            {
                var jointable = _Db.BranchBranchRequest.FirstOrDefault(c => c.BranchRequestsId == record.Id);
                var firstbranch = _Db.Branches.FirstOrDefault(c => c.Id == jointable.BranchesId);

                if (firstbranch is null)
                {
                    throw new ValidationException("No branch associated with this request");
                }

                firstbranch.Name = model.name;
                firstbranch.Description = model.description;
                firstbranch.Status = char.Parse(record.Status);
                firstbranch.State = model.State;
                firstbranch.Number = model.number;
                firstbranch.City = model.city;
                firstbranch.Country = model.country;
                firstbranch.Lga = model.lga;
                firstbranch.PostalCode = model.postalCode;

                _Db.Branches.Update(firstbranch);

                BranchRequestLog log = new BranchRequestLog();
                log.Description = $"Branch modification request submitted by {user.FullName}";
                log.BranchRequestId = record.Id;
                log.Created_By_Id = user.UserId;
                log.Created_By = user.FullName;
                log.BranchId = firstbranch.Id;
                _Db.BranchRequestLogs.Add(log);
            }

            try
            {
                _Db.BranchRequests.Update(record);
                await _Db.SaveChangesAsync();

                if (user.Is_Super.ToLower() == "true" && !model.draft)
                {
                    await ApproveRequest(record.Id, user);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("DB save error", ex);
            }

            return new SuccessApiResponse<bool>("Successful ", true);
        }


        public async Task<ApiResponse<object>> SaveDiactivationRequest(DeactivationRequestModel model, BranchUser user)
        {
            char status = AppContants.DRAFT_BRANCH_STATUS;
            if (!model.Draft)
                status = AppContants.PENDING_BRANCH_STATUS;

            var branch = await _Db.Branches.FirstOrDefaultAsync(c => c.Id == model.BranchId);
            if (branch is null)
                throw new ValidationException("Branch with not found");

            if (branch.IsLocked)
                throw new ValidationException($"You cannot deactivate default branch, {branch.Name}");

            if (branch.IsLocked)
                throw new ValidationException($"{branch.Name} default branch cannot be deleted");

            BranchRequest branchRequest = new BranchRequest();
            branchRequest.Request_Type = AppContants.DEACTIVATE_REQUEST_TYPE;
            branchRequest.Status = status.ToString();
            branchRequest.Created_By = user.FullName;
            branchRequest.Created_By_Id = user.UserId;
            branchRequest.Description = $"Deactivation of {branch.Name}";
            branchRequest.Updated_At = DateTime.UtcNow;
            branchRequest.CreatedByBranchId = user.BranchId;
            branchRequest.Meta = JsonConvert.SerializeObject(model);
          
            if (user.Is_Super.ToLower() == "true")
            {
                branchRequest.Status = AppContants.APPROVED_REQUEST_STATUS.ToString();
            }

            _Db.BranchRequests.Add(branchRequest);
            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Deactivation request submited by {user.FullName}";
            log.BranchRequestId = branchRequest.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            log.BranchId = model.BranchId;
            _Db.BranchRequestLogs.Add(log);

            await _Db.SaveChangesAsync();

            if (user.Is_Super.ToLower() == "true")
            {
                await ApproveDeactivateAdmin(user, branchRequest);
            }

            return new SuccessApiResponse<bool>("Deactivation request has been submited", true); ;
        }


        public async Task<ApiResponse<object>> UpdateBulkReques(Guid requestId, SaveBulkRequest model, BranchUser user)
        {
            List<Core.Entities.Branch> toAdd = new List<Core.Entities.Branch>();

            char status = AppContants.DRAFT_BRANCH_STATUS;
            if (!model.Draft)
                status = AppContants.PENDING_BRANCH_STATUS;


            var branchRequest = _Db.BranchRequests.FirstOrDefault(x => x.Id == requestId);

            if (branchRequest == null)
            {
                throw new ValidationException("Request not found");
            }

            branchRequest.Created_By = user.FullName;
            branchRequest.Created_By_Id = user.UserId;
            branchRequest.Description = $"Creation of bulk request {model.Items?.FirstOrDefault()?.Name} {model.Items?.LastOrDefault()?.Name}";
            branchRequest.Updated_At = DateTime.UtcNow;
            branchRequest.CreatedByBranchId = user.BranchId;

            if (branchRequest.Status == AppContants.PENDING_REQUEST_STATUS.ToString() && model.Draft)
            {
                branchRequest.Status = AppContants.DRAFT_REQUEST_STATUS.ToString();
                branchRequest.Meta = JsonConvert.SerializeObject(model.Items);
            }

            if (branchRequest.Status == AppContants.DRAFT_REQUEST_STATUS.ToString() && model.Draft)
            {
                branchRequest.Status = AppContants.DRAFT_REQUEST_STATUS.ToString();
                branchRequest.Meta = JsonConvert.SerializeObject(model.Items);
            }

            if (branchRequest.Status == AppContants.DRAFT_REQUEST_STATUS.ToString() && !model.Draft)
            {
                branchRequest.Meta = null;
                foreach (var item in model.Items)
                {
                    await VerifyBranchName(new() { Name = item.Name });
                    await VerifyBranchAddress(new ValidateBranchAddressModel(item.Number, item.StreetName, item.City));
                    var branch = new Core.Entities.Branch(item.Name, GenerateSingleBranchCode(item.Name).ToUpperInvariant() + model.Items.IndexOf(item));
                    branch.LocationRef = "";
                    branch.Description = item.Description;
                    branch.Created_By = user.FullName;
                    branch.Created_By_Id = user.UserId;
                    branch.Updated_At = DateTime.UtcNow;
                    branch.Created_By_BranchId = user.BranchId;
                    branch.Status = status;
                    branch.Number = item.Number;
                    branch.StreetName = item.StreetName;
                    branch.City = item.City;
                    branch.State = item.State;
                    branch.Lga = item.Lga;
                    branch.Country = item.Country;
                    branch.PostalCode = item.PostalCode;
                    branchRequest.Branches.Add(branch);
                }

            }

            if (branchRequest.Status == AppContants.PENDING_BRANCH_STATUS.ToString() && !model.Draft)
            {
                branchRequest.Meta = null;
            }

            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Bulk Branch creation submited by {user.FullName}";
            log.BranchRequestId = branchRequest.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            _Db.BranchRequestLogs.Add(log);

            try
            {
                branchRequest.Status = status.ToString();
                _Db.BranchRequests.Update(branchRequest);
                await _Db.SaveChangesAsync();
                if (user.Is_Super.ToLower() == "true" && !model.Draft)
                {
                    await ApproveRequest(branchRequest.Id, user);
                }

                return new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", branchRequest);
            }
            catch (Exception ex)
            {
               // _Logger.LogError("Error Approving request", ex);
                throw new ValidationException("Request was created but not approved");
            }

        }


        private async Task<ApiResponse<object>> VerifyBranchName(ValidateNameModel model)
        {
            var branch = await _Db.Branches.FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower());
            if (branch is not null)
            {
                throw new ValidationException($"Branch name {model.Name} is not available. Please choose another.", null);
            }

            return new SuccessApiResponse<string>("Name is available", "");
        }

        private async Task<ApiResponse<object>> VerifyBranchAddress(ValidateBranchAddressModel model)
        {
            var branchrecord = await _Db.Branches.FirstOrDefaultAsync(
                c => c.Number.ToLower() == model.Number.ToLower()
                    && c.StreetName.ToLower() == model.Street.ToLower()
                    && c.City.ToLower() == model.City.ToLower()
                );
            if (branchrecord is not null)
            {
                throw new ValidationException($"Address is not unique", null);
            }

            return new SuccessApiResponse<string>("Address is available", "");
        }

        private string GenerateSingleBranchCode(string branchName)
        {
            var count = (_Db.Branches.Count() + 1).ToString("D3");

            var branchname = Regex.Replace(branchName.Trim(), "[^a-zA-Z0-9]", String.Empty, RegexOptions.Compiled, TimeSpan.FromSeconds(5));

            var newnamecode = "";
            var namelist = branchname.ToLower().Trim().Split(' ');
            foreach (var item in namelist)
            {
                newnamecode += item.Substring(0, 1);
            }
            var newname = $"{newnamecode}{count}";
            return newname;
        }


    }
}

