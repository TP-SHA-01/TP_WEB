using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using WebApi.CommonEndpoint.Impl;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.EdiModels.Common;
using WebApi.Edi.Topocean.EdiModels.OpenTrack_API;

namespace LevelReport
{
    public partial class MianFrm : Form
    {
        public MianFrm()
        {
            InitializeComponent();

            InitControl();
        }

        private void InitControl()
        {
            InitCombox_Carrier();
            InitCombox_Traffic();
            InitCombox_BookingStatus();
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

        private void InitCombox_BookingStatus() {
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

            cmb_Carrier.BandSource(tb, views);
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
            int id = 1;

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

            cmb_Traffic.BandSource(tb, views);
        }

        private void boxCombox_SelectedValueChanged(object sender, EventArgs e)
        {
            var values = cmb_Carrier.SelectedValues;
        }

        private void cmb_Traffic_SelectedValueChanged(object sender, EventArgs e)
        {
            var values = cmb_Traffic.SelectedValues;
        }

        private void cmb_Status_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
