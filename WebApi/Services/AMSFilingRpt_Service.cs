using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Date;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.EdiModels.Common;
using WebApi.Models;

namespace WebApi.Services
{
    public class AMSFilingRpt_Service : IAMSFilingRpt
    {
        static DBHelper dbHelper = new DBHelper();
        static string env = BaseCont.ENVIRONMENT;

        public IEnumerable<string> GetValue()
        {
            return new List<string>() { "hi", "PO" };
        }

        public static AMS_ResponseMode GetAMSFilingData(string originOffice)
        {
            EmailHelper emailHelper = new EmailHelper();
            DataTable retDB = new DataTable();
            MemoryStream stream = new MemoryStream();
            AMS_ResponseMode responseMode = new AMS_ResponseMode();

            try
            {
                DataTable dt = new DataTable();
                string sql = "usp_RptAMSFilingByOriginOffice";
                List<OleDbParameter> ps = new List<OleDbParameter>();

                ps.Add(new OleDbParameter() { ParameterName = "@DateFrom", OleDbType = DBHelper.DBTypeMapping("DATETIME"), Value = DateTime.Now.AddDays(14).AddMonths(-3) });
                ps.Add(new OleDbParameter() { ParameterName = "@DateTo", OleDbType = DBHelper.DBTypeMapping("DATETIME"), Value = DateTime.Today.AddDays(14) });
                ps.Add(new OleDbParameter() { ParameterName = "@OriginOffice", OleDbType = DBHelper.DBTypeMapping("VARCHAR"), Size = 20, Value = originOffice });
                dt = dbHelper.ExecDataTable(CommandType.StoredProcedure, sql, ps.ToArray());
                if (dt == null)
                {
                    LogHelper.Error("GetAMSFilingData => No Record return");
                    responseMode.result = "Error";
                    return responseMode;
                }
                if (dt.Rows.Count > 0)
                {
                    int week = (int)DateTime.Now.DayOfWeek;
                    int lastVslETD_From = 0;
                    int lastVslETD_To = 0;

                    if (week >= 1 && week <= 4)
                    {
                        // Mon - Th
                        lastVslETD_From = 4;
                        lastVslETD_To = -1;
                    }
                    else if (week == 5)
                    {
                        // Fr
                        lastVslETD_From = 6;
                        lastVslETD_To = -1;
                    }

                    // TODO When DEV change ETD 
                    if (env == "DEV")
                    {
                        lastVslETD_To = -15;
                    }

                    string sheetName = DateTime.Now.AddDays(14).AddMonths(-3).ToString("yyyy-MM-dd") + "=>" + DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
                    stream = NPOIHelper.RenderToExcel(dt, sheetName);
                    var query = (from r in dt.AsEnumerable()
                                 where (r.Field<string>("Late1Y_30Hr").Trim() != "N")
                                 && r.Field<int>("CBPVsLstVslETD") < 24
                                 && r.Field<DateTime>("LastVslETD") <= DateTime.Now.AddDays(lastVslETD_From)
                                 && r.Field<DateTime>("LastVslETD") > DateTime.Now.AddDays(lastVslETD_To)
                                 select r);
                    if (query.Count() > 0)
                    {
                        DataTable tempDB = query.CopyToDataTable<DataRow>();
                        tempDB.DefaultView.Sort = "LastVslETD DESC";
                        retDB = tempDB.DefaultView.ToTable(false, new string[] { "HBL", "MBL", "SCAC", "ShptPOL", "ShptVessel", "ShptLastPort", "Submission", "LastVslETD", "LastUsr", "Remark" });
                        retDB.Columns["ShptPOL"].ColumnName = "RECEIPT PORT";
                        retDB.Columns["ShptVessel"].ColumnName = "AMS VESSEL";
                        retDB.Columns["ShptLastPort"].ColumnName = "AMS LAST PORT";
                        retDB.Columns["Submission"].ColumnName = "SUBMISSION DATE";
                        retDB.Columns["LastVslETD"].ColumnName = "AMS VSL ETD";
                        retDB.Columns["LastUsr"].ColumnName = "CREATE/LAST UPDATE USER";
                        retDB.Columns["Remark"].ColumnName = "CS/OP FOLLOW";
                    }
                    
                    string mailBody = CommonFun.GetHtmlString(retDB);
                    string fileName = "AMSFilingCheckRpt_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                    string title = String.Empty;

                    if (env == "DEV")
                    {
                        title = "[Test] AMS Filing Checking Alert - " + originOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                    }
                    else
                    {
                        title = "AMS Filing Checking Alert - " + originOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                    }


                    Dictionary<string, MemoryStream> keyValues = new Dictionary<string, MemoryStream>();
                    keyValues.Add(fileName, stream);
                    string mailList = ConfigurationManager.AppSettings[originOffice + "_MAIL"];
                    
                    emailHelper.SendMailViaAPI(title, mailBody, mailList, keyValues);

                    responseMode.table = retDB;
                    responseMode.temptable = dt;
                    responseMode.mailTo = mailList;
                    responseMode.result = "Success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                responseMode.result = "Error";
            }

            return responseMode;
        }
    }
}