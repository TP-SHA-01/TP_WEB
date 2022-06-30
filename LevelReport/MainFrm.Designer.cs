
namespace LevelReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.cmb_Carrier = new BaseComboBox();
            this.cmb_ReportType = new System.Windows.Forms.ComboBox();
            this.gb_SearchCondition = new System.Windows.Forms.GroupBox();
            this.rd_BookingType_NONTP = new System.Windows.Forms.RadioButton();
            this.rd_BookingType_TP = new System.Windows.Forms.RadioButton();
            this.dt_ETDTo = new System.Windows.Forms.DateTimePicker();
            this.lb_To = new System.Windows.Forms.Label();
            this.dt_ETDFrom = new System.Windows.Forms.DateTimePicker();
            this.lb_ETD = new System.Windows.Forms.Label();
            this.lb_Status = new System.Windows.Forms.Label();
            this.lb_Traffic = new System.Windows.Forms.Label();
            this.lb_Carrier = new System.Windows.Forms.Label();
            this.lb_ReportType = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_Create = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_Status = new LevelReport.BaseComboBox();
            this.cmb_Traffic = new LevelReport.BaseComboBox();
            this.gb_SearchCondition.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmb_Carrier
            // 
            this.cmb_Carrier.ColumnHeaderVisible = true;
            this.cmb_Carrier.DisplayMember = null;
            this.cmb_Carrier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Carrier.FormattingEnabled = true;
            this.cmb_Carrier.Items.AddRange(new object[] {
            ""});
            this.cmb_Carrier.Location = new System.Drawing.Point(133, 63);
            this.cmb_Carrier.MaxDropDownItems = 10;
            this.cmb_Carrier.MultiSelect = false;
            this.cmb_Carrier.Name = "cmb_Carrier";
            this.cmb_Carrier.RowHeaderVisible = true;
            this.cmb_Carrier.SelectedValue = null;
            this.cmb_Carrier.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("cmb_Carrier.SelectedValues")));
            this.cmb_Carrier.Size = new System.Drawing.Size(281, 23);
            this.cmb_Carrier.TabIndex = 0;
            this.cmb_Carrier.ValueMember = null;
            this.cmb_Carrier.SelectedValueChanged += new System.EventHandler(this.cmb_Carrier_SelectedValueChanged);
            // 
            // cmb_ReportType
            // 
            this.cmb_ReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_ReportType.FormattingEnabled = true;
            this.cmb_ReportType.Location = new System.Drawing.Point(133, 24);
            this.cmb_ReportType.Name = "cmb_ReportType";
            this.cmb_ReportType.Size = new System.Drawing.Size(281, 23);
            this.cmb_ReportType.TabIndex = 1;
            this.cmb_ReportType.SelectedValueChanged += new System.EventHandler(this.cmb_ReportType_SelectedValueChanged);
            // 
            // gb_SearchCondition
            // 
            this.gb_SearchCondition.Controls.Add(this.rd_BookingType_NONTP);
            this.gb_SearchCondition.Controls.Add(this.rd_BookingType_TP);
            this.gb_SearchCondition.Controls.Add(this.dt_ETDTo);
            this.gb_SearchCondition.Controls.Add(this.lb_To);
            this.gb_SearchCondition.Controls.Add(this.dt_ETDFrom);
            this.gb_SearchCondition.Controls.Add(this.lb_ETD);
            this.gb_SearchCondition.Controls.Add(this.lb_Status);
            this.gb_SearchCondition.Controls.Add(this.cmb_Status);
            this.gb_SearchCondition.Controls.Add(this.lb_Traffic);
            this.gb_SearchCondition.Controls.Add(this.cmb_Traffic);
            this.gb_SearchCondition.Controls.Add(this.lb_Carrier);
            this.gb_SearchCondition.Controls.Add(this.lb_ReportType);
            this.gb_SearchCondition.Controls.Add(this.cmb_Carrier);
            this.gb_SearchCondition.Controls.Add(this.cmb_ReportType);
            this.gb_SearchCondition.Location = new System.Drawing.Point(12, 27);
            this.gb_SearchCondition.Name = "gb_SearchCondition";
            this.gb_SearchCondition.Size = new System.Drawing.Size(881, 192);
            this.gb_SearchCondition.TabIndex = 2;
            this.gb_SearchCondition.TabStop = false;
            this.gb_SearchCondition.Text = "Search Condition";
            // 
            // rd_BookingType_NONTP
            // 
            this.rd_BookingType_NONTP.AutoSize = true;
            this.rd_BookingType_NONTP.Location = new System.Drawing.Point(634, 109);
            this.rd_BookingType_NONTP.Name = "rd_BookingType_NONTP";
            this.rd_BookingType_NONTP.Size = new System.Drawing.Size(76, 19);
            this.rd_BookingType_NONTP.TabIndex = 15;
            this.rd_BookingType_NONTP.Text = "NON-TP";
            this.rd_BookingType_NONTP.UseVisualStyleBackColor = true;
            this.rd_BookingType_NONTP.Visible = false;
            this.rd_BookingType_NONTP.CheckedChanged += new System.EventHandler(this.radioBtn_CheckedChange);
            // 
            // rd_BookingType_TP
            // 
            this.rd_BookingType_TP.AutoSize = true;
            this.rd_BookingType_TP.Checked = true;
            this.rd_BookingType_TP.Location = new System.Drawing.Point(571, 109);
            this.rd_BookingType_TP.Name = "rd_BookingType_TP";
            this.rd_BookingType_TP.Size = new System.Drawing.Size(44, 19);
            this.rd_BookingType_TP.TabIndex = 14;
            this.rd_BookingType_TP.TabStop = true;
            this.rd_BookingType_TP.Text = "TP";
            this.rd_BookingType_TP.UseVisualStyleBackColor = true;
            this.rd_BookingType_TP.Visible = false;
            this.rd_BookingType_TP.CheckedChanged += new System.EventHandler(this.radioBtn_CheckedChange);
            // 
            // dt_ETDTo
            // 
            this.dt_ETDTo.CustomFormat = "MM/dd/yyyy";
            this.dt_ETDTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_ETDTo.Location = new System.Drawing.Point(133, 143);
            this.dt_ETDTo.Name = "dt_ETDTo";
            this.dt_ETDTo.Size = new System.Drawing.Size(219, 25);
            this.dt_ETDTo.TabIndex = 11;
            this.dt_ETDTo.Value = new System.DateTime(2022, 6, 27, 0, 0, 0, 0);
            // 
            // lb_To
            // 
            this.lb_To.AutoSize = true;
            this.lb_To.Location = new System.Drawing.Point(104, 150);
            this.lb_To.Name = "lb_To";
            this.lb_To.Size = new System.Drawing.Size(23, 15);
            this.lb_To.TabIndex = 10;
            this.lb_To.Text = "To";
            // 
            // dt_ETDFrom
            // 
            this.dt_ETDFrom.CustomFormat = "MM/dd/yyyy";
            this.dt_ETDFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_ETDFrom.Location = new System.Drawing.Point(133, 109);
            this.dt_ETDFrom.Name = "dt_ETDFrom";
            this.dt_ETDFrom.Size = new System.Drawing.Size(219, 25);
            this.dt_ETDFrom.TabIndex = 9;
            this.dt_ETDFrom.Value = new System.DateTime(2022, 6, 27, 0, 0, 0, 0);
            // 
            // lb_ETD
            // 
            this.lb_ETD.AutoSize = true;
            this.lb_ETD.Location = new System.Drawing.Point(40, 116);
            this.lb_ETD.Name = "lb_ETD";
            this.lb_ETD.Size = new System.Drawing.Size(87, 15);
            this.lb_ETD.TabIndex = 8;
            this.lb_ETD.Text = "ETD : From";
            // 
            // lb_Status
            // 
            this.lb_Status.AutoSize = true;
            this.lb_Status.Location = new System.Drawing.Point(430, 27);
            this.lb_Status.Name = "lb_Status";
            this.lb_Status.Size = new System.Drawing.Size(135, 15);
            this.lb_Status.TabIndex = 7;
            this.lb_Status.Text = "Booking Status :";
            // 
            // lb_Traffic
            // 
            this.lb_Traffic.AutoSize = true;
            this.lb_Traffic.Location = new System.Drawing.Point(486, 66);
            this.lb_Traffic.Name = "lb_Traffic";
            this.lb_Traffic.Size = new System.Drawing.Size(79, 15);
            this.lb_Traffic.TabIndex = 5;
            this.lb_Traffic.Text = "Traffic :";
            // 
            // lb_Carrier
            // 
            this.lb_Carrier.AutoSize = true;
            this.lb_Carrier.Location = new System.Drawing.Point(48, 66);
            this.lb_Carrier.Name = "lb_Carrier";
            this.lb_Carrier.Size = new System.Drawing.Size(79, 15);
            this.lb_Carrier.TabIndex = 3;
            this.lb_Carrier.Text = "Carrier :";
            // 
            // lb_ReportType
            // 
            this.lb_ReportType.AutoSize = true;
            this.lb_ReportType.Location = new System.Drawing.Point(16, 27);
            this.lb_ReportType.Name = "lb_ReportType";
            this.lb_ReportType.Size = new System.Drawing.Size(111, 15);
            this.lb_ReportType.TabIndex = 2;
            this.lb_ReportType.Text = "Report Type :";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_Create);
            this.groupBox1.Location = new System.Drawing.Point(12, 225);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(881, 112);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(652, 24);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(195, 70);
            this.btn_Create.TabIndex = 0;
            this.btn_Create.Text = "Create Report";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(838, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "v1.1";
            // 
            // cmb_Status
            // 
            this.cmb_Status.ColumnHeaderVisible = true;
            this.cmb_Status.DisplayMember = null;
            this.cmb_Status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Status.FormattingEnabled = true;
            this.cmb_Status.Items.AddRange(new object[] {
            ""});
            this.cmb_Status.Location = new System.Drawing.Point(571, 24);
            this.cmb_Status.MaxDropDownItems = 10;
            this.cmb_Status.MultiSelect = false;
            this.cmb_Status.Name = "cmb_Status";
            this.cmb_Status.RowHeaderVisible = true;
            this.cmb_Status.SelectedValue = null;
            this.cmb_Status.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("cmb_Status.SelectedValues")));
            this.cmb_Status.Size = new System.Drawing.Size(276, 23);
            this.cmb_Status.TabIndex = 6;
            this.cmb_Status.ValueMember = null;
            this.cmb_Status.SelectedValueChanged += new System.EventHandler(this.cmb_Status_SelectedValueChanged);
            // 
            // cmb_Traffic
            // 
            this.cmb_Traffic.ColumnHeaderVisible = true;
            this.cmb_Traffic.DisplayMember = null;
            this.cmb_Traffic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Traffic.FormattingEnabled = true;
            this.cmb_Traffic.Items.AddRange(new object[] {
            ""});
            this.cmb_Traffic.Location = new System.Drawing.Point(571, 63);
            this.cmb_Traffic.MaxDropDownItems = 10;
            this.cmb_Traffic.MultiSelect = false;
            this.cmb_Traffic.Name = "cmb_Traffic";
            this.cmb_Traffic.RowHeaderVisible = true;
            this.cmb_Traffic.SelectedValue = null;
            this.cmb_Traffic.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("cmb_Traffic.SelectedValues")));
            this.cmb_Traffic.Size = new System.Drawing.Size(276, 23);
            this.cmb_Traffic.TabIndex = 4;
            this.cmb_Traffic.ValueMember = null;
            this.cmb_Traffic.SelectedValueChanged += new System.EventHandler(this.cmb_Traffic_SelectedValueChanged);
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 351);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb_SearchCondition);
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report Tool";
            this.gb_SearchCondition.ResumeLayout(false);
            this.gb_SearchCondition.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BaseComboBox cmb_Carrier;
        private System.Windows.Forms.ComboBox cmb_ReportType;
        private System.Windows.Forms.GroupBox gb_SearchCondition;
        private System.Windows.Forms.Label lb_ReportType;
        private System.Windows.Forms.Label lb_Carrier;
        private BaseComboBox cmb_Traffic;
        private System.Windows.Forms.Label lb_Traffic;
        private BaseComboBox cmb_Status;
        private System.Windows.Forms.Label lb_Status;
        private System.Windows.Forms.Label lb_ETD;
        private System.Windows.Forms.DateTimePicker dt_ETDFrom;
        private System.Windows.Forms.Label lb_To;
        private System.Windows.Forms.DateTimePicker dt_ETDTo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rd_BookingType_NONTP;
        private System.Windows.Forms.RadioButton rd_BookingType_TP;
    }
}