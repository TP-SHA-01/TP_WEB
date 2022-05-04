using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class PortVO
    {
        public string isoPortCode { get; set; }
        public string isoCountryCode { get; set; }
        public string portAbbr { get; set; }
        public string akaPortAbbr { get; set; }
        public string portKey { get; set; }
    }
}
