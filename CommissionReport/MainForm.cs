using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommissionReport
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btn_Import_Click(object sender, EventArgs e)
        {
            string path = String.Empty;

            if (showDialogGetSaveFolder(out path))
            {
                txt_importPath.Text = path;
            }
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {

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
