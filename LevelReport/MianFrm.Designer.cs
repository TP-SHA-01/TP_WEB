
namespace LevelReport
{
    partial class MianFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MianFrm));
            this.lb_Carrier = new System.Windows.Forms.Label();
            this.lb_Traffic = new System.Windows.Forms.Label();
            this.lb_Status = new System.Windows.Forms.Label();
            this.cmb_Status = new LevelReport.BaseComboBox();
            this.cmb_Traffic = new LevelReport.BaseComboBox();
            this.cmb_Carrier = new LevelReport.BaseComboBox();
            this.lb_ETD = new System.Windows.Forms.Label();
            this.dt_ETDFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dt_ETDTo = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // lb_Carrier
            // 
            this.lb_Carrier.AutoSize = true;
            this.lb_Carrier.Location = new System.Drawing.Point(12, 15);
            this.lb_Carrier.Name = "lb_Carrier";
            this.lb_Carrier.Size = new System.Drawing.Size(79, 15);
            this.lb_Carrier.TabIndex = 1;
            this.lb_Carrier.Text = "Carrier :";
            // 
            // lb_Traffic
            // 
            this.lb_Traffic.AutoSize = true;
            this.lb_Traffic.Location = new System.Drawing.Point(12, 55);
            this.lb_Traffic.Name = "lb_Traffic";
            this.lb_Traffic.Size = new System.Drawing.Size(79, 15);
            this.lb_Traffic.TabIndex = 2;
            this.lb_Traffic.Text = "Traffic :";
            // 
            // lb_Status
            // 
            this.lb_Status.AutoSize = true;
            this.lb_Status.Location = new System.Drawing.Point(386, 15);
            this.lb_Status.Name = "lb_Status";
            this.lb_Status.Size = new System.Drawing.Size(135, 15);
            this.lb_Status.TabIndex = 4;
            this.lb_Status.Text = "Booking Status :";
            // 
            // cmb_Status
            // 
            this.cmb_Status.ColumnHeaderVisible = true;
            this.cmb_Status.DisplayMember = null;
            this.cmb_Status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Status.FormattingEnabled = true;
            this.cmb_Status.Items.AddRange(new object[] {
            ""});
            this.cmb_Status.Location = new System.Drawing.Point(528, 12);
            this.cmb_Status.MaxDropDownItems = 10;
            this.cmb_Status.MultiSelect = false;
            this.cmb_Status.Name = "cmb_Status";
            this.cmb_Status.RowHeaderVisible = true;
            this.cmb_Status.SelectedValue = null;
            this.cmb_Status.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("cmb_Status.SelectedValues")));
            this.cmb_Status.Size = new System.Drawing.Size(245, 23);
            this.cmb_Status.TabIndex = 5;
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
            this.cmb_Traffic.Location = new System.Drawing.Point(97, 52);
            this.cmb_Traffic.MaxDropDownItems = 10;
            this.cmb_Traffic.MultiSelect = false;
            this.cmb_Traffic.Name = "cmb_Traffic";
            this.cmb_Traffic.RowHeaderVisible = true;
            this.cmb_Traffic.SelectedValue = null;
            this.cmb_Traffic.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("cmb_Traffic.SelectedValues")));
            this.cmb_Traffic.Size = new System.Drawing.Size(259, 23);
            this.cmb_Traffic.TabIndex = 3;
            this.cmb_Traffic.ValueMember = null;
            this.cmb_Traffic.SelectedValueChanged += new System.EventHandler(this.cmb_Traffic_SelectedValueChanged);
            // 
            // cmb_Carrier
            // 
            this.cmb_Carrier.ColumnHeaderVisible = true;
            this.cmb_Carrier.DisplayMember = null;
            this.cmb_Carrier.DropDownHeight = 150;
            this.cmb_Carrier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_Carrier.FormattingEnabled = true;
            this.cmb_Carrier.IntegralHeight = false;
            this.cmb_Carrier.Items.AddRange(new object[] {
            ""});
            this.cmb_Carrier.Location = new System.Drawing.Point(97, 12);
            this.cmb_Carrier.MaxDropDownItems = 10;
            this.cmb_Carrier.MultiSelect = false;
            this.cmb_Carrier.Name = "cmb_Carrier";
            this.cmb_Carrier.RowHeaderVisible = true;
            this.cmb_Carrier.SelectedValue = null;
            this.cmb_Carrier.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("cmb_Carrier.SelectedValues")));
            this.cmb_Carrier.Size = new System.Drawing.Size(259, 23);
            this.cmb_Carrier.TabIndex = 0;
            this.cmb_Carrier.ValueMember = null;
            this.cmb_Carrier.SelectedValueChanged += new System.EventHandler(this.boxCombox_SelectedValueChanged);
            // 
            // lb_ETD
            // 
            this.lb_ETD.AutoSize = true;
            this.lb_ETD.Location = new System.Drawing.Point(386, 55);
            this.lb_ETD.Name = "lb_ETD";
            this.lb_ETD.Size = new System.Drawing.Size(87, 15);
            this.lb_ETD.TabIndex = 6;
            this.lb_ETD.Text = "ETD : From";
            // 
            // dt_ETDFrom
            // 
            this.dt_ETDFrom.CustomFormat = "MM/dd/yyyy";
            this.dt_ETDFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_ETDFrom.Location = new System.Drawing.Point(479, 48);
            this.dt_ETDFrom.Name = "dt_ETDFrom";
            this.dt_ETDFrom.Size = new System.Drawing.Size(130, 25);
            this.dt_ETDFrom.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(615, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "To";
            // 
            // dt_ETDTo
            // 
            this.dt_ETDTo.CustomFormat = "MM/dd/yyyy";
            this.dt_ETDTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dt_ETDTo.Location = new System.Drawing.Point(643, 48);
            this.dt_ETDTo.Name = "dt_ETDTo";
            this.dt_ETDTo.Size = new System.Drawing.Size(130, 25);
            this.dt_ETDTo.TabIndex = 9;
            // 
            // MianFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(785, 409);
            this.Controls.Add(this.dt_ETDTo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dt_ETDFrom);
            this.Controls.Add(this.lb_ETD);
            this.Controls.Add(this.cmb_Status);
            this.Controls.Add(this.lb_Status);
            this.Controls.Add(this.cmb_Traffic);
            this.Controls.Add(this.lb_Traffic);
            this.Controls.Add(this.lb_Carrier);
            this.Controls.Add(this.cmb_Carrier);
            this.Name = "MianFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MianFrm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BaseComboBox cmb_Carrier;
        private System.Windows.Forms.Label lb_Carrier;
        private System.Windows.Forms.Label lb_Traffic;
        private BaseComboBox cmb_Traffic;
        private System.Windows.Forms.Label lb_Status;
        private BaseComboBox cmb_Status;
        private System.Windows.Forms.Label lb_ETD;
        private System.Windows.Forms.DateTimePicker dt_ETDFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dt_ETDTo;
    }
}