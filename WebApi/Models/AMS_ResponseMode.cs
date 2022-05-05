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
}