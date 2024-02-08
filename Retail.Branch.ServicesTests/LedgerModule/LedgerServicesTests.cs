using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Retail.Branch.Services.Common;
using Retail.Branch.Services.LedgerModule;
using Retail.Branch.Services.LedgerModule.Model;
using Retail.Branch.Services.Util;
using System.Net;
using Xunit;

namespace Retail.Branch.ServicesTests.LedgerModule
{
    public class LedgerServicesTests
    {
        private readonly MockRepository mockRepository;

        private readonly IHttpHelper mockHttpHelper;
        private readonly Mock<IConfiguration> mockConfiguration;
       
        public LedgerServicesTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockHttpHelper = new MockLegerHttpHelper();
            this.mockConfiguration = this.mockRepository.Create<IConfiguration>();
        }

        private LedgerServices CreateLedgerServices()
        {
            return new LedgerServices(
                this.mockHttpHelper,
                this.mockConfiguration.Object);
        }

        [Fact]
        public async Task GetAll_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var ledgerServices = this.CreateLedgerServices();
            LedgerListFilter filter = new() { Page = 1, Page_Size = 3, state = "OPEN", Search = null };
            string token = "token";

            var configurationSectionMock = new Mock<IConfigurationSection>();

            configurationSectionMock.Setup(x => x.Value)
               .Returns("https://retailcore-accounting-api.qa.bepeerless.co");

            this.mockConfiguration.Setup(c => c.GetSection("Endpoints:AccountsService")).Returns(configurationSectionMock.Object); ;
         

            // Act
            var result = await ledgerServices.GetAll(
               filter,
               token);

            // Assert
            Assert.Equivalent(result.Count, TestData.LedgerData().total);
            this.mockRepository.VerifyAll();
        }
    }
}
