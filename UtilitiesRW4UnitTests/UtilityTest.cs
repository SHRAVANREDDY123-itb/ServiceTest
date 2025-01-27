using Microsoft.Extensions.Logging;
using NUnit.Framework.Internal;
using NUnit.Framework;

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
    }
}
