namespace ComboBoxTest
{
    partial class TestForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textSelectresult = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btBandDic = new System.Windows.Forms.Button();
            this.btBandList = new System.Windows.Forms.Button();
            this.btBandTable = new System.Windows.Forms.Button();
            this.boxShowRowHeader = new System.Windows.Forms.CheckBox();
            this.boxShowHeader = new System.Windows.Forms.CheckBox();
            this.textMaxRow = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.boxDescription = new System.Windows.Forms.RichTextBox();
            this.boxCombox = new ComboBoxTest.BaseComboBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textMaxRow)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.textSelectresult);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btBandDic);
            this.groupBox1.Controls.Add(this.btBandList);
            this.groupBox1.Controls.Add(this.btBandTable);
            this.groupBox1.Controls.Add(this.boxShowRowHeader);
            this.groupBox1.Controls.Add(this.boxShowHeader);
            this.groupBox1.Controls.Add(this.textMaxRow);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.boxCombox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(760, 154);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(283, 125);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(148, 29);
            this.button1.TabIndex = 11;
            this.button1.Text = "绑值";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textSelectresult
            // 
            this.textSelectresult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSelectresult.ForeColor = System.Drawing.Color.Red;
            this.textSelectresult.Location = new System.Drawing.Point(115, 65);
            this.textSelectresult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.textSelectresult.Name = "textSelectresult";
            this.textSelectresult.Size = new System.Drawing.Size(620, 31);
            this.textSelectresult.TabIndex = 10;
            this.textSelectresult.Text = "未选择";
            this.textSelectresult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(13, 72);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "选择结果：";
            // 
            // btBandDic
            // 
            this.btBandDic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBandDic.Location = new System.Drawing.Point(595, 98);
            this.btBandDic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btBandDic.Name = "btBandDic";
            this.btBandDic.Size = new System.Drawing.Size(148, 29);
            this.btBandDic.TabIndex = 8;
            this.btBandDic.Text = "绑定Dictionary";
            this.btBandDic.UseVisualStyleBackColor = true;
            this.btBandDic.Click += new System.EventHandler(this.btBandDic_Click);
            // 
            // btBandList
            // 
            this.btBandList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBandList.Location = new System.Drawing.Point(439, 98);
            this.btBandList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btBandList.Name = "btBandList";
            this.btBandList.Size = new System.Drawing.Size(148, 29);
            this.btBandList.TabIndex = 7;
            this.btBandList.Text = "绑定List";
            this.btBandList.UseVisualStyleBackColor = true;
            this.btBandList.Click += new System.EventHandler(this.btBandList_Click);
            // 
            // btBandTable
            // 
            this.btBandTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btBandTable.Location = new System.Drawing.Point(283, 98);
            this.btBandTable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btBandTable.Name = "btBandTable";
            this.btBandTable.Size = new System.Drawing.Size(148, 29);
            this.btBandTable.TabIndex = 6;
            this.btBandTable.Text = "绑定DataTable";
            this.btBandTable.UseVisualStyleBackColor = true;
            this.btBandTable.Click += new System.EventHandler(this.btBandTable_Click);
            // 
            // boxShowRowHeader
            // 
            this.boxShowRowHeader.AutoSize = true;
            this.boxShowRowHeader.Checked = true;
            this.boxShowRowHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.boxShowRowHeader.Location = new System.Drawing.Point(120, 25);
            this.boxShowRowHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.boxShowRowHeader.Name = "boxShowRowHeader";
            this.boxShowRowHeader.Size = new System.Drawing.Size(89, 19);
            this.boxShowRowHeader.TabIndex = 5;
            this.boxShowRowHeader.Text = "显示行头";
            this.boxShowRowHeader.UseVisualStyleBackColor = true;
            this.boxShowRowHeader.CheckedChanged += new System.EventHandler(this.boxShowRowHeader_CheckedChanged);
            // 
            // boxShowHeader
            // 
            this.boxShowHeader.AutoSize = true;
            this.boxShowHeader.Checked = true;
            this.boxShowHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.boxShowHeader.Location = new System.Drawing.Point(16, 25);
            this.boxShowHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.boxShowHeader.Name = "boxShowHeader";
            this.boxShowHeader.Size = new System.Drawing.Size(89, 19);
            this.boxShowHeader.TabIndex = 4;
            this.boxShowHeader.Text = "显示表头";
            this.boxShowHeader.UseVisualStyleBackColor = true;
            this.boxShowHeader.CheckedChanged += new System.EventHandler(this.boxShowHeader_CheckedChanged);
            // 
            // textMaxRow
            // 
            this.textMaxRow.Location = new System.Drawing.Point(303, 24);
            this.textMaxRow.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textMaxRow.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.textMaxRow.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.textMaxRow.Name = "textMaxRow";
            this.textMaxRow.Size = new System.Drawing.Size(77, 25);
            this.textMaxRow.TabIndex = 2;
            this.textMaxRow.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.textMaxRow.ValueChanged += new System.EventHandler(this.textMaxRow_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(224, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "显示行：";
            // 
            // boxDescription
            // 
            this.boxDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxDescription.Location = new System.Drawing.Point(0, 154);
            this.boxDescription.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.boxDescription.Name = "boxDescription";
            this.boxDescription.ReadOnly = true;
            this.boxDescription.Size = new System.Drawing.Size(760, 545);
            this.boxDescription.TabIndex = 1;
            this.boxDescription.Text = "";
            // 
            // boxCombox
            // 
            this.boxCombox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boxCombox.ColumnHeaderVisible = true;
            this.boxCombox.DisplayMember = null;
            this.boxCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.boxCombox.FormattingEnabled = true;
            this.boxCombox.Items.AddRange(new object[] {
            ""});
            this.boxCombox.Location = new System.Drawing.Point(16, 100);
            this.boxCombox.Margin = new System.Windows.Forms.Padding(4);
            this.boxCombox.MaxDropDownItems = 10;
            this.boxCombox.MultiSelect = false;
            this.boxCombox.Name = "boxCombox";
            this.boxCombox.RowHeaderVisible = true;
            this.boxCombox.SelectedValue = null;
            this.boxCombox.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("boxCombox.SelectedValues")));
            this.boxCombox.Size = new System.Drawing.Size(229, 23);
            this.boxCombox.TabIndex = 0;
            this.boxCombox.ValueMember = null;
            this.boxCombox.SelectedValueChanged += new System.EventHandler(this.boxCombox_SelectedValueChanged);
            this.boxCombox.TextUpdate += new System.EventHandler(this.boxCombox_TextUpdate);
            this.boxCombox.TextChanged += new System.EventHandler(this.boxCombox_TextChanged);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 699);
            this.Controls.Add(this.boxDescription);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "下拉多选控件测试窗口";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textMaxRow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btBandDic;
        private System.Windows.Forms.Button btBandList;
        private System.Windows.Forms.Button btBandTable;
        private System.Windows.Forms.CheckBox boxShowRowHeader;
        private System.Windows.Forms.CheckBox boxShowHeader;
        private System.Windows.Forms.NumericUpDown textMaxRow;
        private System.Windows.Forms.Label label1;
        private BaseComboBox boxCombox;
        private System.Windows.Forms.RichTextBox boxDescription;
        private System.Windows.Forms.Label textSelectresult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}

