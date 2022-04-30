using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Helpers;

namespace TP_Console
{
    class Program
    {
        DBHelper dBHelper = new DBHelper();

        static void Main(string[] args)
        {
            DataTable dt = CommonFun.GetAMSFilingData();

            string ret = JsonConvert.SerializeObject(dt);

            Console.WriteLine(ret);
            Console.ReadLine();
        }
    }
}
