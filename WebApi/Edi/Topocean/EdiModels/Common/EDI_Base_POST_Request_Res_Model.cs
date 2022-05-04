using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class EDI_Base_POST_Request_Res_Model
    {
        /// <summary>
        /// 
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vessel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string created_by { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cnee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string booking_id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string confirmation_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string container_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string scac { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hbl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string load_port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string batch_date { get; set; }
    }
}
