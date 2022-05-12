using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public static class BaseCont
    {
        public static int HTTP_200_OK = 200;
        public static int HTTP_401_UNAUTHORIZED = 401;
        public static int HTTP_404_NOT_FOUND = 404;
        public static int HTTP_405_METHOD_NOT_ALLOWED = 405;
        public static int HTTP_419_AUTH_TIMEOUT = 419;
        public static int HTTP_412_PRECONDITION_FAILED = 412;
        public static int HTTP_422_UNPROCESSABLE = 422;
        public static int HTTP_426_UPGRADE_REQUIRED = 426;
        public static int HTTP_500_ERROR = 500;


        public static string API_POST_SERNULL = "Please check the CartonSerialStart has create";
        public static string API_POST_CreateNewLabelFail = "Print Fail , please try again ! ";


        public static string tokenHeadName = "Authorization";
        public static string JWTHead = "Bearer ";
        public static string AllowCNEE = "SPANX,BIOWORLD";
        public static string authKey = "886143";
        public static string ediPartnerCode = "TOPO";
        public static string usr = ConfigurationManager.AppSettings["HC_USER"] ?? "";
        public static string psd = ConfigurationManager.AppSettings["HC_USERPW"] ?? "";
        public static string ApiUrl = ConfigurationManager.AppSettings["API_END_POINT"] ?? "";
        public static string WBISRV_APP_CODE = ConfigurationManager.AppSettings["WBISRV_APP_CODE"] ?? "";
        public static string Bucket = ConfigurationManager.AppSettings["AWSBucketToLabel"] ?? "";
        public static string ENVIRONMENT = ConfigurationManager.AppSettings["ASPNET_ENVIRONMENT"] ?? "";
        public static string AWSS3DocBucket = ConfigurationManager.AppSettings["AWSS3DocBucket"] ?? "";
        public static string AWSS3ReportSubFolder = ConfigurationManager.AppSettings["AWSS3ReportSubFolder"] ?? "";
        public static string OFFICE_LIST = ConfigurationManager.AppSettings["OFFICE_LIST"] ?? "";

        public static int ExpiresTime = 30;
        public static int pageSize_Terminal_POST = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize_Terminal_POST"] ?? "20") ;
        public static int pageSize_OpenTrack_POST = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize_OpenTrack_POST"] ?? "20") ;
        public static int pageSize_Terminal_GET = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize_Terminal_GET"] ?? "8");
        public static int pageSize_OpenTrack_GET = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize_OpenTrack_GET"] ?? "8");
        public static int Terminal_API_Days = Convert.ToInt32(ConfigurationManager.AppSettings["Terminal_API_Days"] ?? "0");
        public static int pageSize_APM_Terminal_API = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize_APM_Terminal_API"] ?? "5");

        public static string apmTerminal_Source = "apm";
        public static string OpenTrack_Source = "opentrack";
        public static string APM_Terminal_CarrierList = ConfigurationManager.AppSettings["APM_Terminal_CarrierList"] ?? "";
        public static string APM_Terminal_Discharge_Port = ConfigurationManager.AppSettings["APM_Terminal_Discharge_Port"] ?? "";

        public static string Title_IMG = "data:image/png;base64,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";
        public static int PrePrintForm_Cntr_Height = Convert.ToInt32(ConfigurationManager.AppSettings["PrePrintForm_Cntr_Height"] ?? "45");
    }
}
