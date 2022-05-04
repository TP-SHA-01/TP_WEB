using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Edi.Topocean.EdiModels.Common;
using WebApi.Edi.Topocean.EdiModels.Generate_EDI;

namespace WebApi.Edi.Topocean.EdiModels.VGM_EDI
{
    public class VGM_EDI : GenerateEDI
    {
        protected string fileName { get; set; }

        public VGM_EDI() { }
        public VGM_EDI(string fileName)
        {
            this.fileName = fileName;
            setValue();
        }

        public VGM_Create create { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> send { get; set; }

        protected void setValue()
        {
            List<string> sendList = new List<string>();
            sendList.Add("sftp_inttra");
            sendList.Add("/inbound/vgm/" + this.fileName + ".txt");
            //sendList.Add("/inbound-test/vgm/" + this.fileName + ".txt");

            this.type = "vermas";
            this.handler = "inttra";
            this.actions = "create,send";
            this.send = sendList;
            this.create = create;
        }
    }

    public class VGM_Create : Create
    {
        /// <summary>
        /// 
        /// </summary>
        public Shipment shipment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ContainersItem> containers { get; set; }
    }

}
