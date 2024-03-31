using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OCLOBService;
using RW4Entities.Models.RWEDIMgmtEntities;
using RW4Entities.Models.RWOBDistributorsEntities;
using RWUtilities.Common;



namespace RW4OBDistributorProcess
{
    public class OCLOBInspection
    {
        #region "Declarations"    
       
        public const string OCLQueueName = "OCLQueueName";
        public const string OOCLSharedFoleder = "OOCLSharedFoleder";
        public const string EventIDTag = "TriggerID";
        string? filePath;

        readonly SQLDBHelper? sqlDBHelper;
        private ILogger _logger;


        #endregion


        public OCLOBInspection(IConfiguration configuration, IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                sqlDBHelper = serviceProvider.GetRequiredService<SQLDBHelper>();
                _logger = logger;
                filePath = configuration["appSettings:" + OOCLSharedFoleder];
                RWUtilities.Common.Utility.connectionString= configuration["appSettings:AzurePrimaryConnectionString"];

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }





        #region Azure OOCL  Outbound Ispection
        public const string TopicName = "TopicName";
        public const string SubscripName = "SubscripName";
        public const string IsBatchProcess = "IsBatchProcess";
        public const string BatchMsgCount = "BatchMsgCount";
        public void AzureProcessOCLOBInspection(long threadID)
        {
            try
            {               
              
                List<R_SysServiceThreadParams> Params = sqlDBHelper.GetListServiceThreadParamById(threadID);              
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
                                    bool isSuccess = AzureProcessOutbound(json.TriggerCd, Convert.ToInt64(json.TriggerID), json.Unit);
                                    if (isSuccess)
                                        RWUtilities.Common.Utility.DeleteMessagebasedonProperties(topicName, suscriptionName, nameof(json.TriggerID), json.TriggerID, _logger).Wait();
                                }
                            }
                            catch (Exception ex)
                            {

                                _logger.LogError("AzureProcessOCLOBInspection --ThreadId " + threadID + " and messageFaild" + json.Json, ex);
                                // CreateCargoCareExpection("Auto Accepted Thread Id" + threadID + "Error Message: " + ex.Message + "Error StackTrace: " + ex.StackTrace + "Inner Expection" + ex.InnerException, "MonitorUserDefinedCCInspFromAzure", json.Json);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("AzureProcessOCLOBInspection --ThreadId " + threadID, ex);
            }
        }
        public bool AzureProcessOutbound(string eventCd, long eventID, string unit)
        {
            bool isSuccess = false;
            try
            {

                //long eventID = 0;
                string msg = string.Empty;

                filePath = filePath + "OOCL_" + DateTime.Now.Year + DateTime.Now.Month + ".txt";

                if (!File.Exists(filePath))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                        sw.WriteLine("**** OOCL *****");
                    }
                }


               
                USP_OBDist_GetInsDetailsByEventID_Result inspectionDetails = sqlDBHelper.GetInspectionDetailsByEventID(eventID);

                msg = "**** OOCL OB Inspection received request for eventId : " + eventID + " , DateTime : " + DateTime.Now;

                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(msg);
                }


                if (inspectionDetails != null)
                {

                    int count = sqlDBHelper.GetCountSOSVCLOG(inspectionDetails.SOId);
                    if (count > 1)
                    {
                        isSuccess = true;
                        msg = "Already Accepted inspection exist for eventId : " + eventID + " , DateTime : " + DateTime.Now;

                        using (StreamWriter sw = File.AppendText(filePath))
                        {
                            sw.WriteLine(msg);
                        }
                        return isSuccess;
                    }


                    List<USP_OBDist_GetInsTripDetails_Result> tripDetails = sqlDBHelper.GetInspectionTripDetails((long)inspectionDetails.PreNoteId);

                    string origion = (from t in tripDetails where t.FacilityTypeCd == "O" select t.FacilityNm).FirstOrDefault();
                    string destination = (from t in tripDetails where t.FacilityTypeCd == "D" select t.FacilityNm).FirstOrDefault();
                    List<string> midpointslst = (from t in tripDetails where t.FacilityTypeCd == "M" select t.FacilityNm).ToList();
                    ArrayOfString arrMidpoints =new ArrayOfString();
                    if (midpointslst.Count > 0)
                    {
                        for (int j = 0; j < midpointslst.Count; j++)
                        {
                            arrMidpoints.Add(midpointslst[j].ToString());
                        }
                    }

                    StWorkOrder stWorkOrder = new StWorkOrder();
                    stWorkOrder.workOrderNumber = inspectionDetails.PreNoteNbr;
                    stWorkOrder.inspectionTime = inspectionDetails.SOSvcDtTm;

                    Asset[] assetList = new Asset[1];
                    Asset asset = new Asset();
                    asset.reeferContainerNumber = inspectionDetails.UnitNumber;
                    asset.gensetNumber = inspectionDetails.GensetNumber;
                    assetList[0] = asset;
                    stWorkOrder.asset = assetList;

                    Trip trip = new Trip();
                    trip.origin = origion;
                    trip.destination = destination;
                    trip.routePoints = arrMidpoints;

                    stWorkOrder.trip = trip;

                    GenTrakApplicationGatewayPortPortTypeClient s = new GenTrakApplicationGatewayPortPortTypeClient();
                    
                    int result = s.publishStWorkOrderAsync(stWorkOrder).Result;


                    isSuccess = true;

                    msg = "OOCL OB Inspection for Prenotenbr : " + inspectionDetails.PreNoteNbr + " , Unit Number : " + inspectionDetails.UnitNumber + " , Event Id : " + eventID + " , Result : " + result.ToString() + " , DateTime : " + DateTime.Now;

                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine(msg);
                    }

                }
                else
                {
                    isSuccess = true;
                    msg = "OOCL OB Inspection for request eventId : " + eventID + " , Result : No inspection data " + " , DateTime : " + DateTime.Now;

                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine(msg);
                    }

                }

                //}
                //}
                return isSuccess;
            }
            catch (Exception)
            {
                isSuccess = false;
                throw;
            }
        }
        #endregion

    }
}
