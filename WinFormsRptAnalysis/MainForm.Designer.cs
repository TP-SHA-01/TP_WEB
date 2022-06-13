
namespace WinFormsRptAnalysis
{
    partial class MainForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.combYearFrom = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.combYearTo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.combWeekFrom = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.combWeekTo = new System.Windows.Forms.ComboBox();
            this.combRptType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BtnRun = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.barStatus = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(32, 152);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Year From:";
            
            // 
            // combYearFrom
            // 
            this.combYearFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combYearFrom.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.combYearFrom.FormattingEnabled = true;
            this.combYearFrom.Location = new System.Drawing.Point(204, 149);
            this.combYearFrom.Name = "combYearFrom";
            this.combYearFrom.Size = new System.Drawing.Size(174, 28);
            this.combYearFrom.TabIndex = 5;
            
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(32, 203);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Year To:";
            
            // 
            // combYearTo
            // 
            this.combYearTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combYearTo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.combYearTo.FormattingEnabled = true;
            this.combYearTo.Location = new System.Drawing.Point(204, 200);
            this.combYearTo.Name = "combYearTo";
            this.combYearTo.Size = new System.Drawing.Size(174, 28);
            this.combYearTo.TabIndex = 7;
            
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(396, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Week From:";
            
            // 
            // combWeekFrom
            // 
            this.combWeekFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combWeekFrom.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.combWeekFrom.FormattingEnabled = true;
            this.combWeekFrom.Location = new System.Drawing.Point(531, 149);
            this.combWeekFrom.Name = "combWeekFrom";
            this.combWeekFrom.Size = new System.Drawing.Size(174, 28);
            this.combWeekFrom.TabIndex = 10;
            
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(396, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "Week To:";
            
            // 
            // combWeekTo
            // 
            this.combWeekTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combWeekTo.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.combWeekTo.FormattingEnabled = true;
            this.combWeekTo.Location = new System.Drawing.Point(531, 200);
            this.combWeekTo.Name = "combWeekTo";
            this.combWeekTo.Size = new System.Drawing.Size(174, 28);
            this.combWeekTo.TabIndex = 12;
            
            // 
            // combRptType
            // 
            this.combRptType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combRptType.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.combRptType.FormattingEnabled = true;
            this.combRptType.Location = new System.Drawing.Point(204, 95);
            this.combRptType.Name = "combRptType";
            this.combRptType.Size = new System.Drawing.Size(501, 28);
            this.combRptType.TabIndex = 13;
            
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(32, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 20);
            this.label5.TabIndex = 14;
            this.label5.Text = "Report Type:";
            
            // 
            // BtnRun
            // 
            this.BtnRun.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BtnRun.Location = new System.Drawing.Point(531, 272);
            this.BtnRun.Name = "BtnRun";
            this.BtnRun.Size = new System.Drawing.Size(174, 44);
            this.BtnRun.TabIndex = 15;
            this.BtnRun.Text = "Analysis";
            this.BtnRun.UseVisualStyleBackColor = true;
            this.BtnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(44, 247);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 20);
            this.lblStatus.TabIndex = 16;
            
            // 
            // barStatus
            // 
            this.barStatus.Location = new System.Drawing.Point(36, 513);
            this.barStatus.Name = "barStatus";
            this.barStatus.Size = new System.Drawing.Size(622, 29);
            this.barStatus.TabIndex = 17;
            
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(32, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(149, 20);
            this.label6.TabIndex = 18;
            this.label6.Text = "Origin Office:";
            
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.Location = new System.Drawing.Point(204, 43);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(246, 30);
            this.textBox1.TabIndex = 19;
            this.textBox1.Text = "GZO, HKG, SZN, ZHG";
            
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 349);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.barStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.BtnRun);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.combRptType);
            this.Controls.Add(this.combWeekTo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.combWeekFrom);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.combYearTo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.combYearFrom);
            this.Name = "MainForm";
            this.Text = "TBS Booking Advice Report - Analysis";
            this.Load += new System.EventHandler(this.MainFrom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox combYearFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combYearTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox combWeekFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox combWeekTo;
        private System.Windows.Forms.ComboBox combRptType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BtnRun;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar barStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
    }
}

