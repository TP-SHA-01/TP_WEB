using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class EDI_Base_POST_Res_Model<T,M>
    {
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string user_agent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string client_ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual List<M> request { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double start_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual T payload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double end_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double elapsed_time { get; set; }
    }
}
