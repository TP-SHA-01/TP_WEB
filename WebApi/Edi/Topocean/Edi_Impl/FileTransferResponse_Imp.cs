using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Edi.Topocean.Edi_Interface;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Topocean.Edi_Impl
{
    public class FileTransferResponse_Imp : Base_EDI_Impl, IFileTransferResponse
    {
        public FileTransferResponse<T> hydrate<T>()
        {
            FileTransferResponse<T> retObj = null;
            try
            {
                var jsonModel = JsonToObject(this.repJson);
                retObj = this.LoadBaseResponse<FileTransferResponse<T>>();
            }
            catch (Exception ex)
            {
                retObj = null;
            }
            return retObj;
        }
    }
}