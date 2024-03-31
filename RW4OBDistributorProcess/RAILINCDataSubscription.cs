using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RW4Entities.Models.RWEDIMgmtEntities;
using RW4Entities.Models.RWOBDistributorsEntities;
using RWUtilities.Common;


namespace RW4OBDistributorProcess
{
    public class RAILINCDataSubscription
    {
        #region "Declarations"

       
    
        public const string CompanyName = "RAILINC";
        public const string SysParamCd = "EDIFileOBLoc";
        public const string HeaderName = "CLU TO RRDC RTT       ";
        public const string FileName = "CER";
        public const string Prefix = "L";
        public const string StopPrefix = "D";
        public const string EndLine = "EOM ";
        public const string UnitXMLTag = "Unit";
        public const string FileExtension = ".txt";
        public const string DateFormat = "MMddyyyyHHmmss";
        public const string DateFormatHr = "MMddyyyyHH";
        public const string DateFormatHrForStop = "yyMMddHH";
        public const string EvendCd = "EvendCd";
      

        string filePath = string.Empty;
        string? strEvendCd;

        public const string TopicName = "TopicName";
        public const string SubscripName = "SubscripName";
        public const string IsBatchProcess = "IsBatchProcess";
        public const string BatchMsgCount = "BatchMsgCount";
        readonly SQLDBHelper? sqlDBHelper;
        private ILogger _logger;



        #endregion

        public RAILINCDataSubscription(IConfiguration configuration, IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                _logger = logger;
                _logger.LogInformation("RAILINCDataSubscription constructor");
                sqlDBHelper = serviceProvider.GetRequiredService<SQLDBHelper>();
                filePath = sqlDBHelper.GetSysParamValues(CompanyName, SysParamCd, 5);
                strEvendCd = configuration["appSettings:" + EvendCd];
                RWUtilities.Common.Utility.connectionString = configuration["appSettings:AzurePrimaryConnectionString"];
              

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }




        #region  Azure Railinc Processing 
       
        public void AzureProcessRailINCData(long threadID)
        {
            try
            {
                
                List<R_SysServiceThreadParams> Params = sqlDBHelper?.GetListServiceThreadParamById(threadID);
                if (Params != null)
                {
                    string topicName = string.Empty;
                    string suscriptionName = string.Empty;
                    bool isBatchProcess = false;
                    int messageCount = 0;
                    foreach (R_SysServiceThreadParams param in Params)
                    {
                        if (param.SysParamCd == TopicName)
                            topicName = param.SysParamVal;
                        if (param.SysParamCd == SubscripName)
                            suscriptionName = param.SysParamVal;
                        if (param.SysParamCd == IsBatchProcess)
                            isBatchProcess = Convert.ToBoolean(param.SysParamVal);
                        if (param.SysParamCd == BatchMsgCount)
                            messageCount = Convert.ToInt32(param.SysParamVal);
                    }
                   
                    List<DistributorJson> messageList = RWUtilities.Common.Utility.GetQueueMessagefromOBSAzureSubscription(topicName, suscriptionName, isBatchProcess, messageCount, _logger).Result;
                    if (messageList != null && messageList.Count > 0)
                    {
                        foreach (DistributorJson json in messageList)
                        {
                            try
                            {
                                if (json != null)
                                {
                                    bool isSuccess = false;
                                    isSuccess = AzureProcessOutbound(json.TriggerCd, Convert.ToInt64(json.TriggerID), json.Unit, messageList.Count);
                                    if (isSuccess)
                                        RWUtilities.Common.Utility.DeleteMessagebasedonProperties(topicName, suscriptionName, nameof(json.TriggerID), json.TriggerID, _logger).Wait();

                                }
                            }
                            catch (Exception ex)
                            {

                                _logger.LogError("AzureProcessRailINCData --ThreadId " + threadID + " and messageFaild" + json.Json, ex);
                                // CreateCargoCareExpection("Auto Accepted Thread Id" + threadID + "Error Message: " + ex.Message + "Error StackTrace: " + ex.StackTrace + "Inner Expection" + ex.InnerException, "MonitorUserDefinedCCInspFromAzure", json.Json);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.ToString());
            }
        }

        public bool AzureProcessOutbound(string eventCd, long eventID, string unitNumber, int messageCount)
        {
            bool isSuccess = false;
            try
            {

                TextWriter tw;
                
                DateTime dt = System.DateTime.UtcNow;
                string date = dt.ToString(DateFormat);
                filePath = filePath + FileName + date + Prefix + FileExtension;

                FileStream fs1 = null;
                StreamWriter writer = null;
                List<string> lstUnits = new List<string>();
                List<RailincCER> lstRailincCER = new List<RailincCER>();
               
                string[] eventCds = strEvendCd.Split(',');
                
                if (eventCds.Contains(eventCd))
                {
                    
                    if (!string.IsNullOrEmpty(unitNumber))
                    {
                       
                        USP_GetPreNoteUnitStatusByEventId_Result unitStatus = sqlDBHelper?.GetPreNoteUnitStatusByEventId(eventID);
                        if (unitStatus != null && (unitStatus.PreNoteUnitStatusCd == RW4Entities.DBConstants.PreNoteUnitStatus.Pending || unitStatus.PreNoteUnitStatusCd == RW4Entities.DBConstants.PreNoteUnitStatus.Active)
                            && !lstUnits.Contains(unitNumber.Substring(0, 10)))
                        {
                            if (fs1 == null)
                            {
                                fs1 = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                                if (writer == null)
                                    writer = new StreamWriter(fs1);
                                string dateHr = dt.ToString(DateFormatHr);
                                writer.WriteLine(HeaderName + dateHr + messageCount);
                            }
                            // The tag could not be fond
                            string UnitId = Prefix + unitNumber.Substring(0, 10);
                            writer.WriteLine(UnitId);
                            lstUnits.Add(unitNumber.Substring(0, 10));

                         
                            try
                            {
                                
                                RailincCER railincCER = new RailincCER();
                                railincCER.EventId = eventID;
                                railincCER.UnitNumber = unitNumber.Substring(0, 10);
                                railincCER.FleetCd = Prefix;
                                railincCER.FileName = FileName + date + Prefix + FileExtension;
                                lstRailincCER.Add(railincCER);
                            }
                            catch (Exception ex)
                            {
                                // isSuccess = false;
                                _logger.LogError(ex.ToString());
                                //throw;
                            }
                        }
                    }
                }
                // }
                if (writer != null)
                {
                    writer.WriteLine(EndLine);
                    writer.Close();
                    isSuccess = true;
                }

                /**/
                try
                {
                    DeriveRailincCERLog(lstRailincCER);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    _logger.LogError(ex.ToString());
                    //throw;
                }
                // }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }

        public void AzureProcessRailINCDataStop(long threadID)
        {
            try
            {
                
                List<R_SysServiceThreadParams> Params = sqlDBHelper?.GetListServiceThreadParamById(threadID);
                if (Params != null)
                {
                    string topicName = string.Empty;
                    string suscriptionName = string.Empty;
                    bool isBatchProcess = false;
                    int messageCount = 0;
                    foreach (R_SysServiceThreadParams param in Params)
                    {
                        if (param.SysParamCd == TopicName)
                            topicName = param.SysParamVal;
                        if (param.SysParamCd == SubscripName)
                            suscriptionName = param.SysParamVal;
                        if (param.SysParamCd == IsBatchProcess)
                            isBatchProcess = Convert.ToBoolean(param.SysParamVal);
                        if (param.SysParamCd == BatchMsgCount)
                            messageCount = Convert.ToInt32(param.SysParamVal);
                    }

                    List<DistributorJson> messageList = RWUtilities.Common.Utility.GetQueueMessagefromOBSAzureSubscription(topicName, suscriptionName, isBatchProcess, messageCount, _logger).Result;
                    if (messageList != null && messageList.Count > 0)
                    {
                        foreach (DistributorJson json in messageList)
                        {
                            try
                            {
                                if (json != null)
                                {
                                    bool isSuccess = AzureProcessStopOutbound(json.TriggerCd, Convert.ToInt64(json.TriggerID), json.Unit);
                                    if (isSuccess)
                                        RWUtilities.Common.Utility.DeleteMessagebasedonProperties(topicName, suscriptionName, nameof(json.TriggerID), json.TriggerID, _logger).Wait();


                                }
                            }
                            catch (Exception ex)
                            {

                                _logger.LogError("AzureProcessRailINCDataStop --ThreadId " + threadID + " and messageFaild" + json.Json, ex);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("AzureProcessRailINCDataStop --ThreadId " + threadID, ex);
            }
        }

        public bool AzureProcessStopOutbound(string eventCd, long eventID, string unit)
        {
            bool isSuccess = false;
            try
            {
                TextWriter tw;
               
                DateTime dt = System.DateTime.UtcNow;
                string date = dt.ToString(DateFormat);
                filePath = filePath + FileName + date + StopPrefix + FileExtension;

                FileStream fs1 = null;
                StreamWriter writer = null;
                List<string> lstUnits = new List<string>();
                int cnt = 0;
                List<RailincCER> lstRailincCER = new List<RailincCER>();
                
                string[] eventCds = strEvendCd.Split(',');
                
                if (!string.IsNullOrEmpty(unit))
                {
                    List<Usp_GetActivePendingPrenoteUnit_Result> activePendigPrenoteUnits = sqlDBHelper?.GetActivePendingPrenotUnit(unit);
                    var unitNumber = unit.Length >= 10 ? unit.Substring(0, 10) : unit;
                    if ((activePendigPrenoteUnits != null && activePendigPrenoteUnits.Count != 0))
                    {
                        return isSuccess;
                    }
                 
                    if ((activePendigPrenoteUnits == null || activePendigPrenoteUnits.Count == 0) && !lstUnits.Contains(unitNumber))
                    {
                        cnt++;
                        if (fs1 == null)
                        {
                            fs1 = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                            if (writer == null)
                                writer = new StreamWriter(fs1);
                            //string dateHr = dt.ToString(DateFormatHr);
                            string dateHr = dt.ToString(DateFormatHrForStop);
                            writer.WriteLine(HeaderName + dateHr);
                           
                        }
                      
                        string UnitId = StopPrefix + unitNumber;
                        writer.WriteLine(UnitId);
                       
                        lstUnits.Add(unitNumber);

                       
                        try
                        {
                           
                            RailincCER railincCER = new RailincCER();
                            railincCER.EventId = eventID; //Convert.ToInt64(TriggerIDLTag[0].InnerText);
                                                          // railincCER.UnitNumber = UnitIdTag[0].InnerText.Substring(0, 10);
                            railincCER.UnitNumber = unitNumber;
                            railincCER.FleetCd = StopPrefix;
                            railincCER.FileName = FileName + date + StopPrefix + FileExtension;
                            lstRailincCER.Add(railincCER);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.ToString());
                            //throw;
                        }

                    }
                }
              
                if (writer != null)
                {
                    writer.WriteLine(EndLine);
                    writer.Close();
                    isSuccess = true;
                }

                /**/
                try
                {
                    DeriveRailincCERLog(lstRailincCER);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    _logger.LogError(ex.ToString());
                    //throw;
                }
                // }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }
        #endregion


        private void DeriveRailincCERLog(List<RailincCER> lstRailincCER)
        {
            try
            {
                bool result = sqlDBHelper.DeriveRailincCERLog(lstRailincCER);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                //throw;
            }
        }
    }

    public class RailincCER
    {
        public long EventId { get; set; }
        public string FleetCd { get; set; }
        public string UnitNumber { get; set; }
        public string FileName { get; set; }
    }
}
