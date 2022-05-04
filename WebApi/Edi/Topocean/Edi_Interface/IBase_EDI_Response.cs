using System.Collections.Generic;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Topocean.Edi_Interface
{
    public interface IBase_EDI_Response : IBase_EDI
    {
        void init(EDIPostEntity baseEntity, AuthenticationModel auEntity);
        void init(ParameterEntity parameterEntity);
        void Post();
        void Put();
        void Get();
        void filter(Dictionary<string, string> dicValue);
        bool CheckAccess(AuthenticationModel auEntity);
        T LoadBaseResponse<T>();
        string repPOSTJson();
    }
}
