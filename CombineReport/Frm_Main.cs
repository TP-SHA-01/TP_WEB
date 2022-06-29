using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
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
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using TB_WEB.CommonLibrary.Model;

namespace CombineReport
{
    public partial class Frm_Main : Form
    {
        public Frm_Main()
        {
            InitializeComponent();
            combReportType.Items.AddRange(new object[] { "TP" });
            //combReportType.Items.AddRange(new object[] { "NONTP" });
            combReportType.Items.AddRange(new object[] { "BY WEEK" });
            combReportType.SelectedIndex = 0;

            txtFilePath.Enabled = false;
        }

        public bool showDialogGetSaveFolder(out string folderPath)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Files|*.xls;*.xlsx";
            //dlg.Filter = "Files|*.xlsx";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            bool dialogResult = dlg.ShowDialog() == DialogResult.OK;
            folderPath = dlg.FileName;

            return dialogResult;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            try
            {
                if (showDialogGetSaveFolder(out path))
                {
                    txtFilePath.Text = path;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btnCombined_Click(object sender, EventArgs e)
        {
            this.btnCombined.Enabled = false;

            try
            {
                string reportType = combReportType.Text;

                switch (reportType)
                {
                    case "TP":
                        RenderExcel();
                        break;
                    case "NONTP":
                        RenderExcel();
                        break;
                    case "BY WEEK":
                        RenderExcel();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void RenderExcel()
        {
            FileInfo _folder = new FileInfo(txtFilePath.Text);
            string exportPath = String.Empty;
            ExportExcel(_folder, combReportType.Text, _folder.FullName, out exportPath);

            Clipboard.SetDataObject(exportPath);
            if (MessageBox.Show("Is that need Open the File ?", "Tip", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(exportPath);
            }
        }

        public void ExportExcel(FileInfo fileInfo, string tempReportType, string savePath, out string exportfilePath)
        {
            string retfilePath = String.Empty;
            try
            {
                RenderModel renderModel = new RenderModel();
                renderModel.file_info = fileInfo;
                renderModel.report_type = tempReportType;

                CombinedExcel.Render(renderModel, out retfilePath);

                this.btnCombined.Enabled = true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("CombineReport => ExportExcel_TP :" + ex.Message + " StackTrace: " + ex.StackTrace);
            }
            exportfilePath = retfilePath;
        }
    }
}
