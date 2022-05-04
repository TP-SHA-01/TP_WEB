using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Helpers;

namespace WebApi.Edi.Topocean.EdiModels.Generate_EDI
{
    public class EDI_Envelope
    {
        DBHelper dbHelper = new DBHelper();
        string errMsg = String.Empty;

        public EDI_Envelope()
        {
            //setValue();
        }

        public EDI_Envelope(string actionCode)
        {
            setValue();
            this.icn = Convert.ToInt32(GetVGM_ICN());
            this.vgm_trackno = Convert.ToInt32(GetVGM_TrackNo());
            this.actioncode = actionCode;
        }

        protected void setValue()
        {
            this.sender_id = "TOPOCEAN";
            this.sender_qualifier = "ZZZ";
            this.recipient_id = "INTTRA";
            this.recipient_qualifier = "ZZZ";
            this.acknowledge_requested = "1";
        }

        /// <summary>
        /// 
        /// </summary>
        public string sender_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sender_qualifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string recipient_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string recipient_qualifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string acknowledge_requested { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int icn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int vgm_trackno { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string actioncode { get; set; }



        public string GetVGM_ICN()
        {
            string strVGM_ICN = String.Empty;
            try
            {
                string sql = " select vgm_icn from edisysinfo ";
                DataTable dt = dbHelper.ExecDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    strVGM_ICN = dt.Rows[0]["vgm_icn"].ToString().Trim();

                    string udpSql = " update edisysinfo set vgm_icn = vgm_icn+1 ";
                    bool updQuery = dbHelper.ExecuteQuery(CommandType.Text, udpSql);
                    if (updQuery)
                    {
                        return strVGM_ICN;
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = "999";
            }

            return errMsg;
        }

        public string GetVGM_TrackNo()
        {
            string strVGM_TrackNo = String.Empty;
            try
            {
                string sql = " select vgm_trackno from edisysinfo  ";
                DataTable dt = dbHelper.ExecDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    strVGM_TrackNo = dt.Rows[0]["vgm_trackno"].ToString().Trim();

                    string udpSql = " update edisysinfo set vgm_trackno = vgm_trackno+1 ";
                    bool updQuery = dbHelper.ExecuteQuery(CommandType.Text, udpSql);
                    if (updQuery)
                    {
                        return strVGM_TrackNo;
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = "999";
            }

            return errMsg;
        }
    }
}
