using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Topocean.Edi_Interface
{
    public interface IBase_EDI
    {
        object hydrate();
        string serialize();
    }
}
