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
    public class EMFFilingRpt_Service
    {
        static DBHelper dbHelper = new DBHelper();
        static string env = BaseCont.ENVIRONMENT;
        static EmailHelper emailHelper = new EmailHelper();
        static private string pOriginOffice { get; set; }

        public IEnumerable<string> GetValue()
        {
            return new List<string>() { "hi", "PO" };
        }

        public static AMS_ResponseMode GetEMFFilingData(string originOffice)
        {
            string mailList = String.Empty;
            string officeList = String.Empty;
            DataTable retDB = new DataTable(); ;
            DataTable tempDB = new DataTable(); ;
            AMS_ResponseMode responseMode = new AMS_ResponseMode();
            string[] arrMAL_Office = new string[] { "PEN", "TSB", "JOH" };
            string[] arrSPRC_Office = new string[] { "ZHG", "GZO", "SZN" };
            string[] arrAMSCenter_Office = new string[] { "BAL", "BAW", "HKK", "JKT", "JOH", "MNL", "PEN", "SIN", "SMG", "SPL", "SUR", "THI", "TJN", "TLK", "TSB", "TSU", "VNM" };

            try
            {
                DataTable dt = new DataTable();
                pOriginOffice = originOffice;
                mailList = ConfigurationManager.AppSettings[originOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];

                LogHelper.Debug("GetEMFFilingData => GET originOffice :" + originOffice);
                LogHelper.Debug("GetEMFFilingData => GET originOffice mailList :" + ConfigurationManager.AppSettings[originOffice + "_MAIL"]);
                LogHelper.Debug("GetEMFFilingData => GET mailList :" + mailList);

                switch (originOffice)
                {
                    case "SPRC":
                        officeList = String.Join("','", arrSPRC_Office.ToArray());
                        break;
                    case "AMSCenter":
                        officeList = String.Join("','", arrAMSCenter_Office.ToArray());
                        break; 
                    case "MAL":
                        officeList = String.Join("','", arrMAL_Office.ToArray());
                        break;
                    case "SHA":
                        officeList = "SHA";
                        break;
                    default:
                        officeList = originOffice;
                        break;
                }

                string startDay = DateTime.Now.AddDays(14).AddMonths(-3).AddDays(-15).ToString("yyyy-MM-dd HH:mm:ss");
                string endDay = DateTime.Today.AddDays(14).ToString("yyyy-MM-dd HH:mm:ss");

                dt = GetData(startDay, endDay, officeList);

                if (dt == null)
                {
                    SendNoDataMail();
                    LogHelper.Error("GetEMFFilingData => originOffice :" + originOffice + " DB is Null");
                    responseMode.result = "Error return Data is Null";
                    responseMode.mailTo = mailList;
                    return responseMode;
                }

                LogHelper.Debug("GetEMFFilingData => GetData Count :" + dt.Rows.Count);
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

                    string sheetName = startDay + "=>" + endDay;

                    var query = (from r in dt.AsEnumerable()
                                 where (r.Field<string>("Late1Y_30Hr").Trim() != "N")
                                 && r.Field<int>("CBPVsLstVslETD") < 24
                                 && r.Field<DateTime>("LastVslETD") <= DateTime.Now.AddDays(lastVslETD_From)
                                 select r);
                    LogHelper.Debug("GetEMFFilingData => query Count :" + query.Count());
                    if (query.Count() > 0)
                    {
                        tempDB = query.CopyToDataTable<DataRow>();
                        tempDB.DefaultView.Sort = "OriginOffice,LastVslETD,SCAC,ShptVessel,MBL ASC";
                        retDB = tempDB.DefaultView.ToTable(false, new string[] { "OriginOffice", "HBL", "MBL", "SCAC", "ShptPOL", "ShptETD", "ShptVessel", "ShptLastPort", "Submission", "Closed", "CBP_RspDte", "CBP_Status", "CBP_Vessel", "LastVslETD", "LastUsr", "Remark" });
                        retDB.Columns["OriginOffice"].ColumnName = "OFFICE";
                        retDB.Columns["ShptPOL"].ColumnName = "SHPT POL";
                        retDB.Columns["ShptETD"].ColumnName = "SHPT ETD";
                        retDB.Columns["ShptVessel"].ColumnName = "SHPT LST VESSEL"; 
                        retDB.Columns["ShptLastPort"].ColumnName = "SHPT LST POL";
                        retDB.Columns["Submission"].ColumnName = "SUBMISSION DATE";
                        retDB.Columns["CBP_RspDte"].ColumnName = "CBP RSP DATE";
                        retDB.Columns["CBP_Status"].ColumnName = "CBP STATUS";
                        retDB.Columns["CBP_Vessel"].ColumnName = "CBP VESSEL";
                        retDB.Columns["LastVslETD"].ColumnName = "LAST VSL ETD";
                        retDB.Columns["LastUsr"].ColumnName = "CREATE/LAST UPDATE USER";
                        retDB.Columns["Remark"].ColumnName = "CS/OP FOLLOW";
                    }
                    LogHelper.Debug("GetEMFFilingData => retDB Count :" + retDB.Rows.Count);
                    if (retDB.Rows.Count > 0)
                    {
                        string mailBody = CommonFun.GetHtmlString(retDB,"EMF");
                        string fileName = "EMFFilingCheckRpt_" + originOffice + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                        string title = String.Empty;

                        if (env == "DEV")
                        {
                            title = "[Test] EMF Filing Checking Alert - " + originOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                        }
                        else
                        {
                            title = "EMF Filing Checking Alert - " + originOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                        }


                        Dictionary<string, MemoryStream> keyValues = new Dictionary<string, MemoryStream>();

                        MemoryStream stream = NPOIHelper.RenderToExcel_AMS(RenderExcelUpload(tempDB), sheetName);
                        LogHelper.Debug("GetEMFFilingData => RenderToExcel_AMS Get stream");
                        if (env == "DEV")
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + originOffice + "_EXCEL_FOLDER";
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string originPath = path + "\\" + fileName;
                            NPOIHelper.ExportExcel_AMS(RenderExcelUpload(tempDB), sheetName, originPath);
                        }

                        keyValues.Add(fileName, stream);
                        LogHelper.Debug("GetEMFFilingData => SendMailViaAPI : Start");
                        emailHelper.SendMailViaAPI(title, mailBody, mailList, keyValues);
                        LogHelper.Debug("GetEMFFilingData => SendMailViaAPI : End");
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
                        LogHelper.Error("GetEMFFilingData => originOffice :" + originOffice + " Error No Data");
                        responseMode.result = "Error No Data";
                    }

                }
                else
                {
                    LogHelper.Error("GetEMFFilingData => originOffice :" + originOffice + " Error No Record return");
                    responseMode.result = "Error No Record return";
                    responseMode.mailTo = "";
                    return responseMode;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("EMF Checking Service: " + ex.Message + ",StackTrace:" + ex.StackTrace);
                responseMode.result = "Error";
            }

            return responseMode;
        }

        private static DataTable RenderExcelUpload(DataTable dt)
        {
            DataTable tempDT = new DataTable();
            tempDT = dt.DefaultView.ToTable(false
                , new string[] {
                    "OriginOffice",
                    "BookingReqID",
                    "HBL",
                    "MBL",
                    "SCAC",
                    "ShptPOL",
                    "ShptETD",
                    "ShptVessel",
                    "ShptLastPort",
                    "AMS_CutOff",
                    "AMSCutVsLstETD",
                    "Submission",
                    "Closed",
                    "CBP_RspDte",
                    "CBP_Status",
                    "CBP_Vessel",
                    "LastVslETD",
                    "SubmissionVsCBP",
                    "CBPVsLstVslETD",
                    "MissSubmission_48Hr",
                    "Late1Y_30Hr",
                    "LastUsr",
                    "EDI_COUNT",
                    "LateByTopocean",
                    "LateByCarrier",
                    "NotLate",
                    "Remark"
                });

            tempDT.Columns["BookingReqID"].ColumnName = "Booking Req ID";
            tempDT.Columns["ShptPOL"].ColumnName = "SHPT POL";
            tempDT.Columns["ShptETD"].ColumnName = "SHPT ETD";
            tempDT.Columns["ShptVessel"].ColumnName = "SHPT LST VESSEL";
            tempDT.Columns["ShptLastPort"].ColumnName = "SHPT LST POL";
            tempDT.Columns["AMS_CutOff"].ColumnName = "ACI CUT (2 DAYS B/F LST ETD)";
            tempDT.Columns["AMSCutVsLstETD"].ColumnName = "DATE DIFF OF EMF CUT & CBP RSP DATE";
            tempDT.Columns["Submission"].ColumnName = "SUBMISSION DATE";
            tempDT.Columns["CBP_RspDte"].ColumnName = "CBP RSP DATE";
            tempDT.Columns["CBP_Status"].ColumnName = "CBP STATUS";
            tempDT.Columns["CBP_Vessel"].ColumnName = "CBP VESSEL";
            tempDT.Columns["LastVslETD"].ColumnName = "LAST VSL ETD";
            tempDT.Columns["SubmissionVsCBP"].ColumnName = "SUBMISSION VS CBP STATUS (HRS)";
            tempDT.Columns["CBPVsLstVslETD"].ColumnName = "CBP STATUS VS LAST VSL ETD (HRS)";
            tempDT.Columns["MissSubmission_48Hr"].ColumnName = "NO SUBMISSION FROM 48 HRS PRIOR TO LAST VSL ETD";
            tempDT.Columns["Late1Y_30Hr"].ColumnName = "LATE Matched FROM 30HRS PRIOR TO LAST VSL ETD";
            tempDT.Columns["LastUsr"].ColumnName = "CREATE/LAST UPDATE USER";
            tempDT.Columns["EDI_COUNT"].ColumnName = "EDI COUNT";
            tempDT.Columns["LateByTopocean"].ColumnName = "LATE Matched by Topocean(Y)";
            tempDT.Columns["LateByCarrier"].ColumnName = "LATE Matched by Carrier(Y)";
            tempDT.Columns["NotLate"].ColumnName = "Not Late eManifest Result";
            return tempDT;
        }

        private static void SendNoDataMail()
        {
            string title = String.Empty;

            if (env == "DEV")
            {
                title = "[Test] EMF Filing Checking Alert - " + pOriginOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
            }
            else
            {
                title = "EMF Filing Checking Alert - " + pOriginOffice + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
            }

            string mailList = String.Empty;
            mailList = ConfigurationManager.AppSettings[pOriginOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];
            emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString("No Data"), mailList);
        }
        public static DataTable GetData(string pDateFrom, string pDateTo, string pOriginOffice)
        {
            DataTable dt = new DataTable();
            string selSQL = String.Format(" DECLARE @WBICutOff DATETIME;  SET @WBICutOff = '2020-11-01'; " +
                                          "                                                                                                                                           " +
                                          "	select                                                                                                                                  " +
                                          "		p.uID,                                                                                                                              " +
                                          "		p.OriginOffice,                                                                                                                     " +
                                          "		p.BookingReqID,                                                                                                                     " +
                                          "		RIGHT(HBL, 12) as HBL,                                                                                                              " +
                                          "		MBL,                                                                                                                                " +
                                          "		MBL as OrigMBL,                                                                                                                     " +
                                          "		(case when LEFT(MBL, 4) = 'MEDU' AND SCAC = 'MSCU' THEN 'MEDU' ELSE SCAC END) as SCAC,                                              " +
                                          "		(select top 1 '[' +(ISOCountryCode+ISOPortCode)+'] ' + PortName from Port WITH (NOLOCK) where PortAbbr = p.LoadPort) as ShptPOL,    " +
                                          "		P2PETD as ShptETD,                                                                                                                  " +
                                          "		ShptVessel,                                                                                                                         " +
                                          "		ShptLastPort,                                                                                                                       " +
                                          "		ShptLastPortETD                                                                                                                     " +
                                          "                                                                                                                                           " +
                                          "		AMS_CutOff,                                                                                                                         " +
                                          "		DATEDIFF(DAY, AMS_CutOff, CBP_RspDte) AS AMSCutVsLstETD,                                                                            " +
                                          "		Submission,                                                                                                                         " +
                                          "		CBP_RspDte,                                                                                                                         " +
                                          "		Closed,                                                                                                                             " +
                                          "		(case when CBP_RspDte is null then '--' else 'Matched' end) as CBP_Status,                                                          " +
                                          "		ShptVessel as CBP_Vessel,                                                                                                           " +
                                          "		ShptLastPortETD AS LastVslETD,                                                                                                      " +
                                          "                                                                                                                                           " +
                                          "		ISNULL(DATEDIFF(HOUR, Submission, CBP_RspDte), 0) AS SubmissionVsCBP,                                                               " +
                                          "		ISNULL(DATEDIFF(HOUR, CBP_RspDte, ShptLastPortETD), 0) AS CBPVsLstVslETD,                                                           " +
                                          "		(CASE WHEN Submission IS NULL AND DATEDIFF(HOUR, GETDATE(), ShptLastPortETD) < 48 THEN 'Y' ELSE 'N' END) AS MissSubmission_48Hr,    " +
                                          "		(CASE WHEN ISNULL(DATEDIFF(HOUR, CBP_RspDte, ShptLastPortETD), 0) = 0 AND CBP_RspDte IS NULL THEN '--'                              " +
                                          "		WHEN ISNULL(DATEDIFF(HOUR, CBP_RspDte, ShptLastPortETD), 0) < 36 THEN 'Y' ELSE 'N' END) AS Late1Y_30Hr,                             " +
                                          "                                                                                                                                           " +
                                          "		AMSForm_ModifiedBy as LastUsr,                                                                                                      " +
                                          "		EDI_COUNT,                                                                                                                          " +
                                          "		                                                                                                                                    " +
                                          "		'' LateByTopocean,                                                                                                                  " +
                                          "		'' LateByCarrier,                                                                                                                   " +
                                          "		ISNULL((SELECT TOP 1 ex.NotLateAMSResult FROM LateeManifest_FromExcel ex WITH (NOLOCK)                                              " +
                                          "		WHERE ex.BookingReqID = p.BookingReqID AND ex.IsActive = 1 ORDER BY uID DESC), '') AS NotLate,                                      " +
                                          "		ISNULL((SELECT TOP 1 ex.LateAMSRemark FROM LateeManifest_FromExcel ex WITH (NOLOCK)                                                 " +
                                          "		WHERE ex.BookingReqID = p.BookingReqID AND ex.IsActive = 1 ORDER BY uID DESC), '') AS Remark                                        " +
                                          "		                                                                                                                                    " +
                                          "	from POTracing p WITH (NOLOCK) inner join (                                                                                             " +
                                          "		select                                                                                                                              " +
                                          "			                                                                                                                                " +
                                          "			p.uID,                                                                                                                          " +
                                          "			dbo.fun_GetAMSVessel(p.BookingReqID, 2) as SCAC,                                                                                " +
                                          "			dbo.fun_GetAMSVessel(p.BookingReqID, 1) as ShptVessel,                                                                          " +
                                          "			dbo.fun_GetAMSVessel(p.BookingReqID, 3) as ShptLastPort,                                                                        " +
                                          "			dbo.fun_GetAMSVesselDate(p.BookingReqID, 1) as ShptLastPortETD,                                                                 " +
                                          "			(CASE WHEN dbo.fun_GetAMSVesselDate(p.BookingReqID, 1) is null THEN NULL                                                        " +
                                          "			ELSE DATEADD(DAY, -2, dbo.fun_GetAMSVesselDate(p.BookingReqID, 1)) END) AS AMS_CutOff,                                          " +
                                          "			dbo.fun_GetCBP_First_Date(p.BookingReqID, HBL, 'eManifest', 4) AS Submission,                                                   " +
                                          "			dbo.fun_GetCBP_First_Date(p.BookingReqID, HBL, 'eManifest', 5) AS Closed,                                                       " +
                                          "			dbo.fun_GetCBP_First_Date(p.BookingReqID, HBL, 'eManifest', 2) AS CBP_RspDte,                                                   " +
                                          "			'' as CBP_Vessel,                                                                                                               " +
                                          "			ResponseCount as EDI_COUNT,                                                                                                     " +
                                          "			ISNULL(a.ModifiedBy,p.CreatedBy) as AMSForm_ModifiedBy                                                                          " +
                                          "		from POTracing p WITH (NOLOCK) left join AMSForm a WITH (NOLOCK) on p.BookingReqID = a.BookingReqID and a.ManifestType = 'eManifest'" +
                                          "		LEFT JOIN LateeManifest_FromExcel l WITH (NOLOCK) on p.BookingReqID = l.BookingReqID                                                " +
                                          "		and l.IsActive = 1 AND (l.LateByTopocean = 'Y' OR l.LateByCarrier = 'Y' OR l.NotLateAMSResult = 'Y')                                " +
                                          "		where p.BookingReqID is not null                                                                                                    " +
                                          "		and (p.IsTBS = 'Y' or (p.BookingContractType in ('Topocean', 'NVO/Non-Topocean Contract') and p.P2PETD >= @WBICutOff))              " +
                                          "		and p.TransportationMode = 'SEA'                                                                                                    " +
                                          "		AND ISNULL(WB_Status, '') <> 'CANCELLED'                                                                                            " +
                                          "		AND IsTopEManifest = 'Y'                                                                                                            " +
                                          "		and  ISNULL(HBL, '') <> ''                                                                                                          " +
                                          "		and ((P2PETD between  {0} and {1}) or (dbo.fun_GetAMSVesselDate(p.BookingReqID, 1) between  {0} and {1}))         " +
                                          "		and l.uID is null                                                                                                                   " +
                                          "		AND OriginOffice IN ({2})                                                         " +
                                          "	) AS Table1 ON p.uID = Table1.uID                                                                                                       " +
                                          "	ORDER BY OriginOffice,ShptLastPortETD                                                                                                                ",
                                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(pDateFrom)),
                                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(pDateTo)),
                                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(pOriginOffice))
                                          );
            dt = dbHelper.ExecDataTable(selSQL);
            LogHelper.Debug("GetEMFFilingData => SQL :" + selSQL);
            return dt;
        }
    }
}