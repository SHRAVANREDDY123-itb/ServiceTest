using DBConstants;
using Entities.RefData;
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
        public ServiceManager( ServiceThread serviceThread, ServiceDB serviceDB) {
           
            _serviceThread =serviceThread;
            _serviceDB = serviceDB;
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

            LogMessage("Service Invoked");          

            try
            {
                if (await LoadThreads(sServiceCode))
                {
                    LogMessage("Threads Loaded");

                }

            }
            catch (Exception oException)
            {
                Log1.write(oException);
                LogException(oException);
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

            LogMessage("Stopping Threads Attempt " + iAttemptNo);

            if (iAttemptNo > iMaxAttempts)
                return false;


            foreach (IServiceThread oServiceThread in this.ServiceThreads)
            {
                oServiceThread.sRequestedStatus = ServiceStatus.Stop;
                LogMessage("Thread " + oServiceThread.lThreadId + " requested to stop");

            }

            IEnumerable<IServiceThread> oRunningThreads = from IServiceThread oServiceThread in this.ServiceThreads where oServiceThread.sThreadStatus != ServiceStatus.Stop select oServiceThread;

            if (oRunningThreads.Count<IServiceThread>() <= 0)
            {
                LogMessage("All Threads Stoppped in Attempt " + iAttemptNo);

                return true;

            }
            else
            {

                Thread.Sleep(iResting_Tm * 1000);
                return StopThreads(iResting_Tm, iAttemptNo, iMaxAttempts);
            }


        }

        /// <summary>
        /// Method to Log Exception
        /// </summary>
        /// <param name="oException"></param>
        private void LogException(Exception oException)
        {
            LogMessage(oException.Message);
          
        }

        /// <summary>
        /// Method to Log Message
        /// </summary>
        /// <param name="sMessage"></param>
        private void LogMessage(string sMessage)
        {
           Log1.write(DateTime.Now.ToString("dd MMM yyyy HH:mm:ss") + ":Service:" + this.sServiceCode + "-" + sMessage);           
        }       
    

        #endregion
    }

    public static class Log1
    {
        private static object mlock = new object();

        static Log1()
        {
            if (!System.Diagnostics.EventLog.Exists("RW3"))
            {
                System.Diagnostics.EventLog.CreateEventSource("RW", "RW3");
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="ex">This is Exception object.(Default SourceName is RW3)</param>
        public static void write(Exception ex)
        {
            try
            {
                write("RW3", ex);
            }
            catch
            {
                ////ignore
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="Message">This is Any String Message.(Default SourceName is RW3)</param>
        public static void write(string Message)
        {
            try
            {
                write("RW3", Message);
            }
            catch
            {
                ////ignore
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="Source">This is Event Source Name.(Must exist)</param>
        /// <param name="ex">This is Exception object.</param>
        public static void write(string Source, Exception ex)
        {
            try
            {
                lock (mlock)
                {
                    System.Diagnostics.EventLog.WriteEntry(Source, ex.Message + "\\n" + ex.StackTrace + "\\n" + ex.InnerException);
                }
            }
            catch
            {
                ////ignore
            }
        }

        /// <summary>
        /// Write function
        /// </summary>
        /// <param name="Source">This is Event Source Name.(Must exist)</param>
        /// <param name="Message">This is Any String Message.</param>
        public static void write(string Source, string Message)
        {
            try
            {
                lock (mlock)
                {
                    System.Diagnostics.EventLog.WriteEntry(Source, Message);

                    //
                }
            }
            catch
            {
                ////ignore
            }
        }
    }
}
