using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TB_WEB.CommonLibrary.Encrypt;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.CommonEndpoint.Impl;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace LevelReport
{
    public partial class MainFrm : Form
    {
        private string reportTypeSelectItemKey { get; set; }
        private string reportTypeSelectItemValue { get; set; }
        private string statusSelectItemValue { get; set; }
        private string carrierSelectItemValue { get; set; }
        private string trafficSelectItemValue { get; set; }
        private string bookingType { get; set; }

        DBHelper dbHelper = new DBHelper();

        public MainFrm()
        {
            InitializeComponent();

            this.bookingType = "TP";

            InitControl();
        }

        private void InitControl()
        {
            InitCombox_Carrier();
            InitCombox_Traffic();
            InitCombox_BookingStatus();
            InitCombox_ReportType();
        }

        private void InitCombox_Carrier()
        {
            cmb_Carrier.MultiSelect = true;
            cmb_Carrier.ColumnHeaderVisible = true;
            cmb_Carrier.RowHeaderVisible = false;
            cmb_Carrier.MaxDropDownItems = 10;

            GetCombox_Carrier();
        }

        private void InitCombox_Traffic()
        {
            cmb_Traffic.MultiSelect = true;
            cmb_Traffic.ColumnHeaderVisible = true;
            cmb_Traffic.RowHeaderVisible = false;
            cmb_Traffic.MaxDropDownItems = 10;

            GetCombox_Traffic();
        }

        private void InitCombox_BookingStatus()
        {
            List<string> list = new List<string>();
            list.Add("NEW");
            list.Add("PENDING");
            list.Add("CONFIRMED");
            list.Add("REVISED");
            list.Add("REJECTED");
            list.Add("CANCELLED");

            cmb_Status.MultiSelect = true;
            cmb_Status.ColumnHeaderVisible = false;
            cmb_Status.BandSource(list);
        }

        private void InitCombox_ReportType()
        {
            ArrayList list = new ArrayList();

            //list.Add(new DictionaryEntry("", "Default Format"));
            list.Add(new DictionaryEntry("Carrier Container Level Report", "908"));
            list.Add(new DictionaryEntry("(NON-TP) Booking Report", "847"));
            list.Add(new DictionaryEntry("(NON-TP) Loading Report", "908"));

            cmb_ReportType.DataSource = list;
            cmb_ReportType.DisplayMember = "Key";
            cmb_ReportType.ValueMember = "Value";

            //cmb_ReportType.Items.AddRange(new object[] { "Default Format" });
            //cmb_ReportType.Items.AddRange(new object[] { "TBS Booking Advice (Branch)" }); // 847
            //cmb_ReportType.Items.AddRange(new object[] { "Container Level Report" }); //908
        }

        private void GetCombox_Carrier()
        {
            AuthenticationModel authenticationModel = new AuthenticationModel(new AuthenticationEntity() { usr = BaseCont.usr, psd = new Decryption().DecryptDBConnectionString(BaseCont.psd) });

            Hashtable paramQuery = new Hashtable();
            //paramQuery.Add("type", "Traffic");
            paramQuery.Add("is_active", "Y");

            CommonEndpoint_Imp endpoint_Imp = new CommonEndpoint_Imp();
            endpoint_Imp.init(new EDIPostEntity() { queryParams = paramQuery, body = null, apiUrl = BaseCont.ApiUrl + "/carrier" }, authenticationModel);
            //endpoint_Imp.init(new EDIPostEntity() { queryParams = paramQuery, body = null, apiUrl = BaseCont.ApiUrl + "/option" }, authenticationModel);
            endpoint_Imp.Get();
            dynamic jsonObj = JsonConvert.DeserializeObject(endpoint_Imp.repJson);
            Dictionary<string, string> dic = new Dictionary<string, string>();


            var tb = new DataTable();
            tb.Columns.Add(new DataColumn("ID") { Caption = "ID" });
            tb.Columns.Add(new DataColumn("Name") { Caption = "Name" });
            tb.Columns.Add(new DataColumn("Value") { Caption = "Value" });
            int id = 1;

            foreach (var item in jsonObj["payload"]["records"])
            {
                var row = tb.NewRow();
                //string optValue = item["value"].ToString();
                //string optCriteria1 = item["criteria1"].ToString();

                string optValue = CommonUnit.CheckEmpty(item["name"]);
                string optCriteria1 = CommonUnit.CheckEmpty(item["display_name"]);

                if (!String.IsNullOrEmpty(optValue))
                {
                    row["ID"] = id++;
                    row["Name"] = optValue;
                    row["Value"] = optCriteria1;

                    tb.Rows.Add(row);
                }
            }

            var views = new List<string>() { "Name", "Value" };
            cmb_Carrier.ValueMember = "Name";
            cmb_Carrier.DisplayMember = "Value";
            //this.op_Record = endpoint_Imp.LoadBaseResponse<Res_Base_VO<Res_PlayLoad_VO<Res_Record_VO>, GetRecordModel>>();

            cmb_Carrier.BandSource(tb.DefaultView.ToTable(true, new string[] { "Name", "Value" }), views);
        }

        private void GetCombox_Traffic()
        {
            AuthenticationModel authenticationModel = new AuthenticationModel(new AuthenticationEntity() { usr = BaseCont.usr, psd = new Decryption().DecryptDBConnectionString(BaseCont.psd) });

            Hashtable paramQuery = new Hashtable();
            paramQuery.Add("type", "Traffic");
            paramQuery.Add("is_active", "Y");

            CommonEndpoint_Imp endpoint_Imp = new CommonEndpoint_Imp();
            //endpoint_Imp.init(new EDIPostEntity() { queryParams = paramQuery, body = null, apiUrl = BaseCont.ApiUrl + "/carrier" }, authenticationModel);
            endpoint_Imp.init(new EDIPostEntity() { queryParams = paramQuery, body = null, apiUrl = BaseCont.ApiUrl + "/option" }, authenticationModel);
            endpoint_Imp.Get();
            dynamic jsonObj = JsonConvert.DeserializeObject(endpoint_Imp.repJson);
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var tb = new DataTable();
            tb.Columns.Add(new DataColumn("ID") { Caption = "ID" });
            tb.Columns.Add(new DataColumn("Name") { Caption = "Name" });
            tb.Columns.Add(new DataColumn("Value") { Caption = "Value" });
            int id = 0;

            List<string> nonUsaList = new List<string>() { "EUR", "CHN", "SEA", "NEA", "AUS", "CAN", "SAM", "ISC", "AFR", "MDE" };

            var rowNONUSA = tb.NewRow();
            rowNONUSA["ID"] = id++;
            rowNONUSA["Name"] = "NON-USA";
            rowNONUSA["Value"] = "NONUSA";
            tb.Rows.Add(rowNONUSA);

            foreach (var item in jsonObj["payload"]["records"])
            {
                var row = tb.NewRow();
                //string optValue = item["value"].ToString();
                //string optCriteria1 = item["criteria1"].ToString();

                string optValue = CommonUnit.CheckEmpty(item["value"]);
                string optCriteria1 = CommonUnit.CheckEmpty(item["criteria1"]);

                if (!String.IsNullOrEmpty(optValue))
                {
                    row["ID"] = id++;
                    row["Name"] = optValue;
                    row["Value"] = optCriteria1;

                    tb.Rows.Add(row);
                }
            }



            var views = new List<string>() { "Name", "Value" };
            cmb_Traffic.ValueMember = "Name";
            cmb_Traffic.DisplayMember = "Value";
            //this.op_Record = endpoint_Imp.LoadBaseResponse<Res_Base_VO<Res_PlayLoad_VO<Res_Record_VO>, GetRecordModel>>();

            cmb_Traffic.BandSource(tb.DefaultView.ToTable(true, new string[] { "Name", "Value" }), views);
        }

        private void cmb_Carrier_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cmb_Status_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cmb_Traffic_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cmb_ReportType_SelectedValueChanged(object sender, EventArgs e)
        {
            var keyValue = cmb_ReportType.SelectedItem;


            this.reportTypeSelectItemKey = CommonUnit.CheckEmpty(((DictionaryEntry)cmb_ReportType.SelectedItem).Key);
            this.reportTypeSelectItemValue = CommonUnit.CheckEmpty(((DictionaryEntry)cmb_ReportType.SelectedItem).Value);

            if (reportTypeSelectItemKey.Contains("(NON-TP) Booking Report") || reportTypeSelectItemKey.Contains("(NON-TP) Loading Report"))
            {
                this.bookingType = "NON-TP";
            }
            else
            {
                this.bookingType = "TP";
            }

        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            try
            {
                this.btn_Create.Enabled = false;

                string retError = CheckValue();

                if (!String.IsNullOrEmpty(retError))
                {
                    MessageBox.Show(retError);
                    return;
                }

                ExportExcel(GetData());
            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
            finally
            {
                this.btn_Create.Enabled = true;
            }
        }

        private void ExportExcel(DataTable dt)
        {
            try
            {
                if (dt.Rows.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    //设置文件标题
                    saveFileDialog.Title = "Export Excel File";
                    //设置文件类型
                    saveFileDialog.Filter = "Excel WorkSheet (*.xlsx)|*.xlsx|Excel 97-2003 WorkSheet(*.xls)|*.xls";
                    //设置默认文件类型显示顺序  
                    saveFileDialog.FilterIndex = 1;
                    //是否自动在文件名中添加扩展名
                    saveFileDialog.AddExtension = true;
                    //是否记忆上次打开的目录
                    saveFileDialog.RestoreDirectory = true;
                    //设置默认文件名
                    saveFileDialog.FileName = reportTypeSelectItemKey + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond.ToString();

                    //按下确定选择的按钮  
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string localFilePath = saveFileDialog.FileName.ToString();


                        NPOIHelper.ExportExcel(dt, reportTypeSelectItemKey, localFilePath, "MM/dd/yyyy");

                        if (MessageBox.Show("Export Success，Open the File？", "Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(localFilePath);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error Msg: No Data ");
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }

        }

        private string CheckValue()
        {
            bool ret = true;
            string errMsg = String.Empty;

            DateTime etdFromValue = this.dt_ETDFrom.Value;
            DateTime etdToValue = this.dt_ETDTo.Value;

            if (DateTime.Compare(etdFromValue, etdToValue) > 0)
            {
                errMsg += " ETD DateTime ETDFrom > ETDTo \n";
            }

            if (cmb_Carrier.SelectedValues.Count <= 0)
            {
                errMsg += " should select carrier \n";
            }

            if (cmb_Traffic.SelectedValues.Count <= 0)
            {
                errMsg += " should select traffic \n";
            }

            if (cmb_Status.SelectedValues.Count <= 0)
            {
                errMsg += " should select status \n";
            }

            return errMsg;
        }

        private DataTable GetData()
        {
            DataTable dt = new DataTable();
            string sql = String.Empty;
            string sqlSelect = String.Empty;
            string sqlWhere = String.Empty;
            string carrierList = String.Join("','", cmb_Carrier.SelectedValues);
            string trafficList = String.Join("','", cmb_Traffic.SelectedValues);
            if (cmb_Traffic.SelectedItem.Equals("NON-USA"))
            {
                trafficList = "EUR','CHN','SEA','NEA','AUS','CAN','SAM','ISC','AFR','MDE";
            }

            string statusList = String.Join("','", cmb_Status.SelectedValues);

            string etdFromValue = this.dt_ETDFrom.Value.ToString("MM/dd/yyyy");
            string etdToValue = this.dt_ETDTo.Value.ToString("MM/dd/yyyy");

            this.reportTypeSelectItemKey = CommonUnit.CheckEmpty(((DictionaryEntry)cmb_ReportType.SelectedItem).Key);
            this.reportTypeSelectItemValue = CommonUnit.CheckEmpty(((DictionaryEntry)cmb_ReportType.SelectedItem).Value);

            if (reportTypeSelectItemValue.Equals("847"))
            {
                sqlWhere = String.Empty;

                if (!String.IsNullOrEmpty(carrierList))
                {
                    sqlWhere += String.Format(" AND POTracing.Carrier In ('-1',{0}) ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(carrierList)));
                }

                if (!String.IsNullOrEmpty(trafficList))
                {
                    sqlWhere += String.Format(" AND POTracing.Traffic In ('-1',{0}) ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(trafficList)));
                }

                if (!String.IsNullOrEmpty(statusList))
                {
                    sqlWhere += String.Format(" AND WB_Status In ('-1',{0})  ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(statusList)));
                }


                sql = String.Format(" Select ROW_NUMBER() OVER (ORDER BY Week)  AS [Seq.] ," +
                                "        [Week],                       " +
                                "        tb_week._MONTH AS [Month],           " +
                                "        BRANCH,                              " +
                                "        [Booking ID],                        " +
                                "        [Booking Date],                      " +
                                "        [Booking Status],                    " +
                                "        [Transportation Mode],               " +
                                "        [Created By],                        " +
                                "        [Handling User Email],               " +
                                "        [Origin Office],                     " +
                                "        [Dest Office],                       " +
                                "        Principal,                           " +
                                "        Consignee,                           " +
                                "        Vendor,                              " +
                                "        [Place Of Receipt],                  " +
                                "        [Carrier Dest],                      " +
                                "        Carrier,                             " +
                                "        [Contract Type],                     " +
                                "        [Contract No],                       " +
                                "        [Carrier Commodity],                 " +
                                "        Vessel,                              " +
                                "        Voyage,                              " +
                                "        MBL,                                 " +
                                "        HBL,                                 " +
                                "        [Delivery Type],                     " +
                                "        [PO Ready Date],                     " +
                                "        [POL],                               " +
                                "        [POL Country],                       " +
                                "        [Original ETD],                      " +
                                "        [Current ETD],                       " +
                                "        ATD,                                 " +
                                "        [POD],                               " +
                                "        [POD Country],                       " +
                                "        [Original ETA],                      " +
                                "        [Current ETA],                       " +
                                "        [Final Dest],                        " +
                                "        [Final ETA],                         " +
                                "        [Dest Ramp],                         " +
                                "        [Booked 40],                         " +
                                "        [Booked 20],                         " +
                                "        [Booked HQ],                         " +
                                "        [Booked 45],                         " +
                                "        [Booked LCL],                        " +
                                "        [Booked 53],                         " +
                                "        (((a.[Booked 40] + a.[Booked HQ] + a.[Booked 45] + a.[Booked 53]) * 1) +               " +
                                "         (a.[Booked 20] * .5)) As [Booked FEU],                                                " +
                                "        [Approval Status],                                                                     " +
                                "        [Approval Date],                                                                       " +
                                "        [Approval Remark],                                                                     " +
                                "        [VGM Sent],                                                                            " +
                                "        [ISF Sent Date],                                                                       " +
                                "        [Customs Response #],                                                                  " +
                                "        [Forwarder Remarks],                                                                   " +
                                "        [Nomination Office],                                                                   " +
                                "        [Nomination Sales],                                                                    " +
                                "        [Container List],                                                                      " +
                                "        [Rolled],                                                                              " +
                                "        [Service String],                                                                      " +
                                "        [IsTBS]                                                                                " +
                                " From (Select CASE                                                                             " +
                                "                  WHEN POTracing.ISTBS = 'Y' THEN ISNULL(POTracing.WB_WeekOfYear, 0)           " +
                                "                  ELSE [dbo].[udf_GetTBSWeek](POTracing.OriginalVesselETD) END                                                     as [Week],                     " +
                                "              [dbo].[fun_WBIBrhMapping_v5](POTracing.OriginOffice, POTracing.FinalDest,                                                                           " +
                                "                                           POTracing.TransportationMode)                                                           as [BRANCH],                   " +
                                "              POTracing.BookingDate                                                                                                as [Booking Date],             " +
                                "              '' + POTracing.BookingReqID + ''                                                                                     as [Booking ID],               " +
                                "              UPPER(POTracing.WB_STATUS)                                                                                           as [Booking Status],           " +
                                "              POTracing.TransportationMode                                                                                         As [Transportation Mode],      " +
                                "              ISNULL(m.ModifiedBy, '')                                                                                             as [Created By],               " +
                                "              ISNULL(max(lv.UserEmail), '')                                                                                        as [Handling User Email],      " +
                                "              ISNULL(POTracing.OriginOffice, '')                                                                                   as [Origin Office],            " +
                                "              ISNULL(POTracing.Dest, '')                                                                                           as [Dest Office],              " +
                                "              principal.Company                                                                                                    as [Principal],                " +
                                "              POTracing.CNEE                                                                                                       as [Consignee],                " +
                                "              POTracing.Vendor                                                                                                     as [Vendor],                   " +
                                "              POTracing.Orig                                                                                                       as [Place Of Receipt],         " +
                                "              (case                                                                                                                                               " +
                                "                   when POTracing.CarrierDestType = 'PORT' then dbo.fun_ValidPortStrByType(POTracing.CarrierDest, 'ALL')                                          " +
                                "                   else POTracing.CarrierDest end)                                                                                 as [Carrier Dest],             " +
                                "              POTracing.Carrier                                                                                                    as [Carrier],                  " +
                                "              ISNULL(POTracing.BookingContractType, '')                                                                            As [Contract Type],            " +
                                "              ISNULL(contract.ContractNo, '')                                                                                      as [Contract No],              " +
                                "              POTracing.Commodity2                                                                                                 As [Carrier Commodity],        " +
                                "              POTracing.Vessel                                                                                                     as [Vessel],                   " +
                                "              POTracing.Voyage                                                                                                     as [Voyage],                   " +
                                "              POTracing.MBL                                                                                                        as [MBL],                      " +
                                "              POTracing.HBL                                                                                                        as [HBL],                      " +
                                "              POTracing.DeliveryType                                                                                               as [Delivery Type],            " +
                                "              POTracing.POReadyDate                                                                                                As [PO Ready Date],            " +
                                "              loadPort.PortName                                                                                                    as [POL],                      " +
                                "              loadPort.Country                                                                                                     as [POL Country],              " +
                                "              POTracing.OriginalP2PETD                                                                                             As [Original ETD],             " +
                                "              POTracing.P2PETD                                                                                                     As [Current ETD],              " +
                                "              POTracing.P2PATD                                                                                                     As [ATD],                      " +
                                //"              POTracing.DischPort                                                                                                  as [POD],                      " +
                                "              dischPort.PortName                                                                                                   AS [POD] ," +
                                "              dischPort.Country                                                                                                    AS [POD Country], " +
                                "              POTracing.OriginalP2PETA                                                                                             As [Original ETA],             " +
                                "              POTracing.P2PETA                                                                                                     As [Current ETA],              " +
                                "              POTracing.FinalDest                                                                                                  as [Final Dest],               " +
                                "              POTracing.D2DETA                                                                                                     As [Final ETA],                " +
                                "              POTracing.DestRamp                                                                                                   as [Dest Ramp],                " +
                                "              (CASE                                                                                                                                               " +
                                "                   WHEN POTracing.cntrType In                                                                                                                     " +
                                "                        ('40''TKC', '40''SD', '40RF', '40''PLWD', '40''PLF', '40''OPT', '40''GOH', '40''FLR')                                                     " +
                                "                       THEN NULLIF(POTracing.cntrqty, 0)                                                                                                          " +
                                "                   ELSE 0 End) + (CASE                                                                                                                            " +
                                "                                      WHEN POTracing.cntrType2 In                                                                                                 " +
                                "                                           ('40''TKC', '40''SD', '40RF', '40''PLWD', '40''PLF', '40''OPT', '40''GOH',                                             " +
                                "                                            '40''FLR') THEN NULLIF(POTracing.cntrqty2, 0)                                                                         " +
                                "                                      ELSE 0 End) + (CASE                                                                                                         " +
                                "                                                         WHEN POTracing.cntrType3 In                                                                              " +
                                "                                                              ('40''TKC', '40''SD', '40RF', '40''PLWD', '40''PLF',                                                " +
                                "                                                               '40''OPT', '40''GOH', '40''FLR')                                                                   " +
                                "                                                             THEN NULLIF(POTracing.cntrqty3, 0)                                                                   " +
                                "                                                         ELSE 0 End) + (CASE                                                                                      " +
                                "                                                                            WHEN POTracing.cntrType4 In                                                           " +
                                "                                                                                 ('40''TKC', '40''SD', '40RF',                                                    " +
                                "                                                                                  '40''PLWD', '40''PLF', '40''OPT',                                               " +
                                "                                                                                  '40''GOH', '40''FLR')                                                           " +
                                "                                                                                THEN NULLIF(POTracing.cntrqty4, 0)                                                " +
                                "                                                                            ELSE 0 End)                                            as [Booked 40],                " +
                                "              (CASE                                                                                                                                               " +
                                "                   WHEN POTracing.cntrType In ('20''TKC', '20''SD', '20RF', '20''PLF', '20''OPT', '20''NOR', '20''GOH')                                           " +
                                "                       THEN NULLIF(POTracing.cntrqty, 0)                                                                                                          " +
                                "                   ELSE 0 End) + (CASE                                                                                                                            " +
                                "                                      WHEN POTracing.cntrType2 In                                                                                                 " +
                                "                                           ('20''TKC', '20''SD', '20RF', '20''PLF', '20''OPT', '20''NOR', '20''GOH')                                              " +
                                "                                          THEN NULLIF(POTracing.cntrqty2, 0)                                                                                      " +
                                "                                      ELSE 0 End) + (CASE                                                                                                         " +
                                "                                                         WHEN POTracing.cntrType3 In                                                                              " +
                                "                                                              ('20''TKC', '20''SD', '20RF', '20''PLF', '20''OPT',                                                 " +
                                "                                                               '20''NOR', '20''GOH') THEN NULLIF(POTracing.cntrqty3, 0)                                           " +
                                "                                                         ELSE 0 End) + (CASE                                                                                      " +
                                "                                                                            WHEN POTracing.cntrType4 In                                                           " +
                                "                                                                                 ('20''TKC', '20''SD', '20RF', '20''PLF',                                         " +
                                "                                                                                  '20''OPT', '20''NOR', '20''GOH')                                                " +
                                "                                                                                THEN NULLIF(POTracing.cntrqty4, 0)                                                " +
                                "                                                                            ELSE 0 End)                                            as [Booked 20],                " +
                                "              (CASE WHEN POTracing.cntrType In ('HQ''NOR', '40HQ') THEN NULLIF(POTracing.cntrqty, 0) ELSE 0 End) +                                                " +
                                "              (CASE WHEN POTracing.cntrType2 In ('HQ''NOR', '40HQ') THEN NULLIF(POTracing.cntrqty2, 0) ELSE 0 End) +                                              " +
                                "              (CASE WHEN POTracing.cntrType3 In ('HQ''NOR', '40HQ') THEN NULLIF(POTracing.cntrqty3, 0) ELSE 0 End) +                                              " +
                                "              (CASE                                                                                                                                               " +
                                "                   WHEN POTracing.cntrType4 In ('HQ''NOR', '40HQ') THEN NULLIF(POTracing.cntrqty4, 0)                                                             " +
                                "                   ELSE 0 End)                                                                                                     as [Booked HQ],                " +
                                "              (CASE WHEN POTracing.cntrType In ('45RF', '45HQ') THEN NULLIF(POTracing.cntrqty, 0) ELSE 0 End) +                                                   " +
                                "              (CASE WHEN POTracing.cntrType2 In ('45RF', '45HQ') THEN NULLIF(POTracing.cntrqty2, 0) ELSE 0 End) +                                                 " +
                                "              (CASE WHEN POTracing.cntrType3 In ('45RF', '45HQ') THEN NULLIF(POTracing.cntrqty3, 0) ELSE 0 End) + (CASE                                           " +
                                "                                                                                    WHEN POTracing.cntrType4 In ('45RF', '45HQ')                                  " +
                                "                                                                                                                           THEN NULLIF(POTracing.cntrqty4, 0)     " +
                                "                                                                                                                       ELSE 0 End) as [Booked 45],                " +
                                "              (CASE WHEN POTracing.cntrType In ('LCL') THEN NULLIF(POTracing.cntrqty, 0) ELSE 0 End) +                                                            " +
                                "              (CASE WHEN POTracing.cntrType2 In ('LCL') THEN NULLIF(POTracing.cntrqty2, 0) ELSE 0 End) +                                                          " +
                                "              (CASE WHEN POTracing.cntrType3 In ('LCL') THEN NULLIF(POTracing.cntrqty3, 0) ELSE 0 End) + (CASE                                                    " +
                                "                                                                                                              WHEN POTracing.cntrType4 In ('LCL')                 " +
                                "                                                                                                                  THEN NULLIF(POTracing.cntrqty4, 0)              " +
                                "                                                                                                              ELSE 0 End)          as [Booked LCL],               " +
                                "              (CASE WHEN POTracing.cntrType In ('53HQ') THEN NULLIF(POTracing.cntrqty, 0) ELSE 0 End) +                                                           " +
                                "              (CASE WHEN POTracing.cntrType2 In ('53HQ') THEN NULLIF(POTracing.cntrqty2, 0) ELSE 0 End) +                                                         " +
                                "              (CASE WHEN POTracing.cntrType3 In ('53HQ') THEN NULLIF(POTracing.cntrqty3, 0) ELSE 0 End) + (CASE                                                   " +
                                "                                                                                                               WHEN POTracing.cntrType4 In ('53HQ')               " +
                                "                                                                                                                   THEN NULLIF(POTracing.cntrqty4, 0)             " +
                                "                                                                                                               ELSE 0 End)         as [Booked 53],                " +
                                "              POTracing.WBApprovalStatus                                                                                           As [Approval Status],          " +
                                "              POTracing.WBApprovalDate                                                                                             As [Approval Date],            " +
                                "              POTracing.WBApprovalRemark                                                                                           As [Approval Remark],          " +
                                "              ''                                                                                                                   As [VGM Sent],                 " +
                                "              POTracing.ISFSentDate                                                                                                As [ISF Sent Date],            " +
                                "              POTracing.CustomsResponseNo                                                                                          As [Customs Response #],       " +
                                "              POTracing.BookingInstruction                                                                                         As [Forwarder Remarks],        " +
                                "              ISNULL(principal.NomName, '')                                                                                        as [Nomination Office],        " +
                                "              ISNULL(principal.NomSales, '')                                                                                       as [Nomination Sales],         " +
                                "              [dbo].[udf_GetContainerListByBookingReqID](POTracing.BookingReqID)                                                   as [Container List],           " +
                                "              (case                                                                                                                                               " +
                                "                   when ((select count(*)                                                                                                                         " +
                                "                          from TBS_Route                                                                                                                          " +
                                "                          where WBIID = POTracing.BookingReqID                                                                                                    " +
                                "                            and TBS_Route.IsActive = 1) > 1 and                                                                                                   " +
                                "                         POTracing.loadedOnMotherVesselFlag = 'Y') then 'Y'                                                                                       " +
                                "                   else 'N' end)                                                                                                   as [Rolled],                   " +
                                "              [dbo].[udf_GetServiceStringByID](POTracing.BookingReqID)                                                             as [Service String],           " +
                                "              ISNULL(POTracing.IsTBS, 'N')                                                                                         as [IsTBS]                     " +
                                "       FROM POTracing                                                                                                                                             " +
                                "                LEFT JOIN Port loadPort ON POTracing.LoadPort = loadPort.PortAbbr      " +
                                "                LEFT JOIN Port dischPort ON POTracing.DischPort = dischPort.PortAbbr   " +
                                "                LEFT JOIN Customer principal ON principal.uID = POTracing.Principle                                                                               " +
                                "                LEFT JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice' AND                                               " +
                                "                                              agent.IsActive = 'Y' AND Criteria3 = 'Y' AND                                                                        " +
                                "                                              (Criteria5 <> '' OR Criteria5 IS NOT NULL)                                                                          " +
                                "                LEFT JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE                                                                        " +
                                "                LEFT JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser                                                                               " +
                                "                LEFT JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID                                                          " +
                                "                OUTER APPLY (Select Top 1 tm.TRAFFIC                                                                                                              " +
                                "                             From TrafficMap tm                                                                                                                   " +
                                "                                      Join LocationMgr traffic On tm.Country_code = traffic.CountryCode                                                           " +
                                "                             Where traffic.Location = POTracing.FinalDest) tm                                                                                     " +
                                "                OUTER APPLY (Select Top 1 ModifiedBy                                                                                                              " +
                                "                             From ModifyRecord m                                                                                                                  " +
                                "                             Where CargoId = POTracing.uID                                                                                                        " +
                                "                               and m.modifiedBy <> ''                                                                                                             " +
                                "                             Order By ActionDate asc) m                                                                                                           " +
                                "       WHERE (POTracing.BookingContractType IN ('TOPOCEAN', 'NVO/NON-TOPOCEAN CONTRACT') Or POTracing.IsTBS = 'Y')                                                " +
                                "         And POTracing.CNEE Not In ('ABC COMPUTER', 'ABC COMPUTER-TBS', 'ABC COMP EU')                                                                            " +
                                "         And ISNULL(POTracing.WB_Status, '') NOT IN ('REJECTED', 'CANCELLED')                                                                                     " +
                                "         AND POTracing.BookingReqID IS NOT NULL                                                                                                                   " +
                                sqlWhere +
                                //"         and POTracing.Traffic In ('-1',{0})                                                                                                                           " +
                                "         AND (POTracing.P2PETD between {0} and {1})                                                                                                               " +
                                //"         AND POTracing.Carrier In ('-1',{1})                                                                                                                           " +
                                //"         AND WB_Status In ('-1',{2})                                                                                                                                   " +
                                "         and (POTracing.BookingReqID <> '')                                                                                                                       " +
                                "       GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo, POTracing.CNTRType,                                                          " +
                                "                POTracing.CNTRType2, POTracing.CNTRType3, POTracing.CNTRType4, POTracing.CNTRQty, POTracing.CNTRQty2,                                             " +
                                "                POTracing.CNTRQty3, POTracing.CNTRQty4, POTracing.TransportationMode, POTracing.OriginOffice,                                                     " +
                                "                POTracing.Dest, POTracing.CNEE, POTracing.Vendor, m.ModifiedBy, agent.Criteria5, principal.Company,                                               " +
                                "                principal.NomName, principal.NomSales, POTracing.BookingContractType, contract.ContractNo,                                                        " +
                                "                POTracing.BookingConfirmation, POTracing.BookingDate, POTracing.BookingReqID, POTracing.WB_STATUS,                                                " +
                                "                POTracing.DeliveryType, POTracing.Carrier, POTracing.MBL, POTracing.HBL, POTracing.Vessel,                                                        " +
                                "                POTracing.Voyage, POTracing.CarrierDestType, POTracing.CarrierDest, POTracing.LoadPort,                                                           " +
                                "                POTracing.POReadyDate, POTracing.P2PETD, POTracing.OriginalVesselETD, POTracing.OriginalP2PETD,                                                   " +
                                "                POTracing.P2PATD, POTracing.DischPort, POTracing.P2PETA, POTracing.D2DETA, POTracing.OriginalP2PETA,                                              " +
                                "                POTracing.DestRamp, POTracing.FinalDest, POTracing.Description, POTracing.Commodity2, POTracing.Orig,                                             " +
                                "                POTracing.WBApprovalStatus, POTracing.WBApprovalDate, POTracing.WBApprovalRemark, POTracing.ISFSentDate,                                          " +
                                "                POTracing.Comments, POTracing.BookingInstruction, POTracing.ISTBS, POTracing.loadedOnMotherVesselFlag" +
                                "               ,loadPort.PortName" +
                                "               , loadPort.Country" +
                                "               , dischPort.PortName" +
                                "               , dischPort.Country) a "
                                , CommonUnit.retDBStr(CommonUnit.CheckEmpty(etdFromValue))
                                , CommonUnit.retDBStr(CommonUnit.CheckEmpty(etdToValue))

                                );

                sqlSelect = sql + " LEFT JOIN ( " + RetGetMonthSQL() + " ) tb_week ON tb_week._WEEK = a.Week";
            }
            else if (reportTypeSelectItemValue.Equals("908"))
            {
                sqlWhere = String.Empty;
                if (!String.IsNullOrEmpty(carrierList))
                {
                    sqlWhere += String.Format(" AND Carrier In ('-1',{0}) ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(carrierList)));
                }

                if (!String.IsNullOrEmpty(trafficList))
                {
                    sqlWhere += String.Format(" AND Traffic In ('-1',{0}) ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(trafficList)));
                }

                if (!String.IsNullOrEmpty(statusList))
                {
                    sqlWhere += String.Format(" AND WB_Status In ('-1',{0})  ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(statusList)));
                }

                sql = String.Format(" Select MAX(CNEE) AS [CNEE],                               " +
                                "        Vendor,                                                " +
                                "        WB_Status,                                             " +
                                "        TransportationMode,                                    " +
                                "        BookingReqID,                                          " +
                                "        OriginOffice,                                          " +
                                "        Dest,                                                  " +
                                "        principle,                                             " +
                                "        Orig,                                                  " +
                                "        CarrierDest,                                           " +
                                "        Carrier,                                               " +
                                "        Commodity2,                                            " +
                                "        Commodity,                                             " +
                                "        BookingContractType,                                   " +
                                "        CNEECarrierContractNo,                                 " +
                                "        Vessel,                                                " +
                                "        Voyage,                                                " +
                                "        MBL,                                                   " +
                                "        DeliveryType,                                          " +
                                "        RcvdDate,                                              " +
                                "        LoadPort,                                              " +
                                "        OriginalP2PETD,                                        " +
                                "        P2PETD,                                                " +
                                "        P2PATD,                                                " +
                                "        DischPort,                                             " +
                                "        OriginalP2PETA,                                        " +
                                "        P2PETA,                                                " +
                                "        DestRamp,                                              " +
                                "        ShipmentType,                                          " +
                                "        ContainerNo,                                           " +
                                "        ContainerType,                                         " +
                                "        NomName,                                               " +
                                "        NomSales,                                              " +
                                "        Term,                                                  " +
                                "        HBL,                                                   " +
                                "        Traffic,                                               " +
                                "        BookingConfirmation,                                   " +
                                "        DrayageCompany,                                        " +
                                "        FinalDest,                                             " +
                                "        D2DApt,                                                " +
                                "        D2DETA,                                                " +
                                "        D2DRvsdETA,                                            " +
                                "        D2DATA,                                                " +
                                "        D2DPOD,                                                " +
                                "        D2DMTYNotify,                                          " +
                                "        D2DMTYPickup,                                          " +
                                "        DATEADD(dd, DATEDIFF(dd, 0, d2dmtyret), 0) AS [D2DMTYRET],                                                          " +
                                "        DeliveryNumber,                                                                                                     " +
                                "        TopoceanYearAndWeek,                                                                                                " +
                                "        CASE WHEN ISTBS = 'Y' THEN ISNULL(WB_WeekOfYear, 0) ELSE [dbo].[udf_GetTBSWeek](P2PETD) END as [Week],              " +
                                "        BookingCreatedBy,                                                                                                   " +
                                "        UserEmail                                                                                                           " +
                                " From WBIView                                                                                                               " +
                                "          Left Join PrincipleView On WBIView.principleId = PrincipleView.principle                                          " +
                                "          Left Join LoginView On WBIView.HandlingUser = LoginView.UserLoginName                                             " +
                                " Where (1 = 1)                                                                                                              " + sqlWhere +

                                //"   and Traffic In ('-1',{0})                                                                                                " +
                                "   AND (P2PETD between {0} and {1})                                                                                         " +
                                //"   AND Carrier In ('-1',{1})                                                                                                " +
                                //"   AND WB_Status In ('-1',{2})                                                                                              " +
                                "   AND (BookingReqID <> '')                                                                                                 " +
                                "   AND cnee Not In ('ABC COMPUTER', 'ABC COMP EU')                                                                          " +
                                "   AND WB_Status <> 'CANCELLED'                                                                                             " +
                                "   AND (containerNo Is Null Or (containerNo Not Like 'TBA%' and containerNo Not Like '2CTL%'))                              " +
                                "   AND TransportationMode = 'SEA'                                                                                           " +
                                " Group By Vendor, WB_Status, TransportationMode, BookingReqID, OriginOffice, Dest, principle, Orig, CarrierDest, Carrier,   " +
                                "          Commodity2, Commodity, BookingContractType, CNEECarrierContractNo, Vessel, Voyage, MBL, DeliveryType, RcvdDate,   " +
                                "          LoadPort, OriginalP2PETD, P2PETD, P2PATD, DischPort, OriginalP2PETA, P2PETA, DestRamp, ShipmentType,              " +
                                "          ContainerNo, ContainerType, NomName, NomSales, Term, HBL, Traffic, BookingConfirmation, DrayageCompany,           " +
                                "          FinalDest, D2DApt, D2DETA, D2DRvsdETA, D2DATA, D2DPOD, D2DMTYNotify, D2DMTYPickup, D2DMTYRet, DeliveryNumber,     " +
                                "          TopoceanYearAndWeek, BookingCreatedBy, UserEmail, ISTBS, WB_WeekOfYear"
                                , CommonUnit.retDBStr(CommonUnit.CheckEmpty(etdFromValue))
                                , CommonUnit.retDBStr(CommonUnit.CheckEmpty(etdToValue))
                                );

                sqlSelect = " WITH T AS (" + sql + ") " + RetContainerLevelReport_SelectSQL();
            }

            dt = dbHelper.ExecDataTable(sqlSelect);

            return dt;
        }

        private string RetGetMonthSQL()
        {
            string sql = " SELECT DATEPART(YY, D)                         _YEAR,                              " +
                         "        DATEPART(WK, D)                         _WEEK,                              " +
                         "        LEFT(UPPER(MIN(DATENAME(MONTH, D))), 3) _MONTH,                             " +
                         "        CONVERT(VARCHAR, MIN(D), 120)           _S_Date,                            " +
                         "        CONVERT(VARCHAR, MAX(D), 120)           _E_Date                             " +
                         "  FROM (SELECT DATEADD(D, NUMBER, DATEADD(YEAR, DATEDIFF(YEAR, 0, getdate()), 0)) D " +
                         "          FROM MASTER.DBO.SPT_VALUES                                                " +
                         "         WHERE [TYPE] = 'P'                                                         " +
                         "           AND NUMBER < DATEDIFF(D, DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()), 0)  " +
                         " 		     ,DATEADD(YY, 1, DATEADD(YEAR, DATEDIFF(YEAR, 0, GETDATE()), 0)))) T      " +
                         " GROUP BY DATEPART(YY, D),DATEPART(WK, D)                                           ";

            return sql;

        }

        private string RetContainerLevelReport_SelectSQL()
        {

            string sqlSelect_POL = String.Empty;
            string sqlSelect_POD = String.Empty;
            string sqlSelect_YearAndWeek = String.Empty;
            string sqlSelect_Year = String.Empty;
            string sqlSelect_Week = String.Empty;
            string sqlSelect_MONTH = String.Empty;
            string sqlSelect_FK_LAT = String.Empty;
            string sqlSelectNONTP_TEU = String.Empty;
            string sqlSelectTP_TEU = String.Empty;
            string sqlJoin = String.Empty;

            if (bookingType.Equals("TP"))
            {
                sqlSelect_Year = " , LEFT(TopoceanYearAndWeek, 4)                        AS [Year]";
                sqlSelect_Week = " , [Week]";

                sqlSelect_POL = "      , CASE                                                                                                            " +
                                "            WHEN UPPER(loadPort.Country) IN ('THAILAND', 'VIETNAM', 'MALAYSIA', 'SINGAPORE', 'INDONESIA') THEN 'SEA'    " +
                                "            WHEN UPPER(loadPort.Country) IN ('CHINA', 'TAIWAN', 'KOREA') THEN 'CN'                                      " +
                                "            ELSE 'NO' END                                   AS [POL Region]                                             ";

                sqlSelect_POD = "      , CASE                                                                                                            " +
                                "            WHEN UPPER(dischPort.Country) IN ('THAILAND', 'VIETNAM', 'MALAYSIA', 'SINGAPORE', 'INDONESIA') THEN 'SEA'   " +
                                "            WHEN UPPER(dischPort.Country) IN ('CHINA', 'TAIWAN', 'KOREA') THEN 'CN'                                     " +
                                "            ELSE 'NO' END                                   AS [POD Region]                                             ";

                sqlSelect_FK_LAT = "      , CASE                                                                                                            " +
                                   "            WHEN CHARINDEX('Fixed Deal', CNEECarrierContractNo) > 0 OR CHARINDEX('LTA', CNEECarrierContractNo) > 0      " +
                                   "                THEN 'LTA'                                                                                              " +
                                   "            WHEN CNEECarrierContractNo IS NULL THEN ''                                                                  " +
                                   "            ELSE 'FAK' END                                  AS [FAK/LTA]                                                ";

                sqlSelectTP_TEU = " , CASE WHEN ContainerType = '20''SD' THEN 1 ELSE 2 END AS [TEU] ";
            }
            else if (bookingType.Equals("NON-TP"))
            {
                sqlSelect_YearAndWeek = " , TopoceanYearAndWeek                                 AS [Year and Week]  ";
                sqlSelect_MONTH = " , tb_week._MONTH AS [Month] ";
                sqlSelectNONTP_TEU = " , CASE WHEN ContainerType = '20''SD' THEN 1 ELSE 2 END AS [TEU] ";
                sqlJoin = " LEFT JOIN (" + RetGetMonthSQL() + ") tb_week ON tb_week._WEEK = T.Week";
            }

            string sql = " SELECT ROW_NUMBER() OVER (ORDER BY TopoceanYearAndWeek)    AS [Seq.]                                                   " +
                         "      , CNEE                                                AS [CNEE]                                                   " +
                         "      , Vendor                                              AS [Vendor]                                                 " +
                         "      , WB_Status                                           AS [Booking Status]                                         " +
                         "      , TransportationMode                                  AS [Transportation Mode]                                    " +
                         "      , BookingReqID                                        AS [Booking ID]                                             " +
                         "      , OriginOffice                                        AS [Origin Office]                                          " +
                         "      , T.Dest                                              AS [Dest Office]                                            " +
                         "      , principle                                           AS [Principal]                                              " +
                         "      , Orig                                                AS [Place Of Receipt]                                       " +
                         "      , CarrierDest                                         AS [Carrier Dest]                                           " +
                         "      , Carrier                                             AS [Carrier]                                                " +
                         "      , Commodity2                                          AS [Carrier Commodity]                                      " +
                         "      , Commodity                                           AS [Actual Commodity]                                       " +
                         "      , BookingContractType                                 AS [Contract Type]                                          " +
                         "      , CNEECarrierContractNo                               AS [Contract #]                                             " +
                         "      , Vessel                                              AS [Vessel]                                                 " +
                         "      , Voyage                                              AS [Voyage]                                                 " +
                         "      , MBL                                                 AS [MBL]                                                    " +
                         "      , DeliveryType                                        AS [Delivery Type]                                          " +
                         "      , RcvdDate                                            AS [Gate In Date]                                           " +
                         "      , loadPort.PortName                                   AS [POL]                                                    " +
                         "      , loadPort.Country                                    AS [POL Country]                                            " +
                         sqlSelect_POL +
                         "      , OriginalP2PETD                                      AS [Original ETD]                                           " +
                         "      , P2PETD                                              AS [Current ETD]                                            " +
                         "      , P2PATD                                              AS [ATD]                                                    " +
                         "      , dischPort.PortName                                  AS [POD]                                                    " +
                         "      , dischPort.Country                                   AS [POD Country]                                            " +
                         sqlSelect_POD +
                         "      , OriginalP2PETA                                      AS [Original ETA]                                           " +
                         "      , P2PETA                                              AS [Current ETA]                                            " +
                         "      , DestRamp                                            AS [Dest Ramp]                                              " +
                         "      , ShipmentType                                        AS [Shipment Type]                                          " +
                         "      , ContainerNo                                         AS [Container Number]                                       " +
                         "      , ContainerType                                       AS [Container Type]                                         " +
                         sqlSelectNONTP_TEU +
                         "      , NomName                                             AS [Nomination Office]                                      " +
                         "      , NomSales                                            AS [Nomination Sales]                                       " +
                         "      , Term                                                AS [Trade Term]                                             " +
                         "      , HBL                                                 AS [HBL]                                                    " +
                         "      , Traffic                                             AS [Traffic]                                                " +
                         "      , BookingConfirmation                                 AS [SO No]                                                  " +
                         "      , DrayageCompany                                      AS [Drayage Company]                                        " +
                         "      , FinalDest                                           AS [Final Dest]                                             " +
                         "      , D2DApt                                              AS [Final Apt]                                              " +
                         "      , D2DETA                                              AS [Final ETA]                                              " +
                         "      , D2DRvsdETA                                          AS [Final Rvsd ETA]                                         " +
                         "      , D2DATA                                              AS [Final ATA]                                              " +
                         "      , D2DPOD                                              AS [Proof of Delivery Receipt Date]                         " +
                         "      , D2DMTYNotify                                        AS [Empty Ready Notification]                               " +
                         "      , D2DMTYPickup                                        AS [Empty Picked Up from Final]                             " +
                         "      , D2DMTYRET                                           AS [Final Empty Return]                                     " +
                         "      , DeliveryNumber                                      AS [Delivery Number]                                        " +
                         sqlSelect_YearAndWeek +
                         sqlSelect_Year +
                         sqlSelect_Week +
                         sqlSelect_MONTH +
                         sqlSelectTP_TEU +
                         sqlSelect_FK_LAT +
                         "      , BookingCreatedBy                                    AS [Booking Created By]                                     " +
                         "      , UserEmail                                           AS [Handling User Email]                                    " +
                         " FROM T                                                                                                                 " +
                         " LEFT JOIN Port loadPort ON T.LoadPort = loadPort.PortAbbr                                                              " +
                         " LEFT JOIN Port dischPort ON T.DischPort = dischPort.PortAbbr                                                           " + sqlJoin;

            return sql;
        }

        private void radioBtn_CheckedChange(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked)
            {
                return;
            }

            switch (((RadioButton)sender).Text.ToString())
            {
                case "TP":
                    this.bookingType = "TP";
                    break;
                case "NON-TP":
                    this.bookingType = "NON-TP";
                    break;
            }
        }
    }
}
