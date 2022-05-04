using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class EDI_Payload<T>
    {
        public int total_count { get; set; }
        public List<T> records { get; set; }
        public bool cache { get; set; }
    }
}
