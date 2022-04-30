using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Helpers
{
    public class DBHelperBase
    {

        public static string ReplaceVbcrlf2BR(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            //str = replace(str, "'", "''")
            str = str.Replace(System.Environment.NewLine, "<br>");

            return str;
        }

        public static string ToDBStr(string str)
        {

            str = str.Replace("'", "''");
            return str;
        }

        public static string ToDBStr_ReplaceVbcrlf2BR(string str)
        {
            str = ReplaceVbcrlf2BR(str);
            str = str.Replace("'", "''");
            return str;
        }

        public static string GetLoopParams(OleDbParameter[] inParams)
        {
            string retVal = string.Empty;
            try
            {
                if (inParams != null)
                {
                    foreach (OleDbParameter paramItem in inParams)
                    {
                        retVal += string.Format("DbType={0}; OleDbType={1}; ParameterName={2}; Value={3} \n", paramItem.DbType, paramItem.OleDbType, paramItem.ParameterName, paramItem.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "";
            }

            return retVal;
        }

        public static string GetLoopParams(SqlParameter[] inParams)
        {
            string retVal = string.Empty;
            try
            {
                if (inParams != null)
                {
                    foreach (SqlParameter paramItem in inParams)
                    {
                        retVal += string.Format("DbType={0}; OleDbType={1}; ParameterName={2}; Value={3} \n", paramItem.DbType, paramItem.DbType, paramItem.ParameterName, paramItem.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                retVal = "";
            }

            return retVal;
        }

        public static T GetDataColumnValue<T>(object dataObj, T defaultVal)
        {

            try
            {
                object retVal;

                if (dataObj == null || dataObj == DBNull.Value)
                {
                    return defaultVal;

                }
                else
                {
                    return (T)Convert.ChangeType(dataObj, typeof(T));
                }

            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static T GetDataColumnValue<T>(DataRow dr, string fieldName, T defaultVal)
        {
            try
            {
                object retVal;

                if (!dr.Table.Columns.Contains(fieldName) || dr[fieldName] == DBNull.Value)
                {
                    return defaultVal;

                }
                else
                {
                    return (T)Convert.ChangeType(dr[fieldName], typeof(T));
                }

            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

    }
}
