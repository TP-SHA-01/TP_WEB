using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelReport
{
    /// <summary>
    /// 定义一个控件，用于显示所有下拉选项
    /// </summary>
    internal partial class ComboBoxItemControl : Form
    {
        private bool _multiSelect = false;

        public ComboBoxItemControl()
        {
            InitializeComponent();
            dgvData.ReadOnly = true;
            dgvData.AutoGenerateColumns = false;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            btOk.Focus();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Deactivate += SelectBoxDataSourceControl_Deactivate;
        }
        /// <summary>
        /// 获取或设置一个值，指示是否允许多选
        /// </summary>
        public bool MultiSelect
        {
            get { return _multiSelect; }
            set
            {
                _multiSelect = value;
                dgvData.MultiSelect = value;
            }
        }
        /// <summary>
        /// 获取或设置被选中对象值的列名称
        /// </summary>
        public string ValueMember { get; set; }
        /// <summary>
        /// 获取或设置被选中对象显示名称的列名称
        /// </summary>
        public string DisplayMember { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值指示是否显示包含列标题的行。
        /// </summary>
        public bool ColumnHeaderVisible { get { return dgvData.ColumnHeadersVisible; } set { dgvData.ColumnHeadersVisible = value; } }
        /// <summary>
        /// 获取或设置一个值，该值指示是否显示包含行标题的列。
        /// </summary>
        public bool RowHeaderVisible { get { return dgvData.RowHeadersVisible; } set { dgvData.RowHeadersVisible = value; } }
        /// <summary>
        /// 获取或设置要在列表的下拉部分中显示的最大项数。
        /// </summary>
        public int MaxDropDownItems { get; set; } = 10;
        /// <summary>
        /// 选中后的事件
        /// </summary>
        public event EventHandler AfterSelect;
        /// <summary>
        /// 取消选择的事件
        /// </summary>
        public event EventHandler CancelSelect;
        /// <summary>
        /// 绑定下拉项的数据源
        /// </summary>
        /// <param name="data">数据源</param>
        /// <param name="viewMember">需要显示到下来列表中的列名称</param>
        public void BandDataSource(DataTable data, List<string> viewMember)
        {
            dgvData.DataSource = null;
            dgvData.Rows.Clear();
            dgvData.Columns.Clear();
            textKeyword.Text = "";
            var datacols = data?.Columns?.OfType<DataColumn>()?.ToDictionary(s => s.ColumnName, s => s.Caption);
            if (datacols == null || datacols.Count == 0)
                return;
            //按用户传入的视图顺序排序
            viewMember = viewMember ?? datacols.Keys.ToList();
            var coldatas = viewMember.Select(s => datacols.FirstOrDefault(m => s == m.Key)).Where(s => string.IsNullOrEmpty(s.Key) == false).ToList();
            coldatas.AddRange(datacols.Where(s => !coldatas.Contains(s)));
            //添加需要选中的列（如果未传入用户自定义的列，则显示数据源中所有列）
            foreach (var datacol in coldatas)
            {
                var name = datacol.Key; var text = string.IsNullOrEmpty(datacol.Value) ? name : datacol.Value;
                var col = new DataGridViewTextBoxColumn() { Name = name, HeaderText = text, DataPropertyName = name };
                col.Visible = viewMember.Contains(name);
                dgvData.Columns.Add(col);
            }
            //绑定列表中数据源数据
            foreach (DataRow row in data.Rows)
            {
                var dgvrow = dgvData.Rows[dgvData.Rows.Add()];
                datacols.Keys.ToList().ForEach(s => dgvrow.Cells[s].Value = row[s]);
            }
            //自动设置列宽（如果各列宽小于控件宽度，则填充显示，否则自动延长dgv宽度）
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            var dgvcolumns = dgvData.Columns.OfType<DataGridViewColumn>().Where(s => s.Visible).ToList();
            var maxwidth = dgvcolumns.Any() ? (dgvcolumns.Sum(s => s.Width) + (RowHeaderVisible ? dgvData.RowHeadersWidth : 0)) : 0;
            var wh = Width * 1.3;
            if (maxwidth < wh)
                dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            else
            {
                foreach (var dgvc in dgvcolumns)
                {
                    dgvc.AutoSizeMode = dgvc.Width > 80 ? DataGridViewAutoSizeColumnMode.NotSet : DataGridViewAutoSizeColumnMode.DisplayedCells;
                    if (dgvc.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
                        dgvc.Width = 80;
                }
            }

            //为dgv增加复选框列
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "Select";
            checkBoxColumn.Name = "IsChecked";
            checkBoxColumn.TrueValue = true;
            checkBoxColumn.FalseValue = false;
            checkBoxColumn.DataPropertyName = "IsChecked";
            checkBoxColumn.Width = 50;
            checkBoxColumn.Resizable = DataGridViewTriState.False;
            this.dgvData.Columns.Insert(0, checkBoxColumn);
        }
        /// <summary>
        /// 获取已选中的值（Key为选中的值，Value为选中的显示名）
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> GetSelectedValues()
        {
            var columns = dgvData.Columns.OfType<DataGridViewColumn>().Select(s => s.Name).ToList();
            if (string.IsNullOrEmpty(ValueMember) || columns.Contains(ValueMember) == false)
                return new List<KeyValuePair<string, string>>();
            var displayMember = string.IsNullOrEmpty(DisplayMember) || !columns.Contains(DisplayMember) ? ValueMember : DisplayMember;

            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            for (int i = 0; i < dgvData.Rows.Count; i++)
            {
                if (dgvData.Rows[i].Cells[0].Value != null)
                {
                    if ((bool)dgvData.Rows[i].Cells[0].Value)
                    {
                        DataGridViewRow row = dgvData.Rows[i];
                        rows.Add(row);
                    }
                }
            }

            var datas = rows.Select(s => new KeyValuePair<string, string>(s.Cells[ValueMember].Value?.ToString() ?? "", s.Cells[displayMember].Value?.ToString() ?? "")).ToList();
            return datas;
        }
        /// <summary>
        /// 设置被选中的项
        /// </summary>
        /// <param name="values"></param>
        public void SetSelectedValues(List<string> values)
        {
            var columns = dgvData.Columns.OfType<DataGridViewColumn>().Select(s => s.Name).ToList();
            if (string.IsNullOrEmpty(ValueMember) || columns.Contains(ValueMember) == false || values == null || values.Any() == false)
            {
                dgvData.ClearSelection();
                return;
            }
            var rows = dgvData.Rows.OfType<DataGridViewRow>().ToList();
            foreach (var s in rows)
            {
                if (values.Contains(s.Cells[ValueMember].Value?.ToString()))
                {
                    s.Selected = true;
                    this.dgvData.Rows[s.Index].Cells[0].Value = true;
                }
            }
        }
        public void ShowControl(List<string> selecteds)
        {
            textKeyword.Text = "";
            this.Show();
            btOk.Focus();
            SetSelectedValues(selecteds);
            var maxcount = MaxDropDownItems < 3 ? 3 : MaxDropDownItems > 50 ? 50 : MaxDropDownItems;//最大显示行数
            var rowheigth = dgvData.RowCount == 0 ? 0 : dgvData.Rows[0].Height;//单行高度
            var otherheight = (ColumnHeaderVisible ? dgvData.ColumnHeadersHeight : 0) + panel1.Height + panel2.Height + 10;//除dwg外的其他高度
            var realheight = rowheigth * dgvData.RowCount + otherheight;//实际高度
            var maxheight = (maxcount * rowheigth) + otherheight;//允许的最大高度
            Height = realheight > maxheight ? maxheight : realheight;
        }
        public int GetControlWidth()
        {
            if (dgvData.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.Fill)
                return Width;
            var columns = dgvData.Columns.OfType<DataGridViewColumn>().Where(s => s.Visible).ToList();
            var w = columns.Any() ? (columns.Sum(s => s.Width) + (RowHeaderVisible ? dgvData.RowHeadersWidth : 0)) : 0;
            return w < (Width * 1.3) ? Width : w > 2 * Width ? 2 * Width : w;
        }

        private void textKeyword_TextChanged(object sender, EventArgs e)
        {
            var keyword = textKeyword.Text;
            var rows = dgvData.Rows.OfType<DataGridViewRow>().ToList();
            if (string.IsNullOrEmpty(keyword))
            {
                rows.ForEach(s => s.Visible = true);
                return;
            }
            var cols = dgvData.Columns.OfType<DataGridViewColumn>().Where(s => s.Visible).ToList();
            rows.ForEach(m => m.Visible = cols.Any(s => (m.Cells[s.Name].Value?.ToString() ?? "").Contains(keyword)));
        }
        private void btOk_Click(object sender, EventArgs e)
        {
            this.Hide();
            AfterSelect?.Invoke(this, e);
        }
        private void btCancle_Click(object sender, EventArgs e)
        {
            this.Hide();
            CancelSelect?.Invoke(this, e);
        }
        private void dgvData_DoubleClick(object sender, EventArgs e)
        {
            btOk_Click(sender, e);
        }
        private void dgvData_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvData == null)
            {
                return;
            }

            int SelectedRowsCount = 0;
            for (int i = 0; i < dgvData.Rows.Count; i++)
            {
                if (dgvData.Rows[i].Cells[0].Value != null)
                {
                    if ((bool)dgvData.Rows[i].Cells[0].Value)
                    {
                        SelectedRowsCount++;
                    }
                }
            }

            labCount.Text = "select:" + SelectedRowsCount.ToString() + "/" + dgvData.Rows.Count;
        }
        private void SelectBoxDataSourceControl_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void dgvData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (dgvData.RowHeadersVisible)
            {
                Rectangle rectangle = new Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, dgvData.RowHeadersWidth - 4, e.RowBounds.Height);
                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), dgvData.RowHeadersDefaultCellStyle.Font, rectangle, dgvData.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
        }

        private void dgvData_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //不是序号列和标题列时才执行
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                //checkbox 勾上
                if ((bool)dgvData.Rows[e.RowIndex].Cells[0].EditedFormattedValue == true)
                {
                    //选中改为不选中
                    this.dgvData.Rows[e.RowIndex].Cells[0].Value = false;
                }
                else
                {
                    //不选中改为选中
                    this.dgvData.Rows[e.RowIndex].Cells[0].Value = true;
                }
            }

            dgvData_SelectionChanged(sender, e);
        }
    }
    
}
