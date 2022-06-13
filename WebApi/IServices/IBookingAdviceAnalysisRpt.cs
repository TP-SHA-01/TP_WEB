using System.Collections.Generic;

namespace WebApi
{
    public interface IBookingAdviceAnalysisRpt
    {
        IEnumerable<string> GetValue();
    }
}
