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

        public void TempQuery()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();

                 //List<R_SysServiceThreads> r_SysServiceThreads=  db.R_SysServiceThreads.Where(x=>x.SysServiceThreadId==419 || x.SysServiceThreadId == 420).ToList();
                 //   r_SysServiceThreads.ForEach(x => x.IsActive = "N");
                 //   db.SaveChanges();

                    db.R_ApplicationParam.Where(x=>x.ParamID==27).FirstOrDefault().Value="1";
                    db.R_ApplicationParam.Where(x => x.ParamID == 11).FirstOrDefault().Value = "eyJ0eXAiOiJKV1QiLCJraWQiOiJHRkFMUWtWTzFvNFc3YXpweWJ6RjhrOE4wdEk9IiwiYWxnIjoiUlMyNTYifQ.eyJzdWIiOiJRalRZZnRLSTBtakd5SDNvS2sxYTlMVXF3TXROTXJzSSIsImN0cyI6Ik9BVVRIMl9TVEFURUxFU1NfR1JBTlQiLCJhdWRpdFRyYWNraW5nSWQiOiI4YzU0ZmExMC00ODllLTQ5ZWQtYmRmNS0wMDE0ODhjY2I3MWItNDYwNDk3NDIiLCJzdWJuYW1lIjoiUWpUWWZ0S0kwbWpHeUgzb0trMWE5TFVxd010Tk1yc0kiLCJpc3MiOiJodHRwczovL2lhbS5tYWVyc2suY29tL2FjbS9vYXV0aDIvbWF1IiwidG9rZW5OYW1lIjoiYWNjZXNzX3Rva2VuIiwidG9rZW5fdHlwZSI6IkJlYXJlciIsImF1dGhHcmFudElkIjoiZGxNU28ydG5MdThlVW1rV2h1QzhDR1hWZ2xzIiwiYXVkIjoiUWpUWWZ0S0kwbWpHeUgzb0trMWE5TFVxd010Tk1yc0kiLCJuYmYiOjE3MzgxMjkxNDksImdyYW50X3R5cGUiOiJjbGllbnRfY3JlZGVudGlhbHMiLCJzY29wZSI6WyJvcGVuaWQiXSwiYXV0aF90aW1lIjoxNzM4MTI5MTQ5LCJyZWFsbSI6Ii9tYXUiLCJleHAiOjE3MzgxMzYzNDksImlhdCI6MTczODEyOTE0OSwiZXhwaXJlc19pbiI6NzIwMCwianRpIjoiN2NjQy1sSGtRVDljNVdpZDRTQzlRWVR2RUtNIn0.EwNj-C_Bn31FRlSlEoSWj3WkY3qQmJ0tG770Vxkq0Ktuq6c9BjBSytv66vH_anr-JZXDqFS1ec9e_7P3DwOFD322Z2QYbCn6G_6HYL3iLEYF0WjAXn4tDatyqAsgFdvxONMDy7mRWbUYrLwmedH-MXL6CfK8mtnMiI7vdzbHRobw1rSwsIB2ViC4hlrSvrqIbCr81gD6ERcQrQwnIVHcagbcz__fVIHTnF_nzxKHgC_HkxnxIkpgpVDr7p_4nmdYO5uznwwCAj359OgYgjxvkc58fW-5CBo8Wccn4-zb47L5b-bankSXsPwnqFEf-1d3lpXxJ4-fVxKnrYH7msG7-g";
                    db.SaveChanges();

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
