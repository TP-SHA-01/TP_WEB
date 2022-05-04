using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Encrypt;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using WebApi.Edi.Topocean.Edi_Impl;
using WebApi.Edi.Topocean.EdiModels.Common;

namespace WebApi.Edi.Common
{
    public class DocumentHelper
    {
        DocumentHelper()
        {

        }

        public static string GetFileIcon(string inDOCFilename)
        {
            string fExt = inDOCFilename.Substring(inDOCFilename.LastIndexOf("."));
            string retVal = string.Empty;
            switch (fExt.ToLower())
            {
                case "txt":
                    retVal = "icon-doc.gif";
                    break;

                case "xls":
                    retVal = "icon-xls.gif";
                    break;

                case "xlsx":
                    retVal = "icon-xls.gif";
                    break;

                case "doc":
                    retVal = "icon-word.gif";
                    break;

                case "docx":
                    retVal = "icon-word.gif";
                    break;

                case "wma":
                    retVal = "icon-audio.gif";
                    break;

                case "wmv":
                    retVal = "icon-film.gif";
                    break;

                case "gif":
                    retVal = "icon-image.gif";
                    break;

                case "jpg":
                    retVal = "icon-image.gif";
                    break;

                case "tif":
                    retVal = "icon-image.gif";
                    break;

                case "png":
                    retVal = "icon-image.gif";
                    break;

                case "bmp":
                    retVal = "icon-image.gif";
                    break;

                case "mdb":
                    retVal = "icon-mdb.gif";
                    break;

                case "pdf":
                    retVal = "icon-pdf.gif";
                    break;

                case "htm":
                    retVal = "icon-ie.gif";
                    break;

                case "html":
                    retVal = "icon-ie.gif";
                    break;

                case "ppsx":
                    retVal = "icon-pps.gif";
                    break;

                case "pps":
                    retVal = "icon-pps.gif";
                    break;

                case "zip":
                    retVal = "icon-zip.gif";
                    break;

                case "rar":
                    retVal = "icon-zip.gif";
                    break;

                default:
                    retVal = "icon-doc.gif";
                    break;
            }

            return retVal;
        }

        public static string[] PODownloadDoc2S3(string uID)
        {
            try
            {
                List<string> retVal = new List<string>();
                using (DBMSSQLHelper dbHelper = new DBMSSQLHelper())
                {
                    DataTable dt = dbHelper.ExecDataTable(string.Format("select DOCFilename, FileData, FileSize, ContentType, SaveLocation, RootDir, RelPath from podownloaddoc where uID = '{0}';", uID));
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow dr = dt.Rows[0];
                        string base64EncodeStr = string.Empty;
                        string DOCFilename = CommonFun.GetDRValue<string>(dr, "DOCFilename", "");
                        byte[] FileData = ((dr["FileData"] == null || dr["FileData"] == DBNull.Value) ? null : ((byte[])dr["FileData"]));
                        int FileSize = CommonFun.GetDRValue<int>(dr, "FileSize", 0);
                        string ContentType = CommonFun.GetDRValue<string>(dr, "ContentType", "");
                        string SaveLocation = CommonFun.GetDRValue<string>(dr, "SaveLocation", "");
                        string RootDir = CommonFun.GetDRValue<string>(dr, "RootDir", "");
                        string RelPath = CommonFun.GetDRValue<string>(dr, "RelPath", "");

                        if (SaveLocation == "FILE")
                        {
                            string tmpFile = string.Format(@"{0}\{1}", RootDir, RelPath);
                            FileData = File.ReadAllBytes(tmpFile);
                        }

                        if (FileData == null || FileData.Length <= 0)
                        {
                            throw (new Exception("No file data"));
                        }

                        if (FileData != null)
                        {
                            base64EncodeStr = StringExtensions.Base64Encode(FileData);
                        }

                        if (FileData == null || FileData.Length <= 0)
                        {
                            throw (new Exception("Fail to generate base64 content"));
                        }


                        DataTable tmpDT = new DataTable();
                        tmpDT.Columns.Add(new DataColumn() { ColumnName = "source", DataType = Type.GetType("System.String") });
                        tmpDT.Columns.Add(new DataColumn() { ColumnName = "source_content", DataType = Type.GetType("System.String") });
                        tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination", DataType = Type.GetType("System.String") });
                        tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination_content", DataType = Type.GetType("System.String") });
                        tmpDT.Columns.Add(new DataColumn() { ColumnName = "sync", DataType = Type.GetType("System.Int32") });
                        tmpDT.Rows.Add(new Object[] { "base64", base64EncodeStr, "s3_topol", string.Format("GTtest/{0}", DOCFilename), 1 });

                        FileTransfer_Imp fileTransfer_Imp = new FileTransfer_Imp();
                        fileTransfer_Imp.init(tmpDT);
                        string postJSON = fileTransfer_Imp.serialize();
                        // Authentication
                        AuthenticationModel authenticationModel = new AuthenticationModel(
                                new AuthenticationEntity() { usr = BaseCont.usr, psd = new Decryption().DecryptDBConnectionString(BaseCont.psd) });

                        //call /file-transfer API via FileTransferResponse_Imp
                        FileTransferResponse_Imp fileTransferResponse_Imp = new FileTransferResponse_Imp();
                        fileTransferResponse_Imp.init(
                                new EDIPostEntity() { queryParams = null, body = postJSON, apiUrl = BaseCont.ApiUrl + "/file-transfer" }
                                , authenticationModel);

                        fileTransferResponse_Imp.Post();

                        //get the response object
                        FileTransferResponse<FileTransfer_PayloadItem> respObj = fileTransferResponse_Imp.LoadBaseResponse<FileTransferResponse<FileTransfer_PayloadItem>>();

                        retVal.Add(respObj.payload.status);
                        retVal.Add(DOCFilename);
                        retVal.Add(respObj.payload.destination);
                        retVal.Add(respObj.payload.destination_content);
                    }
                }

                return retVal.ToArray();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
                return null;
            }
        }

        public static string[] PODownloadDoc2S3(MemoryStream stream,string DOCFilename)
        {
            List<string> retVal = new List<string>();

            try
            {
                string base64EncodeStr = string.Empty;
                if (stream.Length < 0 && stream == null)
                {
                    LogHelper.Error("PODownloadDoc2S3 => No File");
                    return null;
                }
                else
                {
                    base64EncodeStr = StringExtensions.Base64Encode(stream.ToArray());
                }

                DataTable tmpDT = new DataTable();
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "source", DataType = Type.GetType("System.String") });
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "source_content", DataType = Type.GetType("System.String") });
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination", DataType = Type.GetType("System.String") });
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "destination_content", DataType = Type.GetType("System.String") });
                tmpDT.Columns.Add(new DataColumn() { ColumnName = "sync", DataType = Type.GetType("System.Int32") });
                tmpDT.Rows.Add(new Object[] { "base64", base64EncodeStr, "s3_topol", string.Format("GTtest/{0}", DOCFilename), 1 });

                FileTransfer_Imp fileTransfer_Imp = new FileTransfer_Imp();
                fileTransfer_Imp.init(tmpDT);
                string postJSON = fileTransfer_Imp.serialize();
                // Authentication
                AuthenticationModel authenticationModel = new AuthenticationModel(
                        new AuthenticationEntity() { usr = BaseCont.usr, psd = new Decryption().DecryptDBConnectionString(BaseCont.psd) });

                //call /file-transfer API via FileTransferResponse_Imp
                FileTransferResponse_Imp fileTransferResponse_Imp = new FileTransferResponse_Imp();
                fileTransferResponse_Imp.init(
                        new EDIPostEntity() { queryParams = null, body = postJSON, apiUrl = BaseCont.ApiUrl + "/file-transfer" }
                        , authenticationModel);

                fileTransferResponse_Imp.Post();

                //get the response object
                FileTransferResponse<FileTransfer_PayloadItem> respObj = fileTransferResponse_Imp.LoadBaseResponse<FileTransferResponse<FileTransfer_PayloadItem>>();

                retVal.Add(respObj.payload.status);
                retVal.Add(DOCFilename);
                retVal.Add(respObj.payload.destination);
                retVal.Add(respObj.payload.destination_content);
            }
            catch (Exception)
            {
            }
            return retVal.ToArray();
        }

    }
}
