using DBConstants;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Utility.DataSetManagement;
//TODO remove sqlclient and use context instead in future EF core

namespace ServiceManagerRW4
{
    public class ServiceDB
    {
        private  readonly string? sConnectString;      

        public ServiceDB(IConfiguration configuration)
        {
            string? DBConnectionName = configuration["appSettings:DBConnectionName"];
            sConnectString = configuration["appSettings:" + DBConnectionName];
                        
        }

        #region " GET Methods "

        /// <summary>
        /// To Get Service and Service Threads based on Service Code
        /// </summary>
        /// <param name="sSysService_Cd"></param>
        /// <returns></returns>
        public DataSet GetServiceDefinition(string sSysService_Cd)
        {
            try
            {
                string[] tableNames = new string[2];

                tableNames[0] = RG_SysService.TableName;
                tableNames[1] = RG_SysServiceThreads.TableName;

                string sProcedureName = "TSP_GetSysService";
                using (SqlConnection connection=new SqlConnection(sConnectString))
                {
                    using (SqlCommand command=new SqlCommand(sProcedureName,connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter() { Value= sSysService_Cd, ParameterName= "ipSysService_Cd" });
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();
                        try
                        {
                            connection.Open();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataSet);
                            dataSet.Tables[0].TableName = tableNames[0];
                            dataSet.Tables[1].TableName = tableNames[1];
                            return dataSet;

                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// To Get Service Configuration based on Service Id
        /// </summary>
        /// <param name="lSysService_Id"></param>
        /// <returns></returns>
        public DataTable GetServiceConfiguration(long lSysService_Id)
        {
            try
            {
                string[] tableNames = new string[1];

                tableNames[0] = RG_SysService.TableName;

                string sProcedureName = "TSP_SysServiceSel";

                using (SqlConnection connection = new SqlConnection(sConnectString))
                {
                    using (SqlCommand command = new SqlCommand(sProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter() { Value = lSysService_Id, ParameterName = "ipSysService_Id" });
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();
                        try
                        {
                            connection.Open();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataSet);

                            if (dataSet.Tables.Count > 0)
                            {
                                dataSet.Tables[0].TableName = tableNames[0];
                                return dataSet.Tables[0];
                            }
                            else
                                return new DataTable();

                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }

            }
            catch (Exception)
            {
                throw ;
            }
        }


        /// <summary>
        /// To Get Thread Configuration based on SysServiceThread_Id
        /// </summary>
        /// <param name="SysServiceThread_Id"></param>
        /// <returns></returns>
        public DataTable GetThreadConfiguration(long SysServiceThread_Id)
        {
            try
            {
                string[] tableNames = new string[1];

                tableNames[0] = RG_SysServiceThreads.TableName;

                string sProcedureName = "TSP_SysServiceThreadSel";

                using (SqlConnection connection = new SqlConnection(sConnectString))
                {
                    using (SqlCommand command = new SqlCommand(sProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter() { Value = SysServiceThread_Id, ParameterName = "ipSysServiceThread_Id" });
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        DataSet dataSet = new DataSet();
                        try
                        {
                            connection.Open();
                            adapter.SelectCommand = command;
                            adapter.Fill(dataSet);
                            if (dataSet.Tables.Count > 0)
                            {
                                dataSet.Tables[0].TableName= tableNames[0];
                                return dataSet.Tables[0];
                            }
                            else
                                return new DataTable();

                        }
                        catch (Exception)
                        {
                            throw;
                        }

                    }
                }

            }
            catch (Exception)
            {
                throw ;
            }
        }

        #endregion


        #region " Update Methods "
        public bool UpdateServiceStatus(SqlTransaction oTrans, long lSysService_Id, string sCurrentStatus_Cd)
        {
            try
            {
                SqlParameterCollection aParams;

                using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = "TSP_SysServiceStatusUpd";
                        command.Transaction=oTrans;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Connection = oTrans.Connection;

                        SqlCommandBuilder.DeriveParameters(command);
                        aParams = command.Parameters;
                        command.Parameters[1].Value = lSysService_Id;
                        command.Parameters[2].Value = sCurrentStatus_Cd;
                        command.Parameters[3].Value = DBNull.Value;
                                        

                        int outputValue = command.ExecuteNonQuery();
                        if (outputValue != 0)
                        {
                            oTrans.Commit();
                        }
                        else
                        {
                            oTrans.Rollback();

                        }

                    return outputValue != 0;
                    }

               

            }
            catch (System.Data.SqlClient.SqlException oSqlException)
            {
                throw (oSqlException);
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public bool UpdateServiceThread( DataRow drSysServiceThread)
        {
            try
            {                

                SqlParameterCollection aParams;
                using (SqlConnection oConn = new SqlConnection(sConnectString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        oConn.Open();
                        SqlTransaction oTrans = oConn.BeginTransaction();

                        command.CommandText = "TSP_SysServiceThreadUpd";
                        command.Transaction = oTrans;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Connection = oTrans.Connection;
                        SqlCommandBuilder.DeriveParameters(command);
                        aParams = command.Parameters;

                        aParams[1].Value = drSysServiceThread[RG_SysServiceThreads.CurrentStatus_Cd];
                        aParams[2].Value = drSysServiceThread[RG_SysServiceThreads.RequestedStatus_Cd];
                        aParams[3].Value = drSysServiceThread[RG_SysServiceThreads.LastStarted_DtTm];
                        aParams[4].Value = drSysServiceThread[RG_SysServiceThreads.LastStopped_DtTm];
                        aParams[5].Value = drSysServiceThread[RG_SysServiceThreads.CurrentProcessingStart_DtTm];
                        aParams[6].Value = drSysServiceThread[RG_SysServiceThreads.ReLoad_Flg];
                        aParams[7].Value = drSysServiceThread[RG_SysServiceThreads.IsSuccesful];
                        aParams[8].Value = drSysServiceThread[RG_SysServiceThreads.SysServiceThread_Id];
                        aParams[9].Value = DBNull.Value;

                        int outputValue = command.ExecuteNonQuery();

                        return outputValue != 0;
                    }
                }

            }
            catch (System.Data.SqlClient.SqlException oSqlException)
            {
                throw (oSqlException);
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public bool UpdateSysService(SqlTransaction oTrans, DataRow drSysService)
        {
            try
            {
               
                SqlParameterCollection aParams;

                using (SqlCommand command = new SqlCommand())
                {
                    DateTime LastStarted_DtTm;
                    DateTime LastStopped_DtTm;
                    command.CommandText = "TSP_SysServiceUpd";
                    command.Transaction = oTrans;
                    command.CommandType = CommandType.StoredProcedure;
                    command.Connection = oTrans.Connection;
                    SqlCommandBuilder.DeriveParameters(command);
                    aParams = command.Parameters;
                   
                    aParams[1].Value = drSysService[RG_SysService.CurrentStatus_Cd];
                    aParams[2].Value = drSysService[RG_SysService.RequestedStatus_Cd];
                    aParams[3].Value = DateTime.TryParse( drSysService[RG_SysService.LastStarted_DtTm].ToString(), out LastStarted_DtTm)? LastStarted_DtTm.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    aParams[4].Value = DateTime.TryParse(drSysService[RG_SysService.LastStopped_DtTm].ToString(), out LastStopped_DtTm)? LastStopped_DtTm.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                    aParams[5].Value = drSysService[RG_SysServiceThreads.ReLoad_Flg];
                    aParams[6].Value = drSysService[RG_SysServiceThreads.SysService_Id];
                    aParams[7].Value = DBNull.Value;

                    int outputValue = command.ExecuteNonQuery();

                    return outputValue != 0;
                }

            }
            catch (System.Data.SqlClient.SqlException oSqlException)
            {
                throw (oSqlException);
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {
            }
        }
        #endregion

        #region " Insert Methods "

        public void InsertThreadExceptionLog( long lSysServiceThread_Id, string sThreadException, DateTime dtCreate_DtTm)
        {
            try
            {

                SqlParameterCollection aParams;
                using (SqlConnection oConn = new SqlConnection(sConnectString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        oConn.Open();
                        SqlTransaction oTrans = oConn.BeginTransaction();

                        command.CommandText = "TSP_ThreadExceptionLogIns";
                        command.Transaction = oTrans;
                        command.CommandType = CommandType.StoredProcedure;
                        command.Connection = oTrans.Connection;
                        SqlCommandBuilder.DeriveParameters(command);
                        aParams = command.Parameters;
                        long lSysServiceThreadLog_Id = 0;
                        aParams[1].Value = lSysServiceThread_Id;
                        aParams[2].Value = sThreadException;
                        aParams[3].Value = dtCreate_DtTm;
                        aParams[4].Value = DBNull.Value;

                        command.ExecuteNonQuery();
                        lSysServiceThreadLog_Id = Convert.ToInt64(aParams[4].Value);
                        if (lSysServiceThreadLog_Id > 0)
                            oTrans.Commit();
                        else
                            oTrans.Rollback();
                    }
                }

            }
            catch (System.Data.SqlClient.SqlException oSqlException)
            {
                throw (oSqlException);
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion
    }
}
