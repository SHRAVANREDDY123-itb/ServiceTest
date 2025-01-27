using RW4Entities;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using RW4Entities.Models;
using ServiceManagerRW4.Models;
using Microsoft.Azure.Amqp.Framing;

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
        public SysServiceThread GetActiveThreadDetails(long threadId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    R_SysServiceThreads thread = db.R_SysServiceThreads
                                   .Where(x => x.SysServiceThreadId == threadId && x.IsActive == "Y")
                                   .FirstOrDefault();


                    if (thread == null)
                    {
                        throw new InvalidOperationException($"No active thread found with ID {threadId}.");
                    }

                    return new SysServiceThread
                    {
                        SysServiceThreadId = thread.SysServiceThreadId,
                        ServiceId = thread.SysServiceId,
                        CurrentStatusCode = thread.CurrentStatusCd,
                        RequestedStatusCode = thread.RequestedStatusCd,
                        LastStarted = thread.LastStartedDtTm,
                        LastStopped = thread.LastStoppedDtTm,
                        CurrentProcessingStart = thread.CurrentProcessingStartDtTm,
                        SleepTime = thread.ThreadSleepTm,
                        ReloadFlag = thread.ReLoadFlg,
                        AssemblyFullName = thread.MethodNm,
                        IsActive = thread.IsActive,
                        RetryCount = thread.Retries,
                        ThreadType = thread.ThreadType,
                        TaskTime = thread.TaskTm,
                        IsSuccessful = thread.IsSuccesful
                    };


                }
            }
            catch (Exception)
            {

                throw;
            }
        }     

        public async Task< bool> UpdateServiceThreadAsync(SysServiceThread updatedThread, CancellationToken cancellationToken)
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
                    existingThread.CurrentStatusCd = updatedThread.CurrentStatusCode;
                    existingThread.RequestedStatusCd = updatedThread.RequestedStatusCode;
                    existingThread.LastStartedDtTm = updatedThread.LastStarted;
                    existingThread.LastStoppedDtTm = updatedThread.LastStopped;
                    existingThread.CurrentProcessingStartDtTm = updatedThread.CurrentProcessingStart;                    
                    existingThread.IsSuccesful = updatedThread.IsSuccessful;
                    existingThread.ReLoadFlg = updatedThread.ReloadFlag;
                    await db.SaveChangesAsync(cancellationToken);
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        public async  Task<bool> InsertThreadException(ThreadExceptionLog threadException, CancellationToken cancellationToken)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    db.T_ThreadExceptionLog.Add(new T_ThreadExceptionLog() { SysServiceThreadId=threadException.SysServiceThreadId, CreateDtTm=threadException.CreateDtTm, ThreadException=threadException.ThreadException});
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
