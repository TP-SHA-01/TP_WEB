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
	public class BookingAdviceAnalysisRpt_Service : IBookingAdviceAnalysisRpt
	{

		static string env = BaseCont.ENVIRONMENT;
		static EmailHelper emailHelper = new EmailHelper();

		//For testing : isnull(principal.Company,POTracing.CNEE)  && ModifiedBy,Principal

		static string selSQL = String.Format(@" if object_id(N'tempsheet',N'U') is not null drop table tempsheet  Select Top 20000 
											[Week] as WeekTemp, BRANCH,[Booking ID],[Booking Date],[Booking Status],
											[Transportation Mode],(case when [Created By]='tbs_HKG_kiny' then 'tbs_HKG_skiny' else [Created By] end) as [Created By],[Handling User Email],
											[Origin Office],[Dest Office], Principal, Consignee, Vendor,
											[Place Of Receipt],[Carrier Dest], isnull(Carrier,'') as Carrier,[Contract Type],
											[Contract No],[Carrier Commodity], Vessel, Voyage, MBL, HBL,
											[Delivery Type],[PO Ready Date],[POL],[Original ETD],[Current ETD],
											ATD,[POD],[Original ETA],[Current ETA],[Final Dest],[Final ETA],
											[Dest Ramp],[Booked 40],[Booked 20],[Booked HQ],[Booked 45],
											[Booked LCL],[Booked 53],
											(((a.[Booked 40] + a.[Booked HQ] + a.[Booked 45] + a.[Booked 53]) * 1) + (a.[Booked 20] * .5)) As[Booked FEU],
											[Approval Status],[Approval Date],[Approval Remark],[VGM Sent],
											[ISF Sent Date],[Customs Response #],[Forwarder Remarks],[Nomination Office],
											[Nomination Sales],[Container List],[Rolled],[Service String],[IsTBS] into tempsheet
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
												FROM POTracing WITH (NOLOCK)
												LEFT JOIN Customer principal ON principal.uID = POTracing.Principle
												LEFT JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice'
												AND agent.IsActive = 'Y' AND Criteria3 = 'Y' AND(Criteria5 <> '' OR Criteria5 IS NOT NULL)
												LEFT JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE
												LEFT JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser
												LEFT JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID
												OUTER APPLY(Select Top 1 tm.TRAFFIC From TrafficMap tm Join LocationMgr traffic On tm.Country_code = traffic.CountryCode Where traffic.Location = POTracing.FinalDest) tm
											    OUTER APPLY(Select Top 1 ModifiedBy From ModifyRecord m Where CargoId = POTracing.uID and m.modifiedBy <> '' Order By ActionDate asc) m 
												WHERE
												(POTracing.BookingContractType IN ('TOPOCEAN','NVO/NON-TOPOCEAN CONTRACT') Or POTracing.IsTBS = 'Y')
												And POTracing.CNEE Not In('ABC COMPUTER', 'ABC COMPUTER-TBS', 'ABC COMP EU')
												And ISNULL(POTracing.WB_Status, '') NOT IN('REJECTED','CANCELLED') AND POTracing.BookingReqID IS NOT NULL
												And(TopoceanYearAndWeek between weekfrom and weekto)   
												And(POTracing.BookingReqID <> '')
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
												POTracing.BookingInstruction, POTracing.ISTBS, POTracing.loadedOnMotherVesselFlag,POTracing.ModifiedBy,POTracing.CreatedBy  ) a
												
												 if object_id(N'tempsheet1',N'U') is not null drop table tempsheet1
												select case when (isnull(temp.Carrier,'')='' and isnull(temp.Vessel,'')='' and isnull(Voyage,'')='') then '0' else [WeekTemp] end as Week, 
												temp.* into tempsheet1 from tempsheet temp

												");

		static string SQLAirShipment = string.Format(@" if object_id(N'tempAirShipment',N'U') is not null drop table tempAirShipment 
													Select [Week],[BRANCH],[Booking Date],[Booking ID],[Transportation Mode],
													[Booking Status],[PO Ready Date],(case when [Created By]='tbs_HKG_kiny' then 'tbs_HKG_skiny' else [Created By] end) as [Created By],
													(case when len([Origin Office])=5 then substring([Origin Office],3,3) else [Origin Office] end) as [Origin Office],
													(case when len([Dest Office])=5 then substring([Dest Office],3,3) else [Dest Office] end) as [Dest Office],
													Principle,Consignee,Vendor,[Place Of Receipt],(Case when len(a.[FLIGHT#]) > 4 then SUBSTRING(a.[FLIGHT#],1,2) else  a.Airline end) as Airline,
													[Contract Type],
													[Trade Term],[Actual Commodity] ,(Case when len(a.[FLIGHT#])>4 then SUBSTRING(a.[FLIGHT#],3,len(a.[FLIGHT#])-2) else  a.[FLIGHT#] end) as [FLIGHT#],
													MBL,HBL,[Delivery Type],
													POL,ETD,ATD,POD,ETA,ATA,[Final Dest],[Dimension (CBM)],[WEIGHT],
													[CHARGABLE WEIGHT],[Forwarder Remarks] ,[Nomination Office],PieceCount,bookingreqid,BookingContractType,
													[Nomination Sales],Traffic,[TopoceanYearAndWeek] into tempAirShipment 
													From 
													( Select POtracing.PieceCount,POTracing.bookingreqid,POTracing.BookingContractType,
													POTracing.WB_WeekOfYear as [Week],[dbo].[fun_WBIBrhMapping_v5](POTracing.OriginOffice, 
													POTracing.FinalDest, POTracing.TransportationMode) as [BRANCH],POTracing.BookingDate as [Booking Date],
													'' + POTracing.BookingReqID + '' as [Booking ID],ISNULL(POTracing.TransportationMode, '') as [Transportation Mode],
													UPPER(POTracing.WB_STATUS) as [Booking Status],POTracing.POReadyDate As [PO Ready Date],
													ISNULL(m.ModifiedBy, '') as [Created By],ISNULL(POTracing.LoadPort, '') as [Origin Office],
													ISNULL(POTracing.DischPort, '') as [Dest Office],Principle.Company as [Principle],POTracing.CNEE as [Consignee],
													POTracing.Vendor as [Vendor],POTracing.orig AS [Place of Receipt],POTracing.Carrier as [Airline],
													ISNULL(POTracing.BookingContractType, '') As [Contract Type],ISNULL(Term, '') as [Trade Term],
													POTracing.Description As [Actual Commodity],POTracing.Voyage as [FLIGHT#], POTracing.MBL as [MBL],
													POTracing.HBL as [HBL],POTracing.DeliveryType as [Delivery Type],POTracing.LoadPort as [POL],
													POTracing.P2PETD As [ETD],POTracing.P2PATD As [ATD],POTracing.DischPort as [POD],
													POTracing.P2PETA As [ETA],POTracing.P2PATA As [ATA],POTracing.FinalDest as [Final Dest],
													ROUND(SUM(ISNULL(WB_CNTR.CBM, 0)), 4) as [Dimension (CBM)],ROUND(SUM(ISNULL(WB_CNTR.Weight, 0)), 4) as [WEIGHT],
													ROUND(SUM(ISNULL(WB_CNTR.ChargeableWeight, 0)), 4) as [CHARGABLE WEIGHT],POTracing.BookingInstruction As [Forwarder Remarks],
													ISNULL(Principle.NomName, '') as [Nomination Office],ISNULL(Principle.NomSales, '') as [Nomination Sales],
													POTracing.Traffic,POTracing.[TopoceanYearAndWeek] FROM POTracing  WITH (NOLOCK)         
													LEFT JOIN WB_CNTR  ON WB_CNTR.BookingReqID = POTracing.BookingReqID 
													LEFT JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice' 
													AND agent.IsActive ='Y' AND Criteria3 ='Y' AND (Criteria5 <> '' OR Criteria5 IS NOT NULL) 
													LEFT JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE 
													LEFT JOIN Customer Principle ON Principle.uID = POTracing.Principle 
													OUTER APPLY 
													(Select Top 1 ModifiedBy From ModifyRecord m  WITH (NOLOCK)         
													Where CargoId = POTracing.uID and m.modifiedBy <> '' Order By ActionDate asc) m 
													WHERE 

													POTracing.CNEE Not In ('ABC COMPUTER','ABC COMPUTER-TBS','ABC COMP EU') 
													And ISNULL(POTracing.WB_Status, '') NOT IN ('REJECTED','CANCELLED') 
													And ISNULL(TransportationMode, '') IN ('AIR','PARCEL') 
													AND POTracing.BookingReqID IS NOT NULL 
													and (TopoceanYearAndWeek between strTopoceanYearAndWeek_from and strTopoceanYearAndWeek_to) 
													and (POTracing.BookingReqID<>'') 

													GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, 
													POTracing.OriginOffice, POTracing.Dest,Principle.Company, 
													POTracing.CNEE, POTracing.Vendor, Principle.NomName, 
													Principle.NomSales, POTracing.BookingContractType,
													POTracing.BookingDate, POTracing.BookingReqID, 
													POTracing.WB_STATUS, POTracing.Orig, POTracing.DeliveryType,
													POTracing.Carrier, POTracing.MBL, POTracing.HBL, POTracing.LoadPort, 
													POTracing.POReadyDate, POTracing.P2PETD, POTracing.P2PATD, 
													POTracing.DischPort, POTracing.P2PETA, POTracing.P2PATA, 
													POTracing.Description, POTracing.Traffic, POTracing.Voyage, 
													POTracing.BookingInstruction, POTracing.FinalDest,
													POTracing.Term,POTracing.TransportationMode,m.ModifiedBy,[TopoceanYearAndWeek],POtracing.PieceCount,
													POTracing.BookingContractType,POTracing.bookingreqid	)
													a
													");

		////Add rule: CS没有Input Carrier（Q),Vessel(U),Voyage(V) ,自动Week为0

		public IEnumerable<string> GetValue()
		{
			return new List<string>() { "hi", "PO" };
		}

		public static Load_SPRC_ResponseData GetBookingAdviceByBranchData(string weekfrom, string weekto)
		{

			Load_SPRC_ResponseData responseMode = new Load_SPRC_ResponseData();

			try
			{
				DataTable dt_sheet1 = new DataTable();
				dt_sheet1 = GetData(weekfrom, weekto, "sheet1");

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
					selSQL = " if object_id(N'Temcarrier',N'U') is not null drop table Temcarrier  select Carrier,[Week],SUM(isnull([Booked FEU],0)) as [Booked FEU]  into Temcarrier from ( " + selSQL + "";

					selSQL = selSQL + @")b group by Carrier,[Week] order by Carrier asc
				SELECT
				  Carrier,sql_each 
				  SUM(isnull([Booked FEU],0)) AS[Total Booked FEU]
					FROM Temcarrier
					GROUP BY Carrier
					ORDER BY Carrier ASC
				drop table Temcarrier";


				}
				else if (datatype == "Account" || datatype == "Volume")
				{
					selSQL = " if object_id(N'TemPrincipal',N'U') is not null drop table TemPrincipal  select Principal,[Week],SUM(isnull([Booked FEU],0)) as [Booked FEU]  into TemPrincipal from ( " + selSQL + "";

					selSQL = selSQL + @")b group by Principal,[Week] order by Principal asc
				SELECT
				 isnull(Principal,'') as Principal,sql_each 
				  SUM(isnull([Booked FEU],0)) AS[Total Booked FEU]
					FROM TemPrincipal
					GROUP BY Principal
					ORDER BY Principal ASC
				drop table TemPrincipal";

				}
				else if (datatype == "WorkLoad")
				{
					selSQL = " if object_id(N'TemWorkLoad',N'U') is not null drop table TemWorkLoad  select Principal,[Week],SUM(isnull([Booked FEU],0)) as [Booked FEU]  into TemWorkLoad from ( " + selSQL + "";

					selSQL = selSQL + @")b group by Principal,[Week] order by Principal asc
				SELECT
				  isnull([Created By],'') as [Created By],sql_each 
				  SUM(isnull([Booked FEU],0)) AS[Total Booked FEU],  SUM(isnull([Booked FEU],0))/weekcount as [SHPTS PER WK]
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


		public static DataSet GetCarrierRptByBranchData(string weekfrom, string weekto)
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

				sql_each = sql_each + " MAX(CASE Week WHEN '0' THEN [Booked FEU] ELSE 0 END) AS '0',";
				sql_Sum = sql_Sum + " sum(cast([0] as money)) '0', ";

				if (fromyear == toyear)
				{
					if (fromNo <= toNo)
					{
						for (int k = Convert.ToInt32(fromNo); k <= Convert.ToInt32(toNo); k++)
						{

							sql_each = sql_each + " MAX(CASE Week WHEN '" + k.ToString() + "' THEN [Booked FEU] ELSE 0 END) AS '" + k.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + k.ToString() + "] as money)) '" + k.ToString() + "', ";
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
							sql_each = sql_each + "MAX(CASE Week WHEN '" + m.ToString() + "' THEN [Booked FEU] ELSE 0 END) AS '" + m.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + m.ToString() + "] as money)) '" + m.ToString() + "', ";
							weekcount = weekcount + 1;
						}
						for (int n = Convert.ToInt32("1"); n <= Convert.ToInt32(toNo); n++)
						{
							sql_each = sql_each + "MAX(CASE Week WHEN '" + n.ToString() + "' THEN [Booked FEU] ELSE 0 END) AS 'n.ToString()',";
							sql_Sum = sql_Sum + " sum(cast([" + n.ToString() + "] as money)) '" + n.ToString() + "', ";
							weekcount = weekcount + 1;
						}
					}
				}

				RunSQL = RunSQL + @" if object_id(N'Temcarrier',N'U') is not null drop table Temcarrier  
									 select Carrier,[Week],
									 SUM(isnull([Booked FEU],0)) as [Booked FEU]  into Temcarrier 
									 from  tempsheet1 group by Carrier,[Week] order by Carrier asc";
				RunSQL = RunSQL + @" select case when Carrier is not null then Carrier else 'Total' end Carrier,
								sql_Sum
								sum(cast([Total Booked FEU] as money)) as [Total]
								from (
									SELECT
										Carrier,sql_each 
										SUM(isnull([Booked FEU],0)) AS[Total Booked FEU]
										FROM Temcarrier
										GROUP BY Carrier
								) as a group by Carrier with rollup										
								drop table Temcarrier";



				RunSQL = RunSQL + @" if object_id(N'TemPrincipal',N'U') is not null drop table TemPrincipal  
									 select Principal,[Week],
									 SUM(isnull([Booked FEU],0)) as [Booked FEU]  into TemPrincipal 
									 from tempsheet1  group by Principal,[Week] order by Principal asc";
				RunSQL = RunSQL + @" select case when Principal is not null then Principal else 'Total' end Principal,
									sql_Sum
									sum(cast([Total Booked FEU] as money)) as [Total]
									from (
									SELECT
										isnull(Principal,'') as Principal,sql_each 
										SUM(isnull([Booked FEU],0)) AS[Total Booked FEU]
										FROM TemPrincipal
										GROUP BY Principal
									) as a group by Principal with rollup				
									drop table TemPrincipal";


				RunSQL = RunSQL.Replace("sql_each", sql_each).Replace("weekcount", weekcount.ToString() + ".00").Replace("sql_Sum", sql_Sum.ToString());

				RunSQL = RunSQL + @" Select * from tempsheet1 ";
			}

			DBHelper dbHelper = new DBHelper();

			DataSet ds = dbHelper.ExecuteDataset(CommandType.Text, RunSQL, null);
			if (ds != null && ds.Tables.Count == 3)
			{
				ds.Tables[0].TableName = "By Carrier";
				ds.Tables[1].TableName = "By Account";
				ds.Tables[2].TableName = "Booking Advice List";

			}
			return ds;
		}

		public static DataSet GetVolumeRptByBranchData(string weekfrom, string weekto)
		{

			string RunSQL = selSQL.Replace("weekfrom", weekfrom).Replace("weekto", weekto).ToString();

			string sql_each = "";
			string sql_Sum = "";
			string sql_Team = "";

			//LoadTeamNumber from webconfig
			string Workload_LAX = ConfigurationManager.AppSettings["Workload_LAX"];
			string Workload_NYC = ConfigurationManager.AppSettings["Workload_NYC"];
			string Workload_ORD = ConfigurationManager.AppSettings["Workload_ORD"];

			string Workload_All = ConfigurationManager.AppSettings["Workload_All"];

			//202217-202230
			//202247-202310
			if (weekfrom != "" && weekto != "")
			{
				int fromNo = Convert.ToInt32(weekfrom.Substring(4, 2));
				int fromyear = Convert.ToInt32(weekfrom.Substring(0, 4));
				int toNo = Convert.ToInt32(weekto.Substring(4, 2));
				int toyear = Convert.ToInt32(weekto.Substring(0, 4));

				int weekcount = 0;
				//SUM([0]) as '0',SUM([25]) as '25',SUM([26]) as '26',SUM([27]) as '27',SUM([28]) as '28'
				//Team Need

				sql_each = sql_each + " MAX(CASE Week WHEN '0' THEN [Booked FEU] ELSE 0 END) AS '0',";
				sql_Sum = sql_Sum + " sum(cast([0] as money)) '0', ";

				sql_Team = sql_Team + " SUM([0]) as '0', ";

				if (fromyear == toyear)
				{
					if (fromNo <= toNo)
					{
						for (int k = Convert.ToInt32(fromNo); k <= Convert.ToInt32(toNo); k++)
						{
							sql_each = sql_each + " MAX(CASE Week WHEN '" + k.ToString() + "' THEN [Booked FEU] ELSE 0 END) AS '" + k.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + k.ToString() + "] as money)) '" + k.ToString() + "', ";
							sql_Team = sql_Team + "SUM([" + k.ToString() + "]) as '" + k.ToString() + "',";
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
							sql_each = sql_each + " MAX(CASE Week WHEN '" + m.ToString() + "' THEN [Booked FEU] ELSE 0 END) AS '" + m.ToString() + "',";
							sql_Sum = sql_Sum + " sum(cast([" + m.ToString() + "] as money)) '" + m.ToString() + "', ";
							sql_Team = sql_Team + "SUM([" + m.ToString() + "]) as '" + m.ToString() + "',";
							weekcount = weekcount + 1;
						}
						for (int n = Convert.ToInt32("1"); n <= Convert.ToInt32(toNo); n++)
						{
							sql_each = sql_each + " MAX(CASE Week WHEN '" + n.ToString() + "' THEN [Booked FEU] ELSE 0 END) AS 'n.ToString()',";
							sql_Sum = sql_Sum + " sum(cast([" + n.ToString() + "] as money)) '" + n.ToString() + "', ";
							sql_Team = sql_Team + "SUM([" + n.ToString() + "]) as '" + n.ToString() + "',";
							weekcount = weekcount + 1;
						}
					}
				}



				RunSQL = RunSQL + @" if object_id(N'VolumeRpt1',N'U') is not null drop table VolumeRpt1  
									select Principal,[Week],
									SUM (isnull([Booked FEU],0)) as [Booked FEU]  into VolumeRpt1 
									from tempsheet1  group by Principal,[Week] order by Principal asc";

				RunSQL = RunSQL + @" select case when Principal is not null then Principal else 'Total' end Principal,
									sql_Sum
									sum(cast([Total Booked FEU] as money)) as [Total],'' as Reason ,'' as Detail
									from(
										SELECT
										isnull(Principal,'') as Principal,
										sql_each 
										SUM(isnull([Booked FEU],0)) AS[Total Booked FEU]
										FROM VolumeRpt1
										GROUP BY Principal 
									) as a group by Principal with rollup				
								    drop table VolumeRpt1";

				RunSQL = RunSQL + @" if object_id(N'VolumeRpt2', N'U') is not null drop table VolumeRpt2 
								select [Created By],[Week],
								count ([week])  as [Booked FEU]  into VolumeRpt2 
								from tempsheet1 where  1=1 and [Created By] like 'tbs%'  
								group by [Created By],[Week] order by [Created By] asc

								if object_id(N'CSTemp', N'U') is not null drop table CSTemp

								SELECT
								  isnull([Created By],'') as [CS],sql_each 
								  SUM(isnull([Booked FEU],0)) AS[Total], 
								  cast((SUM(isnull([Booked FEU],0))/weekcount) as decimal(15,2)) [SHPTS PER WK] into CSTemp
								  FROM VolumeRpt2 where [Created By] in Workload_All
								  GROUP BY [Created By]
								  ORDER BY [Created By] ASC

								drop table VolumeRpt2
							
								";

				RunSQL = RunSQL + @" select * from CSTemp

								select 'LAX' as Team ,sql_Team SUM([Total]) as Total,cast((SUM([Total])/4.00/7.00) as decimal(15,2)) as [SHPTS PER WK]
								from CSTemp where CS in Workload_LAX
								union
								select 'NYC' as Team ,sql_Team SUM([Total]) as Total,cast((SUM([Total])/4.00/3.00) as decimal(15,2)) as [SHPTS PER WK]
								from CSTemp where CS in Workload_NYC
								union
								select 'ORD/SFO' as Team ,sql_Team SUM([Total]) as Total,cast((SUM([Total])/4.00/2.00) as decimal(15,2)) as [SHPTS PER WK]
								from CSTemp where CS in Workload_ORD

								if object_id(N'CSTemp', N'U') is not null drop table CSTemp";




				RunSQL = RunSQL.Replace("sql_each", sql_each).Replace("weekcount", weekcount.ToString() + ".00").Replace("sql_Sum", sql_Sum.ToString());

				RunSQL = RunSQL.Replace("sql_Team", sql_Team);

				//Workload_LAX
				RunSQL = RunSQL.Replace("Workload_All", Workload_All);
				RunSQL = RunSQL.Replace("Workload_LAX", Workload_LAX);
				RunSQL = RunSQL.Replace("Workload_NYC", Workload_NYC);
				RunSQL = RunSQL.Replace("Workload_ORD", Workload_ORD);

				RunSQL = RunSQL + @" Select * from tempsheet1  drop table tempsheet1 ";
			}

			DBHelper dbHelper = new DBHelper();

			DataSet ds = dbHelper.ExecuteDataset(CommandType.Text, RunSQL, null);

			if (ds != null && ds.Tables.Count == 4)
			{
				ds.Tables[0].TableName = "Volume analysis";
				ds.Tables[1].TableName = "Work Load";
				ds.Tables[2].TableName = "Work Load By Origin";
				ds.Tables[3].TableName = "Booking Advice List";

			}
			// else
			// {
			//	ds.Tables.Add("Error");
			//	ds.Tables[0].Columns.Add("SQL", Type.GetType("System.String"));
			//	ds.Tables[0].Rows.Add(new object[] { RunSQL });
			//}
			return ds;
		}

		public static DataSet GetAirShipmentReport(string weekfrom, string weekto)
		{

			DataSet dt = new DataSet();
			//注意，这里的周数是月份
			//202201 指的是一月  202201-202204

			//202105 - 202205 应该是202105-202112  + 202201-202205

			string TPList = ConfigurationManager.AppSettings["TPList"];
			string NONList = ConfigurationManager.AppSettings["NONList"];

			if (weekfrom != "" && weekto != "")
			{

				int fromNo = Convert.ToInt32(weekfrom.Substring(4, 2));
				int toNo = Convert.ToInt32(weekto.Substring(4, 2));

				string strFromno = "";
				string strToNo = "";

				switch (fromNo)
				{
					case 1:
						strFromno = "01";
						break;
					case 2:
						strFromno = "06";
						break;
					case 3:
						strFromno = "10";
						break;
					case 4:
						strFromno = "14";
						break;
					case 5:
						strFromno = "19";
						break;
					case 6:
						strFromno = "23";
						break;
					case 7:
						strFromno = "27";
						break;
					case 8:
						strFromno = "32";
						break;
					case 9:
						strFromno = "36";
						break;
					case 10:
						strFromno = "40";
						break;
					case 11:
						strFromno = "45";
						break;
					case 12:
						strFromno = "49";
						break;
					default:
						break;
				}

				switch (toNo)
				{
					case 1:
						strToNo = "05";
						break;
					case 2:
						strToNo = "09";
						break;
					case 3:
						strToNo = "13";
						break;
					case 4:
						strToNo = "18";
						break;
					case 5:
						strToNo = "22";
						break;
					case 6:
						strToNo = "26";
						break;
					case 7:
						strToNo = "31";
						break;
					case 8:
						strToNo = "35";
						break;
					case 9:
						strToNo = "39";
						break;
					case 10:
						strToNo = "44";
						break;
					case 11:
						strToNo = "48";
						break;
					case 12:
						strToNo = "52";
						break;
					default:
						break;
				}

				int fromyear = Convert.ToInt32(weekfrom.Substring(0, 4));
				int toyear = Convert.ToInt32(weekto.Substring(0, 4));

				string strTopoceanYearAndWeek_from = fromyear.ToString() + strFromno.ToString();
				string strTopoceanYearAndWeek_to = toyear.ToString() + strToNo.ToString();

				SQLAirShipment = SQLAirShipment.Replace("strTopoceanYearAndWeek_from", strTopoceanYearAndWeek_from).Replace("strTopoceanYearAndWeek_to", strTopoceanYearAndWeek_to);

				string strSQLByMonth = "";
				int TableCount = 0;

				//202005 - 202205
				for (int n = fromyear; n <= toyear; n++)  //循环年份  2020(05-12) 2021(01-12)... 2022(01-05)
				{
					//2020
					if (fromyear < toyear)
					{
						if (n == fromyear && n < toyear)
						{
							for (int m = fromNo; m <= 12; m++)
							{
								#region Make SQL 
								string TempFromno = GetMonthWeekFrom(m);
								string TempToNo = GetMonthWeekTo(m);

								string eachFrom = fromyear + TempFromno;
								string eachTo = toyear + TempToNo;

								strSQLByMonth = strSQLByMonth + GetSQLByMonth(eachFrom, eachTo);
								TableCount = TableCount + 1;
								#endregion
							}
						}
						else if (n > fromyear && n < toyear)
						{
							for (int m = 1; m <= 12; m++)
							{

								#region Make SQL 
								string TempFromno = GetMonthWeekFrom(m);
								string TempToNo = GetMonthWeekTo(m);

								string eachFrom = fromyear + TempFromno;
								string eachTo = toyear + TempToNo;

								strSQLByMonth = strSQLByMonth + GetSQLByMonth(eachFrom, eachTo);
								TableCount = TableCount + 1;
								#endregion
							}
						}
						else if (n > fromyear && n == toyear)
						{
							for (int m = 1; m <= toNo; m++)
							{

								#region Make SQL 
								string TempFromno = GetMonthWeekFrom(m);
								string TempToNo = GetMonthWeekTo(m);

								string eachFrom = fromyear + TempFromno;
								string eachTo = toyear + TempToNo;

								strSQLByMonth = strSQLByMonth + GetSQLByMonth(eachFrom, eachTo);
								TableCount = TableCount + 1;
								#endregion
							}
						}
					}
					else if (fromyear == toyear)
					{
						for (int m = fromNo; m <= toNo; m++)
						{

							#region Make SQL 
							string TempFromno = GetMonthWeekFrom(m);
							string TempToNo = GetMonthWeekTo(m);

							string eachFrom = fromyear + TempFromno;
							string eachTo = toyear + TempToNo;

							strSQLByMonth = strSQLByMonth + GetSQLByMonth(eachFrom, eachTo);
							TableCount = TableCount + 1;
							#endregion
						}
					}
				}

				strSQLByMonth = strSQLByMonth.Replace("TPList", TPList).Replace("NONList", NONList);

				DBHelper dbHelper = new DBHelper();

				DataSet ds = dbHelper.ExecuteDataset(CommandType.Text, SQLAirShipment + strSQLByMonth, null);

				if (ds != null && ds.Tables.Count > 0)
				{
					int t = 0;

					for (int n = fromyear; n <= toyear; n++)  //循环年份  2020(05-12) 2021(01-12)... 2022(01-05)
					{
						//2020
						//TableCount 7 + 12 + 5
						if (fromyear < toyear)
						{
							if (n == fromyear && n < toyear)
							{
								for (int m = fromNo; m <= 12; m++)
								{
									#region Make SQL 
									string strName = GetTableNameByMonth(m);
									ds.Tables[t].TableName = n.ToString() + "(" + strName + ")";
									t = t + 1;
									#endregion
								}
							}
							else if (n > fromyear && n < toyear)
							{
								for (int m = 1; m <= 12; m++)
								{
									#region Make SQL 
									string strName = GetTableNameByMonth(m);
									ds.Tables[t].TableName = n.ToString() + "(" + strName + ")";
									t = t + 1;
									#endregion
								}
							}
							else if (n > fromyear && n == toyear)
							{
								for (int m = 1; m <= toNo; m++)
								{
									#region Make SQL 
									string strName = GetTableNameByMonth(m);
									ds.Tables[t].TableName = n.ToString() + "(" + strName + ")";
									t = t + 1;
									#endregion
								}
							}
						}
						else if (fromyear == toyear)
						{
							for (int m = fromNo; m <= toNo; m++)
							{
								#region Make SQL 
								string strName = GetTableNameByMonth(m);
								ds.Tables[t].TableName = n.ToString() + "(" + strName + ")";
								t = t + 1;
								#endregion
							}

						}
					}
				}
				return ds;
			}

			return dt;
		}

		public static string GetColoader(string strType)
		{
			string ColoaderList = ConfigurationManager.AppSettings[strType];

			return ColoaderList;
		}

		public static string GetMonthWeekFrom(int month)
		{
			int m = month;
			string TempFromno = "";
			switch (m)
			{
				case 1:
					TempFromno = "01";
					break;
				case 2:
					TempFromno = "06";
					break;
				case 3:
					TempFromno = "10";
					break;
				case 4:
					TempFromno = "14";
					break;
				case 5:
					TempFromno = "19";
					break;
				case 6:
					TempFromno = "23";
					break;
				case 7:
					TempFromno = "27";
					break;
				case 8:
					TempFromno = "32";
					break;
				case 9:
					TempFromno = "36";
					break;
				case 10:
					TempFromno = "40";
					break;
				case 11:
					TempFromno = "45";
					break;
				case 12:
					TempFromno = "49";
					break;
				default:
					break;
			}

			return TempFromno;
		}
		public static string GetMonthWeekTo(int month)
		{
			int m = month;
			string TempToNo = "";
			switch (m)
			{
				case 1:
					TempToNo = "05";
					break;
				case 2:
					TempToNo = "09";
					break;
				case 3:
					TempToNo = "13";
					break;
				case 4:
					TempToNo = "18";
					break;
				case 5:
					TempToNo = "22";
					break;
				case 6:
					TempToNo = "26";
					break;
				case 7:
					TempToNo = "31";
					break;
				case 8:
					TempToNo = "35";
					break;
				case 9:
					TempToNo = "39";
					break;
				case 10:
					TempToNo = "44";
					break;
				case 11:
					TempToNo = "48";
					break;
				case 12:
					TempToNo = "52";
					break;
				default:
					break;
			}

			return TempToNo;
		}

		public static string GetTableNameByMonth(int month)
		{
			string tablename = "";
			//JAN(WK1 - 5)

			switch (month)
			{
				case 1:
					tablename = "JAN(WK1-5)";
					break;
				case 2:
					tablename = "FEB(WK6-9)";
					break;
				case 3:
					tablename = "MAR(WK10-13)";
					break;
				case 4:
					tablename = "APR(WK14-18)";
					break;
				case 5:
					tablename = "MAY(WK19-22)";
					break;
				case 6:
					tablename = "JUN(WK23-26)";
					break;
				case 7:
					tablename = "JUL(WK27-31)";
					break;
				case 8:
					tablename = "AUG(WK32-35)";
					break;
				case 9:
					tablename = "SEP(WK36-39)";
					break;
				case 10:
					tablename = "OCT(WK40-44)";
					break;
				case 11:
					tablename = "NOV(WK45-48)";
					break;
				case 12:
					tablename = "DEC(WK49-52)";
					break;
				default:
					break;
			}

			return tablename;
		}

		public static string GetSQLByMonth(string WeekFrom,string WeekTo)
        {
			string strSQLByMonth = "";

			strSQLByMonth = @" Select  row_number()over(order by HBL)  as [Item No.],
											  ( case when ('/'+TBS.[Dest Office]+'/') in TPList then 'TP' 
													when ('/'+TBS.[Dest Office]+'/') in NONList then 'NON-TP' else '' end )
													as [TP/Non TP],
													TBS.Vendor as [Shipper],
													TBS.Consignee as [Cnee],
													TBS.bookingreqid as [BookingID],
													TBS.MBL as [Mawb],
													TBS.HBL as [Hawb],
													TBS.BookingContractType as [Contract Type],
													TBS.[Origin Office] as [Origin],
													TBS.[Dest Office] as [Dest.],
													TBS.Airline as [Airline],
													TBS.FLIGHT# as [Flt No.],
													TBS.PieceCount  as [Package],
													TBS.WEIGHT as [G.W.],
													TBS.[CHARGABLE WEIGHT] as [C.W.],
													TBS.[Dimension (CBM)] as [Volume],
													
													case when len(TBS.[Forwarder Remarks])>13 and  CHARINDEX('##COLOADER',TBS.[Forwarder Remarks])>0  then
													replace(replace(substring(TBS.[Forwarder Remarks],charindex('##',(TBS.[Forwarder Remarks]))+2,
													len(TBS.[Forwarder Remarks])-charindex('##',reverse(TBS.[Forwarder Remarks]))-charindex('##',(TBS.[Forwarder Remarks]))-2)
													,'COLOADER:',''),' ','') else '' end as [Co-loader],
													
													TBS.ETD as [ETD],
													TBS.ATD as [ATD],
													TBS.ETA as [ETA],
													TBS.ATA as [ATA],
													TBS.[Forwarder Remarks] as [Remark],
													''  as [Result],
													''  as [Carrier Direct]

													from tempAirShipment TBS ";

			strSQLByMonth = strSQLByMonth + @" where TBS.TopoceanYearAndWeek between 'eachFrom' and 'eachTo'";

			strSQLByMonth = strSQLByMonth.Replace("eachFrom", WeekFrom).Replace("eachTo", WeekTo);

			strSQLByMonth = strSQLByMonth + @"   ";

			return strSQLByMonth;
		}
	}
}