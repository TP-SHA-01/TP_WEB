using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Encrypt
{
    #region Decryption Class
    /// <summary>
    /// Summary description for Decryption.
    /// </summary>
    public class Decryption
    {
        #region Decryption Constructor
        public Decryption()
        {
        }
        #endregion Decryption Constructor

        #region Decrypt
        public string Decrypt(string argInput, string argDecrKey)
        {


            byte[] byteKey;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byte[] inputByteArray = new byte[argInput.Length];

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                Encoding encoding = System.Text.Encoding.UTF8;

                if (argDecrKey.Length < 4)
                    argDecrKey += argDecrKey;

                if (argDecrKey.Length < 8)
                    argDecrKey += argDecrKey.Substring(0, 8 - argDecrKey.Length);

                byteKey = System.Text.Encoding.UTF8.GetBytes(argDecrKey.Substring(0, 8));
                inputByteArray = Convert.FromBase64String(argInput);

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byteKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion Decrypt

        #region DecryptPassword
        public string DecryptPassword(string argInput)
        {
            return Decrypt(argInput, "&%#@?,:*");
        }
        #endregion DecryptPassword

        #region DecryptDBConnectionString
        public string DecryptDBConnectionString(string argInput)
        {
            return Decrypt(argInput, "database");
        }
        #endregion DecryptDBConnectionString
    }
    #endregion Decryption Class
}
