using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

namespace TB_WEB.CommonLibrary.CommonFun
{
    public static class CommonFun
    {
        static DBHelper dbHelper = new DBHelper();

        public static DataTable GetAMSFilingData()
        {

            DataTable retDB = new DataTable();
            try
            {
                DataTable dt = new DataTable();
                string sql = "usp_RptAMSFilingByOriginOffice";
                List<OleDbParameter> ps = new List<OleDbParameter>();

                ps.Add(new OleDbParameter() { ParameterName = "@DateFrom", OleDbType = DBHelper.DBTypeMapping("DATETIME"), Value = DateTime.Now.AddDays(14).AddMonths(-3) });
                ps.Add(new OleDbParameter() { ParameterName = "@DateTo", OleDbType = DBHelper.DBTypeMapping("DATETIME"), Value = DateTime.Today.AddDays(14) });
                ps.Add(new OleDbParameter() { ParameterName = "@OriginOffice", OleDbType = DBHelper.DBTypeMapping("VARCHAR"), Size = 20, Value = "SHA" });
                dt = dbHelper.ExecDataTable(CommandType.StoredProcedure, sql, ps.ToArray());

                if (dt.Rows.Count > 0)
                {
                    var query = (from r in dt.AsEnumerable()
                                 where (r.Field<string>("Late1Y_30Hr").Trim() != "N")
                                 && r.Field<int>("CBPVsLstVslETD") < 24
                                 && r.Field<DateTime>("LastVslETD") <= DateTime.Now.AddDays(4)
                                 //TODO Debug
                                 // && r.Field<DateTime>("LastVslETD") > DateTime.Now.AddDays(-1)
                                 && r.Field<DateTime>("LastVslETD") > DateTime.Now.AddDays(-4)
                                 select r);
                    if (query.Count() > 0)
                    {
                        DataTable tempDB = query.CopyToDataTable<DataRow>();

                        retDB = tempDB.DefaultView.ToTable(false, new string[] { "HBL", "MBL", "SCAC", "ShptPOL","ShptVessel", "ShptLastPort", "Submission", "LastVslETD", "LastUsr", "Remark" });
                        retDB.Columns["ShptPOL"].ColumnName = "RECEIPT PORT";
                        retDB.Columns["ShptVessel"].ColumnName = "AMS VESSEL";
                        retDB.Columns["ShptLastPort"].ColumnName = "AMS LAST PORT";
                        retDB.Columns["Submission"].ColumnName = "SUBMISSION DATE";
                        retDB.Columns["LastVslETD"].ColumnName = "AMS VSL ETD";
                        retDB.Columns["LastUsr"].ColumnName = "CREATE/LAST UPDATE USER";
                        retDB.Columns["Remark"].ColumnName = "CS/OP FOLLOW";

                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }

            return retDB;
        }
    }
}
