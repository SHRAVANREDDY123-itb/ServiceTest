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
using RW4Entities.Models;
using System.Threading;

namespace ServiceManagerRW4
{
    public class ServiceManagerDBHelper
    {
       
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

        public async Task<DataTable> GetThreadConfigurationAsync(long threadId, CancellationToken cancellationToken)
        {
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                return await db.GetThreadConfigurationAsync(threadId, cancellationToken);
            }
        }

        public async Task< bool> UpdateServiceThreadAsync(R_SysServiceThreads updatedThread, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                 var existingThread= db.R_SysServiceThreads.FirstOrDefault(t=>t.SysServiceThreadId == updatedThread.SysServiceThreadId);
                if (existingThread==null)
                {
                    return false;
                }
                existingThread.CurrentStatusCd = updatedThread.CurrentStatusCd;
                existingThread.RequestedStatusCd = updatedThread.RequestedStatusCd;
                existingThread.LastStartedDtTm = updatedThread.LastStartedDtTm;
                existingThread.LastStoppedDtTm = updatedThread.LastStoppedDtTm;
                existingThread.CurrentProcessingStartDtTm = updatedThread.CurrentProcessingStartDtTm;
                existingThread.ReLoadFlg = updatedThread.ReLoadFlg;
                existingThread.IsSuccesful = updatedThread.IsSuccesful;
                await db.SaveChangesAsync(cancellationToken);
                return true;
            }
          
        }

        public async  Task<bool> InsertThreadException(T_ThreadExceptionLog threadException, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                db.T_ThreadExceptionLog.Add(threadException);
                await db.SaveChangesAsync(cancellationToken);
                return true;
            }

           
        }



    }
}
