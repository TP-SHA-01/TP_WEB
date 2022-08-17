
namespace OceanTrackingRateAnalysisReport
{
    partial class TrackingRateReport
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
            this.lblCompare = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblOldDate = new System.Windows.Forms.Label();
            this.lblNewDate = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAnalysis = new System.Windows.Forms.Button();
            this.btnSelectNew = new System.Windows.Forms.Button();
            this.txtFilePathNewName = new System.Windows.Forms.TextBox();
            this.btnSelectOld = new System.Windows.Forms.Button();
            this.txtFilePathOldName = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtFilePath2 = new System.Windows.Forms.TextBox();
            this.txtFilePath1 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.lblCompare);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lblOldDate);
            this.panel1.Controls.Add(this.lblNewDate);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.btnAnalysis);
            this.panel1.Controls.Add(this.btnSelectNew);
            this.panel1.Controls.Add(this.txtFilePathNewName);
            this.panel1.Controls.Add(this.btnSelectOld);
            this.panel1.Controls.Add(this.txtFilePathOldName);
            this.panel1.Location = new System.Drawing.Point(14, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(844, 199);
            this.panel1.TabIndex = 26;
            // 
            // lblCompare
            // 
            this.lblCompare.AutoSize = true;
            this.lblCompare.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompare.ForeColor = System.Drawing.Color.Red;
            this.lblCompare.Location = new System.Drawing.Point(258, 150);
            this.lblCompare.Name = "lblCompare";
            this.lblCompare.Size = new System.Drawing.Size(78, 24);
            this.lblCompare.TabIndex = 38;
            this.lblCompare.Text = "Compare";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(362, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 24);
            this.label6.TabIndex = 37;
            this.label6.Text = "Old Date:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(32, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 24);
            this.label5.TabIndex = 36;
            this.label5.Text = "New Date:";
            // 
            // lblOldDate
            // 
            this.lblOldDate.AutoSize = true;
            this.lblOldDate.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOldDate.Location = new System.Drawing.Point(448, 150);
            this.lblOldDate.Name = "lblOldDate";
            this.lblOldDate.Size = new System.Drawing.Size(0, 24);
            this.lblOldDate.TabIndex = 35;
            // 
            // lblNewDate
            // 
            this.lblNewDate.AutoSize = true;
            this.lblNewDate.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewDate.Location = new System.Drawing.Point(154, 150);
            this.lblNewDate.Name = "lblNewDate";
            this.lblNewDate.Size = new System.Drawing.Size(0, 24);
            this.lblNewDate.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(32, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(493, 24);
            this.label3.TabIndex = 33;
            this.label3.Text = "Topocean Trucking Rate Effective on ETD MM dd yyyy FINAL.xls";
            // 
            // btnAnalysis
            // 
            this.btnAnalysis.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.btnAnalysis.Location = new System.Drawing.Point(629, 147);
            this.btnAnalysis.Name = "btnAnalysis";
            this.btnAnalysis.Size = new System.Drawing.Size(170, 37);
            this.btnAnalysis.TabIndex = 32;
            this.btnAnalysis.Text = "Analysis Report";
            this.btnAnalysis.UseVisualStyleBackColor = true;
            this.btnAnalysis.Click += new System.EventHandler(this.btnAnalysis_Click);
            // 
            // btnSelectNew
            // 
            this.btnSelectNew.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.btnSelectNew.Location = new System.Drawing.Point(629, 56);
            this.btnSelectNew.Name = "btnSelectNew";
            this.btnSelectNew.Size = new System.Drawing.Size(170, 37);
            this.btnSelectNew.TabIndex = 30;
            this.btnSelectNew.Tag = "Fdud";
            this.btnSelectNew.Text = "Select New File";
            this.btnSelectNew.UseVisualStyleBackColor = true;
            this.btnSelectNew.Click += new System.EventHandler(this.btnSelectNew_Click);
            // 
            // txtFilePathNewName
            // 
            this.txtFilePathNewName.Location = new System.Drawing.Point(32, 61);
            this.txtFilePathNewName.Name = "txtFilePathNewName";
            this.txtFilePathNewName.ReadOnly = true;
            this.txtFilePathNewName.Size = new System.Drawing.Size(587, 25);
            this.txtFilePathNewName.TabIndex = 29;
            // 
            // btnSelectOld
            // 
            this.btnSelectOld.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.btnSelectOld.Location = new System.Drawing.Point(629, 102);
            this.btnSelectOld.Name = "btnSelectOld";
            this.btnSelectOld.Size = new System.Drawing.Size(170, 37);
            this.btnSelectOld.TabIndex = 27;
            this.btnSelectOld.Text = "Select Old File";
            this.btnSelectOld.UseVisualStyleBackColor = true;
            this.btnSelectOld.Click += new System.EventHandler(this.btnSelectOld_Click);
            // 
            // txtFilePathOldName
            // 
            this.txtFilePathOldName.Location = new System.Drawing.Point(32, 102);
            this.txtFilePathOldName.Name = "txtFilePathOldName";
            this.txtFilePathOldName.ReadOnly = true;
            this.txtFilePathOldName.Size = new System.Drawing.Size(587, 25);
            this.txtFilePathOldName.TabIndex = 26;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(580, 23);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 15);
            this.lblStatus.TabIndex = 27;
            // 
            // txtFilePath2
            // 
            this.txtFilePath2.Location = new System.Drawing.Point(17, 486);
            this.txtFilePath2.Name = "txtFilePath2";
            this.txtFilePath2.Size = new System.Drawing.Size(943, 25);
            this.txtFilePath2.TabIndex = 20;
            // 
            // txtFilePath1
            // 
            this.txtFilePath1.Location = new System.Drawing.Point(17, 443);
            this.txtFilePath1.Name = "txtFilePath1";
            this.txtFilePath1.Size = new System.Drawing.Size(943, 25);
            this.txtFilePath1.TabIndex = 19;
            // 
            // TrackingRateReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 249);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtFilePath2);
            this.Controls.Add(this.txtFilePath1);
            this.Name = "TrackingRateReport";
            this.Text = "TrackingRateReport";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblCompare;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblOldDate;
        private System.Windows.Forms.Label lblNewDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAnalysis;
        private System.Windows.Forms.Button btnSelectNew;
        private System.Windows.Forms.TextBox txtFilePathNewName;
        private System.Windows.Forms.Button btnSelectOld;
        private System.Windows.Forms.TextBox txtFilePathOldName;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtFilePath2;
        private System.Windows.Forms.TextBox txtFilePath1;
    }
}