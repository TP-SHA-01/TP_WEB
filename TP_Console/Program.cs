using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.EdiModels.Common;
using WebApi.Models;
using WebApi.Services;

namespace TP_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string officeList = String.Empty;
            string[] officeGroup = ConfigurationManager.AppSettings["OFFICE_GROUP"].Split(',');
            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Start Program"));

            string pPara = CommonUnit.CheckEmpty(args[0]);

            if (String.IsNullOrEmpty(pPara))
            {
                LogHelper.Error("Input Error Office:" + pPara);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "input Office is Null"));
                Console.ReadKey();
            }
            else
            {
                string[] arrArg = pPara.Split(',');
                for (int i = 0; i < arrArg.Length; i++)
                {
                    if (!officeGroup.Contains(arrArg[i]))
                    {
                        LogHelper.Error("Input Error Office:" + arrArg[i]);
                        Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "input Office did not Exists"));
                        Console.ReadKey();
                    }
                }
            }

            officeList = pPara;
            
            string[] list = officeList.Split(',');

            for (int i = 0; i < list.Length; i++)
            {
                string strOffice = list[i];
                if (!DoGetAMSFiling(strOffice))
                {
                    LogHelper.Debug("Sent Error Office:" + strOffice);
                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Fail"));
                }
                else
                {
                    LogHelper.Debug("Sent Success Office:" + strOffice);
                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Success"));
                }
            }


            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "End Program"));
        }

        private static bool DoGetAMSFiling(string _office)
        {
            bool ret = true;
            DataTable dt = new DataTable();
            try
            {
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Office-" + _office + " Strat"));
                AMS_ResponseMode responseMode = AMSFilingRpt_Service.GetAMSFilingData(_office);
                EDIMapModel model = new EDIMapModel();
                model.EDIType = "AMS_ALERT";
                model.Identifier = _office;
                model.Identifier2 = null;
                model.Identifier3 = responseMode.result;
                model.icn_type = "AMS_ALERT_ICN";
                EDIArchiveModel archive_model = new EDIArchiveModel();
                archive_model.EDIType = "AMS_ALERT";
                archive_model.DataContent = JsonConvert.SerializeObject(responseMode.table);
                archive_model.EDIContent = JsonConvert.SerializeObject(responseMode.temptable);

                CommonUnit.InsertAMSEDIRecord(archive_model,model);

                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), responseMode.result));

                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Office-" + _office + " End"));

                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Office-" + _office + " Sent Sucess"));

                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Wait...."));

                Thread.Sleep(180000);
            }
            catch (Exception ex)
            {
                ret = false;
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message: " + ex.Message + ",StackTrace: " + ex.StackTrace));
            }

            return ret;
        }
    }
}
