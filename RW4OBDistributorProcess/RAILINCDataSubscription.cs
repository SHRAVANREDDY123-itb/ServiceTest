using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RW4Entities.Models.RWOBDistributorsEntities;
using RW4Entities.Models.RWServiceManagerEntities;
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
        readonly OBDBHelper? sqlDBHelper;
        private ILogger _logger;



        #endregion

        public RAILINCDataSubscription(IConfiguration configuration, ILogger<RAILINCDataSubscription> logger, OBDBHelper oBDBHelper)
        {
            try
            {
                _logger = logger;
                _logger.LogInformation("RAILINCDataSubscription constructor");
                sqlDBHelper = oBDBHelper;                
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

                _logger.LogError($"Error in AzureProcessRailINCData, error message {ex.Message}, stracktrace {ex.StackTrace}");
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
                                _logger.LogError($"Error in AzureProcessOutbound, error message {ex.Message}, stracktrace {ex.StackTrace}");
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
                    _logger.LogError($"Error in AzureProcessOutbound, error message {ex.Message}, stracktrace {ex.StackTrace}");
                    //throw;
                }
                // }
            }
            catch (Exception)
            {
                isSuccess = false;
                throw;
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
                _logger.LogError($"Error in DeriveRailincCERLog, error message {ex.Message}, stracktrace {ex.StackTrace}");
                //throw;
            }
        }
    }

   
}
