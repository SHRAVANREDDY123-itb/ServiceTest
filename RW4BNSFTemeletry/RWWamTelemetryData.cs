using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RW4Entities.Models.RWBNSFTelemetryEntities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RW4BNSFTelemetry
{
    public class RWWamTelemetryData
    {
        DBHelper? sqlDb;
        private readonly IConfiguration? _configuration;
        private ILogger _logger;
        string? WamSubscriptionBaseUrl;

        string? WAMauthenticationURL_RequestData;
        string? authentication_URL;
        string? WAMAPIExceedAlertTopic;
        string? WAMAPIExceedAlertSub;
        public const int paramId = 11;
        string? WamConsumerKey;
        string? WamMaxPageSize;
        string? WamMessageType;
        string? WamFilePath;

        public const string WAMAuthAPICallMaxCount = "WAMAuthAPICallMaxCount";
        public const string WAMAuthAPICallCurrentCount = "WAMAuthAPICallCurrentCount";
        public const string WAMAuthAPICallCurrentCountLstUpdTime = "WAMAuthAPICallCurrentCountLstUpdTime";


        public RWWamTelemetryData(IConfiguration configuration, ILogger<RWWamTelemetryData> logger, DBHelper DBHelper)
        {
            try
            {
                _configuration = configuration;
                _logger = logger;
                sqlDb = DBHelper;
                WamSubscriptionBaseUrl = configuration["appSettings:WamSubscriptionBaseUrl"];

                WAMauthenticationURL_RequestData = configuration["appSettings:WAMauthenticationURLRequestData"];
                WAMAPIExceedAlertTopic = configuration["appSettings:WAMAPIExceedAlertTopic"];
                WAMAPIExceedAlertSub = configuration["appSettings:WAMAPIExceedAlertSub"];
                authentication_URL = configuration["appSettings:WAMauthenticationURL"];
                WamConsumerKey = configuration["appSettings:WamConsumerKey"];
                WamMaxPageSize = configuration["appSettings:WamMaxPageSize"];
                WamMessageType = configuration["appSettings:WamMessageType"];
                WamFilePath = configuration["appSettings:WamFilePath"];
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in RWWamTelemetryData constructor, error message {ex.Message}, stracktrace {ex.StackTrace}");
            }
        }
        public void GetWamTelemetryData(long threadID)
        {

            try
            {
                int? threadSleepTime = sqlDb.GetThreadSleepTime(threadID);
                List<USP_GetWamsubscriptionIdByUnits_Result> lstWamsubscriptionIds = sqlDb.GetWamsubscriptionIdsByUnits();
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                using (HttpClient client = new HttpClient(handler))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    if (lstWamsubscriptionIds != null && lstWamsubscriptionIds.Count > 0)
                    {
                        string basepath = _configuration["appSettings:WamFilePath"];
                        string jwtToken = sqlDb.GetParamData(paramId);
                        if (string.IsNullOrEmpty(jwtToken) || IsTokenExpired(jwtToken))
                        {
                            jwtToken = GetJwtToken(threadID);
                            if (!string.IsNullOrEmpty(jwtToken))
                                sqlDb.UpdatePramData(jwtToken, paramId);
                        }

                        // string Jwttoken = GetJwtToken(threadID);
                        client.BaseAddress = new Uri(WamSubscriptionBaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        client.DefaultRequestHeaders.Add("consumer-key", WamConsumerKey);
                        if (!string.IsNullOrEmpty(jwtToken))
                        {
                            foreach (USP_GetWamsubscriptionIdByUnits_Result wamsubscription in lstWamsubscriptionIds)
                            {
                                if (wamsubscription != null && wamsubscription.WamSubscriptionGuid != null)
                                {
                                    string ContToken = "";
                                    bool isApiTokenExpire = false;
                                    do
                                    {
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                                        ContToken = "";
                                        isApiTokenExpire = false;
                                        var response = new HttpResponseMessage();
                                        try
                                        {
                                            string StartDate = (!string.IsNullOrEmpty(wamsubscription.APIStartDttm)) ? wamsubscription.APIStartDttm : DateTime.UtcNow.AddSeconds(-Convert.ToDouble(threadSleepTime)).ToString("yyyy-MM-ddTHH:mm:ssZ");
                                            string requestData = String.Empty;
                                            requestData = "?SubscriptionId=" + wamsubscription.WamSubscriptionGuid;
                                            requestData += "&MaxPageSize=" + Convert.ToInt32(WamMaxPageSize);
                                            requestData += "&FromDate=" + DateTime.Parse(StartDate).ToString("yyyy-MM-ddTHH:mm:ssZ"); ;
                                            requestData += "&ContinuationToken=" + ContToken;

                                            //if (wamsubscription.WamSubscriptionGuid== new Guid("B7632DBD-273F-4E3D-A7F2-4BA5146A7A7D"))
                                            //CreateSubscriptionExpection("From Date:" + StartDate + "UTC Currene Date " + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss")+ "Request Data :" + requestData, "Request Data");

                                            response = client.GetAsync("api/equipmentwatch" + requestData).Result;

                                            //  HttpResponseMessage dfresponse = client.GetAsync(string.Format("api/equipmentwatch?SubscriptionId={0}&MaxPageSize={1}&FromDate={2}&ContinuationToken={3}", "76d7484f-f174-4187-9f31-8946e3bfcf11", 400, "2019-06-24T11:59:40.166Z", "")).Result;

                                        }
                                        catch (Exception ex)
                                        {

                                            if (response.StatusCode == HttpStatusCode.OK && response.Content == null)
                                            {
                                                sqlDb.InsertThreadExceptions(threadID, "GetAsync resuest is unauthorized becuase  of sevice is restarted:" + ex.InnerException.InnerException.Message.ToString(), DateTime.Now);
                                                isApiTokenExpire = true;
                                                jwtToken = GetJwtToken(threadID);
                                                if (!string.IsNullOrEmpty(jwtToken))
                                                    sqlDb.UpdatePramData(jwtToken, paramId);
                                                _logger.LogError("GetAsync resuest is unauthorized becuase  of sevice is restarted." + response.ToString());
                                                // sqlDb.InsertThreadExceptions(threadID, ex.InnerException.InnerException.Message.ToString(), DateTime.Now);

                                            }
                                            else
                                                sqlDb.InsertThreadExceptions(threadID, "GetAsync resuest with Expection:" + ex.InnerException.InnerException.Message.ToString(), DateTime.Now);
                                        }

                                        if (response.IsSuccessStatusCode && response.Content != null)
                                        {
                                            var result = response.Content.ReadAsStringAsync();

                                            //if (wamsubscription.WamSubscriptionGuid == new Guid("B7632DBD-273F-4E3D-A7F2-4BA5146A7A7D"))
                                            //    CreateSubscriptionExpection("Response Json" + result.Result, "Json Data");
                                            string apiStartDttm = "";
                                            if (result.Result.Length > 0)
                                            {
                                                try
                                                {

                                                    WamUnitTelemetryData wamUnitTelemetryData = JsonConvert.DeserializeObject<WamUnitTelemetryData>(result.Result);
                                                    ContToken = wamUnitTelemetryData.ContinuationToken;
                                                    if (wamUnitTelemetryData != null && wamUnitTelemetryData.reeferDeviceTelemetryDto.Count > 0)
                                                    {
                                                        apiStartDttm = wamUnitTelemetryData.reeferDeviceTelemetryDto.OrderByDescending(p => p.telemetryDateTime).FirstOrDefault().telemetryDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                                                        int Count = 0;
                                                        foreach (ReeferDeviceTelemetryFeedDto reeferDeviceTelemetryFeedDto in wamUnitTelemetryData.reeferDeviceTelemetryDto)
                                                        {

                                                            if (reeferDeviceTelemetryFeedDto != null && reeferDeviceTelemetryFeedDto.messageType != WamMessageType)
                                                            {
                                                                //if (wamsubscription.WamSubscriptionGuid == new Guid("B7632DBD-273F-4E3D-A7F2-4BA5146A7A7D"))
                                                                //    CreateSubscriptionExpection("From Date:" + StartDate + "Current UTC date time:" + DateTime.UtcNow + "Subscription Id:" + wamsubscription.WamSubscriptionGuid + "Event Date time:" + reeferDeviceTelemetryFeedDto.telemetryDateTime, "Container Telemtry Data");
                                                                Count = Count + 1;
                                                                WamMessage WamMessage = CreateWamXML(reeferDeviceTelemetryFeedDto);
                                                                DateTime dt = DateTime.Now;
                                                                string date = dt.ToString("MMddyyyyHHmmssFFF");
                                                                //  string fileName = string.Format("{0}_{1}_{2}", date, System.IO.Path.GetFileName("wam_ml"), WamUnitTelemetryData.ContinuationToken);
                                                                string finalXmlFile = SerializeXML<WamMessage>(WamMessage);
                                                                XmlDocument doc = new XmlDocument();
                                                                doc.LoadXml(finalXmlFile);
                                                                string filepath = basepath + "wam_ml" + "_" + date + "_" + Count + ".xml";
                                                                doc.Save(filepath);
                                                            }
                                                        }
                                                        // Update wamsubscription table for apistartdttm
                                                        if (!string.IsNullOrEmpty(apiStartDttm))
                                                            sqlDb.UpdateWamSubscriptionForAPIStartDt(wamsubscription.WamSubscriptionGuid, wamsubscription.PrenoteId, wamsubscription.UnitMasterId, apiStartDttm);
                                                    }

                                                    //else
                                                    //{
                                                    //}

                                                }
                                                catch (Exception ex)
                                                {
                                                    _logger.LogError($" error message {ex.Message}, stracktrace {ex.StackTrace}");
                                                    sqlDb.InsertThreadExceptions(threadID, "SuScription id:" + wamsubscription.WamSubscriptionGuid + " Data Serlization and XML Error for :" + ex.Message.ToString(), DateTime.Now);
                                                }
                                            }
                                            if (string.IsNullOrEmpty(apiStartDttm) && string.IsNullOrEmpty(wamsubscription.APIStartDttm))
                                                sqlDb.UpdateWamSubscriptionForAPIStartDt(wamsubscription.WamSubscriptionGuid, wamsubscription.PrenoteId, wamsubscription.UnitMasterId, DateTime.UtcNow.AddSeconds(-Convert.ToDouble(threadSleepTime)).ToString("yyyy-MM-ddTHH:mm:ss"));
                                        }
                                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                                        {
                                            isApiTokenExpire = true;
                                            jwtToken = GetJwtToken(threadID);
                                            if (!string.IsNullOrEmpty(jwtToken))
                                                sqlDb.UpdatePramData(jwtToken, paramId);
                                            _logger.LogError("GetAsync resuest is unauthorized." + response.ToString());
                                            sqlDb.InsertThreadExceptions(threadID, "GetAsync resuest is unauthorized." + response.ToString(), DateTime.Now);

                                        }
                                        else
                                        {
                                            sqlDb.InsertThreadExceptions(threadID, "GetAsync Request Respons Error:" + response.ToString(), DateTime.Now);
                                            _logger.LogError("Internal server Error");
                                        }

                                    } while (!string.IsNullOrEmpty(ContToken) || isApiTokenExpire == true);
                                }

                            }
                        }
                        else
                        {
                            sqlDb.InsertThreadExceptions(threadID, "Authentication serivice is given  empty JWT token,should not able to get wam telemetry data for that date time :" + DateTime.Now, DateTime.Now);
                            _logger.LogError("Authentication serivice is given empty JWT toke, should not able to get wam telemetry data for that date time :" + DateTime.Now);
                        }
                    }
                }
                //Update thread params after the call
                // sqlDb.UpdateEDI_R_SysServiceThreadParams(lstParams);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

        }

        public class Department
        {

            public string DepartmentName { get; set; }
            public int DepartmentId { get; set; }
        }



        public WamMessage CreateWamXML(ReeferDeviceTelemetryFeedDto reeferDeviceTelemetryFeeds)
        {
            try
            {

                // ReeferDeviceTelemetryFeedDto reeferDeviceTelemetryFeeds = wamUnitTelemetryData.reeferDeviceTelemetryDto.FirstOrDefault();
                MessageData Messagedata = new MessageData();
                WamMessage WamMessage = new WamMessage();
                ReeferProperties Reeferlist = new ReeferProperties();
                WamMessage.Errors = "";
                WamMessage.Messages = new MessageData();
                Message message = new Message();
                WamData WamData = new WamData();
                IBndMsg iBndMsg = new IBndMsg();
                Control control = new Control();
                ReeferStatus ReeferStatus = new ReeferStatus();
                Data data = new Data();
                RcdStatus rcdStatus = new RcdStatus();

                // Control node
                control.MsgType = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.messageType) ? reeferDeviceTelemetryFeeds.messageType : "";
                if (reeferDeviceTelemetryFeeds.equipmentInfo != null && !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.alarmsList))
                    control.MsgType = "ReeferAlert";
                control.MsgID = reeferDeviceTelemetryFeeds.id;
                // control.RcdSerialNum = reeferDeviceTelemetryFeeds.deviceId;
                data.MsgID = reeferDeviceTelemetryFeeds.id;
                rcdStatus = BindRcdStatus(reeferDeviceTelemetryFeeds);
                List<ReeferProp> reeferProplst = BindReeferProp(reeferDeviceTelemetryFeeds);

                Reeferlist.ReeferProp = reeferProplst;
                ReeferStatus.ReeferProps = Reeferlist;
                data.RcdStatus = rcdStatus;
                data.ReeferStatus = ReeferStatus;
                iBndMsg.Data = data;
                iBndMsg.Control = control;
                WamData.IBndMsg = iBndMsg;
                message.Data = WamData;
                //lstMessage.Add(message);
                Messagedata.Message = message;
                WamMessage.Messages = Messagedata;

                return WamMessage;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ReeferProp> BindReeferProp(ReeferDeviceTelemetryFeedDto reeferDeviceTelemetryFeeds)
        {
            List<ReeferProp> lstReeferProps = new List<ReeferProp>();
            ReeferProp ReeferProp = null;

            ReeferProp = new ReeferProp();
            ReeferProp.name = ReerferPro.ContainerID.ToString();
            ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentId) ? reeferDeviceTelemetryFeeds.equipmentId : "";
            lstReeferProps.Add(ReeferProp);

            if (reeferDeviceTelemetryFeeds.equipmentInfo != null)
            {
                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.PretripTestState.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.pretripTestStateID) ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.pretripTestStateID) : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SupplyTemperature1.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemp1);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.SupplyTemperature1Qualifier.ToString();
                //ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo) ? reeferDeviceTelemetryFeeds.deviceId : "";
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.ReturnTemperature1.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.returnTemp1);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.AmbientTemperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.ambientTemp);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.AmbientTemperatureQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.ambie);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SupplyTemperature2.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemp2);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.SupplyTemperature2Qualifier.ToString();
                //ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceId) ? reeferDeviceTelemetryFeeds.deviceId : "";
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.VentPositionSensorCMH.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.ventPositionSensorCMH != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.ventPositionSensorCMH) : "0";
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.VentPositionSensorCMHQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.ve);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.Humidity.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.humidity);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.HumidityQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.humidity) ;
                //lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();  //Control serial number is commented becuase of unwanted data is coming and not user in 3.0 app
                //ReeferProp.name = ReerferPro.ControllerSerialNumber.ToString();
                //ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.controllerSerialNumber) ? reeferDeviceTelemetryFeeds.equipmentInfo.controllerSerialNumber : "";
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.MicroVersion.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.microVersion != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.microVersion) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SetPoint.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.setPoint);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.SetPointQualifier.ToString();
                //ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.set) ? reeferDeviceTelemetryFeeds.deviceId : "";
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.IsoFaultBlock.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.isoFaultBlock) ? reeferDeviceTelemetryFeeds.equipmentInfo.isoFaultBlock : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.OperatingMode.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.operatingModeID) ? reeferDeviceTelemetryFeeds.equipmentInfo.operatingModeID : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.USDA1Temperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.usdA1Temp);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.USDA1TemperatureQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.usdA1Temp) ;
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.USDA2Temperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.usdA2Temp);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.USDA2TemperatureQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.usdA2Temp) ;
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.USDA3Temperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.usdA3Temp);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.USDA3TemperatureQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.usdA3Temp) ;
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.Cargo4Temperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.cargo4Temp);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.Cargo4TemperatureQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.HumiditySetpoint.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.humiditySetpoint != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.humiditySetpoint) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.HumiditySetpointStatus.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.humiditySetpointStatus) ? reeferDeviceTelemetryFeeds.equipmentInfo.humiditySetpointStatus : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SoftwareRevision.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.softwareRevision) ? reeferDeviceTelemetryFeeds.equipmentInfo.softwareRevision : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.ReeferType.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.modelNumber) ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.modelNumber) : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SoftwareSubrevision.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.softwareSubRevision) ? reeferDeviceTelemetryFeeds.equipmentInfo.softwareSubRevision : ""; ;
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.DynValReadCmdRev.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.deviceInfo.d);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.O2Setpoint.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.o2Setpoint != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.o2Setpoint) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.O2SetpointSupport.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.o2SetpointSupport) ? reeferDeviceTelemetryFeeds.equipmentInfo.o2SetpointSupport : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.O2Reading.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.o2Reading != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.o2Reading) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.O2ReadingSupport.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.o2ReadingSupport) ? reeferDeviceTelemetryFeeds.equipmentInfo.o2ReadingSupport : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CO2Setpoint.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.cO2Setpoint != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.cO2Setpoint) : "0";
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.CO2SetpointSupport.ToString();
                //ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.co;
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CO2Reading.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.cO2Reading != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.cO2Reading) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CO2ReadingSupport.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.cO2ReadingSupport) ? reeferDeviceTelemetryFeeds.equipmentInfo.cO2ReadingSupport : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.OemAlarmBlock.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.oemAlarmBlock) ? reeferDeviceTelemetryFeeds.equipmentInfo.oemAlarmBlock : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CompressorDischargeTemperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compressorDischargeTemp);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CompressorSuctionTemperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compressorSuctionTemp);
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.CompressorSuctionTemperatureQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CompositeSuctionPressure.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compositeSuctionPressure);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.DischargePressure.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.dischargePressure);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.LineVoltage1.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.lineVoltage1 != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.lineVoltage1) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.LineVoltage2.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.lineVoltage2 != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.lineVoltage2) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.LineVoltage3.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.lineVoltage3 != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.lineVoltage3) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CurrentPhaseA.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.currentPhaseA);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CurrentPhaseB.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.currentPhaseB);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CurrentPhaseC.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.currentPhaseC != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.currentPhaseC) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.LineFrequency.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.lineFrequency);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CompressorFrequency.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.compressorFrequency != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compressorFrequency) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CondenserFanOutput.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.condenserFanOutPut) ? reeferDeviceTelemetryFeeds.equipmentInfo.condenserFanOutPut : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.HighSpeedEvaporatorFanOutput.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.highSpeedEvaporatorFanOutPut) ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.highSpeedEvaporatorFanOutPut) : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.LowSpeedEvaporatorFanOutput.ToString();
                ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.lowSpeedEvaporatorFanOutPut) ? reeferDeviceTelemetryFeeds.equipmentInfo.lowSpeedEvaporatorFanOutPut : "";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.HeaterOnTime.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.heaterOnTime != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.heaterOnTime) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.EvaporatorExpansionValveOpening.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.evaporatorExpansionValveOpening != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.evaporatorExpansionValveOpening) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.HotGasValveOpening.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.hotGasValveOpening != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.hotGasValveOpening) : "0";
                lstReeferProps.Add(ReeferProp);


                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.EconomizerValveOpening.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.economizerValveOpening);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SupplyTemperatureLongAvg.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemperatureLongAvg != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemperatureLongAvg) : "0";
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.SupplyTemperatureLongAvgQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemperatureLongAv);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.ReturnTemperatureLongAvg.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.returnTemperatureLongAvg != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.returnTemperatureLongAvg) : "0";
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.ReturnTemperatureLongAvgQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.returnTemperatureLongAvg);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.AmbientTemperatureLongAvg.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.ambientTempLongAvg != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.ambientTempLongAvg) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CompressorSuctionTempLongAvg.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.compressorSuctionTempLongAvg != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compressorSuctionTempLongAvg) : "0";
                lstReeferProps.Add(ReeferProp);

                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.CompressorSuctionTempLongAvgQualifier.ToString();
                //ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compressorSuctionTempLongAvg);
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.CompositeSuctionPressureLongAvg.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.compositeSuctionPressureLongAvg != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.compositeSuctionPressureLongAvg) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.DischargePressureLongAvg.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.dischargePressureLongAvg != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.dischargePressureLongAvg) : "0";
                lstReeferProps.Add(ReeferProp);


                // not Configured Properties
                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.AmbientTemperatureLongAvgQualifier.ToString();
                //ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceId) ? reeferDeviceTelemetryFeeds.deviceId : "";
                //lstReeferProps.Add(ReeferProp);


                //ReeferProp = new ReeferProp();
                //ReeferProp.name = ReerferPro.SupplyTemperatureQualifier.ToString();
                //ReeferProp.value = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceId) ? reeferDeviceTelemetryFeeds.deviceId : "";
                //lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.SupplyTemperature.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemp != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.supplyTemp) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.TimeToDefrost.ToString();
                ReeferProp.value = reeferDeviceTelemetryFeeds.equipmentInfo.timeToDefrost != null ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.timeToDefrost) : "0";
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.EvaporatorTemperature.ToString();
                ReeferProp.value = Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.evaporatorTemp);
                lstReeferProps.Add(ReeferProp);

                ReeferProp = new ReeferProp();
                ReeferProp.name = ReerferPro.AlarmsList.ToString();
                ReeferProp.value = (reeferDeviceTelemetryFeeds.equipmentInfo != null && !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.equipmentInfo.alarmsList)) ? Convert.ToString(reeferDeviceTelemetryFeeds.equipmentInfo.alarmsList) : "";
                lstReeferProps.Add(ReeferProp);


            }



            return lstReeferProps;

        }

        public RcdStatus BindRcdStatus(ReeferDeviceTelemetryFeedDto reeferDeviceTelemetryFeeds)
        {
            RcdStatus rcdStatus = new RcdStatus();

            if (reeferDeviceTelemetryFeeds.deviceInfo != null)
            {
                // RcdStatus node
                rcdStatus.AcceptableACVoltage = reeferDeviceTelemetryFeeds.deviceInfo.isAcceptableACVoltage != null ? Convert.ToBoolean(reeferDeviceTelemetryFeeds.deviceInfo.isAcceptableACVoltage) : false;
                // AcceptableBatteryVoltage
                rcdStatus.AcceptableBatteryVoltage = reeferDeviceTelemetryFeeds.deviceInfo.isAcceptableBatteryVoltage != null ? Convert.ToBoolean(reeferDeviceTelemetryFeeds.deviceInfo.isAcceptableBatteryVoltage) : false;
                //BatteryVoltage
                rcdStatus.BatteryVoltage = reeferDeviceTelemetryFeeds.deviceInfo.batteryVoltage;
                // DeviceTemp
                rcdStatus.DeviceTemp = reeferDeviceTelemetryFeeds.deviceInfo.deviceTemp;
                rcdStatus.EventDtm = reeferDeviceTelemetryFeeds.telemetryDateTime != null ? reeferDeviceTelemetryFeeds.telemetryDateTime : DateTime.Now;
                // GeofenceDefinitionRevision
                rcdStatus.GeofenceDefinitionRevision = reeferDeviceTelemetryFeeds.deviceInfo.geofenceRevision;
                // GPSLastLock
                rcdStatus.GPSLastLock = reeferDeviceTelemetryFeeds.deviceInfo.gpsLastLock;
                //GPSLatitude
                rcdStatus.GPSLatitude = reeferDeviceTelemetryFeeds.deviceInfo.gpsLatitude != null ? reeferDeviceTelemetryFeeds.deviceInfo.gpsLatitude : 0;

                rcdStatus.GPSLockState = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceInfo.gpsLockState) ? reeferDeviceTelemetryFeeds.deviceInfo.gpsLockState : "";
                //GPSLongitude
                rcdStatus.GPSLongitude = reeferDeviceTelemetryFeeds.deviceInfo.gpsLongitude != null ? reeferDeviceTelemetryFeeds.deviceInfo.gpsLongitude : 0;
                // GPSSatelliteCount
                rcdStatus.GPSSatelliteCount = reeferDeviceTelemetryFeeds.deviceInfo.gpsSatelliteCount;
                rcdStatus.InsideGeofence = reeferDeviceTelemetryFeeds.deviceInfo.isInsideGeofence;
                //Is3G
                rcdStatus.Is3G = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceInfo.iS3G) ? reeferDeviceTelemetryFeeds.deviceInfo.iS3G : "false";
                //MCC
                rcdStatus.MCC = reeferDeviceTelemetryFeeds.deviceInfo.mcc;
                //MessageFormatRevision
                rcdStatus.MessageFormatRevision = reeferDeviceTelemetryFeeds.formatVersion;
                //MNC
                rcdStatus.MNC = reeferDeviceTelemetryFeeds.deviceInfo.mnc;
                //OnWakeUp
                rcdStatus.OnWakeUp = reeferDeviceTelemetryFeeds.deviceInfo.wakeupFromSleep;
                //PtiRunning
                rcdStatus.PtiRunning = reeferDeviceTelemetryFeeds.deviceInfo.pretripState;
                //RcdMoving
                rcdStatus.RcdMoving = reeferDeviceTelemetryFeeds.deviceInfo.rcdIsMoving;
                rcdStatus.RcdPowerSource = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceInfo.powerSourceCd) ? reeferDeviceTelemetryFeeds.deviceInfo.powerSourceCd : "";
                //ReeferACPowerLevel
                rcdStatus.ReeferACPowerLevel = reeferDeviceTelemetryFeeds.deviceInfo.reeferACPowerLevel;
                //ReeferClockInSync
                rcdStatus.ReeferClockInSync = reeferDeviceTelemetryFeeds.deviceInfo.reeferClockInSync;
                //ReeferCommunicationEstablished

                // do have properties
                rcdStatus.ReeferCommunicationEstablished = reeferDeviceTelemetryFeeds.deviceInfo.isEquipmentCommunicationEstablished;
                rcdStatus.ReeferOemType = !string.IsNullOrEmpty(reeferDeviceTelemetryFeeds.deviceInfo.equipmentOemType) ? reeferDeviceTelemetryFeeds.deviceInfo.equipmentOemType : "";
                //ReeferPhysicallyConnected
                rcdStatus.ReeferPhysicallyConnected = reeferDeviceTelemetryFeeds.deviceInfo.isEquipmentPhysicallyConnected;
                //ReeferSwitchOn
                rcdStatus.ReeferSwitchOn = reeferDeviceTelemetryFeeds.deviceInfo.equipmentSwitchOn;
                //RSSI
                rcdStatus.RSSI = reeferDeviceTelemetryFeeds.deviceInfo.rssi;
                rcdStatus.RSSIQualifier = reeferDeviceTelemetryFeeds.deviceInfo.rssiQualifierID != null ? Convert.ToString(reeferDeviceTelemetryFeeds.deviceInfo.rssiQualifierID) : "";
                //RTDLOn
                rcdStatus.RTDLOn = reeferDeviceTelemetryFeeds.deviceInfo.realtimeDownloadOn;
                //TowerBaseStationId
                rcdStatus.TowerBaseStationId = reeferDeviceTelemetryFeeds.deviceInfo.towerBaseStationID;
                //TowerLocalizationAreaCode
                rcdStatus.TowerLocalizationAreaCode = reeferDeviceTelemetryFeeds.deviceInfo.towerLocalizationAreaCode;
                //ReeferComms
                rcdStatus.ReeferComms = reeferDeviceTelemetryFeeds.deviceInfo.rfrCommsErr;
                //ReeferCommsCounter
                rcdStatus.ReeferCommsCounter = reeferDeviceTelemetryFeeds.deviceInfo.commErrorCounter;
                //ReeferCommsDisconnectCounter
                rcdStatus.ReeferCommsDisconnectCounter = reeferDeviceTelemetryFeeds.deviceInfo.commDisconnectCounter;
            }

            return rcdStatus;
        }

        public string GetJwtToken(long threadID)
        {
            string accessToken = string.Empty;

            try
            {
                if (IsWamAuthCallLimitExceeded()) { return string.Empty; }
                // Read configuration values from the app.config or web.config file
                string authenticationURL = authentication_URL;
                var requestData = WAMauthenticationURL_RequestData;
                byte[] encodedRequestData = Encoding.UTF8.GetBytes(requestData);

                // Create a request to the Identity Server
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var request = (HttpWebRequest)WebRequest.Create(authenticationURL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = encodedRequestData.Length;

                // Write the request data
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(encodedRequestData, 0, encodedRequestData.Length);
                }

                // Get the response from the Identity Server
                var response = (HttpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                string responseText = reader.ReadToEnd();

                // Parse the JSON response to extract the access token
                JObject jsonResponse = JObject.Parse(responseText);
                accessToken = jsonResponse["access_token"].ToString();
                sqlDb.IncrementWAmAPICurrentCount(WAMAuthAPICallCurrentCount);

            }
            catch (Exception ex)
            {
                sqlDb.InsertThreadExceptions(threadID, "Authentication Error:" + ex.InnerException.InnerException.Message.ToString(), DateTime.Now);
                _logger.LogError(ex.ToString());
            }

            return accessToken;
        }


        public bool IsTokenExpired(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return true;
                }
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;

                if (tokenS.ValidTo < DateTime.UtcNow)
                {
                    // Token is expired
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return true;
            }

            // Token is not expired
            return false;
        }



        public bool IsWamAuthCallLimitExceeded()
        {
            try
            {
                List<KeyValuePair<string, string>> wamAPiParams = new List<KeyValuePair<string, string>>();
                wamAPiParams = sqlDb.GetWAMApiParams(new List<string>() { WAMAuthAPICallCurrentCount, WAMAuthAPICallMaxCount, WAMAuthAPICallCurrentCountLstUpdTime });


                int WAMAuthAPICallMaxCountV = Convert.ToInt16(wamAPiParams.FirstOrDefault(a => a.Key == WAMAuthAPICallMaxCount).Value);
                int WAMAuthAPICallCurrentCountV = Convert.ToInt16(wamAPiParams.FirstOrDefault(a => a.Key == WAMAuthAPICallCurrentCount).Value);
                DateTime WAMAuthAPICallCurrentCountLstUpdTimeV = Convert.ToDateTime(wamAPiParams.FirstOrDefault(a => a.Key == WAMAuthAPICallCurrentCountLstUpdTime).Value);

                // Check if the last updated time is yesterday (UTC)
                DateTime yesterday = DateTime.UtcNow.AddDays(-1);
                if (WAMAuthAPICallCurrentCountLstUpdTimeV.Date <= yesterday.Date)
                {
                    // Reset the current count to zero
                    sqlDb.UpdateParamByParamCd("0", WAMAuthAPICallCurrentCount);
                    sqlDb.UpdateParamByParamCd(DateTime.Now.ToString(), WAMAuthAPICallCurrentCountLstUpdTime);
                    return false;
                }

                // Check if the current count exceeds the max count
                if (WAMAuthAPICallCurrentCountV >= WAMAuthAPICallMaxCountV)
                {
                    RWUtilities.Common.Utility.SendMessageToAzureSubscription(WAMAPIExceedAlertTopic, WAMAPIExceedAlertSub, JsonConvert.SerializeObject(new { Comment = $"Wam API {authentication_URL} has reached the limit of call count {WAMAuthAPICallCurrentCountV}" }),_logger);
                    return true;
                }

                // If none of the conditions are met, return false
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        public string SerializeXML<T>(object objClass) where T : class
        {
            try
            {
                using (var stringWriter = new System.IO.StringWriter())
                using (var xmlTextWriter = new System.Xml.XmlTextWriter(stringWriter))
                {
                    var dcs = new XmlSerializer(typeof(T));
                    dcs.Serialize(xmlTextWriter, objClass);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     


      


    }
}
