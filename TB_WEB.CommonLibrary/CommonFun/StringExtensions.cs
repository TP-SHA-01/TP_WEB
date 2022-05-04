using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.CommonFun
{
    public static class StringExtensions
    {
        #region 64Encode function(s)
        public static string Base64Encode(string plainText)
        {
            try
            {
                return Base64Encode(plainText, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Base64Encode(string plainText, Encoding encoding)
        {
            try
            {
                var plainTextBytes = encoding.GetBytes(plainText);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Base64Encode(byte[] fByte)
        {
            try
            {
                return Convert.ToBase64String(fByte);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region 64Decode function(2)
        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                return Base64Decode(base64EncodedData, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Base64Decode(string base64EncodedData, Encoding encoding)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return encoding.GetString(base64EncodedBytes);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
