using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RW4OBDistributorProcess;
namespace WamSubscriptionUpdDltUnitTests
{

    public class WamSubscriptionTests
    {
        private WAMDataSubscription wam;
        private Mock<IConfiguration> mockConfiguration;
        private Mock<ILogger<WAMDataSubscription>> mockLogger;
        private Mock<OBDBHelper> mockOBDBHelper;
        private ServiceProvider _serviceProvider;
        [SetUp]
        public void Setup()
        {
            mockConfiguration = new Mock<IConfiguration>();
            var services = new ServiceCollection();
            services.AddTransient<OBDBHelper>();
            _serviceProvider = services.BuildServiceProvider();
            // Setup mock IConfiguration to return specific values when requested
            mockConfiguration.Setup(config => config["appSettings:WamSubscriptionBaseUrl"]).Returns("https://api-stage.maersk.com/rcm/reefer-watch/");
            mockConfiguration.Setup(config => config["appSettings:WAMauthenticationURLRequestData"]).Returns("grant_type=client_credentials&client_id=GRQUplsN2IMNiq3GrBp1DJIdVlbGqseU&client_secret=ANeGdg2GIRy84L43&scope=openid");
            mockConfiguration.Setup(config => config["appSettings:WAMAPIExceedAlertTopic"]).Returns("AlertTopic");
            mockConfiguration.Setup(config => config["appSettings:WAMAPIExceedAlertSub"]).Returns("AlertSubscription");
            mockConfiguration.Setup(config => config["appSettings:WAMauthenticationURL"]).Returns("hhttps://api-stage.maersk.com/oauth2/access_token");
            mockConfiguration.Setup(config => config["appSettings:subsciptionaddedDays"]).Returns("35");
            mockConfiguration.Setup(config => config["appSettings:WamConsumerKey"]).Returns("foGHArSsjKaa34K4vrNRy1ehHSrzPefZ");
            mockConfiguration.Setup(config => config["appSettings:AzurePrimaryConnectionString"]).Returns("connectionString");
            mockConfiguration.Setup(config => config["appSettings:logpath"]).Returns("C:\\error\\");

            // Mock ILogger
            mockLogger = new Mock<ILogger<WAMDataSubscription>>();

            // Mock OBDBHelper (this is where your issue is, we mock its behavior)
            mockOBDBHelper = new Mock<OBDBHelper>();

            // Optionally, set up any methods on the OBDBHelper mock that are needed for the test
            //mockOBDBHelper.Setup(helper => helper.SomeMethod()).Returns(someValue);

            // Instantiate WAMDataSubscription with the mocked dependencies
            wam = new WAMDataSubscription(
                mockConfiguration.Object,
                mockLogger.Object,
                 _serviceProvider.GetRequiredService<OBDBHelper>()
            );
        }
        [Test]
        public void DeleteDataWAMSubscription_Test()
        {

            string jwtToken = "eyJ0eXAiOiJKV1QiLCJraWQiOiI4anlKRk93VFlWaFhYRDJIMS8wUEEwNGZLWUk9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJHUlFVcGxzTjJJTU5pcTNHckJwMURKSWRWbGJHcXNlVSIsImN0cyI6Ik9BVVRIMl9TVEFURUxFU1NfR1JBTlQiLCJhdWRpdFRyYWNraW5nSWQiOiI0MTY3ZmI3ZC1lMDdjLTQ3ZjctODk2My0yNzgyMDY5N2Q5YjMtMTgxNDg5NzQiLCJzdWJuYW1lIjoiR1JRVXBsc04ySU1OaXEzR3JCcDFESklkVmxiR3FzZVUiLCJpc3MiOiJodHRwczovL2lhbS1zdGFnZS5tYWVyc2suY29tL2FjbS9vYXV0aDIvbWF1IiwidG9rZW5OYW1lIjoiYWNjZXNzX3Rva2VuIiwidG9rZW5fdHlwZSI6IkJlYXJlciIsImF1dGhHcmFudElkIjoiMEV3b1FBOWNfMUNCVE1BYXJJaFJGQTNJYnM4IiwiYXVkIjoiR1JRVXBsc04ySU1OaXEzR3JCcDFESklkVmxiR3FzZVUiLCJuYmYiOjE3Mzk3NzM1NTEsImdyYW50X3R5cGUiOiJjbGllbnRfY3JlZGVudGlhbHMiLCJzY29wZSI6WyJvcGVuaWQiXSwiYXV0aF90aW1lIjoxNzM5NzczNTUxLCJyZWFsbSI6Ii9tYXUiLCJleHAiOjE3Mzk3ODA3NTEsImlhdCI6MTczOTc3MzU1MSwiZXhwaXJlc19pbiI6NzIwMCwianRpIjoiMGRGamJZODJ1TWF4QVZWcGs2c2t5bXQ2ZWkwIn0.UyZUJCWvIndFqEV8UZH7AVJy1z9Vl-N58cu5vrcaXvk1RAcSROyyTXcsL_v3pMf4yqw5lOVvisve9ertHwM7aiKPs7Wu2BCk8Ebj2LXKJ4CnZcB61y1hFjujKdeHhcfYnBKDDWyCJYsXz1jERAC8XpOrXg5uuo-ro3ToHV5FJxz1BUPfhP-E4yrzENT4bXXkfg5cJq7hx5cqTlmasNTxyBvSLAsZ0W-d0qrWexpfK1VOYqiBlhAaENdFMB43-MDUeQND2p3GFUmabPCP9EKtEDFYrlyQWQtrdKE3seWT5QSCtoOTtNXFmT7hzWkoXSWoxixYYyts_30vhqQL_zWl7w";
            RW4Entities.Models.RWOBDistributorsEntities.T_WAMDataSubscription rwSubscription = new RW4Entities.Models.RWOBDistributorsEntities.T_WAMDataSubscription
            {
                WamSubscriptionGuid = Guid.Parse("86EBDBFE-D8CB-459D-BA03-7F062933F198")//S
            };

            wam.DeleteDataWAMSubscription(rwSubscription, jwtToken, 421);

        }
        [Test]
        public void UpdateDataWAMSubscription_Test()
        {
            //wam.GetJwtToken(421);
            string jwtToken = "eyJ0eXAiOiJKV1QiLCJraWQiOiI4anlKRk93VFlWaFhYRDJIMS8wUEEwNGZLWUk9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJHUlFVcGxzTjJJTU5pcTNHckJwMURKSWRWbGJHcXNlVSIsImN0cyI6Ik9BVVRIMl9TVEFURUxFU1NfR1JBTlQiLCJhdWRpdFRyYWNraW5nSWQiOiI0MTY3ZmI3ZC1lMDdjLTQ3ZjctODk2My0yNzgyMDY5N2Q5YjMtMTgxNDg5NzQiLCJzdWJuYW1lIjoiR1JRVXBsc04ySU1OaXEzR3JCcDFESklkVmxiR3FzZVUiLCJpc3MiOiJodHRwczovL2lhbS1zdGFnZS5tYWVyc2suY29tL2FjbS9vYXV0aDIvbWF1IiwidG9rZW5OYW1lIjoiYWNjZXNzX3Rva2VuIiwidG9rZW5fdHlwZSI6IkJlYXJlciIsImF1dGhHcmFudElkIjoiMEV3b1FBOWNfMUNCVE1BYXJJaFJGQTNJYnM4IiwiYXVkIjoiR1JRVXBsc04ySU1OaXEzR3JCcDFESklkVmxiR3FzZVUiLCJuYmYiOjE3Mzk3NzM1NTEsImdyYW50X3R5cGUiOiJjbGllbnRfY3JlZGVudGlhbHMiLCJzY29wZSI6WyJvcGVuaWQiXSwiYXV0aF90aW1lIjoxNzM5NzczNTUxLCJyZWFsbSI6Ii9tYXUiLCJleHAiOjE3Mzk3ODA3NTEsImlhdCI6MTczOTc3MzU1MSwiZXhwaXJlc19pbiI6NzIwMCwianRpIjoiMGRGamJZODJ1TWF4QVZWcGs2c2t5bXQ2ZWkwIn0.UyZUJCWvIndFqEV8UZH7AVJy1z9Vl-N58cu5vrcaXvk1RAcSROyyTXcsL_v3pMf4yqw5lOVvisve9ertHwM7aiKPs7Wu2BCk8Ebj2LXKJ4CnZcB61y1hFjujKdeHhcfYnBKDDWyCJYsXz1jERAC8XpOrXg5uuo-ro3ToHV5FJxz1BUPfhP-E4yrzENT4bXXkfg5cJq7hx5cqTlmasNTxyBvSLAsZ0W-d0qrWexpfK1VOYqiBlhAaENdFMB43-MDUeQND2p3GFUmabPCP9EKtEDFYrlyQWQtrdKE3seWT5QSCtoOTtNXFmT7hzWkoXSWoxixYYyts_30vhqQL_zWl7w";
            RW4Entities.Models.RWOBDistributorsEntities.T_WAMDataSubscription rwSubscription = new RW4Entities.Models.RWOBDistributorsEntities.T_WAMDataSubscription
            {
                WAMDataSubscriptionId = 166988,
                PrenoteId = 1814769,
                UnitMasterId = 860616,
                StatusCd = "S",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                CreateDt = DateTime.UtcNow,
                ModifyDt = DateTime.UtcNow,
                APIStartDttm = "2025-02-13T21:35:37",
                WamSubscriptionGuid = Guid.Parse("1B12B1D3-ED0B-4B72-A64E-0D537AF3C0C4")//E
                //WamSubscriptionGuid = Guid.Parse("83044ACB-0518-4D76-AC54-B5E640FCC9A8")//S
            };
            wam.UpdateDataWAMSubscription(rwSubscription, jwtToken, 421);
        }

    }
}
