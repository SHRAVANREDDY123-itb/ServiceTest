

using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using RW4Entities.Models.RWOBDistributorsEntities;


namespace RW4Entities
{
    public class RWOBDistributorsEntities:DbContext
    {
        public RWOBDistributorsEntities(DbContextOptions<RWOBDistributorsEntities> dbContextOptions):base(dbContextOptions)
        {
                
        }

       
        public virtual DbSet<R_EventMaster> R_EventMaster { get; set; }
        public virtual DbSet<R_UnitMaster> R_UnitMaster { get; set; }
        public virtual DbSet<T_Events> T_Events { get; set; }
        public virtual DbSet<T_PreNoteEvents> T_PreNoteEvents { get; set; }
       
        public virtual DbSet<T_PreNoteUnitEvents> T_PreNoteUnitEvents { get; set; }
        public virtual DbSet<T_PreNoteUnits> T_PreNoteUnits { get; set; }
        public virtual DbSet<T_PreNotes> T_PreNotes { get; set; }
        public virtual DbSet<T_RailincCERLog> T_RailincCERLog { get; set; }
        public virtual DbSet<T_UnitCurrentCondition> T_UnitCurrentCondition { get; set; }
      
        public virtual DbSet<T_UnitEvents> T_UnitEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usp_GetActivePendingPrenoteUnit_Result>()
                .HasNoKey();

            modelBuilder.Entity<USP_GetPreNoteUnitStatusByEventId_Result>()
               .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetCCMessage_Result>()
               .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetWAMSubscriptionMessage_Result>()
              .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetOCLMessage_Result>()
            .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetSysParamValues_Result>()
           .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetRAILMessageBatch_Result>()
            .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetMessageBatch_Result>()
            .HasNoKey();

            modelBuilder.Entity<USP_OBDist_GetServiceQueue_Result>()
            .HasNoKey();

        }
        private  DbSet<Usp_GetActivePendingPrenoteUnit_Result> Usp_GetActivePendingPrenoteUnit_Result { get; set; }
        private DbSet<USP_GetPreNoteUnitStatusByEventId_Result> USP_GetPreNoteUnitStatusByEventId_Result { get; set; }
        private DbSet<USP_OBDist_GetCCMessage_Result> USP_OBDist_GetCCMessage_Result { get; set; }
        private DbSet<USP_OBDist_GetWAMSubscriptionMessage_Result> USP_OBDist_GetWAMSubscriptionMessage_Result { get; set; }
        private DbSet<USP_OBDist_GetOCLMessage_Result> USP_OBDist_GetOCLMessage_Result { get; set; }
        private DbSet<USP_OBDist_GetSysParamValues_Result> USP_OBDist_GetSysParamValues_Result { get; set; }
        private DbSet<USP_OBDist_GetRAILMessageBatch_Result> USP_OBDist_GetRAILMessageBatch_Result { get; set; }
        public DbSet<USP_OBDist_GetMessageBatch_Result> USP_OBDist_GetMessageBatch_Result { get; set; }

        public DbSet<USP_OBDist_GetServiceQueue_Result> USP_OBDist_GetServiceQueue_Result { get; set; }

        //TODO This following Database.ExecuteSqlRaw just returns number of rows
        public virtual int USP_OBDist_EndConversation(Guid? conversationHandle)
        {
            var conversationHandleParameter = conversationHandle.HasValue ?
                new SqlParameter("@ConversationHandle", conversationHandle) :
                new SqlParameter("@ConversationHandle", DBNull.Value);

            return Database.ExecuteSqlRaw("exec USP_OBDist_EndConversation @ConversationHandle", conversationHandleParameter);
        }

        public virtual IEnumerable<USP_OBDist_GetServiceQueue_Result> USP_OBDist_GetServiceQueue(string queueName)
        {
            var queueNameParameter = queueName != null ?
                new SqlParameter("@QueueName", queueName) :
                new SqlParameter("@QueueName", DBNull.Value);

            return USP_OBDist_GetServiceQueue_Result.FromSqlRaw<USP_OBDist_GetServiceQueue_Result>("exec USP_OBDist_GetServiceQueue @QueueName", queueNameParameter).ToList();
        }

        public virtual IEnumerable<USP_OBDist_GetMessageBatch_Result> USP_OBDist_GetMessageBatch(int? count, int? timeout, string queueName)
        {
            var countParameter = count.HasValue ?
                new SqlParameter("@count", count) :
                new SqlParameter("@count", DBNull.Value);

            var timeoutParameter = timeout.HasValue ?
                new SqlParameter("@timeout", timeout) :
                new SqlParameter("@timeout", DBNull.Value);

            var queueNameParameter = queueName != null ?
                new SqlParameter("@queueName", queueName) :
                new SqlParameter("@queueName", DBNull.Value);

            return USP_OBDist_GetMessageBatch_Result.FromSqlRaw<USP_OBDist_GetMessageBatch_Result>("exec USP_OBDist_GetMessageBatch @count, @timeout, @queueName", countParameter, timeoutParameter, queueNameParameter).ToList();
        }

        public virtual IEnumerable<USP_OBDist_GetRAILMessageBatch_Result> USP_OBDist_GetRAILMessageBatch(int? count)
        {
            var countParameter = count.HasValue ?
                new SqlParameter("@count", count) :
                new SqlParameter("@count", DBNull.Value);

            return USP_OBDist_GetRAILMessageBatch_Result.FromSqlRaw<USP_OBDist_GetRAILMessageBatch_Result>("exec USP_OBDist_GetRAILMessageBatch @count", countParameter).ToList();
        }

        public virtual IEnumerable<USP_OBDist_GetSysParamValues_Result> USP_OBDist_GetSysParamValues(string companyNm, string sysParamCd, int? interfaceId)
        {
            var companyNmParameter = companyNm != null ?
                new SqlParameter("CompanyNm", companyNm) :
                new SqlParameter("CompanyNm", typeof(string));

            var sysParamCdParameter = sysParamCd != null ?
                new SqlParameter("SysParamCd", sysParamCd) :
                new SqlParameter("SysParamCd", typeof(string));

            var interfaceIdParameter = interfaceId.HasValue ?
                new SqlParameter("InterfaceId", interfaceId) :
                new SqlParameter("InterfaceId", typeof(int));

            return USP_OBDist_GetSysParamValues_Result.FromSqlRaw("exec USP_OBDist_GetSysParamValues @CompanyNm, @SysParamCd,@InterfaceId ", companyNmParameter, sysParamCdParameter, interfaceIdParameter);
            
        }

        public virtual IEnumerable<USP_OBDist_GetOCLMessage_Result> USP_OBDist_GetOCLMessage()
        {
            return USP_OBDist_GetOCLMessage_Result.FromSqlRaw("exec USP_OBDist_GetOCLMessage");
        }

        public virtual IEnumerable<USP_OBDist_GetWAMSubscriptionMessage_Result> USP_OBDist_GetWAMSubscriptionMessage()
        {
            return USP_OBDist_GetWAMSubscriptionMessage_Result.FromSqlRaw("exec USP_OBDist_GetWAMSubscriptionMessage");
        }

        public virtual IEnumerable<USP_OBDist_GetCCMessage_Result> USP_OBDist_GetCCMessage(string oBCCQueue)
        {
            var oBCCQueueParameter = oBCCQueue != null ?
                new SqlParameter("OBCCQueue", oBCCQueue) :
                new SqlParameter("OBCCQueue", typeof(string));

            return USP_OBDist_GetCCMessage_Result.FromSqlRaw("exec USP_OBDist_GetCCMessage @OBCCQueue", oBCCQueueParameter);
        }

        public virtual IEnumerable<Usp_GetActivePendingPrenoteUnit_Result> Usp_GetActivePendingPrenoteUnit(string unitNumber)
        {
            var unitNumberParameter = unitNumber != null ?
               new SqlParameter("UnitNumber", unitNumber) :
               new SqlParameter("UnitNumber", typeof(string));

           return  Usp_GetActivePendingPrenoteUnit_Result.FromSqlRaw("exec Usp_GetActivePendingPrenoteUnit @UnitNumber", unitNumberParameter);          
          
        }

        public virtual IEnumerable<USP_GetPreNoteUnitStatusByEventId_Result> USP_GetPreNoteUnitStatusByEventId(long? eventId)
        {
            var eventIdParameter = eventId.HasValue ?
                new SqlParameter("EventId", eventId) :
                new SqlParameter("EventId", typeof(long));

            return USP_GetPreNoteUnitStatusByEventId_Result.FromSqlRaw("exec USP_GetPreNoteUnitStatusByEventId @EventId", eventIdParameter);
        }


        public virtual DbSet<T_SOSvcLog> T_SOSvcLog { get; set; }
        public virtual DbSet<T_WAMDataSubscription> T_WAMDataSubscription { get; set; }

        public DbSet<USP_OBDist_GetInsTripDetails_Result> USP_OBDist_GetInsTripDetails_Result { get; set; }
        public DbSet<USP_OBDist_GetPrenoteDetails_Result> USP_OBDist_GetPrenoteDetails_Result { get; set; }
        public DbSet<USP_OBDist_GetInsDetailsByEventID_Result> USP_OBDist_GetInsDetailsByEventID_Result { get; set; }
        public DbSet<USP_MirgrateExstingWamSubscriptionToNewSubscriptionUrl_Result> USP_MirgrateExstingWamSubscriptionToNewSubscriptionUrl_Result { get; set; }



        public virtual IEnumerable<USP_OBDist_GetInsTripDetails_Result> USP_OBDist_GetInsTripDetails(long? prenoteId)
        {
            var prenoteIdParameter = prenoteId.HasValue ?
                new SqlParameter("@PrenoteId", prenoteId) :
                new SqlParameter("@PrenoteId", DBNull.Value);

            return USP_OBDist_GetInsTripDetails_Result.FromSqlRaw("exec USP_OBDist_GetInsTripDetails @PrenoteId", prenoteIdParameter).ToList();
        }

        public virtual IEnumerable<USP_OBDist_GetPrenoteDetails_Result> USP_OBDist_GetPrenoteDetails(long? eventId)
        {
            var eventIdParameter = eventId.HasValue ?
                new SqlParameter("@EventId", eventId) :
                new SqlParameter("@EventId", DBNull.Value);

            return USP_OBDist_GetPrenoteDetails_Result.FromSqlRaw("exec USP_OBDist_GetPrenoteDetails @EventId", eventIdParameter).ToList();
        }

        public virtual IEnumerable<USP_OBDist_GetInsDetailsByEventID_Result> USP_OBDist_GetInsDetailsByEventID(long? eventId)
        {
            var eventIdParameter = eventId.HasValue ?
                new SqlParameter("@EventId", eventId) :
                new SqlParameter("@EventId", DBNull.Value);

            return USP_OBDist_GetInsDetailsByEventID_Result.FromSqlRaw("exec USP_OBDist_GetInsDetailsByEventID @EventId", eventIdParameter).ToList();
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

        public DbSet<Usp_GetWamApiTokenByParamCd_Result> Usp_GetWamApiTokenByParamCd_Result { get; set; }
        public virtual IEnumerable<Usp_GetWamApiTokenByParamCd_Result> Usp_GetWamApiTokenByParamCd(int? paramId)
        {
            var paramIdParameter = paramId.HasValue ?
                new SqlParameter("@ParamId", paramId) :
                new SqlParameter("@ParamId", DBNull.Value);

            return Usp_GetWamApiTokenByParamCd_Result.FromSqlRaw("exec Usp_GetWamApiTokenByParamCd @ParamId", paramIdParameter);
        }

    }


}
