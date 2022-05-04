using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class BaseEDIPayloadItem
    {
        protected string outputStr;
        protected JObject outputObj = null;

        protected string file_transfer_outputStr;
        protected JObject file_transfer_outputObj = null;

        public int id { get; set; }
        public string actions { get; set; }
        public string status { get; set; }
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
        public int file_transfer_queue_id { get; set; }
        public string file_transfer_status { get; set; }
        public object file_transfer_output
        {
            get
            {
                if (file_transfer_outputObj == null)
                {
                    return file_transfer_outputStr;
                }
                else
                {
                    return file_transfer_outputObj;
                }
            }
            set
            {
                if (value is JObject)
                {
                    //outputObj = ((JObject)value).ToObject<AMS_Output>();
                    file_transfer_outputObj = ((JObject)value);
                }
                else
                {
                    file_transfer_outputStr = value.ToString();
                }
            }
        }
        public string elapsed_time { get; set; }
        public string created_time { get; set; }
    }
}
