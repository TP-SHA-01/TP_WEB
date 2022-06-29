using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Model
{
    public class ExportModel
    {
        public DataSet dt_source { get; set; }
        public List<string> sheet_name_list { get; set; }
        public List<string> temp_sheet_name_list { get; set; }
        public IWorkbook temp_work_book { get; set; }
        public string temp_work_tpye { get; set; }
        public string report_type { get; set; }
        public string file_name { get; set; }
    }
}
