using RW4Entities;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using RW4Entities.Models;
using ServiceManagerRW4.Models;

namespace ServiceManagerRW4
{
    public class ServiceManagerDBHelper
    {
       
        private readonly IServiceScopeFactory _scopeFactory;
        public ServiceManagerDBHelper(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public bool CheckServiceAndThreadsExists(string sSysService_Cd)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();

                    var service = db.R_SysService.FirstOrDefault(x => x.SysServiceCd == sSysService_Cd);
                    if (service != null)
                    {
                        var threadcount = db.R_SysServiceThreads.Where(x => x.SysServiceId == service.SysServiceId && x.IsActive == "Y").Count();
                        if (threadcount > 0)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<long> GetActiveThreads(string sSysService_Cd)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    var service = db.R_SysService.FirstOrDefault(x => x.SysServiceCd == sSysService_Cd);
                    var threads = db.R_SysServiceThreads.Where(x => x.SysServiceId == service.SysServiceId && x.IsActive == "Y").ToList<R_SysServiceThreads>().Select(x => x.SysServiceThreadId);
                    return threads.ToList();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public SyServiceThreadConfiguration GetActiveThreadDetails(long threadId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    var thread = db.R_SysServiceThreads
                                   .Where(x => x.SysServiceThreadId == threadId && x.IsActive == "Y")
                                   .FirstOrDefault();


                    if (thread == null)
                    {
                        throw new InvalidOperationException($"No active thread found with ID {threadId}.");
                    }

                    return new SyServiceThreadConfiguration() { threadID = thread.SysServiceThreadId, AssemblyFullName = thread.MethodNm, ThreadSleepTm = (int)thread.ThreadSleepTm };

                }
            }
            catch (Exception)
            {

                throw;
            }
        }     

        public async Task< bool> UpdateServiceThreadAsync(R_SysServiceThreads updatedThread, CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    var existingThread = db.R_SysServiceThreads.FirstOrDefault(t => t.SysServiceThreadId == updatedThread.SysServiceThreadId);
                    if (existingThread == null)
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
            catch (Exception)
            {

                throw;
            }
          
        }

        public async  Task<bool> InsertThreadException(T_ThreadExceptionLog threadException, CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    db.T_ThreadExceptionLog.Add(threadException);
                    await db.SaveChangesAsync(cancellationToken);
                    return true;
                }

            }
            catch (Exception)
            {

                throw;
            }
           
        }



    }
}
