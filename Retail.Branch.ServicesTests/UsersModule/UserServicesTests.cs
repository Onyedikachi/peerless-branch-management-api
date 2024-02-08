using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.LedgerModule.Model;
using Retail.Branch.Services.UsersModule;
using Retail.Branch.Services.UsersModule.Models;
using Retail.Branch.Services.Util;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Retail.Branch.ServicesTests.UsersModule
{
    public class UserServicesTests
    {
        private MockRepository mockRepository;

        private Mock<ILogger<UserServices>> mockLogger;
        private readonly IHttpHelper mockHttpHelper;
        private Mock<IConfiguration> mockConfiguration;

        public UserServicesTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockLogger = this.mockRepository.Create<ILogger<UserServices>>();
            this.mockHttpHelper = new MockUsersListHttpHelper();
            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();
        }

        private UserServices CreateUserServices()
        {
            return new UserServices(
                this.mockLogger.Object,
                this.mockHttpHelper,
                this.mockConfiguration.Object);
        }

        [Fact]
        public async Task GetUsers_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var userServices = this.CreateUserServices();
            
            UserListFilter filter = new() {  Page=1, Page_Size=2, BranchId=TestData.DefaultBranchId.ToString(),Search=null};
            string token = "all";

            var configurationSectionMock = new Mock<IConfigurationSection>();

            configurationSectionMock
               .Setup(x => x.Value)
               .Returns("https://retailcore-auth-api.qa.bepeerless.co");

            this.mockConfiguration.Setup(c => c.GetSection("Endpoints:UsersService")).Returns(configurationSectionMock.Object); ;

            var data = Task.FromResult(new PagedResponse<List<UserInfoModel>>("", TestData.GetUsers(), 1, 1, 1));
          //  mockHttpHelper.Setup(s => s.GetData<PagedResponse<List<UserInfoModel>>>("", token)).Returns(data);


            // Act
            var result = await userServices.GetUsers(
                filter,
                token);

            
            // Assert
            Assert.Equivalent(result.Count, 0);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public async Task GetUser_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var userServices = this.CreateUserServices();
            UserListFilter filter = new() { Page = 1, Page_Size = 2, BranchId = TestData.DefaultBranchId.ToString(), Search = null };
            string token = "testtoken";
            Guid id=Guid.NewGuid();
            var configurationSectionMock = new Mock<IConfigurationSection>();

            configurationSectionMock
               .Setup(x => x.Value)
               .Returns("https://retailcore-auth-api.qa.bepeerless.co");

            this.mockConfiguration.Setup(c => c.GetSection("Endpoints:UsersService")).Returns(configurationSectionMock.Object); ;

            var data = Task.FromResult(TestData.GetUser());
           // mockHttpHelper.Setup(s => s.GetData<UserInfoModel>("", token)).Returns(data);


            // Act
            var result = await userServices.GetUser(id,token);

            // Assert
            // Assert
            Assert.Equivalent(result.username, TestData.GetUser().username);
            this.mockRepository.VerifyAll();
        }
    }
}
