using System.Diagnostics;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Collections;

namespace Utility
{
	namespace DataSetManagement
	{
		
		public class DataSetComponent
		{
			public static object ReplaceDBNull(DataRow drRecValue, string sColumnName, object vReplace)
			{
				if (drRecValue.IsNull(sColumnName))
				{
					return vReplace;
				}
				else
				{
					return drRecValue[sColumnName];
				}
			}
			
            //public static object ReplaceDBNull(object vActual, object vReplace)
            //{
            //    try
            //    {
            //        if ((vActual == null) ||(vActual == System.DBNull.Value))
            //        {
            //            return vReplace;
            //        }
            //        else
            //        {
            //            return vActual;
            //        }
            //    }
            //    catch (Exception oException)
            //    {
					
            //        throw (oException);
            //    }
            //    finally
            //    {
					
            //    }
            //}

            public static string ReplaceDBNull(object InputValue, string ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (string)InputValue;
                }
            }

            public static byte ReplaceDBNull(object InputValue, byte ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (byte)InputValue;
                }
            }

            public static short ReplaceDBNull(object InputValue, short ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (short)InputValue;
                }
            }

            public static int ReplaceDBNull(object InputValue, int ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (int)InputValue;
                }
            }

            public static long ReplaceDBNull(object InputValue, long ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (long)InputValue;
                }
            }

            public static decimal ReplaceDBNull(object InputValue, decimal ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (decimal)InputValue;
                }
            }

            public static double ReplaceDBNull(object InputValue, double ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (double)InputValue;
                }
            }

            public static bool ReplaceDBNull(object InputValue, bool ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (bool)InputValue;
                }
            }

            public static DateTime ReplaceDBNull(object InputValue, DateTime ReplacementValue)
            {
                if ((InputValue == null) || (InputValue == System.DBNull.Value))
                {
                    return ReplacementValue;
                }
                else
                {
                    return (DateTime)InputValue;
                }
            }

            /// <summary>
            /// To coerce a value to DBNull if it contains only spaces or empty string
            /// </summary>
            /// <param name="OriginalValue">Control value or cell value of a datarow</param>
            /// <returns></returns>
            public static object ForceDBNull(object OriginalValue)
            {
                //================================================================================
                // CopyRight         : Advent Software Systems 
                // Function Name     : ForeceDBNull
                // Created By        : Oleg Goldenberg
                // Modified By       :
                // Scope             : To coerce a value to DBNull if it contains only spaces or empty string
                // Schema Name       : None
                // Business Rules    : None
                // Procedure Calls   : None
                // Description       : To coerce a value to DBNull if it contains only spaces or empty string
                //================================================================================

                if (!(OriginalValue == DBNull.Value))
                {
                    if (OriginalValue == null)
                        return DBNull.Value;
                    else if (OriginalValue.ToString().Trim() == string.Empty)
                        return DBNull.Value;
                }

                // Otherwise return original value
                return OriginalValue;

            }

            public static bool CheckRecordExist(DataSet dsInput, string TableName)
            {
                if (dsInput != null && dsInput.Tables.Contains(TableName) == true)
                {
                    if (dsInput.Tables[TableName] != null && dsInput.Tables[TableName].Rows.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            public static bool CheckRecordExist(DataTable dtInput)
            {
                if (dtInput != null && dtInput.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static bool CheckUniqueColumn(DataTable dtInput, string ColumnName)
            {
                if (dtInput != null && dtInput.Rows.Count > 0 && dtInput.Columns.Contains(ColumnName) == true)
                {
                    string sColumnValue;

                    foreach (DataRow dr in dtInput.Rows)
                    {
                        sColumnValue = dr[ColumnName].ToString();
                        if (dr[ColumnName].GetType() == typeof(string))
                        {
                            sColumnValue = "'" + sColumnValue.Replace("'","''") + "'";
                        }
                        
                        DataRow[] drRows = dtInput.Select(ColumnName + " = " + sColumnValue);
                        if (drRows.Length > 1)
                            return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static DataTable GetDistinctRows(DataTable dtSource)
            {
                DataTable dtDistinct = null;

                if (CheckRecordExist(dtSource))
                {
                    DataView dvSource = new DataView(dtSource);

                    string sColNamesStr = string.Empty;

                    foreach (DataColumn dcCol in dtSource.Columns)
                    {
                        if (sColNamesStr != string.Empty)
                            sColNamesStr += ",";

                        sColNamesStr += dcCol.ColumnName;
                    }

                    string[] sColumns = sColNamesStr.Split(',');

                    dtDistinct = dvSource.ToTable(true, sColumns);
                }

                return dtDistinct;
            }

            public static DataTable GetDistinctRows(DataTable dtSource, string sColNames)
            {
                DataTable dtDistinct = null;

                if (CheckRecordExist(dtSource))
                {
                    DataView dvSource = new DataView(dtSource);

                    string[] sColumns = sColNames.Split(',');

                    dtDistinct = dvSource.ToTable(true, sColumns);
                }

                return dtDistinct;
            }
		}
		
	}
	
}
