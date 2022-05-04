using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.OpenTrack_API
{
    public class POST_ResponseModel<T>
    {
        public int success { get; set; }
        public int errors { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<T> error_results { get; set; }
    }
}
