using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Retail.Branch.Core.Common;
using Retail.Branch.Infrastructure;
using Retail.Branch.Services;
using Retail.Branch.Services.BranchModule;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchModule.QueryFilters;
using Retail.Branch.Services.BranchRequestModule;
using Retail.Branch.Services.BranchRequestModule.QueryFilter;
using retail_teams_management_api.Controllers.v1;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Retail.Branch.ServicesTests.BranchModule
{
    public class BranchServiceTests
    {
        private readonly MockRepository mockRepository;

        private readonly Mock<ILogger<BranchService>> mockLogger;
        private readonly Mock<IBranchRequestService> mockbranchService;
        private BranchDataContext db;
        public BranchServiceTests()
        {

            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockLogger = this.mockRepository.Create<ILogger<BranchService>>();
            this.mockbranchService = this.mockRepository.Create<IBranchRequestService>();
        }

        private BranchService CreateService()
        {
           
            var options = new DbContextOptionsBuilder<BranchDataContext>()
             .UseInMemoryDatabase(databaseName: "NewDb")
             .Options;
            db = new BranchDataContext(options);
            var defaultBranch = new Core.Entities.Branch("HQ", "HQ");
            defaultBranch.Id = TestData.DefaultBranchId;
            defaultBranch.Number = "1";
            defaultBranch.StreetName = "Lagos";
            defaultBranch.City = "Lagos";
            db.Branches.AddAsync(defaultBranch);
            db.SaveChangesAsync();
            return new BranchService(
                db,
                this.mockLogger.Object,
                this.mockbranchService.Object);

        }


        [Fact]
        public async Task CreateBranch_StateUnderTest_UnExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            CreateSingleBranch model = new CreateSingleBranch();
            model.Description = "Description";
            model.Name = "HQ";
            model.Draft = false;
            BranchUser user = TestData.GetCurrentUser();

            // Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.CreateBranch(model, user));
        }


        [Fact]
        public async Task SaveBulk_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            var model = new List<CreateBulkBranchItem>()
                  {
                       new CreateBulkBranchItem()
                       { City="warri",  Description="Description", Lga="North", Name="North Branch", Number="2", Zip="234"
                        , State="Lagos", Street="ikoyi"}
                  };

            // Act
            var result = await service.BulkUpload(model);

            // Assert
            var expected = new SuccessApiResponse<CreateBulkResponse>("", null);
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }

       
        [Fact]
        public async Task VerifyName_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            ValidateNameModel model = new();
            model.Name = "Test";

            var expected = new SuccessApiResponse<object>("Name is available", "");
            // Act
            var result = await service.VerifyName(
                model);

            // Assert
            Assert.Equivalent(expected, result);

        }


        [Fact]
        public void GetAnalytics_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            string? filter_By = null;
            BranchUser user = TestData.GetCurrentUser();
            var anaytics = new Dictionary<string, int>();
            anaytics.Add("All", 1);
            anaytics.Add("A", 1);
            anaytics.Add("I", 0);
            var expected = new SuccessApiResponse<object>("success", anaytics);

            // Act
            var result = service.BranchAnalytics(
                filter_By,
                user);

            // Assert
            Assert.Equivalent(expected.Succeeded, result.Succeeded);

        }

        [Fact]
        public async Task GetBranchByCodeUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var code = "HQ";
            var hqbranch = new Core.Entities.Branch("HQ", "HQ");

            var expected = new SuccessApiResponse<Core.Entities.Branch>("Branch found", hqbranch);
            // Act
            var result = await service.GetBranchByCode(code);
            // Assert
            Assert.Equivalent(expected.Msg, result.Msg);

        }

        [Fact]
        public async Task BulkUpload_ExpectedBehavior()
        {
            //Arrange
            var service = this.CreateService();
            List<CreateBulkBranchItem> model = new List<CreateBulkBranchItem>();
            model.Add(new() { City = "Lagos", Description = "test", Lga = "", Name = "TestBranch", Number = "1" });

            CreateBulkResponse response = new();
            response.Total = 1;
            response.Success = 1;
            var expected = new SuccessApiResponse<CreateBulkResponse>("", response);

            //Act
            var result = await service.BulkUpload(model);

            //Asert
            Assert.Equivalent(expected, result);
        }


        [Fact]
        public async Task BulkUpload_DuplicateNamesExpectedBehavior()
        {
            //Arrange
            var service = this.CreateService();
            List<CreateBulkBranchItem> model = new List<CreateBulkBranchItem>();
            model.Add(new() { City = "Lagos", Description = "test", Lga = "", Name = "TestBranch", Number = "1" });
            model.Add(new() { City = "Lagos", Description = "test", Lga = "", Name = "TestBranch", Number = "1" });

            CreateBulkResponse response = new();
            response.Total = 1;
            response.Success = 1;
            response.Errors.Add($"There is more than one row in the file with this branch name TestBranch.");

            var expected = new SuccessApiResponse<CreateBulkResponse>("", response);
          
            //Act
            var action = await service.BulkUpload(model);
           
            var result = (CreateBulkResponse)action.Data;
            //Asert
            Assert.Equal(response.Errors.Count, result.Errors.Count);
        }

        [Fact]
        public async Task ActivateBranch_StateUnderTest_UExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var id = Guid.Parse("18063564-c028-4543-b2df-e0c12dffb2ca");
            
            db.ChangeTracker.Clear();
            await Assert.ThrowsAsync<ValidationException>(() => service.ActivateBranch(id, TestData.GetCurrentUser()));
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task CheckNameUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var code = "HQ";
            var hqbranch = new Core.Entities.Branch("HQ", "HQ");

            var expected = new SuccessApiResponse<Core.Entities.Branch>("Branch found", hqbranch);
            // Act
            var result = await service.GetBranchByCode(code);
            // Assert
            Assert.Equivalent(expected.Msg, result.Msg);

        }

        [Fact]
        public async Task CheckDuplicateName_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var hqbranch = new Core.Entities.Branch("HQ", "HQ");
            var model = new ValidateNameModel() { Name = "HQ" };

            // Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.VerifyName(model));

        }

        [Fact]
        public async Task CheckDuplicateName_UnExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            var model = new ValidateNameModel() { Name = "BenHQ" };
            var expected = new SuccessApiResponse<string>("Address is available", "");
            //Act
            var result = await service.VerifyName(model);
            // Assert
            Assert.Equivalent(expected.Succeeded, result.Succeeded);

        }

        [Fact]
        public async Task CheckDuplicateAdressnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            var hqbranch = new Core.Entities.Branch("HQ", "HQ");
            var model = new ValidateBranchAddressModel("1", "Ibadan", "Lagos");

            var expected = new SuccessApiResponse<string>("Address is available", "");
            // Act
            var result = await service.VerifyAddress(model);
            // Assert
            Assert.Equivalent(expected.Succeeded, result.Succeeded);
        }


        [Fact]
        public async Task CheckDuplicateAdressnderTest_UnExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();

            var hqbranch = new Core.Entities.Branch("HQ", "HQ");
            var model = new ValidateBranchAddressModel("1", "Lagos", "Lagos");

            var expected = new SuccessApiResponse<string>("Address is available", "");
            // Act

            // Assert
            await Assert.ThrowsAsync<ValidationException>(() => service.VerifyAddress(model));
        }

        [Fact]
        public async Task UpdateBranch_Basic_Admin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            ModifyBranch model = new ModifyBranch();
            model.Description = "Description";
            model.Name = "Hq Update";
            model.Draft = false;
            model.Number = "12";
            model.StreetName = "Test street";
            model.State = "Lagos";
            model.City = "Lagos";
            model.Country = "Nigeria";
            model.Lga = "Lagos";
            model.PostalCode = "12345";
            BranchUser user = TestData.GetBasicUser();

            db.ChangeTracker.Clear();
            // Act
            var result = await service.Update(TestData.DefaultBranchId, model, user);


            // Assert
            var expected = new SuccessApiResponse<object>("Branch modification request submited", result.Data);
            Assert.Equal(result.Succeeded, expected.Succeeded);

        }

        [Fact]
        public async Task GetBranchRequests_with_filters_ExpectedBehavior()
        {
            // Arrange
            var service = this.CreateService();
            char[] statusin = new char[] { 'A', 'P' };
            string[] requestType = new string[] { "CREATE" };

            GetBranchFilter filter = new()
            {
                Page = 1,
                Page_Size = 2,
                Filter_by = "created_by_me",
                Start_Date = DateTime.UtcNow.AddDays(-1).ToUniversalTime(),
                End_Date = DateTime.UtcNow.AddDays(2),
                Status__In = statusin, 
                Q = "Create",

            };
            // Act
            var result = await service.GetBranches(
                filter,
                TestData.GetCurrentUser().UserId, TestData.GetCurrentUser().BranchId);

            // Assert

            Assert.Equal<int>(result.Count, 0);

        }

        [Fact]
        public async Task GetTemplate_ReturnsFileResult()
        {
            // Arrange
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            webHostEnvironmentMock.Setup(m => m.ContentRootPath).Returns("\"C:\\Users\\hp\\Documents\\Seabas\\retail-teams-management-api\\wwwroot\\data\\bulktemplate.csv");

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.SetupGet(c => c.RequestServices)
                           .Returns(Mock.Of<IServiceProvider>(s => s.GetService(typeof(IWebHostEnvironment)) == webHostEnvironmentMock.Object));

            var controller = new BranchController(null, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };

            // Act
            var result = await controller.GetTemplate();

            // Assert
            Assert.IsType<FileContentResult>(result);

            var fileResult = result as FileContentResult;
            Assert.NotNull(fileResult);
            Assert.Equal("bulktemplate.csv", fileResult.FileDownloadName);
            Assert.Equal("text/csv", fileResult.ContentType);
        }


    }




}
