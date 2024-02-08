using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using Retail.Branch.Core.Common;
using Retail.Branch.Core.Entities;
using Retail.Branch.Infrastructure;
using Retail.Branch.Services;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchRequestModule;
using Retail.Branch.Services.BranchRequestModule.Models;
using Retail.Branch.Services.BranchRequestModule.QueryFilter;
using Retail.Branch.Services.Common;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Xunit;

namespace Retail.Branch.ServicesTests.BranchRequestModule
{
    public class BranchRequestServiceTests
    {

        private readonly MockRepository mockRepository;
        private BranchDataContext db;
        private BranchRequestService service;

        private readonly Mock<ILogger<BranchRequestService>> mockLogger;

        public BranchRequestServiceTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockLogger = this.mockRepository.Create<ILogger<BranchRequestService>>();
            if(service is null)
            {
                service = this.CreateService();
            }
           
        }

        private BranchRequestService CreateService()
        {

            var options = new DbContextOptionsBuilder<BranchDataContext>()
            .UseInMemoryDatabase(databaseName: "BranchService1")
            .Options;
            db = new BranchDataContext(options);

            if (db.Branches.Count() == 0)
            {
                // db.Branches.AddAsync(new Core.Entities.Branch("HQ", "HQ"));
                var defaultBranch = new Core.Entities.Branch("HQ", "HQ");
                //  defaultBranch.Id = TestData.DefaultBranchId;
                defaultBranch.Number = "1";
                defaultBranch.StreetName = "Lagos";
                defaultBranch.City = "Lagos";
                defaultBranch.IsLocked = true;
                db.Branches.Add(defaultBranch);

                db.BranchRequests.Add(
                    new BranchRequest()
                    {
                        Id = TestData.PendingRequestId,
                        Request_Type = AppContants.CREATE_REQUEST_TYPE,
                        Description = "Create new branch-1",
                        Status = AppContants.APPROVED_REQUEST_STATUS.ToString(),
                        Created_By_Id = Guid.Empty.ToString(),
                        Updated_At = DateTime.UtcNow,
                        Branches = new List<Core.Entities.Branch>()
                        {
                         new Core.Entities.Branch("Test Branch","TB1"){ Id=TestData.PendingRequestId3, Number="233",City="Warr", StreetName="testcity"}
                        }

                    });
                db.BranchBranchRequest.Add(new() { BranchesId = defaultBranch.Id, BranchRequestsId = Guid.Parse("18063564-c028-4543-b2df-e0b12dffb2ca") });


                db.BranchRequests.Add(
                    new BranchRequest()
                    {
                        Id = TestData.PendingRequestId2,
                        Request_Type = AppContants.CREATE_REQUEST_TYPE,
                        Description = "Create new branch-2",
                        Status = AppContants.PENDING_REQUEST_STATUS.ToString(),
                        Updated_At = DateTime.UtcNow,
                        Branches = new List<Core.Entities.Branch>()
                        {
                         new Core.Entities.Branch("Test Branch 2","TB2"){Number="093", StreetName="ogu", City="ibadan"}

                        },

                    }); ;

                //  db.BranchBranchRequest.AddAsync(new() { BranchesId = defaultBranch.Id, BranchRequestsId = Guid.Parse("18063564-c028-4543-b2df-e0b12dffb2ca") });


                db.BranchRequests.Add(
                   new BranchRequest()
                   {
                       Id = TestData.PendingChangeRequestId,
                       Request_Type = AppContants.CHANGE_REQUEST_TYPE,
                       Description = "change new branch-3",
                       Status = AppContants.PENDING_REQUEST_STATUS.ToString(),
                       Updated_At = DateTime.UtcNow,
                       Branches = new List<Core.Entities.Branch>()
                       {
                         new Core.Entities.Branch(" changeTest Branch 2","ch2"){Number="09333", StreetName="ogunu", City="ibadan"}
                       }
                       ,
                       Meta = JsonConvert.SerializeObject(TestData.GetEditModel()),

                   });

                //  db.SaveChanges();
                db.BranchRequests.Add(
                  new BranchRequest()
                  {
                      Id = TestData.PendingBulkRequestId,
                      Request_Type = AppContants.BULK_CREATE_REQUEST_TYPE,
                      Description = "Create Bulk-1",
                      Status = AppContants.PENDING_REQUEST_STATUS.ToString(),
                      Updated_At = DateTime.UtcNow,
                      Branches = new List<Core.Entities.Branch>()
                      {
                         new Core.Entities.Branch("Bulk Test Branch 2","Bulk TB2"){Number="01193", StreetName="oweri", City="imo"}

                      }

                  });
                BranchRequestLog log = new BranchRequestLog()
                {
                    BranchRequestId = TestData.PendingBulkRequestId,
                    Description = "Test Log Description",

                };
                db.BranchRequestLogs.Add(log);

                db.SaveChanges();
            }
            if (service is null)
            {
                service = new BranchRequestService(
                     db,
                     this.mockLogger.Object);
            }
            return service;
          



        }

        [Fact]
        public void GetAnalytics_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
         //   var service = this.CreateService();
            string? filter_By = null;
            BranchUser user = TestData.GetCurrentUser();
            var anaytics = new Dictionary<string, int>();
            anaytics.Add("All", 2);
            anaytics.Add("P", 1);
            anaytics.Add("D", 0);
            anaytics.Add("A", 1);
            anaytics.Add("R", 0);
            var expected = new SuccessApiResponse<object>("success", anaytics);

            // Act
            var result = service.GetAnalytics(
                filter_By,
                user);

            // Assert
            Assert.Equivalent(expected.Succeeded, result.Succeeded);

        }

        [Fact]
        public async Task GetBranchRequests_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
         //   var service = this.CreateService();
            BranchRequestFilter filter = new() { Page = 1, Page_Size = 3, };
            string? userId = TestData.GetCurrentUser().UserId;
            string? branchId = null;

            // Act
            var result = await service.GetBranchRequests(
                filter,
                TestData.GetCurrentUser());

            // Assert

            Assert.Equal<int>(result.Count, 4);

        }

        [Fact]
        
        public async Task GetBranchRequests_with_filters_ExpectedBehavior()
        {
            // Arrange
            //var service = this.CreateService();
            string[] statusin = new string[] { "A", "P" };
            string[] requestType = new string[] { "CREATE" };
          
            BranchRequestFilter filter = new()
            {
                Page = 1,
                Page_Size = 2,
                Filter_By = "created_by_me",
                Start_Date = DateTime.UtcNow.AddDays(-1).ToUniversalTime(),
                End_Date = DateTime.UtcNow.AddDays(2),
                Status__In = statusin,
                Request_Type__In = requestType,
                Q = "Create",
                
            };
          
            // Act
            var result = await service.GetBranchRequests(
                filter,                 
                TestData.GetCurrentUser());

            // Assert

            Assert.Equal<int>(result.Count, 1);

        }

        [Fact]
        public async Task GetDetails_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
           // var service = this.CreateService();
            Guid Id = TestData.PendingRequestId;

            // Act
            var result = await service.GetDetails(
                Id);
            var expected = new SuccessApiResponse<BranchRequestModel>("Branch Record", result.Data);


            // Assert
            Assert.Equivalent(expected.Data, result.Data);

        }

        [Fact]
        public async Task ApproveRequest_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
           // var service = this.CreateService();
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>($"Branch approved", true);
            db.ChangeTracker.Clear();

            // Act
            var result = await service.ApproveRequest(
                TestData.PendingChangeRequestId,
                user);

            // Assert
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }

        [Fact]
        public async Task ActivateBranch_StateUnderTest_UExpectedBehavior()
        {
            // Arrange
          //  var service = this.CreateService();
            var id =TestData.PendingRequestId3;
            db.ChangeTracker.Clear();
            await Assert.ThrowsAsync<ValidationException>(() => service.ApproveRequest(id, TestData.GetCurrentUser()));
           // this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task RejectRequest_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
           // var service = this.CreateService();
            Guid Id = TestData.PendingRequestId2;
            BranchUser user = TestData.GetCurrentUser();
            RejectBranchRequest model = new RejectBranchRequest("this is a test reason");
            model.RouteTo = new BasicUserInfo() { Email = "ojunix@appquest.com.ng", FullName = "ojorma", UserId = Guid.Empty.ToString() };
            var expected = new SuccessApiResponse<bool>($"Branch  request Rejected", true);
            db.ChangeTracker.Clear();
            // Act
            var result = await service.RejectRequest(
                Id,
                user,
                model);

            // Assert
            Assert.Equal(result.Succeeded, expected.Succeeded);
        }

        [Fact]
        public void GetBranchRequestLogs_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
          //  var service = this.CreateService();
            Guid request_id = TestData.DefaultRequestId;

            // Act
            var result = service.GetBranchRequestLogs(
                request_id);
            var expected = new PagedResponse<BranchRequestLog>("Success", result.Results, 1, 1, 10);

            // Assert
            Assert.Equal(result.Results, expected.Results);

        }

        [Fact]
        public async Task DeleteRequest_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
          //  var service = this.CreateService();
            Guid Id = TestData.PendingRequestId2;
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>("Request withdrawn and deleted successfully", true);

            // Act
            db.ChangeTracker.Clear();
            var result = await service.DeleteRequest(
                Id,
                user);

            // Assert
            Assert.Equivalent(result.Succeeded, expected.Succeeded);
            //this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task ApproveRequest_ExpectedBehaviour()
        {
            //Arrange
           // var service = this.CreateService();
            Guid Id = TestData.PendingRequestId;
         //   var i= "18063564-c028-4543-b2df-e0b12dffb2ca"
            BranchUser user = TestData.GetCurrentUser();
            var expected =   new SuccessApiResponse<bool>($"Branch approved", true);
           db.ChangeTracker.Clear();
            //Act
            var result =await service.ApproveRequest(Id, user);

            //Assert
            Assert.Equivalent(result.Succeeded, expected.Succeeded);
        }

        [Fact]
        public async Task ApproveModifyRequest_ExpectedBehaviour()
        {
            
            //Arrange
          //  var service = this.CreateService();
            var request = db.BranchBranchRequest.ToList();
            Guid Id = TestData.PendingChangeRequestId;
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>($"Branch approved", true);

            //Act
            db.ChangeTracker.Clear();
            var result = await service.ApproveRequest(Id, user);

            //Assert
            Assert.Equivalent(result.Succeeded, expected.Succeeded);
        }

        [Fact]
        public async Task ApproveRequestBulk_ExpectedBehaviour()
        {
            //Arrange
         //   var service = this.CreateService();
            Guid Id = TestData.PendingBulkRequestId;
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>($"Branch approved", true);

            //Act
            var result = await service.ApproveRequest(Id, user);

            //Assert
            Assert.Equivalent(result.Succeeded, expected.Succeeded);
        }

        [Fact]
        public async Task UpdateApprovedRequest_ExpectedBehavior()
        {
            // Arrange
           // var service = this.CreateService();
            EditRequestModel model = new EditRequestModel()
            {
                name = "HQ",
                city = "Lagos",
                code = "code",
                country = "Nigeria",
                description = "description",
                draft = false,
                id = Guid.NewGuid(),
                lga = "ikeja",
                number = "3",
                postalCode = "2",
                streetname = "2",
                State = "Lagos",
                type = ""

            };
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>("Successful ", true);
            mockLogger.Setup(x => x.Log(LogLevel.Error, 0,
                           It.IsAny<object>(), It.IsAny<Exception>(),
                           It.IsAny<Func<object, Exception, string>>()))
                           .Verifiable();

            // Act

            // Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateRequest(model, user));
        }

        [Fact]
        public async Task UpdateRequest_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
          //  var service = this.CreateService();
            EditRequestModel model = new EditRequestModel()
            {
                name = "name",
                city = "Lagos",
                code = "code",
                country = "Nigeria",
                description = "description",
                draft = true,
                id = TestData.PendingChangeRequestId,
                lga = "ikeja",
                number = "3",
                postalCode = "2",
                streetname = "2",
                State = "Lagos",
                type = ""

            };
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>("Successful ", true);
            // Act
            db.ChangeTracker.Clear();
            var result = await service.UpdateRequest(
                model,
                user);

            // Assert
            Assert.Equivalent(expected.Succeeded, result.Succeeded);
          //  this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task SaveDiactivationRequest_for_DefaultBranch_ExpectedBehavior()
        {
            // Arrange
          //  var service = this.CreateService();
            DeactivationRequestModel model = new DeactivationRequestModel()
            {
                BranchId = TestData.DefaultBranchId,
                Draft = false,
                Ledgers = new List<DeactivateLedgerModel>() { 
                                    new DeactivateLedgerModel() { Code="abc", Currency="NGN", DestinationLedgerId="1232",
                                     DestinationLedgerName="Cash", LegerId="123", Name="Cash", NetBalance=200000} 
                                    },
                Reason = "Non needed",
                Users = new List<DiactivateUserModel>()
                {
                    new DiactivateUserModel() {  DictivateAction=0, FullName="ben", ReassignBranchId="123", UserId="123"}
                }
            };
            BranchUser user = TestData.GetCurrentUser();

            // Act

            // Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.SaveDiactivationRequest(model, user));
        }


        [Fact]
        public async Task EditBulkRequestBasicAdmin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
           // var service = this.CreateService();
            SaveBulkRequest model = new()
            {
                Draft = false,
                Items = new List<CreateBulkBranchItemResponse>()
                  {
                       new CreateBulkBranchItemResponse()
                       { City="warri", Country="Nigeria", Description="Description", Lga="North", Name="North Branch", Number="1", PostalCode="234"
                        , State="Lagos", StreetName="ikoyi", Status=true}
                  }
            };
            BranchUser user = TestData.GetBasicUser();

            // Act
            db.ChangeTracker.Clear();
            var result = await service.UpdateBulkReques(TestData.PendingBulkRequestId,
                                                         model,
                                                         user);
            // Assert
            var expected = new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);
          
        }

        [Fact]
        public async Task EditBulkRequestSuperAdmin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
          //  var service = this.CreateService();
            SaveBulkRequest model = new()
            {
                Draft = false,
                Items = new List<CreateBulkBranchItemResponse>()
                  {
                       new CreateBulkBranchItemResponse()
                       { City="warri", Country="Nigeria", Description="Description", Lga="North", Name="North Branch", Number="1", PostalCode="234"
                        , State="Lagos", StreetName="ikoyi", Status=true}
                  }
            };
            BranchUser user = TestData.GetCurrentUser();

            // Act
            db.ChangeTracker.Clear();
            var result = await service.UpdateBulkReques(TestData.PendingBulkRequestId,
                                                         model,
                                                         user);
            // Assert
            var expected = new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }

        [Fact]
        public async Task EditBulkRequestAdmin_StateUnderTest_UnExpectedBehavior()
        {
            // Arrange
            //var service = this.CreateService();
            SaveBulkRequest model = new()
            {
                Draft = false,
                Items = new List<CreateBulkBranchItemResponse>()
                  {
                       new CreateBulkBranchItemResponse()
                       { City="warri", Country="Nigeria", Description="Description", Lga="North", Name="North Branch", Number="1", PostalCode="234"
                        , State="Lagos", StreetName="ikoyi", Status=true}
                  }
            };
            BranchUser user = TestData.GetBasicUser();

            // Act &
            // Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.UpdateBulkReques(Guid.NewGuid(), model, user));
           
        }
       


    }

  
}
