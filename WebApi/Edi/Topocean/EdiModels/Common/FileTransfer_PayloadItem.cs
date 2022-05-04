using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class FileTransfer_PayloadItem
    {
        private string outputStr;
        private JObject outputObj = null;

        public int id { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public string source_content { get; set; }
        public string destination_content { get; set; }
        public string status { get; set; }
        public string elapsed_time { get; set; }
        public string created_time { get; set; }
        public object output
        {
            get
            {
                if (outputObj == null)
                {
                    return outputStr;
                }
                else
                {
                    return outputObj;
                }
            }
            set
            {
                if (value is JObject)
                {
                    //outputObj = ((JObject)value).ToObject<AMS_Output>();
                    outputObj = ((JObject)value);
                }
                else
                {
                    outputStr = value.ToString();
                }
            }
        }

    }
}