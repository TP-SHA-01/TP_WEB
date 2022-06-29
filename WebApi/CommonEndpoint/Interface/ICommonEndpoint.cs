using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.CommonEndpoint.Model;
using WebApi.Edi.Topocean.Edi_Interface;

namespace WebApi.CommonEndpoint.Interface
{
    public interface ICommonEndpoint : IBase_EDI_Response
    {
        void initSetting(ApiOptionVo optionVo);
    }
}
