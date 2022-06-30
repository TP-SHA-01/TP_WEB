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

namespace CSAPPBookingReport
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
            this.combRptType.Items.Add("SPRC Account and Carrier Analysis Report");
            this.combRptType.Items.Add("Volume and Workload Analysis Report");

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


            for (int i = 1; i < 53; i++)
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

            if (this.combRptType.SelectedIndex == -1)
            {
                strmsg = strmsg + "Error Msg:" + "Please select report type." + "\r\n";
            }
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
                strmsg = strmsg + "Error Msg:" + "The selected to week must be after the from week." + "\r\n";
            }

            if (strmsg != "")
            {
                MessageBox.Show(strmsg);
            }
            else
            {
                weekfrom = this.combYearFrom.Text.Trim() + this.combWeekFrom.Text.Trim();
                weekto = this.combYearTo.Text.Trim() + this.combWeekTo.Text.Trim();

                if (this.combRptType.SelectedIndex == 0)
                {
                    DataSet ds = WebApi.Services.BookingAdviceAnalysisRpt_Service.GetCarrierRptByBranchData(weekfrom, weekto);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        ExportDataToExcel2(ds, "SPRC_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss")+ DateTime.Now.Millisecond.ToString());
                    }
                    else
                    {
                        MessageBox.Show("No Data.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (this.combRptType.SelectedIndex == 1)
                {
                    DataSet ds = WebApi.Services.BookingAdviceAnalysisRpt_Service.GetVolumeRptByBranchData(weekfrom, weekto);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        ExportDataToExcel2(ds, "Volume_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss")+ DateTime.Now.Millisecond.ToString());
                    }
                    else
                    {
                        MessageBox.Show("No Data", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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
                    font.FontHeightInPoints = 12;
                    font.FontName = "Arial";
                    style_header.SetFont(font);//HEAD 样式 
                    style_header.FillForegroundColor = 0;
                    style_header.FillPattern = FillPattern.SolidForeground;
                    ((XSSFColor)style_header.FillForegroundColorColor).SetRgb(new byte[] { 188, 210, 238 });
                    style_header.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                    ICellStyle style_row = workbook.CreateCellStyle();
                    IFont font1 = workbook.CreateFont();
                    font1.FontHeightInPoints = 10;
                    font1.FontName = "Arial";
                    style_row.SetFont(font1);//row 样式 
                    style_row.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    

                    ICellStyle style_row_Ye = workbook.CreateCellStyle();
                    IFont font2 = workbook.CreateFont();
                    font2.FontHeightInPoints = 10;
                    font2.FontName = "Arial";
                    style_row_Ye.SetFont(font2);//row 样式 
                    style_row_Ye.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Ye.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Ye.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Ye.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Ye.FillForegroundColor = 0;
                    style_row_Ye.FillPattern = FillPattern.SolidForeground;
                    ((XSSFColor)style_row_Ye.FillForegroundColorColor).SetRgb(new byte[] { 255, 255, 0 });



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

                            if (i == 0 && TableName.TableName != "Booking Advice List")
                            {
                                sheet.SetColumnWidth(i, 50 * 256);
                            }
                            else if( i >0 && TableName.TableName != "Booking Advice List" && TableName.Columns[i].ColumnName.Length < 5)
                            {
                                sheet.SetColumnWidth(i, 7 * 256);
                            }
                            else
                            {
                                sheet.AutoSizeColumn(i);
                            }

                        }

                        //Tables count = 3, sheet1 = By Carrier,Sheet2 = By Account,Sheet3 = List
                        //Sheet1 & Sheet2 最后一行为Header的样式。
                        //Value为0 则为空
                        //Tables Count = 4，Sheet1 = VolumeAnalysis,Sheet2 = WorkLoad,Sheet3 = List
                        //Sheet1 , 为空的数据标黄色
                        //Sheet ， 最后一行为Header的样式
                        //读取数据  

                        //k第几张表
                        for (int i = 0; i < TableName.Rows.Count; i++)
                        {
                            IRow rowData = sheet.CreateRow(i + 1);

                            for (int j = 0; j < TableName.Columns.Count; j++)
                            {
                                ICell cell = rowData.CreateCell(j);

                                if (TableName.Rows[i][j].ToString() == "0" || TableName.Rows[i][j].ToString() == "0.0000")
                                {
                                    cell.SetCellValue("");

                                    if (rowscount == 4 && k == 0 && i != TableName.Rows.Count - 1 && j != 1)
                                    {
                                        cell.CellStyle = style_row_Ye;
                                    }
                                    else if(rowscount == 4 && k == 0 && i == TableName.Rows.Count - 1)
                                    {
                                        cell.CellStyle = style_header; 
                                    }
                                    else if (rowscount == 3 && k < 2 && i == TableName.Rows.Count - 1)
                                    {
                                        cell.CellStyle = style_header;
                                    }
                                    else
                                    {
                                        cell.CellStyle = style_row;
                                    }

                                }
                                else
                                {
                                    if (j != 0 && TableName.Rows[i][j].ToString()!="" && TableName.TableName != "Booking Advice List")
                                    {
                                        try
                                        {
                                            cell.SetCellValue(double.Parse(TableName.Rows[i][j].ToString()));
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }else
                                    {
                                        cell.SetCellValue(TableName.Rows[i][j].ToString());
                                    }

                                    if (rowscount == 4 && k == 0 && i == TableName.Rows.Count - 1)
                                    {
                                        cell.CellStyle = style_header; 
                                    }
                                    else if (rowscount == 3 && k < 2 && i == TableName.Rows.Count - 1)
                                    {
                                        cell.CellStyle = style_header;
                                    }
                                    else
                                    {
                                       cell.CellStyle = style_row;
                                    }

                                }

                               

                            }
                            Application.DoEvents();
                        }

                        if (rowscount == 4 && k == 1)
                        {
                            IRow rowHeader1 = sheet.CreateRow(CSRowsCount);

                            DataTable TableName1 = SetTable.Tables[k + 1];
                            for (int i = 0; i < TableName1.Columns.Count; i++)
                            {
                                ICell cell = rowHeader1.CreateCell(i);
                                cell.SetCellValue(TableName1.Columns[i].ColumnName);
                                cell.CellStyle = style_header;


                                if (i == 0 && TableName1.TableName != "Booking Advice List")
                                {
                                    sheet.SetColumnWidth(i, 50 * 256);
                                }
                                else if (i > 0 && TableName1.TableName != "Booking Advice List" && TableName1.Columns[i].ColumnName.Length < 5)
                                {
                                    sheet.SetColumnWidth(i, 7 * 256);
                                }
                                else
                                {
                                    sheet.AutoSizeColumn(i);
                                }

                            }
                            CSRowsCount = CSRowsCount + 1;

                            for (int i = 0; i < TableName1.Rows.Count; i++)
                            {
                                IRow rowData1 = sheet.CreateRow(CSRowsCount + i);

                                for (int j = 0; j < TableName1.Columns.Count; j++)
                                {
                                    ICell cell = rowData1.CreateCell(j);
                                    if (TableName1.Rows[i][j].ToString() == "0" || TableName1.Rows[i][j].ToString() == "0.0000")
                                    {
                                        cell.SetCellValue("");
                                        cell.CellStyle = style_row;
                                    }
                                    else
                                    {
                                        if (j != 0 && TableName1.Rows[i][j].ToString() != "" && TableName1.TableName != "Booking Advice List" )
                                        {
                                            try
                                            {
                                                cell.SetCellValue(double.Parse(TableName1.Rows[i][j].ToString()));
                                            }
                                            catch (Exception ex)
                                            {
                                                MessageBox.Show(ex.Message, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                        }
                                        else
                                        {
                                            cell.SetCellValue(TableName1.Rows[i][j].ToString());
                                        }


                                        cell.CellStyle = style_row;
                                    }
                                }
                                Application.DoEvents();
                            }
                            k = k + 1;
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
                    barStatus.Visible = false;

                }
            }
        }

        private void btnConnectDB_Click(object sender, EventArgs e)
        {

        }
    }
}
