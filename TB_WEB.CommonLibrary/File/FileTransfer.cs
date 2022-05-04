using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Helpers;

namespace TB_WEB.CommonLibrary.File
{
    public class FileTransfer
    {
        protected DataTable baseDataTable { set; get; }

        #region construction
        public FileTransfer()
        {

        }

        public FileTransfer(DataTable baseDataTable)
        {
            this.baseDataTable = baseDataTable;
            this.setValue();
        }
        #endregion

        #region function(S)
        public void setValue()
        {
            try
            {
                this.source = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "source", "");
                this.source_content = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "source_content", "");
                this.destination = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "destination", "");
                this.destination_content = DBHelperBase.GetDataColumnValue<string>(baseDataTable.Rows[0], "destination_content", "");
                this.sync = DBHelperBase.GetDataColumnValue<Int32>(baseDataTable.Rows[0], "sync", 0);
            }
            catch (Exception ex)
            {

            }
        }



        #endregion

        #region public properties
        public string source { get; set; } //base64
        public string source_content { get; set; } //base64 content
        public string destination { get; set; } //email
        public string destination_content { get; set; } //spencer@topocean.com,gavin_tse@topocean.com.hk
        public int sync { get; set; } // 0 ; 1
        #endregion


    }
}
