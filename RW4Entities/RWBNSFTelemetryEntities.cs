

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RW4Entities.Models.RWOBDistributorsEntities;
using RW4Entities.Models.RWBNSFTelemetryEntities;
using System.Data.Objects;

namespace RW4Entities
{
    public class RWBNSFTelemetryEntities : DbContext
    {
        public RWBNSFTelemetryEntities(DbContextOptions<RWBNSFTelemetryEntities> dbContextOptions):base(dbContextOptions)
        {
                
        }      
      
     
        private DbSet<USP_GetWamsubscriptionIdByUnits_Result> USP_GetWamsubscriptionIdByUnits_Result { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<USP_GetWamsubscriptionIdByUnits_Result>()
                .HasNoKey();
           

        }

        public virtual IEnumerable<USP_GetWamsubscriptionIdByUnits_Result> USP_GetWamsubscriptionIdByUnits()
        {
           
            return USP_GetWamsubscriptionIdByUnits_Result.FromSqlRaw("exec USP_GetWamsubscriptionIdByUnits").ToList();
        }

        public virtual int Usp_UpdateWamApiTokenByParamId(string wamApiToken, int? paramId)
        {
            var wamApiTokenParameter = wamApiToken != null ?
                new SqlParameter("@WamApiToken", wamApiToken) :
                new SqlParameter("@WamApiToken", DBNull.Value);

            var paramIdParameter = paramId.HasValue ?
                new SqlParameter("@ParamId", paramId) :
                new SqlParameter("@ParamId", DBNull.Value);

            return Database.ExecuteSqlRaw("exec Usp_UpdateWamApiTokenByParamId @WamApiToken, @ParamId", wamApiTokenParameter, paramIdParameter);
        }

      
        public virtual IEnumerable<string> Usp_GetWamApiTokenByParamCd(int? paramId)
        {
            var paramIdParameter = paramId.HasValue ?
                new SqlParameter("@ParamId", paramId) :
                new SqlParameter("@ParamId", DBNull.Value);

            return Database.SqlQueryRaw<string>("exec Usp_GetWamApiTokenByParamCd @ParamId", paramIdParameter);
                
        }

        public virtual int USP_UpdateWAMDataSubscriptionForAPIStartDt(     Nullable<System.Guid> wamSubsriptionGuid,
     Nullable<long> prenoteId,
     Nullable<long> unitMasterId,
     string appiStartDttm)
        {
            var wamSubsriptionGuidParameter = new SqlParameter("@WamSubsriptionGuid",
                wamSubsriptionGuid.HasValue ? (object)wamSubsriptionGuid.Value : DBNull.Value);

            var prenoteIdParameter = new SqlParameter("@PrenoteId",
                prenoteId.HasValue ? (object)prenoteId.Value : DBNull.Value);

            var unitMasterIdParameter = new SqlParameter("@UnitMasterId",
                unitMasterId.HasValue ? (object)unitMasterId.Value : DBNull.Value);

            var appiStartDttmParameter = new SqlParameter("@AppiStartDttm",
                !string.IsNullOrEmpty(appiStartDttm) ? (object)appiStartDttm : DBNull.Value);

            return Database.ExecuteSqlRaw(
                "EXEC USP_UpdateWAMDataSubscriptionForAPIStartDt @WamSubsriptionGuid, @PrenoteId, @UnitMasterId, @AppiStartDttm",
                wamSubsriptionGuidParameter, prenoteIdParameter, unitMasterIdParameter, appiStartDttmParameter);
        }





    }


}
