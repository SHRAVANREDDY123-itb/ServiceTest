using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RW4Entities;
using RW4Entities.Models.RWServiceManagerEntities;
using RW4OBDistributorProcess;

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
            var services=new ServiceCollection();

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


        [TearDown]
        public void TearDown() { _serviceProvider.Dispose(); }
    }
}