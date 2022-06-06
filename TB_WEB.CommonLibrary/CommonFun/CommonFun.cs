using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TB_WEB.CommonLibrary.Encrypt;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

namespace TB_WEB.CommonLibrary.CommonFun
{
    public static class CommonFun
    {
        static DBHelper dbHelper = new DBHelper();

        #region Log functions
        public static void SaveLog(string msg, string type = "I")
        {
            try
            {
                bool tmpEnable = false;
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
                {
                    if (System.Web.HttpContext.Current.Session["TmpEnableLog"] != null && System.Web.HttpContext.Current.Session["TmpEnableLog"].ToString() == "Y")
                    {
                        tmpEnable = true;
                    }
                }

                if (Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"]) >= 1 || tmpEnable || type == "EX")
                {
                    string logPath = System.Configuration.ConfigurationManager.AppSettings["LogFilelocation"].ToString();
                    string MsgHdr = "";

                    // Check Log Path Existance
                    if (!System.IO.Directory.Exists(logPath))
                    {
                        System.IO.Directory.CreateDirectory(logPath);
                    }

                    StreamWriter fso = new StreamWriter(logPath + "\\" + DateTime.Now.ToString("yyyy.MM.dd") + ".txt", true);

                    if (type == "I")
                    {
                        MsgHdr = "Message:";
                    }
                    else
                    {
                        MsgHdr = "Error:";
                    }

                    fso.WriteLine("======================================================");
                    fso.WriteLine("Date: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    fso.WriteLine();
                    fso.WriteLine(MsgHdr);
                    fso.WriteLine();
                    fso.WriteLine(msg);
                    fso.WriteLine("======================================================");
                    fso.WriteLine();

                    // Close File Object
                    fso.Close();

                    MsgHdr = null;
                    fso.Dispose();
                    fso = null;
                    logPath = null;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " - Save Log errors captured, please review the error log.");
                // Console.Read();
            }
            finally
            {

            };

        }

        #endregion

        #region Datetime function
        public static bool ConvertToDate(ref DateTime inDateTime, string dateStr, string formatStr, CultureInfo cultureInfo = null, DateTimeStyles dateTimeStyles = DateTimeStyles.None)
        {

            try
            {
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                }

                if (DateTime.TryParseExact(dateStr, formatStr, cultureInfo, dateTimeStyles, out inDateTime))
                {
                    return true;
                }
                else
                {
                    throw new Exception("ConvertToDate: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {

                SaveLog(ex.ToString(), "E");

                return false;
            }


        }

        public static DateTime ConvertToDate(string dateStr, string formatStr, CultureInfo cultureInfo = null, DateTimeStyles dateTimeStyles = DateTimeStyles.None)
        {

            try
            {
                DateTime newDate = DateTime.MinValue;
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                }

                if (DateTime.TryParseExact(dateStr, formatStr, cultureInfo, dateTimeStyles, out newDate))
                {
                    return newDate;
                }
                else
                {
                    throw new Exception("ConvertToDate: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {



                return DateTime.MinValue;
            }


        }

        public static bool ConvertToDateFrmDispaly(ref DateTime inDateTime, string dateStr)
        {
            DateTime tmpDateTime = inDateTime;
            try
            {

                if (ConvertToDate(ref inDateTime, dateStr, "d/M/yyyy", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None))
                {
                    return true;
                }
                else
                {

                    throw new Exception("ConvertToDateFrmDispaly: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {
                inDateTime = tmpDateTime;
                SaveLog(ex.ToString(), "E");

                return false;
            }


        }

        public static DateTime ConvertToDateFrmDispaly(string dateStr)
        {

            try
            {
                DateTime newDate = DateTime.MinValue;
                if (ConvertToDate(ref newDate, dateStr, "d/M/yyyy", CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None))
                {
                    return newDate;
                }
                else
                {

                    throw new Exception("ConvertToDateFrmDispaly: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return DateTime.MinValue;
            }

        }

        public static int CaculateWeekBy2Date(DateTime date1, DateTime date2)
        {
            DateTimeFormatInfo dinfo = DateTimeFormatInfo.CurrentInfo;
            return (dinfo.Calendar.GetWeekOfYear(date2, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday) - dinfo.Calendar.GetWeekOfYear(date1, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday));
        }
        #endregion


        #region UI function

        #endregion

        public static string GetURL(String urlValue)
        {
            string url = string.Empty;
            string pageName = string.Empty;



            if (urlValue.ToString().ToUpper() == "MCS_HOME")
            {
                pageName = "BookingList.aspx";
            }
            else if (urlValue.ToString().ToUpper() == "TIMEOUTHANDLER")
            {
                pageName = "ErrorHandler/ErrorHandler.aspx?error=timeout";
            }
            else if (urlValue.ToString().ToUpper() == "CONNECTIONERRORHANDLER")
            {
                pageName = "ErrorHandler/ErrorHandler.aspx?error=connection";
            }
            else if (urlValue.ToString().ToUpper() == "ERRORHANDLER")
            {
                pageName = "ErrorHandler/ErrorHandler.aspx?error=errorhandler";
            }
            else if (urlValue.ToString().ToUpper() == "MCS_LOGIN")
            {
                pageName = "Default.aspx";
            }



            HttpContext.Current.Response.Redirect(pageName);


            return url;

        }

        public static string GetWebBaseUrl(HttpContext httpContext)
        {
            try
            {
                return httpContext.Request.Url.OriginalString.Replace(httpContext.Request.Url.PathAndQuery, "");
            }
            catch
            {
                return "";
            }
        }

        public static string GetWebPageWithQuery(HttpContext httpContext)
        {
            try
            {
                return httpContext.Request.Url.PathAndQuery.Replace(httpContext.Request.ApplicationPath + "/", "");

            }
            catch
            {
                return "";
            }
        }

        public static string BuildResponseQuery(string url, Dictionary<string, string> dictionary)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return url;
                }
                else
                {
                    string rMsg = "";
                    foreach (string tmpkey in dictionary.Keys)
                    {
                        rMsg = string.Format("{0}#*#{1}", tmpkey, dictionary[tmpkey]);
                    }
                    Encryption encryption = new Encryption();
                    rMsg = HttpUtility.UrlEncode(encryption.EncryptPassword(rMsg));
                    encryption = null;
                    //-----------
                    UriBuilder builder = new UriBuilder(url);
                    var query = HttpUtility.ParseQueryString(builder.Query);
                    url = url.Replace(query.ToString(), "");

                    if (query.AllKeys.Contains("response"))
                    {
                        query["response"] = rMsg;
                    }
                    else
                    {
                        query.Add("response", rMsg);
                    }
                    return (url + query.ToString());
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.ToString(), "E");
                return "";
            }
        }

        public static string RetrieveResponseQuery()
        {
            try
            {
                string retVal = string.Empty;
                retVal = GetRequestString("response", "");
                retVal = HttpUtility.UrlDecode(retVal);
                if (!string.IsNullOrEmpty(retVal))
                {
                    Decryption decryption = new Decryption();
                    retVal = decryption.DecryptPassword(retVal);
                    decryption = null;
                    string[] tmpAry = retVal.Split(new string[] { "#*#" }, StringSplitOptions.None);
                    if (tmpAry.Length == 2)
                    {
                        retVal = string.Format("<h2><font color=red>{0}:</font>{1}<br /></h2>", tmpAry[0], tmpAry[1]);
                    }
                    else
                    {
                        retVal = "";
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                SaveLog(ex.ToString(), "E");
                return "";
            }

        }

        public static bool GetDRValue<T>(DataRow dr, string fieldName, ref T inVar, T defaultVal)
        {

            try
            {
                object retVal;
                if (dr != null && dr[fieldName] != DBNull.Value && dr[fieldName].ToString() != "")
                {
                    retVal = dr[fieldName];

                }
                else
                {
                    retVal = defaultVal;
                    //switch (Type.GetTypeCode(typeof(T)))
                    //{
                    //    case TypeCode.Int16:
                    //    case TypeCode.Int32:
                    //    case TypeCode.Int64:
                    //    case TypeCode.Decimal:
                    //    case TypeCode.Double:
                    //        retVal = 0;
                    //        break;

                    //    case TypeCode.DateTime:
                    //        retVal = DateTime.MinValue;
                    //        break;

                    //    default:
                    //        retVal = "";
                    //        break;
                    //}
                }

                inVar = (T)Convert.ChangeType(retVal, typeof(T));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public static T GetDRValue<T>(DataRow dr, string fieldName, T defaultVal)
        {

            try
            {
                object retVal;
                if (dr != null && dr[fieldName] != DBNull.Value && dr[fieldName].ToString() != "")
                {
                    retVal = dr[fieldName];

                }
                else
                {
                    retVal = defaultVal;
                }

                return (T)Convert.ChangeType(retVal, typeof(T));
            }
            catch (Exception ex)
            {
                return default(T);
            }


        }

        public static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                }
            }

            return res.ToString();
        }

        public static string GetRequestString(string reqName, string defVal = "")
        {

            if (HttpContext.Current.Request.Form.AllKeys.Contains(reqName))
            {
                return HttpContext.Current.Request.Form[reqName].ToString();
            }
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains(reqName))
            {
                return HttpContext.Current.Request.QueryString[reqName].ToString();
            }
            else
            {
                return "";
            }
        }

        public static string GetRequestString(HttpRequest request, string reqName, string defVal = "")
        {

            if (request.Form.AllKeys.Contains(reqName))
            {
                return request.Form[reqName].ToString();
            }
            else if (request.QueryString.AllKeys.Contains(reqName))
            {
                return request.QueryString[reqName].ToString();
            }
            else
            {
                return "";
            }
        }

        public static int GetRequestInt(string reqName, int defVal = 0)
        {
            int retVal = defVal;
            if (HttpContext.Current.Request.Form.AllKeys.Contains(reqName))
            {
                Int32.TryParse(HttpContext.Current.Request.Form[reqName].ToString(), out retVal);
            }
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains(reqName))
            {
                Int32.TryParse(HttpContext.Current.Request.QueryString[reqName].ToString(), out retVal);
            }

            return retVal;
        }

        public static int GetRequestInt(HttpRequest request, string reqName, int defVal = 0)
        {
            int retVal = defVal;
            if (request.Form.AllKeys.Contains(reqName))
            {
                Int32.TryParse(request.Form[reqName].ToString(), out retVal);
            }
            else if (request.QueryString.AllKeys.Contains(reqName))
            {
                Int32.TryParse(request.QueryString[reqName].ToString(), out retVal);
            }

            return retVal;
        }


        public static JObject GetRequestJSON(HttpRequest request)
        {
            try
            {
                using (Stream stream = request.InputStream)
                {
                    stream.Position = 0;
                    using (StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        using (DBHelper dbHelper = new DBHelper())
                        {
                            string jsonPostData = reader.ReadToEnd();
                            if (string.IsNullOrEmpty(jsonPostData))
                            {
                                return null;
                            }
                            else
                            {
                                return JObject.Parse(jsonPostData);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string GetBookingDetailScript()
        {
            string filename = "~/js/BookingDetailScript.tpl";
            string path = HttpContext.Current.Server.MapPath(filename);
            string content = System.IO.File.ReadAllText(path);

            return content;
        }

        public static bool IsValidEmail(string emailAddr)
        {
            try
            {
                if (string.IsNullOrEmpty(emailAddr) || string.IsNullOrWhiteSpace(emailAddr))
                {
                    return false;
                }
                else
                {
                    return false;
                    //return new EmailAddressAttribute().IsValid(emailAddr);
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex.ToString(), "E");
                return false;
            }
        }

        /// <summary>
        /// DataTable 转换为 Html
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetHtmlString(DataTable dt,string htmlName="AMS")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html><head>");
            sb.Append("<title></title>");
            sb.Append("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/> ");
            sb.Append("<style type=text/css>");
            sb.Append("td{font-size: 9pt;border:solid 1 #000000;width:200px;}");
            sb.Append("table{padding:3 0 3 0;border:solid 1 #000000;margin:0 0 0 0;BORDER-COLLAPSE: collapse;}");
            sb.Append(".button { width: 200px;  padding:8px;  background-color: #428bca;  border-color: #357ebd;  " +
                "color: #fff;  -moz-border-radius: 10px;  -webkit-border-radius: 10px;  border-radius: 10px; " +
                "-khtml-border-radius: 10px;text-align: center;   vertical-align: middle;   border: 1px solid transparent;  " +
                "font-weight: 900;  font-size:125% }   ");
            sb.Append("</style>");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append("<div> <i>To whom may concern: </i>  </div>");
            sb.Append("<table cellSpacing='0' cellPadding='0' width ='1300px' border='1'>");
            sb.Append("<tr style='background:#03a9f4; vertical-align:middle;'>");
            sb.Append("<td><b></b></td>");
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append("<td><b><span>" + column.ColumnName + "</span></b></td>");
            }
            sb.Append("</tr>");
            int iColsCount = dt.Columns.Count;
            int rowsCount = dt.Rows.Count - 1;
            for (int j = 0; j <= rowsCount; j++)
            {
                sb.Append("<tr>");
                sb.Append("<td style='width:30px;display:table-cell; vertical-align:middle;' >" + ((int)(j + 1)).ToString() + "</td>");
                for (int k = 0; k <= iColsCount - 1; k++)
                {
                    sb.Append("<td style='width:120px;'>");
                    object obj = dt.Rows[j][k];
                    if (obj == DBNull.Value)
                    {
                        obj = " ";//如果是NULL则在HTML里面使用一个空格替换之
                    }
                    if (obj.ToString() == "")
                    {
                        obj = " ";
                    }
                    string strCellContent = obj.ToString().Trim();
                    sb.Append("<span>" + strCellContent + "</span>");
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("</br>");
            sb.Append("<div> <i>Authorized "+ htmlName + " monitor can  synchronize remarks in TBS page via below link </i>  </div>");
            sb.Append("<div> <p><a href='https://www.topocean.com/WebBooking/toTBS.asp?targetPage="+ htmlName +"FilingCheck.aspx'>" + htmlName + " Filing Check</a></p> </div>");

            //点击单元格 输出 行和列
            sb.Append("<script src='https://cdn.bootcss.com/jquery/1.12.4/jquery.min.js'></script>");
            sb.Append("<script type='text/javascript'>");
            sb.Append("$('table tbody').on('click', 'td', function (e) {");
            sb.Append("var row = $(this).parent().prevAll().length-1 ;");
            sb.Append("var column = $(this).prevAll().length-1 ;");
            sb.Append("var str = 'dt.Rows[' + row + '][' + column + '].ToString()';");
            sb.Append("console.log(str);alert(str);");
            sb.Append("});");
            sb.Append("</script>");

            sb.Append("</body></html>");
            return sb.ToString();
        }

        public static string GetHtmlString(string strText)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<html>" +
                      "<head>");
            sb.Append("<title></title>");
            sb.Append("<meta http-equiv='Content-Type' content='text/html; charset=UTF-8'/> ");
            sb.Append("</head>");
            sb.Append("<body>");
            sb.Append(" <div>" + strText + "</div>");
            sb.Append("</body>" +
                      "</html>");
            return sb.ToString();
        }

        public static string ExportDatatableToHtml(DataTable dt)
        {
            StringBuilder strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:12px'>");

            strHTMLBuilder.Append("<tr >");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td >");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</td>");

            }
            strHTMLBuilder.Append("</tr>");


            foreach (DataRow myRow in dt.Rows)
            {

                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");

                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags.  
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");

            string Htmltext = strHTMLBuilder.ToString();

            return Htmltext;

        }

        public static bool IsEmpty(Object str)
        {
            return str != null && !"".Equals(str) && !Convert.IsDBNull(str);
        }

        public static bool IsNotEmpty(string str)
        {
            return str != null && !String.IsNullOrEmpty(str);
        }

        public static string ReplaceWrap(Object str)
        {
            return CheckEmpty(str).Replace("\x0A", "<br/>").Replace("\x0D", "<br/>").Replace(" ", "&nbsp;");
        }

        public static string CheckEmpty(Object str)
        {
            string ret = String.Empty;

            if (IsEmpty(str))
            {
                ret = str.ToString().Trim();
            }

            return ret;
        }
    }
}
