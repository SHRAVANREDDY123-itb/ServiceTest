using RW4Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RW4Entities.Models.RWServiceManagerEntities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RW4Entities.Models.RWBNSFTelemetryEntities;
using System.Threading;
using RW4Entities.DBConstants;

namespace RW4BNSFTelemetry
{
    public class DBHelper
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DBHelper(IServiceScopeFactory scopeFactory) { _scopeFactory = scopeFactory; }


        public int? GetThreadSleepTime(long threadId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    int? paramValue = (from a in db.R_SysServiceThreads
                                       where a.SysServiceThreadId == threadId
                                       select a.ThreadSleepTm).FirstOrDefault();

                    return paramValue;

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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
                    long param;
                    db.TSP_ThreadExceptionLogIns(threadId, exception, createdDtTm, out param);
                    long threadExceptionLogId = param;
                    return true;
                }

            }
            catch (Exception)
            {
                throw;
            }
          
        }
        public List<USP_GetWamsubscriptionIdByUnits_Result> GetWamsubscriptionIdsByUnits()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWBNSFTelemetryEntities>();
                    List<USP_GetWamsubscriptionIdByUnits_Result> lstUnits = new List<USP_GetWamsubscriptionIdByUnits_Result>();
                    lstUnits = db.USP_GetWamsubscriptionIdByUnits().ToList();
                    return lstUnits;
                }

            }
            catch (Exception)
            {
                throw;
            }

           
        }
        public void UpdatePramData(string Token, int paramId)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWBNSFTelemetryEntities>();

                    db.Usp_UpdateWamApiTokenByParamId(Token, paramId);
                }


            }
            catch (Exception)
            {

                throw;
            }

        }
        public string GetParamData(int paramId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWBNSFTelemetryEntities>();

                    return db.Usp_GetWamApiTokenByParamCd(paramId)?.FirstOrDefault() ?? string.Empty;

                }

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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();
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

            }
            catch (Exception)
            {
                throw;
            }
           
        }
        public void UpdateWamSubscriptionForAPIStartDt(Guid? wamSubscriptionGuid, long? prenoteId, long? unitmasterId, string APIStartDttm)
        {

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWBNSFTelemetryEntities>();

                    db.USP_UpdateWAMDataSubscriptionForAPIStartDt(wamSubscriptionGuid, prenoteId, unitmasterId, APIStartDttm);
                }


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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();

                    List<KeyValuePair<string, string>> ParamsValues = (from rp in db.R_Params
                                                                       join rap in db.R_ApplicationParam on rp.ParamID equals rap.ParamID
                                                                       where paramCDs.Contains(rp.ParamCd)
                                                                       select new { rp.ParamCd, rap.Value })
                   .AsEnumerable() // Switch to LINQ to Objects
                   .Select(x => new KeyValuePair<string, string>(x.ParamCd, x.Value))
                   .ToList();

                    return ParamsValues;
                }



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
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RWServiceManagerEntities>();


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


            }
            catch (Exception)
            {
                throw;
            }
           
        }
    }
  
}
