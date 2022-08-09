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


        public static double CheckNumber(Object str)
        {
            double ret = 0;
            //try
            //{
            //    ret = double.Parse(CheckEmpty(str));
            //}
            //catch (Exception)
            //{
            //    ret = 0;
            //}

            if (double.TryParse(CheckEmpty(str), out ret))
            {
                return ret;
            }

            return ret;
        }

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
        public static string GetHtmlString(DataTable dt, string htmlName = "AMS")
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
            sb.Append("<div> <i>Authorized " + htmlName + " monitor can  synchronize remarks in TBS page via below link </i>  </div>");
            sb.Append("<div> <p><a href='https://www.topocean.com/WebBooking/toTBS.asp?targetPage=" + htmlName + "FilingCheck.aspx'>" + htmlName + " Filing Check</a></p> </div>");

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

        public static DataTable GenerateTBSData(string arrMBL)
        {
            string sql = String.Format(

                                                 //" IF OBJECT_ID('tempdb..#TEMP_MAPPING_CNEE') IS NOT NULL                                              						" +
                                                 // //"     BEGIN	                                                                                                                " +
                                                 // "         DROP TABLE #TEMP_MAPPING_CNEE	                                                                                    " +
                                                 // //"     END	                                                                                                                " +
                                                 // " 	                                                                                                                        " +
                                                 // " CREATE TABLE #TEMP_MAPPING_CNEE	                                                                                        " +
                                                 // " (	                                                                                                                        " +
                                                 // "     CNEE  VARCHAR(100),	                                                                                                " +
                                                 // "     SALES VARCHAR(20)	                                                                                                    " +
                                                 // " )	                                                                                                                        " +
                                                 // " 	                                                                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('COMPASS HEALTH BRANDS', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('WOODSTREAM CORP', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('DRIVE DEVILBISS HEALTHCARE INC.', 'GLOBAL')	                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('BIOWORLD MERCHANDISING INC', 'GLOBAL')	                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('MONOPRICE, INC.', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('NOLAN ORIGINALS LLC', 'GLOBAL')	                                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('UTC CCS CARLYLE', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('CARRIER CORPORATION', 'GLOBAL')	                                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('LOZIER CORPORATION-IN', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('BIOWORLD CANADA', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FELLOWES,INC.', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('DRIVE DEVILBISS HEALTHCARE LTD - UK', 'GLOBAL')	                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('MEDICAL DEPOT INC DBA DRIVE DEVILBISS HEALTHCARE', 'GLOBAL')	                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('SOGESMA', 'GLOBAL')	                                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('PRENATAL RETAIL GROUP SPA', 'GLOBAL')	                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('UTC CARRIER CORP', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FELLOWES,INC. CANADA LTD', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('MONOPRICE INC', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('LOZIER CORPORATION', 'GLOBAL')	                                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('SNAVELY INTERNATIONAL', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('WOODSTREAM', 'GLOBAL')	                                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('WORLDWIDE ELECTRIC CORPORATION', 'GLOBAL')	                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('DRIVE FRANCE', 'GLOBAL')	                                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FLAMINGO PET PRODUCTS NV', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FOX RUN CANADA CORP', 'GLOBAL')	                                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FELLOWES CANADA LTD', 'GLOBAL')	                                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FELLOWES INC.', 'GLOBAL')	                                                                                    " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('JEMELLA AUSTRALIA PTY LTD C/O CEVA LOGISTICS', 'GLOBAL')	                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('GHD NORTH AMERICA LLC', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FELLOWES INC DBA ESI', 'GLOBAL')	                                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('TECHNICOLOR DELIVERY TECHNOLOGIES FM POLSKA SP Z O', 'GLOBAL')	                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('BIOWORLD MERCHANDISING LTD', 'GLOBAL')	                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('PRENATAL RETAIL GROUP SPA TEXTILE', 'GLOBAL')	                                                                " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('PIKDARE SPA', 'GLOBAL')	                                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('ARTSANA UK', 'GLOBAL')	                                                                                        " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('ARTSANA SPAIN, S.A.U.', 'GLOBAL')	                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('SPANX', 'GLOBAL')	                                                                                            " +
                                                 // " INSERT INTO #TEMP_MAPPING_CNEE (CNEE, SALES)	                                                                            " +
                                                 // " VALUES ('FOX RUN CRAFTSMEN', 'GLOBAL');	                                                                                " +
                                                 " 	                                                                                                                        " +
                                                 " 	                                                                                                                        " +
                                                 " WITH TEMP AS (	                                                                                                        " +
                                                 "     Select [Week],	                                                                                                    " +
                                                 "            [Booking ID],	                                                                                                " +
                                                 "            [Booking Date],	                                                                                            " +
                                                 "            [Booking Status],	                                                                                            " +
                                                 "            [Transportation Mode],	                                                                                        " +
                                                 "            [Created By],	                                                                                                " +
                                                 "            [Handling User Email],	                                                                                        " +
                                                 "            [Origin Office],	                                                                                            " +
                                                 "            [Dest Office],	                                                                                                " +
                                                 "            Principal,	                                                                                                    " +
                                                 "            Consignee,	                                                                                                    " +
                                                 "            Vendor,	                                                                                                    " +
                                                 "            [Place Of Receipt],	                                                                                        " +
                                                 "            Carrier,	                                                                                                    " +
                                                 "            [Contract Type],	                                                                                            " +
                                                 "            [Carrier Commodity],	                                                                                        " +
                                                 "            Vessel,	                                                                                                    " +
                                                 "            Voyage,	                                                                                                    " +
                                                 "            MBL,	                                                                                                        " +
                                                 "            HBL,	                                                                                                        " +
                                                 "            [Delivery Type],	                                                                                            " +
                                                 "            [PO Ready Date],	                                                                                            " +
                                                 "            [POL],	                                                                                                        " +
                                                 "            [Original ETD],	                                                                                            " +
                                                 "            [Current ETD],	                                                                                                " +
                                                 "            ATD,	                                                                                                        " +
                                                 "            [POD],	                                                                                                        " +
                                                 "            [Original ETA],	                                                                                            " +
                                                 "            [Current ETA],	                                                                                                " +
                                                 "            [Final Dest],	                                                                                                " +
                                                 "            [Final ETA],	                                                                                                " +
                                                 "            [Dest Ramp],	                                                                                                " +
                                                 "            [Approval Status],	                                                                                            " +
                                                 "            [Approval Date],	                                                                                            " +
                                                 "            [Approval Remark],	                                                                                            " +
                                                 "            [ISF Sent Date],	                                                                                            " +
                                                 "            [Customs Response #],	                                                                                        " +
                                                 "            [Forwarder Remarks],	                                                                                        " +
                                                 "            [Nomination Office],	                                                                                        " +
                                                 "            [Nomination Sales],	                                                                                        " +
                                                 "            [IsTBS],	                                                                                                    " +
                                                 "            [Release Type]                                                                                                " +
                                                 "     From (Select CASE	                                                                                                    " +
                                                 "                      WHEN POTracing.ISTBS = 'Y' THEN ISNULL(POTracing.WB_WeekOfYear, 0)	                                " +
                                                 "                      ELSE [dbo].[udf_GetTBSWeek](POTracing.OriginalVesselETD) END as [Week],	                            " +
                                                 "                  POTracing.BookingDate                                            as [Booking Date],	                    " +
                                                 "                  '' + POTracing.BookingReqID + ''                                 as [Booking ID],	                    " +
                                                 "                  UPPER(POTracing.WB_STATUS)                                       as [Booking Status],	                " +
                                                 "                  POTracing.TransportationMode                                     As [Transportation Mode],	            " +
                                                 "                  ISNULL(m.ModifiedBy, '')                                         as [Created By],	                    " +
                                                 "                  ISNULL(max(lv.UserEmail), '')                                    as [Handling User Email],	            " +
                                                 "                  ISNULL(POTracing.OriginOffice, '')                               as [Origin Office],	                    " +
                                                 "                  ISNULL(POTracing.Dest, '')                                       as [Dest Office],	                    " +
                                                 "                  principal.Company                                                as [Principal],	                        " +
                                                 "                  POTracing.CNEE                                                   as [Consignee],	                        " +
                                                 "                  POTracing.Vendor                                                 as [Vendor],	                        " +
                                                 "                  POTracing.Orig                                                   as [Place Of Receipt],	                " +
                                                 "                  POTracing.Carrier                                                as [Carrier],	                        " +
                                                 "                  ISNULL(POTracing.BookingContractType, '')                        As [Contract Type],	                    " +
                                                 "                  POTracing.Commodity2                                             As [Carrier Commodity],	                " +
                                                 "                  POTracing.Vessel                                                 as [Vessel],	                        " +
                                                 "                  POTracing.Voyage                                                 as [Voyage],	                        " +
                                                 "                  POTracing.MBL                                                    as [MBL],	                            " +
                                                 "                  POTracing.HBL                                                    as [HBL],	                            " +
                                                 "                  POTracing.DeliveryType                                           as [Delivery Type],	                    " +
                                                 "                  POTracing.POReadyDate                                            As [PO Ready Date],	                    " +
                                                 "                  POTracing.LoadPort                                               as [POL],	                            " +
                                                 "                  POTracing.OriginalP2PETD                                         As [Original ETD],	                    " +
                                                 "                  POTracing.P2PETD                                                 As [Current ETD],	                    " +
                                                 "                  POTracing.P2PATD                                                 As [ATD],	                            " +
                                                 "                  POTracing.DischPort                                              as [POD],	                            " +
                                                 "                  POTracing.OriginalP2PETA                                         As [Original ETA],	                    " +
                                                 "                  POTracing.P2PETA                                                 As [Current ETA],	                    " +
                                                 "                  POTracing.FinalDest                                              as [Final Dest],	                    " +
                                                 "                  POTracing.D2DETA                                                 As [Final ETA],	                        " +
                                                 "                  POTracing.DestRamp                                               as [Dest Ramp],	                        " +
                                                 "                  POTracing.WBApprovalStatus                                       As [Approval Status],	                " +
                                                 "                  POTracing.WBApprovalDate                                         As [Approval Date],	                    " +
                                                 "                  POTracing.WBApprovalRemark                                       As [Approval Remark],	                " +
                                                 "                  POTracing.ISFSentDate                                            As [ISF Sent Date],	                    " +
                                                 "                  POTracing.CustomsResponseNo                                      As [Customs Response #],	            " +
                                                 "                  POTracing.BookingInstruction                                     As [Forwarder Remarks],	                " +
                                                 "                  ISNULL(principal.NomName, '')                                    as [Nomination Office],	                " +
                                                 "                  ISNULL(principal.NomSales, '')                                   as [Nomination Sales],	                " +
                                                 "                  ISNULL(POTracing.IsTBS, 'N')                                     as [IsTBS],	                            " +
                                                 "                  ISNULL(POTracing.ReleaseType, 'N/A')                             as [Release Type]	                            " +
                                                 "           FROM POTracing	                                                                                                " +
                                                 "                    LEFT JOIN Customer principal ON principal.uID = POTracing.Principle	                                " +
                                                 "                    LEFT JOIN OptionList agent ON agent.OptValue = POTracing.Dest AND agent.OptType = 'TitanOffice' AND    " +
                                                 "                                                  agent.IsActive = 'Y' AND Criteria3 = 'Y' AND                             " +
                                                 "                                                  (Criteria5 <> '' OR Criteria5 IS NOT NULL)                               " +
                                                 "                    LEFT JOIN Customer non_sea_trans ON non_sea_trans.Company = POTracing.CNEE                             " +
                                                 "                    LEFT JOIN LoginView lv ON lv.UserLoginName = POTracing.HandlingUser                                    " +
                                                 "                    LEFT JOIN CNEECarrierContract contract ON contract.uID = POTracing.CNEECarrierContractID               " +
                                                 "                    OUTER APPLY (Select Top 1 tm.TRAFFIC                                                                   " +
                                                 "                                 From TrafficMap tm                                                                        " +
                                                 "                                          Join LocationMgr traffic On tm.Country_code = traffic.CountryCode                " +
                                                 "                                 Where traffic.Location = POTracing.FinalDest) tm                                          " +
                                                 "                    OUTER APPLY (Select Top 1 ModifiedBy                                                                   " +
                                                 "                                 From ModifyRecord m                                                                       " +
                                                 "                                 Where CargoId = POTracing.uID                                                             " +
                                                 "                                   and m.modifiedBy <> ''                                                                  " +
                                                 "                                 Order By ActionDate asc) m                                                                " +
                                                 "           WHERE 1 = 1                                                                                                     " +
                                                 "             AND (POTracing.BookingContractType IN ('TOPOCEAN', 'NVO/NON-TOPOCEAN CONTRACT') Or POTracing.IsTBS = 'Y')     " +
                                                 "             And POTracing.CNEE Not In ('ABC COMPUTER', 'ABC COMPUTER-TBS', 'ABC COMP EU')                                 " +
                                                 "             And ISNULL(POTracing.WB_Status, '') NOT IN ('REJECTED', 'CANCELLED')                                          " +
                                                 "             AND POTracing.BookingReqID IS NOT NULL                                                                        " +
                                                 //"             AND POTracing.Traffic In ('-1', 'USA', 'CAN')                                                                 " +
                                                 //"             AND (POTracing.P2PETD between '06/01/2022' and '07/20/2022')                                                  " +
                                                 //"             AND POTracing.TransportationMode In ('-1', 'SEA')                                                             " +
                                                 "             AND (POTracing.BookingReqID <> '')                                                                            " +
                                                 "             AND POTracing.MBL <> ''                                                                                       " +
                                                 "             AND  POTracing.MBL IN ('{0}')                                                                                   " +
                                                 "           GROUP BY POTracing.WB_WeekOfYear, POTracing.uid, POTracing.CustomsResponseNo, POTracing.CNTRType,               " +
                                                 "                    POTracing.CNTRType2, POTracing.CNTRType3, POTracing.CNTRType4, POTracing.CNTRQty, POTracing.CNTRQty2,  " +
                                                 "                    POTracing.CNTRQty3, POTracing.CNTRQty4, POTracing.TransportationMode, POTracing.OriginOffice,          " +
                                                 "                    POTracing.Dest, POTracing.CNEE, POTracing.Vendor, m.ModifiedBy, agent.Criteria5, principal.Company,    " +
                                                 "                    principal.NomName, principal.NomSales, POTracing.BookingContractType, contract.ContractNo,             " +
                                                 "                    POTracing.BookingConfirmation, POTracing.BookingDate, POTracing.BookingReqID, POTracing.WB_STATUS,     " +
                                                 "                    POTracing.DeliveryType, POTracing.Carrier, POTracing.MBL, POTracing.HBL, POTracing.Vessel,             " +
                                                 "                    POTracing.Voyage, POTracing.CarrierDestType, POTracing.CarrierDest, POTracing.LoadPort,                " +
                                                 "                    POTracing.POReadyDate, POTracing.P2PETD, POTracing.OriginalVesselETD, POTracing.OriginalP2PETD,        " +
                                                 "                    POTracing.P2PATD, POTracing.DischPort, POTracing.P2PETA, POTracing.D2DETA, POTracing.OriginalP2PETA,   " +
                                                 "                    POTracing.DestRamp, POTracing.FinalDest, POTracing.Description, POTracing.Commodity2, POTracing.Orig,  " +
                                                 "                    POTracing.WBApprovalStatus, POTracing.WBApprovalDate, POTracing.WBApprovalRemark,                      " +
                                                 "                    POTracing.ISFSentDate,                                                                                 " +
                                                 "                    POTracing.Comments, POTracing.BookingInstruction, POTracing.ISTBS,                                     " +
                                                 "                    POTracing.loadedOnMotherVesselFlag,POTracing.ReleaseType) a)                                                                 " +
                                                 "                                                                                                                           " +
                                                 "                                                                                                                           " +
                                                 " SELECT   DISTINCT    MBL                                                                                                  " +
                                                 "               , MAX([Created By]) AS [Created By]                                                                         " +
                                                 "               , CONVERT(VARCHAR(10), [Current ETD], 111)   AS [ETD]                                                       " +
                                                 "               , MAX([Handling User Email]) AS [Handling User Email]                                                       " +
                                                 "               , MAX([Nomination Office]) AS [Nomination Office]                                                           " +
                                                 "               , CASE WHEN MAX(Country) = 'CHINA' THEN MAX([Nomination Sales]) ELSE 'N/A' END AS [PIC(销售）]              " +
                                                 "               , MAX([Consignee]) AS [Consignee]                                                                           " +
                                                 "               , MAX([Release Type]) AS [Release Type]                                                                           " +
                                                 " FROM TEMP                                                                                                                 " +
                                                 //"          LEFT JOIN tempdb...#TEMP_MAPPING_CNEE TEMP_CNEE ON TEMP_CNEE.CNEE = TEMP.[Consignee]                                " +
                                                 " LEFT JOIN Port ON TEMP.[Nomination Office] = PORTNAME AND IsActive = 'Y' AND ORG = 'Y'                                     " +
                                                 "  GROUP BY MBL,[Current ETD]                          "
                                                 , arrMBL);

            DataTable dt = dbHelper.ExecDataTable(sql);

            return dt;
        }


        public static string[] getCNEEList()
        {
            string[] cneeList = new string[] {
                 "WOODSTREAM CORP"
                ,"DRIVE DEVILBISS HEALTHCARE INC."
                ,"BIOWORLD MERCHANDISING INC"
                ,"MONOPRICE, INC."
                ,"NOLAN ORIGINALS LLC"
                ,"UTC CCS CARLYLE"
                ,"CARRIER CORPORATION"
                ,"LOZIER CORPORATION-IN"
                ,"BIOWORLD CANADA"
                ,"FELLOWES,INC."
                ,"DRIVE DEVILBISS HEALTHCARE LTD - UK"
                ,"DRIVE DEVILBISS HEALTHCARE"
                ,"MEDICAL DEPOT INC DBA DRIVE DEVILBISS HEALTHCARE"
                ,"SOGESMA"
                ,"PRENATAL RETAIL GROUP SPA"
                ,"UTC CARRIER CORP"
                ,"FELLOWES,INC. CANADA LTD"
                ,"MONOPRICE INC"
                ,"LOZIER CORPORATION"
                ,"SNAVELY INTERNATIONAL"
                ,"WOODSTREAM"
                ,"WORLDWIDE ELECTRIC CORPORATION"
                ,"DRIVE FRANCE"
                ,"WORLDWIDE ELECTRIC CORPORATION"
                ,"FLAMINGO PET PRODUCTS NV"
                ,"FOX RUN CANADA CORP"
                ,"FELLOWES CANADA LTD"
                ,"FELLOWES INC."
                ,"JEMELLA AUSTRALIA PTY LTD C/O CEVA LOGISTICS"
                ,"GHD NORTH AMERICA LLC"
                ,"FELLOWES INC DBA ESI"
                ,"TECHNICOLOR DELIVERY TECHNOLOGIES FM POLSKA SP Z O"
                ,"BIOWORLD MERCHANDISING LTD"
                ,"PRENATAL RETAIL GROUP SPA TEXTILE"
                ,"PIKDARE SPA"
                ,"ARTSANA UK"
                ,"ARTSANA SPAIN, S.A.U."
                ,"SPANX"
                ,"FOX RUN CRAFTSMEN"
             };
            return cneeList;
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

        /// <summary>
        ///     Converts a column in a DataTable to another type using a user-defined converter function.
        /// </summary>
        /// <param name="dt">The source table.</param>
        /// <param name="columnName">The name of the column to convert.</param>
        /// <param name="valueConverter">Converter function that converts existing values to the new type.</param>
        /// <typeparam name="TTargetType">The target column type.</typeparam>
        private static void ConvertColumnTypeTo<TTargetType>(this DataTable dt, string columnName, Func<object, TTargetType> valueConverter)
        {
            var newType = typeof(TTargetType);

            DataColumn dc = new DataColumn(columnName + "_new", newType);

            // Add the new column which has the new type, and move it to the ordinal of the old column
            int ordinal = dt.Columns[columnName].Ordinal;
            dt.Columns.Add(dc);
            dc.SetOrdinal(ordinal);

            // Get and convert the values of the old column, and insert them into the new
            foreach (DataRow dr in dt.Rows)
            {
                dr[dc.ColumnName] = valueConverter(dr[columnName]);
                //dr[dc.ColumnName] = dr[columnName] == DBNull.Value ? DBNull.Value : valueConverter(dr[columnName]);
            }

            // Remove the old column
            dt.Columns.Remove(columnName);

            // Give the new column the old column's name
            dc.ColumnName = columnName;
        }

        private static void ConvertColumnType(this DataTable dt, string columnName, Type newType)
        {
            using (DataColumn dc = new DataColumn(columnName + "_new", newType))
            {
                // Add the new column which has the new type, and move it to the ordinal of the old column
                int ordinal = dt.Columns[columnName].Ordinal;
                dt.Columns.Add(dc);
                dc.SetOrdinal(ordinal);

                // Get and convert the values of the old column, and insert them into the new
                foreach (DataRow dr in dt.Rows)
                {
                    dr[dc.ColumnName] = dr[columnName] == DBNull.Value || !IsEmpty(dr[columnName]) ? DBNull.Value : Convert.ChangeType(dr[columnName], newType);
                }
                   
                // Remove the old column
                dt.Columns.Remove(columnName);

                // Give the new column the old column's name
                dc.ColumnName = columnName;
            }
        }

        public static void RenderColumn(DataTable dt)
        {
            

            dt.Columns.Remove("EXCHANGE_RATE");
            dt.Columns["AC_CODE"].ColumnName = "A/C CODE";
            dt.Columns["BILLNO"].ColumnName = "中國發票號碼.";
            dt.Columns["INVOICE_NO"].ColumnName = "INVOICE NO.";
            dt.Columns["HBL"].ColumnName = "HOUSE BL";
            dt.Columns["MBL"].ColumnName = "MASTER BL";
            dt.Columns["ORIGINAL_AMOUNT"].ColumnName = "ORIGINAL AMOUNT";
            dt.Columns["ISSUE_DATE"].ColumnName = "ISSUE DATE";
            dt.Columns["EQUIVANLENT_AMOUNT"].ColumnName = "EQUIVANLENT AMOUNT";
            //dt.Columns["EXCHANGE_RATE"].ColumnName = "EXCHANGE RATE";
            dt.Columns["OUTSTANDING_AMOUNT"].ColumnName = "OUTSTANDING AMOUNT";
            dt.Columns["PAYMENT_TERMS"].ColumnName = "PAYMENT TERMS";
            dt.Columns["NOMINATION_OFFICE"].ColumnName = "NOMINATION OFFICE";
            dt.Columns["PCI_SALES"].ColumnName = "PIC(销售）";
            dt.Columns["TIP"].ColumnName = "是否有协议";
            dt.Columns["Release_Type"].ColumnName = "Release Type";

        }

        public static void changeColumnType(DataTable dt)
        {
            dt.ConvertColumnType("ISSUE_DATE", typeof(DateTime));
            dt.ConvertColumnType("ETA", typeof(DateTime));
            dt.ConvertColumnType("ETD", typeof(DateTime));
            dt.ConvertColumnType("ORIGINAL_AMOUNT", typeof(decimal));
            dt.ConvertColumnType("EQUIVANLENT_AMOUNT", typeof(decimal));
            dt.ConvertColumnType("OUTSTANDING_AMOUNT", typeof(decimal));
            dt.ConvertColumnType("WEEK", typeof(int));
        }

        /// <summary>
        /// 建立两内存表的链接
        /// </summary>
        /// <param name="dt1">左表（主表）</param>
        /// <param name="dt2">右表</param>
        /// <param name="FJC">左表中关联的字段名（字符串）</param>
        /// <param name="SJC">右表中关联的字段名（字符串）</param>
        /// <returns></returns>
        public static DataTable Join(DataTable dt1, DataTable dt2, DataColumn[] FJC, DataColumn[] SJC)
        {
            //创建一个新的DataTable
            DataTable table = new DataTable("Join");

            // Use a DataSet to leverage DataRelation
            using (DataSet ds = new DataSet())
            {
                //把DataTable Copy到DataSet中
                ds.Tables.AddRange(new DataTable[] { dt1.Copy(), dt2.Copy() });

                DataColumn[] First_columns = new DataColumn[FJC.Length];
                for (int i = 0; i < First_columns.Length; i++)
                {
                    First_columns[i] = ds.Tables[0].Columns[FJC[i].ColumnName];
                }

                DataColumn[] Second_columns = new DataColumn[SJC.Length];
                for (int i = 0; i < Second_columns.Length; i++)
                {
                    Second_columns[i] = ds.Tables[1].Columns[SJC[i].ColumnName];
                }

                //创建关联
                DataRelation r = new DataRelation(string.Empty, First_columns, Second_columns, false);
                ds.Relations.Add(r);

                //为关联表创建列
                for (int i = 0; i < dt1.Columns.Count; i++)
                {
                    table.Columns.Add(dt1.Columns[i].ColumnName, dt1.Columns[i].DataType);
                }

                for (int i = 0; i < dt2.Columns.Count; i++)
                {
                    //看看有没有重复的列，如果有在第二个DataTable的Column的列明后加_Second
                    if (!table.Columns.Contains(dt2.Columns[i].ColumnName))
                        table.Columns.Add(dt2.Columns[i].ColumnName, dt2.Columns[i].DataType);
                    else
                        table.Columns.Add(dt2.Columns[i].ColumnName + "_Second", dt2.Columns[i].DataType);
                }

                table.BeginLoadData();
                int itable2Colomns = ds.Tables[1].Rows[0].ItemArray.Length;
                foreach (DataRow firstrow in ds.Tables[0].Rows)
                {
                    //得到行的数据
                    DataRow[] childrows = firstrow.GetChildRows(r);//第二个表关联的行
                    if (childrows != null && childrows.Length > 0)
                    {
                        object[] parentarray = firstrow.ItemArray;
                        foreach (DataRow secondrow in childrows)
                        {
                            object[] secondarray = secondrow.ItemArray;
                            object[] joinarray = new object[parentarray.Length + secondarray.Length];
                            Array.Copy(parentarray, 0, joinarray, 0, parentarray.Length);
                            Array.Copy(secondarray, 0, joinarray, parentarray.Length, secondarray.Length);
                            table.LoadDataRow(joinarray, true);
                        }

                    }
                    else//如果有外连接(Left Join)添加这部分代码
                    {
                        object[] table1array = firstrow.ItemArray;//Table1
                        object[] table2array = new object[itable2Colomns];
                        object[] joinarray = new object[table1array.Length + itable2Colomns];
                        Array.Copy(table1array, 0, joinarray, 0, table1array.Length);
                        Array.Copy(table2array, 0, joinarray, table1array.Length, itable2Colomns);
                        table.LoadDataRow(joinarray, true);
                        DataColumn[] dc = new DataColumn[2];
                        dc[0] = new DataColumn("");
                    }
                }
                table.EndLoadData();
            }
            return table;//***在此处打断点，程序运行后点击查看即可观察到结果
        }

        /// <summary>
        /// 重载1
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="FJC"></param>
        /// <param name="SJC"></param>
        /// <returns></returns>
        public static DataTable Join(DataTable dt1, DataTable dt2, DataColumn FJC, DataColumn SJC)
        {
            return Join(dt1, dt2, new DataColumn[] { FJC }, new DataColumn[] { SJC });
        }

        /// <summary>
        /// 重载2
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <param name="FJC"></param>
        /// <param name="SJC"></param>
        /// <returns></returns>
        public static DataTable Join(DataTable dt1, DataTable dt2, string FJC, string SJC)
        {
            return Join(dt1, dt2, new DataColumn[] { dt1.Columns[FJC] }, new DataColumn[] { dt1.Columns[SJC] });
        }
    }
}
