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

namespace RW4OBDistributorProcessUnitTests
{
    public class OBDBHelperTests
    {

        //RWOBDistributorsEntities
        //RWServiceManagerEntities

        private ServiceProvider _serviceProvider;

        private string? sConnectString = "data source=172.16.201.61;initial catalog=RW3DBDEV;integrated security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";
        private IServiceScopeFactory scopeFactory;
        OBDBHelper dbhelper;
        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddDbContext<RWOBDistributorsEntities>(options => options.UseSqlServer(sConnectString));
            services.AddDbContext<RWServiceManagerEntities>(options => options.UseSqlServer(sConnectString));
            //services.AddScoped<IServiceScopeFactory, scop>
            _serviceProvider = services.BuildServiceProvider();
            scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            dbhelper = new OBDBHelper(scopeFactory);
        }

        [Test]
        public void GetListServiceThreadParamByIdTest()
        {


            List<R_SysServiceThreadParams> result = dbhelper.GetListServiceThreadParamById(421);
            Assert.AreEqual(4, result.Count);

        }

        [Test]
        public void GetSysParamValuesTest()
        {
            string CompanyName = "RAILINC";
            string SysParamCd = "EDIFileOBLoc";
            var result = dbhelper.GetSysParamValues(CompanyName, SysParamCd, 5);
            Assert.IsNotNull(result);

        }

        [Test]
        public void GetPreNoteUnitStatusByEventIdTest()
        {


            USP_GetPreNoteUnitStatusByEventId_Result result = dbhelper.GetPreNoteUnitStatusByEventId(8499517);
            Assert.IsNotNull(result);

        }
        [Test]
        public void GetActivePendingPrenotUnitTest()
        {


            List<Usp_GetActivePendingPrenoteUnit_Result> result = dbhelper.GetActivePendingPrenotUnit("TEMU942406");
            Assert.AreEqual(1, result.Count);

        }
        [Test]
        public void GetPrenoteDetailsTest()
        {


            List<USP_OBDist_GetPrenoteDetails_Result> result = dbhelper.GetPrenoteDetails(9422152);
            Assert.AreEqual(1, result.Count);

        }
        [Test]
        public void GetWamDataSubcriptionDetailsTest()
        {


            T_WAMDataSubscription result = dbhelper.GetWamDataSubcriptionDetails(1248187, 738345);
            Assert.IsNotNull(result);

        }
        [Test]
        public void GetWamSubsciptionApiTokenTest()
        {


            String result = dbhelper.GetWamSubsciptionApiToken(12);
            Assert.IsNotNull(result);

        }
        [Test]
        public void GetWAMApiParamsTest()
        {

            var paramCDs = new List<string> { "EDIUserId", "AppURL", "MOBAPPVer" };
            List<KeyValuePair<string, string>> result = dbhelper.GetWAMApiParams(paramCDs);
            Assert.IsNotNull(result);

        }
        [Test]
        public void UpdateWamAPITokenTest()
        {

            string token = "35e9476e-49c3-4712-b4ae-8b8b8968-1996";
            int paramId = 12;
            dbhelper.UpdateWamAPIToken(token, paramId);
            Assert.DoesNotThrow(() => dbhelper.UpdateWamAPIToken(token, paramId), "UpdateWamAPIToken should not throw an exception.");
        }
        [Test]
        public void UpdateWamDataSubscriptionETSDateTest()
        {

            long wamDataSubscriptionId = 125359;
            string status = "E";
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            dbhelper.UpdateWamDataSubscriptionETSDate(wamDataSubscriptionId, status, startDate, endDate);
            Assert.DoesNotThrow(() => dbhelper.UpdateWamDataSubscriptionETSDate(wamDataSubscriptionId, status, startDate, endDate), "UpdateWamDataSubscriptionETSDate should not throw an exception.");
        }
        [Test]
        public void UpdateWamDataSubscriptionForInactiveTest()
        {

            long wamDataSubscriptionId = 125359;
            string status = "N";

            dbhelper.UpdateWamDataSubscriptionForInactive(wamDataSubscriptionId, status);
            Assert.DoesNotThrow(() => dbhelper.UpdateWamDataSubscriptionForInactive(wamDataSubscriptionId, status), "UpdateWamDataSubscriptionForInactive should not throw an exception.");
        }
        [Test]
        public void UpdateWamDataSubscriptionTest()
        {

            long wamDataSubscriptionId = 125359;
            long subscriptionId = 125359;
            string status = "S";
            string subscriptionGuid = "63AB53C7-BDDA-4842-844F-A8CDF3CE95BC";

            dbhelper.UpdateWamDataSubscription(wamDataSubscriptionId, subscriptionId, status, subscriptionGuid);
            Assert.DoesNotThrow(() => dbhelper.UpdateWamDataSubscription(wamDataSubscriptionId, subscriptionId, status, subscriptionGuid), "UpdateWamDataSubscription should not throw an exception.");
        }
        [Test]
        public void UpdateParamByParamCdTest()
        {
            string Pvalue = "35e9476e-49c3-4712-b4ae-8b8b8968-1997";
            string ParamCd = "PTALLIANCEAPITOKEN";

            dbhelper.UpdateParamByParamCd(Pvalue, ParamCd);
            Assert.DoesNotThrow(() => dbhelper.UpdateParamByParamCd(Pvalue, ParamCd), "UpdateParamByParamCd should not throw an exception.");
        }
        [Test]
        public void InsertThreadExceptionsTest()
        {
            long threadId = 143;
            string exception = "Exception has been thrown by the target of an invocation.";
            DateTime createdDtTm = DateTime.Now;
            var result = dbhelper.InsertThreadExceptions(threadId, exception, createdDtTm);
            Assert.IsTrue(result);
        }
        [Test]
        public void IncrementWAmAPICurrentCountTest()
        {
            long threadId = 143;
            string exception = "Exception has been thrown by the target of an invocation.";
            DateTime createdDtTm = DateTime.Now;
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                int initialCount = dbContext.T_ThreadExceptionLog.Count();
                dbhelper.InsertThreadExceptions(threadId, exception, createdDtTm);
                int finalCount = dbContext.T_ThreadExceptionLog.Count();
                Assert.AreEqual(initialCount + 1, finalCount, "The record should have been inserted into the database.");
            }
        }
        [Test]
        public void CreateWamDataSubscriptionTest()
        {
            long preNoteId = 1248208;
            long unitMasterId = 738366;
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                int initialCount = dbContext.T_WAMDataSubscription.Count();
                dbhelper.CreateWamDataSubscription(preNoteId, unitMasterId, startDate, endDate);
                int finalCount = dbContext.T_WAMDataSubscription.Count();
                Assert.AreEqual(initialCount + 1, finalCount, "The record should have been inserted into the database.");
            }
        }
        [Test]
        public void DeriveRailincCERLogTest()
        {
            var lstRailincCER = new List<RW4OBDistributorProcess.RailincCER>
              {
               new RW4OBDistributorProcess.RailincCER { EventId = 794080326, FleetCd = "L", UnitNumber = "TEME936638", FileName = "CER11192023072940L.txt" }
               };
            bool result = dbhelper.DeriveRailincCERLog(lstRailincCER);
            Assert.IsTrue(result);
        }
        [TearDown]
        public void TearDown() { _serviceProvider.Dispose(); }
    }
}