using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class EDIMapModel
    {
        public string EDIType { get; set; }
        public string Identifier { get; set; }
        public string Identifier2 { get; set; }
        public string Identifier3 { get; set; }
        public string ICN { get; set; }
        public string icn_type { get; set; }
        
    }
}