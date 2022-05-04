using System.Collections.Generic;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class BaseReponseModel_V3<T,R>
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
        public virtual R request { get; set; }
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
