using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
using NUnit.Framework;
using RWUtilities.Common;

namespace UtilitiesRW4UnitTests
{
    public class UtilityTest
    {
        private static Microsoft.Extensions.Logging.ILogger _logger;
        [SetUp]
        public void Setup()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            _logger = loggerFactory.CreateLogger(typeof(RWUtilities.Common.Utility));
            RWUtilities.Common.Utility.connectionString = "Endpoint=sb://rwimswbsvcbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=JOzMcnZGgWaykW2pQxaBVVLVAzaqpAsIlwXr6AjTm4M=";
        }

        [Test]
        public void SendMail_WithValidaParameters_ReturnsEmptyString()
        {
            //Arrange
            string strFrom = "suma.dhara123@gmail.com";
            string strTo = "sdharwad@raytex.co.in";
            string strSubject = "Sample Subject";
            string strBody = "Sample Body";
            string strMailServerName = "172.16.200.33";

            //Act
            string result = RWUtilities.Common.Utility.SendMail(_logger, strFrom, strTo, strSubject, strBody, strMailServerName);

            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public async Task SendMessageToAzureSubscription_Test()
        {
            string SubscriptNm = "OBWAMSUBSQueue";
            string queuetopicName = "outboundprocess";

            string jsonString = "{\"TriggerCondition\":\"Event\",\"TriggerCd\":\"PNM\",\"TriggerID\":\"794091940\",\"TriggerDtTm\":\"22-05-2024 17:12:45\",\"TriggerMsgType\":\"PN\",\"PSP\":\"1\",\"Line\":\"94\",\"Unit\":\"MNBU3882114\",\"Json\":null}";


             await RWUtilities.Common.Utility.SendMessageToAzureSubscription(queuetopicName, SubscriptNm, jsonString, _logger);
        }

        [Test]
        public void GetQueueMessagefromOBSAzureSubscription()
        {
            for (int i = 0; i < 42; i++)
            {

                string SubscriptNm = "OBSTOPRAILINCTracQ";
                string queuetopicName = "outboundprocess";
                bool isbatchprocess = false;

                List<DistributorJson> messageList = RWUtilities.Common.Utility.GetQueueMessagefromOBSAzureSubscription(queuetopicName, SubscriptNm, isbatchprocess, 1, _logger).Result;

                if (messageList != null && messageList.Count > 0)
                {
                    foreach (DistributorJson json in messageList)
                    {
                        RWUtilities.Common.Utility.DeleteMessagebasedonProperties(queuetopicName, SubscriptNm, nameof(json.TriggerID), json.TriggerID, _logger).Wait();
                    }
                }

            }

        }

    }
}
