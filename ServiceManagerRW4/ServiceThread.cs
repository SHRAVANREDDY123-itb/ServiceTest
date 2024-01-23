using DBConstants;
using Entities.RefData;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utility.DataSetManagement;

namespace ServiceManagerRW4
{
    public class ServiceThread : IServiceThread
    {
        #region " Properties "

        private long _lThreadId;

        public long lThreadId
        {
            get { return _lThreadId; }
            set { _lThreadId = value; }
        }

        private string _sThreadStatus = string.Empty;

        public string sThreadStatus
        {
            get { return _sThreadStatus; }
            set { _sThreadStatus = value; }
        }


        private string _sRequestedStatus = string.Empty;

        public string sRequestedStatus
        {
            get { return _sRequestedStatus; }
            set { _sRequestedStatus = value; }
        }


        #endregion

        #region " Definitions  "

        private DataTable dtSysServiceThreads = null;

       

        private readonly string? sAssemblyPath ;
        
        private ServiceDB _serviceDB;

        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        public ServiceThread(IConfiguration configuration, IServiceProvider serviceProvider, ServiceDB serviceDB)
        {
            _serviceProvider=serviceProvider;
            _configuration = configuration;
            sAssemblyPath = _configuration["appSettings:AssemblyPath"];
           _serviceDB=serviceDB;
        }

       

        #endregion

        #region " Methods "

        /// <summary>
        /// Method To Execute Business Method
        /// Class intended to be called should be in namespace MTLibrary
        /// </summary>
        /// <param name="sMethod_Nm">AssemblyName.ClassName.Method</param>
        /// <returns>true if Success else false</returns>
        private bool DoProcess(string sMethod_Nm)
        {
            try
            {
                string sAssembly_Nm = string.Empty;
                string sClassName = string.Empty;
                string sMethodName = string.Empty;

                string[] sParams = sMethod_Nm.Split(new char[] { '.' });

                if (sParams.Length > 2)
                {
                    sAssembly_Nm = sParams[0];
                    sClassName = sParams[1];
                    sMethodName = sParams[2];
                }
                else
                {
                    return false;
                }

                MethodInfo oMethod = null;
                Assembly Assembly = Assembly.LoadFrom(sAssemblyPath + sAssembly_Nm + ".dll");
                //Type Type = Assembly.GetType("RWProcessEDI." + sClassName);
                Type Type = Assembly.GetType(sAssembly_Nm + "." + sClassName);

                //.net core change, class type constructor is passed with configuration
                ConstructorInfo constructor = Type.GetConstructor(new[] { typeof(IConfiguration), typeof(IServiceProvider) });
                object cls = constructor.Invoke(new object[] { _configuration, _serviceProvider });

                // Activator.CreateInstance(Type);
                oMethod = cls.GetType().GetMethod(sMethodName);

                //# Call according to the signature
                object bSuccess = oMethod.Invoke(cls, new object[] { this.lThreadId });              

                if (bSuccess != null && bSuccess.GetType() == typeof(bool))
                {
                    return (bool)bSuccess;
                }

                return true;

            }
            catch (Exception oException)
            {
                Log1.write(oException.Message + "Inner Exception:" + oException.InnerException + "Source:" + oException.Source + "Trace :" + oException.StackTrace);
                LogException(oException);
                return false;

            }

        }

        /// <summary>
        /// Method to Update Thread Status 
        /// </summary>
        /// <param name="drSysServiceThread"></param>
        /// <returns></returns>
        private bool UpdateServiceThread(DataRow drSysServiceThread)
        {
            bool bSuccess = false;
            
            try
            {
                ServiceDB oServiceDB = _serviceDB;
                bSuccess = oServiceDB.UpdateServiceThread(drSysServiceThread);
            }
            catch (Exception oException)
            { 
                LogException(oException);
            }

            return bSuccess;
        }

        /// <summary>
        /// Method to Log Exception
        /// </summary>
        /// <param name="oException"></param>
        private void LogException(Exception oException)
        {
            LogMessage(oException.Message);
            LogExceptionToDB(oException);
        }

        /// <summary>
        /// Method to Log Message to Database
        /// </summary>
        /// <param name="oException"></param>
        private void LogExceptionToDB(Exception oException)
        {
            try
            {
                ServiceDB oServiceDB = _serviceDB;
                oServiceDB.InsertThreadExceptionLog(this.lThreadId, oException.Message, DateTime.Now);
            }
            catch (Exception oEx)
            {
                LogMessage(oEx.Message);

            }

        }

        /// <summary>
        /// Method to Log Message to Log Target(File/Console)
        /// </summary>
        /// <param name="sMessage"></param>
        private void LogMessage(string sMessage)
        {
            try
            {
                
                    Log1.write("-Thread:" + this.lThreadId + ":" + sMessage);

                

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// To Invoke Job Daily
        /// </summary>
        /// <param name="lSysServiceThread_Id"></param>
        public async Task InvokeThread(long lSysServiceThread_Id)
        {


            ServiceDB oServiceDB = _serviceDB;
            this.lThreadId = lSysServiceThread_Id;
            LogMessage("Job Invoked");

            try
            {
                bool bKeepAlive = true;
                int iThreadSleepTime = 0;
                DataRow drSysServiceThread = null;
                string sReLoad_Flg = Entities.RefData.Flag.YES;
                string sMethod_Nm = string.Empty;
                string sRequestedStatus = string.Empty;
                string sTask_Tm = "";
                DateTime dtTmLastStopped_DtTm = DateTime.MaxValue;
                string sThreadType = string.Empty;
                int iRetries = 1;

                dtSysServiceThreads = oServiceDB.GetThreadConfiguration(this.lThreadId);

                if (DataSetComponent.CheckRecordExist(dtSysServiceThreads))
                {
                    drSysServiceThread = dtSysServiceThreads.Rows[0];
                    iThreadSleepTime = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.ThreadSleep_Tm], (int)0);
                    sMethod_Nm = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.Method_Nm], string.Empty);
                    sRequestedStatus = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.RequestedStatus_Cd], string.Empty);
                    sTask_Tm = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.Task_Tm], string.Empty);
                    sThreadType = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.ThreadType], string.Empty);
                    iRetries = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.Retries], (int)0);
                    dtTmLastStopped_DtTm = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.LastStopped_DtTm], DateTime.MaxValue);
                }


                DateTime dtTask_DtTm = DateTime.MaxValue;
                CultureInfo provider = CultureInfo.InvariantCulture;
                string Format = "MM/dd/yyyy HH:mm";

                if (sTask_Tm != string.Empty && sThreadType == "J")
                {
                    dtTask_DtTm = DateTime.ParseExact(DateTime.Now.Date.ToString("MM/dd/yyyy") + " " + sTask_Tm, Format, provider);
                }


                while (bKeepAlive)
                {
                    try
                    {
                        if ((DateTime.Now > dtTask_DtTm && dtTask_DtTm.Date > dtTmLastStopped_DtTm.Date) || sThreadType != "J")
                        {

                            this.sThreadStatus = ServiceStatus.Running;
                            //# Update The Thread Current Start Time
                            drSysServiceThread[RG_SysServiceThreads.CurrentProcessingStart_DtTm] = DateTime.Now;
                            drSysServiceThread[RG_SysServiceThreads.CurrentStatus_Cd] = ServiceStatus.Running;
                            UpdateServiceThread(drSysServiceThread);
                            //# Invoke The Busniess Method
                            bool bSuccess = false;

                            int i = 0;
                            do
                            {
                                i++;


                                if (i > 1)
                                {// # Interval between Retries
                                 //  Thread.Sleep((iThreadSleepTime * 1000));
                                    await Task.Delay((iThreadSleepTime * 1000));
                                }


                                //   LogMessage("Try "+ i);
                                //   LogMessage("Begin DoProcess");
                                bSuccess = DoProcess(sMethod_Nm);
                                //   LogMessage("End DoProcess:Success:" + bSuccess);
                                this.sThreadStatus = ServiceStatus.Sleep;


                            } while (!bSuccess && i <= (iRetries - 1) && sThreadType == "J");// Retry specific no of times for Daily Job At Interval of Thread Sleep Time

                            //# Update the Thread Current, Last Current Time and Last Stopped Time
                            drSysServiceThread[RG_SysServiceThreads.IsSuccesful] = bSuccess == true ? Flag.YES : Flag.NO;
                            drSysServiceThread[RG_SysServiceThreads.LastStarted_DtTm] = drSysServiceThread[RG_SysServiceThreads.CurrentProcessingStart_DtTm];
                            drSysServiceThread[RG_SysServiceThreads.LastStopped_DtTm] = DateTime.Now;
                            drSysServiceThread[RG_SysServiceThreads.CurrentProcessingStart_DtTm] = DBNull.Value;
                            UpdateServiceThread(drSysServiceThread);
                        }


                        // Thread.Sleep((iThreadSleepTime * 1000));
                        await Task.Delay((iThreadSleepTime * 1000));

                        this.sThreadStatus = ServiceStatus.Running;
                        // # Load The Thread Configuration 
                        dtSysServiceThreads = oServiceDB.GetThreadConfiguration(this.lThreadId);

                        if (DataSetComponent.CheckRecordExist(dtSysServiceThreads))
                        {
                            drSysServiceThread = dtSysServiceThreads.Rows[0];
                            sReLoad_Flg = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.ReLoad_Flg], Entities.RefData.Flag.NO);
                            sRequestedStatus = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.RequestedStatus_Cd], string.Empty);
                            sTask_Tm = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.Task_Tm], string.Empty);
                            sThreadType = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.ThreadType], string.Empty);
                            dtTmLastStopped_DtTm = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.LastStopped_DtTm], DateTime.MaxValue);
                            iRetries = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.Retries], (int)0);

                            if (sTask_Tm != string.Empty && sThreadType == "J")
                            {
                                dtTask_DtTm = DateTime.ParseExact(DateTime.Now.Date.ToString("MM/dd/yyyy") + " " + sTask_Tm, Format, provider);
                            }

                            //# Check If Reload Flag Is Set To Y
                            if (sReLoad_Flg == Flag.YES)
                            {
                                //# Get Latest Sleep Time
                                iThreadSleepTime = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.ThreadSleep_Tm], (int)0);
                                sMethod_Nm = DataSetComponent.ReplaceDBNull(drSysServiceThread[RG_SysServiceThreads.Method_Nm], string.Empty);

                                if (sRequestedStatus == ServiceStatus.Stop)
                                {
                                    LogMessage("Thread Stop request received ");
                                    //# Stop the Thread
                                    bKeepAlive = false;
                                    this.sThreadStatus = ServiceStatus.Stop;
                                    //# Update Status back  to DB
                                    drSysServiceThread[RG_SysServiceThreads.CurrentStatus_Cd] = ServiceStatus.Stop;
                                    drSysServiceThread[RG_SysServiceThreads.RequestedStatus_Cd] = DBNull.Value;
                                }
                                //# Update Reload_Flg = 'N' to prevent Reloading every time
                                drSysServiceThread[RG_SysServiceThreads.ReLoad_Flg] = Flag.NO;
                                UpdateServiceThread(drSysServiceThread);
                            }

                            if (this.sRequestedStatus == ServiceStatus.Stop)
                            {
                                LogMessage("Thread Stop request received from Service ");
                                //# Stop the Thread
                                bKeepAlive = false;
                                this.sThreadStatus = ServiceStatus.Stop;
                                //# Update Status back  to DB
                                drSysServiceThread[RG_SysServiceThreads.CurrentStatus_Cd] = ServiceStatus.Stop;
                                drSysServiceThread[RG_SysServiceThreads.RequestedStatus_Cd] = DBNull.Value;
                                UpdateServiceThread(drSysServiceThread);
                            }
                        }

                    }
                    catch (Exception oEx)
                    {
                        Log1.write(oEx.Message + "Inner Exception:" + oEx.InnerException + "Source:" + oEx.Source + "Trace :" + oEx.StackTrace);
                        LogException(oEx);
                    }
                }

            }
            catch (Exception oException)
            {
                Log1.write(oException.Message + "Inner Exception:" + oException.InnerException + "Source:" + oException.Source + "Trace :" + oException.StackTrace);
                LogException(oException);

            }


        }
        #endregion
    }
}
