namespace LevelReport
{
    partial class ComboBoxItemControl
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
            this.textKeyword = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labCount = new System.Windows.Forms.Label();
            this.btOk = new System.Windows.Forms.Button();
            this.btCancle = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textKeyword);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(218, 25);
            this.panel1.TabIndex = 3;
            // 
            // textKeyword
            // 
            this.textKeyword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textKeyword.Location = new System.Drawing.Point(0, 0);
            this.textKeyword.Margin = new System.Windows.Forms.Padding(4);
            this.textKeyword.MaxLength = 200;
            this.textKeyword.Multiline = true;
            this.textKeyword.Name = "textKeyword";
            this.textKeyword.Size = new System.Drawing.Size(218, 25);
            this.textKeyword.TabIndex = 0;
            this.textKeyword.TextChanged += new System.EventHandler(this.textKeyword_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labCount);
            this.panel2.Controls.Add(this.btOk);
            this.panel2.Controls.Add(this.btCancle);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 231);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(218, 25);
            this.panel2.TabIndex = 5;
            // 
            // labCount
            // 
            this.labCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labCount.Font = new System.Drawing.Font("SimSun", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labCount.Location = new System.Drawing.Point(0, 0);
            this.labCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labCount.Name = "labCount";
            this.labCount.Size = new System.Drawing.Size(69, 25);
            this.labCount.TabIndex = 2;
            this.labCount.Text = "select:0/0";
            this.labCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btOk
            // 
            this.btOk.Dock = System.Windows.Forms.DockStyle.Right;
            this.btOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btOk.Font = new System.Drawing.Font("SimSun", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btOk.Location = new System.Drawing.Point(69, 0);
            this.btOk.Margin = new System.Windows.Forms.Padding(4, 4, 13, 4);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(76, 25);
            this.btOk.TabIndex = 1;
            this.btOk.Text = "select";
            this.btOk.UseVisualStyleBackColor = true;
            this.btOk.Click += new System.EventHandler(this.btOk_Click);
            // 
            // btCancle
            // 
            this.btCancle.Dock = System.Windows.Forms.DockStyle.Right;
            this.btCancle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btCancle.Font = new System.Drawing.Font("SimSun", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btCancle.Location = new System.Drawing.Point(145, 0);
            this.btCancle.Margin = new System.Windows.Forms.Padding(4);
            this.btCancle.Name = "btCancle";
            this.btCancle.Size = new System.Drawing.Size(73, 25);
            this.btCancle.TabIndex = 0;
            this.btCancle.Text = "cancle";
            this.btCancle.UseVisualStyleBackColor = true;
            this.btCancle.Click += new System.EventHandler(this.btCancle_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.dgvData);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(220, 258);
            this.panel3.TabIndex = 7;
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToResizeRows = false;
            this.dgvData.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 25);
            this.dgvData.Margin = new System.Windows.Forms.Padding(4);
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersWidth = 30;
            this.dgvData.RowTemplate.Height = 23;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(218, 206);
            this.dgvData.TabIndex = 6;
            this.dgvData.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvData_CellMouseClick);
            this.dgvData.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvData_RowPostPaint);
            this.dgvData.SelectionChanged += new System.EventHandler(this.dgvData_SelectionChanged);
            this.dgvData.DoubleClick += new System.EventHandler(this.dgvData_DoubleClick);
            // 
            // ComboBoxItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 258);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ComboBoxItemControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ComboBoxItemControl";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textKeyword;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labCount;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btCancle;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dgvData;
    }
}