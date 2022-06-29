using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace LevelReport
{
    public class BaseComboBox : ComboBox
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            boxControl.AfterSelect += boxControl_AfterSelect;
            boxControl.Hide();
        }

        #region 参数定义
        private ComboBoxItemControl boxControl = new ComboBoxItemControl();
        private List<string> _selectedValues = new List<string>();
        private List<string> _selectedTexts = new List<string>();
        private int _selectedIndex = -1;
        private ComboBoxTest.BaseComboBox baseComboBox1;
        ToolTip tip = new ToolTip();
        #endregion

        #region 控件属性
        [Description("获取或设置要在列表的下拉部分中显示的最大项数。")]
        public new int MaxDropDownItems { get { return boxControl.MaxDropDownItems; } set { boxControl.MaxDropDownItems = value; } }
        [Description("获取或设置一个值，该值指示是否显示包含列标题的行。")]
        public bool ColumnHeaderVisible { get { return boxControl.ColumnHeaderVisible; } set { boxControl.ColumnHeaderVisible = value; } }
        [Description("获取或设置一个值，该值指示是否显示包含行标题的列。")]
        public bool RowHeaderVisible { get { return boxControl.RowHeaderVisible; } set { boxControl.RowHeaderVisible = value; } }
        [Description("获取或设置一个值，指示是否允许多选")]
        public bool MultiSelect { get { return boxControl.MultiSelect; } set { boxControl.MultiSelect = value; } }
        [Description("获取或设置要用作中的各项的实际值的属性路径 System.Windows.Forms.ListControl。")]
        public new string ValueMember { get { return boxControl.ValueMember; } set { boxControl.ValueMember = value; } }
        [Description("获取或设置该属性以显示此 System.Windows.Forms.ListControl。")]
        public new string DisplayMember { get { return boxControl.DisplayMember; } set { boxControl.DisplayMember = value; } }
        [Description("获取或设置已多选的值"), Browsable(false)]
        public List<string> SelectedValues
        {
            get { return _selectedValues; }
            set
            {
                var newvalue = value ?? new List<string>();
                _selectedValues = newvalue;
                boxControl.SetSelectedValues(newvalue);
                boxControl_AfterSelect(null, null);
            }
        }
        [Description("获取已选中的显示内容"), Browsable(false)]
        public List<string> SelectedTexts { get { return _selectedTexts; } }
        [Description("获取或设置已选的值"), Browsable(false)]
        public new string SelectedValue { get { return SelectedValues?.FirstOrDefault(); } set { SelectedValues = value == null ? null : new List<string>() { value }; } }
        [Description("获取已选的值"), Browsable(false)]
        public new string SelectedItem { get { return SelectedValue; } }
        [Description("设置数据源"), Browsable(false)]
        public new List<string> DataSource { set { BandSource(value); } }
        public new int SelectedIndex { get { return _selectedIndex; } }
        [Description("当显示下拉选项列表时发生。")]
        public new event EventHandler DropDown;
        [Description("当被选中的值更改时发生。")]
        public new event EventHandler SelectedValueChanged;
        [Description("当被选中的索引更改时发生。"), Browsable(false)]
        public new event EventHandler SelectedIndexChanged;
        #endregion

        #region 绑定数据源
        /// <summary>
        /// 绑定数据表集合（ColumnName为列Name，Caption为列表HeaderText）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="viewColumns"></param>
        public void BandSource(DataTable data, List<string> viewColumns = null)
        {
            _selectedValues?.Clear();
            _selectedTexts?.Clear();
            _selectedIndex = -1;
            boxControl.Width = Width;
            boxControl.Hide();
            boxControl.BandDataSource(data, viewColumns);
        }
        /// <summary>
        /// 绑定字典类型的数据集合（Key为值，Value为显示名）
        /// </summary>
        /// <param name="data"></param>
        public void BandSource(Dictionary<string, string> data)
        {
            var tb = conventToItemTable(data, false);
            ValueMember = "Key";
            DisplayMember = "Value";
            ColumnHeaderVisible = false;
            BandSource(tb, new List<string>() { "Value" });
        }
        /// <summary>
        /// 绑定List类型的数据集合
        /// </summary>
        /// <param name="data"></param>
        public void BandSource(List<string> data)
        {
            var dic = data?.Distinct().ToDictionary(s => s, s => s);
            var tb = conventToItemTable(dic, false);
            ValueMember = "Key";
            DisplayMember = "Value";
            ColumnHeaderVisible = false;
            BandSource(tb, new List<string>() { "Value" });
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 将一个列表集合转换为Item表
        /// </summary>
        /// <param name="values"></param>
        /// <param name="addEmpty"></param>
        /// <returns></returns>
        private DataTable conventToItemTable(Dictionary<string, string> values, bool addEmpty = false)
        {
            var tb = createItemTable(addEmpty);
            foreach (var value in values)
            {
                if (addEmpty && string.IsNullOrEmpty(value.Key))
                    continue;
                var datarow = tb.NewRow();
                datarow["Key"] = value.Key;
                datarow["Value"] = value.Value;
                tb.Rows.Add(datarow);
            }
            return tb;
        }
        /// <summary>
        /// 创建一个Item表，列为：Key和Value
        /// </summary>
        /// <param name="addEmpty">指示是否添加一个空行</param>
        /// <returns></returns>
        private DataTable createItemTable(bool addEmpty = false)
        {
            var tb = new DataTable();
            tb.Columns.Add(new DataColumn("Key"));
            tb.Columns.Add(new DataColumn("Value"));
            if (addEmpty)
            {
                var datarow = tb.NewRow();
                datarow["Key"] = datarow["Value"] = "";
                tb.Rows.Add(datarow);
            }
            return tb;
        }
        /// <summary>
        /// 选中项后设置显示名称
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boxControl_AfterSelect(object sender, EventArgs e)
        {
            var values = boxControl.GetSelectedValues();
            _selectedValues = values?.Select(s => s.Key)?.ToList() ?? new List<string>();
            _selectedTexts = values?.Select(s => s.Value)?.ToList() ?? new List<string>();
            var text = string.Join(";", _selectedTexts);
            Items.Clear();
            Items.Add(text);
            base.SelectedItem = text;
            tip.SetToolTip(this, text);
            boxControl.Hide();
            SelectedValueChanged?.Invoke(this, e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            DroppedDown = false;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            DroppedDown = false;
            if (boxControl.Visible == false)
            {
                boxControl.Width = Width;
                boxControl.ShowControl(SelectedValues);
                boxControl.Location = PointToScreen(new Point(0, Height));
                boxControl.Width = boxControl.GetControlWidth();
                boxControl.Focus();
                DropDown?.Invoke(this, new EventArgs());
            }
        }
        #endregion

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BaseComboBox));
            this.baseComboBox1 = new ComboBoxTest.BaseComboBox();
            this.SuspendLayout();
            // 
            // baseComboBox1
            // 
            this.baseComboBox1.ColumnHeaderVisible = true;
            this.baseComboBox1.DisplayMember = null;
            this.baseComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.baseComboBox1.FormattingEnabled = true;
            this.baseComboBox1.Location = new System.Drawing.Point(0, 0);
            this.baseComboBox1.MaxDropDownItems = 10;
            this.baseComboBox1.MultiSelect = false;
            this.baseComboBox1.Name = "baseComboBox1";
            this.baseComboBox1.RowHeaderVisible = true;
            this.baseComboBox1.SelectedValue = null;
            this.baseComboBox1.SelectedValues = ((System.Collections.Generic.List<string>)(resources.GetObject("baseComboBox1.SelectedValues")));
            this.baseComboBox1.Size = new System.Drawing.Size(121, 23);
            this.baseComboBox1.TabIndex = 0;
            this.baseComboBox1.ValueMember = null;
            this.ResumeLayout(false);

        }
    }
}
