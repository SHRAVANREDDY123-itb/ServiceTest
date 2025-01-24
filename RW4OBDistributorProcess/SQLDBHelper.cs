using RW4Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RW4Entities.Models.RWOBDistributorsEntities;
using RW4Entities.Models.RWServiceManagerEntities;
using Microsoft.Extensions.DependencyInjection;

namespace RW4OBDistributorProcess
{
    public class SQLDBHelper
    {
       
       
        RWOBDistributorsEntities obdb;
        RWServiceManagerEntities db;
        public SQLDBHelper( IServiceScopeFactory scopeFactory)
        {
           
           
            //this.obdb = obdb;
            //this.db = db;
        }


        public List<R_SysServiceThreadParams> GetListServiceThreadParamById(long threadId)
        {
            try
            {

                var paramValue = (from a in db.R_SysServiceThreadParams
                                  where a.SysServiceThreadId == threadId
                                  select a).ToList();

                return paramValue;
            }
            catch (Exception)
            {
                throw;
            }
        }        

        public USP_OBDist_GetInsDetailsByEventID_Result? GetInspectionDetailsByEventID(long eventID)
        {
            try
            {
                USP_OBDist_GetInsDetailsByEventID_Result inspectionDetails = obdb.USP_OBDist_GetInsDetailsByEventID(eventID).FirstOrDefault();
                return inspectionDetails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<USP_OBDist_GetInsTripDetails_Result> GetInspectionTripDetails(long prenoteID)
        {
            try
            {

                List<USP_OBDist_GetInsTripDetails_Result> lstTripDetails = obdb.USP_OBDist_GetInsTripDetails(prenoteID).ToList();
                return lstTripDetails;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public int GetCountSOSVCLOG(long soId)
        {
            try
            {

                return obdb.T_SOSvcLog.Where(x => x.SOid == soId && x.SOSvcLogStatusCd == "A").Count();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public string GetSysParamValues(string companyName, string sysParamCd, int interfaceId)
        {
            try
            {
                return obdb.USP_OBDist_GetSysParamValues(companyName, sysParamCd, interfaceId).FirstOrDefault().SysParamVal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public USP_GetPreNoteUnitStatusByEventId_Result? GetPreNoteUnitStatusByEventId(long eventId)
        {
            try
            {
                return obdb.USP_GetPreNoteUnitStatusByEventId(eventId).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeriveRailincCERLog(List<RailincCER> lstRailincCER)
        {
            List<T_RailincCERLog> lstRailincCERLog = new List<T_RailincCERLog>();
            bool flag = false;
            try
            {
                foreach (RailincCER railincCER in lstRailincCER)
                {
                    T_RailincCERLog railincCERLog = new T_RailincCERLog();
                    Guid EventGUID = Guid.NewGuid();
                    railincCERLog.FleetCd = railincCER.FleetCd;
                    railincCERLog.CreatedDtTm = DateTime.Now;
                    railincCERLog.CERLogGUID = EventGUID;
                    railincCERLog.UnitNumber = railincCER.UnitNumber;
                    railincCERLog.EventId = railincCER.EventId;
                    railincCERLog.FileName = railincCER.FileName;

                    T_PreNoteUnitEvents preNoteUnitEvents = obdb.T_PreNoteUnitEvents.Where(w => w.EventId == railincCER.EventId).FirstOrDefault();

                    if (preNoteUnitEvents != null)
                    {
                        railincCERLog.PreNoteUnitId = preNoteUnitEvents.PreNoteUnitId;
                        railincCERLog.PreNoteId = obdb.T_PreNoteUnits.Where(w => w.PreNoteUnitId == preNoteUnitEvents.PreNoteUnitId).Select(s => s.PreNoteId).FirstOrDefault();
                    }
                    else
                    {
                        T_PreNoteEvents preNoteEvents = obdb.T_PreNoteEvents.Where(w => w.EventId == railincCER.EventId).FirstOrDefault();
                        if (preNoteEvents != null)
                        {
                            railincCERLog.PreNoteId = preNoteEvents.PreNoteId;
                            R_UnitMaster unitMaster = obdb.R_UnitMaster.Where(w => w.UnitNumber.Contains(railincCER.UnitNumber.Trim())).FirstOrDefault();
                            if (unitMaster != null)
                            {
                                T_PreNoteUnits preNoteUnits = obdb.T_PreNoteUnits.Where(w => w.PreNoteId == (long)railincCERLog.PreNoteId && w.UnitMasterId == unitMaster.UnitMasterId).FirstOrDefault();
                                if (preNoteUnits != null)
                                {
                                    railincCERLog.PreNoteUnitId = preNoteUnits.PreNoteUnitId;
                                }
                            }
                        }
                        else
                        {
                            T_UnitEvents unitEvents = obdb.T_UnitEvents.Where(w => w.EventId == railincCER.EventId).FirstOrDefault();
                            if (unitEvents != null)
                            {
                                T_Events events = obdb.T_Events.Where(w => w.EventId == railincCER.EventId).FirstOrDefault();
                                if (events != null)
                                {
                                    R_EventMaster eventMaster = obdb.R_EventMaster.Where(w => w.EventMasterId == events.EventMasterId).FirstOrDefault();
                                    if (eventMaster != null)
                                    {
                                        if (eventMaster.EventCd == RW4Entities.DBConstants.TTEvents.ResendTrace)
                                        {
                                            T_UnitCurrentCondition unitCurrentCondition = obdb.T_UnitCurrentCondition.Where(w => w.UnitMasterID == unitEvents.UnitMasterId).FirstOrDefault();
                                            if (unitCurrentCondition != null && unitCurrentCondition.PreNoteId != null)
                                            {
                                                railincCERLog.PreNoteId = unitCurrentCondition.PreNoteId;
                                                railincCERLog.PreNoteUnitId = unitCurrentCondition.PreNoteUnitID;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    lstRailincCERLog.Add(railincCERLog);
                }
                InsertRailincCERLog(lstRailincCERLog);

                flag = true;
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }

        private bool InsertRailincCERLog(List<T_RailincCERLog> lstRailincCERLog)
        {
            bool flag = false;
            try
            {
                foreach (T_RailincCERLog railincCERLog in lstRailincCERLog)
                {
                    obdb.T_RailincCERLog.Add(railincCERLog);
                }
                obdb.SaveChanges();
            }
            catch (Exception)
            {
                flag = false;
                throw;
            }
            return flag;
        }

        public List<Usp_GetActivePendingPrenoteUnit_Result> GetActivePendingPrenotUnit(string unitNumber)
        {
            try
            {
                return obdb.Usp_GetActivePendingPrenoteUnit(unitNumber).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<USP_OBDist_GetPrenoteDetails_Result> GetPrenoteDetails(long eventId)
        {
            try
            {
                List<USP_OBDist_GetPrenoteDetails_Result> prenoteDetails = obdb.USP_OBDist_GetPrenoteDetails(eventId).ToList();
                return prenoteDetails;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public long CreateWamDataSubscription(long preNoteId, long unitMasterId, DateTime startDate, DateTime endDate)
        {
            try
            {
                T_WAMDataSubscription subscription = new T_WAMDataSubscription();
                subscription.PrenoteId = preNoteId;
                subscription.UnitMasterId = unitMasterId;
                subscription.StatusCd = "N";
                subscription.StartDate = startDate;
                subscription.EndDate = endDate;
                subscription.CreateDt = DateTime.Now;

                obdb.T_WAMDataSubscription.Add(subscription);
                obdb.SaveChanges();

                return subscription.WAMDataSubscriptionId;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void UpdateWamDataSubscription(long? wamDataSubscriptionId, long subscriptionId, string status, string subscriptionGuid)
        {
            try
            {
                T_WAMDataSubscription wamsubscription = (from a in obdb.T_WAMDataSubscription
                                                         where a.WAMDataSubscriptionId == wamDataSubscriptionId
                                                         select a).FirstOrDefault();



                if (wamsubscription != null)
                {
                    if (status == Status.E.ToString())
                        wamsubscription.StatusCd = status;
                    else
                    {
                        //if (subscriptionId > 0)
                        //    wamsubscription.WamSubscriptionId = subscriptionId;
                        if (subscriptionGuid != "")
                            wamsubscription.WamSubscriptionGuid = new Guid(subscriptionGuid);
                        wamsubscription.StatusCd = status;
                    }

                    wamsubscription.ModifyDt = DateTime.Now;

                    obdb.SaveChanges();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateWamDataSubscriptionForInactive(long wamDataSubscriptionId, string status)
        {
            try
            {
                T_WAMDataSubscription wamsubscription = (from a in obdb.T_WAMDataSubscription
                                                         where a.WAMDataSubscriptionId == wamDataSubscriptionId
                                                         select a).FirstOrDefault();

                if (wamsubscription != null)
                {
                    if (status == Status.E.ToString())
                        wamsubscription.StatusCd = status;
                    else
                    {
                        wamsubscription.EndDate = DateTime.Now;
                        wamsubscription.StatusCd = status;
                    }

                    wamsubscription.ModifyDt = DateTime.Now;

                    obdb.SaveChanges();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        public void UpdateWamDataSubscriptionETSDate(long wamDataSubscriptionId, string status, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                T_WAMDataSubscription wamsubscription = (from a in obdb.T_WAMDataSubscription
                                                         where a.WAMDataSubscriptionId == wamDataSubscriptionId
                                                         select a).FirstOrDefault();
                if (wamsubscription != null)
                {
                    wamsubscription.StartDate = startDate;
                    wamsubscription.EndDate = endDate;
                    wamsubscription.ModifyDt = DateTime.Now;
                    obdb.SaveChanges();
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        public T_WAMDataSubscription? GetWamDataSubcriptionDetails(long prenoteId, long unitMasterId)
        {
            try
            {
                T_WAMDataSubscription wamsubscription = (from a in obdb.T_WAMDataSubscription
                                                         where a.PrenoteId == prenoteId && a.UnitMasterId == unitMasterId
                                                         select a).FirstOrDefault();

                return wamsubscription;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void UpdateWamAPIToken(string Token, int paramId)
        {
            try
            {
                obdb.Usp_UpdateWamApiTokenByParamId(Token, paramId);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public string? GetWamSubsciptionApiToken(int paramId)
        {
            try
            {
                var wamParam = obdb.Usp_GetWamApiTokenByParamCd(paramId).FirstOrDefault();
                if (wamParam != null)
                {
                    return wamParam.WamToken;
                }
                else
                    throw new Exception("No value found for provide paramID: " + paramId);

            }
            catch (Exception)
            {

                throw;
            }
        }

       

        public List<KeyValuePair<string, string>> GetWAMApiParams(List<string> paramCDs)
        {
            try
            {
                List<KeyValuePair<string, string>> ParamsValues = (from rp in db.R_Params
                                                                   join rap in db.R_ApplicationParam on rp.ParamID equals rap.ParamID
                                                                   where paramCDs.Contains(rp.ParamCd)
                                                                   select new { rp.ParamCd, rap.Value })
                    .AsEnumerable() // Switch to LINQ to Objects
                    .Select(x => new KeyValuePair<string, string>(x.ParamCd, x.Value))
                    .ToList();

                return ParamsValues;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void IncrementWAmAPICurrentCount(string ParamCd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ParamCd))
                {
                    var PId = (from a in db.R_Params// join b in db.R_ApplicationParam on a.ParamID equals b.ParamID
                               where a.ParamCd == ParamCd
                               select a.ParamID).FirstOrDefault(); //.Select(s=>s.ParamID).FirstOrDefault();
                    var obj = db.R_ApplicationParam.Where(o => o.ParamID == PId).FirstOrDefault();

                    // Increment the current value by the specified incrementValue.
                    obj.Value = (int.Parse(obj.Value) + 1).ToString();
                    db.SaveChanges();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertThreadExceptions(long threadId, string exception, DateTime createdDtTm)
        {
            try
            {
                long param;
                db.TSP_ThreadExceptionLogIns(threadId, exception, createdDtTm, out param);
                long threadExceptionLogId = param;
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateParamByParamCd(string Pvalue, string ParamCd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ParamCd))
                {
                    var PId = (from a in db.R_Params// join b in db.R_ApplicationParam on a.ParamID equals b.ParamID
                               where a.ParamCd == ParamCd
                               select a.ParamID).FirstOrDefault(); //.Select(s=>s.ParamID).FirstOrDefault();
                    var obj = db.R_ApplicationParam.Where(o => o.ParamID == PId).FirstOrDefault();
                    obj.Value = Pvalue;
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
