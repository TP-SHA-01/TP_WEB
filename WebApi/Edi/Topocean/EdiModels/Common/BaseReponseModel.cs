using System.Collections.Generic;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class BaseReponseModel<T>
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
        public Request request { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double start_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Payload payload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double end_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double elapsed_time { get; set; }

        public class Payload
        {
            /// <summary>
            /// 
            /// </summary>
            public int total_count { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<T> records { get; set; }
        }

        public class Request
        {
            
        }
        public string repJson { set; get; }
    }
}
