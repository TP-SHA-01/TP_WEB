
namespace CombineReport
{
    partial class Frm_Main
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.combReportType = new System.Windows.Forms.ComboBox();
            this.lbReportType = new System.Windows.Forms.Label();
            this.lb_FilePath = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCombined = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.combReportType);
            this.groupBox1.Controls.Add(this.lbReportType);
            this.groupBox1.Controls.Add(this.lb_FilePath);
            this.groupBox1.Controls.Add(this.btnSelect);
            this.groupBox1.Controls.Add(this.txtFilePath);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(635, 95);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Setting";
            // 
            // combReportType
            // 
            this.combReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combReportType.FormattingEnabled = true;
            this.combReportType.Location = new System.Drawing.Point(187, 18);
            this.combReportType.Name = "combReportType";
            this.combReportType.Size = new System.Drawing.Size(148, 23);
            this.combReportType.TabIndex = 4;
            // 
            // lbReportType
            // 
            this.lbReportType.AutoSize = true;
            this.lbReportType.Location = new System.Drawing.Point(6, 21);
            this.lbReportType.Name = "lbReportType";
            this.lbReportType.Size = new System.Drawing.Size(103, 15);
            this.lbReportType.TabIndex = 3;
            this.lbReportType.Text = "Report Type:";
            // 
            // lb_FilePath
            // 
            this.lb_FilePath.AutoSize = true;
            this.lb_FilePath.Location = new System.Drawing.Point(6, 58);
            this.lb_FilePath.Name = "lb_FilePath";
            this.lb_FilePath.Size = new System.Drawing.Size(175, 15);
            this.lb_FilePath.TabIndex = 2;
            this.lb_FilePath.Text = "Please Import Folder:";
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(521, 47);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(98, 31);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(187, 52);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(328, 25);
            this.txtFilePath.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnCombined);
            this.groupBox2.Location = new System.Drawing.Point(12, 113);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(635, 61);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btnCombined
            // 
            this.btnCombined.Location = new System.Drawing.Point(521, 15);
            this.btnCombined.Name = "btnCombined";
            this.btnCombined.Size = new System.Drawing.Size(98, 39);
            this.btnCombined.TabIndex = 0;
            this.btnCombined.Text = "Combined";
            this.btnCombined.UseVisualStyleBackColor = true;
            this.btnCombined.Click += new System.EventHandler(this.btnCombined_Click);
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 187);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Combine Report";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnCombined;
        private System.Windows.Forms.Label lb_FilePath;
        private System.Windows.Forms.ComboBox combReportType;
        private System.Windows.Forms.Label lbReportType;
    }
}

