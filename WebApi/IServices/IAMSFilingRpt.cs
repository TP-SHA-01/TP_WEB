using System.Collections.Generic;

namespace WebApi
{
    public interface IAMSFilingRpt
    {
        IEnumerable<string> GetValue();
    }
}