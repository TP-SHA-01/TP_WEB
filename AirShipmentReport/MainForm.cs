using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WebApi.Services;
using WebApi.Models;
using System.Diagnostics;

using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

using System.Configuration;

namespace AirShipmentReport
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            int intDateYear = Convert.ToInt32(DateTime.Now.Year.ToString());

            combYearFrom.Items.Add((intDateYear - 3).ToString());


            combYearFrom.Items.Add((intDateYear - 2).ToString());
            combYearFrom.Items.Add((intDateYear - 1).ToString());
            combYearFrom.Items.Add((intDateYear - 0).ToString());
            combYearFrom.Items.Add((intDateYear + 1).ToString());
            combYearFrom.Items.Add((intDateYear + 2).ToString());
            combYearFrom.Items.Add((intDateYear + 3).ToString());

            combYearFrom.SelectedIndex = 3;

            combYearTo.Items.Add((intDateYear - 3).ToString());
            combYearTo.Items.Add((intDateYear - 2).ToString());
            combYearTo.Items.Add((intDateYear - 1).ToString());
            combYearTo.Items.Add((intDateYear - 0).ToString());
            combYearTo.Items.Add((intDateYear + 1).ToString());
            combYearTo.Items.Add((intDateYear + 2).ToString());
            combYearTo.Items.Add((intDateYear + 3).ToString());

            combYearTo.SelectedIndex = 3;


            for (int i = 1; i < 13; i++)
            {
                this.combWeekFrom.Items.Add(i.ToString().PadLeft(2, '0'));

                this.combWeekTo.Items.Add(i.ToString().PadLeft(2, '0'));
            }
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            string strmsg = "";
            string weekfrom = "";
            string weekto = "";

          
            if (this.combWeekFrom.SelectedIndex == -1)
            {
                strmsg = strmsg + "Error Msg:" + "Please select week from value." + "\r\n";
            }
            if (this.combWeekTo.SelectedIndex == -1)
            {
                strmsg = strmsg + "Error Msg:" + "Please select week to value." + "\r\n";
            }

            if (this.combYearTo.SelectedIndex < this.combYearFrom.SelectedIndex)
            {
                strmsg = strmsg + "Error Msg:" + "The selected to year must be after the from year." + "\r\n";
            }
            if (this.combWeekTo.SelectedIndex < this.combWeekFrom.SelectedIndex && this.combYearTo.SelectedIndex == this.combYearFrom.SelectedIndex)
            {
                strmsg = strmsg + "Error Msg:" + "The selected to Month must be after the from Month." + "\r\n";
            }

            if (strmsg != "")
            {
                MessageBox.Show(strmsg);
            }
            else
            {
                weekfrom = this.combYearFrom.Text.Trim() + this.combWeekFrom.Text.Trim();
                weekto = this.combYearTo.Text.Trim() + this.combWeekTo.Text.Trim();

               
                DataSet ds = WebApi.Services.BookingAdviceAnalysisRpt_Service.GetAirShipmentReport(weekfrom, weekto);

                if (ds != null && ds.Tables.Count > 0)
                {
                    ExportDataToExcel2(ds, "Air_Shipment_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond.ToString());
                }
                else
                {
                    MessageBox.Show("No Data.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        public void ExportDataToExcel2(DataSet SetTable, string FileName)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //设置文件标题
            saveFileDialog.Title = "导出Excel文件";
            //设置文件类型
            saveFileDialog.Filter = "Excel 工作簿(*.xlsx)|*.xlsx|Excel 97-2003 工作簿(*.xls)|*.xls";
            //设置默认文件类型显示顺序  
            saveFileDialog.FilterIndex = 1;
            //是否自动在文件名中添加扩展名
            saveFileDialog.AddExtension = true;
            //是否记忆上次打开的目录
            saveFileDialog.RestoreDirectory = true;
            //设置默认文件名
            saveFileDialog.FileName = FileName;

            //按下确定选择的按钮  
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //获得文件路径 
                string localFilePath = saveFileDialog.FileName.ToString();

                //NPOI
                IWorkbook workbook;
                IDataFormat format;
                string FileExt = Path.GetExtension(localFilePath).ToLower();
                if (FileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook();

                }
                else if (FileExt == ".xls")
                {
                    workbook = new HSSFWorkbook();
                }
                else
                {
                    workbook = null;
                }
                if (workbook == null)
                {
                    return;
                }

                //秒钟
                Stopwatch timer = new Stopwatch();
                timer.Start();

                try
                {
                    int rowscount = SetTable.Tables.Count;

                    ICellStyle style_header = workbook.CreateCellStyle();
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 11;
                    font.FontName = "Comic Sans MS";
                    font.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                    style_header.SetFont(font);//HEAD 样式 
                    style_header.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_header.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                    ICellStyle style_row = workbook.CreateCellStyle();
                    IFont font1 = workbook.CreateFont();
                    font1.FontHeightInPoints = 11;
                    font1.FontName = "Comic Sans MS";
                    style_row.SetFont(font1);//row 样式 
                    style_row.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;


                    ICellStyle style_Money = workbook.CreateCellStyle();
                    style_Money.SetFont(font1);//row 样式 
                    style_Money.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_Money.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_Money.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_Money.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_Money.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    format = workbook.CreateDataFormat();
                    style_Money.DataFormat = format.GetFormat("$#,##0");


                    ICellStyle style_Date = workbook.CreateCellStyle();
                    style_Date.SetFont(font1);//row 样式 
                    style_Date.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_Date.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_Date.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_Date.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_Date.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    format = workbook.CreateDataFormat();

                    style_Date.DataFormat = format.GetFormat("yyyy/m/d");

                    for (int k = 0; k < rowscount; k++)
                    {
                        DataTable TableName = SetTable.Tables[k];

                        ISheet sheet = workbook.CreateSheet(SetTable.Tables[k].TableName.ToString());

                        //读取标题  
                        int CSRowsCount = TableName.Rows.Count + 3;

                        IRow rowHeader = sheet.CreateRow(0);

                        for (int i = 0; i < TableName.Columns.Count; i++)
                        {
                            ICell cell = rowHeader.CreateCell(i);
                            cell.SetCellValue(TableName.Columns[i].ColumnName);

                            cell.CellStyle = style_header;

                            //Item No.  TP / Non    TP   Shipper     Cnee    Mawb    Hawb    Origin      Dest.   Airline    Flt No .Package G.W.            Volume                   Co-loader           ETD ATD ETA ATA Remark      Result  PO
                            //Item No.	TP/Non      TP	 Shipper 	 Cnee 	 MAWB 	 Hawb 	 Origin 	 Dest. 	 Airline 	Flt No.	Package	G.W.	C.W.	Volume	 S/R 	 B/R 	 Co-loader 	Cost	ETD	ATD	ETA	ATA	 Remark 	 Result 


                            if (TableName.Columns[i].ColumnName.Trim().ToLower() == "shipper" || TableName.Columns[i].ColumnName.Trim().ToLower() == "cnee" || TableName.Columns[i].ColumnName.Trim().ToLower() == "remark")
                            {
                                sheet.SetColumnWidth(i, 30 * 256);
                            }
                            else if (TableName.Columns[i].ColumnName.Length == 3 ) //eta,ted,ata,atd
                            {
                                if (TableName.Columns[i].ColumnName.Trim().ToLower() == "s/r" || TableName.Columns[i].ColumnName.Trim().ToLower() == "b/r")
                                {
                                    sheet.SetColumnWidth(i, 10 * 256);
                                }
                                else
                                {
                                    sheet.SetColumnWidth(i, 20 * 256);
                                }
                            }
                            else if (TableName.Columns[i].ColumnName.ToLower().Contains("awb")|| TableName.Columns[i].ColumnName.ToLower().Contains("booking")) //mawb,hawb
                            {
                                sheet.SetColumnWidth(i, 15 * 256);
                            }
                            else if (TableName.Columns[i].ColumnName.Trim().ToLower() == "airline" || TableName.Columns[i].ColumnName.Trim().ToLower() == "flt no.")
                            {
                                sheet.SetColumnWidth(i, 6 * 256);
                            }
                            else if (TableName.Columns[i].ColumnName.Trim().ToLower() == "item no.")
                            {
                                sheet.SetColumnWidth(i, 5 * 256);
                            }
                            else
                            {
                                sheet.AutoSizeColumn(i);
                            }

                        }

                        //k第几张表
                        for (int i = 0; i < TableName.Rows.Count; i++)
                        {
                            IRow rowData = sheet.CreateRow(i + 1);

                            for (int j = 0; j < TableName.Columns.Count; j++)
                            {
                                ICell cell = rowData.CreateCell(j);

                                if (TableName.Rows[i][j].ToString() == "0" || TableName.Rows[i][j].ToString() == "0.0000" || TableName.Rows[i][j].ToString() == "0.000000")
                                {
                                    cell.SetCellValue("");
                                    cell.CellStyle = style_row;
                                }
                                else
                                {
                                    try
                                    {
                                        string strValue = TableName.Rows[i][j].ToString();
                                        //13,14,15,16 Num
                                        //18,19,20, 21 DTE
                                        if (strValue == "")
                                        {
                                            cell.SetCellValue(strValue);
                                            cell.CellStyle = style_row;
                                        }
                                        else
                                        {
                                            if (j == 12 || j == 13 || j == 14 || j == 15)
                                            {
                                                cell.SetCellValue(double.Parse(strValue));
                                                cell.CellStyle = style_row;
                                            }
                                            else if (j == 17 || j == 18 || j == 19 || j == 20)
                                            {
                                                strValue = strValue.Replace("0:00:00", "");
                                                cell.SetCellValue(strValue);
                                                cell.CellStyle = style_Date;

                                            }
                                            else
                                            {
                                                cell.SetCellValue(strValue);
                                                cell.CellStyle = style_row;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                            Application.DoEvents();
                        }

                        Application.DoEvents();

                        //转为字节数组  
                        MemoryStream stream = new MemoryStream();
                        workbook.Write(stream);
                        var buf = stream.ToArray();

                        //保存为Excel文件  
                        using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(buf, 0, buf.Length);
                            fs.Flush();
                            fs.Close();
                        }
                    }

                    //状态栏更改
                    lblStatus.Text = "生成Excel成功，共耗时" + timer.ElapsedMilliseconds + "毫秒。";
                    Application.DoEvents();

                    //成功提示
                    if (MessageBox.Show("导出成功，是否立即打开？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(localFilePath);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {

                    //关闭秒钟
                    timer.Reset();
                    timer.Stop();

                    //赋初始值
                    lblStatus.Visible = false;

                }
            }
        }

        public bool IsNumberic(string strNum)
        {
            try
            {
                System.Decimal i = System.Convert.ToDecimal(strNum);
                return true;
            }
            catch(System.Exception ex)
            {
                return false;
            }
        }

        private void btnExportColoaderPO_Click(object sender, EventArgs e)
        {

            if (this.txtSelReport.Text != "")
            {
                string path = this.txtSelReport.Text.ToString();
                 DataSet ds = ExcelToDataSet(path, true);

                string Coloader_ENG = WebApi.Services.BookingAdviceAnalysisRpt_Service.GetColoader("Coloader_ENG");
                string Coloader_CHN = WebApi.Services.BookingAdviceAnalysisRpt_Service.GetColoader("Coloader_CHN");

                if (ds != null)
                {
                    //Master Loader Compare

                    //1.Get all Co-Loader data 排序
                    //Month           JAN(WK1-5) 			 FEB(WK6 - 9)           MAR(WK10 - 13)             TTL
                    //Co - Loader      G.W.Volume %        G.W.Volume %           G.W.Volume %               G.W.Volume %
                    //Month     2Column
                    //Co-Loader 2Columns
                    //JAN(WK1-5)3Columns
                    //TTL       3Columns


                    DataTable tblDatas = new DataTable("Datas");//存储所有Co-Loader的Datatabel
                    tblDatas.Columns.Add("Co-loader", Type.GetType("System.String"));

                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        DataTable products = ds.Tables[k];

                        IEnumerable<DataRow> rows = from p in products.AsEnumerable()
                                                    select p;
                        foreach (DataRow row in rows)
                        {
                            //row.Field<string>("Co-loader")
                            tblDatas.Rows.Add(new object[] { row.Field<string>("Co-loader") });
                        }
                    }

                    //创建Table
                    #region

                    //Month JAN(WK1-5) 			 FEB(WK6 - 9)              MAR(WK10 - 13)            APR(WK14 - 18)            MAY(WK19 - 22)            JUN(WK23 - 26)            JUL(WK27 - 31)            AUG(WK32 - 35)            SEP(WK36 - 39)            OCT(WK40 - 44)            NOV(WK45 - 48)            DEC(WK49 - 52)            TTL
                    //Co - Loader      G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume %

                    DataTable MergeDatas = new DataTable("Master Loader Compare");

                    MergeDatas.Columns.Add("Co-Loader0", Type.GetType("System.String"));
                    MergeDatas.Columns.Add("Co-Loader1", Type.GetType("System.String"));

                    for (int k = 0; k < ds.Tables.Count; k++)
                    {
                        MergeDatas.Columns.Add(ds.Tables[k].TableName.Trim() + "_G.W.", Type.GetType("System.Double"));
                        MergeDatas.Columns.Add(ds.Tables[k].TableName.Trim() + "_Volume", Type.GetType("System.Double"));
                        MergeDatas.Columns.Add(ds.Tables[k].TableName.Trim() + "_%", Type.GetType("System.String"));
                    }
                    //TTl
                    MergeDatas.Columns.Add("TTL_G.W.", Type.GetType("System.Double"));
                    MergeDatas.Columns.Add("TTL_Volume", Type.GetType("System.Double"));
                    MergeDatas.Columns.Add("TTL_%", Type.GetType("System.String"));
                    #endregion

                    //记录最后一行数据。
                   
                    string strLastLine = "Total2";
                    string MonthTotalGW = "";
                    double TotalGW = 0.00;
                    double TotalVolume = 0.00;

                    double AllTotalGW = 0.00;
                    double AllTotalVolume = 0.00;

                    for (int m = 0; m < ds.Tables.Count; m++) //循环每个月的表
                    {
                        DataTable eachTable = ds.Tables[m];
                        double ColumnTotalGW = 0.00;
                        double ColumnTotalVolume = 0.00;
                        TotalGW = 0.00;
                        TotalVolume = 0.00;

                        for (int n = 0; n < eachTable.Rows.Count; n++)//循环某张表的每一行
                        {
                            if (eachTable.Rows[n]["G.W."].ToString().Trim() != "")
                            {
                                string strGW = eachTable.Rows[n]["G.W."].ToString().Replace(" ", "").ToString();
                                ColumnTotalGW = Convert.ToDouble(ColumnTotalGW) + Convert.ToDouble(strGW);
                                ColumnTotalGW = Math.Round(ColumnTotalGW, 2);
                            }
                            if (eachTable.Rows[n]["Volume"].ToString().Trim() != "")
                            {
                                string strVolume = eachTable.Rows[n]["Volume"].ToString().Replace(" ", "").ToString();
                                ColumnTotalVolume = Convert.ToDouble(ColumnTotalVolume) + Convert.ToDouble(strVolume);//计算这表的Volume
                                ColumnTotalVolume = Math.Round(ColumnTotalVolume, 2);
                            }
                        }

                        TotalGW = Math.Round(Convert.ToDouble(TotalGW) + Convert.ToDouble(ColumnTotalGW),2);
                        TotalVolume = Math.Round(Convert.ToDouble(TotalVolume) + Convert.ToDouble(ColumnTotalVolume),2);

                        AllTotalGW = Math.Round(AllTotalGW + TotalGW,2);
                        AllTotalVolume = Math.Round(AllTotalVolume + TotalVolume, 2);

                        string Present = ",100%";
                        if (eachTable.Rows.Count ==0)
                        {
                            Present = ",0%";
                        }

                        strLastLine = strLastLine + "," + TotalGW + "," + TotalVolume + Present;
                        MonthTotalGW = MonthTotalGW + TotalGW + ",";
                    }

                    strLastLine = "Total1," + strLastLine + "," + AllTotalGW + "," + AllTotalVolume + ",100%";

                    //针对：Co-loader ，进行唯一排序
                    if (tblDatas.Rows.Count > 0)
                    {
                        var query = tblDatas.AsEnumerable()
                        .OrderBy(dr => dr.ItemArray[0].ToString().Trim() as string)
                        .Distinct(DataRowComparer.Default).Select(dr => dr.ItemArray[0].ToString().Trim()); //进行唯一排序


                        foreach (string item in query)
                        {
                            string strline = "";

                            //Console.WriteLine(item); //按排序结果顺序显示
                            if (item.Trim().ToString() == "")
                            {
                                strline = strline + " " + "," + " ";
                            }
                            else
                            {
                                //Coloader ENG / Coloader CHN
                                string ItemColoader_ENG = "";
                                string ItemColoader_CHN = "";
                                string text = item;

                                //('/APEX/','/TCI/','/YY/','/CTS/','/PCS/','/ALL/','/WFF/','/EFD/','/AWOT/','/EPE/','/ECL/','/LHG/','/GOJ/','/GT/','/AAF/','/XFT/','/ADS/','/YS/','/FVS/','/XTF/','/LY/','/DYB/','/SJ/','/CB/','/HY/','/TJ/','/LW/','/YH/','/JX/','/LFF/','/ZY/','/HL/')"
                                //('/ 爱派克斯 /','/ 腾隆 /','/ 鹰扬 /','/ 华茂 /','/ 圆欣 /','/ 传盛 /','/ 唯凯 /','/ 东方富达 /','/ 欧华 /','/ 弘致 /','/ 翊捷 /','/ 北京洛豪 /','/ 高捷 /','/ 鑫鼎 /','/ 上海崴仕 /','/ 雪伏特 /','/ 安达顺 /','/ 江苏奕舜 /','/ 瀚钰通 /','/ 轩泰 /','/ 霖元 /','/ 递一步 /','/ 宿杰 /','/ 承邦 /','/ 瀚阳 /','/ 添玖 /','/ 朗崴 /','/ 盈恒 /','/ 经信 /','/ 联飞 /','/ 宁波中远 /','/ 汇力 /')"

                                Coloader_CHN = Coloader_CHN.Replace("(", "").Replace(")", "");
                                Coloader_ENG = Coloader_ENG.Replace("(", "").Replace(")", "");

                                string[] strCHN = Coloader_CHN.Split(',');
                                string[] strENG = Coloader_ENG.Split(',');
                                bool IsChinese = false;

                                for (int i = 0; i < text.Length; i++)
                                {
                                    if ((int)text[i] > 127) //Chinese char
                                    {
                                        IsChinese = true;
                                    }
                                }

                                if(IsChinese)
                                {
                                    for (int k = 0; k < strCHN.Length; k++)
                                    {
                                        if (strCHN[k].Contains("/" + item + "/"))
                                        {
                                            ItemColoader_CHN = strCHN[k].Replace("/", "").Replace("'", "");
                                            ItemColoader_ENG = strENG[k].Replace("/", "").Replace("'", "");
                                        }
                                    }

                                    if (ItemColoader_CHN == "" && ItemColoader_ENG == "")
                                    {
                                        ItemColoader_CHN = item;
                                        ItemColoader_ENG = "";
                                    }
                                }
                                else
                                {
                                    for (int k = 0; k < strENG.Length; k++)
                                    {
                                        if (strENG[k].Contains("/" + item + "/"))
                                        {
                                            ItemColoader_CHN = strCHN[k].Replace("/", "").Replace("'", "");
                                            ItemColoader_ENG = strENG[k].Replace("/", "").Replace("'", "");
                                        }
                                    }

                                    if (ItemColoader_CHN == "" && ItemColoader_ENG == "")
                                    {
                                        ItemColoader_CHN = "";
                                        ItemColoader_ENG = item;
                                    }
                                }
                                strline = strline + ItemColoader_ENG + "," + ItemColoader_CHN;
                            }
                            
                            double RowTotalGW = 0.00;
                            double RowTotalVolume = 0.00;
                            double RowTotalPrecent = 0.00;

                            //double RowTotalPrecent = 0.00; //=AM5/AM38  第N行GW的Total/所有行GW的Total

                            for (int m = 0; m < ds.Tables.Count; m++) //循环每个月的表
                            {
                                string[] eachTotalGW = MonthTotalGW.Split(',');

                                DataTable eachTable = ds.Tables[m];
                                //累加每一行的GW Total ，用于一行中的最后Total
                                double ColoaderGW = 0.00;
                                double ColoaderVolume = 0.00;
                                double ColoaderPrecent = 0.00;

                                for (int n = 0; n < eachTable.Rows.Count; n++)//循环某张表的每一行
                                {
                                    if (eachTable.Rows[n]["Co-loader"].ToString().Trim() == item.Trim())//找到对应的Coloader叠加起来

                                    {
                                        if (eachTable.Rows[n]["G.W."].ToString() != "")
                                        {
                                            string strGW = eachTable.Rows[n]["G.W."].ToString().Replace(" ", "").ToString();
                                            ColoaderGW = Convert.ToDouble(ColoaderGW) + Convert.ToDouble(strGW); //计算这张表的GW和
                                            ColoaderGW = Math.Round(ColoaderGW, 2);

                                        }
                                        else
                                        {
                                            ColoaderGW = ColoaderGW + Convert.ToDouble("0.00"); //计算这张表的GW和
                                            ColoaderGW = Math.Round(ColoaderGW, 2);
                                        }
                                        if (eachTable.Rows[n]["Volume"].ToString() != "")
                                        {
                                            string strVolume = eachTable.Rows[n]["Volume"].ToString().Replace(" ", "").ToString();
                                            ColoaderVolume = Math.Round(Convert.ToDouble(ColoaderVolume) + Convert.ToDouble(strVolume), 2);//计算这表的Volume
                                        }
                                        else
                                        {
                                            ColoaderVolume = Math.Round(ColoaderVolume + Convert.ToDouble("0.00"), 2);//计算这表的Volume
                                        }
                                    }
                                }
                                if (Convert.ToDouble(eachTotalGW[m]) == 0.00)
                                {
                                    ColoaderPrecent = 0.00;
                                }
                                else
                                {
                                    ColoaderPrecent = Math.Round(ColoaderGW * 100 / Convert.ToDouble(eachTotalGW[m]),2);
                                }

                                RowTotalGW = Math.Round(RowTotalGW + ColoaderGW, 2);

                                RowTotalVolume = Math.Round(RowTotalVolume + ColoaderVolume, 2);

                                strline = strline + "," + ColoaderGW + "," + ColoaderVolume + "," + ColoaderPrecent + "%";

                            }

                            if(AllTotalGW == 0.00)
                            {
                                RowTotalPrecent = 0.00;
                            }
                            else
                            {
                                RowTotalPrecent = Math.Round(RowTotalGW*100 / Convert.ToDouble(AllTotalGW),2);
                            }

                            strline = strline + "," + RowTotalGW + "," + RowTotalVolume+ "," + RowTotalPrecent+"%";

                            if (RowTotalGW != 0)
                            {
                                //% G.W./Total G.W.
                                string[] strlines = strline.Split(',');

                                DataRow dr = MergeDatas.NewRow();

                                for (int k = 0; k < strlines.Length; k++)
                                {
                                    dr[k] = strlines[k];
                                }
                                MergeDatas.Rows.Add(dr);
                            }
                        }

                        string[] strLastLines = strLastLine.Split(',');

                        DataRow dr1 = MergeDatas.NewRow();

                        for (int k = 0; k < strLastLines.Length; k++)
                        {
                            dr1[k] = strLastLines[k];
                        }
                        MergeDatas.Rows.Add(dr1);
                    }

                    //Co - loader(Eng)|Co - loader(CN)|G.W.|Volume|%|G.W.|Volume|%|G.W.|Volume|%.......|G.W.|Volume|%
                    ExportDataToExcel(MergeDatas, "Master Loader Compare");
                }
                else
                {
                    MessageBox.Show("No Data!", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSelectRpt_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            try
            {
                System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
                if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string Selected_File = openfiledialog.FileName;
                    this.txtSelReport.Text = Selected_File.Trim();
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        public static DataSet ExcelToDataSet(string filePath, bool isFirstLineColumnName)
        {
            DataSet dataSet = new DataSet();
            int startRow = 0;
            try
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    IWorkbook workbook = null;
                    // 如果是2007+的Excel版本
                    if (filePath.IndexOf(".xlsx") > 0)
                    {
                        workbook = new XSSFWorkbook(fs);
                    }
                    // 如果是2003-的Excel版本
                    else if (filePath.IndexOf(".xls") > 0)
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    if (workbook != null)
                    {
                        //循环读取Excel的每个sheet，每个sheet页都转换为一个DataTable，并放在DataSet中
                        for (int p = 0; p < workbook.NumberOfSheets; p++)
                        {
                            ISheet sheet = workbook.GetSheetAt(p);
                            DataTable dataTable = new DataTable();
                            dataTable.TableName = sheet.SheetName;
                            if (sheet != null)
                            {
                                if (sheet.SheetName.Contains("WK"))
                                {
                                    int rowCount = sheet.LastRowNum;//获取总行数
                                    if (rowCount > 0)
                                    {
                                        IRow firstRow = sheet.GetRow(0);//获取第一行
                                        int cellCount = 24; //firstRow.LastCellNum;//获取总列数

                                        //构建datatable的列
                                        if (isFirstLineColumnName)
                                        {
                                            startRow = 1;//如果第一行是列名，则从第二行开始读取
                                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                            {
                                                ICell cell = firstRow.GetCell(i);
                                                if (cell != null)
                                                {
                                                    if (cell.StringCellValue != null)
                                                    {
                                                        DataColumn column = new DataColumn(cell.StringCellValue);
                                                        dataTable.Columns.Add(column);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                            {
                                                DataColumn column = new DataColumn("column" + (i + 1));
                                                dataTable.Columns.Add(column);
                                            }
                                        }

                                        //填充行
                                        for (int i = startRow; i <= rowCount; ++i)
                                        {
                                            IRow row = sheet.GetRow(i);
                                            if (row == null) continue;

                                            DataRow dataRow = dataTable.NewRow();
                                            for (int j = row.FirstCellNum; j < cellCount; ++j)
                                            {
                                                ICell cell = row.GetCell(j);
                                                if (cell == null)
                                                {
                                                    dataRow[j] = "";
                                                }
                                                else
                                                {
                                                    //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                                    switch (cell.CellType)
                                                    {
                                                        case CellType.Blank:
                                                            dataRow[j] = "";
                                                            break;
                                                        case CellType.Numeric:
                                                            short format = cell.CellStyle.DataFormat;
                                                            //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                            if (format == 14 || format == 22 || format == 31 || format == 57 || format == 58)
                                                                dataRow[j] = cell.DateCellValue;
                                                            else
                                                                dataRow[j] = cell.NumericCellValue;
                                                            break;
                                                        case CellType.String:
                                                            dataRow[j] = cell.StringCellValue;
                                                            break;
                                                    }
                                                }
                                            }
                                            dataTable.Rows.Add(dataRow);
                                        }
                                    }

                                    dataSet.Tables.Add(dataTable);
                                }
                            }
                        }

                    }
                }

                return dataSet;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        private void ExportDataToExcel(DataTable SetTable, string FileName)
        {
            DataSet dataSet = new DataSet();
            string filePath = this.txtSelReport.Text.Trim();
            using (FileStream fs = File.OpenRead(filePath))
            {

                //NPOI
                IWorkbook workbook;
                IDataFormat format;
                string FileExt = Path.GetExtension(filePath).ToLower();
                if (FileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);

                }
                else if (FileExt == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = null;
                }
                if (workbook == null)
                {
                    return;
                }

                int SheetCount = workbook.NumberOfSheets;//获取表的数量
                string[] SheetName = new string[SheetCount];//保存表的名称
                for (int i = 0; i < SheetCount; i++)
                {
                    SheetName[i] = workbook.GetSheetName(i);
                }

                for (int k = 0; k < SheetName.Length; k++)
                {
                    if (SheetName[k].Trim() == "Master Loader Compare")
                    {
                        FileName = "Master Loader Compare(" + System.DateTime.Now.ToString("MMddHHmm")+")";
                    }
                }
               
                Stopwatch timer = new Stopwatch();
                timer.Start();

                try
                {
                    #region Style

                    ICellStyle style_header11 = workbook.CreateCellStyle();
                    IFont font11 = workbook.CreateFont();
                    font11.FontHeightInPoints = 11;
                    font11.FontName = "宋体";
                    font11.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    font11.IsBold = true;
                    style_header11.SetFont(font11);//HEAD 样式 
                    style_header11.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_header11.VerticalAlignment = VerticalAlignment.Center;
                    style_header11.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_header11.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_header11.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_header11.BorderRight = NPOI.SS.UserModel.BorderStyle.Thick;

                    ICellStyle style_header15 = workbook.CreateCellStyle();
                    IFont font15 = workbook.CreateFont();
                    font15.FontHeightInPoints = 15;
                    font15.FontName = "宋体";
                    font15.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    font15.IsBold = true;
                    style_header15.SetFont(font15);//HEAD 样式 
                    style_header15.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_header15.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_header15.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_header15.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_header15.BorderRight = NPOI.SS.UserModel.BorderStyle.Thick;

                    ICellStyle style_Column = workbook.CreateCellStyle();
                    IFont font4 = workbook.CreateFont();
                    font4.FontHeightInPoints = 15;
                    font4.FontName = "宋体";
                    font4.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    font4.IsBold = true;
                    style_Column.SetFont(font4);//HEAD 样式 
                    style_Column.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_Column.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_Column.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_Column.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_Column.BorderRight = NPOI.SS.UserModel.BorderStyle.Thick;


                    ICellStyle style_row_Old = workbook.CreateCellStyle();
                    IFont font_Old = workbook.CreateFont();
                    font_Old.FontHeightInPoints = 10;
                    font_Old.FontName = "Comic Sans MS";
                    font_Old.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                    style_row_Old.SetFont(font_Old);//row 样式 
                    style_row_Old.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row_Old.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Old.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Old.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Old.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                    //ICellStyle style_Song = workbook.CreateCellStyle();
                    //IFont font2 = workbook.CreateFont();
                    //font2.FontHeightInPoints = 10;
                    //font2.FontName = "宋体";
                    //style_Song.SetFont(font2);//row 样式 
                    //style_Song.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    //style_Song.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    //style_Song.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    //style_Song.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                    ICellStyle style_row = workbook.CreateCellStyle();
                    IFont font1 = workbook.CreateFont();
                    font1.FontHeightInPoints = 11;
                    font1.FontName = "宋体";
                    style_row.SetFont(font1);//row 样式 
                    style_row.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    

                    ICellStyle style_row_Ye = workbook.CreateCellStyle();
                    IFont font3 = workbook.CreateFont();
                    font3.FontHeightInPoints = 11;
                    font3.FontName = "宋体";
                    font3.Color = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    font3.IsBold = true;
                    style_row_Ye.SetFont(font3);//row 样式 
                    style_row_Ye.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row_Ye.BorderTop = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_row_Ye.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_row_Ye.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_row_Ye.BorderRight = NPOI.SS.UserModel.BorderStyle.Thick;
                    style_row_Ye.FillForegroundColor = 0;
                    style_row_Ye.FillPattern = FillPattern.SolidForeground;
                    ((XSSFColor)style_row_Ye.FillForegroundColorColor).SetRgb(new byte[] { 255, 255, 0 });
                    //workbook.createDataFormat().getFormat("0.0%")

                    format = workbook.CreateDataFormat();
                    style_row_Ye.DataFormat = format.GetFormat("0.00%");


                    #endregion

                    DataTable TableName = SetTable;

                    //第一张表
                    ISheet sheet = workbook.CreateSheet(FileName);

                    //读取标题  
                    int CSRowsCount = TableName.Rows.Count + 3;

                    //Co - loader(Eng)	Co - loader(CN)	(WK0)G.W.	(WK0)Volume	(WK0)%	(WK1)G.W.	(WK1)Volume	(WK1)%	(WK2)G.W.	(WK2)Volume	(WK2)%	(WK3)G.W.	(WK3)Volume	(WK3)%	(WK4)G.W.	(WK4)Volume	(WK4)%	(WK5)G.W.	(WK5)Volume	(WK5)%	(WK6)G.W.	(WK6)Volume	(WK6)%	(WK7)G.W.	(WK7)Volume	(WK7)%	(WK8)G.W.	(WK8)Volume	(WK8)%	(WK9)G.W.	(WK9)Volume	(WK9)%	(WK10)G.W.	(WK10)Volume	(WK10)%	TTL_G.W.	TTL_Volume	TTL_%

                    //Month                   JAN(WK1-5) 			 FEB(WK6 - 9)              MAR(WK10 - 13)            APR(WK14 - 18)            MAY(WK19 - 22)            JUN(WK23 - 26)            JUL(WK27 - 31)            AUG(WK32 - 35)            SEP(WK36 - 39)            OCT(WK40 - 44)            NOV(WK45 - 48)            DEC(WK49 - 52)            TTL
                    //Co - Loader      G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume % G.W.Volume %

                    //headerRow.CreateCell(0).SetCellValue("XXX");
                    //headerRow.GetCell(0).CellStyle = headStyle;
                    //sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 1)); //合并

                    IRow rowHeader0 = sheet.CreateRow(0);
                    for (int i = 0; i < TableName.Columns.Count; i++)
                    {
                        ICell cell = rowHeader0.CreateCell(i);

                        string ColumnName = TableName.Columns[i].ColumnName;
                        string newColumnName = "";
                        if (ColumnName.Contains("Co-Loader0"))
                        {
                            newColumnName = "Month";
                        }
                        else if (ColumnName.Contains("Co-Loader1"))
                        {
                            newColumnName = "";
                        }
                        else if (ColumnName.Contains("G.W."))
                        {
                            newColumnName = ColumnName.Replace("_G.W.", "");
                        }
                        else if (ColumnName.Contains("Volume"))
                        {
                            newColumnName ="";
                        }
                        else if (ColumnName.Contains("%"))
                        {
                            newColumnName = "";
                        }
                        else if (ColumnName.Contains("TTL_G.W."))
                        {
                            newColumnName = ColumnName.Replace("_G.W.", "");
                        }
                        else if (ColumnName.Contains("TTL_Volume"))
                        {
                            newColumnName = "";
                        }
                        else if (ColumnName.Contains("TTL_%"))
                        {
                            newColumnName = "";
                        }

                        cell.SetCellValue(newColumnName);

                        if (ColumnName.Contains("Co-Loader0"))
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 1));
                        }
                        else if (ColumnName.Contains("Co-Loader1"))
                        {
                            
                        }
                        else if (ColumnName.Contains("G.W."))
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, i, i+2));
                        }
                        else if (ColumnName.Contains("TTL_G.W."))
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(0, 0, i, i + 2));
                        }

                        if (newColumnName == "Month")
                        {
                            cell.CellStyle = style_header15;
                        }
                        else
                        {
                            cell.CellStyle = style_header11;
                        }
                        //sheet.AutoSizeColumn(i);
                        sheet.SetColumnWidth(i, 15 * 256);
                    }

                    IRow rowHeader1 = sheet.CreateRow(1);
                    for (int i = 0; i < TableName.Columns.Count; i++)
                    {
                        ICell cell = rowHeader1.CreateCell(i);

                        string ColumnName = TableName.Columns[i].ColumnName;
                        string newColumnName = "";
                        if (ColumnName.Contains("Co-Loader0"))
                        {
                            newColumnName = "Co-Loader";
                        }
                        else if (ColumnName.Contains("Co-Loader1"))
                        {
                            newColumnName = "";
                        }
                        else if (ColumnName.Contains("G.W."))
                        {
                            newColumnName = "G.W.";
                        }
                        else if (ColumnName.Contains("Volume"))
                        {
                            newColumnName = "Volume";
                        }
                        else if (ColumnName.Contains("%"))
                        {
                            newColumnName = "%";
                        }

                        cell.SetCellValue(newColumnName);


                        if (ColumnName.Contains("Co-Loader0"))
                        {
                            sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 1));
                        }

                        if (newColumnName == "Co-Loader")
                        {
                            cell.CellStyle = style_header15;
                        }
                        else
                        {
                            cell.CellStyle = style_header11;
                        }
                        sheet.AutoSizeColumn(i);

                    }

                    //k第几张表
                    for (int i = 0; i < TableName.Rows.Count; i++)
                    {
                        IRow rowData = sheet.CreateRow(i + 2);

                        for (int j = 0; j < TableName.Columns.Count; j++)
                        {
                            ICell cell = rowData.CreateCell(j);
                            
                            if (TableName.Rows[i][j].ToString() == "0" || TableName.Rows[i][j].ToString() == "0.00")
                            {
                                if (TableName.Columns[j].ToString().Contains("WT")) 
                                {
                                    cell.SetCellValue(double.Parse("0"));
                                }
                                else
                                {
                                    cell.SetCellValue(double.Parse("0.00"));
                                }
                                
                                cell.CellStyle = style_row;
                            }
                            else
                            {
                                if (j > 1)
                                {
                                    if (TableName.Rows[i][j].ToString().Contains("%"))
                                    {
                                        cell.SetCellValue(TableName.Rows[i][j].ToString());
                                        cell.CellStyle = style_row_Ye;
                                    }
                                    else
                                    {
                                        cell.SetCellValue(double.Parse(TableName.Rows[i][j].ToString()));
                                        cell.CellStyle = style_row;
                                    }

                                }
                                else
                                {
                                    if (TableName.Rows[i][j].ToString() == "Total1")
                                    {
                                        cell.SetCellValue("Total");
                                    }
                                    else if (TableName.Rows[i][j].ToString() == "Total2")
                                    {
                                        cell.SetCellValue("");
                                        sheet.AddMergedRegion(new CellRangeAddress(i+2, i+2, 0, 1));
                                    }
                                    else
                                    {
                                        cell.SetCellValue(TableName.Rows[i][j].ToString());
                                    }
                                    cell.CellStyle = style_header15;
                                }
                            }

                            sheet.SetColumnWidth(j, 15 * 256);
                        }
                        Application.DoEvents();
                    }

                    Application.DoEvents();

                    fs.Close();

                    using (FileStream fs1 = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                    {
                        try
                        {
                            workbook.Write(fs1);
                            fs1.Close();

                            //状态栏更改
                            lblStatus.Text = "生成Excel成功，共耗时" + timer.ElapsedMilliseconds + "毫秒。";
                            Application.DoEvents();

                            //成功提示
                            if (MessageBox.Show("导出成功，是否立即打开？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(filePath);
                            }
                        }
                        catch (Exception error)
                        {
                            fs.Close();
                            MessageBox.Show(error.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {

                    //关闭秒钟
                    timer.Reset();
                    timer.Stop();

                    //赋初始值
                    lblStatus.Visible = false;

                }
            }
           
        }

    }
}
