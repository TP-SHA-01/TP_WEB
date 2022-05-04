using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Topocean.EdiModels.VGM_EDI
{
    public class VGMResponse : BaseReponseModel<VGMResponse>
    {
        public string id { set; get; }
        public Payload payload { get; set; }
        /// </summary>
        public Request request { get; set; }
        /// <summary>
        public class Payload
        {
            public string status { set; get; }
            public string id { set; get; }
            public Create create { get; set; }
        }

        public class Request
        {
            /// <summary>
            /// 
            /// </summary>
            public User user { get; set; }
        }

        public class User
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string username { get; set; }
        }

        public class Create
        {
            /// <summary>
            /// 
            /// </summary>
            public Edi_envelope edi_envelope { get; set; }
        }

        public class Edi_envelope
        {
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
        }


        public string repJson { set; get; }
    }
}
