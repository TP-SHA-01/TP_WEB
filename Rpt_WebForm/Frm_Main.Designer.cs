
namespace Rpt_WebForm
{
    partial class Frm_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_SelectMultiExcel = new System.Windows.Forms.Button();
            this.txt_MultiExcel = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.combReportType = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_ShowInfo = new System.Windows.Forms.Button();
            this.btn_Combined = new System.Windows.Forms.Button();
            this.hidExcelPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btn_SelectMultiExcel);
            this.groupBox1.Controls.Add(this.txt_MultiExcel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.combReportType);
            this.groupBox1.Location = new System.Drawing.Point(12, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(625, 114);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Report Type:";
            // 
            // btn_SelectMultiExcel
            // 
            this.btn_SelectMultiExcel.Location = new System.Drawing.Point(524, 65);
            this.btn_SelectMultiExcel.Name = "btn_SelectMultiExcel";
            this.btn_SelectMultiExcel.Size = new System.Drawing.Size(94, 29);
            this.btn_SelectMultiExcel.TabIndex = 3;
            this.btn_SelectMultiExcel.Text = "Select";
            this.btn_SelectMultiExcel.UseVisualStyleBackColor = true;
            this.btn_SelectMultiExcel.Click += new System.EventHandler(this.btn_SelectMultiExcel_Click);
            // 
            // txt_MultiExcel
            // 
            this.txt_MultiExcel.Location = new System.Drawing.Point(164, 67);
            this.txt_MultiExcel.Name = "txt_MultiExcel";
            this.txt_MultiExcel.Size = new System.Drawing.Size(354, 27);
            this.txt_MultiExcel.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Please Export Folder:";
            // 
            // combReportType
            // 
            this.combReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combReportType.FormattingEnabled = true;
            this.combReportType.Location = new System.Drawing.Point(164, 26);
            this.combReportType.Name = "combReportType";
            this.combReportType.Size = new System.Drawing.Size(155, 28);
            this.combReportType.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_ShowInfo);
            this.groupBox2.Controls.Add(this.btn_Combined);
            this.groupBox2.Location = new System.Drawing.Point(12, 142);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(625, 69);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btn_ShowInfo
            // 
            this.btn_ShowInfo.Location = new System.Drawing.Point(422, 16);
            this.btn_ShowInfo.Name = "btn_ShowInfo";
            this.btn_ShowInfo.Size = new System.Drawing.Size(95, 46);
            this.btn_ShowInfo.TabIndex = 1;
            this.btn_ShowInfo.Text = "View File";
            this.btn_ShowInfo.UseVisualStyleBackColor = true;
            this.btn_ShowInfo.Visible = false;
            this.btn_ShowInfo.Click += new System.EventHandler(this.btn_ShowInfo_Click);
            // 
            // btn_Combined
            // 
            this.btn_Combined.Location = new System.Drawing.Point(523, 17);
            this.btn_Combined.Name = "btn_Combined";
            this.btn_Combined.Size = new System.Drawing.Size(95, 46);
            this.btn_Combined.TabIndex = 0;
            this.btn_Combined.Text = "Combined";
            this.btn_Combined.UseVisualStyleBackColor = true;
            this.btn_Combined.Click += new System.EventHandler(this.btn_Combined_Click);
            // 
            // hidExcelPath
            // 
            this.hidExcelPath.BackColor = System.Drawing.SystemColors.Window;
            this.hidExcelPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hidExcelPath.Location = new System.Drawing.Point(626, 276);
            this.hidExcelPath.Name = "hidExcelPath";
            this.hidExcelPath.Size = new System.Drawing.Size(11, 20);
            this.hidExcelPath.TabIndex = 5;
            this.hidExcelPath.TabStop = false;
            this.hidExcelPath.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(581, -1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "v1.3";
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 220);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.hidExcelPath);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Combine Tools";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox combReportType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_SelectMultiExcel;
        private System.Windows.Forms.TextBox txt_MultiExcel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_ShowInfo;
        private System.Windows.Forms.Button btn_Combined;
        private System.Windows.Forms.TextBox hidExcelPath;
        private System.Windows.Forms.Label label3;
    }
}

