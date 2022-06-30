
namespace AirShipmentReport
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.combWeekTo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.combWeekFrom = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.combYearTo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.combYearFrom = new System.Windows.Forms.ComboBox();
            this.BtnRun = new System.Windows.Forms.Button();
            this.btnSelectRpt = new System.Windows.Forms.Button();
            this.txtSelReport = new System.Windows.Forms.TextBox();
            this.btnExportColoaderPO = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // combWeekTo
            // 
            this.combWeekTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combWeekTo.Font = new System.Drawing.Font("Arial", 12F);
            this.combWeekTo.FormattingEnabled = true;
            this.combWeekTo.Location = new System.Drawing.Point(516, 87);
            this.combWeekTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combWeekTo.Name = "combWeekTo";
            this.combWeekTo.Size = new System.Drawing.Size(155, 31);
            this.combWeekTo.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 12F);
            this.label4.Location = new System.Drawing.Point(396, 93);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 23);
            this.label4.TabIndex = 19;
            this.label4.Text = "Month To:";
            // 
            // combWeekFrom
            // 
            this.combWeekFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combWeekFrom.Font = new System.Drawing.Font("Arial", 12F);
            this.combWeekFrom.FormattingEnabled = true;
            this.combWeekFrom.Location = new System.Drawing.Point(516, 49);
            this.combWeekFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combWeekFrom.Name = "combWeekFrom";
            this.combWeekFrom.Size = new System.Drawing.Size(155, 31);
            this.combWeekFrom.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F);
            this.label3.Location = new System.Drawing.Point(396, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 23);
            this.label3.TabIndex = 17;
            this.label3.Text = "Month From:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F);
            this.label1.Location = new System.Drawing.Point(72, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 23);
            this.label1.TabIndex = 16;
            this.label1.Text = "Year To:";
            // 
            // combYearTo
            // 
            this.combYearTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combYearTo.Font = new System.Drawing.Font("Arial", 12F);
            this.combYearTo.FormattingEnabled = true;
            this.combYearTo.Location = new System.Drawing.Point(199, 87);
            this.combYearTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combYearTo.Name = "combYearTo";
            this.combYearTo.Size = new System.Drawing.Size(155, 31);
            this.combYearTo.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F);
            this.label2.Location = new System.Drawing.Point(72, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 23);
            this.label2.TabIndex = 14;
            this.label2.Text = "Year From:";
            // 
            // combYearFrom
            // 
            this.combYearFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combYearFrom.Font = new System.Drawing.Font("Arial", 12F);
            this.combYearFrom.FormattingEnabled = true;
            this.combYearFrom.Location = new System.Drawing.Point(199, 49);
            this.combYearFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combYearFrom.Name = "combYearFrom";
            this.combYearFrom.Size = new System.Drawing.Size(155, 31);
            this.combYearFrom.TabIndex = 13;
            // 
            // BtnRun
            // 
            this.BtnRun.Font = new System.Drawing.Font("Arial", 12F);
            this.BtnRun.Location = new System.Drawing.Point(354, 167);
            this.BtnRun.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(317, 33);
            this.BtnRun.TabIndex = 21;
            this.BtnRun.Text = "Export  Monthly Report";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // btnSelectRpt
            // 
            this.btnSelectRpt.Font = new System.Drawing.Font("Arial", 12F);
            this.btnSelectRpt.Location = new System.Drawing.Point(67, 226);
            this.btnSelectRpt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectRpt.Name = "btnSelectRpt";
            this.btnSelectRpt.Size = new System.Drawing.Size(151, 33);
            this.btnSelectRpt.TabIndex = 22;
            this.btnSelectRpt.Text = "Select Report";
            this.btnSelectRpt.UseVisualStyleBackColor = true;
            this.btnSelectRpt.Click += new System.EventHandler(this.btnSelectRpt_Click);
            // 
            // txtSelReport
            // 
            this.txtSelReport.Location = new System.Drawing.Point(219, 230);
            this.txtSelReport.Name = "txtSelReport";
            this.txtSelReport.Size = new System.Drawing.Size(452, 25);
            this.txtSelReport.TabIndex = 23;
            // 
            // btnExportColoaderPO
            // 
            this.btnExportColoaderPO.Font = new System.Drawing.Font("Arial", 12F);
            this.btnExportColoaderPO.Location = new System.Drawing.Point(354, 283);
            this.btnExportColoaderPO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExportColoaderPO.Name = "btnExportColoaderPO";
            this.btnExportColoaderPO.Size = new System.Drawing.Size(317, 33);
            this.btnExportColoaderPO.TabIndex = 24;
            this.btnExportColoaderPO.Text = "Export Coloader and PO Report";
            this.btnExportColoaderPO.UseVisualStyleBackColor = true;
            this.btnExportColoaderPO.Click += new System.EventHandler(this.btnExportColoaderPO_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(69, 332);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 25;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 359);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExportColoaderPO);
            this.Controls.Add(this.txtSelReport);
            this.Controls.Add(this.btnSelectRpt);
            this.Controls.Add(this.BtnRun);
            this.Controls.Add(this.combWeekTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.combWeekFrom);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.combYearTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.combYearFrom);
            this.Name = "MainForm";
            this.Text = "AirShipmentReport";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox combWeekTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox combWeekFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combYearTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combYearFrom;
        private System.Windows.Forms.Button BtnRun;
        private System.Windows.Forms.Button btnSelectRpt;
        private System.Windows.Forms.TextBox txtSelReport;
        private System.Windows.Forms.Button btnExportColoaderPO;
        private System.Windows.Forms.Label lblStatus;
    }
}