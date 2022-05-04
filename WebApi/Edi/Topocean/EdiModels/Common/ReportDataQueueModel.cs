using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class ReportDataQueueModel
    {
        public string ReportType { get; set; }
        public int DataID { get; set; }
        public string AddData1 { get; set; }
        public string AddData2 { get; set; }
        public string Active { get; set; }
    }
}