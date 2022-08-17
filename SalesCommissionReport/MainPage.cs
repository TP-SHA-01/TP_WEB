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
using System.Diagnostics;

using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace SalesCommissionReport
{
    public partial class MainPage : Form
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnSelect1_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            try
            {
                System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
                if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string Selected_File = openfiledialog.FileName;
                    this.txtFilePath1.Text = Selected_File.Trim();
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btnSelect2_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            try
            {
                System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
                if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string Selected_File = openfiledialog.FileName;
                    this.txtFilePath2.Text = Selected_File.Trim();
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string path = string.Empty;

            try
            {
                System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
                if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string Selected_File = openfiledialog.FileName;
                    this.txtFilePath3.Text = Selected_File.Trim();
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            string Path1 = this.txtFilePath1.Text.ToString();
            string Path2 = this.txtFilePath2.Text.ToString();
            string Path3 = this.txtFilePath3.Text.ToString();
            DateTime dtAnalysisDate = this.dateTimePicker1.Value;

            if(dtAnalysisDate.ToString()=="")
            {
                MessageBox.Show("Please inout the Analysis Date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Path1.ToString() != "" && Path2.ToString() != "")
            {
                DataSet TableNew = ExcelToDataSet(Path1, Path2, Path3, dtAnalysisDate);

                if (TableNew.Tables.Count > 0)
                {
                    bool SaveSuccess = AnalysisData(TableNew);
                }
            }
            else
            {
                MessageBox.Show("Please inout the Sales Report Path. or Sales Commiontion Path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        public static DataSet ExcelToDataSet(string Path1, string Path2, string Path3, DateTime oldDate)
        {
            int startRow = 0;
            DataSet dsTable = new DataSet();
            DataTable otherTable = new DataTable();

            string File_salesreport = Path1.ToString();
            string File_salesCommissionreport = Path2.ToString();
            string File_ARreport = Path3.ToString();

            using (FileStream fs = File.OpenRead(File_salesreport))
            {
                IWorkbook workbook_salesreport = null;
                IWorkbook workbook_salesCommissionreport = null;
                IWorkbook workbook_ARreport = null;

                // 如果是2007+的Excel版本
                if (File_salesreport.IndexOf(".xlsx") > 0)
                {
                    workbook_salesreport = new XSSFWorkbook(fs);
                }
                else if (File_salesreport.IndexOf(".xls") > 0) // 如果是2003-的Excel版本
                {
                    workbook_salesreport = new HSSFWorkbook(fs);
                }

                FileStream fs1 = File.OpenRead(File_salesCommissionreport);

                if (File_salesCommissionreport.IndexOf(".xlsx") > 0)
                {    
                   workbook_salesCommissionreport = new XSSFWorkbook(fs1);
                }
                else if (File_salesCommissionreport.IndexOf(".xls") > 0) // 如果是2003-的Excel版本
                {
                    workbook_salesCommissionreport = new HSSFWorkbook(fs1);
                }

                if (File_ARreport != "")
                {
                    FileStream fs2 = File.OpenRead(File_ARreport);

                    if (File_ARreport.IndexOf(".xlsx") > 0)
                    {
                        workbook_ARreport = new XSSFWorkbook(fs2);
                    }
                    else if (File_ARreport.IndexOf(".xls") > 0) // 如果是2003-的Excel版本
                    {
                        workbook_ARreport = new HSSFWorkbook(fs2);
                    }
                }

                if (workbook_salesreport != null && workbook_salesCommissionreport!=null)
                {
                    string strColumn = @"Month,Week,b/l,Profit(RMB),Empty1,Consignee,Shipper,Principal,NoofCtns,LotNo,Income,
                                        Cost,Profit/(loss)RMB,%,Exchdiff,Empty2,Whicheverislower,>RMB20000,>RMB50000,>RMB120000,>RMB120001,
                                        Empty3,(X)-RMB9000,30%,Type,No";

                    //income='C:\FLXOLE\FLXACC.XLA'!FlxAcc("leg("&$K$5&")val(cya)pri("&$K$6&")a/c(4000..59999)rdc(1)ana(1:"&J10&")bch(00..ZZ)")
                    //Cost='C:\FLXOLE\FLXACC.XLA'!FlxAcc("leg("&$K$5&")val(cya)pri("&$K$6&")a/c(6000..69999)rdc(1)ana(1:"&J10&")bch(00..ZZ)")
                    //Profit=SUM(K10:L10)
                    //OnSales=+M10/K10
                    // = =+M11-D11
                    //=IF(+M10>D10,D10,M10)

                    string[] strColumn_S0 = strColumn.Split(',');
                    for (int cc = 0; cc < strColumn_S0.Length; cc++)
                    {
                        //构建datatable的列名称
                        DataColumn column = new DataColumn(strColumn_S0[cc].Trim());
                        otherTable.Columns.Add(column);
                    }

                    //循环读取Excel的每个sheet，每个sheet页都转换为一个DataTable，并放在DataSet中

                    int SheetARreportCount = 0;
                    int SheetsalesreportCount = 0;
                    SheetsalesreportCount = workbook_salesreport.NumberOfSheets;
                    int SheetsalesCommissionreportCount = 0;
                    SheetsalesCommissionreportCount = workbook_salesCommissionreport.NumberOfSheets;

                    if (workbook_ARreport != null)
                        SheetARreportCount = workbook_ARreport.NumberOfSheets;
                    else
                        SheetARreportCount = 0;

                    if (SheetsalesreportCount > 1)
                    {
                        for (int p = 1; p < SheetsalesreportCount; p++)
                        {
                            DataTable dataTable = new DataTable();

                            string[] strColumn_S = strColumn.Split(',');
                            for (int cc = 0; cc < strColumn_S.Length; cc++)
                            {
                                //构建datatable的列名称
                                DataColumn column = new DataColumn(strColumn_S[cc].Trim());
                                dataTable.Columns.Add(column);
                            }
                            //构建datatable的列

                            ISheet sheet = workbook_salesreport.GetSheetAt(p);

                            string ErrorSheetName = sheet.SheetName.ToString();

                            if (sheet != null)
                            {
                                //查找数据在第几行
                                int rowCount = sheet.LastRowNum;//获取总行数
                                int FindRow = 0;

                                if (rowCount > 0)
                                {
                                    for (int r = 0; r <= rowCount; r++)
                                    {
                                        FindRow = 0;

                                        IRow FindFirstRow = sheet.GetRow(r);//获取第r行

                                        if (FindFirstRow != null)
                                        {
                                            if (FindFirstRow.LastCellNum > 10 && FindFirstRow.GetCell(1) != null)
                                            {
                                               
                                                    if (FindFirstRow.GetCell(0).ToString().ToLower().Trim() == "no" && FindFirstRow.GetCell(1).ToString().ToLower().Contains("year"))
                                                    {
                                                        FindRow = r+1;//找到数据开始的位置
                                                        break;

                                                    }
                                            }
                                        }
                                    }
                                }

                                if (FindRow > 0)
                                {
                                    // TransType,Month,Week,b/l,Profit(RMB),Consignee,Shipper,Principal,No of Ctns,Lot No,Income,    //10
                                    // Cost,RMB,%,Exch diff,is lower,>RMB20,000,>RMB50,000,>RMB120,000,,,(X)-RMB9000,30%             //11

                                    //1. LATE PAYMENT FROM CLIENT -10 % (ETD, 需要提供ETD Report 来判断，只要存在于AR Report 中的数据，就认为是此类型.)
                                    //2. LOST ACCOUNT -10 % - （Commission List Type 里面 = LOSS ACCOUNT）
                                    //3. CREDIT OVER 30 DAYS - 10 % - （Remark 里面判断大于30天的，不包含30天。）
                                    //根据Remarks里面的关键字来判断:30 days,45 days,60 days,90 days
                                    //4. SALE LEAD -OVER ONE YEAR(fm 2nd yr to 5th yr) -7 % （52 < Week <= 5 * 52周的数据）
                                    //5. 正常的30 %（不属于下列任何情况,第一年，Week =< 52周）

                                    //TransType:LATE PAYMENT/LOST ACCOUNT/CREDIT OVER/SALE LEAD/NORMAL/OTHER(不属于以上5种情况，都归属于OTHER) 
                                    //TP
                                    //NON - TP
                                    //IMPORT
                                    //AIR


                                    IRow firstRow = sheet.GetRow(FindRow);//获取第r行
                                    int cellCount = firstRow.LastCellNum;//获取总列数

                                    //填充行
                                    startRow = FindRow + 1;

                                    for (int i = startRow; i <= rowCount; i++)
                                    {
                                        IRow row = sheet.GetRow(i);

                                        if (row == null) continue;

                                        if (strColumn_S.Length > 15 && cellCount > 8)
                                        {
                                            if (row == null) continue;
                                            if (row.GetCell(0) == null) continue;
                                            if (row.GetCell(1) == null) continue;
                                            if (row.GetCell(0).ToString() == "" && row.GetCell(2).ToString() == "") continue;

                                            //int SheetsalesreportCount = workbook_salesreport.NumberOfSheets;
                                            //int SheetsalesCommissionreportCount = workbook_salesCommissionreport.NumberOfSheets;
                                            //int SheetARreportCount = workbook_ARreport.NumberOfSheets;
                                            bool FindAR = false;

                                            DataRow dataRow = dataTable.NewRow();
                                            #region Ren Open
                                            string str_TransType = "OTHER"; //LATE PAYMENT/LOST ACCOUNT/CREDIT OVER/SALE LEAD/NORMAL/OTHER
                                            string str_NO = "";
                                            string str_Month = "";
                                            string str_Week = "";
                                            string str_bAndl = "";
                                            string str_Profit= "";
                                            string str_Consignee = "";
                                            string str_Shipper = "";
                                            string str_Principal = "";
                                            string str_NoofCtns = ""; 
                                            string str_LotNo = "";
                                            string str_Income = "";
                                            string str_Cost = "";
                                            string str_RMB = "";
                                            string str_Procent = "";
                                            string str_Exchdiff = "";
                                            string str_islower = "";
                                            string str_X9000 = "";
                                            string str_30 = "";

                                            str_LotNo = row.GetCell(13).ToString();
                                            str_NO = row.GetCell(0).ToString();
                                            str_Month = row.GetCell(1).ToString();
                                            str_Week = row.GetCell(2).ToString();
                                            str_bAndl = row.GetCell(3).ToString();
                                            str_Profit = row.GetCell(4).ToString();
                                            str_Consignee = row.GetCell(6).ToString();
                                            str_Shipper = row.GetCell(5).ToString();
                                            str_Principal = row.GetCell(7).ToString();
                                            str_NoofCtns = row.GetCell(11).ToString();


                                            try
                                            {
                                                string strAnalysis = "";

                                                if (SheetARreportCount > 0)
                                                {
                                                    //AR 20220601.xlsx
                                                    // J列，ColumnName = Analysis code 1(Lost No.)
                                                    //CommissionList20220715.xls
                                                    //Shanghai Sales Commission Report _ Jun.xlsx = LotNo
                                                    //AR.Analysis code 1 = Sales.LotNo 找到的话 Type = 1. LATE PAYMENT FROM CLIENT -10 %
                                                    //根据SHPR & CNEE 找到相同的 Type的类型：C&F,DDP,FDH,FOB,LOSS ACCOUNT,SALES LEAD
                                                    //根据Remark填入的时间来判断在什么区间内 CREDIT OVER 30 DAYS - 10% 
                                                    //CommissionList Total Sales
                                                    //str_NO = row.GetCell(0);
                                                    //查找数据在第几行
                                                    
                                                    int FindRowR = 0;
                                                    for (int r = 1; r < SheetARreportCount; r++)
                                                    {   //xia
                                                        ISheet sheetR = workbook_ARreport.GetSheetAt(r);
                                                        int rowCountR = sheetR.LastRowNum;//获取总行数
                                                        string ErrorSheetNameR = sheetR.SheetName.ToString();
                                                        if (sheetR != null)
                                                        {
                                                            if (rowCountR > 0)
                                                            {
                                                                for (int rr = 0; rr <= rowCount; rr++)
                                                                {
                                                                    FindRowR = 0;

                                                                    IRow FindFirstRowR = sheet.GetRow(rr);//获取第r行

                                                                    if (FindFirstRowR != null)
                                                                    {
                                                                        if (FindFirstRowR.LastCellNum > 1 && FindFirstRowR.GetCell(1) != null)
                                                                        {

                                                                            if (FindFirstRowR.GetCell(0).ToString().ToLower().Trim() == "'Analysis code 1" && FindFirstRowR.GetCell(1).ToString().ToLower().Contains("'Account code"))
                                                                            {
                                                                                FindRowR = rr + 1;//找到数据开始的位置
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }


                                                            }
                                                        }
                                                    }

                                                    for(int rrr=FindRowR;rrr<SheetARreportCount;rrr++)
                                                    {

                                                        if (row.GetCell(9).CellType == CellType.Numeric || row.GetCell(9).CellType == CellType.Formula)
                                                        {
                                                            strAnalysis = row.GetCell(9).NumericCellValue.ToString();
                                                        }
                                                        else if (row.GetCell(0).CellType == CellType.String)
                                                            strAnalysis = row.GetCell(9).StringCellValue.ToString();
                                                        else
                                                            strAnalysis = "";

                                                        if(str_LotNo == strAnalysis)
                                                        {
                                                            FindAR = true;
                                                            break;
                                                        }
                                                    }
                                                  
                                                }
                                                                                

                                            }
                                            catch (Exception ex)
                                            {
                                                var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + ex.Message + "Line:" + i;
                                                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return null;
                                            }

                                            string strTypeFromCommission = "";
                                            string strRemarkFromCommission = "";
                                            string strFirstCommissionDateWeek = "";
                                            string strFirstShipmentDateWeek = "";

                                            
                                            dataRow[0] = str_Month.Trim();
                                            dataRow[1] = str_Week.Trim();
                                            dataRow[2] = str_bAndl.Trim();
                                            dataRow[3] = "";
                                            dataRow[4] = str_Profit.Trim();     
                                            dataRow[5] = str_Consignee.Trim();
                                            dataRow[6] = str_Shipper.Trim();
                                            dataRow[7] = str_Principal.Trim();
                                            dataRow[8] = str_NoofCtns.Trim();
                                            dataRow[9] = str_LotNo.Trim();

                                            try
                                            {
                                                if (SheetsalesCommissionreportCount > 0)
                                                {
                                                    //AR.Analysis code 1 = Sales.LotNo 找到的话 Type = 1. LATE PAYMENT FROM CLIENT -10 %
                                                    //根据SHPR & CNEE 找到相同的 Type的类型：C&F,DDP,FDH,FOB,LOSS ACCOUNT,SALES LEAD
                                                    //根据Remark填入的时间来判断在什么区间内 CREDIT OVER 30 DAYS - 10% 
                                                    //CommissionList Total Sales

                                                    int FindRowS = 0;
                                                    for (int r = 1; r < SheetsalesCommissionreportCount; r++)
                                                    {
                                                        ISheet sheetS = workbook_salesCommissionreport.GetSheetAt(r);
                                                        int rowCountS = sheetS.LastRowNum;//获取总行数
                                                        string ErrorSheetNameR = sheetS.SheetName.ToString();
                                                        if (sheetS != null)
                                                        {
                                                            if (rowCountS > 0)
                                                            {
                                                                for (int rr = 0; rr <= rowCount; rr++)
                                                                {
                                                                    FindRowS = 0;

                                                                    IRow FindFirstRowR = sheet.GetRow(rr);//获取第r行

                                                                    if (FindFirstRowR != null)
                                                                    {
                                                                        if (FindFirstRowR.LastCellNum > 1 && FindFirstRowR.GetCell(1) != null)
                                                                        {

                                                                            if (FindFirstRowR.GetCell(0).ToString().ToLower().Trim() == "RefId" && FindFirstRowR.GetCell(1).ToString().ToLower().Contains("ConCd"))
                                                                            {
                                                                                FindRowS = rr + 1;//找到数据开始的位置
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }


                                                            }
                                                        }
                                                    }

                                                    for (int rrr = FindRowS; rrr < SheetsalesCommissionreportCount; rrr++)
                                                    {

                                                        string CNEES = "";
                                                        string SHPRS = "";

                                                       if (row.GetCell(2).CellType == CellType.Numeric || row.GetCell(2).CellType == CellType.Formula)
                                                        {
                                                            CNEES = row.GetCell(2).NumericCellValue.ToString();
                                                        }
                                                        else if (row.GetCell(2).CellType == CellType.String)
                                                            CNEES = row.GetCell(2).StringCellValue.ToString();
                                                        else
                                                            CNEES = "";

                                                        if (row.GetCell(4).CellType == CellType.Numeric || row.GetCell(4).CellType == CellType.Formula)
                                                        {
                                                            SHPRS = row.GetCell(4).NumericCellValue.ToString();
                                                        }
                                                        else if (row.GetCell(4).CellType == CellType.String)
                                                            SHPRS = row.GetCell(4).StringCellValue.ToString();
                                                        else
                                                            SHPRS = "";

                                                        if(str_Consignee==CNEES && str_Shipper==SHPRS)
                                                        {
                                                            strTypeFromCommission = row.GetCell(9).StringCellValue.ToString();
                                                            strRemarkFromCommission = row.GetCell(19).StringCellValue.ToString();
                                                            
                                                            strFirstCommissionDateWeek = row.GetCell(16).StringCellValue.ToString();
                                                            strFirstShipmentDateWeek = row.GetCell(12).StringCellValue.ToString();

                                                            strRemarkFromCommission = row.GetCell(16).StringCellValue.ToString();
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + ex.Message + "Line:" + i;
                                                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return null;
                                            }

                                            dataRow[10] = str_Income.Trim();
                                            dataRow[11] = str_Cost.Trim();
                                            dataRow[12] = str_RMB.Trim();
                                            dataRow[13] = str_Procent.Trim();
                                            dataRow[14] = "";
                                            dataRow[15] = str_Exchdiff.Trim();
                                            dataRow[16] = str_islower.Trim();
                                            dataRow[17] = "";
                                            dataRow[18] = "";
                                            dataRow[19] = "";
                                            dataRow[20] = "";
                                            dataRow[21] = "";
                                            dataRow[22] = str_X9000.Trim();
                                            dataRow[23] = str_30.Trim();

                                            if (FindAR)
                                            {
                                                str_TransType = "LATE PAYMENT";
                                            }
                                            else
                                            {
                                                if(strRemarkFromCommission.ToLower().Contains("45 days") || strRemarkFromCommission.ToLower().Contains("60 days")|| strRemarkFromCommission.ToLower().Contains("90 days"))
                                                {
                                                    str_TransType = "CREDIT OVER";
                                                }
                                                else
                                                {
                                                    if(strTypeFromCommission.ToUpper().Contains("LOSS ACCOUNT"))
                                                    {
                                                        str_TransType = "LOSS ACCOUNT";
                                                    }
                                                    else
                                                    {
                                                        if(Convert.ToInt32(strFirstShipmentDateWeek)<=52)
                                                        {
                                                            str_TransType = "NORMAL";
                                                        }else if (30 < Convert.ToInt32(strFirstCommissionDateWeek) && Convert.ToInt32(strFirstCommissionDateWeek) <= 5 * 52)
                                                        {
                                                            str_TransType = "SALES LEAD";
                                                        }
                                                        else
                                                        {
                                                            str_TransType = "OTHER";
                                                        }
                                                    }
                                                }
                                            }

                                            dataRow[24] = str_TransType;
                                            dataRow[25] = str_NO;

                                            //,DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP
                                            if (str_TransType=="OTHER")
                                                otherTable.Rows.Add(dataRow);
                                            else
                                                dataTable.Rows.Add(dataRow);
                                        }

                                    }
                                    #endregion
                                }

                                dataTable.TableName = sheet.SheetName.ToString();
                                dsTable.Tables.Add(dataTable);

                            }
                        }

                        otherTable.TableName = "OTHER";
                        dsTable.Tables.Add(otherTable);
                    }

                }
            }

            return dsTable;

        }

        private bool AnalysisData(DataSet dsData)
        {
            bool Finish = false;
            DataSet ds = new DataSet();


            for (int k = 0; k < dsData.Tables.Count; k++)
            {
                DataTable dtTable = dsData.Tables[k];

                if(dtTable.TableName!="OTHER")
                {

                    //string strColumn = @"Month,Week,b/l,Profit(RMB),Empty1,Consignee,Shipper,Principal,NoofCtns,LotNo,Income,Cost,Profit/(loss)RMB,%,Exchdiff,Empty2,Whicheverislower,(X)-RMB9000,30%,Type,No";


                    int strweek = Convert.ToInt32(dtTable.Rows[0]["Week"].ToString());
                    int strYEAR = Convert.ToInt32(dtTable.Rows[0]["Year"].ToString());
                    string strUserName = dtTable.TableName.ToString();

                    string strRow1 = strUserName + "'s Performance" + 
                        " for commission for " + GetMonthWeekFrom(strweek).ToString() +" "+ strYEAR.ToString();

                    //XXX's Performance for commission for Jun 2022  0-15column
                    DataColumn dc0 = new DataColumn(strRow1, Type.GetType("System.String"));
                    DataColumn dc1 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc2 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc3 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc4 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc5 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc6 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc7 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc8 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc9 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc10 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc11 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc12 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc13 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc14 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc15 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc16 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc17 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc18 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc19 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc20 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc21 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc22 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc23 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc24 = new DataColumn("", Type.GetType("System.String"));
                    DataColumn dc25 = new DataColumn("", Type.GetType("System.String"));


                    DataTable dtsales = new DataTable();
                    dtsales.Columns.Add(dc0);//A
                    dtsales.Columns.Add(dc1);//B
                    dtsales.Columns.Add(dc2);//C
                    dtsales.Columns.Add(dc3);//D
                    dtsales.Columns.Add(dc4);//E
                    dtsales.Columns.Add(dc5);//F
                    dtsales.Columns.Add(dc6);//G
                    dtsales.Columns.Add(dc7);//H
                    dtsales.Columns.Add(dc8);//I
                    dtsales.Columns.Add(dc9); //J
                    dtsales.Columns.Add(dc10);//K

                    dtsales.Columns.Add(dc11);//L  //Cost
                    dtsales.Columns.Add(dc12);//M
                    dtsales.Columns.Add(dc13);//N
                    dtsales.Columns.Add(dc14);//O
                    dtsales.Columns.Add(dc15);//P
                    dtsales.Columns.Add(dc16);//Q
                    dtsales.Columns.Add(dc17);//R
                    dtsales.Columns.Add(dc18);//S
                    dtsales.Columns.Add(dc19);//T
                    dtsales.Columns.Add(dc20);//U

                    dtsales.Columns.Add(dc21);//V
                    dtsales.Columns.Add(dc22);//W
                    dtsales.Columns.Add(dc23);//X

                    dtsales.Columns.Add(dc24);//
                    dtsales.Columns.Add(dc25);//

                    Double ALLTotalProfit = 0.00;

                    #region Two empty lines
                    DataRow dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " ";dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " "; 
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);
                    #endregion

                    DataRow dataNewline = dtsales.NewRow();
                    dataNewline[0] = " "; dataNewline[1] = " "; dataNewline[2] = "From Sales department"; dataNewline[3] = " "; dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = "Accounts Department"; dataNewline[11] = " ";
                    dataNewline[12] = " "; dataNewline[13] = " "; dataNewline[14] = " "; dataNewline[15] = " "; dataNewline[16] = " "; dataNewline[17] = " ";
                    dataNewline[18] = " "; dataNewline[19] = " "; dataNewline[20] = "Commission"; dataNewline[21] = " "; dataNewline[22] = " "; dataNewline[23] = " ";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = " "; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = " "; dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = "SHG"; dataNewline[11] = " ";
                    dataNewline[12] = " "; dataNewline[13] = " "; dataNewline[14] = " "; dataNewline[15] = " "; dataNewline[16] = "Y"; dataNewline[17] = " ";
                    dataNewline[18] = "20000"; dataNewline[19] = "50000"; dataNewline[20] = "120000"; dataNewline[21] = "200000"; dataNewline[22] = " "; dataNewline[23] = " ";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = " "; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = " "; dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = GetMonthWeekFrom(strweek).ToString() + "/" + strYEAR.ToString(); dataNewline[11] = "3000";
                    dataNewline[12] = " "; dataNewline[13] = " "; dataNewline[14] = " "; dataNewline[15] = " "; dataNewline[16] = "N"; dataNewline[17] = " ";
                    dataNewline[18] = "15%"; dataNewline[19] = "20%"; dataNewline[20] = "25%"; dataNewline[21] = "30%"; dataNewline[22] = " "; dataNewline[23] = " ";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = " "; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = "X"; dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = " "; dataNewline[11] = " ";
                    dataNewline[12] = "Profit/(loss)"; dataNewline[13] = " "; dataNewline[14] = " "; dataNewline[15] = " "; dataNewline[16] = "Whichever"; dataNewline[17] = " ";
                    dataNewline[18] = " "; dataNewline[19] = " "; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = " "; dataNewline[23] = " ";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);


                    dataNewline = dtsales.NewRow();
                    for (int m = 0; k < dtsales.Columns.Count; k++)
                    {
                        dataNewline[m] = dtsales.Columns[m];
                    }
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = " "; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = " "; dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] ="a"; dataNewline[11] = "b";
                    dataNewline[12] = "a-b=c"; dataNewline[13] = "on sales"; dataNewline[14] = "d-x : RMB"; dataNewline[15] = "RMB (X)"; dataNewline[16] = "N/Y"; dataNewline[17] = "N/Y";
                    dataNewline[18] = "N/Y"; dataNewline[19] = "N/Y"; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = "'(Z)"; dataNewline[23] = "RMB";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);


                    //str_TransType = "CREDIT OVER";
                    //str_TransType = "LOSS ACCOUNT";
                    //str_TransType = "NORMAL";
                    //str_TransType = "SALES LEAD";
                    //str_TransType = "OTHER";

                    double TotalProfit = 0.00;

                    for (int n =0;n< dtTable.Rows.Count;n++)
                    {
                        DataRow dr = dtsales.Rows[n];

                        if (dr[26].ToString() == "NORMAL")
                        {
                            dtsales.Rows.Add(dr);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }
                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "SALE LEAD - OVER ONE YEAR (fm 2nd yr to 5th yr) - 7%"; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);

                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtsales.Rows[n];

                        if (dr[26].ToString() == "SALES LEAD")
                        {
                            dtsales.Rows.Add(dr);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }
                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);
                 

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "CREDIT OVER 30 DAYS - 10%"; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);

                    //CREDIT OVER 30 DAYS - 10%
                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtsales.Rows[n];

                        if (dr[26].ToString() == "CREDIT OVER")
                        {
                            dtsales.Rows.Add(dr);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }

                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "LATE PAYMENT FROM CLIENT - 10%"; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);

                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtsales.Rows[n];

                        if (dr[26].ToString() == "LOSS ACCOUNT")
                        {
                            dtsales.Rows.Add(dr);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }

                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "Total"; dataNewline[1] = " "; dataNewline[2] = ""; dataNewline[3] = ALLTotalProfit.ToString(); dataNewline[4] = " "; dataNewline[5] = " ";
                    dataNewline[6] = " "; dataNewline[7] = " "; dataNewline[8] = " "; dataNewline[9] = " "; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = " "; dataNewline[21] = " "; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = " "; dataNewline[25] = " ";
                    dtsales.Rows.Add(dataNewline);


                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "Reviewed by :  "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "Prepared by:  "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                    dataEmpty[6] = " "; dataEmpty[7] = " "; dataEmpty[8] = " "; dataEmpty[9] = " "; dataEmpty[10] = " "; dataEmpty[11] = " ";
                    dataEmpty[12] = " "; dataEmpty[13] = " "; dataEmpty[14] = " "; dataEmpty[15] = " "; dataEmpty[16] = " "; dataEmpty[17] = " ";
                    dataEmpty[18] = " "; dataEmpty[19] = " "; dataEmpty[20] = " "; dataEmpty[21] = " "; dataEmpty[22] = " "; dataEmpty[23] = " ";
                    dataEmpty[24] = " "; dataEmpty[25] = " ";
                    dtsales.Rows.Add(dataEmpty);

                }
                else
                {
                    
                    ds.Tables.Add(dtTable);
                }
            }

            return Finish;
        }

        public string GetMonthWeekFrom(int month)
        {
            int m = month;
            string TempFromno = "";
            switch (m)
            {
                case 1:
                    TempFromno = "01";
                    break;
                case 2:
                    TempFromno = "06";
                    break;
                case 3:
                    TempFromno = "10";
                    break;
                case 4:
                    TempFromno = "14";
                    break;
                case 5:
                    TempFromno = "19";
                    break;
                case 6:
                    TempFromno = "23";
                    break;
                case 7:
                    TempFromno = "27";
                    break;
                case 8:
                    TempFromno = "32";
                    break;
                case 9:
                    TempFromno = "36";
                    break;
                case 10:
                    TempFromno = "40";
                    break;
                case 11:
                    TempFromno = "45";
                    break;
                case 12:
                    TempFromno = "49";
                    break;
                default:
                    break;
            }

            return TempFromno;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
