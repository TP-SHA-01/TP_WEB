using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Topocean.Edi_Interface
{
    public interface IFileTransferResponse : IBase_EDI_Response
    {
        FileTransferResponse<T> hydrate<T>();
    }
}