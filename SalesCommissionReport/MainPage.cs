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

            if(dtAnalysisDate.ToString()=="" && Path3 !="")
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

        public static DataSet ExcelToDataSet(string Path1, string Path2, string Path3, DateTime dtAnalysisDate)
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
                                        Cost,RMB,%,Exchdiff,Empty2,islower,>RMB20000,>RMB50000,>RMB120000,>RMB120001,
                                        Empty3,(X)-RMB9000,30%,Type,No,Year,sales";

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
                                            string strTypeFromCommission = "";
                                            string strRemarkFromCommission = "";
                                            string strFirstCommissionDateWeek = "0";
                                            string strFirstShipmentDateWeek = "0";

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
                                            string str_year = "";
                                            string str_sales = "";

                                            str_LotNo = row.GetCell(13).ToString();
                                            str_NO = row.GetCell(0).ToString();
                                            str_Week = row.GetCell(2).ToString();
                                            str_bAndl = row.GetCell(3).ToString();
                                            str_Profit = row.GetCell(4).ToString();
                                            str_Consignee = row.GetCell(6).ToString();
                                            str_Shipper = row.GetCell(5).ToString();
                                            str_Principal = row.GetCell(7).ToString();
                                            str_NoofCtns = row.GetCell(11).ToString();
                                            str_year = row.GetCell(1).ToString();
                                            str_sales = row.GetCell(9).ToString();

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
                                                    int rowCountR = 0;

                                                    if (SheetARreportCount > 0)
                                                    {
                                                        ISheet sheetR = workbook_ARreport.GetSheetAt(0);
                                                        rowCountR = sheetR.LastRowNum;//获取总行数
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


                                                        //LATE PAYMENT FROM CLIENT -10 %
                                                        //(1.Run Date > ETD )
                                                        //(2.并存在于AR Report 中的数据。)

                                                        for (int rrr = FindRowR; rrr < SheetARreportCount; rrr++)
                                                        {
                                                            IRow rowR = sheetR.GetRow(rrr);

                                                            string strETDDate = "";

                                                            if (rowR.GetCell(9).CellType == CellType.Numeric || rowR.GetCell(9).CellType == CellType.Formula)
                                                            {
                                                                strAnalysis = rowR.GetCell(9).NumericCellValue.ToString();
                                                            }
                                                            else if (rowR.GetCell(9).CellType == CellType.String)
                                                                strAnalysis = rowR.GetCell(9).StringCellValue.ToString();
                                                            else
                                                                strAnalysis = "";


                                                            if (rowR.GetCell(4).CellType == CellType.Numeric || rowR.GetCell(4).CellType == CellType.Formula)
                                                            {
                                                                strETDDate = rowR.GetCell(4).NumericCellValue.ToString();
                                                            }
                                                            else if (rowR.GetCell(4).CellType == CellType.String)
                                                                strETDDate = rowR.GetCell(4).StringCellValue.ToString();
                                                            else
                                                                strETDDate = "";


                                                            if (str_LotNo == strAnalysis)
                                                            {
                                                                if (Convert.ToDateTime(dtAnalysisDate) > Convert.ToDateTime(strETDDate))
                                                                {
                                                                    FindAR = true;
                                                                    break;
                                                                }
                                                            }
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

                                            dataRow[0] = GetMonthByWeek(Convert.ToInt32(str_Week)).ToString(); //Month,
                                            dataRow[1] = str_Week.Trim();//Week,
                                            dataRow[2] = str_bAndl.Trim();//b/l,
                                            dataRow[3] = str_Profit.Trim(); //Profit(RMB),
                                            dataRow[4] = "";                //Empty1,
                                            dataRow[5] = str_Consignee.Trim();//Consignee,
                                            dataRow[6] = str_Shipper.Trim();//Shipper,
                                            dataRow[7] = str_Principal.Trim();//Principal,
                                            dataRow[8] = str_NoofCtns.Trim();//NoofCtns,
                                            dataRow[9] = str_LotNo.Trim();//LotNo,

                                            #region

                                            try
                                            {
                                                if (SheetsalesCommissionreportCount > 0)
                                                {
                                                    //AR.Analysis code 1 = Sales.LotNo 找到的话 Type = 1. LATE PAYMENT FROM CLIENT -10 %
                                                    //根据SHPR & CNEE 找到相同的 Type的类型：C&F,DDP,FDH,FOB,LOSS ACCOUNT,SALES LEAD
                                                    //根据Remark填入的时间来判断在什么区间内 CREDIT OVER 30 DAYS - 10% 
                                                    //CommissionList Total Sales

                                                    int FindRowS = 0;
                                                    int CommissionSheet = 0;
                                                    int rowCountS = 0;
                                                    for (int r = 0; r < SheetsalesCommissionreportCount; r++)
                                                    {
                                                        ISheet sheetS = workbook_salesCommissionreport.GetSheetAt(r);
                                                        rowCountS  = sheetS.LastRowNum;//获取总行数
                                                        string ErrorSheetNameR = sheetS.SheetName.ToString();

                                                        CommissionSheet = r;

                                                        if (sheetS != null)
                                                        {
                                                            if (rowCountS > 0)
                                                            {
                                                                for (int rr = 0; rr <= rowCount; rr++)
                                                                {
                                                                    FindRowS = 0;

                                                                    IRow FindFirstRowR = sheetS.GetRow(rr);//获取第r行

                                                                    if (FindFirstRowR != null)
                                                                    {
                                                                        if (FindFirstRowR.LastCellNum > 1 && FindFirstRowR.GetCell(1) != null)
                                                                        {

                                                                            if (FindFirstRowR.GetCell(0).ToString().Trim() == "RefId" && FindFirstRowR.GetCell(1).ToString().Contains("ConCd"))
                                                                            {
                                                                                FindRowS = rr + 1;//找到数据开始的位置
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                if(FindRowS>0)
                                                                {
                                                                    break;
                                                                }

                                                            }
                                                        }
                                                    }

                                                    for (int rrr = FindRowS; rrr <= rowCountS; rrr++)
                                                    {

                                                        ISheet sheetS = workbook_salesCommissionreport.GetSheetAt(CommissionSheet);
                                                        IRow rowS = sheetS.GetRow(rrr);

                                                        string CNEES = "";
                                                        string SHPRS = "";

                                                       if (rowS.GetCell(2).CellType == CellType.Numeric || rowS.GetCell(2).CellType == CellType.Formula)
                                                        {
                                                            CNEES = rowS.GetCell(2).NumericCellValue.ToString();
                                                        }
                                                        else if (rowS.GetCell(2).CellType == CellType.String)
                                                            CNEES = rowS.GetCell(2).StringCellValue.ToString();
                                                        else
                                                            CNEES = "";

                                                        if (rowS.GetCell(4).CellType == CellType.Numeric || rowS.GetCell(4).CellType == CellType.Formula)
                                                        {
                                                            SHPRS = rowS.GetCell(4).NumericCellValue.ToString();
                                                        }
                                                        else if (rowS.GetCell(4).CellType == CellType.String)
                                                            SHPRS = rowS.GetCell(4).StringCellValue.ToString();
                                                        else
                                                            SHPRS = "";

                                                        if(str_Consignee.ToLower().Trim()==CNEES.ToLower().Trim() && str_Shipper.ToLower().Trim() == SHPRS.ToLower().Trim())
                                                        {                                                 

                                                            if (rowS.GetCell(9).CellType == CellType.Numeric || rowS.GetCell(9).CellType == CellType.Formula)
                                                            {
                                                                strTypeFromCommission = rowS.GetCell(9).NumericCellValue.ToString();
                                                            }
                                                            else if (rowS.GetCell(9).CellType == CellType.String)
                                                                strTypeFromCommission = rowS.GetCell(9).StringCellValue.ToString();
                                                            else
                                                                strTypeFromCommission = "";

                                                            if (rowS.GetCell(19).CellType == CellType.Numeric || rowS.GetCell(19).CellType == CellType.Formula)
                                                            {
                                                                strRemarkFromCommission = rowS.GetCell(19).NumericCellValue.ToString();
                                                            }
                                                            else if (rowS.GetCell(19).CellType == CellType.String)
                                                                strRemarkFromCommission = rowS.GetCell(19).StringCellValue.ToString();
                                                            else
                                                                strRemarkFromCommission = "";

                                                            if (rowS.GetCell(16).CellType == CellType.Numeric || rowS.GetCell(16).CellType == CellType.Formula)
                                                            {
                                                                strFirstCommissionDateWeek = rowS.GetCell(16).NumericCellValue.ToString();
                                                            }
                                                            else if (rowS.GetCell(16).CellType == CellType.String)
                                                                strFirstCommissionDateWeek = rowS.GetCell(16).StringCellValue.ToString();
                                                            else
                                                                strFirstCommissionDateWeek = "";

                                                            if (rowS.GetCell(12).CellType == CellType.Numeric || rowS.GetCell(12).CellType == CellType.Formula)
                                                            {
                                                                strFirstShipmentDateWeek = rowS.GetCell(12).NumericCellValue.ToString();
                                                            }
                                                            else if (rowS.GetCell(12).CellType == CellType.String)
                                                                strFirstShipmentDateWeek = rowS.GetCell(12).StringCellValue.ToString();
                                                            else
                                                                strFirstShipmentDateWeek = "";

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
                                            #endregion

                                            dataRow[10] = str_Income.Trim();//Income, 
                                            dataRow[11] = str_Cost.Trim();//Cost,
                                            dataRow[12] = str_RMB.Trim();//RMB,
                                            dataRow[13] = str_Procent.Trim();//%
                                            dataRow[14] = str_Exchdiff.Trim(); //Exchdiff,
                                            dataRow[15] = "";//Empty2
                                            dataRow[16] = str_islower.Trim();//islower,
                                            dataRow[17] = "";//>RMB20000,
                                            dataRow[18] = "";//>RMB50000,
                                            dataRow[19] = "";//>RMB120000,
                                            dataRow[20] = "";//>RMB120001,
                                            dataRow[21] = "";//Empty3,
                                            dataRow[22] = str_X9000.Trim();//(X) - RMB9,000
                                            dataRow[23] = str_30.Trim();//30%,

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
                                                        if(Convert.ToInt32(strFirstShipmentDateWeek)<=52 && Convert.ToInt32(strFirstShipmentDateWeek) >0)
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

                                            dataRow[24] = str_TransType;//Type,
                                            dataRow[25] = str_NO;//No,
                                            dataRow[26] = str_year;//Year,
                                            dataRow[27] = str_sales;//sales

                                            //,DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP
                                            if (str_TransType == "OTHER")
                                            {
                                                DataRow dataRow_Other = otherTable.NewRow();

                                                dataRow_Other[0] = dataRow[0];
                                                dataRow_Other[1] = dataRow[1];
                                                dataRow_Other[2] = dataRow[2];
                                                dataRow_Other[3] = dataRow[3];
                                                dataRow_Other[4] = dataRow[4];
                                                dataRow_Other[5] = dataRow[5];
                                                dataRow_Other[6] = dataRow[6];
                                                dataRow_Other[7] = dataRow[7];
                                                dataRow_Other[8] = dataRow[8];
                                                dataRow_Other[9] = dataRow[9];
                                                dataRow_Other[10] = dataRow[10];
                                                dataRow_Other[11] = dataRow[11];
                                                dataRow_Other[12] = dataRow[12];
                                                dataRow_Other[13] = dataRow[13];
                                                dataRow_Other[14] = dataRow[14];
                                                dataRow_Other[15] = dataRow[15];
                                                dataRow_Other[16] = dataRow[16];
                                                dataRow_Other[17] = dataRow[17];
                                                dataRow_Other[18] = dataRow[18];
                                                dataRow_Other[19] = dataRow[19];
                                                dataRow_Other[20] = dataRow[20];
                                                dataRow_Other[21] = dataRow[21];
                                                dataRow_Other[22] = dataRow[22];
                                                dataRow_Other[23] = dataRow[23];
                                                dataRow_Other[24] = dataRow[24];
                                                dataRow_Other[25] = dataRow[25];
                                                dataRow_Other[26] = dataRow[26];
                                                dataRow_Other[27] = dataRow[27];

                                                otherTable.Rows.Add(dataRow_Other);
                                            }
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


            for (int ta = 0; ta < dsData.Tables.Count; ta++)
            {
                DataTable dtTable = dsData.Tables[ta];

                if(dtTable.TableName!="OTHER")
                {

                    //string strColumn = @"Month,Week,b/l,Profit(RMB),Empty1,Consignee,Shipper,Principal,NoofCtns,LotNo,Income,Cost,Profit/(loss)RMB,%,Exchdiff,Empty2,Whicheverislower,
                    //'(X)-RMB9000,30%,Type,No";

                    int strweek = Convert.ToInt32(dtTable.Rows[0]["Week"].ToString());

                    int strMonth = GetMonthByWeek(strweek);

                    int strYear = Convert.ToInt32(dtTable.Rows[0]["Year"].ToString());

                    string strUserName = dtTable.TableName.ToString();

                    string strRow1 = strUserName + "'s Performance" + 
                        " for commission for " + strMonth + " "+ strYear.ToString();

                    #region  Define the column

                    //string strColumn = @"Month,Week,b/l,Profit(RMB),Empty1,Consignee,Shipper,Principal,NoofCtns,LotNo,Income,
                    //Cost,Profit/(loss)RMB,%,Exchdiff,Empty2,Whicheverislower,>RMB20000,>RMB50000,>RMB120000,>RMB120001,
                    //Empty3,(X)-RMB9000,30%,Type,No,Year,sales";


                    //XXX's Performance for commission for Jun 2022  0-15column
                    DataColumn dc0 = new DataColumn("Month", Type.GetType("System.String"));
                    DataColumn dc1 = new DataColumn("Week", Type.GetType("System.String"));
                    DataColumn dc2 = new DataColumn("b/l", Type.GetType("System.String"));
                    DataColumn dc3 = new DataColumn("Profit(RMB)", Type.GetType("System.String"));
                    DataColumn dc4 = new DataColumn("Empty1", Type.GetType("System.String"));
                    DataColumn dc5 = new DataColumn("Consignee", Type.GetType("System.String"));
                    DataColumn dc6 = new DataColumn("Shipper", Type.GetType("System.String"));
                    DataColumn dc7 = new DataColumn("Principal", Type.GetType("System.String"));
                    DataColumn dc8 = new DataColumn("NoofCtns", Type.GetType("System.String"));
                    DataColumn dc9 = new DataColumn("LotNo", Type.GetType("System.String"));
                    DataColumn dc10 = new DataColumn("Income", Type.GetType("System.String"));
                    DataColumn dc11 = new DataColumn("Cost", Type.GetType("System.String"));
                    DataColumn dc12 = new DataColumn("RMB", Type.GetType("System.String"));
                    DataColumn dc13 = new DataColumn("%", Type.GetType("System.String"));
                    DataColumn dc14 = new DataColumn("Exchdiff", Type.GetType("System.String"));
                    DataColumn dc15 = new DataColumn("Empty2", Type.GetType("System.String"));
                    DataColumn dc16 = new DataColumn("islower", Type.GetType("System.String"));
                    DataColumn dc17 = new DataColumn(">RMB20000", Type.GetType("System.String"));
                    DataColumn dc18 = new DataColumn(">RMB50000", Type.GetType("System.String"));
                    DataColumn dc19 = new DataColumn(">RMB120000", Type.GetType("System.String"));
                    DataColumn dc20 = new DataColumn(">RMB120001", Type.GetType("System.String"));
                    DataColumn dc21 = new DataColumn("Empty3", Type.GetType("System.String"));
                    DataColumn dc22 = new DataColumn("(X)-RMB9000", Type.GetType("System.String"));
                    DataColumn dc23 = new DataColumn("30%", Type.GetType("System.String"));
                    DataColumn dc24 = new DataColumn("Type", Type.GetType("System.String"));
                    DataColumn dc25 = new DataColumn("No", Type.GetType("System.String"));
                    //DataColumn dc26 = new DataColumn("Year", Type.GetType("System.String"));
                    //DataColumn dc27 = new DataColumn("sales", Type.GetType("System.String"));


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
                    dtsales.Columns.Add(dc24);//Y
                    dtsales.Columns.Add(dc25);//Z



                    #endregion


                    DataRow dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = strRow1; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);


                    Double ALLTotalProfit = 0.00;

                    #region Two empty lines
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = "";dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = ""; 
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    

                    DataRow dataNewline = dtsales.NewRow();
                    dataNewline[0] = "From Sales department"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = ""; dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = "Accounts Department"; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "Commission";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = ""; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = ""; dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = "SHG"; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = "Y"; dataNewline[17] = "";
                    dataNewline[18] = "20000"; dataNewline[19] = "50000"; dataNewline[20] = "120000"; dataNewline[21] = "200000"; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = ""; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = ""; dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = strMonth + "/" + strYear.ToString(); dataNewline[11] = "3000";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = "N"; dataNewline[17] = "";
                    dataNewline[18] = "15%"; dataNewline[19] = "20%"; dataNewline[20] = "25%"; dataNewline[21] = "30%"; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = ""; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = ""; dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = "Profit/(loss)"; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = "Whichever"; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);
                    #endregion

                    dataNewline = dtsales.NewRow();
                    for (int m = 0; m < dtsales.Columns.Count; m++)
                    {
                        dataNewline[m] = dtsales.Columns[m].ColumnName.Trim();
                    }
                    dtsales.Rows.Add(dataNewline);

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = ""; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = "X"; dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] ="a"; dataNewline[11] = "b";
                    dataNewline[12] = "a-b=c"; dataNewline[13] = "on sales"; dataNewline[14] = "d-x : RMB"; dataNewline[15] = "RMB (X)"; dataNewline[16] = "N/Y"; dataNewline[17] = "N/Y";
                    dataNewline[18] = "N/Y"; dataNewline[19] = "N/Y"; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = "'(Z)"; dataNewline[23] = "RMB";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);

                    double TotalProfit = 0.00;

                    for (int n =0;n< dtTable.Rows.Count;n++)
                    {
                        DataRow dr = dtTable.Rows[n];

                        if (dr["Type"].ToString() == "NORMAL")
                        {
                            dataNewline = dtsales.NewRow();
                            for (int o = 0; o < 10; o++)
                            {
                                dataNewline[o] = dr[o];
                            }
                            dtsales.Rows.Add(dataNewline);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }
                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    #region 
                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);


                    #region Sales lead begin 

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "SALE LEAD - OVER ONE YEAR (fm 2nd yr to 5th yr) - 7%"; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    

                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtTable.Rows[n];

                        if (dr["Type"].ToString() == "SALES LEAD")
                        {
                            dataNewline = dtsales.NewRow();
                            for (int o = 0; o < 10; o++)
                            {
                                dataNewline[o] = dr[o];
                            }
                            dtsales.Rows.Add(dataNewline);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }
                    ALLTotalProfit = ALLTotalProfit + TotalProfit;
                   
                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);
                    #endregion


                    #region LOST ACCOUNT
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "LOST ACCOUNT - 10%"; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    #endregion

                    //CREDIT OVER 30 DAYS - 10%
                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtTable.Rows[n];

                        if (dr["Type"].ToString() == "LOST ACCOUNT")
                        {
                            dataNewline = dtsales.NewRow();
                            for (int o = 0; o < 10; o++)
                            {
                                dataNewline[o] = dr[o];
                            }
                            dtsales.Rows.Add(dataNewline);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }

                    ALLTotalProfit = ALLTotalProfit + TotalProfit;
                   

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);

                    #endregion

                    #region
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "CREDIT OVER 30 DAYS - 10%"; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                   

                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtTable.Rows[n];

                        if (dr["Type"].ToString() == "CREDIT OVER")
                        {
                            dataNewline = dtsales.NewRow();
                            for (int o = 0; o < 10; o++)
                            {
                                dataNewline[o] = dr[o];
                            }
                            dtsales.Rows.Add(dataNewline);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }

                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);
                    #endregion

                    #region
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "LATE PAYMENT FROM CLIENT - 10"; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);


                    TotalProfit = 0.00;

                    for (int n = 0; n < dtTable.Rows.Count; n++)
                    {
                        DataRow dr = dtTable.Rows[n];

                        if (dr["Type"].ToString() == "LATE PAYMENT")
                        {
                            dataNewline = dtsales.NewRow();
                            for (int o = 0; o < 10; o++)
                            {
                                dataNewline[o] = dr[o];
                            }
                            dtsales.Rows.Add(dataNewline);

                            TotalProfit = TotalProfit + Convert.ToDouble(dr[3]);
                        }
                    }

                    ALLTotalProfit = ALLTotalProfit + TotalProfit;

                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "S.TTL"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = TotalProfit.ToString(); dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    #endregion


                    #region
                    dataNewline = dtsales.NewRow();
                    dataNewline[0] = "Total"; dataNewline[1] = ""; dataNewline[2] = ""; dataNewline[3] = ALLTotalProfit.ToString(); dataNewline[4] = ""; dataNewline[5] = "";
                    dataNewline[6] = ""; dataNewline[7] = ""; dataNewline[8] = ""; dataNewline[9] = ""; dataNewline[10] = ""; dataNewline[11] = "";
                    dataNewline[12] = ""; dataNewline[13] = ""; dataNewline[14] = ""; dataNewline[15] = ""; dataNewline[16] = ""; dataNewline[17] = "";
                    dataNewline[18] = ""; dataNewline[19] = ""; dataNewline[20] = ""; dataNewline[21] = ""; dataNewline[22] = ""; dataNewline[23] = "";
                    dataNewline[24] = ""; dataNewline[25] = "";
                    dtsales.Rows.Add(dataNewline);


                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "Reviewed by :  "; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = ""; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);

                    dataEmpty = dtsales.NewRow();
                    dataEmpty[0] = "Prepared by:  "; dataEmpty[1] = ""; dataEmpty[2] = ""; dataEmpty[3] = ""; dataEmpty[4] = ""; dataEmpty[5] = "";
                    dataEmpty[6] = ""; dataEmpty[7] = ""; dataEmpty[8] = ""; dataEmpty[9] = ""; dataEmpty[10] = ""; dataEmpty[11] = "";
                    dataEmpty[12] = ""; dataEmpty[13] = ""; dataEmpty[14] = ""; dataEmpty[15] = ""; dataEmpty[16] = ""; dataEmpty[17] = "";
                    dataEmpty[18] = ""; dataEmpty[19] = ""; dataEmpty[20] = ""; dataEmpty[21] = ""; dataEmpty[22] = ""; dataEmpty[23] = "";
                    dataEmpty[24] = ""; dataEmpty[25] = "";
                    dtsales.Rows.Add(dataEmpty);
                    #endregion

                    DataTable dtnew = new DataTable();
                    dtnew = dtsales.Copy();
                    dtnew.TableName = dtTable.TableName.ToString();
                    ds.Tables.Add(dtnew);

                }
                else
                {
                    DataTable dtnew = new DataTable();
                    dtnew = dsData.Tables[ta].Copy();
                    dtnew.TableName ="Other's Sales List";
                    ds.Tables.Add(dtnew);
                }
            }

            ExportDataToExcel2(ds, "Sales Commission Report" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond.ToString());

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

        public static int GetMonthByWeek(int IntWeek)
        {

            int IntMonth = 1;

            if(IntWeek>0 && IntWeek<5) //1-4
            {
                IntMonth = 1;
            }
            else if (IntWeek > 5 && IntWeek < 11) //6-10
            {
                IntMonth = 2;
            }
            else if (IntWeek > 10 && IntWeek < 15) //11-14
            {
                IntMonth = 3;
            }
            else if (IntWeek > 14 && IntWeek < 20) //15-19
            {
                IntMonth = 4;
            }
            else if (IntWeek > 19 && IntWeek < 24) //20-23
            {
                IntMonth = 5;
            }
            else if (IntWeek > 23 && IntWeek < 28) //24-27
            {
                IntMonth = 6;
            }
            else if (IntWeek > 27 && IntWeek < 33) //28-32
            {
                IntMonth = 7;
            }
            else if (IntWeek > 32 && IntWeek < 37) //33-36
            {
                IntMonth = 8;
            }
            else if (IntWeek > 36 && IntWeek < 42) //37-40
            {
                IntMonth = 9;
            }
            else if (IntWeek > 40 && IntWeek < 46) //41-45
            {
                IntMonth = 10;
            }
            else if (IntWeek > 45 && IntWeek < 50) //46-49
            {
                IntMonth = 11;
            }
            else if (IntWeek > 49 && IntWeek < 54) //50-53
            {
                IntMonth = 12;
            }

            return IntMonth;


        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

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
                    font.FontName = "Times New Roman";
                    font.Color = NPOI.HSSF.Util.HSSFColor.Blue.Index;
                    style_header.SetFont(font);//HEAD 样式 
                    style_header.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_header.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_header.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_header.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_header.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_header.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;


                    ICellStyle style_row = workbook.CreateCellStyle();
                    IFont font1 = workbook.CreateFont();
                    font1.FontHeightInPoints = 11;
                    font1.FontName = "Times New Roman";
                    style_row.SetFont(font1);//row 样式 
                    style_row.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;


                    ICellStyle style_row_yellow = workbook.CreateCellStyle();
                    font1 = workbook.CreateFont();
                    font1.FontHeightInPoints = 11;
                    font1.FontName = "Times New Roman";
                    style_row_yellow.SetFont(font1);//row 样式 
                    style_row_yellow.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row_yellow.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_yellow.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_yellow.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_yellow.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_yellow.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_yellow.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_yellow.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_yellow.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_yellow.FillForegroundColor = 0;
                    style_row_yellow.FillPattern = FillPattern.SolidForeground;
                    ((XSSFColor)style_row_yellow.FillForegroundColorColor).SetRgb(new byte[] { 255, 255, 0 });


                    ICellStyle style_row_Bold = workbook.CreateCellStyle();
                    IFont fontB = workbook.CreateFont();
                    fontB.FontHeightInPoints = 11;
                    fontB.FontName = "Times New Roman";
                    fontB.IsBold = true;
                    fontB.IsItalic = true;
                    style_row_Bold.SetFont(fontB);//row 样式 
                    style_row_Bold.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    style_row_Bold.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Bold.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Bold.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Bold.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Bold.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_Bold.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_Bold.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_Bold.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

                    ICellStyle style_row_Type = workbook.CreateCellStyle();
                    IFont font_type = workbook.CreateFont();
                    font_type.FontHeightInPoints = 11;
                    font_type.FontName = "Times New Roman";
                    font_type.IsItalic = true;
                    font_type.IsBold = true;
                    style_row_Type.SetFont(font_type);//row 样式 
                    style_row_Type.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    style_row_Type.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Type.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Type.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Type.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    style_row_Type.TopBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_Type.BottomBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_Type.LeftBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;
                    style_row_Type.RightBorderColor = NPOI.HSSF.Util.HSSFColor.Black.Index;

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

                    for (int k = 0; k < rowscount; k++)//k是第几个Sheet
                    {
                        DataTable TableName = SetTable.Tables[k];

                        ISheet sheet = workbook.CreateSheet(SetTable.Tables[k].TableName.ToString());

                        //读取标题  
                        if (sheet.SheetName.Contains("Other's"))
                        {
                            IRow rowHeader = sheet.CreateRow(0);

                            for (int i = 0; i < TableName.Columns.Count; i++)
                            {
                                ICell cell = rowHeader.CreateCell(i);
                                cell.SetCellValue(string.Format(TableName.Columns[i].ColumnName));
                                cell.CellStyle = style_header;

                                sheet.AutoSizeColumn(i);
                            }

                            for (int i = 0; i < TableName.Rows.Count ; i++)
                            {
                                IRow rowData = sheet.CreateRow(i+1);
                                //j列数名称
                                for (int j = 0; j < TableName.Columns.Count; j++)
                                {
                                    ICell cell = rowData.CreateCell(j);

                                    try
                                    {
                                        string strValue = TableName.Rows[i][j].ToString();
                                        cell.SetCellValue(string.Format(strValue));
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //i第几行
                            for (int i = 0; i < TableName.Rows.Count; i++)
                            {
                                IRow rowData = sheet.CreateRow(i);
                                //j列数名称
                                for (int j = 0; j < TableName.Columns.Count; j++)
                                {
                                    ICell cell = rowData.CreateCell(j);

                                    try
                                    {
                                        string strValue = TableName.Rows[i][j].ToString();

                                        if (!strValue.Contains("Column") && strValue.Trim() != "")
                                        {

                                            if (strValue.Contains("Empty"))
                                            {
                                                cell.SetCellValue("");
                                            }
                                            else
                                            {
                                                cell.SetCellValue(string.Format(strValue));
                                            }

                                            if (strValue.Contains("SALE LEAD") || strValue.Contains("LOST ACCOUNT") || strValue.Contains("CREDIT OVER") || strValue.Contains("LATE PAYMENT"))
                                            {
                                                sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 3));
                                            }
                                            else if (strValue.Contains("Reviewed by") || strValue.Contains("Prepared by"))
                                            {
                                                sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 2));
                                            }
                                        }

                                        if ((i == 7 || i == 8) && j < 4)
                                        {
                                            cell.CellStyle = style_row_yellow;
                                        }
                                        else
                                        {
                                            if (strValue.Contains("SALE LEAD") || strValue.Contains("LOST ACCOUNT") || strValue.Contains("CREDIT OVER") || strValue.Contains("LATE PAYMENT"))
                                            {
                                                cell.CellStyle = style_row_Type;
                                            }
                                            else
                                            {
                                                cell.CellStyle = style_row;
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }

                                    if (j == 5 || j == 6 || j == 7) //Consignee  Shipper	Principal
                                    {
                                        sheet.SetColumnWidth(j, 50 * 256);
                                    }
                                    else if (j == 2 || j == 3 || j == 8 || j == 9)
                                    {
                                        sheet.SetColumnWidth(j, 20 * 256);
                                    } else if (j == 4)
                                    {
                                        sheet.SetColumnWidth(j, 5 * 256);
                                    }

                                }

                                if (i == 0)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 3));
                                }
                                else if (i == 3)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 3));
                                    sheet.AddMergedRegion(new CellRangeAddress(i, i, 10, 12));
                                    sheet.AddMergedRegion(new CellRangeAddress(i, i, 17, 23));
                                }


                                Application.DoEvents();
                            }

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
    }
}
