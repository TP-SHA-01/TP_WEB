using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Helpers;

namespace TB_WEB.CommonLibrary.File
{
    public class FileTransfer_Mail
    {
        protected DataTable baseDataTable { set; get; }

        #region construction
        public FileTransfer_Mail()
        {
        }

        public FileTransfer_Mail(DataTable baseDataTable)
        {
            this.baseDataTable = baseDataTable;
            setValue();
        }
        #endregion

        #region function(s)
        public void setValue()
        {
            try
            {
                this.from = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "from", "");
                this.subject = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "subject", "");
                this.body = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "body", "");
                this.type = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "type", "");
            }
            catch (Exception ex)
            {
                //SaveLog(ex.ToString(), "EX");
            }
        }

        public void AddAttach(string source, string path, string name)
        {
            try
            {
                this.AddAttach(new FileTransfer_Mail_Attach() { source = source, path = path, name = name });
            }
            catch (Exception ex)
            {
                //SaveLog(ex.ToString());
            }
        }

        public void AddAttach(FileTransfer_Mail_Attach attachItem)
        {
            try
            {
                if (attachments == null)
                {
                    attachments = new List<FileTransfer_Mail_Attach>();
                }

                attachments.Add(attachItem);
            }
            catch (Exception ex)
            {
                //SaveLog(ex.ToString(), "EX");
            }
        }
        #endregion

        #region public properties
        public string from { get; set; } //web@topocean.com
        public string subject { get; set; } //New Booking Submitted, Shipper Name : FOSHAN GREENYELLOW ELECT / CNEE: WOODSTREAM /  Booking ID# WB49810163
        public string body { get; set; } //New Booking Submitted, Shipper Name : FOSHAN GREENYELLOW ELECT / CNEE: WOODSTREAM /  Booking ID# WB49810163
        public string type { get; set; }  //html
        public List<FileTransfer_Mail_Attach> attachments { get; set; }
        #endregion
    }
}
