using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComboBoxTest
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        #region 
        // 注意：为了在测试界面时时看到各参数的控制效果，所以做了动态驱动。在实际开发中直接设置下拉框控件对应的属性即可
        private void boxMultiSelect_CheckedChanged(object sender, EventArgs e)
        {
            boxCombox.MultiSelect = true;
        }
        private void boxShowHeader_CheckedChanged(object sender, EventArgs e)
        {
            boxCombox.ColumnHeaderVisible = boxShowHeader.Checked;
        }
        private void boxShowRowHeader_CheckedChanged(object sender, EventArgs e)
        {
            boxCombox.RowHeaderVisible = boxShowRowHeader.Checked;
        }
        private void textMaxRow_ValueChanged(object sender, EventArgs e)
        {
            boxCombox.MaxDropDownItems = (int)textMaxRow.Value;
        }
        #endregion


        private void btBandTable_Click(object sender, EventArgs e)
        {
            //定义数据源表，实际开发中，根据业务获取到数据表。
            //注意：将数据表中每列的【Caption】属性设置为要显示的表头名称，如果不设置，则在界面会显示【ColumnName】的值
            var tb = new DataTable();
            tb.Columns.Add(new DataColumn("ID") { Caption = "ID" });
            tb.Columns.Add(new DataColumn("Code") { Caption = "学号" });
            tb.Columns.Add(new DataColumn("Name") { Caption = "姓名" });
            tb.Columns.Add(new DataColumn("Sex") { Caption = "性别" });
            tb.Columns.Add(new DataColumn("Age") { Caption = "年龄" });
            //为了测试，自动生成一些数据
            var r = new Random();
            for (int i = 1; i <= 100; i++)
            {
                var row = tb.NewRow();
                row["ID"] = i;
                row["Code"] = i.ToString("D3");
                row["Name"] = "张三" + i;
                row["Sex"] = "男";
                row["Age"] = r.Next(0, 100).ToString();
                tb.Rows.Add(row);
            }

            //定义需要显示到下来界面中的列（因为有些列不需要显示，比如【ID】列）
            //注意：如果不指定该值，则默认显示数据源中所有列。如果指定，则列的显示顺序即为这里指定的顺序，并不是数据与中顺序
            var views = new List<string>() { "Code", "Name", "Age", "Sex" };
            //指定表示被选中对象值的列名称
            boxCombox.ValueMember = "ID";
            //指定显示被选中对象属性的列名称
            boxCombox.DisplayMember = "Name";
            //绑定数据源
            boxCombox.BandSource(tb, views);
        }
        List<string> listOut = new List<string>();
        private void btBandList_Click(object sender, EventArgs e)
        {
            listOut.Add("张三1"); listOut.Add("张三2"); listOut.Add("张三3"); listOut.Add("张三4");
            listOut.Add("张三5"); listOut.Add("张三6"); listOut.Add("张三7"); listOut.Add("张三8");

            boxCombox.ColumnHeaderVisible = false;
            boxCombox.BandSource(listOut);
        }

        Dictionary<string, string> dic = new Dictionary<string, string>();
        private void btBandDic_Click(object sender, EventArgs e)
        {
            dic.Add("1", "USA");
            dic.Add("2", "USSS");
            dic.Add("3", "张三23");
            dic.Add("4", "张三14");
            dic.Add("5", "Canda");
            dic.Add("6", "JOAAAasxsaxasxasx");
            dic.Add("7", "张三17");
            dic.Add("8", "张三18");

            boxCombox.BandSource(dic);
        }

        private void boxCombox_SelectedValueChanged(object sender, EventArgs e)
        {
            var values = boxCombox.SelectedValues;
            textSelectresult.Text = string.Join("、", values);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = "5, 6, 7, 9, 11";
            string[] str_Array = str.Replace(", ", ",").Split(',');
            List<string> lst = new List<string>(str_Array);
            boxCombox.SelectedValues = lst;
        }

        private void boxCombox_TextChanged(object sender, EventArgs e)
        {
        }

        Dictionary<string, string> listNew = new Dictionary<string, string>();

        private void boxCombox_TextUpdate(object sender, EventArgs e)
        {
            //this.boxCombox.Items.Clear();
            //listNew.Clear();

            //foreach (var item in dic)
            //{
            //    if (item.Value.Contains(this.boxCombox.Text))
            //    {
            //        listNew.Add(item.Key, item.Value);
            //    }
            //}

            //boxCombox.BandSource(listNew);
            //boxCombox.SelectionStart = boxCombox.Text.Length;
            //Cursor = Cursors.Default;
            //boxCombox.DroppedDown = true;
            //this.boxCombox.Items.AddRange(listNew.ToArray());
        }
    }
}
