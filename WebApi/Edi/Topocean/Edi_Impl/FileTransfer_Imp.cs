using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.File;
using TB_WEB.CommonLibrary.Helpers;
using WebApi.Edi.Topocean.Edi_Interface;

namespace WebApi.Edi.Topocean.Edi_Impl
{
    public class FileTransfer_Imp : IFileTransfer
    {
        protected DataTable BaseDataTable { get; set; }
        private List<FileTransfer_Mail_Attach> attachList;

        public void init(DataTable baseDT)
        {
            this.BaseDataTable = baseDT;
        }

        public void AddMailAttach(string source, string path, string name)
        {
            try
            {
                AddMailAttach(new FileTransfer_Mail_Attach() { source = source, path = path, name = name });
            }
            catch (Exception ex)
            {

            }
        }

        public void AddMailAttach(FileTransfer_Mail_Attach attachItem)
        {
            try
            {
                if (attachList == null)
                {
                    attachList = new List<FileTransfer_Mail_Attach>();
                }

                attachList.Add(attachItem);
            }
            catch (Exception ex)
            {

            }
        }

        //buil object which will used to convert to JSON
        public object hydrate()
        {
            try
            {//destination
                if (DBHelperBase.GetDataColumnValue<string>(BaseDataTable.Rows[0], "source", "").ToUpper() == "BASE64" && DBHelperBase.GetDataColumnValue<string>(BaseDataTable.Rows[0], "destination", "").ToUpper() == "EMAIL")
                {
                    FileTransfer_Mail fileTransfer_Mail = new FileTransfer_Mail(BaseDataTable);
                    if (attachList != null && attachList.Count > 0)
                    {
                        foreach (FileTransfer_Mail_Attach item in attachList)
                        {
                            fileTransfer_Mail.AddAttach(item);
                        }
                    }
                    string source_content = JsonConvert.SerializeObject(fileTransfer_Mail);
                    source_content = StringExtensions.Base64Encode(source_content);
                    BaseDataTable.Rows[0]["source_content"] = source_content;
                }

                return (new FileTransfer(BaseDataTable));

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //convert object to JSON
        public string serialize()
        {

            try
            {
                return JsonConvert.SerializeObject(((FileTransfer)hydrate()));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}