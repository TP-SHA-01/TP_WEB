using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
        static DBHelper dbHelper = new DBHelper();
        static string env = BaseCont.ENVIRONMENT;
        static EmailHelper emailHelper = new EmailHelper();

        static void Main(string[] args)
        {
            try
            {
                //string pPara = CommonUnit.CheckEmpty(args[0]);
                string pRpt = CommonUnit.CheckEmpty(args[0]);

                if (!String.IsNullOrEmpty(pRpt))
                {
                    pRptValue = pRpt;
                    switch (CommonUnit.CheckEmpty(pRpt).ToUpper())
                    {
                        case "OCEAN_WEEKLY_REPORT":
                            OCEAN_WEEKLY_REPORT();
                            break;
                        case "AIR_WEEKLY_REPORT":
                            AIR_WEEKLY_REPORT();
                            break;
                        case "MISSING_TRAFFIC_WEEKLY_REPORT":
                            MISSING_TRAFFIC_WEEKLY_REPORT();
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

        private static void OCEAN_WEEKLY_REPORT()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(PO_NOT_ASSIGNED());
            ds.Tables.Add(MISSING_ATD());
            ds.Tables.Add(MISSING_MBL());
            ds.Tables.Add(MISSING_CONTAINERNO());
            ds.Tables.Add(MISSING_NOMINATION_SALES());
            ds.Tables.Add(OUTSTANDING_BOOKING_STATUS());
            ds.Tables.Add(CONTRACT_TYPE_OR_NUMBER_DISCREPANCY());

            Dictionary<string, MemoryStream> keyValues = new Dictionary<string, MemoryStream>();
            MemoryStream stream = NPOIHelper.RenderToMissingDataReport(ds);
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + "OCEAN_WEEKLY_REPORT";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string originPath = path + "\\" + "OCEAN_WEEKLY_REPORT_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            NPOIHelper.ExportExcel_MissingData(ds, originPath);


            string fileName = "OCEAN_WEEKLY_REPORT_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            string title = String.Empty;

            if (env == "DEV")
            {
                title = "[Test] OCEAN WEEKLY REPORT - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
            }
            else
            {
                title = "[Test] OCEAN WEEKLY REPORT - " + DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss");
            }

            string msg = " <div>To whom it may concern </div>" +
                         " <div>OCEAN WEEKLY REPORT - ("+ DateTime.Now.ToString("MM/dd/yyyy") + ") is ready for you to download. </div>" +
                         " <div>Please review attachment file(" + fileName + "). </div>";
            keyValues.Add(fileName, stream);
            //LogHelper.Debug("GetAMSFilingData => SendMailViaAPI : Start");
            emailHelper.SendMailViaAPI(title, CommonFun.GetHtmlString(msg), "ethanshen@topocean.com.cn;lindama@topocean.com.cn", keyValues);

            //LogHelper.Debug("GetAMSFilingData => SendMailViaAPI : End");

        }
        private static void AIR_WEEKLY_REPORT()
        {

        }
        private static void MISSING_TRAFFIC_WEEKLY_REPORT()
        {

        }

        private static DataTable PO_NOT_ASSIGNED()
        {
            DataTable dt = new DataTable("PO NOT ASSIGNED");

            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND (ISNULL(WB_CNTR_POLine.ContainerNo, '') IN ('TBA', '')) ";


            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "PO NOT ASSIGNED";
            return dt;
        }

        private static DataTable MISSING_ATD()
        {
            DataTable dt = new DataTable("MISSING ATD");

            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND CYCutOffDate IS NOT NULL " +
                              " AND P2PATD IS NULL ";


            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "MISSING ATD";
            return dt;
        }

        private static DataTable MISSING_MBL()
        {
            DataTable dt = new DataTable("MISSING MBL");

            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND ISNULL(MBL,'') = '' ";


            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "MISSING MBL";
            return dt;
        }

        private static DataTable MISSING_CONTAINERNO()
        {
            DataTable dt = new DataTable("MISSING CONTAINERNO");

            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND (ISNULL(WB_CNTR_POLine.ContainerNo, '') IN ('TBA', '')) ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "MISSING CONTAINERNO";
            return dt;
        }

        private static DataTable MISSING_NOMINATION_SALES()
        {
            DataTable dt = new DataTable("MISSING NOMINATION SALES");

            string sqlWhere = " AND ISNULL(WB_Status, '') IN ('CONFIRMED') " +
                              " AND ISNULL(principal.NomSales,'') = '' ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "MISSING NOMINATION SALES";
            return dt;
        }

        private static DataTable OUTSTANDING_BOOKING_STATUS()
        {
            DataTable dt = new DataTable("OUTSTANDING_BOOKING_STATUS");

            string sqlWhere = " AND WB_STATUS IN ('PENDING','NEW') ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "OUTSTANDING BOOKING STATUS";
            return dt;
        }

        private static DataTable CONTRACT_TYPE_OR_NUMBER_DISCREPANCY()
        {
            DataTable dt = new DataTable("CONTRACT TYPE/NUMBER DISCREPANCY");

            string sqlWhere = " AND POTracing.BookingContractType IN ('Topocean')" +
                              " AND contract.ContractNo = 'AGENT CONTRACT' ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "CONTRACT_TYPE_DISCREPANCY";
            return dt;
        }

        private static DataTable CONTRACT_TYPE_OR_NUMBER_DISCREPANCY()
        {
            DataTable dt = new DataTable("CONTRACT TYPE/NUMBER DISCREPANCY");

            string sqlWhere = " AND POTracing.BookingContractType IN ('Topocean')" +
                              " AND contract.ContractNo = 'AGENT CONTRACT' ";

            string sql = OCEAN_WEEKLY_REPORT_SQL().Replace("@SQL_WHERE", sqlWhere);

            dt = dbHelper.ExecDataTable(sql);
            dt.TableName = "CONTRACT_TYPE_DISCREPANCY";
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

            sql_where = "  WHERE (1 = 1)                                                    " +
                        "    AND (POTracing.ISTBS = 'Y' AND POTracing.ISTBS IS NOT NULL)    " +
                        "    AND (POTracing.BookingReqID <> '')                             " +
                        "    AND (BookingDate > '8/13/2021 3:49:53 PM')                     " +
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) <= 2)                         " +
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) >0)                           " +
                        "    AND (BookingDate >= DateAdd(yy, -1, GetDate()))                " +
                        "    AND POTracing.TransportationMode IN ('-1', 'SEA') @SQL_WHERE   ";

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
                         "                       '')                                                                                                                                                                              AS [Nomination Sales],   " +
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
                        "    AND (DATEDIFF(Day,P2PETD,@UDate) >0)                           " +
                        "    AND (BookingDate >= DateAdd(yy, -1, GetDate()))                " +
                        "    AND POTracing.TransportationMode IN ('-1', 'SEA') @SQL_WHERE   ";

            sql_group = " GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo, POTracing.CNTRType,                  " +
                        "          POTracing.CNTRType2, POTracing.CNTRType3, POTracing.CNTRType4, POTracing.CNTRQty, POTracing.CNTRQty2,     " +
                        "          POTracing.CNTRQty3, POTracing.CNTRQty4, POTracing.TransportationMode, POTracing.OriginOffice,             " +
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
                        "          POTracing.ISTBS, SICutoffDate, CYCutoffDate) a                                                            ";


            sql = sql_select + sql_where + sql_group;

            return sql;
        }

    }
}
