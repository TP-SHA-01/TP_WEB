using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class EDIArchiveModel
    {
        public string InputDate { get; set; }
        public string EDIType { get; set; }
        public string EDIContent { get; set; }
        public string DataContent { get; set; }
        public string ICN { get; set; }
        public string TransactionSetCtrlNum { get; set; }
        public string Ack997ST { get; set; }
        public string Ack997DateTime { get; set; }
        public string Ack997Content { get; set; }
        public string Ack997Flag { get; set; }
        public string SentStatus { get; set; }
        public string SentBy { get; set; }
        public string IP { get; set; }
        public string Remark { get; set; }
        public string AckResponseStatus { get; set; }
        public string EDIStatus { get; set; }
        public string EDIStatusDate { get; set; }
    }
}