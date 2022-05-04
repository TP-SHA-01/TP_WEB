using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class EDIPostEntity
    {
        public string apiUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Hashtable queryParams { get; set; }

        public string body { get; set; }
    }
}
