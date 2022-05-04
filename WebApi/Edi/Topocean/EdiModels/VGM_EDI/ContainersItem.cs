using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Helpers;
using WebApi.Edi.Topocean.EdiModels.Generate_EDI;

namespace WebApi.Edi.Topocean.EdiModels.VGM_EDI
{
    public class ContainersItem
    {
        protected DataTable ediDataTable { get; set; }
        DBHelper dbHelper = new DBHelper();
        public ContainersItem() { }
        public ContainersItem(DataTable dt)
        {
            this.ediDataTable = dt;
        }

        public List<ContainersItem> GetContainersItemList()
        {
            List<ContainersItem> containers = new List<ContainersItem>();

            if (ediDataTable.Rows.Count > 0)
            {
                string currCBM = String.Empty;
                string currContainerType = String.Empty;
                string currContainerISO = String.Empty;
                string currShipQty = String.Empty;
                string weighing_method = String.Empty;
                string currMethodCode = String.Empty;

                Double vgmweight = 0;
                Double tareweight = 0;
                Double packweight = 0;
                Double goodsweight = 0;

                for (int i = 0; i < ediDataTable.Rows.Count; i++)
                {
                    ContainersItem containersItem = new ContainersItem();

                    currCBM = ediDataTable.Rows[i]["cbm"].ToString().Trim();
                    currContainerType = ediDataTable.Rows[i]["containertype"].ToString().Trim();
                    currContainerISO = ediDataTable.Rows[i]["containerISO"].ToString().Trim();
                    currShipQty = ediDataTable.Rows[i]["shipqty"].ToString().Trim();

                    vgmweight = Convert.ToDouble(ediDataTable.Rows[i]["vgmweight"].ToString().Trim());
                    tareweight = Convert.ToDouble(ediDataTable.Rows[i]["tareweight"].ToString().Trim());
                    packweight = Convert.ToDouble(ediDataTable.Rows[i]["packweight"].ToString().Trim());
                    goodsweight = Convert.ToDouble(ediDataTable.Rows[i]["goodsweight"].ToString().Trim());

                    weighing_method = ediDataTable.Rows[i]["weighingmethod"].ToString().Trim().ToUpper();
                    string scac = ediDataTable.Rows[0]["scac"].ToString().Trim();
                    Double equipment_weight = 0;
                    Double equipment_tareweight = 0;
                    Double equipment_packweight = 0;
                    Double equipment_goodsweight = 0;

                    if (weighing_method == "METHOD 1")
                    {
                        currMethodCode = "SM1";
                        equipment_weight = Math.Round(vgmweight * 100) / 100;
                        equipment_tareweight = 0;
                        equipment_packweight = 0;
                        equipment_goodsweight = 0;
                    }
                    else if (weighing_method == "METHOD 2")
                    {
                        currMethodCode = "SM2";
                        equipment_weight = Math.Round((tareweight + packweight + goodsweight) * 100) / 100;
                        equipment_tareweight = tareweight;
                        equipment_packweight = packweight;
                        equipment_goodsweight = goodsweight;
                    }

                    string equipmentTypeCode = String.Empty;
                    if (currContainerType.Contains("LCL"))
                    {
                        equipmentTypeCode = "42G0";
                    }
                    else
                    {
                        equipmentTypeCode = currContainerISO;
                    }

                    containersItem.equipment_number = ediDataTable.Rows[i]["containerno"].ToString().Trim();
                    containersItem.equipment_weight = equipment_weight;
                    containersItem.equipment_tareweight = equipment_tareweight;
                    containersItem.equipment_packweight = equipment_packweight;
                    containersItem.equipment_goodsweight = equipment_goodsweight;
                    containersItem.equipment_weight_qualifier = "G";
                    containersItem.equipment_volume = currCBM;
                    containersItem.equipment_volume_qualifier = "X";
                    containersItem.equipment_type_code = equipmentTypeCode;
                    containersItem.equipment_shipped_qty = currShipQty;
                    containersItem.equipment_seal_no = ediDataTable.Rows[i]["sealno"].ToString().Trim();
                    containersItem.equipment_weighing_method = ediDataTable.Rows[i]["weighingmethod"].ToString().Trim();
                    containersItem.equipment_method_code = currMethodCode;
                    containersItem.equipment_vgmid = ediDataTable.Rows[i]["vgmid"].ToString().Trim();
                    containersItem.equipment_weighdate = ediDataTable.Rows[i]["weighdate"].ToString().Trim();
                    containersItem.equipment_signeddate = ediDataTable.Rows[i]["signeddate"].ToString().Trim();
                    containersItem.equipment_signatory = ediDataTable.Rows[i]["signatory"].ToString().Trim();
                    containersItem.equipment_carrier = ediDataTable.Rows[i]["carrier"].ToString().Trim();
                    if (!isInttraValidSCAC(scac))
                    {
                        containersItem.equipment_comseg = getCOMSeg();
                    }
                    Weighing_party weighing_party = new Weighing_party();
                    weighing_party.name = ediDataTable.Rows[i]["Weighing_name"].ToString().Trim();
                    weighing_party.address = ediDataTable.Rows[i]["Weighing_address"].ToString().Trim();
                    weighing_party.city = ediDataTable.Rows[i]["Weighing_city"].ToString().Trim();
                    weighing_party.state = ediDataTable.Rows[i]["Weighing_state"].ToString().Trim();
                    weighing_party.zip = ediDataTable.Rows[i]["Weighing_zip"].ToString().Trim();
                    weighing_party.country = ediDataTable.Rows[i]["Weighing_country"].ToString().Trim();
                    containersItem.weighing_party = weighing_party;

                    Authorized_party authorized_party = new Authorized_party();
                    authorized_party.name = ediDataTable.Rows[i]["Authorized_name"].ToString().Trim();
                    authorized_party.address = ediDataTable.Rows[i]["Authorized_address"].ToString().Trim();
                    authorized_party.city = ediDataTable.Rows[i]["Authorized_city"].ToString().Trim();
                    authorized_party.state = ediDataTable.Rows[i]["Authorized_state"].ToString().Trim();
                    authorized_party.zip = ediDataTable.Rows[i]["Authorized_zip"].ToString().Trim();
                    authorized_party.country = ediDataTable.Rows[i]["Authorized_country"].ToString().Trim();
                    containersItem.authorized_party = authorized_party;

                    containers.Add(containersItem);
                }
            }


            return containers;
        }

        /// <summary>
        /// 
        /// </summary>
        public string equipment_number { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double equipment_weight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double equipment_tareweight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double equipment_packweight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double equipment_goodsweight { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_weight_qualifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_volume { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_volume_qualifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_type_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_shipped_qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_seal_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_weighing_method { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_method_code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_vgmid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_weighdate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_signeddate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_signatory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_carrier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string equipment_comseg { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        public Weighing_party weighing_party { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Authorized_party authorized_party { get; set; }


        public bool isInttraValidSCAC(string scac)
        {
            bool isInttraValidSCAC = true;
            try
            {
                string sql = "select 1 as valid from optionlist where opttype='INTTRA_VGM_SCAC' and isactive='Y' and criteria1='" + scac + "'";
                DataTable dt = dbHelper.ExecDataTable(sql);

                string valid = String.Empty;
                if (dt.Rows.Count > 0)
                {
                    valid = dt.Rows[0]["valid"].ToString().Trim();
                    if (valid != "1")
                    {
                        isInttraValidSCAC = false;
                    }
                }
            }
            catch (Exception e)
            { }

            return isInttraValidSCAC;
        }

        public string getCOMSeg()
        {
            string ret = String.Empty;

            try
            {
                string sql = " SELECT max(ID) over() ID, Data FROM [dbo].[Split] " +
                             " ((select top 1 emails from emaillist Where ProfileName = 'TopoceanUtil_DevErrorMsg' and IsActive = 'ACTIVE'),',') ";
                string COMSeg = String.Empty;

                DataTable dt = dbHelper.ExecDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        COMSeg += dt.Rows[0]["Data"].ToString().Trim() + ",";
                    }
                }

                ret = COMSeg.TrimEnd(',');
            }
            catch (Exception e)
            { }

            return ret;
        }

    }

    public class Weighing_party : Party { }

    public class Authorized_party : Party { }


}
