using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class AMS_ResponseMode
    {
        public DataTable table { get; set; }
        public DataTable temptable { get; set; }
        public string mailFrom { get; set; }
        public string mailTo { get; set; }
        public string result { get; set; }
    }

    public class Load_SPRC_ResponseData
    {
        public DataTable sheet1 { get; set; }
        public DataTable Account { get; set; }
        public DataTable Carrier { get; set; }
    }

    public class Load_Volume_ResponseData
    {
        public DataTable sheet1 { get; set; }
        public DataTable Volume { get; set; }
        public DataTable Workload { get; set; }
    }
}