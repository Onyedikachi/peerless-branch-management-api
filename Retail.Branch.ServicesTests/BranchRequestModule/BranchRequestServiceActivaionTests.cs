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
using Xunit;

namespace Retail.Branch.ServicesTests.BranchRequestModule
{
    public class BranchRequestServiceActivaionTests
    {

        private readonly MockRepository mockRepository;
        private BranchDataContext db;

        private readonly Mock<ILogger<BranchRequestService>> mockLogger;

        public static Guid BranchId1 = Guid.Parse("18063565-c028-4543-b2df-e0b13dffb2ca");
        public static Guid BranchId2 = Guid.Parse("18063565-c028-4543-b2df-e0b12dffb2cb");
        public static Guid BranchId3 = Guid.Parse("18063565-c028-4543-b2df-e0b12dffb2cc");


        public BranchRequestServiceActivaionTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Loose);
            this.mockLogger = this.mockRepository.Create<ILogger<BranchRequestService>>();
        }

        private BranchRequestService CreateService()
        {

            var options = new DbContextOptionsBuilder<BranchDataContext>()
            .UseInMemoryDatabase(databaseName: "BranchRequesServiceAC")
            .Options;
            db = new BranchDataContext(options);
            if (db.Branches.Count() == 0)
            {

           

            var defaultBranch = new Core.Entities.Branch("HQ", "HQ");
            defaultBranch.Id = TestData.DefaultBranchId;
            defaultBranch.Number = "1";
            defaultBranch.StreetName = "Lagos";
            defaultBranch.City = "Lagos";
            defaultBranch.IsLocked = true;
            db.Branches.Add(defaultBranch);
            db.BranchRequests.Add(
                new BranchRequest()
                {
                    Id = Guid.Parse("18063564-c028-4543-b2df-e0b12dffb2ca"),
                    Request_Type = AppContants.CREATE_REQUEST_TYPE,
                    Description = "Create new branch",
                    Status = AppContants.APPROVED_REQUEST_STATUS.ToString(),
                    Created_By_Id = Guid.Empty.ToString(),
                    Updated_At = DateTime.UtcNow,
                    Branches = new List<Core.Entities.Branch>()
                    {
                         new Core.Entities.Branch("Test Branch","TB1"){Id= BranchId1, Number="233",City="Warr", StreetName="testcity"}
                    }

                });
            db.BranchRequests.Add(
              new BranchRequest()
              {
                  Id = TestData.PendingChangeRequestId,
                  Request_Type = AppContants.CHANGE_REQUEST_TYPE,
                  Description = "change new branch",
                  Status = AppContants.DRAFT_REQUEST_STATUS.ToString(),
                  Updated_At = DateTime.UtcNow,
                  Branches = new List<Core.Entities.Branch>()
                  {
                         new Core.Entities.Branch(" changeTest Branch 2","ch2"){Id= BranchId2, Number="233",City="Warr", StreetName="testcity"}

                  }
                  ,
                  Meta = JsonConvert.SerializeObject(TestData.GetEditModel()),

              });

            db.BranchRequests.Add(
            new BranchRequest()
            {
                Id = TestData.DraftBulkRequestId,
                Request_Type = AppContants.BULK_CREATE_REQUEST_TYPE,
                Description = "Create Bulk Draft",
                Status = AppContants.DRAFT_REQUEST_STATUS.ToString(),
                Updated_At = DateTime.UtcNow,
                Branches = new List<Core.Entities.Branch>()
                {
                         new Core.Entities.Branch("Bulk Draft Test Branch 2","Dradt-TB2"){Id= BranchId3, Number="0233",City="Warr0", StreetName="testcity0"}

                }

            });

            db.SaveChanges();
            }
            return new BranchRequestService(
                db,
                this.mockLogger.Object);


        }




        [Fact]
        public async Task SaveDiactivationRequest_for_DraftBranch_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
          
            DeactivationRequestModel model = new DeactivationRequestModel()
            {
                BranchId = BranchId3,
                Draft = false,
                Ledgers = new List<DeactivateLedgerModel>() {
                                    new DeactivateLedgerModel() { Code="abc", Currency="NGN", DestinationLedgerId="1232",
                                     DestinationLedgerName="Cash", LegerId="123", Name="Cash", NetBalance=200000}
                                    },
                Reason = "Not needed",
                Users = new List<DiactivateUserModel>()
                {
                    new DiactivateUserModel() {  DictivateAction=0, FullName="ben", ReassignBranchId="123", UserId="123"}
                },

            };
            BranchUser user = TestData.GetCurrentUser();
            var expected = new SuccessApiResponse<bool>("Deactivation request has been submited", true); ;
            db.ChangeTracker.Clear();
            // Act
            var result = await service.SaveDiactivationRequest(model, user);
            // Assert
            Assert.Equivalent(expected.Succeeded, result.Succeeded);
        }

        [Fact]
        public async Task ApproveRequestDraftBasicUser_ExpectedBehaviour()
        {
            //Arrange
            var service = this.CreateService();
            BranchUser user = TestData.GetBasicUser();
            // var branch = db.Branches.FirstOrDefault();
            DeactivationRequestModel model = new DeactivationRequestModel()
            {
                BranchId = BranchId2,
                Draft = true,
                Ledgers = new List<DeactivateLedgerModel>() {
                                    new DeactivateLedgerModel() { Code="abc", Currency="NGN", DestinationLedgerId="1232",
                                     DestinationLedgerName="Cash", LegerId="123", Name="Cash", NetBalance=200000}
                                    },
                Reason = "Not needed",
                Users = new List<DiactivateUserModel>()
                {
                    new DiactivateUserModel() {  DictivateAction=0, FullName="ben", ReassignBranchId="123", UserId="123"}
                }
            };

            db.ChangeTracker.Clear();
            await service.SaveDiactivationRequest(model, user);

            Guid Id = db.BranchRequests.FirstOrDefault(c => c.Status == "D").Id;

            var expected = new SuccessApiResponse<bool>($"Branch approved", true);
            db.ChangeTracker.Clear();
            //Act
            var result = await service.ApproveRequest(Id, user);

            //Assert
            Assert.Equivalent(result.Succeeded, expected.Succeeded);
        }


        [Fact]
        public async Task EditBulkRequestinDraftBasicAdmin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
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
            var result = await service.UpdateBulkReques(TestData.DraftBulkRequestId,
                                                         model,
                                                         user);
            // Assert
            var expected = new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }

    }


}
