using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Retail.Branch.Core.Common;
using Retail.Branch.Infrastructure;
using Retail.Branch.Services;
using Retail.Branch.Services.BranchModule;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchRequestModule;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Retail.Branch.ServicesTests.BranchModule
{
    public class BranchServiceTests2
    {
        private readonly MockRepository mockRepository;

        private readonly Mock<ILogger<BranchService>> mockLogger;
        private readonly Mock<IBranchRequestService> mockRequestbranchService;
        private BranchDataContext db;
        public BranchServiceTests2()
        {

            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockLogger = this.mockRepository.Create<ILogger<BranchService>>();
            this.mockRequestbranchService = this.mockRepository.Create<IBranchRequestService>();
        }

        private BranchService CreateService()
        {
            var options = new DbContextOptionsBuilder<BranchDataContext>()
              .UseInMemoryDatabase(databaseName: "TestNewDb")
              .Options;
            db = new BranchDataContext(options);
            return new BranchService(
                db,
                this.mockLogger.Object,
                this.mockRequestbranchService.Object);

        }

        [Fact]
        public async Task CreateBranch_BySuperAdmin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var options = new DbContextOptionsBuilder<BranchDataContext>()
            .UseInMemoryDatabase(databaseName: "NewDb2")
            .Options;
            db = new BranchDataContext(options);
            CreateSingleBranch model = new CreateSingleBranch();
            model.Description = "Description";
            model.Name = "New Admin Branch";
            model.Draft = false;
            model.Number = "12";
            model.StreetName = "Test street";
            model.City = "Lagos";
            BranchUser user = TestData.GetCurrentUser();
            mockLogger.Setup(x => x.Log(LogLevel.Information, 0,
                             It.IsAny<object>(), It.IsAny<Exception>(),
                             It.IsAny<Func<object, Exception, string>>()))
                             .Verifiable();

            //  mockLogger.Setup(s => s.LogError("Error", new Exception()));

            // Act
            var result = await service.CreateBranch(
                model,
                user);

            // Assert
            var expected = new SuccessApiResponse<object>("Branch Created", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }


        [Fact]
        public async Task CreateBulkBranch_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var options = new DbContextOptionsBuilder<BranchDataContext>()
            .UseInMemoryDatabase(databaseName: "NewDb3")
            .Options;
            db = new BranchDataContext(options);
            SaveBulkRequest model = new()
            {
                Draft = false,
                Items = new List<CreateBulkBranchItemResponse>()
                  {
                       new CreateBulkBranchItemResponse()
                       { City="warri1", Country="Nigeria", Description="Dev Description",
                           Lga="North", Name="Dev Branch 11", Number="1", PostalCode="234"
                        , State="Lagos", StreetName="ikoyi", Status=true,
                       }
                  }
            };
            BranchUser user = TestData.GetBasicUser();

            // Act
            var result = await service.CreateBulkReques(
                model,
                user);

            // Assert
            var expected = new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);
            // this.mockRepository.VerifyAll();
        }


       
        [Fact]
        public async Task CreateDraftBulkBranch_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            var service = this.CreateService();
            var options = new DbContextOptionsBuilder<BranchDataContext>()
            .UseInMemoryDatabase(databaseName: "NewDraftDb")
            .Options;
            db = new BranchDataContext(options);
            SaveBulkRequest model = new()
            {
                Draft = true,
                Items = new List<CreateBulkBranchItemResponse>()
                  {
                       new CreateBulkBranchItemResponse()
                       { City="warri", Country="Nigeria", Description="Description", Lga="North", Name="Cool West Branch", Number="1", PostalCode="234"
                        , State="Lagos", StreetName="ikoyi", Status=true}
                  }
            };
            BranchUser user = TestData.GetBasicUser();

            // Act
            var result = await service.CreateBulkReques(
                model,
                user);

            // Assert
            var expected = new SuccessApiResponse<Core.Entities.Branch>("Bulk request saved", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CreateDraftBranch_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            CreateSingleBranch model = new CreateSingleBranch();
            model.Description = "Draft Description";
            model.Name = "New Draft Name";
            model.Draft = true;
            model.Number= "1";
            model.StreetName = "lag";
            model.City = "Lagos";
            BranchUser user = TestData.GetCurrentUser();
            mockLogger.Setup(x => x.Log(LogLevel.Information, 0,
                            It.IsAny<object>(), It.IsAny<Exception>(),
                            It.IsAny<Func<object, Exception, string>>()))
                            .Verifiable();


            // Act
            var result = await service.CreateBranch(
                model,
                user);

            // Assert
            var expected = new SuccessApiResponse<object>("Branch Created", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }

    }




}
