using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.OpenTrack_API
{
    public class Res_Record_VO
    {
        /// <summary>
        /// 
        /// </summary>
        public string source { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string container_status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mbl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string container_no { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string vessel_name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string carrier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string discharge_port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string container_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string container_size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string create_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string appointment_time { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<MilestonesItem> milestones { get; set; }

        public class MilestonesItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string milestone { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string timestamp { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string firms_code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string address { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string unlocode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double latitude { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double longitude { get; set; }
        }
    }
}
