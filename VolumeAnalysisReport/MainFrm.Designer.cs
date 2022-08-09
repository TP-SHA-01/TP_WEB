
namespace VolumeAnalysisReport
{
    partial class MainFrm
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
            this.label1 = new System.Windows.Forms.Label();
            this.dt_ETDTo = new System.Windows.Forms.DateTimePicker();
            this.dt_ETDFrom = new System.Windows.Forms.DateTimePicker();
            this.lb_ETD = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_AgencyAgreement = new System.Windows.Forms.Button();
            this.txt_AgencyAgreement = new System.Windows.Forms.TextBox();
            this.lb_AgencyAgreement = new System.Windows.Forms.Label();
            this.btn_RemarkUpload = new System.Windows.Forms.Button();
            this.btn_TitanUpload = new System.Windows.Forms.Button();
            this.txt_RemarkFIleName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_TitanFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Create = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dt_ETDTo);
            this.groupBox1.Controls.Add(this.dt_ETDFrom);
            this.groupBox1.Controls.Add(this.lb_ETD);
            this.groupBox1.Location = new System.Drawing.Point(12, 282);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 107);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "To";
            // 
            // dt_ETDTo
            // 
            this.dt_ETDTo.CustomFormat = "MM/dd/yyyy";
            this.dt_ETDTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_ETDTo.Location = new System.Drawing.Point(109, 60);
            this.dt_ETDTo.Name = "dt_ETDTo";
            this.dt_ETDTo.Size = new System.Drawing.Size(200, 25);
            this.dt_ETDTo.TabIndex = 2;
            this.dt_ETDTo.Value = new System.DateTime(2022, 7, 20, 0, 0, 0, 0);
            // 
            // dt_ETDFrom
            // 
            this.dt_ETDFrom.CustomFormat = "MM/dd/yyyy";
            this.dt_ETDFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_ETDFrom.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dt_ETDFrom.Location = new System.Drawing.Point(109, 17);
            this.dt_ETDFrom.Name = "dt_ETDFrom";
            this.dt_ETDFrom.Size = new System.Drawing.Size(200, 25);
            this.dt_ETDFrom.TabIndex = 1;
            this.dt_ETDFrom.Value = new System.DateTime(2022, 7, 20, 0, 0, 0, 0);
            // 
            // lb_ETD
            // 
            this.lb_ETD.AutoSize = true;
            this.lb_ETD.Location = new System.Drawing.Point(16, 21);
            this.lb_ETD.Name = "lb_ETD";
            this.lb_ETD.Size = new System.Drawing.Size(87, 15);
            this.lb_ETD.TabIndex = 0;
            this.lb_ETD.Text = "ETD : From";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_AgencyAgreement);
            this.groupBox2.Controls.Add(this.txt_AgencyAgreement);
            this.groupBox2.Controls.Add(this.lb_AgencyAgreement);
            this.groupBox2.Controls.Add(this.btn_RemarkUpload);
            this.groupBox2.Controls.Add(this.btn_TitanUpload);
            this.groupBox2.Controls.Add(this.txt_RemarkFIleName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txt_TitanFileName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 35);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(704, 183);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // btn_AgencyAgreement
            // 
            this.btn_AgencyAgreement.Location = new System.Drawing.Point(550, 120);
            this.btn_AgencyAgreement.Name = "btn_AgencyAgreement";
            this.btn_AgencyAgreement.Size = new System.Drawing.Size(142, 27);
            this.btn_AgencyAgreement.TabIndex = 8;
            this.btn_AgencyAgreement.Text = "Select";
            this.btn_AgencyAgreement.UseVisualStyleBackColor = true;
            this.btn_AgencyAgreement.Click += new System.EventHandler(this.btn_AgencyAgreement_Click);
            // 
            // txt_AgencyAgreement
            // 
            this.txt_AgencyAgreement.Enabled = false;
            this.txt_AgencyAgreement.Location = new System.Drawing.Point(165, 123);
            this.txt_AgencyAgreement.Name = "txt_AgencyAgreement";
            this.txt_AgencyAgreement.ReadOnly = true;
            this.txt_AgencyAgreement.Size = new System.Drawing.Size(379, 25);
            this.txt_AgencyAgreement.TabIndex = 7;
            // 
            // lb_AgencyAgreement
            // 
            this.lb_AgencyAgreement.AutoSize = true;
            this.lb_AgencyAgreement.Location = new System.Drawing.Point(17, 126);
            this.lb_AgencyAgreement.Name = "lb_AgencyAgreement";
            this.lb_AgencyAgreement.Size = new System.Drawing.Size(142, 15);
            this.lb_AgencyAgreement.TabIndex = 6;
            this.lb_AgencyAgreement.Text = "出口代理协议登记表";
            // 
            // btn_RemarkUpload
            // 
            this.btn_RemarkUpload.Location = new System.Drawing.Point(550, 74);
            this.btn_RemarkUpload.Name = "btn_RemarkUpload";
            this.btn_RemarkUpload.Size = new System.Drawing.Size(142, 27);
            this.btn_RemarkUpload.TabIndex = 5;
            this.btn_RemarkUpload.Text = "Select";
            this.btn_RemarkUpload.UseVisualStyleBackColor = true;
            this.btn_RemarkUpload.Click += new System.EventHandler(this.btn_RemarkUpload_Click);
            // 
            // btn_TitanUpload
            // 
            this.btn_TitanUpload.Location = new System.Drawing.Point(550, 20);
            this.btn_TitanUpload.Name = "btn_TitanUpload";
            this.btn_TitanUpload.Size = new System.Drawing.Size(142, 28);
            this.btn_TitanUpload.TabIndex = 4;
            this.btn_TitanUpload.Text = "Select";
            this.btn_TitanUpload.UseVisualStyleBackColor = true;
            this.btn_TitanUpload.Click += new System.EventHandler(this.btn_TitanUpload_Click);
            // 
            // txt_RemarkFIleName
            // 
            this.txt_RemarkFIleName.Enabled = false;
            this.txt_RemarkFIleName.Location = new System.Drawing.Point(165, 76);
            this.txt_RemarkFIleName.Name = "txt_RemarkFIleName";
            this.txt_RemarkFIleName.ReadOnly = true;
            this.txt_RemarkFIleName.Size = new System.Drawing.Size(379, 25);
            this.txt_RemarkFIleName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "File From Remark";
            // 
            // txt_TitanFileName
            // 
            this.txt_TitanFileName.Enabled = false;
            this.txt_TitanFileName.Location = new System.Drawing.Point(165, 24);
            this.txt_TitanFileName.Name = "txt_TitanFileName";
            this.txt_TitanFileName.ReadOnly = true;
            this.txt_TitanFileName.Size = new System.Drawing.Size(379, 25);
            this.txt_TitanFileName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(127, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "File From Titan";
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(562, 224);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(154, 56);
            this.btn_Create.TabIndex = 2;
            this.btn_Create.Text = "Create";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(677, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "v1.4";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 292);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_Create);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Weekly Oustanding Report Tool";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lb_ETD;
        private System.Windows.Forms.DateTimePicker dt_ETDTo;
        private System.Windows.Forms.DateTimePicker dt_ETDFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.Button btn_RemarkUpload;
        private System.Windows.Forms.Button btn_TitanUpload;
        private System.Windows.Forms.TextBox txt_RemarkFIleName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_TitanFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_AgencyAgreement;
        private System.Windows.Forms.TextBox txt_AgencyAgreement;
        private System.Windows.Forms.Label lb_AgencyAgreement;
    }
}

