using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RW4Entities.Models;
using RW4Entities.Models.RWServiceManagerEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities
{
    public class RWServiceManagerEntities:DbContext
    {
        public RWServiceManagerEntities(DbContextOptions<RWServiceManagerEntities> dbContextOptions) : base(dbContextOptions)
        {

        }

        public virtual DbSet<R_Params> R_Params { get; set; }
        public virtual DbSet<R_SysService> R_SysService { get; set; }
        public virtual DbSet<R_ApplicationParam> R_ApplicationParam { get; set; }
        public virtual DbSet<R_SysServiceThreads> R_SysServiceThreads { get; set; }
        public virtual DbSet<T_ThreadExceptionLog> T_ThreadExceptionLog { get; set; }
        public virtual int TSP_ThreadExceptionLogIns(long? ipSysServiceThread_Id, string ipThreadException, DateTime? ipCreate_DtTm, out long op_SysServiceThreadLog_Id)
        {
            var ipSysServiceThread_IdParameter = ipSysServiceThread_Id.HasValue ?
                new SqlParameter("ipSysServiceThread_Id", ipSysServiceThread_Id) :
                new SqlParameter("ipSysServiceThread_Id", DBNull.Value);

            var ipThreadExceptionParameter = ipThreadException != null ?
                new SqlParameter("ipThreadException", ipThreadException) :
                new SqlParameter("ipThreadException", DBNull.Value);

            var ipCreate_DtTmParameter = ipCreate_DtTm.HasValue ?
                new SqlParameter("ipCreate_DtTm", ipCreate_DtTm) :
                new SqlParameter("ipCreate_DtTm", DBNull.Value);

            var op_SysServiceThreadLog_IdParameter = new SqlParameter
            {
                ParameterName = "op_SysServiceThreadLog_Id",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Output
            };

           int O= Database.ExecuteSqlRaw("EXEC TSP_ThreadExceptionLogIns @ipSysServiceThread_Id, @ipThreadException, @ipCreate_DtTm, @op_SysServiceThreadLog_Id OUTPUT",
                                    ipSysServiceThread_IdParameter, ipThreadExceptionParameter, ipCreate_DtTmParameter, op_SysServiceThreadLog_IdParameter);

            op_SysServiceThreadLog_Id = (long)op_SysServiceThreadLog_IdParameter.Value;

            return O; // Return value as needed
        }
        public virtual DbSet<R_SysServiceThreadParams> R_SysServiceThreadParams { get; set; }

    }
}
