using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

namespace Rpt_WebForm
{
    public partial class Frm_Main : Form
    {
        public Frm_Main()
        {
            InitializeComponent();
            combReportType.Items.AddRange(new object[] { "Loading Report" });
            combReportType.SelectedIndex = 0;
            txt_MultiExcel.Enabled = false;
            //combReportType.Enabled = false;
        }

        private void btn_SelectMultiExcel_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            try
            {
                if (showDialogGetSaveFolder(out path))
                {
                    txt_MultiExcel.Text = path;
                    DirectoryInfo _folder = new DirectoryInfo(path);

                    if (_folder.GetFiles().Length <= 0)
                    {
                        MessageBox.Show("Folder has no files, Please check again");
                    }

                    ArrayList list = new ArrayList();
                    Hashtable hasList = new Hashtable();
                    Dictionary<string, FileInfo> dict = new Dictionary<string, FileInfo>();
                    foreach (FileSystemInfo fsi in _folder.GetFileSystemInfos())
                    {
                        if (fsi is FileInfo)
                        {
                            FileInfo fi = (FileInfo)fsi;
                            if (fi.Extension.ToUpper() == ".XLSX" || fi.Extension.ToUpper() == ".XLS")
                            {
                                //FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                                if (!dict.ContainsKey(fi.Name))
                                {
                                    dict.Add(fi.Name, fi);
                                }
                            }
                        }
                    }


                    NPOIHelper.ImportExcelByFileList(dict);

                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
            }
        }

        public bool showDialogGetSaveFolder(out string folderPath)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            bool dialogResult = dlg.ShowDialog() == DialogResult.OK;
            folderPath = dlg.SelectedPath.ToString();
            return dialogResult;
        }
    }
}
