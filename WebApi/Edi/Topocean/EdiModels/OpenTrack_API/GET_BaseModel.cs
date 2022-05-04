using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.OpenTrack_API
{
    public class GET_BaseModel
    {
        //public GetRecordModel record { get; set; }
        public Dictionary<string, PoTracingModel> po_dict { get; set; }
        //public Res_Base_VO<Res_PlayLoad_VO<Res_Record_VO>, GetRecordModel> op_record { get; set; }

        public class PoTracingModel {
            public int uID { get; set; }
            public string BookingReqID { get; set; }
            public string MBL { get; set; }
            public string ContainerNo { get; set; }
            public string CNEE { get; set; }
            public string Carrier { get; set; }
            public string Vessel { get; set; }
            public string Voyage { get; set; }
            public string BookingContractType { get; set; }
            public string LoadPort { get; set; }
            public string LoadPortCode { get; set; }
            public string DischPort { get; set; }
            public string DischPortCode { get; set; }
            public string Locked { get; set; }
            public string P2PATA { get; set; }
            public string P2PATD { get; set; }
            public string SCAC { get; set; }
            public string P2PETD { get; set; }
            public string OutGateDate { get; set; }
            public string D2DATA { get; set; }
            public string DestRamp { get; set; }
            public string OpentrackStatus { get; set; }
            public string DictKey { get; set; }


            public string IsTBS { get; set; }
        }
    }
}
