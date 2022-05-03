using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Helpers;
using WebApi.Services;

namespace TP_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable dt = AMSFilingRpt_Service.GetAMSFilingData();

            string ret = JsonConvert.SerializeObject(dt);

            Console.WriteLine(ret);
            Console.ReadLine();
        }
    }
}
