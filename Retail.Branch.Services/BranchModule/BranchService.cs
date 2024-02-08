using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Infrastructure;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchModule.QueryFilters;
using Retail.Branch.Services.BranchRequestModule;
using Retail.Branch.Services.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Retail.Branch.Services.BranchModule
{
    public class BranchService : IBranchService
    {
        private readonly BranchDataContext _Db;
        private readonly ILogger<BranchService> _logger;
        private readonly IBranchRequestService _branchRequestService;
        public BranchService(BranchDataContext db, ILogger<BranchService> logger, IBranchRequestService branchRequestService)
        {
            _Db = db;
            _logger = logger;
            _branchRequestService = branchRequestService;
            _Db.ChangeTracker.AutoDetectChangesEnabled = false;
            _Db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        public async Task<ApiResponse<object>> CreateBranch(CreateSingleBranch model, BranchUser user)
        {
            char status = AppContants.DRAFT_BRANCH_STATUS;
            if (!model.Draft)
            {
                status = AppContants.PENDING_BRANCH_STATUS;
                await VerifyName(new() { Name = model.Name });
                await VerifyAddress(new ValidateBranchAddressModel(model.Number, model.StreetName, model.City ));
            }

            var branch = new Core.Entities.Branch(model.Name, GenerateBranchCode(model.Name).ToUpperInvariant());
            branch.LocationRef = model.LocationRef;
            branch.Description = model.Description;
            branch.Created_By = user.FullName;
            branch.Created_By_Id = user.UserId;
            branch.Updated_At = DateTime.UtcNow;
            branch.Created_By_BranchId = user.BranchId;
            branch.Number = model.Number;
            branch.StreetName = model.StreetName;
            branch.City = model.City;
            branch.State = model.State;
            branch.Lga = model.Lga;
            branch.Country = model.Country;
            branch.PostalCode = model.PostalCode;
            _Db.Branches.Add(branch);

            BranchRequest branchRequest = new BranchRequest();
            branchRequest.Request_Type = AppContants.CREATE_REQUEST_TYPE;
            branchRequest.Status = status.ToString();
            branchRequest.Created_By = user.FullName;
            branchRequest.Created_By_Id = user.UserId;
            branchRequest.Description = $"Creation of {branch.Name}";
            branchRequest.Updated_At = DateTime.UtcNow;
            branchRequest.CreatedByBranchId = user.BranchId;

            if (user.Is_Super.ToLower() == "true" && !model.Draft)
            {
                branch.Status = AppContants.APPROVED_BRANCH_STATUS;
                branchRequest.Status = AppContants.APPROVED_BRANCH_STATUS.ToString();
            }

            try
            {
                _Db.Branches.Add(branch);
               _Db.BranchRequests.Add(branchRequest);  
                
                await _Db.SaveChangesAsync();

                BranchBranchRequest br = new BranchBranchRequest();
                br.BranchesId = branch.Id;
                br.BranchRequestsId= branchRequest.Id;
                _Db.BranchBranchRequest.Add(br);

                BranchRequestLog log = new BranchRequestLog();
                log.Description = $"Branch creation submited for {branch.Name} by {user.FullName}";
                log.BranchRequestId = branchRequest.Id;
                log.Created_By_Id = user.UserId;
                log.Created_By = user.FullName;
                log.BranchId = branch.Id;
                _Db.BranchRequestLogs.Add(log);
                await _Db.SaveChangesAsync();

                return new SuccessApiResponse<Core.Entities.Branch>("Branch Created", branch);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Db error occured");
                throw new ValidationException("Could not create branch");
            }

        }

        public Task<PagedResponse<BranchModel>> GetBranches(GetBranchFilter filter, string? userId, Guid? branchId)
        {
            var records = _Db.Branches.OrderByDescending(c=>c.IsLocked).ThenByDescending(c => c.Updated_At).AsQueryable();

            records = records.Where(c => c.Status == 'A' | c.Status == 'I');
            if (!string.IsNullOrEmpty(filter.Q))
            {
                records = records.Where(
                            c => c.Name.ToLower().Contains(filter.Q.ToLower())
                            || c.Code.ToLower().Contains(filter.Q.ToLower())
                            ).AsQueryable();
            }

            if (!string.IsNullOrEmpty(filter.Filter_by))
            {
                switch (filter.Filter_by)
                {
                    case $"created_by_me":
                        records = records.Where(c => c.Created_By_Id == userId);
                        break;

                    case $"created_by_my_branch":
                        records = records.Where(c => c.Created_By_BranchId == branchId);
                        break;

                    case $"created_system_wide":

                        break;

                    case $"approved_by_me":
                        records = records.Where(c => c.Treated_By_Id == userId && c.Status == AppContants.APPROVED_BRANCH_STATUS);
                        break;

                    case $"approved_by_my_branch":
                        records = records.Where(c => c.Treated_By_Branch_Id == branchId.ToString() && c.Status == AppContants.APPROVED_BRANCH_STATUS);
                        break;

                    case $"approved_system_wide":
                        records = records.Where(c => c.Status == AppContants.APPROVED_BRANCH_STATUS);
                        break;
                }
            }

            if (filter.Status__In is not null)
            {
                records = records.Where(c => filter.Status__In.Contains(c.Status));

            }
            else
            {
                records = records.Where(c => c.Status != 'D' && c.Status != 'P');
            }

            if (filter.Start_Date is not null)
            {
                var from = new DateTime(filter.Start_Date.Value.Year, filter.Start_Date.Value.Month, filter.Start_Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);
                records = records.Where(c => c.Updated_At >= from);
            }

            if (filter.End_Date is not null)
            {
                var to = new DateTime(filter.End_Date.Value.Year, filter.End_Date.Value.Month, filter.End_Date.Value.Day, 0, 0, 0, DateTimeKind.Utc);

                records = records.Where(c => c.Updated_At <= to);
            }


            var count = records.Count();

            var pagedData = records.GetPaginatedReponseAsync(filter.Page, filter.Page_Size);
            var result = (from m in pagedData
                          select new BranchModel()
                          {
                              Id = m.Id,
                              Code = m.Code,
                              Created_At = m.Created_At,
                              Created_By = m.Created_By,
                              Created_By_BranchId = m.Created_By_BranchId,
                              Description = m.Description,
                              Name = m.Name,
                              Status = m.Status,
                              Treated_By = m.Treated_By,
                              Treated_By_Branch_Id = m.Treated_By_Branch_Id,
                              Treated_By_Id = m.Treated_By_Id,
                              Created_By_Id = m.Created_By_Id,
                              Updated_At = m.Updated_At,
                              IsLocked = m.IsLocked,
                              LocationRef = m.LocationRef,
                              City = m.City,
                              Number = m.Number,
                              Country = m.Country,
                              Lga = m.Lga,
                              State = m.State,
                              PostalCode = m.PostalCode,
                              StreetName = m.StreetName

                          });

            return Task.FromResult(new PagedResponse<BranchModel>("Success", result, count, filter.Page, filter.Page_Size));
        }

        public ApiResponse<object> BranchAnalytics(string? filterBy, BranchUser user)
        {
            var records = _Db.Branches.AsNoTracking().AsQueryable();
            records = records.Where(c => c.Status == 'A' | c.Status == 'I');

            if (!string.IsNullOrEmpty(filterBy))
            {
                switch (filterBy)
                {
                    case "created_by_anyone":
                        break;

                    case "created_by_me":
                        records = records.Where(c => c.Created_By_Id == user.UserId);
                        break;

                    case $"created_by_my_branch":
                        records = records.Where(c => c.Created_By_BranchId == user.BranchId);
                        break;

                    case $"created_system_wide":

                        break;

                    case $"approved_by_me":
                        records = records.Where(c => c.Treated_By_Id == user.UserId && c.Status == AppContants.APPROVED_BRANCH_STATUS);
                        break;

                    case $"approved_by_my_branch":
                        records = records.Where(c => c.Treated_By_Branch_Id == user.BranchId.ToString() && c.Status == AppContants.APPROVED_BRANCH_STATUS);
                        break;

                    case $"approved_system_wide":
                        records = records.Where(c => c.Status == AppContants.APPROVED_BRANCH_STATUS);
                        break;

                    default:
                        break;

                }
            }


            var active = records.Where(c => c.Status == 'A');
            var inactive = records.Where(c => c.Status == 'I');

            var anaytics = new Dictionary<string, int>();
            anaytics.Add("All", active.Count() + inactive.Count());
            anaytics.Add("A", active.Count());
            anaytics.Add("I", inactive.Count());

            return new SuccessApiResponse<string>("success", anaytics);
        }
        public async Task<ApiResponse<object>> VerifyName(ValidateNameModel model)
        {
            var record = await _Db.Branches.FirstOrDefaultAsync(c => c.Name.ToLower() == model.Name.ToLower());
            if (record is not null)
            {
                throw new ValidationException($"Branch name {model.Name} is not available. Please choose another.", null);
            }

            return new SuccessApiResponse<string>("Name is available", "");
        }

        public async Task<ApiResponse<object>> VerifyAddress(ValidateBranchAddressModel model)
        {
            if (string.IsNullOrEmpty(model.Number))
            {
                throw new ValidationException("Number cannot be empty");
            }
            if (string.IsNullOrEmpty(model.Street))
            {
                throw new ValidationException("Street cannot be empty");
            }

            if (string.IsNullOrEmpty(model.City))
            {
                throw new ValidationException("City cannot be empty");
            }
            var record = await _Db.Branches.FirstOrDefaultAsync(
                c => c.Number.ToLower() == model.Number.ToLower()
                    && c.StreetName.ToLower() == model.Street.ToLower()
                    && c.City.ToLower() == model.City.ToLower()
                );
            if (record is not null)
            {
                throw new ValidationException($"Address is not unique", null);
            }

            return new SuccessApiResponse<string>("Address is available", "");
        }

        private string GenerateBranchCode(string branchName)
        {
            var count = (_Db.Branches.Count() + 1).ToString("D3");

            var name = Regex.Replace(branchName.Trim(), "[^a-zA-Z0-9]", String.Empty,RegexOptions.Compiled, TimeSpan.FromSeconds(5));

            var namecode = "";
            var namelist = name.ToLower().Trim().Split(' ');
            foreach (var item in namelist)
            {
                namecode += item.Substring(0, 1);
            }
            var newname = $"{namecode}{count}";
            return newname;
        }

        public async Task<ApiResponse<object>> GetBranchByCode(string code)
        {
            var record = await _Db.Branches.FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());
            if (record is null)
            {
                throw new ValidationException($"Branch with code {code} not found");
            }
            return new SuccessApiResponse<Core.Entities.Branch>("Branch found", record);
        }



        public async Task<ApiResponse<object>> Update(Guid id, ModifyBranch model, BranchUser user)
        {
            var record = await _Db.Branches.FirstOrDefaultAsync(c => c.Id == id);
            if (record is null)
            {
                throw new ValidationException($"Branch not found");
            }

            if (model.Name.ToLower() != record.Name.ToLower())
            {
                await VerifyName(new ValidateNameModel { Name = model.Name });
            }

            BranchRequest request = new BranchRequest();
            request.CreatedByBranchId = user.BranchId;
            request.Created_By_Id = user.UserId;
            request.Created_By = user.FullName;
            request.Status = AppContants.PENDING_REQUEST_STATUS.ToString();
            request.Description = $"Modification of Branch {record.Name}";
            request.Request_Type = AppContants.CHANGE_REQUEST_TYPE;
            request.Alt_Name = model.Name;
            request.Alt_Description = model.Description;
            request.Meta = JsonConvert.SerializeObject(model);
            request.Updated_At = DateTime.UtcNow;

            record.BranchRequests.Add(request);
            _Db.Branches.Update(record);
            await _Db.SaveChangesAsync();

            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Modification of Branch {record.Name} to {model.Name} by {user.FullName}";
            log.BranchRequestId = request.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            log.BranchId = record.Id;
            _Db.BranchRequestLogs.Add(log);

            try
            {
                await _Db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured on branch mondify request", ex);
                throw;
            }

            if (user.Is_Super.ToLower() == "true")
            {
                await _branchRequestService.ApproveRequest(request.Id, user);
            }

            return new SuccessApiResponse<BranchRequest>("Branch modification request submited", record);


            ;
        }

        public async Task<ApiResponse<object>> BulkUpload(List<CreateBulkBranchItem> model)
        {
            CreateBulkResponse response = new();

            var duplicates = model.GroupBy(g => g.Name)
                   .Where(g => g.Count() > 1)
                   .Select(g => g.Key).ToList();

            foreach (var item in model)
            {              
                response.Items.Add(await ValidateUploadItem(item, model));
            }

            response.Total = response.Items.Count;
            response.Success = response.Items.Count(c => c.Status == true);
            response.Failed = response.Items.Count(c => c.Status == false);
            foreach (var item in duplicates)
            {
                response.Errors.Add($"There is more than one row in the file with this branch name {item}.");
            }

            return new SuccessApiResponse<CreateBulkResponse>("", response);
        }

        public async Task<ApiResponse<object>> CreateBulkReques(SaveBulkRequest model, BranchUser user)
        {
            
            char status = AppContants.DRAFT_BRANCH_STATUS;
            if (!model.Draft)
                status = AppContants.PENDING_BRANCH_STATUS;
           

            BranchRequest branchRequest = new BranchRequest();
            branchRequest.Request_Type = AppContants.BULK_CREATE_REQUEST_TYPE;
            branchRequest.Status = status.ToString();
            branchRequest.Created_By = user.FullName;
            branchRequest.Created_By_Id = user.UserId;
            branchRequest.Description = $"Creation of bulk request {model.Items?.FirstOrDefault()?.Name} {model.Items?.LastOrDefault()?.Name}";
            branchRequest.Updated_At = DateTime.UtcNow;
            branchRequest.CreatedByBranchId = user.BranchId;
            if (model.Draft)
            {
                branchRequest.Meta = JsonConvert.SerializeObject(model.Items);
            }
            else
            {
                if(model.Items is not null)
                {
                    foreach (var item in model.Items)
                    {
                        await VerifyName(new() { Name = item.Name });
                        await VerifyAddress(new ValidateBranchAddressModel(item.Number, item.StreetName, item.City));
                        var branch = new Core.Entities.Branch(item.Name, GenerateBranchCode(item.Name).ToUpperInvariant() + model.Items.IndexOf(item));
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
                

            }


            _Db.BranchRequests.Add(branchRequest);
            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Bulk Branch creation submited by {user.FullName}";
            log.BranchRequestId = branchRequest.Id;
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            _Db.BranchRequestLogs.Add(log);

            try
            {
                await _Db.SaveChangesAsync();

                if (user.Is_Super.ToLower() == "true" && !model.Draft)
                {
                    await _branchRequestService.ApproveRequest(branchRequest.Id, user);
                }
                return new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", branchRequest);
            }
            catch (Exception ex)
            {

                throw;
            }



        }
        public async Task<ApiResponse<object>> ActivateBranch(Guid Id,BranchUser user)
        {
            var record = await _Db.Branches.FirstOrDefaultAsync(c => c.Id == Id);
            if (record is null)
            {
                throw new ValidationException($"Branch not found");
            }

            record.Status = AppContants.APPROVED_BRANCH_STATUS;
            _Db.Branches.Update(record);          

            BranchRequestLog log = new BranchRequestLog();
            log.Description = $"Branch has been activated by {user.FullName}";          
            log.Created_By_Id = user.UserId;
            log.Created_By = user.FullName;
            log.BranchId = record.Id;
            _Db.BranchRequestLogs.Add(log);
            await _Db.SaveChangesAsync();

            return new SuccessApiResponse<Core.Entities.Branch>("Branch Activated", record);
        }
        private async Task<CreateBulkBranchItemResponse> ValidateUploadItem(CreateBulkBranchItem item, List<CreateBulkBranchItem> uploadlist)
        {
            CreateBulkBranchItemResponse model = new();
            model.Name = item?.Name??"";
            model.Description = item?.Description;
            model.Status = true;
            model.Number = item.Number;
            model.StreetName = item.Street;
            model.City = item.City;
            model.State = item.State;
            model.Lga = item.Lga;
            model.PostalCode = item.Zip;
            model.Country= item.Country;

            if (string.IsNullOrEmpty(item.Name))
            {
                model.Status = false;
                model.Status_Description = "Name cannot be empty";
            }
            else
            {
                var namestatus = await CheckName(item.Name);
                if (namestatus == false)
                {
                    model.Status = false;
                    model.Status_Description += " Name is not available";
                }
            }

          

            var namecount = uploadlist.Where(c => c.Name == item.Name).Count();
            if (namecount > 1)
            {
                model.Status = false;
                model.Status_Description += "Branch Name is not unique";
            }

            var addresscount = uploadlist.Where(c => c.Number == item.Number && c.Street == model.StreetName && c.City == model.City).Count();
            if (addresscount > 1)
            {
                model.Status = false;
                model.Status_Description += " Address is not unique";
            }
            return model;
        }

        public async Task<bool> CheckName(string name)
        {

            var record = await _Db.Branches.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (record is not null)
            {
                return false;
            }

            return true;
        }

        public PagedResponse<BranchRequestLog> GetBranchRequestLogs(Guid request_id)
        {
            var records = _Db.BranchRequestLogs.AsNoTracking().Where(c => c.BranchId == request_id).AsQueryable();

            var count = records.Count();

            var pagedData = records.GetPaginatedReponseAsync(1, 10);

            return new PagedResponse<BranchRequestLog>("Success", pagedData, count, 1, 10);
        }


    }
}
