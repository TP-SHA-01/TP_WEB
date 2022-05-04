using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class ParameterEntity
    {
        public DataTable dt { get; set; }
        public string fileName { get; set; }
        public string actionCode { get; set; }

        public string edicontent { get; set; }
        public int icn { get; set; }
        public string icn_type { get; set; }
        public string mbl { get; set; }
        public string transactionSetCtrlnum { get; set; }
        public string identifier { get; set; }
        public string sendby { get; set; }
        public string edi_type { get; set; }

        #region For Port 
        public string potracingPort { get; set; }
        public string velaTrackPort { get; set; }
        public string ediPortCode { get; set; }
        public Dictionary<string, PortVO> inPortMap { get; set; }
        #endregion

        #region For Sanity
        public string strDateName { get; set; }
        public string strDateType { get; set; }
        public string strDate { get; set; }
        public string refDate { get; set; }
        #endregion
    }
}
