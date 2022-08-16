using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace MissingDataReport
{
    class Program
    {
        static private string pRptValue { get; set; }
        static private string pOfficeList_OCEAN { get; set; }
        static private string pOfficeList_AIR { get; set; }
        static private string pOfficeList_TRAFFIC { get; set; }
        static DBHelper dbHelper = new DBHelper();
        static string env = BaseCont.ENVIRONMENT;
        static EmailHelper emailHelper = new EmailHelper();

        static void Main(string[] args)
        {
            try
            {
                string officeList = String.Empty;
                string[] officeGroup = ConfigurationManager.AppSettings["OFFICE_GROUP"].Split(',');
                string pPara = CommonUnit.CheckEmpty(args[0]);
                string pRpt = CommonUnit.CheckEmpty(args[1]);

                if (String.IsNullOrEmpty(pPara))
                {
                    LogHelper.Error("Input Error Office:" + pPara);
                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "input Office is Null"));
                    Console.ReadKey();
                }
                else
                {
                    string[] arrArg = pPara.Split(',');
                    for (int i = 0; i < arrArg.Length; i++)
                    {
                        if (!officeGroup.Contains(arrArg[i]))
                        {
                            LogHelper.Error("Input Error Office:" + arrArg[i]);
                            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "input Office did not Exists"));
                            Console.ReadKey();
                        }
                    }
                }


                officeList = pPara;

                string[] list = officeList.Split(',');

                if (!String.IsNullOrEmpty(pRpt))
                {
                    pRptValue = pRpt;
                    switch (CommonUnit.CheckEmpty(pRpt).ToUpper())
                    {
                        case "OCEAN_WEEKLY_REPORT":
                            for (int i = 0; i < list.Length; i++)
                            {
                                string strOffice = list[i];
                                if (!OCEAN_WEEKLY_REPORT(strOffice))
                                {
                                    LogHelper.Debug("Sent Error Office:" + strOffice);
                                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Fail"));
                                }
                                else
                                {
                                    LogHelper.Debug("Sent Success Office:" + strOffice);
                                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Success"));
                                }

                                Thread.Sleep(180000);
                            }


                            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "End Program"));
                            break;
                        case "AIR_WEEKLY_REPORT":
                            for (int i = 0; i < list.Length; i++)
                            {
                                string strOffice = list[i];
                                if (!AIR_WEEKLY_REPORT(strOffice))
                                {
                                    LogHelper.Debug("Sent Error Office:" + strOffice);
                                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Fail"));
                                }
                                else
                                {
                                    LogHelper.Debug("Sent Success Office:" + strOffice);
                                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Success"));
                                }

                                Thread.Sleep(180000);
                            }


                            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "End Program"));
                            break;
                        case "MISSING_TRAFFIC_MOTHLY_REPORT":
                            for (int i = 0; i < list.Length; i++)
                            {
                                string strOffice = list[i];
                                if (!MISSING_TRAFFIC_MOTHLY_REPORT(strOffice))
                                {
                                    LogHelper.Debug("Sent Error Office:" + strOffice);
                                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Fail"));
                                }
                                else
                                {
                                    LogHelper.Debug("Sent Success Office:" + strOffice);
                                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Success"));
                                }

                                Thread.Sleep(180000);
                            }

                            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "End Program"));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Rpt Type is Null"));
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Main => Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Main => Message: " + ex.Message + ",StackTrace: " + ex.StackTrace));
            }
        }

        private static bool OCEAN_WEEKLY_REPORT(string originOffice)
        {
            DataSet ds = new DataSet();
            bool ret = true;
            string pOffice = String.Empty;

            try
            {
                string[] arrMAL_Office = new string[] { "PEN", "TSB", "JOH" };
                string[] arrSPRC_Office = new string[] { "ZHG", "GZO", "SZN" };
                string[] arrAMSCenter_Office = new string[] { "BAL", "BAW", "HKK", "JKT", "JOH", "MNL", "PEN", "SIN", "SMG", "SPL", "SUR", "THI", "TJN", "TLK", "TSB", "TSU", "VNM" };
                string mailList = ConfigurationManager.AppSettings[originOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];

                switch (originOffice)
                {
                    case "SPRC":
                        pOffice = String.Join("','", arrSPRC_Office.ToArray());
                        break;
                    case "AMSCenter":
                        pOffice = String.Join("','", arrAMSCenter_Office.ToArray());
                        break;
                    case "SHA":
                        pOffice = "SHA";
                        break;
                    case "MAL":
                        pOffice = String.Join("','", arrMAL_Office.ToArray());
                        break;
                    default:
                        pOffice = originOffice;
                        break;
                }

                pOfficeList_OCEAN = pOffice;

                string fileName = "OCEAN_WEEKLY_REPORT_" + pOfficeList_OCEAN + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                string title = String.Empty;

                if (env == "DEV")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + "OCEAN_WEEKLY_REPORT";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string originPath = path + "\\" + "OCEAN_WEEKLY_REPORT_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    NPOIHelper.ExportExcel_MissingData(ds, originPath);
                    title = "[Test] OCEAN WEEKLY REPORT - " + pOfficeList_OCEAN + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                }
                else
                {
                    title = "OCEAN WEEKLY REPORT - " + pOfficeList_OCEAN + " - " + DateTime.Now.ToString("MM /dd/yyyy/ HH:mm:ss");
                }

                string msg = " <div>To whom it may concern </div>" +
                             " <div>OCEAN WEEKLY REPORT - " + pOfficeList_OCEAN + " (" + DateTime.Now.ToString("MM/dd/yyyy") + ") is ready for you to download. </div>" +
                             " <div>Please review attachment file(" + fileName + "). </div>";

                if (PO_NOT_ASSIGNED() != null )
                {
                    ds.Tables.Add(PO_NOT_ASSIGNED());
                }

                if (MISSING_ATD() != null )
                {
                    ds.Tables.Add(MISSING_ATD());
                }

                if (MISSING_MBL() != null)
                {
                    ds.Tables.Add(MISSING_MBL());
                }

                if (MISSING_CONTAINERNO() != null )
                {
                    ds.Tables.Add(MISSING_CONTAINERNO());
                }

                if (MISSING_NOMINATION_SALES() != null)
                {
                    ds.Tables.Add(MISSING_NOMINATION_SALES());
                }

                if (OUTSTANDING_BOOKING_STATUS() != null)
                {
                    ds.Tables.Add(OUTSTANDING_BOOKING_STATUS());
                }

                if (CONTRACT_TYPE_OR_NUMBER_DISCREPANCY() != null)
                {
                    ds.Tables.Add(CONTRACT_TYPE_OR_NUMBER_DISCREPANCY());
                }

                if (ds.Tables.Count > 0)
                {
                    Dictionary<string, MemoryStream> keyValues = new Dictionary<string, MemoryStream>();
                    MemoryStream stream = NPOIHelper.RenderToMissingDataReport(ds);


                    keyValues.Add(fileName, stream);
                    //LogHelper.Debug("GetAMSFilingData => SendMailViaAPI : Start");
                    emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), mailList, keyValues);

                    //LogHelper.Debug("GetAMSFilingData => SendMailViaAPI : End");
                }
                else
                {
                    msg = "No Data";
                    emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), mailList);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message: " + ex.Message + ",StackTrace: " + ex.StackTrace));
            }

            return ret;
        }
        private static bool AIR_WEEKLY_REPORT(string originOffice)
        {
            DataSet ds = new DataSet();
            bool ret = true;
            string pOffice = String.Empty;

            try
            {
                string[] arrMAL_Office = new string[] { "PEN", "TSB", "JOH" };
                string[] arrSPRC_Office = new string[] { "ZHG", "GZO", "SZN" };
                string[] arrAMSCenter_Office = new string[] { "BAL", "BAW", "HKK", "JKT", "JOH", "MNL", "PEN", "SIN", "SMG", "SPL", "SUR", "THI", "TJN", "TLK", "TSB", "TSU", "VNM" };
                string mailList = ConfigurationManager.AppSettings[originOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];

                switch (originOffice)
                {
                    case "SPRC":
                        pOffice = String.Join("','", arrSPRC_Office.ToArray());
                        break;
                    case "AMSCenter":
                        pOffice = String.Join("','", arrAMSCenter_Office.ToArray());
                        break;
                    case "SHA":
                        pOffice = "SHA";
                        break;
                    case "MAL":
                        pOffice = String.Join("','", arrMAL_Office.ToArray());
                        break;
                    default:
                        pOffice = originOffice;
                        break;
                }

                pOfficeList_AIR = pOffice;
                string fileName = "AIR_WEEKLY_REPORT_" + pOfficeList_AIR + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                string title = String.Empty;

                if (env == "DEV")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + "AIR_WEEKLY_REPORT";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string originPath = path + "\\" + "AIR_WEEKLY_REPORT_" + pOfficeList_AIR + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    NPOIHelper.ExportExcel_MissingData(ds, originPath);
                    title = "[Test] AIR WEEKLY REPORT - " + pOfficeList_AIR + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                }
                else
                {
                    title = "AIR WEEKLY REPORT - " + pOfficeList_AIR + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                }

                string msg = " <div>To whom it may concern </div>" +
                             " <div>AIR WEEKLY REPORT - " + pOfficeList_AIR + "(" + DateTime.Now.ToString("MM/dd/yyyy") + ") is ready for you to download. </div>" +
                             " <div>Please review attachment file(" + fileName + "). </div>";

                if (MISSING_CHARGEBLE_WEIGHT() != null)
                {
                    ds.Tables.Add(MISSING_CHARGEBLE_WEIGHT());
                }

                if (MISSING_ATD_AIR() != null)
                {
                    ds.Tables.Add(MISSING_ATD_AIR());
                }

                if (ds.Tables.Count > 0)
                {
                    Dictionary<string, MemoryStream> keyValues = new Dictionary<string, MemoryStream>();
                    MemoryStream stream = NPOIHelper.RenderToMissingDataReport(ds);


                    keyValues.Add(fileName, stream);
                    //LogHelper.Debug("GetAMSFilingData => SendMailViaAPI : Start");
                    emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), mailList, keyValues);
                }
                else
                {
                    msg = "No Data";
                    emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), mailList);
                }
                

            }
            catch (Exception ex)
            {
                ret = false;
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message: " + ex.Message + ",StackTrace: " + ex.StackTrace));
            }

            return ret;
        }


        private static bool MISSING_TRAFFIC_MOTHLY_REPORT(string originOffice)
        {
            DataSet ds = new DataSet();
            bool ret = true;
            string pOffice = String.Empty;

            try
            {
                string[] arrMAL_Office = new string[] { "PEN", "TSB", "JOH" };
                string[] arrSPRC_Office = new string[] { "ZHG", "GZO", "SZN" };
                string[] arrAMSCenter_Office = new string[] { "BAL", "BAW", "HKK", "JKT", "JOH", "MNL", "PEN", "SIN", "SMG", "SPL", "SUR", "THI", "TJN", "TLK", "TSB", "TSU", "VNM" };
                string mailList = ConfigurationManager.AppSettings[originOffice + "_MAIL"] + "," + ConfigurationManager.AppSettings["Default_MAIL"];

                switch (originOffice)
                {
                    case "SPRC":
                        pOffice = String.Join("','", arrSPRC_Office.ToArray());
                        break;
                    case "AMSCenter":
                        pOffice = String.Join("','", arrAMSCenter_Office.ToArray());
                        break;
                    case "SHA":
                        pOffice = "SHA";
                        break;
                    case "MAL":
                        pOffice = String.Join("','", arrMAL_Office.ToArray());
                        break;
                    default:
                        pOffice = originOffice;
                        break;
                }

                pOfficeList_TRAFFIC = pOffice;

                string fileName = "MISSING_TRAFFIC_MONTHLY_REPORT_" + pOfficeList_TRAFFIC + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                string title = String.Empty;

                if (env == "DEV")
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + "MISSING_TRAFFIC_MONTHLY_REPORT";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string originPath = path + "\\" + "MISSING_TRAFFIC_MONTHLY_REPORT_" + pOfficeList_TRAFFIC + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    NPOIHelper.ExportExcel_MissingData(ds, originPath);
                    title = "[Test] MISSING TRAFFIC MONTHLY REPORT - " + pOfficeList_TRAFFIC + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                }
                else
                {
                    title = "MISSING TRAFFIC MONTHLY REPORT - " + pOfficeList_TRAFFIC + " - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
                }

                string msg = " <div>To whom it may concern </div>" +
                             " <div>MISSING TRAFFIC MONTHLY REPORT - " + pOfficeList_TRAFFIC + " (" + DateTime.Now.ToString("MM/dd/yyyy") + ") is ready for you to download. </div>" +
                             " <div>Please review attachment file(" + fileName + "). </div>";

                if (MISSING_TRAFFIC() != null)
                {
                    ds.Tables.Add(MISSING_TRAFFIC());

                    Dictionary<string, MemoryStream> keyValues = new Dictionary<string, MemoryStream>();
                    MemoryStream stream = NPOIHelper.RenderToMissingDataReport(ds);

                    
                    keyValues.Add(fileName, stream);
                    //LogHelper.Debug("GetAMSFilingData => SendMailViaAPI : Start");
                    emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), mailList, keyValues);
                }
                else
                {
                    msg = "No Data";
                    emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), mailList);
                }
            }
            catch (Exception ex)
            {
                ret = false;
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message: " + ex.Message + ",StackTrace: " + ex.StackTrace));
            }

            return ret;

        }

        private static DataTable PO_NOT_ASSIGNED()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND (ISNULL(WB_CNTR_POLine.ContainerNo, '') IN ('TBA', '')) ";


            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);

            if (dt != null)
            {
                dt.TableName = "PO NOT ASSIGNED";
            }

            return dt;
        }

        private static DataTable MISSING_ATD()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND CYCutOffDate IS NOT NULL " +
                              " AND P2PATD IS NULL ";


            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);
            if (dt != null)
            {
                dt.TableName = "MISSING ATD";
            }

            return dt;
        }

        private static DataTable MISSING_MBL()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND ISNULL(MBL,'') = '' ";


            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);

            if (dt != null)
            {
                dt.TableName = "MISSING MBL";
            }

            return dt;
        }

        private static DataTable MISSING_CONTAINERNO()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND (ISNULL(WB_CNTR_POLine.ContainerNo, '') IN ('TBA', '')) ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);

            if (dt != null)
            {
                dt.TableName = "MISSING CONTAINERNO";
            }

            return dt;
        }

        private static DataTable MISSING_NOMINATION_SALES()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND ISNULL(principal.NomSales,'') = '' ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);
            if (dt != null)
            {
                dt.TableName = "MISSING NOMINATION SALES";
            }

            return dt;
        }

        private static DataTable OUTSTANDING_BOOKING_STATUS()
        {
            string sqlWhere = " AND WB_STATUS IN ('PENDING','NEW') ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);

            if (dt != null)
            {
                dt.TableName = "OUTSTANDING BOOKING STATUS";
            }
            return dt;
        }

        private static DataTable CONTRACT_TYPE_OR_NUMBER_DISCREPANCY()
        {
            string sqlWhere = " AND POTracing.BookingContractType IN ('Topocean')" +
                              " AND contract.ContractNo = 'AGENT CONTRACT' ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);
            if (dt != null)
            {
                dt.TableName = "CONTRACT_TYPE_DISCREPANCY";
            }

            return dt;
        }

        private static DataTable MISSING_CHARGEBLE_WEIGHT()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND WB_CNTR.ChargeableWeight IS NULL ";

            string sql = AIR_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);
            if (dt != null)
            {
                dt.TableName = "MISSING CHARGEBLE WEIGHT";
            }

            return dt;
        }

        private static DataTable MISSING_ATD_AIR()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND CYCutOffDate IS NOT NULL " +
                              " AND P2PATD IS NULL";

            string sql = AIR_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);

            if (dt != null)
            {
                dt.TableName = "MISSING ATD AIR";
            }

            return dt;
        }

        private static DataTable MISSING_TRAFFIC()
        {
            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND ISNULL(tm.TRAFFIC,'') = '' ";

            string sql = MISSING_TRAFFIC_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            DataTable dt = dbHelper.ExecDataTable(sql);
            if (dt != null)
            {
                dt.TableName = "MISSING TRAFFIC";
            }

            return dt;
        }

        private static string ReturnMainSQL()
        {
            string sql = String.Empty;

            switch (CommonUnit.CheckEmpty(pRptValue).ToUpper())
            {
                case "OCEAN_WEEKLY_REPORT":
                    break;
                case "AIR_WEEKLY_REPORT":
                    break;
                case "MISSING_TRAFFIC_WEEKLY_REPORT":
                    break;
                default:
                    break;

            }

            return sql;
        }

        private static string OCEAN_WEEKLY_REPORT_SQL()
        {
            string sql = String.Empty;
            string sql_select = String.Empty;
            string sql_where = String.Empty;
            string sql_group = String.Empty;

            sql_select = String.Format(" Declare @UDate DATETIME;                                                                                                       " +
                         " SET @UDate = dbo.fun_GetDateAsUserTimeZone('sha_lindama');                                                                                   " +
                         " Select                                                                                                                                       " +
                         "      [BRANCH],                                                                                                                               " +
                         "      [Booking ID],                                                                                                                           " +
                         "      [Booking Status],                                                                                                                       " +
                         "      [Created By],                                                                                                                           " +
                         "      [Handling User],                                                                                                                        " +
                         " 	    [ETD],                                                                                                                                  " +
                         " 	    [ATD],                                                                                                                                  " +
                         " 	    [HBL],                                                                                                                                  " +
                         " 	    [MBL],                                                                                                                                  " +
                         " 	    [Container#],                                                                                                                           " +
                         " 	    [Principal],                                                                                                                            " +
                         " 	    [Nomination Sales],                                                                                                                     " +
                         " 	    [Contract Type],                                                                                                                        " +
                         " 	    [Contract#]                                                                                                                             " +
                         " From (Select [dbo].[fun_WBIBrhMapping_v5](POTracing.OriginOffice, POTracing.FinalDest,POTracing.TransportationMode)  AS [BRANCH],	        " +
                         "             POTracing.BookingReqID                                                                                  AS [Booking ID],         " +
                         "             UPPER(POTracing.WB_STATUS)                                                                              AS [Booking Status],     " +
                         "             ISNULL(m.ModifiedBy, '')                                                                                AS [Created By],         " +
                         " 			   ISNULL(max(lv.UserLoginName), '')                                                                       AS [Handling User],      " +
                         " 			   POTracing.P2PETD                                                                                        AS [ETD],                " +
                         " 			   POTracing.P2PATD                                                                                        AS [ATD],                " +
                         " 			   POTracing.HBL                                                                                           AS [HBL],                " +
                         " 			   POTracing.MBL                                                                                           AS [MBL],                " +
                         " 			   [dbo].[udf_GetContainerListByBookingReqID](POTracing.BookingReqID)                                      AS [Container#],         " +
                         " 			   principal.Company                                                                                       AS [Principal],          " +
                         " 			   ISNULL(CASE                                                                                                                      " +
                         "                           WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomSales                                                  " +
                         "                           ELSE non_sea_trans.NomSales END," +
                         "                       '')                                                                                                                                                                              AS [Nomination Sales],   " +
                         " 			   ISNULL(POTracing.BookingContractType, '')                                                               AS [Contract Type],      " +
                         " 			   ISNULL(contract.ContractNo, '')                                                                         AS [Contract#]           " +
                         "           FROM POTracing                                                                                                                     " +
                         "     LEFT  JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser                                                                     " +
                         "     INNER JOIN WB_CNTR_POLine ON WB_CNTR_POLine.bookingreqid = POTracing.bookingreqid                                                        " +
                         "     LEFT  JOIN WB_CNTR ON WB_CNTR_POLine.ContainerNo = WB_CNTR.ContainerNo and WB_CNTR_POLine.BookingReqID = WB_CNTR.BookingReqID            " +
                         "     LEFT  JOIN Customer principal ON principal.uID = POTracing.Principle                                                                     " +
                         "     LEFT  JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice' AND agent.IsActive = 'Y' AND Criteria3 = 'Y' AND (Criteria5 <> '' OR Criteria5 IS NOT NULL) " +
                         "     LEFT  JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE " +
                         "     LEFT  JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID " +
                         "     OUTER APPLY (Select Top 1 tm.TRAFFIC From TrafficMap tm Join LocationMgr traffic On tm.Country_code = traffic.CountryCode Where traffic.Location = POTracing.FinalDest) tm " +
                         "     OUTER APPLY (Select Top 1 ModifiedBy From ModifyRecord m Where CargoId = POTracing.uID and m.modifiedBy <> '' Order By ActionDate asc) m  "
                         );

            sql_where = string.Format("  WHERE (1 = 1)                                                    " +
                        "    AND (POTracing.ISTBS = 'Y' AND POTracing.ISTBS IS NOT NULL)    " +
                        "    AND (POTracing.BookingReqID <> '')                             " +
                        "    AND (BookingDate > '8/13/2021 3:49:53 PM')                     " +
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) <= 2)                         " +
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) >= 0)                         " +
                        "    AND (BookingDate >= DateAdd(yy, -1, GetDate()))                " +
                        "    AND POTracing.TransportationMode IN ('-1', 'SEA') " +
                        "    AND POTracing.OriginOffice IN ({0}) " +
                        "    @SQL_WHERE   ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(pOfficeList_OCEAN)));

            sql_group = " GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo, POTracing.CNTRType,                  " +
                        "          POTracing.CNTRType2, POTracing.CNTRType3, POTracing.CNTRType4, POTracing.CNTRQty, POTracing.CNTRQty2,     " +
                        "          POTracing.CNTRQty3, POTracing.CNTRQty4, POTracing.TransportationMode, POTracing.OriginOffice,             " +
                        "          POTracing.Dest, POTracing.CNEE, POTracing.Vendor, m.ModifiedBy, agent.Criteria5, principal.Company,       " +
                        "          CASE WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomName ELSE non_sea_trans.NomName END,     " +
                        "          CASE WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomSales ELSE non_sea_trans.NomSales END,   " +
                        "          POTracing.BookingContractType, contract.ContractNo, POTracing.BookingConfirmation, POTracing.BookingDate, " +
                        "          POTracing.BookingReqID, POTracing.WB_STATUS, POTracing.DeliveryType, POTracing.Carrier,                   " +
                        "          POTracing.FreightForwarder, POTracing.MBL, POTracing.HBL, POTracing.Vessel, POTracing.Voyage,             " +
                        "          POTracing.CarrierDest, POTracing.LoadPort, POTracing.POReadyDate, POTracing.P2PETD,                       " +
                        "          POTracing.OriginalP2PETD, POTracing.P2PATD, POTracing.DischPort, POTracing.P2PETA, POTracing.D2DETA,      " +
                        "          POTracing.OriginalP2PETA, POTracing.DestRamp, POTracing.FinalDest, POTracing.Description,                 " +
                        "          POTracing.Commodity2, POTracing.Orig, POTracing.WBApprovalStatus, POTracing.WBApprovalDate,               " +
                        "          POTracing.WBApprovalRemark, POTracing.ISFSentDate, POTracing.Comments, POTracing.BookingInstruction,      " +
                        "          POTracing.ISTBS, SICutoffDate, CYCutoffDate) a                                                            ";


            sql = sql_select + sql_where + sql_group;

            return sql;
        }

        private static string AIR_WEEKLY_REPORT_SQL()
        {
            string sql = String.Empty;
            string sql_select = String.Empty;
            string sql_where = String.Empty;
            string sql_group = String.Empty;

            sql_select = String.Format(" Declare @UDate DATETIME;                                                                                                       " +
                         " SET @UDate = dbo.fun_GetDateAsUserTimeZone('sha_lindama');                                                                                   " +
                         " Select                                                                                                                                       " +
                         "      [BRANCH],                                                                                                                               " +
                         "      [Booking ID],                                                                                                                           " +
                         "      [Booking Status],                                                                                                                       " +
                         "      [Created By],                                                                                                                           " +
                         "      [Handling User],                                                                                                                        " +
                         " 	    [ETD],                                                                                                                                  " +
                         " 	    [ATD],                                                                                                                                  " +
                         " 	    [HBL],                                                                                                                                  " +
                         " 	    [MBL],                                                                                                                                  " +
                         " 	    [Container#],                                                                                                                           " +
                         " 	    [Principal],                                                                                                                            " +
                         " 	    [Nomination Sales],                                                                                                                     " +
                         " 	    [Chargeable Weight]                                                                                                                     " +
                         //" 	    [Contract Type],                                                                                                                        " +
                         //" 	    [Contract#]                                                                                                                             " +
                         " From (Select [dbo].[fun_WBIBrhMapping_v5](POTracing.OriginOffice, POTracing.FinalDest,POTracing.TransportationMode)  AS [BRANCH],	        " +
                         "             POTracing.BookingReqID                                                                                  AS [Booking ID],         " +
                         "             UPPER(POTracing.WB_STATUS)                                                                              AS [Booking Status],     " +
                         "             ISNULL(m.ModifiedBy, '')                                                                                AS [Created By],         " +
                         " 			   ISNULL(max(lv.UserLoginName), '')                                                                       AS [Handling User],      " +
                         " 			   POTracing.P2PETD                                                                                        AS [ETD],                " +
                         " 			   POTracing.P2PATD                                                                                        AS [ATD],                " +
                         " 			   POTracing.HBL                                                                                           AS [HBL],                " +
                         " 			   POTracing.MBL                                                                                           AS [MBL],                " +
                         " 			   [dbo].[udf_GetContainerListByBookingReqID](POTracing.BookingReqID)                                      AS [Container#],         " +
                         " 			   principal.Company                                                                                       AS [Principal],          " +
                         " 			   ISNULL(CASE                                                                                                                      " +
                         "                           WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomSales                                                  " +
                         "                           ELSE non_sea_trans.NomSales END," +
                         "                       '')                                                                                            AS [Nomination Sales],   " +
                         "              WB_CNTR.ChargeableWeight                                                                                AS [Chargeable Weight]   " +
                         //" 			   ISNULL(POTracing.BookingContractType, '')                                                               AS [Contract Type],      " +
                         //" 			   ISNULL(contract.ContractNo, '')                                                                         AS [Contract#]           " +
                         "           FROM POTracing                                                                                                                     " +
                         "     LEFT  JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser                                                                     " +
                         "     INNER JOIN WB_CNTR_POLine ON WB_CNTR_POLine.bookingreqid = POTracing.bookingreqid                                                        " +
                         "     LEFT  JOIN WB_CNTR ON WB_CNTR_POLine.ContainerNo = WB_CNTR.ContainerNo and WB_CNTR_POLine.BookingReqID = WB_CNTR.BookingReqID            " +
                         "     LEFT  JOIN Customer principal ON principal.uID = POTracing.Principle                                                                     " +
                         "     LEFT  JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice' AND agent.IsActive = 'Y' AND Criteria3 = 'Y' AND (Criteria5 <> '' OR Criteria5 IS NOT NULL) " +
                         "     LEFT  JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE " +
                         //"     LEFT  JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID " +
                         "     OUTER APPLY (Select Top 1 tm.TRAFFIC From TrafficMap tm Join LocationMgr traffic On tm.Country_code = traffic.CountryCode Where traffic.Location = POTracing.FinalDest) tm " +
                         "     OUTER APPLY (Select Top 1 ModifiedBy From ModifyRecord m Where CargoId = POTracing.uID and m.modifiedBy <> '' Order By ActionDate asc) m  "
                         );

            sql_where = "  WHERE (1 = 1)                                                    " +
                        "    AND (POTracing.BookingContractType in ('TOPOCEAN' , 'NVO/Non-Topocean Contract') Or (POTracing.ISTBS = 'Y' and POTracing.ISTBS is not null) )    " +
                        "    AND (POTracing.BookingReqID <> '')                             " +
                        "    AND (BookingDate > '8/13/2021 3:49:53 PM')                     " +
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) <= 2)                         " +
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) >= 0)                         " +
                        "    AND (BookingDate >= DateAdd(yy, -1, GetDate()))                " +
                        "    AND POTracing.TransportationMode IN ('-1', 'AIR') @SQL_WHERE   ";

            sql_group = " GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo,WB_CNTR.ContainerNo                   " +
                        "          , POTracing.TransportationMode, POTracing.OriginOffice,             " +
                        "          POTracing.Dest, POTracing.CNEE, POTracing.Vendor, m.ModifiedBy, agent.Criteria5, principal.Company,       " +
                        "          CASE WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomName ELSE non_sea_trans.NomName END,     " +
                        "          CASE WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomSales ELSE non_sea_trans.NomSales END,   " +
                        "          POTracing.BookingContractType, POTracing.BookingConfirmation, POTracing.BookingDate, " +
                        "          POTracing.BookingReqID, POTracing.WB_STATUS, POTracing.DeliveryType, POTracing.Carrier,                   " +
                        "          POTracing.FreightForwarder, POTracing.MBL, POTracing.HBL, POTracing.Vessel, POTracing.Voyage,             " +
                        "          POTracing.CarrierDest, POTracing.LoadPort, POTracing.POReadyDate, POTracing.P2PETD,                       " +
                        "          POTracing.OriginalP2PETD, POTracing.P2PATD, POTracing.DischPort, POTracing.P2PETA, POTracing.D2DETA,      " +
                        "          POTracing.OriginalP2PETA, POTracing.DestRamp, POTracing.FinalDest, POTracing.Description,                 " +
                        "          POTracing.Commodity2, POTracing.Orig, POTracing.WBApprovalStatus, POTracing.WBApprovalDate,               " +
                        "          POTracing.WBApprovalRemark, POTracing.ISFSentDate, POTracing.Comments, POTracing.BookingInstruction,      " +
                        "          POTracing.ISTBS, SICutoffDate, CYCutoffDate, WB_CNTR.ChargeableWeight) a                                                            ";


            sql = sql_select + sql_where + sql_group;

            return sql;
        }

        private static string MISSING_TRAFFIC_WEEKLY_REPORT_SQL()
        {
            string sql = String.Empty;
            string sql_select = String.Empty;
            string sql_where = String.Empty;
            string sql_group = String.Empty;

            sql_select = String.Format(" Declare @UDate DATETIME;                                                                                                       " +
                         " SET @UDate = dbo.fun_GetDateAsUserTimeZone('sha_lindama');                                                                                   " +
                         " Select                                                                                                                                       " +
                         "      [Booking ID],                                                                                                                           " +
                         "      [Place of Delivery],                                                                                                                    " +
                         "      [Traffic],                                                                                                                              " +
                         " 	    [Principal],                                                                                                                            " +
                         "      [CNEE]                                                                                                                           " +

                         " From (Select [dbo].[fun_WBIBrhMapping_v5](POTracing.OriginOffice, POTracing.FinalDest,POTracing.TransportationMode)  AS [BRANCH],	        " +
                         "             POTracing.BookingReqID                                                                                  AS [Booking ID],         " +
                         "             POTracing.FinalDest                                                                                     AS [Place of Delivery],  " +
                         " 			   principal.Company                                                                                       AS [Principal],          " +
                         " 			   tm.TRAFFIC                                                                                              AS [Traffic],            " +
                         " 			   POTracing.CNEE                                                                                          AS [CNEE]                " +
                         "           FROM POTracing                                                                                                                     " +
                         "     LEFT  JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser                                                                     " +
                         "     INNER JOIN WB_CNTR_POLine ON WB_CNTR_POLine.bookingreqid = POTracing.bookingreqid                                                        " +
                         "     LEFT  JOIN WB_CNTR ON WB_CNTR_POLine.ContainerNo = WB_CNTR.ContainerNo and WB_CNTR_POLine.BookingReqID = WB_CNTR.BookingReqID            " +
                         "     LEFT  JOIN Customer principal ON principal.uID = POTracing.Principle                                                                     " +
                         "     LEFT  JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice' AND agent.IsActive = 'Y' AND Criteria3 = 'Y' AND (Criteria5 <> '' OR Criteria5 IS NOT NULL) " +
                         "     LEFT  JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE " +
                         "     LEFT  JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID " +
                         "     OUTER APPLY (Select Top 1 tm.TRAFFIC From TrafficMap tm Join LocationMgr traffic On tm.Country_code = traffic.CountryCode Where traffic.Location = POTracing.FinalDest) tm " +
                         "     OUTER APPLY (Select Top 1 ModifiedBy From ModifyRecord m Where CargoId = POTracing.uID and m.modifiedBy <> '' Order By ActionDate asc) m  "
                         );

            sql_where = string.Format("  WHERE (1 = 1)                                                    " +
                        "    AND (POTracing.BookingContractType in ('TOPOCEAN' , 'NVO/Non-Topocean Contract') Or (POTracing.ISTBS = 'Y' and POTracing.ISTBS is not null) )    " +
                        "    AND (POTracing.BookingReqID <> '')                             " +
                        "    AND (BookingDate > '8/13/2021 3:49:53 PM')                     " +
                        "    AND (DATEDIFF(MONTH,P2PETD,@UDate) <= 1)                       " +
                        "    AND (DATEDIFF(MONTH,P2PETD,@UDate) >=0)                        " +
                        "    AND (BookingDate >= DateAdd(yy, -1, GetDate()))                " +
                        "    AND POTracing.OriginOffice IN ({0}) " +
                        "    @SQL_WHERE   ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(pOfficeList_TRAFFIC)));

            sql_group = " GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo, POTracing.CNTRType,                  " +
                        "          POTracing.CNTRType2, POTracing.CNTRType3, POTracing.CNTRType4, POTracing.CNTRQty, POTracing.CNTRQty2,     " +
                        "          POTracing.CNTRQty3, POTracing.CNTRQty4, POTracing.TransportationMode, POTracing.OriginOffice,             " +
                        "          POTracing.Dest, POTracing.CNEE, POTracing.Vendor, m.ModifiedBy, agent.Criteria5, principal.Company,       " +
                        "          CASE WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomName ELSE non_sea_trans.NomName END,     " +
                        "          CASE WHEN POTracing.TransportationMode = 'SEA' THEN principal.NomSales ELSE non_sea_trans.NomSales END,   " +
                        "          POTracing.BookingContractType, contract.ContractNo, POTracing.BookingConfirmation, POTracing.BookingDate, " +
                        "          POTracing.BookingReqID, POTracing.WB_STATUS, POTracing.DeliveryType, POTracing.Carrier,                   " +
                        "          POTracing.FreightForwarder, POTracing.MBL, POTracing.HBL, POTracing.Vessel, POTracing.Voyage,             " +
                        "          POTracing.CarrierDest, POTracing.LoadPort, POTracing.POReadyDate, POTracing.P2PETD,                       " +
                        "          POTracing.OriginalP2PETD, POTracing.P2PATD, POTracing.DischPort, POTracing.P2PETA, POTracing.D2DETA,      " +
                        "          POTracing.OriginalP2PETA, POTracing.DestRamp, POTracing.FinalDest, POTracing.Description,                 " +
                        "          POTracing.Commodity2, POTracing.Orig, POTracing.WBApprovalStatus, POTracing.WBApprovalDate,               " +
                        "          POTracing.WBApprovalRemark, POTracing.ISFSentDate, POTracing.Comments, POTracing.BookingInstruction,      " +
                        "          POTracing.ISTBS, SICutoffDate, CYCutoffDate,tm.TRAFFIC) a                                                 ";


            sql = sql_select + sql_where + sql_group;

            return sql;
        }

    }
}
