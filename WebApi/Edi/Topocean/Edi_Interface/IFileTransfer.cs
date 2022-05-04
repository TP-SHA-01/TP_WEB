using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApi.Edi.Topocean.Edi_Interface
{
    interface IFileTransfer : IBase_EDI
    {
        void init(DataTable baseDT);

        /*
         * void init(DataTable baseDT, LabelGenerationModelVO lgVO);
        bool CheckHasCartonSerialStart(string CNEE_ID);
        bool UpdateAfterGetAPI(LabelGenerationModelVO lgVO);
        object RetLabelGenerationModelVo(LabelGenerationModelVO lgVO);
        object initData(LabelGenerationModelVO lgVO);
         * */

    }
}