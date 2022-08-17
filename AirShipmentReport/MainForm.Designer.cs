
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnExportColoaderPO = new System.Windows.Forms.Button();
            this.txtSelReport = new System.Windows.Forms.TextBox();
            this.btnSelectRpt = new System.Windows.Forms.Button();
            this.BtnRun = new System.Windows.Forms.Button();
            this.combWeekTo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.combWeekFrom = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.combYearTo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.combYearFrom = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.btnExportColoaderPO);
            this.panel1.Controls.Add(this.txtSelReport);
            this.panel1.Controls.Add(this.btnSelectRpt);
            this.panel1.Controls.Add(this.BtnRun);
            this.panel1.Controls.Add(this.combWeekTo);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.combWeekFrom);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.combYearTo);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.combYearFrom);
            this.panel1.Location = new System.Drawing.Point(22, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(663, 282);
            this.panel1.TabIndex = 26;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(14, 234);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 38;
            // 
            // btnExportColoaderPO
            // 
            this.btnExportColoaderPO.Font = new System.Drawing.Font("Arial", 12F);
            this.btnExportColoaderPO.Location = new System.Drawing.Point(317, 223);
            this.btnExportColoaderPO.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnExportColoaderPO.Name = "btnExportColoaderPO";
            this.btnExportColoaderPO.Size = new System.Drawing.Size(317, 33);
            this.btnExportColoaderPO.TabIndex = 37;
            this.btnExportColoaderPO.Text = "Export Master Loader Compare";
            this.btnExportColoaderPO.UseVisualStyleBackColor = true;
            this.btnExportColoaderPO.Click += new System.EventHandler(this.btnExportColoaderPO_Click);
            // 
            // txtSelReport
            // 
            this.txtSelReport.Location = new System.Drawing.Point(38, 174);
            this.txtSelReport.Name = "txtSelReport";
            this.txtSelReport.Size = new System.Drawing.Size(418, 25);
            this.txtSelReport.TabIndex = 36;
            // 
            // btnSelectRpt
            // 
            this.btnSelectRpt.Font = new System.Drawing.Font("Arial", 12F);
            this.btnSelectRpt.Location = new System.Drawing.Point(462, 166);
            this.btnSelectRpt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectRpt.Name = "btnSelectRpt";
            this.btnSelectRpt.Size = new System.Drawing.Size(172, 33);
            this.btnSelectRpt.TabIndex = 35;
            this.btnSelectRpt.Text = "Select Report";
            this.btnSelectRpt.UseVisualStyleBackColor = true;
            this.btnSelectRpt.Click += new System.EventHandler(this.btnSelectRpt_Click);
            // 
            // BtnRun
            // 
            this.BtnRun.Font = new System.Drawing.Font("Arial", 12F);
            this.BtnRun.Location = new System.Drawing.Point(317, 107);
            this.BtnRun.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(317, 33);
            this.BtnRun.TabIndex = 34;
            this.BtnRun.Text = "Export  Monthly Report";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // combWeekTo
            // 
            this.combWeekTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combWeekTo.Font = new System.Drawing.Font("Arial", 12F);
            this.combWeekTo.FormattingEnabled = true;
            this.combWeekTo.Location = new System.Drawing.Point(479, 54);
            this.combWeekTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combWeekTo.Name = "combWeekTo";
            this.combWeekTo.Size = new System.Drawing.Size(155, 31);
            this.combWeekTo.TabIndex = 33;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 12F);
            this.label4.Location = new System.Drawing.Point(345, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 23);
            this.label4.TabIndex = 32;
            this.label4.Text = "Month To:";
            // 
            // combWeekFrom
            // 
            this.combWeekFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combWeekFrom.Font = new System.Drawing.Font("Arial", 12F);
            this.combWeekFrom.FormattingEnabled = true;
            this.combWeekFrom.Location = new System.Drawing.Point(479, 11);
            this.combWeekFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combWeekFrom.Name = "combWeekFrom";
            this.combWeekFrom.Size = new System.Drawing.Size(155, 31);
            this.combWeekFrom.TabIndex = 31;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F);
            this.label3.Location = new System.Drawing.Point(345, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 23);
            this.label3.TabIndex = 30;
            this.label3.Text = "Month From:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F);
            this.label1.Location = new System.Drawing.Point(35, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 23);
            this.label1.TabIndex = 29;
            this.label1.Text = "Year To:";
            // 
            // combYearTo
            // 
            this.combYearTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combYearTo.Font = new System.Drawing.Font("Arial", 12F);
            this.combYearTo.FormattingEnabled = true;
            this.combYearTo.Location = new System.Drawing.Point(162, 54);
            this.combYearTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combYearTo.Name = "combYearTo";
            this.combYearTo.Size = new System.Drawing.Size(155, 31);
            this.combYearTo.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F);
            this.label2.Location = new System.Drawing.Point(35, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 23);
            this.label2.TabIndex = 27;
            this.label2.Text = "Year From:";
            // 
            // combYearFrom
            // 
            this.combYearFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combYearFrom.Font = new System.Drawing.Font("Arial", 12F);
            this.combYearFrom.FormattingEnabled = true;
            this.combYearFrom.Location = new System.Drawing.Point(162, 11);
            this.combYearFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.combYearFrom.Name = "combYearFrom";
            this.combYearFrom.Size = new System.Drawing.Size(155, 31);
            this.combYearFrom.TabIndex = 26;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 314);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "AirShipmentReport";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnExportColoaderPO;
        private System.Windows.Forms.TextBox txtSelReport;
        private System.Windows.Forms.Button btnSelectRpt;
        private System.Windows.Forms.Button BtnRun;
        private System.Windows.Forms.ComboBox combWeekTo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox combWeekFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combYearTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combYearFrom;
    }
}