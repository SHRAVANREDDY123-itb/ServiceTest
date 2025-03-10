﻿
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Collections.Concurrent;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace RWUtilities.Common
{
    public static class Utility
    {

        public static string? connectionString;       
       
        static List<ServiceBusReceivedMessage>? obReceivedMessageList;      

        

        public static async Task SendMessageToAzureSubscription(string topicName, string subscriptionNm, string jsonMessage, ILogger logger)
        {


            try
            {

                string SubscriptNm = subscriptionNm;
                string queuetopicName = topicName;
                var Client = new ServiceBusClient(connectionString);
                var Sender = Client.CreateSender(topicName);               
                ServiceBusMessage message = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage));
                message.ApplicationProperties["SubscriptionNm"] = subscriptionNm;
                await Sender.SendMessageAsync(message);
                await Sender.CloseAsync();
            }
            catch (Exception ex)
            {

                logger.LogError($"Error in SendMessageToAzureSubscription, error message {ex.Message}, stracktrace {ex.StackTrace}");
            }
        }

        public  static async Task<List<DistributorJson>> GetQueueMessagefromOBSAzureSubscription(string topicName, string subscriptionName, bool isbatchprocess, int messageCount,  ILogger logger)
        {

            ConcurrentBag<DistributorJson> msgList = new ConcurrentBag<DistributorJson>();
            obReceivedMessageList = new List<ServiceBusReceivedMessage>();
            try
            {

                TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(10);

                var obSubscriptionClient = new ServiceBusClient(connectionString);
                if (isbatchprocess)
                {
                    ServiceBusReceiver receiver = obSubscriptionClient.CreateReceiver(topicName, subscriptionName);
                    
                    IReadOnlyList<ServiceBusReceivedMessage> receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages: messageCount);
                    
                    Parallel.ForEach(receivedMessages, receivemsg =>
                    {
                        if (receivemsg != null)
                        {
                            DistributorJson dct = new DistributorJson();


                            dct.TriggerCondition = receivemsg.ApplicationProperties["TriggerCondition"].ToString();
                            dct.TriggerCd = receivemsg.ApplicationProperties["TriggerCd"].ToString();
                            dct.TriggerID = receivemsg.ApplicationProperties["TriggerID"].ToString();
                            dct.TriggerDtTm = receivemsg.ApplicationProperties["TriggerDtTm"].ToString();
                            dct.PSP = receivemsg.ApplicationProperties["PSP"].ToString();
                            dct.Line = receivemsg.ApplicationProperties["Line"].ToString();
                            dct.Unit = receivemsg.ApplicationProperties["Unit"].ToString();
                            string msgData = receivemsg.Body.ToString();
                            dct.Json = msgData;
                            
                            obReceivedMessageList.Add(receivemsg);
                            msgList.Add(dct);
                            
                        }
                    });
                }
                else
                {
                    ServiceBusReceiver receiver = obSubscriptionClient.CreateReceiver(topicName, subscriptionName);
                    ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync(ReceiveTimeout);
                    
                        if (receivedMessage != null)
                        {
                            DistributorJson dct = new DistributorJson();

                            dct.TriggerCondition = receivedMessage.ApplicationProperties["TriggerCondition"].ToString();
                            dct.TriggerCd = receivedMessage.ApplicationProperties["TriggerCd"].ToString();
                            dct.TriggerID = receivedMessage.ApplicationProperties["TriggerID"].ToString();
                            dct.TriggerDtTm = receivedMessage.ApplicationProperties["TriggerDtTm"].ToString();
                            dct.PSP = receivedMessage.ApplicationProperties["PSP"].ToString();
                            dct.Line = receivedMessage.ApplicationProperties["Line"].ToString();
                            dct.Unit = receivedMessage.ApplicationProperties["Unit"].ToString();
                        //TODO some special characters attched before message
                            string msgData = receivedMessage.Body.ToString();
                            obReceivedMessageList.Add(receivedMessage);
                            dct.Json = msgData;

                            msgList.Add(dct);

                        }
                   
                }
               


            }
            catch (Exception ex)
            {
                logger.LogError($"Error in GetQueueMessagefromOBSAzureSubscription, subscription name {subscriptionName}, error message {ex.Message}, stracktrace {ex.StackTrace}");
                throw;
            }
            return msgList.ToList();

        }

        public static async  Task DeleteMessagebasedonProperties(string topicName, string subscriptionName, string PropertyName, string PropertyValue, ILogger logger)
        {
            try
            {
                var obSubscriptionClient = new ServiceBusClient(connectionString);
                ServiceBusReceiver receiver = obSubscriptionClient.CreateReceiver(topicName, subscriptionName);
                
                ServiceBusReceivedMessage receiveMessage = obReceivedMessageList.Where(p => p.ApplicationProperties[PropertyName].ToString() == PropertyValue).FirstOrDefault();
                
                await receiver.CompleteMessageAsync(receiveMessage);
               
               
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in DeleteMessagebasedonProperties, error message {ex.Message}, stracktrace {ex.StackTrace}");
                throw;
            }
           
        }

        public static string SendMail(ILogger logger,string strFrom, string strTo, string strSubject, string strBody, string strMailServerName, string attachmentFileName = "", List<Attachment> mailAttachments = null)
        {
            try
            {
                if (strTo.Trim() != "-")
                {
                    using (MailMessage mail = new MailMessage(strFrom, strTo, strSubject, strBody))
                    {
                        mail.IsBodyHtml = true;
                        if (!string.IsNullOrEmpty(attachmentFileName))
                        {
                            Attachment attachment = new Attachment(attachmentFileName);
                            mail.Attachments.Add(attachment);
                           
                        }
                        if (mailAttachments != null && mailAttachments.Count > 0)
                        {
                            foreach (var item in mailAttachments)
                            {
                                mail.Attachments.Add(item);
                            }

                        }

                        using (SmtpClient SmtpMail = new SmtpClient())
                        {
                            SmtpMail.Host = strMailServerName;
                            SmtpMail.UseDefaultCredentials = true;
                            SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;  
                            SmtpMail.Send(mail);
                        }
                        return "";

                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                logger.LogError("ToEmailId: " + strTo + " Subject: " + strSubject + " Exception: " + ex);
                logger.LogError($"Error in SendMail, error message {ex.Message}, stracktrace {ex.StackTrace}");
                return "ToEmailId: " + strTo + " Subject: " + strSubject + " Exception: " + ex.Message;
            }
        }

    }


    public class DistributorJson
    {
        public string? TriggerCondition { get; set; }
        public string? TriggerCd { get; set; }
        public string? TriggerID { get; set; }
        public string? TriggerDtTm { get; set; }
        public string? TriggerMsgType { get; set; }
        public string? PSP { get; set; }
        public string? Line { get; set; }
        public string? Unit { get; set; }
        public string? Json { get; set; }


    }




}
