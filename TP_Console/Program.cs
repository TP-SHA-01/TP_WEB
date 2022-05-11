using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Common;
using WebApi.Edi.Topocean.EdiModels.Common;
using WebApi.Models;
using WebApi.Services;

namespace TP_Console
{
    class Program
    {
        private static int resend_count = 2;
        static void Main(string[] args)
        {

            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Start Program"));

            if (!DoGetAMSFiling())
            {
                for (int i = 1; i < resend_count; i++)
                {
                    if (DoGetAMSFiling())
                    {
                        break;
                    }
                }
            }
            else
            {
                LogHelper.Debug("Sent Success");
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Program Success"));
            }

            Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "End Program"));
        }

        private static bool DoGetAMSFiling() {
            bool ret = true;
            DataTable dt = new DataTable();

            try
            {
                string officeList = ConfigurationManager.AppSettings["OFFICE_LIST"];

                if (String.IsNullOrEmpty(officeList))
                {
                    LogHelper.Error("No OFFICE_LIST");
                }

                string[] list = officeList.Split(',');

                for (int i = 0; i < list.Length; i++)
                {
                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Office-" + list[i] + " Strat"));
                    AMS_ResponseMode responseMode = AMSFilingRpt_Service.GetAMSFilingData(list[i]);
                    EDIMapModel model = new EDIMapModel();
                    model.EDIType = "AMS_ALERT";
                    model.Identifier = list[i];
                    model.Identifier2 = null;
                    model.Identifier3 = responseMode.result;
                    model.icn_type = "AMS_ALERT_ICN";
                    EDIArchiveModel archive_model = new EDIArchiveModel();
                    archive_model.EDIType = "AMS_ALERT";
                    archive_model.DataContent = JsonConvert.SerializeObject(responseMode.table);
                    archive_model.EDIContent = JsonConvert.SerializeObject(responseMode.temptable);

                    CommonUnit.InsertAMSEDIRecord(archive_model,model);

                    if (responseMode.result != "Success")
                    {
                        Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), responseMode.result));
                        for (int x = 1; x < resend_count; x++)
                        {
                            if (DoGetAMSFiling())
                            {
                                break;
                            }
                        }
                    }

                    Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Office-" + list[i] + " End"));

                    Thread.Sleep(180000);
                }

                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Sent Sucess"));
            }
            catch (Exception ex)
            {
                ret = false;
                LogHelper.Error("Message: " +  ex.Message + ",StackTrace: " + ex.StackTrace);
                Console.WriteLine(string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Message: " + ex.Message + ",StackTrace: " + ex.StackTrace));
            }
            
            return ret;
        }
    }
}
