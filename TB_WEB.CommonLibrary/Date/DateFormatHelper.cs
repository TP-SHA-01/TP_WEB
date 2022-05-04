using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

namespace TB_WEB.CommonLibrary.Date
{
    public class DateFormatHelper
    {
        private static readonly string DISPLAY_DATE_FORMAT = System.Configuration.ConfigurationManager.AppSettings["DefaultDateFormat"].ToString();
        private static readonly string DISPLAY_TIME_FORMAT = "hh:mm";
        private static readonly string DISPLAY_DATETIME_FORMAT = "MM/dd/yyyy HH:mm:ss";
        private static readonly bool UseSessionDateFormat = bool.Parse((System.Configuration.ConfigurationManager.AppSettings["UseSessionDateFormat"]));
        private static readonly string DateFormatSessionKey = System.Configuration.ConfigurationManager.AppSettings["DateFormatSessionKey"].ToString();

        private static readonly string DB_DATE_FORMAT = "yyyy-MM-dd";
        private static readonly string DB_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public static readonly string JS_DATE_FORMAT = "dd/MMM/yyyy";
        public static readonly string JS_DATETIME_FORMAT = "dd/MMM/yyyy HH:mm:ss";

        public static string DisplayDate(DateTime dateTime, string dispFormat = "")
        {
            //if dispFormat is blank, use Login.DateFormat
            if (string.IsNullOrEmpty(dispFormat))
            {
                dispFormat = CurrentDateFormat();
            }

            if (dateTime == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                return dateTime.ToString(dispFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static string DisplayTime(DateTime dateTime, string dispFormat = "")
        {
            return dateTime.ToString(DISPLAY_TIME_FORMAT);
        }

        public static string DisplayDateTime(DateTime dateTime, string dispFormat = "")
        {
            if (string.IsNullOrEmpty(dispFormat))
            {
                dispFormat = DISPLAY_DATETIME_FORMAT;
            }

            if (dateTime == DateTime.MinValue)
            {
                return "";
            }
            else
            {
                return dateTime.ToString(dispFormat, System.Globalization.CultureInfo.InvariantCulture);
            }

        }

        public static string DisplayDateTime(DateTime? dateTime, string dispFormat = "")
        {
            if (string.IsNullOrEmpty(dispFormat))
            {
                dispFormat = DISPLAY_DATETIME_FORMAT;
            }

            if (dateTime == null)
            {
                return "";
            }
            else
            {
                return ((DateTime)dateTime).ToString(dispFormat, System.Globalization.CultureInfo.InvariantCulture);
            }

        }

        public static string DisplayAsDBDateFormat(DateTime dateTime)
        {
            return DisplayDateTime(dateTime, DB_DATE_FORMAT);
        }

        public static string DisplayAsDBDateFormat(DateTime? dateTime)
        {
            return DisplayDateTime(dateTime, DB_DATE_FORMAT);
        }

        public static string DisplayAsDBDateTimeFormat(DateTime dateTime)
        {
            return DisplayDateTime(dateTime, DB_DATETIME_FORMAT);
        }

        public static string DisplayAsDBDateTimeFormat(DateTime? dateTime)
        {
            return DisplayDateTime(dateTime, DB_DATETIME_FORMAT);
        }

        //Convert string to datetime object
        //inDateTime: output datetime object
        //dateStr: input date string
        //formatStr: date format of input date string
        //cultureInfo: convert CultureInfo
        //dateTimeStyles: 
        public static bool ConvertToDate(ref DateTime inDateTime, string dateStr, string formatStr = "", CultureInfo cultureInfo = null, DateTimeStyles dateTimeStyles = DateTimeStyles.None)
        {
            DateTime tmpDateTime = inDateTime;
            try
            {
                //if formatSt is blank, use Login.DateFormat
                if (string.IsNullOrEmpty(formatStr))
                {
                    formatStr = CurrentDateFormat();
                }

                //use default "en-US" culture if is null
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

                inDateTime = tmpDateTime;
                return false;
            }
        }

        public static DateTime ConvertToDate(string dateStr, string formatStr = "", CultureInfo cultureInfo = null, DateTimeStyles dateTimeStyles = DateTimeStyles.None)
        {
            DateTime tmpDateTime = DateTime.MinValue;
            try
            {
                //if formatSt is blank, use Login.DateFormat
                if (string.IsNullOrEmpty(formatStr))
                {
                    formatStr = CurrentDateFormat();
                }

                //use default "en-US" culture if is null
                if (cultureInfo == null)
                {
                    cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
                }

                if (DateTime.TryParseExact(dateStr, formatStr, cultureInfo, dateTimeStyles, out tmpDateTime))
                {
                    return tmpDateTime;
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

        public static bool ConvertToDateFrmDispaly(ref DateTime inDateTime, string dateStr, string dispFormat = "")
        {
            try
            {
                //if dispFormat is blank, use Login.DateFormat
                if (string.IsNullOrEmpty(dispFormat))
                {
                    if (dateStr.IndexOf(':') > 0)
                    {
                        dispFormat = CurrentDateTimeFormat();
                        if (dateStr.Split(':').Count() > 2)
                        {
                            dispFormat += ":ss";
                        }
                    }
                    else
                    {
                        dispFormat = CurrentDateFormat();
                    }
                }

                if (ConvertToDate(ref inDateTime, dateStr, dispFormat, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None))
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

                //this.SaveLog(ex.ToString(), "E");

                return false;
            }


        }

        public static DateTime ConvertToDateFrmDispaly(string dateStr, string dispFormat = "")
        {
            try
            {
                DateTime inDateTime = DateTime.MinValue;
                //if dispFormat is blank, use Login.DateFormat
                if (string.IsNullOrEmpty(dispFormat))
                {
                    if (dateStr.IndexOf(':') > 0)
                    {
                        dispFormat = CurrentDateTimeFormat();
                        if (dateStr.Split(':').Count() > 2)
                        {
                            dispFormat += ":ss";
                        }
                    }
                    else
                    {
                        dispFormat = CurrentDateFormat();
                    }
                }

                if (ConvertToDate(ref inDateTime, dateStr, dispFormat, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None))
                {
                    return inDateTime;
                }
                else
                {
                    throw new Exception("ConvertToDateFrmDispaly: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {

                //this.SaveLog(ex.ToString(), "E");

                return DateTime.MinValue;
            }


        }

        public static bool ConvertToDateFrmDB(ref DateTime inDateTime, string dateStr, string dispFormat = "")
        {
            try
            {
                //if dispFormat is blank, use Login.DateFormat
                if (string.IsNullOrEmpty(dispFormat))
                {
                    dispFormat = DB_DATETIME_FORMAT;
                }

                if (ConvertToDate(ref inDateTime, dateStr, dispFormat, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None))
                {
                    return true;
                }
                else
                {
                    throw new Exception("ConvertToDateFrmDB: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {

                //this.SaveLog(ex.ToString(), "E");

                return false;
            }


        }

        public static DateTime ConvertToDateFrmDB(string dateStr, string dispFormat = "")
        {
            try
            {
                DateTime inDateTime = DateTime.MinValue;
                //if dispFormat is blank, use Login.DateFormat
                if (string.IsNullOrEmpty(dispFormat))
                {
                    dispFormat = DB_DATETIME_FORMAT;
                }

                if (ConvertToDate(ref inDateTime, dateStr, dispFormat, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None))
                {
                    return inDateTime;
                }
                else
                {
                    throw new Exception("ConvertToDateFrmDB: call TryParseExact fail");
                }
            }
            catch (Exception ex)
            {

                //this.SaveLog(ex.ToString(), "E");

                return DateTime.MinValue;
            }


        }


        public static string CurrentDateFormat()
        {
            if (UseSessionDateFormat == true && System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session[DateFormatSessionKey] != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.Session[DateFormatSessionKey].ToString()))
            {
                return ConvertWBIDateFormatStrToCSharp(System.Web.HttpContext.Current.Session[DateFormatSessionKey].ToString());
            }
            else
            {
                return DISPLAY_DATE_FORMAT;
            }
        }

        public static string CurrentDateTimeFormat()
        {
            if (UseSessionDateFormat == true && HttpContext.Current.Session[DateFormatSessionKey] != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.Session[DateFormatSessionKey].ToString()))
            {
                return ConvertWBIDateFormatStrToCSharp(System.Web.HttpContext.Current.Session[DateFormatSessionKey].ToString()) + " " + DISPLAY_TIME_FORMAT;
            }
            else
            {
                return DISPLAY_DATE_FORMAT + " " + DISPLAY_TIME_FORMAT;
            }
        }

        public static string ConvertWBIDateFormatStrToCSharp(string inStr)
        {

            switch (inStr.ToLower())
            {
                case "dd/mm/yyyy":
                    return "dd/MM/yyyy";
                    break;

                case "yyyy/mm/dd":
                    return "yyyy/MM/dd";
                    break;

                case "mm-dd-yyyy":
                    return "MM-dd-yyyy";
                    break;

                case "yyyy-mm-dd":
                    return "yyyy-MM-dd";
                    break;

                case "dd.mm.yyyy":
                    return "dd.MM.yyyy";
                    break;

                case "yyyy.mm.dd":
                    return "yyyy.MM.dd";
                    break;

                default:
                    return DISPLAY_DATE_FORMAT;
                    break;
            }

        }

        public static DateTime UTCLocalDateByUser(string userLogin)
        {
            DateTime retVal = DateTime.MinValue;
            try
            {
                using (DBHelper dbhelper = new DBHelper())
                {
                    int UTCOffsetFactor = 0;
                    //use user's UTCOffsetFactor if login have set UTCOffsetFactor
                    DataTable dt = dbhelper.ExecDataTable(string.Format("select ISNULL(UTCOffsetFactor, '') as UTCOffsetFactor from Login where loginname = '{0}'", userLogin));
                    if (dt != null && dt.Rows.Count > 0)
                    {

                        if (dt.Rows[0]["UTCOffsetFactor"].ToString() != "" && Int32.TryParse(dt.Rows[0]["UTCOffsetFactor"].ToString(), out UTCOffsetFactor))
                        {
                            retVal = DateTime.UtcNow.AddHours(UTCOffsetFactor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                retVal = DateTime.MinValue;
            }

            return retVal;
        }

        #region Datetime time zone convert functions
        public static DateTime? ConvertAMSDateTimeToUTC(DateTime inDateTime)
        {
            try
            {
                if (inDateTime > DateTime.MinValue)
                {
                    //AMS using "Eastern Standard Time" 
                    TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    TimeZoneInfo utcZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");

                    inDateTime = TimeZoneInfo.ConvertTime(inDateTime, fromZone, utcZone);


                    return inDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }

        public static DateTime? ConvertAMSDateTimeToUTC(DateTime? inDateTime)
        {
            try
            {
                if (inDateTime != null && inDateTime > DateTime.MinValue)
                {
                    //AMS using "Eastern Standard Time" 
                    TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    TimeZoneInfo utcZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");

                    inDateTime = TimeZoneInfo.ConvertTime((DateTime)inDateTime, fromZone, utcZone);


                    return inDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }


        public static DateTime? ConvertSysDatetimeToUTC(DateTime inDateTime)
        {
            try
            {
                if (inDateTime > DateTime.MinValue)
                {
                    //System using "Pacific Standard Time"
                    TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                    TimeZoneInfo utcZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");

                    inDateTime = TimeZoneInfo.ConvertTime(inDateTime, fromZone, utcZone);


                    return inDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }

        public static DateTime? ConvertSysDatetimeToUTC(DateTime? inDateTime)
        {
            try
            {
                if (inDateTime != null && inDateTime > DateTime.MinValue)
                {
                    //System using "Pacific Standard Time"
                    TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
                    TimeZoneInfo utcZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");

                    inDateTime = TimeZoneInfo.ConvertTime((DateTime)inDateTime, fromZone, utcZone);


                    return inDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }


        public static DateTime? ConvertUTCToSysDatetime(DateTime inDateTime)
        {
            try
            {
                if (inDateTime > DateTime.MinValue)
                {
                    //System using "Pacific Standard Time"
                    TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                    TimeZoneInfo utcZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

                    inDateTime = TimeZoneInfo.ConvertTime(inDateTime, fromZone, utcZone);


                    return inDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }

        public static DateTime? ConvertUTCToSysDatetime(DateTime? inDateTime)
        {
            try
            {
                if (inDateTime != null && inDateTime > DateTime.MinValue)
                {
                    //System using "Pacific Standard Time"
                    TimeZoneInfo fromZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
                    TimeZoneInfo utcZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

                    inDateTime = TimeZoneInfo.ConvertTime((DateTime)inDateTime, fromZone, utcZone);


                    return inDateTime;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }


        #endregion
    }
}
