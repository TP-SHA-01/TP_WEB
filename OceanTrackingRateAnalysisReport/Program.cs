using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OceanTrackingRateAnalysisReport
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
          
            Application.Run(new TrackingRateReport());
        }
    }
}
