using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using RW4Entities;
using RW4Entities.DBConstants;
using RW4Entities.Models.RWOBDistributorsEntities;
using RW4Entities.Models.RWServiceManagerEntities;
using RW4OBDistributorProcess;
using System.Threading;
using System;
using static System.Formats.Asn1.AsnWriter;
using RW4BNSFTelemetry;

namespace RW4OBDistributorProcessUnitTests
{
    public class BNSFWAMTelemetry
    {

        //RWOBDistributorsEntities
        //RWServiceManagerEntities

        private ServiceProvider _serviceProvider;

        private string? sConnectString = "data source=172.16.201.61;initial catalog=RW3DBDEV;integrated security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
        private IServiceScopeFactory scopeFactory;
        DBHelper dbhelper;
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<RWBNSFTelemetryEntities>(options => options.UseSqlServer(sConnectString));
            services.AddDbContext<RWServiceManagerEntities>(options => options.UseSqlServer(sConnectString));
            
            _serviceProvider = services.BuildServiceProvider();
            scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            dbhelper = new DBHelper(scopeFactory);
        }

        [Test]
        public void InsertThreadExceptionsTest()
        {


           bool result = dbhelper.InsertThreadExceptions(366, "Something is wrong", DateTime.Now);
            Assert.IsTrue( result);

        }

        [Test]
        public void GetThreadSleepTimeTest()
        {


            int? result = dbhelper.GetThreadSleepTime(366);
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetWamsubscriptionIdsByUnitsTest()
        {


            var result = dbhelper.GetWamsubscriptionIdsByUnits();
            Assert.AreEqual(1, result.Count);

        }
        [Test]
        public void UpdatePramDataTest()
        {

            string token = "eyJ0eXAiOiJKV1QiLCJraWQiOiJHRkFMUWtWTzFvNFc3YXpweWJ6RjhrOE4wdEk9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJRalRZZnRLSTBtakd5SDNvS2sxYTlMVXF3TXROTXJzSSIsImN0cyI6Ik9BVVRIMl9TVEFURUxFU1NfR1JBTlQiLCJhdWRpdFRyYWNraW5nSWQiOiIxYmYxMmY1NC1jZTc2LTQzZTQtOTUzMC00NmRhOTUwM2JlOGUtNDkyMzQxOTg2Iiwic3VibmFtZSI6IlFqVFlmdEtJMG1qR3lIM29LazFhOUxVcXdNdE5NcnNJIiwiaXNzIjoiaHR0cHM6Ly9pYW0ubWFlcnNrLmNvbS9hY20vb2F1dGgyL21hdSIsInRva2VuTmFtZSI6ImFjY2Vzc190b2tlbiIsInRva2VuX3R5cGUiOiJCZWFyZXIiLCJhdXRoR3JhbnRJZCI6IkY2Q01YU3JtLWo0MTIxTDNWQnNlSjVZdkVSRSIsImF1ZCI6IlFqVFlmdEtJMG1qR3lIM29LazFhOUxVcXdNdE5NcnNJIiwibmJmIjoxNzM5MDc0NTM3LCJncmFudF90eXBlIjoiY2xpZW50X2NyZWRlbnRpYWxzIiwic2NvcGUiOlsib3BlbmlkIl0sImF1dGhfdGltZSI6MTczOTA3NDUzNywicmVhbG0iOiIvbWF1IiwiZXhwIjoxNzM5MDgxNzM3LCJpYXQiOjE3MzkwNzQ1MzcsImV4cGlyZXNfaW4iOjcyMDAsImp0aSI6InNTUTF6cHF3alhtMEQ5RldQb0ZySkN4VkhtdyJ9.F-LuP5Btygze8N1anAwEws6ycwqxSKNJLb3bV9Md9LeFfn08n8XYCLo_1HE5W4k2jhDiQtM5C28k_p3Ib6dxsDW__SE7IuRGyTA6r9w9JoK628rPXe3VDM2pP2Z56HazoSxkolEwFKQIXKwukUj7Z_1OjINRmm1FmIg_UR7tXy-2i2k4pxa6kllKNf6EPrKdQrF_92h0xcezrb5kLugJjzVyUvkgTYcucZYbbevAp8Lp9DOttCNVlf0VDD_3b1R1UrPEix57pcE6rWd1hkUIhckdbs8uDKlfAN-lLZpBG3yoUGVVfEiZU-6xYMSFFLDF5TSHAoOJ164ozSiCYd9MDw";
            int paramid = 11;
            dbhelper.UpdatePramData(token,paramid);
            Assert.IsTrue(true);

        }
        [Test]
        public void GetParamDataTest()
        {


            var result = dbhelper.GetParamData(11);
            string token = "eyJ0eXAiOiJKV1QiLCJraWQiOiJHRkFMUWtWTzFvNFc3YXpweWJ6RjhrOE4wdEk9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJRalRZZnRLSTBtakd5SDNvS2sxYTlMVXF3TXROTXJzSSIsImN0cyI6Ik9BVVRIMl9TVEFURUxFU1NfR1JBTlQiLCJhdWRpdFRyYWNraW5nSWQiOiIxYmYxMmY1NC1jZTc2LTQzZTQtOTUzMC00NmRhOTUwM2JlOGUtNDkyMzQxOTg2Iiwic3VibmFtZSI6IlFqVFlmdEtJMG1qR3lIM29LazFhOUxVcXdNdE5NcnNJIiwiaXNzIjoiaHR0cHM6Ly9pYW0ubWFlcnNrLmNvbS9hY20vb2F1dGgyL21hdSIsInRva2VuTmFtZSI6ImFjY2Vzc190b2tlbiIsInRva2VuX3R5cGUiOiJCZWFyZXIiLCJhdXRoR3JhbnRJZCI6IkY2Q01YU3JtLWo0MTIxTDNWQnNlSjVZdkVSRSIsImF1ZCI6IlFqVFlmdEtJMG1qR3lIM29LazFhOUxVcXdNdE5NcnNJIiwibmJmIjoxNzM5MDc0NTM3LCJncmFudF90eXBlIjoiY2xpZW50X2NyZWRlbnRpYWxzIiwic2NvcGUiOlsib3BlbmlkIl0sImF1dGhfdGltZSI6MTczOTA3NDUzNywicmVhbG0iOiIvbWF1IiwiZXhwIjoxNzM5MDgxNzM3LCJpYXQiOjE3MzkwNzQ1MzcsImV4cGlyZXNfaW4iOjcyMDAsImp0aSI6InNTUTF6cHF3alhtMEQ5RldQb0ZySkN4VkhtdyJ9.F-LuP5Btygze8N1anAwEws6ycwqxSKNJLb3bV9Md9LeFfn08n8XYCLo_1HE5W4k2jhDiQtM5C28k_p3Ib6dxsDW__SE7IuRGyTA6r9w9JoK628rPXe3VDM2pP2Z56HazoSxkolEwFKQIXKwukUj7Z_1OjINRmm1FmIg_UR7tXy-2i2k4pxa6kllKNf6EPrKdQrF_92h0xcezrb5kLugJjzVyUvkgTYcucZYbbevAp8Lp9DOttCNVlf0VDD_3b1R1UrPEix57pcE6rWd1hkUIhckdbs8uDKlfAN-lLZpBG3yoUGVVfEiZU-6xYMSFFLDF5TSHAoOJ164ozSiCYd9MDw";
            Assert.AreEqual(token, result);

        }

        [Test]
        public void UpdateParamByParamCdTest()
        {

           // sqlDb.UpdateParamByParamCd("0", WAMAuthAPICallCurrentCount);
           // sqlDb.UpdateParamByParamCd(DateTime.Now.ToString(), WAMAuthAPICallCurrentCountLstUpdTime);
            dbhelper.UpdateParamByParamCd("0","WAMAuthAPICallCurrentCount");
            dbhelper.UpdateParamByParamCd(DateTime.Now.ToString(), "WAMAuthAPICallCurrentCountLstUpdTime");
            

        }

        [Test]
        public void UpdateWamSubscriptionForAPIStartDtTest()
        {

            // sqlDb.UpdateParamByParamCd("0", WAMAuthAPICallCurrentCount);
            // sqlDb.UpdateParamByParamCd(DateTime.Now.ToString(), WAMAuthAPICallCurrentCountLstUpdTime);
            Guid myGuid = new Guid("D869A26E-DEBD-41EC-B9D1-5D5194101C49");
            dbhelper.UpdateWamSubscriptionForAPIStartDt(myGuid, 1248208, 738366, DateTime.Now.ToString());
          


        }


        [Test]
        public void GetWAMApiParamsTests()
        {

            List<string> Paramsrings = new List<string>() { "WAMAuthAPICallCurrentCount", "WAMAuthAPICallMaxCount", "WAMAuthAPICallCurrentCountLstUpdTime" };
         var result=    dbhelper.GetWAMApiParams(Paramsrings);
            dbhelper.IncrementWAmAPICurrentCount("WAMAuthAPICallCurrentCount");



        }

        //GetWAMApiParams

        [TearDown]
        public void TearDown() { _serviceProvider.Dispose(); }
    }
}
