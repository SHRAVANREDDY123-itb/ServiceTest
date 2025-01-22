using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using RWUtilities.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using RW4Entities.Models.RWOBDistributorsEntities;
using Microsoft.Extensions.Logging;
using RW4Entities.Models.RWServiceManagerEntities;

namespace RW4OBDistributorProcess
{
    public class WAMDataSubscription
    {

        #region "Declarations"

     
      
        private readonly IConfiguration? _configuration;

       
        string? WamSubscriptionBaseUrl;
        string? WAMauthenticationURL_RequestData;
        string? authentication_URL;
        string? WAMAPIExceedAlertTopic;
        string? WAMAPIExceedAlertSub;
        public const int ParamId = 11; // Token will be shared between telemetry and subscription API
        string? subsciptionaddedDays;
        string? WamConsumerKey;

        public const string WAMAuthAPICallMaxCount = "WAMAuthAPICallMaxCount";
        public const string WAMAuthAPICallCurrentCount = "WAMAuthAPICallCurrentCount";
        public const string WAMAuthAPICallCurrentCountLstUpdTime = "WAMAuthAPICallCurrentCountLstUpdTime";

       readonly SQLDBHelper? sqlDBHelper;
        private ILogger _logger;

        #endregion

        public WAMDataSubscription(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<WAMDataSubscription> logger)
        {
            try
            {
                _configuration = configuration;
                _logger = logger;
                sqlDBHelper = serviceProvider.GetRequiredService<SQLDBHelper>();
                WamSubscriptionBaseUrl = _configuration["appSettings:WamSubscriptionBaseUrl"]; 
                WAMauthenticationURL_RequestData = _configuration["appSettings:WAMauthenticationURLRequestData"];
                WAMAPIExceedAlertTopic = _configuration["appSettings:WAMAPIExceedAlertTopic"]; 
                WAMAPIExceedAlertSub = _configuration["appSettings:WAMAPIExceedAlertSub"]; 
                authentication_URL = _configuration["appSettings:WAMauthenticationURL"];
                subsciptionaddedDays = _configuration["appSettings:subsciptionaddedDays"]; 
                WamConsumerKey = _configuration["appSettings:WamConsumerKey"];
                RWUtilities.Common.Utility.connectionString = configuration["appSettings:AzurePrimaryConnectionString"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }


        #region  WAM Subscription from Azure

        public const string TopicName = "TopicName";
        public const string SubscripName = "SubscripName";
        public const string IsBatchProcess = "IsBatchProcess";
        public const string BatchMsgCount = "BatchMsgCount";
        public void ProcessWAMDataSubscriptionFromAzure(long threadID)
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
                                    bool isSuccess = AzureProcessOutbound(json.TriggerCd, Convert.ToInt64(json.TriggerID), json.Unit, json.Json, threadID);
                                    if (isSuccess) //delete
                                        RWUtilities.Common.Utility.DeleteMessagebasedonProperties(topicName, suscriptionName, nameof(json.TriggerID), json.TriggerID, _logger).Wait();

                                }
                            }
                            catch (Exception ex)
                            {

                                _logger.LogError("ProcessWAMDataSubscriptionFromAzure --ThreadId " + threadID, ex);
                             }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                _logger.LogError("ProcessWAMDataSubscriptionFromAzure --ThreadId " + threadID, ex);
            }
        }

        public bool AzureProcessOutbound(string eventCd, long eventID, string UnitId, string messagebody, long threadID)
        {
            bool isSuccess = true;
            try
            {

                List<USP_OBDist_GetPrenoteDetails_Result> lstPrenoteDetails = sqlDBHelper?.GetPrenoteDetails(eventID);
                if (lstPrenoteDetails != null && lstPrenoteDetails.Count > 0)
                {

                    //string jwtToken = GetJwtToken();
                    string jwtToken = sqlDBHelper?.GetWamSubsciptionApiToken(ParamId);
                    if (string.IsNullOrEmpty(jwtToken) || IsTokenExpired(jwtToken))
                    {
                        jwtToken = GetJwtToken(threadID);
                        if (!string.IsNullOrEmpty(jwtToken))
                            sqlDBHelper?.UpdateWamAPIToken(jwtToken, ParamId);
                    }

                    if (!string.IsNullOrEmpty(jwtToken))
                    {
                        foreach (USP_OBDist_GetPrenoteDetails_Result prenoteDetails in lstPrenoteDetails)
                        {
                            if (prenoteDetails != null)
                            {
                                T_WAMDataSubscription subscription = sqlDBHelper?.GetWamDataSubcriptionDetails(prenoteDetails.PreNoteId, prenoteDetails.UnitMasterId);

                                if (subscription == null || (subscription != null && subscription.WamSubscriptionGuid == null))
                                {
                                    DateTime startDate = prenoteDetails.ETSDt;
                                    DateTime endDate = prenoteDetails.ETSDt.AddDays(Convert.ToInt32(subsciptionaddedDays));
                                    //To creata wam data subscriptions
                                    long? wamDataSubscriptionId = 0;
                                    if (subscription != null && subscription.WamSubscriptionGuid == null)
                                        wamDataSubscriptionId = subscription.WAMDataSubscriptionId;
                                    else
                                        wamDataSubscriptionId = sqlDBHelper?.CreateWamDataSubscription(prenoteDetails.PreNoteId, prenoteDetails.UnitMasterId, startDate, endDate);
                                    CreateDataWAMSubscription(UnitId, startDate, endDate, wamDataSubscriptionId, jwtToken, threadID);
                                    //  isSuccess = true;
                                    //Prepare Data Subscription XML and calling web service 
                                    // PrepareCreateDataSubscriptionXML("PostCreateDataSubscription", UnitId, startDate, endDate, wamDataSubscriptionId);
                                }
                                //Update WAM Subscription 
                                else if (eventCd == "PNEDI" || eventCd == "PNM" || eventCd == "PNUNUPD")
                                {
                                    DateTime startDate = prenoteDetails.ETSDt;
                                    DateTime endDate = prenoteDetails.ETSDt.AddDays(Convert.ToInt32(subsciptionaddedDays));
                                    subscription.StartDate = startDate;
                                    subscription.EndDate = endDate;
                                    sqlDBHelper?.UpdateWamDataSubscriptionETSDate(subscription.WAMDataSubscriptionId, subscription.StatusCd, subscription.StartDate, subscription.EndDate);
                                    UpdateDataWAMSubscription(subscription, jwtToken, threadID);
                                    //isSuccess = true;
                                }
                                // PrepareUpdateDataSubscription("PostUpdateDataSubscription", subscription, "true");
                                //Update WAM Subscription 
                                else if (eventCd == "TripCmp" || eventCd == "PNUnitComp" || eventCd == "TATCTP" || eventCd == "PrenoteCmp")
                                {
                                    DeleteDataWAMSubscription(subscription, jwtToken, threadID);
                                    // isSuccess = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        CreateSubscriptionExpection("Subscription is not created or update or inactive becuase of JWT token is empty for below unit" + messagebody, "Authentication JWT Token is empty");
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                throw ex;
            }
            return isSuccess;
        }

        #endregion



        #region WAM Latest

        public void CreateDataWAMSubscription(string reeferId, DateTime startDate, DateTime endDate, long? wamDataSubscriptionId, string JwtToken, long threadID)
        {
            bool isApiTokenExpire = false;
            try
            {

                string Status = "";
                int subscriptionId = 0;
                string subscriptionGuId = null;
                WamSubscription wamSubscription = new WamSubscription();
                WamSubscriptionCreate subscriptions = new WamSubscriptionCreate();
                List<WamSubscription> WamSubscriptionlst = new List<WamSubscription>();
                wamSubscription.equipmentId = reeferId;//"UNit234234";
                wamSubscription.subscriptionStartDate = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");  //.ToString("MM/dd/yyyy"); "2019-06-24T11:59:40.166Z";
                wamSubscription.subscriptionEndDate = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ");//.ToString("MM/dd/yyyy"); "2019-07-23T11:59:40.166Z";
                WamSubscriptionlst.Add(wamSubscription);
                subscriptions.subscriptionsPost = WamSubscriptionlst;
                do
                {
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    };
                    using (HttpClient client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        isApiTokenExpire = false;
                        string json = JsonConvert.SerializeObject(subscriptions, Newtonsoft.Json.Formatting.None);
                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                        client.BaseAddress = new Uri(WamSubscriptionBaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                        //  HttpResponseMessage response = client.PostAsJsonAsync("api/equipmentwatch/subscribe", subscriptions).Result;
                        client.DefaultRequestHeaders.Add("consumer-key", WamConsumerKey);
                        HttpResponseMessage response = new HttpResponseMessage();
                        try
                        {
                            response = client.PostAsync("api/equipmentwatch/subscribe", stringContent).Result;
                        }
                        catch (Exception ex)
                        {
                            if (response.StatusCode == HttpStatusCode.OK && response.Content == null)
                            {
                                isApiTokenExpire = true;
                                JwtToken = GetJwtToken(threadID);
                                if (!string.IsNullOrEmpty(JwtToken))
                                    sqlDBHelper?.UpdateWamAPIToken(JwtToken, ParamId);
                                _logger.LogError("PostAsync resuest is unauthorized becuase  of sevice is restarted." + ex.InnerException.InnerException.Message);
                                CreateSubscriptionExpection("PostAsync resuest is unauthorized becuase  of sevice is restarted." + ex.InnerException.InnerException.Message, "Create PostAsync service request");

                            }
                            else
                                CreateSubscriptionExpection(ex.InnerException.InnerException.Message, "Create PostAsync service request");
                        }



                        if ((response.StatusCode == HttpStatusCode.Created) && response.IsSuccessStatusCode && response.Content != null)
                        {
                            Status = "S";
                            string result = response.Content.ReadAsStringAsync().Result;
                            List<dynamic> wamSubscriptionOutput = JsonConvert.DeserializeObject<List<dynamic>>(response.Content.ReadAsStringAsync().Result);
                            subscriptionGuId = wamSubscriptionOutput[0].subscriptionId;
                        }
                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            isApiTokenExpire = true;
                            JwtToken = GetJwtToken(threadID);
                            if (!string.IsNullOrEmpty(JwtToken))
                                sqlDBHelper?.UpdateWamAPIToken(JwtToken, ParamId);
                            _logger.LogError("PostAsync resuest is unauthorized." + response.ToString());
                            CreateSubscriptionExpection("PostAsync resuest is unauthorized." + response.ToString(), "Create PostAsync service request");
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            Status = "E";
                            WamSubscriptionError wamSubscriptionError = JsonConvert.DeserializeObject<WamSubscriptionError>(response.Content.ReadAsStringAsync().Result);
                            if (wamSubscriptionError != null)
                            {
                                _logger.LogError("WAM Data Subriction  Error . Message: " + wamSubscriptionError.Message + "Error. ExceptionMessage: " + wamSubscriptionError.ExceptionMessage + "Error StackTrace: " + wamSubscriptionError.StackTrace);
                                CreateSubscriptionExpection("WAM Data Subriction Error . Message: " + wamSubscriptionError.Message + " Error. ExceptionMessage: " + wamSubscriptionError.ExceptionMessage + "Error StackTrace: " + wamSubscriptionError.StackTrace, "Create PostAsync service request");
                            }
                        }
                        else
                        {
                            Status = "E";
                            _logger.LogError("WAM Data Subriction Error. " + response.ToString());
                            CreateSubscriptionExpection("WAM Data Subriction Error. " + response.ToString(), "Create PostAsync service request");
                        }
                    }
                } while (isApiTokenExpire);
                sqlDBHelper?.UpdateWamDataSubscription(wamDataSubscriptionId, subscriptionId, Status, subscriptionGuId);

            }
            catch (Exception ex)
            {
                CreateSubscriptionExpection("WAM Data Subriction Error. Message: " + ex.Message + "Error StackTrace: " + ex.StackTrace + "Inner Expection" + ex.InnerException, "Create PostAsync service request");
                throw ex;
            }
        }


        public void UpdateDataWAMSubscription(T_WAMDataSubscription subscription, string JwtToken, long threadID)
        {
            bool isApiTokenExpire = false;
            try
            {
                string Status = "";
                Int64 subscriptionId = 0;
                string subscriptionGuid = null;
                WamSubscriptionUpdate wamSubscriptionupdate = new WamSubscriptionUpdate();
                DataSubscriptionUpdate subscriptionsupdate = new DataSubscriptionUpdate();
                List<WamSubscriptionUpdate> WamSubscriptionlst = new List<WamSubscriptionUpdate>();
                wamSubscriptionupdate.subscriptionId = Convert.ToString(subscription.WamSubscriptionGuid);
                if (subscription.StartDate != null)
                    wamSubscriptionupdate.subscriptionStartDate = ((DateTime)subscription.StartDate).ToString("yyyy-MM-ddTHH:mm:ssZ");
                if (subscription.EndDate != null)
                    wamSubscriptionupdate.subscriptionEndDate = ((DateTime)subscription.EndDate).ToString("yyyy-MM-ddTHH:mm:ssZ");
                WamSubscriptionlst.Add(wamSubscriptionupdate);
                subscriptionsupdate.subscriptionsPut = WamSubscriptionlst;
                do
                {
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    };
                    using (HttpClient client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        isApiTokenExpire = false;
                        string json = JsonConvert.SerializeObject(subscriptionsupdate, Newtonsoft.Json.Formatting.None);
                        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                        client.BaseAddress = new Uri(WamSubscriptionBaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("consumer-key", WamConsumerKey);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                        HttpResponseMessage response = new HttpResponseMessage();
                        try
                        {
                            response = client.PutAsync("api/equipmentwatch/subscribe", stringContent).Result;
                        }
                        catch (Exception ex)
                        {
                            if (response.StatusCode == HttpStatusCode.OK && response.Content == null)
                            {
                                isApiTokenExpire = true;
                                JwtToken = GetJwtToken(threadID);
                                if (!string.IsNullOrEmpty(JwtToken))
                                    sqlDBHelper?.UpdateWamAPIToken(JwtToken, ParamId);
                                _logger.LogError("PutAsync resuest is unauthorized becuase  of sevice is restarted." + ex.InnerException.InnerException.Message);
                                CreateSubscriptionExpection("PutAsync resuest is unauthorized becuase  of sevice is restarted." + ex.InnerException.InnerException.Message, "Update PutAsync service request");

                            }
                            else
                                CreateSubscriptionExpection(ex.InnerException.InnerException.Message, "Update PutAsync service request");
                        }

                        // var response = client.PostAsync("http://localhost:20799/api/webapiims/PostAddNewEmployee", wamSubscriptionCreate, new System.Net.Http.Formatting.JsonMediaTypeFormatter()).Result;

                        if (response.StatusCode == HttpStatusCode.OK && response.IsSuccessStatusCode && response.Content != null)
                        {
                            Status = "S";
                            var result1 = response.Content.ReadAsStringAsync().Result;
                            var wamSubscriptionOutput = JsonConvert.DeserializeObject<List<dynamic>>(response.Content.ReadAsStringAsync().Result);
                            subscriptionGuid = wamSubscriptionOutput[0].subscriptionId;
                        }
                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            isApiTokenExpire = true;
                            JwtToken = GetJwtToken(threadID);
                            if (!string.IsNullOrEmpty(JwtToken))
                                sqlDBHelper?.UpdateWamAPIToken(JwtToken, ParamId);
                            _logger.LogError("PutAsync resuest is unauthorized." + response.ToString());
                            CreateSubscriptionExpection("PutAsync resuest is unauthorized." + response.ToString(), "Update PutAsync service request");

                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            _logger.LogError("WAM Data Subriction Error. ErrorCd: " + "url not found");
                            CreateSubscriptionExpection("WAM Data Subriction Error. ErrorCd: " + "url not found", "Update PutAsync service request");
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {

                            Status = "E";
                            WamSubscriptionError wamSubscriptionError = JsonConvert.DeserializeObject<WamSubscriptionError>(response.Content.ReadAsStringAsync().Result);
                            if (wamSubscriptionError != null)
                            {
                                _logger.LogError("WAM Data Subriction  Error . Message: " + wamSubscriptionError.Message + " Error. ExceptionMessage: " + wamSubscriptionError.ExceptionMessage + "Error Description: " + wamSubscriptionError.StackTrace);
                                CreateSubscriptionExpection("WAM Data Subriction  Error . Message: " + wamSubscriptionError.Message + " Error. ExceptionMessage: " + wamSubscriptionError.ExceptionMessage + "Error Description: " + wamSubscriptionError.StackTrace, "Update PutAsync service request");
                            }
                        }
                        else
                        {
                            Status = "E";
                            _logger.LogError("WAM Data Subriction Error. " + response.ToString());
                            CreateSubscriptionExpection("WAM Data Subriction Error. " + response.ToString(), "Update PutAsync service request");
                        }
                    }
                } while (isApiTokenExpire);
                if (subscriptionGuid != null)
                    sqlDBHelper?.UpdateWamDataSubscription(subscription.WAMDataSubscriptionId, subscriptionId, Status, subscriptionGuid);


            }
            catch (Exception ex)
            {
                CreateSubscriptionExpection("WAM Data Subriction Error. Message: " + ex.Message + "Error StackTrace: " + ex.StackTrace + "Inner Expection" + ex.InnerException, "Create PutAsync service request");
                throw ex;
            }
        }


        public void DeleteDataWAMSubscription(T_WAMDataSubscription subscription, string JwtToken, long threadID)
        {

            try
            {
                bool isApiTokenExpire = false;
                string Status = "";
                do
                {
                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    };

                    using (HttpClient client = new HttpClient(handler))
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        isApiTokenExpire = false;
                        client.BaseAddress = new Uri(WamSubscriptionBaseUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("consumer-key", WamConsumerKey);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
                        HttpResponseMessage response = new HttpResponseMessage();
                        try
                        {
                            response = client.DeleteAsync("api/equipmentwatch/subscribe?subscriberId=" + subscription.WamSubscriptionGuid).Result;
                        }
                        catch (Exception ex)
                        {
                            if (response.StatusCode == HttpStatusCode.OK && response.Content == null)
                            {
                                isApiTokenExpire = true;
                                JwtToken = GetJwtToken(threadID);
                                if (!string.IsNullOrEmpty(JwtToken))
                                    sqlDBHelper?.UpdateWamAPIToken(JwtToken, ParamId);
                                _logger.LogError("DeleteAsync resuest is unauthorized becuase  of sevice is restarted." + ex.InnerException.InnerException.Message);
                                CreateSubscriptionExpection("DeleteAsync resuest is unauthorized becuase  of sevice is restarted." + ex.InnerException.InnerException.Message, "DeleteAsync service request");

                            }
                            else
                                CreateSubscriptionExpection(ex.InnerException.InnerException.Message, "DeleteAsync service request");
                        }

                        //Int64 subscriptionId = 0;
                        //string subscriptionGuid = "";
                        if (response.StatusCode == HttpStatusCode.OK && response.IsSuccessStatusCode && response.Content != null)
                        {
                            Status = "S";
                            string result2 = response.Content.ReadAsStringAsync().Result;

                        }
                        else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            isApiTokenExpire = true;
                            JwtToken = GetJwtToken(threadID);
                            if (!string.IsNullOrEmpty(JwtToken))
                                sqlDBHelper?.UpdateWamAPIToken(JwtToken, ParamId);
                            _logger.LogError("DeleteAsync resuest is unauthorized." + response.ToString());
                            CreateSubscriptionExpection("DeleteAsync resuest is unauthorized." + response.ToString(), "DeleteAsync service request");

                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            Status = "E";
                            _logger.LogError("WAM Data Subriction Error. ErrorCd: " + "url not found");
                            CreateSubscriptionExpection("WAM Data Subriction Error. ErrorCd: " + "url not found", "DeleteAsync service request");
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            Status = "E";
                            WamSubscriptionError wamSubscriptionError = JsonConvert.DeserializeObject<WamSubscriptionError>(response.Content.ReadAsStringAsync().Result);
                            if (wamSubscriptionError != null)
                            {
                                _logger.LogError("WAM Data Subriction  Error . Message: " + wamSubscriptionError.Message + "Error. ExceptionMessage: " + wamSubscriptionError.ExceptionMessage + "Error Description: " + wamSubscriptionError.StackTrace);
                                CreateSubscriptionExpection("WAM Data Subriction Error . Message: " + wamSubscriptionError.Message + "Error. ExceptionMessage: " + wamSubscriptionError.ExceptionMessage + "Error Description: " + wamSubscriptionError.StackTrace, "DeleteAsync service request");
                            }
                        }
                        else
                        {
                            Status = "E";
                            _logger.LogError("WAM Data Subriction Error. " + response.ToString());
                            CreateSubscriptionExpection("WAM Data Subriction Error. " + response.ToString(), "DeleteAsync service request");
                        }
                    }

                } while (isApiTokenExpire);
                sqlDBHelper?.UpdateWamDataSubscriptionForInactive(subscription.WAMDataSubscriptionId, Status);



            }
            catch (Exception ex)
            {
                CreateSubscriptionExpection("WAM Data Subriction Error. Message: " + ex.Message + "Error StackTrace: " + ex.StackTrace + "Inner Expection" + ex.InnerException, "Create DeleteAsync service request");
                throw;
            }
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
                sqlDBHelper?.IncrementWAmAPICurrentCount(WAMAuthAPICallCurrentCount);
              

            }
            catch (Exception ex)
            {
                sqlDBHelper?.InsertThreadExceptions(threadID, "Authentication Error:" + ex.InnerException.InnerException.Message.ToString(), DateTime.Now);
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
                wamAPiParams = sqlDBHelper?.GetWAMApiParams(new List<string>() { WAMAuthAPICallCurrentCount, WAMAuthAPICallMaxCount, WAMAuthAPICallCurrentCountLstUpdTime });


                int WAMAuthAPICallMaxCountV = Convert.ToInt16(wamAPiParams.FirstOrDefault(a => a.Key == WAMAuthAPICallMaxCount).Value);
                int WAMAuthAPICallCurrentCountV = Convert.ToInt16(wamAPiParams.FirstOrDefault(a => a.Key == WAMAuthAPICallCurrentCount).Value);
                DateTime WAMAuthAPICallCurrentCountLstUpdTimeV = Convert.ToDateTime(wamAPiParams.FirstOrDefault(a => a.Key == WAMAuthAPICallCurrentCountLstUpdTime).Value);

                // Check if the last updated time is yesterday (UTC)
                DateTime yesterday = DateTime.UtcNow.AddDays(-1);

                if (WAMAuthAPICallCurrentCountLstUpdTimeV.Date <= yesterday.Date)
                {
                    // Reset the current count to zero
                    sqlDBHelper?.UpdateParamByParamCd("2", WAMAuthAPICallCurrentCount);
                    sqlDBHelper?.UpdateParamByParamCd(DateTime.Now.ToString(), WAMAuthAPICallCurrentCountLstUpdTime);
                    return false;
                }

                // Check if the current count exceeds the max count
                if (WAMAuthAPICallCurrentCountV >= WAMAuthAPICallMaxCountV)
                {
                    RWUtilities.Common.Utility.SendMessageToAzureSubscription(WAMAPIExceedAlertTopic, WAMAPIExceedAlertSub, JsonConvert.SerializeObject(new { Comment = $"Wam API {authentication_URL} has reached the limit of call count {WAMAuthAPICallCurrentCountV}" }), _logger).Wait();
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


        public void CreateSubscriptionExpection(string message, string method)
        {
            try
            {
                string logPath = _configuration?["appSettings:logpath"]; 
                System.IO.StreamWriter logFile = new System.IO.StreamWriter(logPath + "WamSubscriptionExpection_" + DateTime.Today.ToString(" MMM yyyy") + ".txt", true);

                logFile.WriteLine();
                logFile.WriteLine("-------------------------------------------------------------------------------------------");
                logFile.WriteLine(method + " " + DateTime.Now.ToString());
                logFile.WriteLine("-------------------------------------------------------------------------------------------");
                logFile.WriteLine(message);
                logFile.WriteLine();

                logFile.Close();
            }
            catch (Exception)
            {
                //throw;
            }

        }


        #endregion
    }
}
