using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class AuthenticationEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public string usr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string psd { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int session_expire { get; set; }
    }
}
