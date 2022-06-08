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
    public class BookingAdviceAnalysisRpt_Service:IBookingAdviceAnalysisRpt
    {
	
		static string env = BaseCont.ENVIRONMENT;
        static EmailHelper emailHelper = new EmailHelper();
        static private string pOriginOffice { get; set; }

		//For testing : isnull(principal.Company,POTracing.CNEE)  && ModifiedBy,Principal

		static string selSQL = String.Format(@" if object_id(N'tempsheet1',N'U') is not null drop table tempsheet1  Select Top 20000 
											[Week], BRANCH,[Booking ID],[Booking Date],[Booking Status],
											[Transportation Mode],[Created By],[Handling User Email],
											[Origin Office],[Dest Office], Principal, Consignee, Vendor,
											[Place Of Receipt],[Carrier Dest], Carrier,[Contract Type],
											[Contract No],[Carrier Commodity], Vessel, Voyage, MBL, HBL,
											[Delivery Type],[PO Ready Date],[POL],[Original ETD],[Current ETD],
											ATD,[POD],[Original ETA],[Current ETA],[Final Dest],[Final ETA],
											[Dest Ramp],[Booked 40],[Booked 20],[Booked HQ],[Booked 45],
											[Booked LCL],[Booked 53],
											(((a.[Booked 40] + a.[Booked HQ] + a.[Booked 45] + a.[Booked 53]) * 1) + (a.[Booked 20] * .5)) As[Booked FEU],
											[Approval Status],[Approval Date],[Approval Remark],[VGM Sent],
											[ISF Sent Date],[Customs Response #],[Forwarder Remarks],[Nomination Office],
											[Nomination Sales],[Container List],[Rolled],[Service String],[IsTBS] into tempsheet1
											From(
												Select
												CASE WHEN POTracing.ISTBS = 'Y' THEN ISNULL(POTracing.WB_WeekOfYear, 0)
												ELSE[dbo].[udf_GetTBSWeek](POTracing.OriginalVesselETD) END as [Week],
												[dbo].[fun_WBIBrhMapping_v5](POTracing.OriginOffice, POTracing.FinalDest,
												POTracing.TransportationMode) as [BRANCH], POTracing.BookingDate as [Booking Date],
												'' + POTracing.BookingReqID + '' as [Booking ID], UPPER(POTracing.WB_STATUS) as [Booking Status],
												POTracing.TransportationMode As[Transportation Mode], ISNULL(m.ModifiedBy,POTracing.CreatedBy ) as [Created By],
												ISNULL(max(lv.UserEmail), '') as [Handling User Email], ISNULL(POTracing.OriginOffice,'' ) as [Origin Office],
												ISNULL(POTracing.Dest, '') as [Dest Office], isnull(principal.Company,'') as [Principal],
												POTracing.CNEE as [Consignee], POTracing.Vendor as [Vendor], POTracing.Orig as [Place Of Receipt],
												(case when POTracing.CarrierDestType = 'PORT'
												then dbo.fun_ValidPortStrByType(POTracing.CarrierDest, 'ALL') else POTracing.CarrierDest end) as [Carrier Dest],
												POTracing.Carrier as [Carrier],ISNULL(POTracing.BookingContractType, '') As[Contract Type],
												ISNULL(contract.ContractNo, '') as [Contract No],POTracing.Commodity2 As[Carrier Commodity],
												POTracing.Vessel as [Vessel],POTracing.Voyage as [Voyage],POTracing.MBL as [MBL],
												POTracing.HBL as [HBL],POTracing.DeliveryType as [Delivery Type],POTracing.POReadyDate As[PO Ready Date],
												POTracing.LoadPort as [POL],POTracing.OriginalP2PETD As[Original ETD], POTracing.P2PETD As[Current ETD],
												POTracing.P2PATD As[ATD], POTracing.DischPort as [POD],POTracing.OriginalP2PETA As[Original ETA],
												POTracing.P2PETA As[Current ETA], POTracing.FinalDest as [Final Dest],POTracing.D2DETA As[Final ETA],
												POTracing.DestRamp as [Dest Ramp],
												(CASE WHEN POTracing.cntrType In('40''TKC','40''SD','40RF','40''PLWD','40''PLF','40''OPT','40''GOH','40''FLR')
												THEN NULLIF(POTracing.cntrqty,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType2 In('40''TKC','40''SD','40RF','40''PLWD','40''PLF','40''OPT','40''GOH','40''FLR')
												THEN NULLIF(POTracing.cntrqty2,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType3 In('40''TKC','40''SD','40RF','40''PLWD','40''PLF','40''OPT','40''GOH','40''FLR')
												THEN NULLIF(POTracing.cntrqty3,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType4 In('40''TKC','40''SD','40RF','40''PLWD','40''PLF','40''OPT','40''GOH','40''FLR') 
												THEN NULLIF(POTracing.cntrqty4,0) ELSE 0 End) as [Booked 40],
												(CASE WHEN POTracing.cntrType In('20''TKC','20''SD','20RF','20''PLF','20''OPT','20''NOR','20''GOH') 
												THEN NULLIF(POTracing.cntrqty,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType2 In('20''TKC','20''SD','20RF','20''PLF','20''OPT','20''NOR','20''GOH') 
												THEN NULLIF(POTracing.cntrqty2,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType3 In('20''TKC','20''SD','20RF','20''PLF','20''OPT','20''NOR','20''GOH') 
												THEN NULLIF(POTracing.cntrqty3,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType4 In('20''TKC','20''SD','20RF','20''PLF','20''OPT','20''NOR','20''GOH') 
												THEN NULLIF(POTracing.cntrqty4,0) ELSE 0 End) as [Booked 20],(CASE WHEN POTracing.cntrType In('HQ''NOR','40HQ') 
												THEN NULLIF(POTracing.cntrqty,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType2 In('HQ''NOR','40HQ') 
												THEN NULLIF(POTracing.cntrqty2,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType3 In('HQ''NOR','40HQ') 
												THEN NULLIF(POTracing.cntrqty3,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType4 In('HQ''NOR','40HQ') 
												THEN NULLIF(POTracing.cntrqty4,0) ELSE 0 End) as [Booked HQ],(CASE WHEN POTracing.cntrType In('45RF','45HQ') 
												THEN NULLIF(POTracing.cntrqty,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType2 In('45RF','45HQ') 
												THEN NULLIF(POTracing.cntrqty2,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType3 In('45RF','45HQ') 
												THEN NULLIF(POTracing.cntrqty3,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType4 In('45RF','45HQ') 
												THEN NULLIF(POTracing.cntrqty4,0) ELSE 0 End) as [Booked 45],(CASE WHEN POTracing.cntrType In('LCL') 
												THEN NULLIF(POTracing.cntrqty,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType2 In('LCL') 
												THEN NULLIF(POTracing.cntrqty2,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType3 In('LCL') 
												THEN NULLIF(POTracing.cntrqty3,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType4 In('LCL') 
												THEN NULLIF(POTracing.cntrqty4,0) ELSE 0 End) as [Booked LCL],(CASE WHEN POTracing.cntrType In('53HQ') 
												THEN NULLIF(POTracing.cntrqty,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType2 In('53HQ') 
												THEN NULLIF(POTracing.cntrqty2,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType3 In('53HQ') 
												THEN NULLIF(POTracing.cntrqty3,0) ELSE 0 End) +(CASE WHEN POTracing.cntrType4 In('53HQ') 
												THEN NULLIF(POTracing.cntrqty4,0) ELSE 0 End) as [Booked 53],POTracing.WBApprovalStatus As[Approval Status],
												POTracing.WBApprovalDate As[Approval Date], POTracing.WBApprovalRemark As[Approval Remark],'' As[VGM Sent],
												POTracing.ISFSentDate As[ISF Sent Date], POTracing.CustomsResponseNo As[Customs Response #],
												POTracing.BookingInstruction As[Forwarder Remarks], ISNULL(principal.NomName, '') as [Nomination Office],
												ISNULL(principal.NomSales, '') as [Nomination Sales],[dbo].[udf_GetContainerListByBookingReqID](POTracing.BookingReqID) as [Container List], 
												(case when((select count(*) from TBS_Route
												where WBIID = POTracing.BookingReqID and TBS_Route.IsActive = 1) > 1 and POTracing.loadedOnMotherVesselFlag = 'Y') then 'Y' else 'N' end) as [Rolled], [dbo].[udf_GetServiceStringByID](POTracing.BookingReqID) as [Service String], 
												ISNULL(POTracing.IsTBS, 'N') as [IsTBS]
												FROM POTracing LEFT JOIN Customer principal ON principal.uID = POTracing.Principle
												LEFT JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice'
												AND agent.IsActive = 'Y' AND Criteria3 = 'Y' AND(Criteria5 <> '' OR Criteria5 IS NOT NULL)
												LEFT JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE
												LEFT JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser
												LEFT JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID
												OUTER APPLY(Select Top 1 tm.TRAFFIC From TrafficMap tm Join LocationMgr traffic On tm.Country_code = traffic.CountryCode Where traffic.Location = POTracing.FinalDest) tm
											   OUTER APPLY(Select Top 1 ModifiedBy From ModifyRecord m Where CargoId = POTracing.uID and m.modifiedBy <> '' Order By ActionDate asc) m WHERE(POTracing.BookingContractType IN ('TOPOCEAN','NVO/NON-TOPOCEAN CONTRACT') Or POTracing.IsTBS = 'Y')
												And POTracing.CNEE Not In('ABC COMPUTER', 'ABC COMPUTER-TBS', 'ABC COMP EU')
												And ISNULL(POTracing.WB_Status, '') NOT IN('REJECTED','CANCELLED') AND POTracing.BookingReqID IS NOT NULL
												And(TopoceanYearAndWeek between weekfrom and weekto)   
												And(POTracing.BookingReqID <> '')
												And (POTracing.IsTBS = 'Y')
												And(POTracing.Traffic = 'USA')
												And(POTracing.TransportationMode = 'SEA')
												And(POTracing.BookingContractType = 'TOPOCEAN')
												And(POTracing.OriginOffice in ('GZO', 'HKG', 'SZN', 'ZHG'))
												GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo, POTracing.CNTRType,
												POTracing.CNTRType2, POTracing.CNTRType3, POTracing.CNTRType4, POTracing.CNTRQty, POTracing.CNTRQty2, 
												POTracing.CNTRQty3, POTracing.CNTRQty4, POTracing.TransportationMode, POTracing.OriginOffice, POTracing.Dest, 
												POTracing.CNEE, POTracing.Vendor, m.ModifiedBy, agent.Criteria5, principal.Company, principal.NomName,
												principal.NomSales, POTracing.BookingContractType, contract.ContractNo, POTracing.BookingConfirmation, 
												POTracing.BookingDate, POTracing.BookingReqID, POTracing.WB_STATUS, POTracing.DeliveryType, POTracing.Carrier,
												POTracing.MBL, POTracing.HBL, POTracing.Vessel, POTracing.Voyage, POTracing.CarrierDestType, POTracing.CarrierDest, 
												POTracing.LoadPort, POTracing.POReadyDate, POTracing.P2PETD, POTracing.OriginalVesselETD, POTracing.OriginalP2PETD,
												POTracing.P2PATD, POTracing.DischPort, POTracing.P2PETA, POTracing.D2DETA, POTracing.OriginalP2PETA, POTracing.DestRamp,
												POTracing.FinalDest, POTracing.Description, POTracing.Commodity2, POTracing.Orig, POTracing.WBApprovalStatus,
												POTracing.WBApprovalDate, POTracing.WBApprovalRemark, POTracing.ISFSentDate, POTracing.Comments,
												POTracing.BookingInstruction, POTracing.ISTBS, POTracing.loadedOnMotherVesselFlag,POTracing.ModifiedBy,POTracing.CreatedBy  ) a");
		public IEnumerable<string> GetValue()
        {
            return new List<string>() { "hi", "PO" };
        }

        public static Load_SPRC_ResponseData GetBookingAdviceByBranchData(string weekfrom,string weekto)
        {

			Load_SPRC_ResponseData responseMode = new Load_SPRC_ResponseData();

            try
            {
				DataTable dt_sheet1 = new DataTable();
				dt_sheet1 = GetData(weekfrom, weekto,"sheet1");

				DataTable dt_Account = new DataTable();
				dt_Account = GetData(weekfrom, weekto, "Account");

				DataTable dt_Carrier = new DataTable();
				dt_Carrier = GetData(weekfrom, weekto, "Carrier");

				responseMode.sheet1 = dt_sheet1;
				responseMode.Account = dt_Account;
				responseMode.Carrier = dt_Carrier;


			}
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }

            return responseMode;
        }

		public static Load_Volume_ResponseData GetBookingVolumeByBranchData(string weekfrom, string weekto)
		{

			Load_Volume_ResponseData responseMode = new Load_Volume_ResponseData();

			try
			{
				DataTable dt_sheet1 = new DataTable();
				dt_sheet1 = GetData(weekfrom, weekto, "sheet1");

				DataTable dt_Volume = new DataTable();
				dt_Volume = GetData(weekfrom, weekto, "Volume");

				DataTable dt_Workload = new DataTable();
				dt_Workload = GetData(weekfrom, weekto, "Workload");

				responseMode.sheet1 = dt_sheet1;
				responseMode.Volume = dt_Volume;
				responseMode.Workload = dt_Workload;


			}
			catch (Exception ex)
			{
				LogHelper.Error(ex.Message);
			}

			return responseMode;
		}

		public static DataTable GetData(string weekfrom, string weekto, string datatype)
		{
		

			selSQL = selSQL.Replace("weekfrom", weekfrom).Replace("weekto", weekto).ToString();

			string sql_each = "";
			string sql_Sum = "";

			//202217-202230
			//202247-202310
			if (datatype != "sheet1")
			{
				int fromNo = Convert.ToInt32(weekfrom.Substring(4, 2));
				int fromyear = Convert.ToInt32(weekfrom.Substring(0, 4));
				int toNo = Convert.ToInt32(weekto.Substring(4, 2));
				int toyear = Convert.ToInt32(weekto.Substring(0, 4));

				int weekcount = 0;

				sql_each = sql_each + "MAX(CASE Week WHEN '0' THEN[Booked FEU] ELSE 0 END) AS '0',";

				if (fromyear == toyear)
				{
					if (fromNo <= toNo)
					{
						for (int k = Convert.ToInt32(fromNo); k <= Convert.ToInt32(toNo); k++)
						{

							sql_each = sql_each + "MAX(CASE Week WHEN '" + k.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS '" + k.ToString() + "',";
							weekcount = weekcount + 1;
						}
					}
				}
				else
				{
					//fromyear < toyear

					if (fromNo <= 52)
					{
						for (int m = Convert.ToInt32(fromNo); m <= Convert.ToInt32("52"); m++)
						{
							sql_each = sql_each + "MAX(CASE Week WHEN '" + m.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS '" + m.ToString() + "',";
							weekcount = weekcount + 1;
						}
						for (int n = Convert.ToInt32("1"); n <= Convert.ToInt32(toNo); n++)
						{
							sql_each = sql_each + "MAX(CASE Week WHEN '" + n.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS 'n.ToString()',";
							weekcount = weekcount + 1;
						}
					}
				}

				
				if (datatype == "Carrier")
				{
					selSQL = " if object_id(N'Temcarrier',N'U') is not null drop table Temcarrier  select Carrier,[Week],Count(isnull([Booked FEU],0)) as [Booked FEU]  into Temcarrier from ( " + selSQL + "";

					selSQL = selSQL + @")b group by Carrier,[Week] order by Carrier asc
				SELECT
				  Carrier,sql_each 
				  SUM([Booked FEU]) AS[Total Booked FEU]
					FROM Temcarrier
					GROUP BY Carrier
					ORDER BY Carrier ASC
				drop table Temcarrier";


				}else if (datatype == "Account" || datatype=="Volume")
				{
					selSQL = " if object_id(N'TemPrincipal',N'U') is not null drop table TemPrincipal  select Principal,[Week],Count(isnull([Booked FEU],0)) as [Booked FEU]  into TemPrincipal from ( " + selSQL + "";

					selSQL = selSQL + @")b group by Principal,[Week] order by Principal asc
				SELECT
				 isnull(Principal,'') as Principal,sql_each 
				  SUM([Booked FEU]) AS[Total Booked FEU]
					FROM TemPrincipal
					GROUP BY Principal
					ORDER BY Principal ASC
				drop table TemPrincipal";

				}
				else if (datatype == "WorkLoad")
				{
					selSQL = " if object_id(N'TemWorkLoad',N'U') is not null drop table TemWorkLoad  select Principal,[Week],Count(isnull([Booked FEU],0)) as [Booked FEU]  into TemWorkLoad from ( " + selSQL + "";

					selSQL = selSQL + @")b group by Principal,[Week] order by Principal asc
				SELECT
				  isnull([Created By],'') as [Created By],sql_each 
				  SUM([Booked FEU]) AS[Total Booked FEU],  SUM([Booked FEU])/weekcount as [SHPTS PER WK]
					FROM TemWorkLoad
					GROUP BY [Created By]
					ORDER BY [Created By] ASC
				drop table TemWorkLoad";

				}

				selSQL = selSQL.Replace("sql_each", sql_each).Replace("weekcount", weekcount.ToString()).Replace("sql_Sum", sql_Sum.ToString());
			}

			DBHelper dbHelper = new DBHelper();

			DataTable dt = new DataTable();

			//dt = dbHelper.ExecDataTable(selSQL);

			DataSet ds = dbHelper.ExecuteDataset(CommandType.Text, selSQL, null);

			dt = ds.Tables[0];
            LogHelper.Debug("GetAMSFilingData => SQL :" + selSQL);

            return dt;
        }


		public static DataSet GetCarrierRptByBranchData(string weekfrom,string weekto)
        {


			string RunSQL = selSQL.Replace("weekfrom", weekfrom).Replace("weekto", weekto).ToString();

			string sql_each = "";
			string sql_Sum = "";
			//202217-202230
			//202247-202310
			if (weekfrom != "" && weekto!="")
			{
				int fromNo = Convert.ToInt32(weekfrom.Substring(4, 2));
				int fromyear = Convert.ToInt32(weekfrom.Substring(0, 4));
				int toNo = Convert.ToInt32(weekto.Substring(4, 2));
				int toyear = Convert.ToInt32(weekto.Substring(0, 4));

				int weekcount = 0;

				sql_each = sql_each + " MAX(CASE Week WHEN '0' THEN[Booked FEU] ELSE 0 END) AS '0',";
				sql_Sum = sql_Sum + " sum(cast([0] as int)) '0', ";

				if (fromyear == toyear)
				{
					if (fromNo <= toNo)
					{
						for (int k = Convert.ToInt32(fromNo); k <= Convert.ToInt32(toNo); k++)
						{

							sql_each = sql_each + " MAX(CASE Week WHEN '" + k.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS '" + k.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + k.ToString() + "] as int)) '" + k.ToString() + "', ";
							weekcount = weekcount + 1;
						}
					}
				}
				else
				{
					//fromyear < toyear

					if (fromNo <= 52)
					{
						for (int m = Convert.ToInt32(fromNo); m <= Convert.ToInt32("52"); m++)
						{
							sql_each = sql_each + "MAX(CASE Week WHEN '" + m.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS '" + m.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + m.ToString() + "] as int)) '" + m.ToString() + "', ";
							weekcount = weekcount + 1;
						}
						for (int n = Convert.ToInt32("1"); n <= Convert.ToInt32(toNo); n++)
						{
							sql_each = sql_each + "MAX(CASE Week WHEN '" + n.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS 'n.ToString()',";
							sql_Sum = sql_Sum + " sum(cast([" + n.ToString() + "] as int)) '" + n.ToString() + "', ";
							weekcount = weekcount + 1;
						}
					}
				}

				RunSQL = RunSQL + @" if object_id(N'Temcarrier',N'U') is not null drop table Temcarrier  select Carrier,[Week],Count(isnull([Booked FEU],0)) as [Booked FEU]  into Temcarrier from  tempsheet1 group by Carrier,[Week] order by Carrier asc";
				RunSQL = RunSQL + @" select case when Carrier is not null then Carrier else 'Total' end Carrier,
								sql_Sum
								sum(cast([Total Booked FEU] as int)) as [Total]
								from (
									SELECT
										Carrier,sql_each 
										SUM([Booked FEU]) AS[Total Booked FEU]
										FROM Temcarrier
										GROUP BY Carrier
								) as a group by Carrier with rollup										
								drop table Temcarrier";



				RunSQL = RunSQL + @" if object_id(N'TemPrincipal',N'U') is not null drop table TemPrincipal  select Principal,[Week],Count(isnull([Booked FEU],0)) as [Booked FEU]  into TemPrincipal from tempsheet1  group by Principal,[Week] order by Principal asc";
				RunSQL = RunSQL + @" select case when Principal is not null then Principal else 'Total' end Principal,
									sql_Sum
									sum(cast([Total Booked FEU] as int)) as [Total]
									from (
									SELECT
										isnull(Principal,'') as Principal,sql_each 
										SUM([Booked FEU]) AS[Total Booked FEU]
										FROM TemPrincipal
										GROUP BY Principal
									) as a group by Principal with rollup				
									drop table TemPrincipal";


				RunSQL = RunSQL.Replace("sql_each", sql_each).Replace("weekcount", weekcount.ToString()+".00").Replace("sql_Sum", sql_Sum.ToString());

				RunSQL = RunSQL + @" Select * from tempsheet1 ";
			}

			DBHelper dbHelper = new DBHelper();

			DataSet ds = dbHelper.ExecuteDataset(CommandType.Text, RunSQL, null);
			if (ds!= null && ds.Tables.Count == 3)
			{
				ds.Tables[0].TableName = "By Carrier";
				ds.Tables[1].TableName = "By Account";
				ds.Tables[2].TableName = "Booking Advice List";

			}
			return ds;
		}

		public static DataSet GetzVolumeRptByBranchData(string weekfrom, string weekto)
		{
			

			string RunSQL = selSQL.Replace("weekfrom", weekfrom).Replace("weekto", weekto).ToString();

			string sql_each = "";
			string sql_Sum = "";

			//202217-202230
			//202247-202310
			if (weekfrom != "" && weekto != "")
			{
				int fromNo = Convert.ToInt32(weekfrom.Substring(4, 2));
				int fromyear = Convert.ToInt32(weekfrom.Substring(0, 4));
				int toNo = Convert.ToInt32(weekto.Substring(4, 2));
				int toyear = Convert.ToInt32(weekto.Substring(0, 4));

				int weekcount = 0;

				sql_each = sql_each + " MAX(CASE Week WHEN '0' THEN[Booked FEU] ELSE 0 END) AS '0',";
				sql_Sum = sql_Sum + " sum(cast([0] as int)) '0', ";

				if (fromyear == toyear)
				{
					if (fromNo <= toNo)
					{
						for (int k = Convert.ToInt32(fromNo); k <= Convert.ToInt32(toNo); k++)
						{

							sql_each = sql_each + " MAX(CASE Week WHEN '" + k.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS '" + k.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + k.ToString() + "] as int)) '" + k.ToString() + "', ";
							weekcount = weekcount + 1;
						}
					}
				}
				else
				{
					//fromyear < toyear

					if (fromNo <= 52)
					{
						for (int m = Convert.ToInt32(fromNo); m <= Convert.ToInt32("52"); m++)
						{
							sql_each = sql_each + " MAX(CASE Week WHEN '" + m.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS '" + m.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + m.ToString() + "] as int)) '" + m.ToString() + "', ";
							weekcount = weekcount + 1;
						}
						for (int n = Convert.ToInt32("1"); n <= Convert.ToInt32(toNo); n++)
						{
							sql_each = sql_each + " MAX(CASE Week WHEN '" + n.ToString() + "' THEN[Booked FEU] ELSE 0 END) AS 'n.ToString()',";
							sql_Sum = sql_Sum + " sum(cast([" + n.ToString() + "] as int)) '" + n.ToString() + "', ";
							weekcount = weekcount + 1;
						}
					}
				}



				RunSQL = RunSQL + @" if object_id(N'VolumeRpt1',N'U') is not null drop table VolumeRpt1  
									select Principal,[Week],Count(isnull([Booked FEU],0)) as [Booked FEU]  into VolumeRpt1 
									from tempsheet1  group by Principal,[Week] order by Principal asc";

				RunSQL = RunSQL + @" select case when Principal is not null then Principal else 'Total' end Principal,
									sql_Sum
									sum(cast([Total Booked FEU] as int)) as [Total],'' as Reason ,'' as Detail
									from(
										SELECT
										isnull(Principal,'') as Principal,
										sql_each 
										SUM([Booked FEU]) AS[Total Booked FEU]
										FROM VolumeRpt1
										GROUP BY Principal 
									) as a group by Principal with rollup				
								    drop table VolumeRpt1";

				RunSQL = RunSQL + @" if object_id(N'VolumeRpt2', N'U') is not null drop table VolumeRpt2 
								select [Created By],[Week],Count(isnull([Booked FEU], 0)) as [Booked FEU]  into VolumeRpt2 
								from tempsheet1 where  1=1 and [Created By] like 'tbs%'  
								group by [Created By],[Week] order by [Created By] asc
								SELECT
								  isnull([Created By],'') as [CS],sql_each 
								  SUM([Booked FEU]) AS[Total Booked FEU], 
								  cast((SUM([Booked FEU])/weekcount) as decimal(15,2)) [SHPTS PER WK]
								  FROM VolumeRpt2 
								  GROUP BY [Created By]
								  ORDER BY [Created By] ASC
								drop table VolumeRpt2";

				RunSQL = RunSQL + @" if object_id(N'VolumeRpt3', N'U') is not null drop table VolumeRpt3
								select [Dest Office],[Week],Count(isnull([Booked FEU], 0)) as [Booked FEU] into VolumeRpt3 
								from tempsheet1  where 1=1  and [Created By] like 'tbs%'
								group by [Dest Office],[Week] order by [Dest Office] asc
								SELECT
								  isnull([Dest Office],'') as [Team],sql_each 
								  SUM([Booked FEU]) AS [Total Booked FEU],  
								  cast(SUM([Booked FEU])/weekcount/(select count(distinct [Created By])  from tempsheet1 T1 where T1.[Dest Office] = V3.[Dest Office]
								  and 1=1 and T1.[Created By] like 'tbs%') as decimal(15,2)) as [SHPTS PER WK]
								  FROM VolumeRpt3 V3
								  GROUP BY [Dest Office]
								  ORDER BY [Dest Office] ASC
								drop table VolumeRpt3";



                RunSQL = RunSQL.Replace("sql_each", sql_each).Replace("weekcount", weekcount.ToString()+".00").Replace("sql_Sum", sql_Sum.ToString()); ;

				RunSQL = RunSQL + @" Select * from tempsheet1  drop table tempsheet1 ";
			}

			DBHelper dbHelper = new DBHelper();

			DataSet ds = dbHelper.ExecuteDataset(CommandType.Text, RunSQL, null);
			if(ds != null && ds.Tables.Count==4)
			{
				ds.Tables[0].TableName = "Volume analysis";
				ds.Tables[1].TableName = "Work Load";
				ds.Tables[2].TableName = "Work Load By Origin";
				ds.Tables[3].TableName = "Booking Advice List";

			}
			return ds;
		}

	}
}