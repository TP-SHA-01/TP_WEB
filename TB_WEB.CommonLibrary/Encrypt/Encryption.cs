using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Encrypt
{
    public class Encryption
    {
        #region Encryption Constructor
        public Encryption()
        {

        }
        #endregion Encryption Constructor

        #region Encrypt
        public string Encrypt(string argInput, string argEncrKey)
        {

            byte[] byteKey;
            byte[] IV = new byte[8] { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();

                if (argEncrKey.Length < 4)
                    argEncrKey += argEncrKey;

                if (argEncrKey.Length < 8)
                    argEncrKey += argEncrKey.Substring(0, 8 - argEncrKey.Length);

                byteKey = System.Text.Encoding.UTF8.GetBytes(argEncrKey.Substring(0, 8));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(argInput);

                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byteKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion Encrypt

        #region EncryptPassword
        public string EncryptPassword(string argInput)
        {
            return Encrypt(argInput, "&%#@?,:*");
        }
        #endregion EncryptPassword

        #region EncryptDBConnectionString
        public string EncryptDBConnectionString(string argInput)
        {
            return Encrypt(argInput, "database");
        }
        #endregion EncryptDBConnectionString

        public string EncryptforwardConnectionString(string argInput)
        {
            return Encrypt(argInput, "terence");
        }
    }
}
