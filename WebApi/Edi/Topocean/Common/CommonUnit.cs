using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Common
{
    public static class CommonUnit
    {
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



        public static int ConvertToInt(Object str)
        {
            int i;
            if (IsEmpty(str))
            {
                int.TryParse(CheckEmpty(str), out i);
            }
            else
            {
                i = 0;
            }
            return i;
        }

        public static string retDBStr(Object str)
        {
            string ret = String.Empty;

            try
            {
                ret = CheckEmpty(str) == "" ? string.Format(@"null") : "'" + str + "'";
            }
            catch (Exception e)
            {
                ret = "";
            }

            return ret;

        }

        public static string setResponseLog(int code, string type, string message, string trace_id = null)
        {
            return setResJson<object>(code, type, message, null, trace_id);
        }

        public static string setResJson<T>(int code, string type, string message, T body, string trace_id = null)
        {
            BaseRes<T> baseResr = new BaseRes<T>();
            baseResr.res = new Res<T>() { code = code, type = type, message = message, trace_id = trace_id, body = body };
            return JsonConvert.SerializeObject(baseResr);
        }

        public static BaseRes<object> ModelRerurnRes(int code, string type, string message, string trace_id = null)
        {
            BaseRes<object> baseResr = new BaseRes<object>();
            baseResr.res = new Res<object>() { code = code, type = type, message = message, trace_id = trace_id };
            return baseResr;
        }

        /// <summary>
        /// Get client IP address
        /// </summary>
        /// <returns>If it fails, return the loopback address</returns>
        public static string GetIP()
        {
            //If the client uses a proxy server, use HTTP_X_FORWARDED_FOR to find the client IP address
            string userHostAddress = CheckEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) == null ? null : CheckEmpty(HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]).Split(',')[0];
            //Otherwise, directly read REMOTE_ADDR to obtain the client IP address
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            //If the first two fail, use the Request.UserHostAddress property to obtain the IP address,
            //but at this time, it is impossible to determine whether the IP is the client IP or the proxy IP.
            if (string.IsNullOrEmpty(userHostAddress))
            {
                userHostAddress = HttpContext.Current.Request.UserHostAddress;
            }
            //Finally, judge whether the acquisition is successful, and check the format of the IP address (checking its format is very important)
            if (!string.IsNullOrEmpty(userHostAddress) && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// Check the IP address format
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static string DateTimeFormat(string str)
        {
            string ret = String.Empty;

            if (IsEmpty(str))
            {
                ret = str.Substring(0, 4) + "-" +
                      str.Substring(4, 2) + "-" +
                      str.Substring(6, 2) + " " +
                      str.Substring(8, 2) + ":00:00";
            }

            return ret;
        }

        public static void InsertEDIArchive(EDIArchiveModel model)
        {
            string insSQL = String.Empty;
            try
            {
                if (model != null)
                {
                    insSQL = String.Format(" INSERT INTO EDIArchive (InputDate,EDIType,EDIContent,DataContent,ICN) VALUES (getDate(),{0},{1},{2},{3}) ",
                          CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.EDIType))
                        , CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.EDIContent))
                        , CommonUnit.retDBStr(String2Json(CommonUnit.CheckEmpty(model.DataContent)))
                        , CommonUnit.retDBStr(String2Json(CommonUnit.CheckEmpty(model.ICN)))
                    );

                    DBHelper dbHelper = new DBHelper();
                    dbHelper.ExecuteQuery(CommandType.Text, insSQL);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }

        }

        public static void InsertReportDataQueue(ReportDataQueueModel model)
        {
            string insSQL = String.Empty;
            try
            {
                if (model != null)
                {
                    insSQL = String.Format(" INSERT INTO ReportDataQueue " +
                                           "             (ReportType     " +
                                           "             ,DataID         " +
                                           "             ,AddData1       " +
                                           "             ,AddData2       " +
                                           "             ,AddedDate      " +
                                           "             ,Active)        " +
                                           "      VALUES ({0}              " +
                                           "             ,{1}              " +
                                           "             ,{2}              " +
                                           "             ,{3}             " +
                                           "             ,GETDATE()      " +
                                           "             ,{4})             ",
                        CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.ReportType))
                        , CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.DataID))
                        , CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.AddData1))
                        , CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.AddData2))
                        , CommonUnit.retDBStr(CommonUnit.CheckEmpty(model.Active))
                    );

                    DBHelper dbHelper = new DBHelper();
                    dbHelper.ExecuteQuery(CommandType.Text, insSQL);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        private static string jsonMaxLength(string s)
        {
            if (s.Length >= 4000)
            {
                s = s.Substring(0, 4000);
            }

            return s;
        }

        private static string String2Json(string s)
        {
            return jsonMaxLength(s.Replace("'", "'+char(39)+'").Replace("&", "'+char(38)+'"));
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Reflection realizes the copy of the same attribute value between objects of two classes
        /// Suitable for initializing new entities
        /// </summary>
        /// <typeparam name="D">Returned entity</typeparam>
        /// <typeparam name="S">Data source entity</typeparam>
        /// <param name="s">Data source entity</param>
        /// <returns>Returned new entity</returns>
        public static D Mapper<D, S>(S s)
        {
            D d = Activator.CreateInstance<D>(); //Construct a new instance
            try
            {
                var Types = s.GetType();
                var Typed = typeof(D);
                //Get the attribute field of the type 
                foreach (PropertyInfo sp in Types.GetProperties())
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        //Determine whether the attribute names are the same  
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")
                        {
                            //Get the value of the s object property and copy it to the d object property
                            dp.SetValue(d, sp.GetValue(s, null), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        /// <summary>
        /// Reflection realizes the copy of the same attribute value between the objects of two classes
        /// Suitable for no new entities
        /// </summary>
        /// <typeparam name="D">Returned entity</typeparam>
        /// <typeparam name="S">Data source entity</typeparam>
        /// <param name="d">Returned entity</param>
        /// <param name="s">Data source entity</param>
        /// <returns></returns>
        public static D MapperToModel<D, S>(D d, S s)
        {
            try
            {
                var Types = s.GetType();
                var Typed = typeof(D);
                foreach (PropertyInfo sp in Types.GetProperties())
                {
                    foreach (PropertyInfo dp in Typed.GetProperties())
                    {
                        if (dp.Name == sp.Name && dp.PropertyType == sp.PropertyType && dp.Name != "Error" && dp.Name != "Item")
                        {
                            dp.SetValue(d, sp.GetValue(s, null), null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return d;
        }

        public static string ReturnUpdateField(List<KeyValuePair<string, string>> fieldList)
        {
            string resSql = String.Empty;

            if (fieldList.Count > 0)
            {
                foreach (var fieldItem in fieldList)
                {
                    resSql += String.Format("," + fieldItem.Key + " = {0} ", CommonUnit.retDBStr(CommonUnit.CheckEmpty(fieldItem.Value)));
                }
            }
            else
            {
                resSql = null;
            }
            return resSql;
        }

        /// <summary>
        /// ToWords
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ToWords(int number)
        {
            if (number < 0)
            {
                return "";
            }
            if (number < 20) //0到19
            {
                switch (number)
                {
                    case 0:
                        return "zero";
                    case 1:
                        return "one";
                    case 2:
                        return "two";
                    case 3:
                        return "three";
                    case 4:
                        return "four";
                    case 5:
                        return "five";
                    case 6:
                        return "sex";
                    case 7:
                        return "seven";
                    case 8:
                        return "eight";
                    case 9:
                        return "nine";
                    case 10:
                        return "ten";
                    case 11:
                        return "eleven";
                    case 12:
                        return "twelve";
                    case 13:
                        return "thirteen";
                    case 14:
                        return "fourteen";
                    case 15:
                        return "fifteen";
                    case 16:
                        return "sixteen";
                    case 17:
                        return "seventeen";
                    case 18:
                        return "eighteen";
                    case 19:
                        return "nineteen";
                    default:
                        return "";
                }
            }
            if (number < 100) //20到99
            {
                if (number % 10 == 0) //20,30,40,...90的输出
                {
                    switch (number)
                    {
                        case 20:
                            return "twenty";
                        case 30:
                            return "thirty";
                        case 40:
                            return "forty";
                        case 50:
                            return "fifty";
                        case 60:
                            return "sixty";
                        case 70:
                            return "seventy";
                        case 80:
                            return "eighty";
                        case 90:
                            return "ninety";
                        default:
                            return "";
                    }
                }
                else
                {
                    return string.Format("{0}-{1}", ToWords(10 * (number / 10)),
                        ToWords(number % 10));
                }
            }
            if (number < 1000)
            {
                if (number % 100 == 0)
                {
                    return string.Format("{0} hundred", ToWords(number / 100));
                }
                else
                {
                    return string.Format("{0} hundred and {1}", ToWords(number / 100),
                        ToWords(number % 100));
                }
            }
            if (number < 1000000)
            {
                if (number % 1000 == 0)
                {
                    return string.Format("{0} thousand", ToWords(number / 1000));
                }
                else
                {
                    return string.Format("{0} thousand and {1}", ToWords(number / 1000),
                        ToWords(number % 1000));
                }
            }
            if (number < 1000000000)
            {
                if (number % 1000 == 0)
                {
                    return string.Format("{0} million", ToWords(number / 1000000));
                }
                else
                {
                    return string.Format("{0} million and {1}", ToWords(number / 1000000),
                        ToWords(number % 1000000));
                }
            }
            if (number <= int.MaxValue)
            {
                if (number % 1000000000 == 0)
                {
                    return string.Format("{0} billion", ToWords(number / 1000000000));
                }
                else
                {
                    return string.Format("{0} billion and {1}", ToWords(number / 1000000000),
                        ToWords(number % 1000000000));
                }
            }
            return "";
        }

        public static string[] retSplit(string str)
        {
            string[] strArray = null;
            try
            {
                if (str.Split(',').Length > 1)
                {
                    strArray = str.Split(',');
                }
                else if (str.Split(';').Length > 1)
                {
                    strArray = str.Split(';');
                }
                else if (str.Split('/').Length > 1)
                {
                    strArray = str.Split('/');
                }
                else if (str.Split('\\').Length > 1)
                {
                    strArray = str.Split('\\');
                }
                else
                {
                    strArray = new[] { str };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                strArray = null;
            }

            return strArray;
        }

        public static bool IsDate(string strDate)
        {
            try
            {
                DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static long DateDiff(string interval, DateTime dt1, DateTime dt2)
        {
            if (interval.ToLower() == "y" || interval.ToLower() == "year")
                return dt2.Year - dt1.Year;

            if (interval.ToLower() == "m" || interval.ToLower() == "month")
                return (dt2.Month - dt1.Month) + (12 * (dt2.Year - dt1.Year));

            TimeSpan ts = dt2 - dt1;

            if (interval.ToLower() == "d" || interval.ToLower() == "day")
                return Round(ts.TotalDays);

            if (interval.ToLower() == "h" || interval.ToLower() == "hour")
                return Round(ts.TotalHours);

            if (interval.ToLower() == "mi" || interval.ToLower() == "minute")
                return Round(ts.TotalMinutes);

            if (interval.ToLower() == "s" || interval.ToLower() == "second")
                return Round(ts.TotalSeconds);

            if (interval.ToLower() == "w" || interval.ToLower() == "weekday")
                return Round(ts.TotalDays / 7.0);

            return 0;

        }

        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }

    }

}
