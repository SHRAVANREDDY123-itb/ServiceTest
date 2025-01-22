using RW4Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RW4Entities.Models.RWOBDistributorsEntities;
using RW4Entities.Models.RWServiceManagerEntities;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ServiceManagerRW4
{
    public class ServiceManagerDBHelper
    {
        RWServiceManagerEntities db;
        private readonly IServiceScopeFactory _scopeFactory;
        public ServiceManagerDBHelper(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public virtual DataSet GetServiceDefinition(string sSysService_Cd)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                return db.GetServiceDefinition(sSysService_Cd);
            }
        }

        public async Task<DataTable> GetThreadConfigurationAsync(long threadId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                return await db.GetThreadConfigurationAsync(threadId);
            }
        }

    }
}
