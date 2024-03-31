using DBConstants;
using Entities.RefData;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Data;
using Utility.DataSetManagement;

namespace ServiceManagerRW4
{
    public class ServiceManager
    {
        private ConcurrentBag<IServiceThread> ServiceThreads = new ConcurrentBag<IServiceThread>();
       
        private ServiceThread _serviceThread;
        private ServiceDB _serviceDB;
        private ILogger _logger;
        public ServiceManager( ServiceThread serviceThread, ServiceDB serviceDB, ILogger<ServiceManager> logger) {
           
            _serviceThread =serviceThread;
            _serviceDB = serviceDB;
            _logger = logger;
        }
        #region " Properties "
        public string? sServiceCode { get; set; }
        #endregion

        #region " Methods "
        /// <summary>
        /// Invoke Service
        /// </summary>
        /// <param name="sServiceCode">Service Code passed from Service Launcher</param>
        /// <returns></returns>
        public async Task<bool> InvokeService(string sServiceCode)
        {

            this.sServiceCode = sServiceCode;

            _logger.LogInformation("Service Invoked");    

            try
            {
                if (await LoadThreads(sServiceCode))
                {
                    _logger.LogInformation("Threads Loaded");

                }

            }
            catch (Exception oException)
            {
                _logger.LogError(oException.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// To Load Threads 
        /// </summary>
        /// <param name="sServiceCode"></param>
        /// <returns></returns>
        private async Task<bool> LoadThreads(string sServiceCode)
        {
            ServiceDB oServiceDB = _serviceDB;
            DataSet dsServiceDefinition = oServiceDB.GetServiceDefinition(sServiceCode);

            // Service Configuration
            if (DataSetComponent.CheckRecordExist(dsServiceDefinition, RG_SysService.TableName))
            {
                          

                //Service Threads
                if (DataSetComponent.CheckRecordExist(dsServiceDefinition, RG_SysServiceThreads.TableName))
                {

                    this.ServiceThreads.Clear();
                    foreach (DataRow drSysServiceThread in dsServiceDefinition.Tables[RG_SysServiceThreads.TableName].Rows)
                    {
                        
                        long lSysServiceThread_Id = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.SysServiceThread_Id], (long)0);
                    
                        ServiceThread oServiceThread = _serviceThread;  
                        this.ServiceThreads.Add(oServiceThread);

                        await oServiceThread.InvokeThread(lSysServiceThread_Id);
                        
                    }

                    return true;
                }
            }

            return false;
        }

      

        /// <summary>
        /// To Stop Threads
        /// </summary>
        /// <param name="iResting_Tm"></param>
        /// <param name="iAttemptNo"></param>
        /// <param name="iMaxAttempts"></param>
        /// <returns></returns>
        private bool StopThreads(int iResting_Tm, int iAttemptNo, int iMaxAttempts)
        {
            iAttemptNo++;

            _logger.LogInformation("Stopping Threads Attempt " + iAttemptNo);

            if (iAttemptNo > iMaxAttempts)
                return false;


            foreach (IServiceThread oServiceThread in this.ServiceThreads)
            {
                oServiceThread.sRequestedStatus = ServiceStatus.Stop;
                _logger.LogInformation("Thread " + oServiceThread.lThreadId + " requested to stop");

            }

            IEnumerable<IServiceThread> oRunningThreads = from IServiceThread oServiceThread in this.ServiceThreads where oServiceThread.sThreadStatus != ServiceStatus.Stop select oServiceThread;

            if (oRunningThreads.Count<IServiceThread>() <= 0)
            {
                _logger.LogInformation("All Threads Stoppped in Attempt " + iAttemptNo);

                return true;

            }
            else
            {

                Thread.Sleep(iResting_Tm * 1000);
                return StopThreads(iResting_Tm, iAttemptNo, iMaxAttempts);
            }


        }

       

        #endregion
    }

   
}
