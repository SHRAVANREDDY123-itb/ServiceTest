using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        public virtual DbSet<R_ApplicationParam> R_ApplicationParam { get; set; }

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

        public virtual DataSet GetServiceDefinition(string sSysService_Cd)
        {
            var result = new DataSet();

            try
            {
                using var connection = Database.GetDbConnection();
                using var command = connection.CreateCommand();

                command.CommandText = "TSP_GetSysService";
                command.CommandType = CommandType.StoredProcedure;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "ipSysService_Cd";
                parameter.Value = sSysService_Cd ?? throw new ArgumentNullException(nameof(sSysService_Cd));
                command.Parameters.Add(parameter);

                connection.Open();

                using (var adapter = new SqlDataAdapter((SqlCommand)command))
                {
                    adapter.TableMappings.Add("Table", "RG_SysService");
                    adapter.TableMappings.Add("Table1", "RG_SysServiceThreads");

                    adapter.Fill(result);
                }
            }
            catch (Exception ex)
            {
                // Log exception (replace with actual logging if available)
                throw new InvalidOperationException("Failed to execute GetServiceDefinition", ex);
            }

            return result;
        }


        public async Task<DataTable> GetThreadConfigurationAsync(long threadId)
        {
            try
            {
                var result = new DataTable();

                var connection = Database.GetDbConnection();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "TSP_GetSysServiceThread";
                    command.CommandType = CommandType.StoredProcedure;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "ipThreadId";
                    parameter.Value = threadId;
                    command.Parameters.Add(parameter);

                    connection.Open();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        result.Load(reader);
                    }

                    connection.Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public async Task<bool> UpdateServiceThreadAsync(DataRow serviceThreadRow)
        {
            try
            {
                var connection = Database.GetDbConnection();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "TSP_UpdateSysServiceThread";
                    command.CommandType = CommandType.StoredProcedure;

                    // Map DataRow values to parameters
                    foreach (DataColumn column in serviceThreadRow.Table.Columns)
                    {
                        var parameter = command.CreateParameter();
                        parameter.ParameterName = column.ColumnName;
                        parameter.Value = serviceThreadRow[column];
                        command.Parameters.Add(parameter);
                    }

                    connection.Open();
                    await command.ExecuteNonQueryAsync();
                    connection.Close();
                }

                return true;
            }
            catch (Exception)
            {
               
                throw;
            }
        }

        public async Task<long> InsertThreadExceptionLogAsync(long threadId, string exceptionMessage, DateTime createdAt)
        {
            try
            {
                long result;

                var connection = Database.GetDbConnection();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "TSP_InsertThreadExceptionLog";
                    command.CommandType = CommandType.StoredProcedure;

                    var threadIdParam = command.CreateParameter();
                    threadIdParam.ParameterName = "ipThreadId";
                    threadIdParam.Value = threadId;
                    command.Parameters.Add(threadIdParam);

                    var exceptionMessageParam = command.CreateParameter();
                    exceptionMessageParam.ParameterName = "ipExceptionMessage";
                    exceptionMessageParam.Value = exceptionMessage;
                    command.Parameters.Add(exceptionMessageParam);

                    var createdAtParam = command.CreateParameter();
                    createdAtParam.ParameterName = "ipCreatedAt";
                    createdAtParam.Value = createdAt;
                    command.Parameters.Add(createdAtParam);

                    var outputParam = command.CreateParameter();
                    outputParam.ParameterName = "opLogId";
                    outputParam.DbType = DbType.Int64;
                    outputParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParam);

                    connection.Open();
                    await command.ExecuteNonQueryAsync();

                    result = Convert.ToInt64(outputParam.Value);

                    connection.Close();
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
