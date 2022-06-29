using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Model
{
    public class RenderModel
    {
        public string file_path { get; set; }
        public string report_type { get; set; }

        public FileInfo file_info { get; set; }

        public IWorkbook work_book { get; set; }
    }
}
