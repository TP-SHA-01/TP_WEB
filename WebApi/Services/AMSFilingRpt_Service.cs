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
        static EmailHelper emailHelper = new EmailHelper();
        static private string pOriginOffice { get; set; }

        public IEnumerable<string> GetValue()
        {
            return new List<string>() { "hi", "PO" };
        }

        public static AMS_ResponseMode GetAMSFilingData(string originOffice)
        {
            string mailList = String.Empty;
            string officeList = String.Empty;
            DataTable retDB = new DataTable();
            MemoryStream stream = new MemoryStream();
            AMS_ResponseMode responseMode = new AMS_ResponseMode();
            string[] arrSPRC_Office = new string[] { "ZHG", "GZO", "SZN" };
            string[] arrAMSCenter_Office = new string[] { "BAL", "BAW", "HKK", "JKT", "JOH", "MNL", "PEN", "SIN", "SMG", "SPL", "SUR", "THI", "TJN", "TLK", "TSB", "TSU", "VNM" };

            try
            {
                DataTable dt = new DataTable();
                pOriginOffice = originOffice;
                mailList = ConfigurationManager.AppSettings[originOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];
                switch (originOffice)
                {
                    case "SPRC":
                        officeList = String.Join("','", arrSPRC_Office.ToArray());
                        break;
                    case "AMSCenter":
                        officeList = String.Join("','", arrAMSCenter_Office.ToArray());
                        break;
                    case "SHA":
                        officeList = "SHA";
                        break;
                }

                dt = GetData(DateTime.Now.AddDays(14).AddMonths(-3).ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Today.AddDays(14).ToString("yyyy-MM-dd HH:mm:ss"), officeList);
                if (dt == null)
                {
                    SendNoDataMail();
                    LogHelper.Error("GetAMSFilingData => originOffice :" + originOffice + " DB is Null");
                    responseMode.result = "Error return Data is Null";
                    responseMode.mailTo = mailList;
                    return responseMode;
                }

                LogHelper.Debug("GetAMSFilingData => GetData Count :" + dt.Rows.Count);
                if (dt.Rows.Count > 0)
                {
                    int week = (int)DateTime.Now.DayOfWeek;
                    int lastVslETD_From = 0;
                    int lastVslETD_To = 0;

                    if (week >= 1 && week <= 4)
                    {
                        // Mon - Th {1,2,3,4}
                        lastVslETD_From = 3;
                        lastVslETD_To = -1;
                    }
                    else if (week == 0 || week == 6)
                    {
                        // Sun:{0} Sat:{6}
                        lastVslETD_From = 4;
                        lastVslETD_To = -1;
                    }
                    else if (week == 5)
                    {
                        // Fr {5}
                        lastVslETD_From = 5;
                        lastVslETD_To = -1;
                    }

                    // When DEV change ETD 
                    if (env == "DEV")
                    {
                        lastVslETD_To = -30;
                    }

                    string sheetName = DateTime.Now.AddDays(14).AddMonths(-3).ToString("yyyy-MM-dd") + "=>" + DateTime.Today.AddDays(14).ToString("yyyy-MM-dd");
                    
                    var query = (from r in dt.AsEnumerable()
                                 where (r.Field<string>("Late1Y_30Hr").Trim() != "N")
                                 && r.Field<int>("CBPVsLstVslETD") < 24
                                 && r.Field<DateTime>("LastVslETD") <= DateTime.Now.AddDays(lastVslETD_From)
                                 select r);
                    LogHelper.Debug("GetAMSFilingData => query Count :" + query.Count());
                    if (query.Count() > 0)
                    {
                        DataTable tempDB = query.CopyToDataTable<DataRow>();
                        tempDB.DefaultView.Sort = "OriginOffice,LastVslETD,SCAC,ShptVessel,MBL ASC";
                        retDB = tempDB.DefaultView.ToTable(false, new string[] { "OriginOffice", "HBL", "MBL", "SCAC", "ShptPOL", "ShptVessel", "ShptLastPort", "Submission", "LastVslETD", "LastUsr", "Remark" });
                        retDB.Columns["OriginOffice"].ColumnName = "OFFICE";
                        retDB.Columns["ShptPOL"].ColumnName = "RECEIPT PORT";
                        retDB.Columns["ShptVessel"].ColumnName = "AMS VESSEL";
                        retDB.Columns["ShptLastPort"].ColumnName = "AMS LAST PORT";
                        retDB.Columns["Submission"].ColumnName = "SUBMISSION DATE";
                        retDB.Columns["LastVslETD"].ColumnName = "AMS VSL ETD";
                        retDB.Columns["LastUsr"].ColumnName = "CREATE/LAST UPDATE USER";
                        retDB.Columns["Remark"].ColumnName = "CS/OP FOLLOW";
                    }
                    LogHelper.Debug("GetAMSFilingData => retDB Count :" + retDB.Rows.Count);
                    if (retDB.Rows.Count > 0)
                    {
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

                        stream = NPOIHelper.RenderToExcel(retDB, sheetName);

                        keyValues.Add(fileName, stream);
                        
                        emailHelper.SendMailViaAPI(title, mailBody, mailList, keyValues);

                        responseMode.table = retDB;
                        responseMode.temptable = dt;
                        responseMode.mailTo = mailList;
                        responseMode.result = "Success";
                    }
                    else
                    {
                        SendNoDataMail();
                        responseMode.table = retDB;
                        responseMode.temptable = dt;
                        responseMode.mailTo = mailList;
                        LogHelper.Error("GetAMSFilingData => originOffice :" + originOffice + " Error No Data");
                        responseMode.result = "Error No Data";
                    }

                }
                else
                {
                    LogHelper.Error("GetAMSFilingData => originOffice :" + originOffice + " Error No Record return");
                    responseMode.result = "Error No Record return";
                    responseMode.mailTo = "";
                    return responseMode;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                responseMode.result = "Error";
            }

            return responseMode;
        }


        private static void SendNoDataMail()
        {
            string title = String.Empty;

            if (env == "DEV")
            {
                title = "[Test] AMS Filing Checking Alert - " + pOriginOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
            }
            else
            {
                title = "AMS Filing Checking Alert - " + pOriginOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
            }

            string mailList = String.Empty;
            mailList = ConfigurationManager.AppSettings[pOriginOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];
            emailHelper.SendMailViaAPI(title, "No Data", mailList);
        }
        public static DataTable GetData(string pDateFrom, string pDateTo, string pOriginOffice)
        {
            DataTable dt = new DataTable();
            string selSQL = String.Format(" DECLARE @WBICutOff DATETIME;  SET @WBICutOff = '2020-11-01'; " +

                                          "  select                                                                                                                                       " +
                                          " 		p.uID,                                                                                                                               " +
                                          " 		p.OriginOffice,                                                                                                                      " +
                                          " 		p.BookingReqID,                                                                                                                      " +
                                          " 		RIGHT(HBL, 12) as HBL,                                                                                                               " +
                                          " 		MBL,                                                                                                                                 " +
                                          " 		MBL as OrigMBL,                                                                                                                      " +
                                          " 		(case when LEFT(MBL, 4) = 'MEDU' AND SCAC = 'MSCU' THEN 'MEDU' ELSE SCAC END) as SCAC,                                               " +
                                          " 		(select top 1 '[' +(ISOCountryCode+ISOPortCode)+'] ' + PortName from Port WITH (NOLOCK) where PortAbbr = p.LoadPort) as ShptPOL,     " +
                                          " 		P2PETD as ShptETD,                                                                                                                   " +
                                          " 		ShptVessel,                                                                                                                          " +
                                          " 		ShptLastPort,                                                                                                                        " +
                                          " 		ShptLastPortETD                                                                                                                      " +
                                          "                                                                                                                                              " +
                                          " 		AMS_CutOff,                                                                                                                          " +
                                          " 		DATEDIFF(DAY, AMS_CutOff, CBP_RspDte) AS AMSCutVsLstETD,                                                                             " +
                                          " 		Submission,                                                                                                                          " +
                                          " 		CBP_RspDte,                                                                                                                          " +
                                          " 		CBP_Status,                                                                                                                          " +
                                          " 		CBP_Vessel,                                                                                                                          " +
                                          " 		ShptLastPortETD AS LastVslETD,                                                                                                       " +
                                          "                                                                                                                                              " +
                                          " 		ISNULL(DATEDIFF(HOUR, Submission, CBP_RspDte), 0) AS SubmissionVsCBP,                                                                " +
                                          " 		ISNULL(DATEDIFF(HOUR, CBP_RspDte, ShptLastPortETD), 0) AS CBPVsLstVslETD,                                                            " +
                                          " 		(CASE WHEN Submission IS NULL AND DATEDIFF(HOUR, GETDATE(), ShptLastPortETD) < 48 THEN 'Y' ELSE 'N' END) AS MissSubmission_48Hr,     " +
                                          " 		(CASE WHEN ISNULL(DATEDIFF(HOUR, CBP_RspDte, ShptLastPortETD), 0) = 0 AND CBP_RspDte IS NULL THEN '--' WHEN ISNULL(DATEDIFF(HOUR, CBP_RspDte, ShptLastPortETD), 0) < 36 THEN 'Y' ELSE 'N' END) AS Late1Y_30Hr, " +
                                          " 		AMSForm_ModifiedBy as LastUsr,      " +
                                          " 		EDI_COUNT,                          " +
                                          " 		                                    " +
                                          " 		'' LateByTopocean,                  " +
                                          " 		'' LateByCarrier,                   " +
                                          " 		ISNULL((SELECT TOP 1 ex.NotLateAMSResult FROM LateAMS_FromExcel ex WITH (NOLOCK) WHERE ex.BookingReqID = p.BookingReqID AND ex.IsActive = 1 ORDER BY uID DESC), '') AS NotLate, " +
                                          " 		ISNULL((SELECT TOP 1 ex.LateAMSRemark FROM LateAMS_FromExcel ex WITH (NOLOCK) WHERE ex.BookingReqID = p.BookingReqID AND ex.IsActive = 1 ORDER BY uID DESC), '') AS Remark      " +
                                          " 	from POTracing p WITH(NOLOCK) inner join (                                                  " +
                                          " 		select                                                                                  " +
                                          " 			                                                                                    " +
                                          " 			p.uID,                                                                              " +
                                          " 			dbo.fun_GetAMSVessel(p.BookingReqID, 2) as SCAC,                                    " +
                                          " 			dbo.fun_GetAMSVessel(p.BookingReqID, 1) as ShptVessel,                              " +
                                          " 			dbo.fun_GetAMSVessel(p.BookingReqID, 3) as ShptLastPort,                            " +
                                          " 			dbo.fun_GetAMSVesselDate(p.BookingReqID, 1) as ShptLastPortETD,                     " +
                                          " 			(CASE WHEN dbo.fun_GetAMSVesselDate(p.BookingReqID, 1) is null THEN NULL ELSE DATEADD(DAY, -2, dbo.fun_GetAMSVesselDate(p.BookingReqID, 1)) END) AS AMS_CutOff,  " +
                                          " 			dbo.fun_GetCBP_First_Date(p.BookingReqID, HBL, 'AMS', 4) AS Submission,             " +
                                          " 			dbo.fun_GetCBP_First(p.BookingReqID, HBL, 'AMS', 1) AS CBP_Status,                  " +
                                          " 			dbo.fun_GetCBP_First_Date(p.BookingReqID, HBL, 'AMS', 2) AS CBP_RspDte,             " +
                                          " 			dbo.fun_GetCBP_First(p.BookingReqID, HBL, 'AMS', 3)as CBP_Vessel,                   " +
                                          " 			ResponseCount as EDI_COUNT,                                                         " +
                                          " 			ISNULL(a.ModifiedBy,p.CreatedBy) as AMSForm_ModifiedBy                              " +
                                          " 		from POTracing p WITH (NOLOCK) left join AMSForm a WITH (NOLOCK) on p.BookingReqID = a.BookingReqID and ISNULL(a.ManifestType, '') in ('', 'AMS') " +
                                          " 		where ISNULL(p.BookingReqID, '') <> '' " +
                                          " 		and (ISNULL(p.IsTBS, '') = 'Y' or (p.BookingContractType in ('Topocean', 'NVO/Non-Topocean Contract') and p.P2PETD >= @WBICutOff)) " +
                                          " 		and ISNULL(p.TransportationMode, '') = 'SEA' " +
                                          " 		AND ISNULL(WB_Status, '') <> 'CANCELLED'      " +
                                          " 		AND ISNULL(IsTopAMS, '') = 'Y'                " +
                                          " 		and  ISNULL(HBL, '') <> ''                    " +
                                          " 		and ((P2PETD between {0} and {1}) or (dbo.fun_GetAMSVesselDate(p.BookingReqID, 1) between {0} and {1})) " +
                                          " 		AND (SELECT COUNT(*) FROM LateAMS_FromExcel ex WITH (NOLOCK) WHERE ex.BookingReqID = p.BookingReqID AND ex.IsActive = 1 AND (ex.LateByTopocean = 'Y' OR ex.LateByCarrier = 'Y' OR ex.NotLateAMSResult = 'Y')) = 0 " +
                                          " 		and OriginOffice IN ({2}) " +
                                          " 	) AS Table1 ON p.uID = Table1.uID  " +
                                          " 	ORDER BY OriginOffice,ShptLastPortETD ",
                                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(pDateFrom)),
                                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(pDateTo)),
                                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(pOriginOffice))
                                          );
            dt = dbHelper.ExecDataTable(selSQL);
            LogHelper.Debug("GetAMSFilingData => SQL :" + selSQL);
            return dt;
        }
    }
}