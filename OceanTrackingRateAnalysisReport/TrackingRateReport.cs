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
using System.Threading;

using System.Collections;
using System.Drawing.Drawing2D;

namespace OceanTrackingRateAnalysisReport
{
    public partial class TrackingRateReport : Form
    {

        public TrackingRateReport()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        public bool showDialogGetSaveFolder(out string folderPath,out string safefolderPath)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Files|*.xls;*.xlsx";
            //dlg.Filter = "Files|*.xlsx";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            bool dialogResult = dlg.ShowDialog() == DialogResult.OK;

            folderPath = dlg.FileName;
            safefolderPath = dlg.SafeFileName;

            return dialogResult;
        }

        public static DataTable ExcelToDataSet(string filePath, bool isFirstLineColumnName, string newDate, string oldDate)
        {
            int startRow = 0;
            DataTable dataTable = new DataTable();
            dataTable.TableName = "TOTAL";
            string ErrorSheetName = "";
            string ErrorSheetLine = "0";

            using (FileStream fs = File.OpenRead(filePath))
            {
                IWorkbook workbook = null;
                // 如果是2007+的Excel版本
                if (filePath.IndexOf(".xlsx") > 0)
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (filePath.IndexOf(".xls") > 0) // 如果是2003-的Excel版本
                {
                    workbook = new HSSFWorkbook(fs);
                }

                if (workbook != null)
                {
                    //EASTCOAST: /'ME'/'MH'/'MA'/'RD'/'CT'/'NJ'/'DE'/'MD'/'WA'/'VA'/'NC'/'SC'/'GA'/'FL'/'NY'/
                    string strColumn = @"DATE,KEY,Cargo Pickup Location(City),Cargo Pickup Location(State),Delivery Location,State,Zip Code,[" + newDate.ToString() + "] Rate w/ out fuel,Fuel 45%,Total Base + fuel,[" + oldDate.Trim() + "] Rate w/ out fuel,DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP,REMARKS,CHARGE";

                    string[] strColumn_S = strColumn.Split(',');
                    for (int cc = 0; cc < strColumn_S.Length; cc++)
                    {
                        //构建datatable的列名称
                        DataColumn column = new DataColumn(strColumn_S[cc].Trim());
                        dataTable.Columns.Add(column);
                    }
                    //构建datatable的列

                    //循环读取Excel的每个sheet，每个sheet页都转换为一个DataTable，并放在DataSet中
                    int SheetCount = workbook.NumberOfSheets;
                    if (SheetCount > 3)
                    {
                        for (int p = 0; p < 3; p++)
                        {
                            ISheet sheet = workbook.GetSheetAt(p);

                            ErrorSheetName = sheet.SheetName.ToString();


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
                                        ErrorSheetLine = r.ToString();

                                        if (FindFirstRow != null)
                                        {
                                            if (FindFirstRow.LastCellNum > 6 && FindFirstRow.GetCell(1) != null)
                                            {
                                                if (sheet.SheetName.ToLower().Contains("except") || (sheet.SheetName.ToLower().Contains("chicago") && sheet.SheetName.ToLower().Contains("etd")))
                                                {
                                                    if (FindFirstRow.GetCell(0).ToString().ToLower().Trim() == "date" && FindFirstRow.GetCell(1).ToString().ToLower().Contains("cargo pickup"))
                                                    {
                                                        FindRow = r;//找到数据开始的位置
                                                        break;

                                                    }
                                                }
                                                else if (sheet.SheetName.ToLower().Contains("lax") && sheet.SheetName.ToLower().Contains("etd"))
                                                {
                                                    if (FindFirstRow.GetCell(0).ToString().Trim() == "" && FindFirstRow.GetCell(1).ToString().Contains("Delivery City"))
                                                    {
                                                        FindRow = r;//找到数据开始的位置
                                                        break;

                                                    }
                                                    else if (FindFirstRow.GetCell(0).ToString() == "*" && sheet.GetRow(r + 1).GetCell(0).ToString() == "*")
                                                    {
                                                        FindRow = r - 1;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (FindRow > 0)
                                {
                                    string strDate = "1";
                                    string strLocationCity = "";
                                    string strLocationState = "";
                                    string strDeliveryLocation = "";
                                    string strState = "";
                                    string strZip = "";
                                    string strRateWOfuel = "0";
                                    string strFuel = "0";
                                    string strTotalfuel = "0";
                                    string strDIF = "0";
                                    string strREGION = "";
                                    string strCHANGE = "";
                                    string strQUESTION = "";
                                    string AllRegion = "";
                                    string strRemark = "";
                                    string strCharge = "0";

                                    if (sheet.SheetName.ToLower().Contains("except") || (sheet.SheetName.ToLower().Contains("chicago") && sheet.SheetName.ToLower().Contains("etd")))
                                    {
                                        #region Case 1
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

                                                DataRow dataRow = dataTable.NewRow();

                                                try
                                                {

                                                    if (row.GetCell(0).CellType == CellType.Numeric || row.GetCell(0).CellType == CellType.Formula)
                                                    {
                                                        //strDate = row.GetCell(0).NumericCellValue.ToString();
                                                        strDate = row.GetCell(0).DateCellValue.ToString("yyyy/MM/dd").ToString();
                                                    }
                                                    else if (row.GetCell(0).CellType == CellType.String)
                                                        strDate = row.GetCell(0).StringCellValue.ToString();
                                                    else
                                                        strDate = "1";

                                                    strLocationCity = row.GetCell(1).StringCellValue.Trim();

                                                    try
                                                    {
                                                        if (row.GetCell(2).CellType == CellType.Numeric || row.GetCell(2).CellType == CellType.Formula)
                                                        {
                                                            if (row.GetCell(2).ToString() != "#REF!")
                                                            {
                                                                strLocationState = row.GetCell(2).NumericCellValue.ToString();
                                                            }
                                                            else
                                                            {
                                                                strLocationState = "";
                                                            }
                                                        }
                                                        else if (row.GetCell(2).CellType == CellType.String)
                                                            strLocationState = row.GetCell(2).StringCellValue.ToString();
                                                        else
                                                            strLocationState = "";
                                                    }
                                                    catch (Exception err)
                                                    {
                                                        if (err.Message.ToString() == "Cannot get a numeric value from a error formula cell")
                                                        {
                                                            strLocationState = "";
                                                        }
                                                        else
                                                        {
                                                            if (err.Message.ToString() == "Cannot get a numeric value from a text formula cell")
                                                            {
                                                                strLocationState = row.GetCell(2).StringCellValue.ToString();
                                                            }
                                                            else
                                                            {
                                                                var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + err.Message + "Line:" + i;

                                                                MessageBox.Show(msg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                            }
                                                        }
                                                    }

                                                    strDeliveryLocation = row.GetCell(3).StringCellValue.Trim();

                                                    if (row.GetCell(4).CellType == CellType.Numeric || row.GetCell(4).CellType == CellType.Formula)
                                                        strState = row.GetCell(4).NumericCellValue.ToString();
                                                    else if (row.GetCell(4).CellType == CellType.String)
                                                        strState = row.GetCell(4).StringCellValue.ToString();
                                                    else
                                                        strState = "";

                                                    if (row.GetCell(5).CellType == CellType.Numeric || row.GetCell(5).CellType == CellType.Formula)
                                                        strZip = row.GetCell(5).NumericCellValue.ToString();
                                                    else if (row.GetCell(5).CellType == CellType.String)
                                                        strZip = row.GetCell(5).StringCellValue.ToString();
                                                    else
                                                        strZip = "";

                                                    if (row.GetCell(6).CellType == CellType.Numeric || row.GetCell(6).CellType == CellType.Formula)
                                                        strRateWOfuel = row.GetCell(6).NumericCellValue.ToString();
                                                    else if (row.GetCell(6).CellType == CellType.String)
                                                        strRateWOfuel = row.GetCell(6).StringCellValue.ToString();
                                                    else
                                                        strRateWOfuel = "0";

                                                    if (row.GetCell(7).CellType == CellType.Numeric || row.GetCell(7).CellType == CellType.Formula)
                                                        strFuel = row.GetCell(7).NumericCellValue.ToString();
                                                    else if (row.GetCell(7).CellType == CellType.String)
                                                        strFuel = row.GetCell(7).StringCellValue.ToString();
                                                    else
                                                        strFuel = "0";

                                                    if (row.GetCell(8).CellType == CellType.Numeric || row.GetCell(8).CellType == CellType.Formula)
                                                        strTotalfuel = row.GetCell(8).NumericCellValue.ToString();
                                                    else if (row.GetCell(8).CellType == CellType.String)
                                                        strTotalfuel = row.GetCell(8).StringCellValue.ToString();
                                                    else
                                                        strTotalfuel = "0";


                                                    strDIF = "0";
                                                    strREGION = "";  //REGION : OTHERS/LAX/CHICAGO/EASTCOAST
                                                    strCHANGE = "";  //CHANGE : NOCHANGE/DECREASE/INCREASE/NEWLYADDED
                                                    strQUESTION = "";
                                                    AllRegion = "/'ME'/'MH'/'MA'/'RD'/'CT'/'NJ'/'DE'/'MD'/'WA'/'VA'/'NC'/'SC'/'GA'/'FL'/'NY'/";

                                                    try
                                                    {
                                                        if (row.GetCell(12).CellType == CellType.Numeric || row.GetCell(12).CellType == CellType.Formula)
                                                            strRemark = row.GetCell(12).NumericCellValue.ToString();
                                                        else if (row.GetCell(12).CellType == CellType.String)
                                                            strRemark = row.GetCell(12).StringCellValue.ToString();
                                                        else
                                                            strRemark = "";

                                                    }
                                                    catch (Exception err)
                                                    {
                                                        if (row.GetCell(12).CellFormula.ToString() == "#REF!")
                                                        {
                                                            strRemark = "";
                                                        }
                                                        else
                                                        {
                                                            if (row.GetCell(12).StringCellValue.ToString() != "")
                                                            {
                                                                strRemark = row.GetCell(12).StringCellValue.ToString();
                                                            }
                                                            else
                                                            {
                                                                var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + err.Message + "Line:" + i;

                                                                MessageBox.Show(msg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                            }
                                                        }
                                                    }
                                                    strCharge = strRateWOfuel.ToString();
                                                }
                                                catch (Exception ex)
                                                {
                                                    var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + ex.Message + "Line:" + i;
                                                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    return null;
                                                }

                                                if (sheet.SheetName.ToLower().Contains("except"))
                                                {
                                                    strREGION = "OTHERS";

                                                    if (AllRegion.Contains("/'" + strState.Trim() + "'/"))
                                                    {
                                                        strREGION = "EASTCOAST";
                                                    }
                                                }
                                                else if (sheet.SheetName.ToLower().Contains("chicago"))
                                                {
                                                    strREGION = "CHICAGO";
                                                }


                                                dataRow[0] = strDate;//Date
                                                string key = strLocationCity.Trim() + strLocationState.Trim() + strDeliveryLocation.Trim() + strState.Trim() + strZip.Trim() + strRemark.Trim();
                                                dataRow[1] = key.Trim();
                                                dataRow[2] = strLocationCity.Trim();//Cargo Pickup Location(City)
                                                dataRow[3] = strLocationState.Trim();//Cargo Pickup Location(State)
                                                dataRow[4] = strDeliveryLocation.Trim();//Delivery Location
                                                dataRow[5] = strState.Trim();      //State
                                                dataRow[6] = strZip.Trim();        //Zip
                                                dataRow[7] = Convert.ToDecimal(strRateWOfuel).ToString("0.00"); //New
                                                dataRow[8] = Convert.ToDecimal(strFuel).ToString("0.00");       //New
                                                dataRow[9] = Convert.ToDecimal(Convert.ToDecimal(strRateWOfuel) + (Convert.ToDecimal(strRateWOfuel) * Convert.ToDecimal(strFuel))).ToString("0.00");  //New
                                                dataRow[10] = strRateWOfuel;//Old
                                                dataRow[11] = Convert.ToDecimal(strDIF).ToString("0.00");       // New - Old
                                                dataRow[12] = strREGION.Trim();
                                                dataRow[13] = strCHANGE.Trim();
                                                dataRow[14] = strQUESTION.Trim();
                                                dataRow[15] = strDeliveryLocation.Trim() + strState.Trim();
                                                dataRow[16] = strLocationCity.Trim() + strLocationState.Trim();
                                                dataRow[17] = strRemark;
                                                dataRow[18] = strCharge;


                                                //,DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP

                                                dataTable.Rows.Add(dataRow);
                                            }

                                        }
                                        #endregion

                                    }
                                    else if (sheet.SheetName.ToLower().Contains("lax") && sheet.SheetName.ToLower().Contains("etd"))
                                    {
                                        #region Case 2
                                        IRow firstRow = sheet.GetRow(FindRow);//获取第r行
                                        int cellCount = firstRow.LastCellNum;//获取总列数

                                        strDeliveryLocation = "";
                                        strState = "";
                                        strRemark = "";
                                        strCharge = "";
                                        //string strReport = @"DATE    ,Cargo Pickup Location(City),Cargo Pickup Location(State),Delivery Location,State,Zip Code       Rate w/ out fuel,Fuel 45%,Total Base + fuel,Subject to Layover Fee:，Driver wait time one ( 1) or ( 2) two free hours.，Free  Drop，REMARKS，Validity,(Subject to change without earlier notice)"; ";
                                        string strLaxColumn = @"Delivery City**Delivery State**40'HC APL, CMA, COSCO, Evergreen, Hamburg Sud, Hapag-Lloyd, Hyundai, Maersk, ONE, OOCL)**non-preferred 40'/HC, 20, 45' and specialized container**Mandatory  Layover Fee including fuel.**";
                                        //EASTCOAST: /'ME'/'MH'/'MA'/'RD'/'CT'/'NJ'/'DE'/'MD'/'WA'/'VA'/'NC'/'SC'/'GA'/'FL'/'NY'/

                                        startRow = FindRow + 1;

                                        for (int i = startRow; i <= rowCount; i++)
                                        {
                                            IRow row = sheet.GetRow(i);

                                            if (row == null) continue;
                                            if (row.GetCell(0) == null) continue;
                                            if (row.GetCell(1) == null) continue;
                                            if (row.GetCell(0).ToString() == "" && row.GetCell(2).ToString() == "") continue;


                                            if (strColumn_S.Length > 15 && cellCount > 5)
                                            {

                                                if (row.GetCell(1) == null) continue;
                                                DataRow dataRow = dataTable.NewRow(); //新Table的一行

                                                try
                                                {
                                                    strLocationCity = row.GetCell(1).StringCellValue.ToString();

                                                    try
                                                    {
                                                        if (row.GetCell(2).CellType == CellType.Numeric || row.GetCell(2).CellType == CellType.Formula)
                                                            strLocationState = row.GetCell(2).NumericCellValue.ToString();
                                                        else if (row.GetCell(2).CellType == CellType.String)
                                                            strLocationState = row.GetCell(2).StringCellValue.ToString();
                                                        else
                                                            strLocationState = "";
                                                    }
                                                    catch (Exception err)
                                                    {
                                                        if (err.Message.ToString() == "Cannot get a numeric value from a error formula cell")
                                                        {
                                                            strLocationState = "";
                                                        }
                                                    }

                                                    strZip = "";
                                                    strDeliveryLocation = "";
                                                    strState = "";

                                                    if (row.GetCell(3).CellType == CellType.Numeric || row.GetCell(3).CellType == CellType.Numeric)
                                                        strRateWOfuel = row.GetCell(3).NumericCellValue.ToString();
                                                    else if (row.GetCell(3).CellType == CellType.String)
                                                        strRateWOfuel = row.GetCell(3).StringCellValue.ToString();

                                                    try
                                                    {
                                                        if (row.GetCell(6).CellType == CellType.Numeric || row.GetCell(6).CellType == CellType.Formula)
                                                            strRemark = row.GetCell(6).NumericCellValue.ToString();
                                                        else if (row.GetCell(6).CellType == CellType.String)
                                                            strRemark = row.GetCell(6).StringCellValue.ToString();
                                                        else
                                                            strRemark = "";

                                                    }
                                                    catch (Exception err)
                                                    {
                                                        if (row.GetCell(6).CellFormula.ToString() == "#REF!")
                                                        {
                                                            strRemark = "";
                                                        }
                                                        else
                                                        {
                                                            if (row.GetCell(6).StringCellValue.ToString() != "")
                                                            {
                                                                strRemark = row.GetCell(6).StringCellValue.ToString();
                                                            }
                                                            else
                                                            {
                                                                var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + err.Message + "Line:" + i;
                                                                MessageBox.Show(msg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                            }
                                                        }
                                                    }

                                                    strCharge = strRateWOfuel.ToString();


                                                    dataRow[0] = strDate;//Date
                                                    string key = strLocationCity.Trim() + strLocationState.Trim() + strDeliveryLocation.Trim() + strState.Trim() + strZip.Trim() + strRemark.Trim();
                                                    dataRow[1] = key;                                       //KEY;
                                                    dataRow[2] = strLocationCity.Trim();                   //Cargo Pickup Location(City)
                                                    dataRow[3] = strLocationState.Trim();                  //Cargo Pickup Location(State)
                                                    dataRow[4] = strLocationCity.Trim();                   //Delivery Location
                                                    dataRow[5] = strLocationState.Trim();                  //State
                                                    dataRow[6] = strZip.Trim();                            //Zip
                                                    dataRow[7] = Convert.ToDecimal(strRateWOfuel).ToString("0.00"); //New
                                                    dataRow[8] = Convert.ToDecimal(strFuel).ToString("0.00");       //New
                                                    dataRow[9] = Convert.ToDecimal(Convert.ToDecimal(strRateWOfuel) + (Convert.ToDecimal(strRateWOfuel) * Convert.ToDecimal(strFuel))).ToString("0.00"); //New
                                                    dataRow[10] = Convert.ToDecimal(strRateWOfuel).ToString("0.00");//Old
                                                    dataRow[11] = Convert.ToDecimal(strDIF).ToString("0.00");       // New - Old
                                                    dataRow[12] = "LAX";
                                                    dataRow[13] = strCHANGE.Trim();
                                                    dataRow[14] = strQUESTION.Trim();
                                                    dataRow[15] = strLocationCity.Trim() + strLocationState.Trim();
                                                    dataRow[16] = strLocationCity.Trim() + strLocationState.Trim();
                                                    dataRow[17] = strRemark;
                                                    dataRow[18] = strCharge;
                                                    //,DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP

                                                    dataTable.Rows.Add(dataRow);
                                                }
                                                catch (Exception ex)
                                                {
                                                    var msg = "Sheet Name:" + sheet.SheetName + " exist error when reading data. Error message: " + ex.Message + "Line:" + i;
                                                    MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    return null;
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                }

                            }
                        }
                    }

                }
            }

            return dataTable;

        }

        private void btnSelectNew_Click(object sender, EventArgs e)
        {
            string folderPath = string.Empty;
            string safefolderPath = string.Empty;

            try
            {
                if (showDialogGetSaveFolder(out folderPath, out safefolderPath))
                {
                    this.txtFilePath1.Text = folderPath;
                    this.txtFilePathNewName.Text = safefolderPath;
                    string newDate = safefolderPath.Replace("Topocean Trucking Rate Effective on ETD", "").Replace("FINAL", "").Replace(".xls", "");
                    this.lblNewDate.Text = newDate.Trim();

                    //Topocean Trucking Rate Effective on ETD 06 15 2022 FINAL

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btnSelectOld_Click(object sender, EventArgs e)
        {
            string folderPath = string.Empty;
            string safefolderPath = string.Empty;

            try
            {
                if (showDialogGetSaveFolder(out folderPath, out safefolderPath))
                {
                    this.txtFilePath2.Text = folderPath;
                    this.txtFilePathOldName.Text = safefolderPath;

                    string OldDate = safefolderPath.Replace("Topocean Trucking Rate Effective on ETD", "").Replace("FINAL", "").Replace(".xls", "");
                    this.lblOldDate.Text = OldDate.Trim();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btnAnalysis_Click(object sender, EventArgs e)
        {
            DateTime newdate = new DateTime();
            DateTime olddate = new DateTime();

            try
            {
                newdate = Convert.ToDateTime(this.lblNewDate.Text.Trim());
                olddate = Convert.ToDateTime(this.lblOldDate.Text.Trim());
            }
            catch (Exception error)
            {
                var msg = "Please help to check the Excel file's name, should be like : Topocean Trucking Rate Effective on ETD MM dd yyyy FINAL.xls , " + error.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            #region Check

            if (newdate <= olddate)
            {
                MessageBox.Show("Error Msg:The new date must be later than the old date./r/nPlease check the file's name./r/n And then re-select the file again.");
            }
            #endregion

            string Path1 = this.txtFilePath1.Text.ToString();
            string Path2 = this.txtFilePath2.Text.ToString();

            DataTable TableNew = ExcelToDataSet(Path1, true, newdate.ToString("MMdd"), olddate.ToString("MMdd"));
            DataTable TableOld = ExcelToDataSet(Path2, true, newdate.ToString("MMdd"), olddate.ToString("MMdd"));

            #region 数据处理去重复 //REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP,REMARKS,CHARGE
            if (TableNew != null && TableOld != null)
            {
                //string strColumn = @"DATE,KEY,Cargo Pickup Location(City),Cargo Pickup Location(State),Delivery Location,State,Zip Code,[" + newDate.ToString() + "] Rate w/ out fuel,Fuel 45%,Total Base + fuel,[" + oldDate.Trim() + "],DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP,REMARKS,CHARGE";

                //int TotalNew = TableNew.Rows.Count;
                //int TotalOld = TableOld.Rows.Count;

                DataView dv1 = new DataView(TableNew);
                DataTable dt1 = dv1.ToTable(true, new string[] { "DATE", "KEY", "Cargo Pickup Location(City)", "Cargo Pickup Location(State)", "Delivery Location", "State", "Zip Code", "[" + newdate.ToString("MMdd") + "] Rate w/ out fuel", "Fuel 45%", "Total Base + fuel", "[" + olddate.ToString("MMdd") + "] Rate w/ out fuel", "DIF", "REGION", "CHANGE", "QUESTION", "DELIVERY TO", "PICKUP", "REMARKS", "CHARGE" });

                DataView dv2 = new DataView(TableOld);
                DataTable dt2 = dv2.ToTable(true, new string[] { "DATE", "KEY", "Cargo Pickup Location(City)", "Cargo Pickup Location(State)", "Delivery Location", "State", "Zip Code", "[" + newdate.ToString("MMdd") + "] Rate w/ out fuel", "Fuel 45%", "Total Base + fuel", "[" + olddate.ToString("MMdd") + "] Rate w/ out fuel", "DIF", "REGION", "CHANGE", "QUESTION", "DELIVERY TO", "PICKUP", "REMARKS", "CHARGE" });

                //int TotalNew_DE = dt1.Rows.Count;
                //int TotalOld_DE = dt2.Rows.Count;
                #endregion

                AnalysisData(dt1, dt2, newdate.ToString("MMdd"), olddate.ToString("MMdd"));
            }

        }

        private void AnalysisData(DataTable Tablenew,DataTable Tableold , string newDate, string oldDate)
        {

            #region Parameter 
            DataTable TotalTable = new DataTable();
            DataTable dtSUMMARY = new DataTable();
            DataTable dtNOCHANGE = new DataTable();
            DataTable dtDECREASE = new DataTable();
            DataTable dtINCREASE = new DataTable();
            DataTable dtNEWLYADDED = new DataTable();
            DataTable dtREMOVE = new DataTable();



            string strColumn = @"DATE,KEY,Cargo Pickup Location(City),Cargo Pickup Location(State),Delivery Location,State,Zip Code,[" + newDate.ToString() + "] Rate w/ out fuel,Fuel 45%,Total Base + fuel,[" + oldDate.Trim() + "] Rate w/ out fuel,DIF,REGION,CHANGE,QUESTION,DELIVERY TO,PICKUP, REMARKS, CHARGE";

            double dtNoChange_LAX = 0;
            double dtNoChange_CHICAGO = 0;
            double dtNoChange_EASTCOAST = 0;
            double dtNoChange_OTHERS = 0;

            double dtDECREASE_LAX = 0;
            double dtDECREASE_CHICAGO = 0;
            double dtDECREASE_EASTCOAST = 0;
            double dtDECREASE_OTHERS = 0;

            double dtINCREASE_LAX = 0;
            double dtINCREASE_CHICAGO = 0;
            double dtINCREASE_EASTCOAST = 0;
            double dtINCREASE_OTHERS = 0;

            double dtNEWLYADDED_LAX = 0;
            double dtNEWLYADDED_CHICAGO = 0;
            double dtNEWLYADDED_EASTCOAST = 0;
            double dtNEWLYADDED_OTHERS = 0;

            double dtREMOVE_LAX = 0;
            double dtREMOVE_CHICAGO = 0;
            double dtREMOVE_EASTCOAST = 0;
            double dtREMOVE_OTHERS = 0;

            double Total_Dif_NoChange_LAX = 0;
            double Total_Dif_NoChange_CHICAGO = 0;
            double Total_Dif_NoChange_EASTCOAST = 0;
            double Total_Dif_NoChange_OTHERS = 0;

            double Total_Dif_DECREASE_LAX = 0;
            double Total_Dif_DECREASE_CHICAGO = 0;
            double Total_Dif_DECREASE_EASTCOAST = 0;
            double Total_Dif_DECREASE_OTHERS = 0;

            double Total_Dif_NEWLYADDED_LAX = 0;
            double Total_Dif_NEWLYADDED_CHICAGO = 0;
            double Total_Dif_NEWLYADDED_EASTCOAST = 0;
            double Total_Dif_NEWLYADDED_OTHERS = 0;

            double Total_Dif_INCREASE_LAX = 0;
            double Total_Dif_INCREASE_CHICAGO = 0;
            double Total_Dif_INCREASE_EASTCOAST = 0;
            double Total_Dif_INCREASE_OTHERS = 0;
            
            double Total_Dif_REMOVE_LAX = 0;
            double Total_Dif_REMOVE_CHICAGO = 0;
            double Total_Dif_REMOVE_EASTCOAST = 0;
            double Total_Dif_REMOVE_OTHERS = 0;

            dtSUMMARY.TableName = "TOTAL";
            dtSUMMARY.Columns.Add("");
            dtSUMMARY.Columns.Add("");
            dtSUMMARY.Columns.Add("");
            dtSUMMARY.Columns.Add("");
            dtSUMMARY.Columns.Add("");
            dtSUMMARY.Columns.Add("");

            string[] strColumn_S = strColumn.Split(',');
            DataColumn column1 = new DataColumn();
            DataColumn column2 = new DataColumn();
            DataColumn column3 = new DataColumn();
            DataColumn column4 = new DataColumn();
            DataColumn column5 = new DataColumn();
            DataColumn columnTotal = new DataColumn();

            #endregion

            #region Setting each table data except Summary

            try
            {
                for (int cc = 0; cc < strColumn_S.Length; cc++)
                {
                    //构建datatable的列名称
                    //dtSUMMARY.Columns.Add(column);
                    //NO CHANGE/ DECREASE / INCREASE / NEWLY ADDED / REMOVE

                    column1 = new DataColumn(strColumn_S[cc].Trim());
                    dtNOCHANGE.Columns.Add(column1);
                    column2 = new DataColumn(strColumn_S[cc].Trim());
                    dtDECREASE.Columns.Add(column2);
                    column3 = new DataColumn(strColumn_S[cc].Trim());
                    dtINCREASE.Columns.Add(column3);
                    column4 = new DataColumn(strColumn_S[cc].Trim());
                    dtNEWLYADDED.Columns.Add(column4);
                    column5 = new DataColumn(strColumn_S[cc].Trim());
                    dtREMOVE.Columns.Add(column5);
                    columnTotal = new DataColumn(strColumn_S[cc].Trim());
                    TotalTable.Columns.Add(columnTotal);
                }

                //创建标准表格
                //DATE KEY "Cargo Pickup Location(City)"	"Cargo PickupLocation(State)"	Delivery Location	State	Zip Code	"yyyy / MM / dd(2022 / 06 / 15)
                //Rate w/ out fuel"	Fuel 45%	Total Base  + fuel 	"yyyy / MM / dd(2022 / 06 / 01)"	DIF	REGION	CHANGE	QUESTION	DELIVERY TO	PICKUP	

                //Table Data: NO CHANGE/DECREASE/INCREASE/NEWLY ADDED
                for (int r1 = 0; r1 < Tablenew.Rows.Count; r1++)
                {
                    DataRow dataRow = TotalTable.NewRow();    //创建一笔空行记录
                    DataRow TableNewRow = Tablenew.Rows[r1];
                    string New_Key = TableNewRow[1].ToString();
                    bool findit = false;
                    string strChange = "";

                    dataRow[0] = TableNewRow[0];//Date
                    dataRow[1] = New_Key;       //KEY
                    dataRow[2] = TableNewRow[2];//Cargo Pickup Location(City)
                    dataRow[3] = TableNewRow[3];//Cargo Pickup Location(State)
                    dataRow[4] = TableNewRow[4];//Delivery Location
                    dataRow[5] = TableNewRow[5];//State 
                    dataRow[6] = TableNewRow[6];//Zip Code

                    for (int r2 = 0; r2 < Tableold.Rows.Count; r2++)
                    {
                        DataRow TableOldRow = Tableold.Rows[r2];
                        string Old_Key = TableOldRow[1].ToString();
                       
                        if (New_Key == Old_Key)
                        {
                            dataRow[7] = TableNewRow[7];            //"yyyy / MM / dd(2022 / 06 / 15)
                            dataRow[8] = TableNewRow[8];            //Fuel 45 %
                            dataRow[9] = TableNewRow[9];            //Total Base + fuel
                            dataRow[10] = TableOldRow[7];           //"yyyy / MM / dd(2022 / 06 / 01)"
                            double ChangeData = Convert.ToDouble(TableNewRow[7]) - Convert.ToDouble(TableOldRow[7]);// DIF(New - Old)
                            dataRow[11] = ChangeData.ToString();    //DIF
                            dataRow[12] = TableNewRow[12].ToString().Trim();          //REGION: CHICAGO LAX EASTCOAST OTHERS TOTAL

                        
                            if (ChangeData > 0)
                            {
                                strChange = "INCREASE";
                            }
                            else if (ChangeData < 0)
                            {
                                strChange = "DECREASE";
                            }
                            else if (ChangeData == 0)
                            {
                                strChange = "NO CHANGE";
                            }
                            else
                            {
                                strChange = "";
                            }

                            dataRow[13] = strChange;//CHANGE ：NO CHANGE/DECREASE/INCREASE/NEWLY ADDED/REMOVE

                            findit = true;

                            break;
                        }
                    }

                    if (!findit)
                    {

                        dataRow[7] = TableNewRow[7];    //[0615] Rate w/ out fuel
                        dataRow[8] = TableNewRow[8];    //Fuel 45 %
                        dataRow[9] = TableNewRow[9];    //Total Base + fuel

                        dataRow[10] = "0";              //"yyyy / MM / dd(2022 / 06 / 01)"
                        dataRow[11] = "0";              //DIF(New - Old)
                        dataRow[12] = TableNewRow[12];   //REGION: CHICAGO LAX EASTCOAST OTHERS TOTAL
                        dataRow[13] = "NEWLY ADDED";    //CHANGE
                    }

                    dataRow[14] = TableNewRow[14];//QUESTION
                    dataRow[15] = TableNewRow[15];//DeliveryLocation + State
                    dataRow[16] = TableNewRow[16];//LocationCity + strLocationState

                    dataRow[17] = TableNewRow[17];
                    dataRow[18] = TableNewRow[18];

                    TotalTable.Rows.Add(dataRow);
                }

                //Table Data: REMOVE
                
                for (int r2 = 0; r2 < Tableold.Rows.Count; r2++)
                {
                    bool findOld = false;

                    DataRow TableOldRow = Tableold.Rows[r2];
                    string Old_Key = TableOldRow[1].ToString();

                    for (int r1 = 0; r1 < Tablenew.Rows.Count; r1++)
                    {
                        DataRow TableNewRow = Tablenew.Rows[r1];
                        string New_Key = TableNewRow[1].ToString();

                        if (New_Key == Old_Key)
                        {
                            findOld = true;
                            break;
                        }
                    }

                    if (!findOld)
                    {
                        DataRow dataRow = TotalTable.NewRow();
                        dataRow[0] = TableOldRow[0];//Date
                        dataRow[1] = Old_Key;//Key
                        dataRow[2] = TableOldRow[2];//Cargo Pickup Location(City)
                        dataRow[3] = TableOldRow[3];//Cargo Pickup Location(State)
                        dataRow[4] = TableOldRow[4];//Delivery Location
                        dataRow[5] = TableOldRow[5];//State 
                        dataRow[6] = TableOldRow[6];//Zip
                        dataRow[7] = "0";
                        dataRow[8] = "0";
                        dataRow[9] = "0";
                        dataRow[10] = TableOldRow[10];
                        dataRow[11] = "0";
                        dataRow[12] = TableOldRow[12];//REGION
                        dataRow[13] = "REMOVE";
                        dataRow[14] = TableOldRow[14];
                        dataRow[15] = TableOldRow[15];//DeliveryLocation + State
                        dataRow[16] = TableOldRow[16];//LocationCity + strLocationState

                        dataRow[17] = TableOldRow[17];
                        dataRow[18] = TableOldRow[18];

                        TotalTable.Rows.Add(dataRow);
                    }
                }

                //Calculate all Table :DECREASE/INCREASE/NEWLY ADDED/REMOVE
                for (int k = 0; k < TotalTable.Rows.Count; k++)
                {
                    DataRow TableNewRow = TotalTable.Rows[k];

                    string strType = TableNewRow[13].ToString().ToUpper(); //CHANGE:NO CHANGE/DECREASE/INCREASE/NEWLY ADDED/REMOVE
                    string strRegion = TableNewRow[12].ToString().ToUpper(); //CATEGORY: CHICAGO LAX EASTCOAST OTHERS TOTAL
                    double dbDif = Convert.ToDouble(TableNewRow[11]);

                    switch (strType)
                    {
                        case "NO CHANGE":

                            DataRow dtNOCHANGERow = dtNOCHANGE.NewRow();
                            DataRow TableSourceRow = TotalTable.Rows[k];
                            for (int r = 0; r < 17; r++)
                            {
                                dtNOCHANGERow[r] = TableSourceRow[r];
                            }
                            dtNOCHANGE.Rows.Add(dtNOCHANGERow);

                            switch (strRegion.ToString())
                            {
                                case "LAX":
                                    dtNoChange_LAX = dtNoChange_LAX + 1;
                                    Total_Dif_NoChange_LAX = Total_Dif_NoChange_LAX + dbDif;
                                    break;
                                case "CHICAGO":
                                    dtNoChange_CHICAGO = dtNoChange_CHICAGO + 1;
                                    Total_Dif_NoChange_CHICAGO = Total_Dif_NoChange_CHICAGO + dbDif;
                                    break;
                                case "EASTCOAST":
                                    dtNoChange_EASTCOAST = dtNoChange_EASTCOAST + 1;
                                    Total_Dif_NoChange_EASTCOAST = Total_Dif_NoChange_EASTCOAST + dbDif;
                                    break;
                                case "OTHERS":
                                    dtNoChange_OTHERS = dtNoChange_OTHERS + 1;
                                    Total_Dif_NoChange_OTHERS = Total_Dif_NoChange_OTHERS + dbDif;
                                    break;
                            }
                           break;
                        case "DECREASE":

                            DataRow dtDECREASERow = dtDECREASE.NewRow();
                            DataRow TableSourceRow1 = TotalTable.Rows[k];
                            for (int r = 0; r < 17; r++)
                            {
                                dtDECREASERow[r] = TableSourceRow1[r];
                            }
                            dtDECREASE.Rows.Add(dtDECREASERow);

                            switch (strRegion.ToString())
                            {
                                case "LAX":
                                    dtDECREASE_LAX = dtDECREASE_LAX + 1;
                                    Total_Dif_DECREASE_LAX = Total_Dif_DECREASE_LAX + dbDif;
                                    break;
                                case "CHICAGO":
                                    dtDECREASE_CHICAGO = dtDECREASE_CHICAGO + 1;
                                    Total_Dif_DECREASE_CHICAGO = Total_Dif_DECREASE_CHICAGO + dbDif;
                                    break;
                                case "EASTCOAST":
                                    dtDECREASE_EASTCOAST = dtDECREASE_EASTCOAST + 1;
                                    Total_Dif_DECREASE_EASTCOAST = Total_Dif_DECREASE_EASTCOAST + dbDif;
                                    break;
                                case "OTHERS":
                                    dtDECREASE_OTHERS = dtDECREASE_OTHERS + 1;
                                    Total_Dif_DECREASE_OTHERS = Total_Dif_DECREASE_OTHERS + dbDif;
                                    break;
                            }

                            break;
                        case "INCREASE":

                            DataRow dtINCREASERow = dtINCREASE.NewRow();
                            DataRow TableSourceRow2 = TotalTable.Rows[k];
                            for (int r = 0; r < 17; r++)
                            {
                                dtINCREASERow[r] = TableSourceRow2[r];
                            }
                            dtINCREASE.Rows.Add(dtINCREASERow);

                            switch (strRegion.ToString())
                            {
                                case "LAX":
                                    dtINCREASE_LAX = dtINCREASE_LAX + 1;
                                    Total_Dif_INCREASE_LAX = Total_Dif_INCREASE_LAX + dbDif;
                                    break;
                                case "CHICAGO":
                                    dtINCREASE_CHICAGO = dtINCREASE_CHICAGO + 1;
                                    Total_Dif_INCREASE_CHICAGO = Total_Dif_INCREASE_CHICAGO + dbDif;
                                    break;
                                case "EASTCOAST":
                                    dtINCREASE_EASTCOAST = dtINCREASE_EASTCOAST + 1;
                                    Total_Dif_INCREASE_EASTCOAST = Total_Dif_INCREASE_EASTCOAST + dbDif;
                                    break;
                                case "OTHERS":
                                    dtINCREASE_OTHERS = dtINCREASE_OTHERS + 1;
                                    Total_Dif_INCREASE_OTHERS = Total_Dif_INCREASE_OTHERS + dbDif;
                                    break;
                            }

                            break;
                        case "NEWLY ADDED":

                            DataRow dtNEWLYRow = dtNEWLYADDED.NewRow();
                            DataRow TableSourceRow3 = TotalTable.Rows[k];
                            for (int r = 0; r < 17; r++)
                            {
                                dtNEWLYRow[r] = TableSourceRow3[r];
                            }
                            dtNEWLYADDED.Rows.Add(dtNEWLYRow);
                           
                            dbDif = Convert.ToDouble(TableNewRow[7]);

                            switch (strRegion.ToString())
                            {
                                case "LAX":
                                    dtNEWLYADDED_LAX = dtNEWLYADDED_LAX + 1;
                                    Total_Dif_NEWLYADDED_LAX = Total_Dif_NEWLYADDED_LAX + dbDif;
                                    break;
                                case "CHICAGO":
                                    dtNEWLYADDED_CHICAGO = dtNEWLYADDED_CHICAGO + 1;
                                    Total_Dif_NEWLYADDED_CHICAGO = Total_Dif_NEWLYADDED_CHICAGO + dbDif;
                                    break;
                                case "EASTCOAST":
                                    dtNEWLYADDED_EASTCOAST = dtNEWLYADDED_EASTCOAST + 1;
                                    Total_Dif_NEWLYADDED_EASTCOAST = Total_Dif_NEWLYADDED_EASTCOAST + dbDif;
                                    break;
                                case "OTHERS":
                                    dtNEWLYADDED_OTHERS = dtNEWLYADDED_OTHERS + 1;
                                    Total_Dif_NEWLYADDED_OTHERS = Total_Dif_NEWLYADDED_OTHERS + dbDif;
                                    break;
                            }

                            break;
                        case "REMOVE":

                            DataRow dtREMOVERow = dtREMOVE.NewRow();
                            DataRow TableSourceRow4 = TotalTable.Rows[k];
                            for (int r = 0; r < 17; r++)
                            {
                                dtREMOVERow[r] = TableSourceRow4[r];
                            }
                            dtREMOVE.Rows.Add(dtREMOVERow);

                            dbDif = Convert.ToDouble(TableNewRow[7]);

                            switch (strRegion.ToString())
                            {
                                case "LAX":
                                    dtREMOVE_LAX = dtREMOVE_LAX + 1;
                                    Total_Dif_REMOVE_LAX = Total_Dif_REMOVE_LAX + dbDif;
                                    break;
                                case "CHICAGO":
                                    dtREMOVE_CHICAGO = dtREMOVE_CHICAGO + 1;
                                    Total_Dif_REMOVE_CHICAGO = Total_Dif_REMOVE_CHICAGO + dbDif;
                                    break;
                                case "EASTCOAST":
                                    dtREMOVE_EASTCOAST = dtREMOVE_EASTCOAST + 1;
                                    Total_Dif_REMOVE_EASTCOAST = Total_Dif_REMOVE_EASTCOAST + dbDif;
                                    break;
                                case "OTHERS":
                                    dtREMOVE_OTHERS = dtREMOVE_OTHERS + 1;
                                    Total_Dif_REMOVE_OTHERS = Total_Dif_REMOVE_OTHERS + dbDif;
                                    break;
                            }

                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            #endregion

            #region Setting Summary table data 
            try
            {
                #region SummaryTable

                double TotalDecrease = dtDECREASE_CHICAGO + dtDECREASE_EASTCOAST + dtDECREASE_LAX + dtDECREASE_OTHERS;
                double TotalIncrease = dtINCREASE_CHICAGO + dtINCREASE_EASTCOAST + dtINCREASE_LAX + dtINCREASE_OTHERS;
                double TotalNewly = dtNEWLYADDED_CHICAGO + dtNEWLYADDED_EASTCOAST + dtNEWLYADDED_LAX + dtNEWLYADDED_OTHERS;
                double TotalOnChange = dtNoChange_CHICAGO + dtNoChange_EASTCOAST + dtNoChange_LAX + dtNoChange_OTHERS;
                double TotalRemove = dtREMOVE_CHICAGO + dtREMOVE_EASTCOAST + dtREMOVE_LAX + dtREMOVE_OTHERS;

                double Total_Chicago = dtDECREASE_CHICAGO + dtINCREASE_CHICAGO + dtNEWLYADDED_CHICAGO + dtNoChange_CHICAGO + dtREMOVE_CHICAGO;
                double Total_Lax = dtDECREASE_LAX + dtINCREASE_LAX + dtNEWLYADDED_LAX + dtNoChange_LAX + dtREMOVE_LAX;
                double Total_East = dtDECREASE_EASTCOAST + dtINCREASE_EASTCOAST + dtNEWLYADDED_EASTCOAST + dtNoChange_EASTCOAST + dtREMOVE_EASTCOAST;
                double Total_Other = dtDECREASE_OTHERS + dtINCREASE_OTHERS + dtNEWLYADDED_OTHERS + dtNoChange_OTHERS + dtREMOVE_OTHERS;
                double Total_Total = Total_Chicago + Total_Lax + Total_East + Total_Other;

                #region Sumarry Table 1
                DataColumn dc1 = new DataColumn("", Type.GetType("System.String"));
                DataColumn dc2 = new DataColumn("", Type.GetType("System.String"));
                DataColumn dc3 = new DataColumn("", Type.GetType("System.String"));
                DataColumn dc4 = new DataColumn("", Type.GetType("System.String"));
                DataColumn dc5 = new DataColumn("", Type.GetType("System.String"));
                DataColumn dc6 = new DataColumn("", Type.GetType("System.String"));

                dtSUMMARY = new DataTable();
                dtSUMMARY.Columns.Add(dc1);
                dtSUMMARY.Columns.Add(dc2);
                dtSUMMARY.Columns.Add(dc3);
                dtSUMMARY.Columns.Add(dc4);
                dtSUMMARY.Columns.Add(dc5);
                dtSUMMARY.Columns.Add(dc6);

                DataRow dataSumTitleRow = dtSUMMARY.NewRow();
                DataRow dataSumRow = dtSUMMARY.NewRow();
                DataRow dataSumRow1 = dtSUMMARY.NewRow();
                DataRow dataSumRow2 = dtSUMMARY.NewRow();
                DataRow dataSumRow3 = dtSUMMARY.NewRow();
                DataRow dataSumRow4 = dtSUMMARY.NewRow();
                DataRow dataSumRow5 = dtSUMMARY.NewRow();
                DataRow dataSumRow6 = dtSUMMARY.NewRow();

                dataSumRow[0] = "COUNT"; dataSumRow[1] = ""; dataSumRow[2] = ""; dataSumRow[3] = ""; dataSumRow[4] = ""; dataSumRow[5] = "";
                dataSumTitleRow[0] = "CATEGORY"; dataSumTitleRow[1] = "CHICAGO"; dataSumTitleRow[2] = "LAX"; dataSumTitleRow[3] = "EASTCOAST"; dataSumTitleRow[4] = "OTHERS"; dataSumTitleRow[5] = "TOTAL";
                dataSumRow1[0] = "DECREASE"; dataSumRow1[1] = dtDECREASE_CHICAGO.ToString(); dataSumRow1[2] = dtDECREASE_LAX.ToString(); dataSumRow1[3] = dtDECREASE_EASTCOAST.ToString(); dataSumRow1[4] = dtDECREASE_OTHERS.ToString(); dataSumRow1[5] = TotalDecrease.ToString();
                dataSumRow2[0] = "INCREASE"; dataSumRow2[1] = dtINCREASE_CHICAGO.ToString(); dataSumRow2[2] = dtINCREASE_LAX.ToString(); dataSumRow2[3] = dtINCREASE_EASTCOAST.ToString(); dataSumRow2[4] = dtINCREASE_OTHERS.ToString(); dataSumRow2[5] = TotalIncrease.ToString();
                dataSumRow3[0] = "NEWLY ADDED"; dataSumRow3[1] = dtNEWLYADDED_CHICAGO.ToString(); dataSumRow3[2] = dtNEWLYADDED_LAX.ToString(); dataSumRow3[3] = dtNEWLYADDED_EASTCOAST.ToString(); dataSumRow3[4] = dtNEWLYADDED_OTHERS; dataSumRow3[5] = TotalNewly.ToString();
                dataSumRow4[0] = "NO CHANGE"; dataSumRow4[1] = dtNoChange_CHICAGO.ToString(); dataSumRow4[2] = dtNoChange_LAX.ToString(); dataSumRow4[3] = dtNoChange_EASTCOAST.ToString(); dataSumRow4[4] = dtNoChange_OTHERS; dataSumRow4[5] = TotalOnChange.ToString();
                dataSumRow5[0] = "REMOVE"; dataSumRow5[1] = dtREMOVE_CHICAGO.ToString(); dataSumRow5[2] = dtREMOVE_LAX.ToString(); dataSumRow5[3] = dtREMOVE_EASTCOAST.ToString(); dataSumRow5[4] = dtREMOVE_OTHERS.ToString(); dataSumRow5[5] = TotalRemove.ToString();
                dataSumRow6[0] = "TOTAL"; dataSumRow6[1] = Total_Chicago.ToString(); dataSumRow6[2] = Total_Lax.ToString(); dataSumRow6[3] = Total_East.ToString(); dataSumRow6[4] = Total_Other.ToString(); dataSumRow6[5] = Total_Total.ToString();

                dtSUMMARY.Rows.Add(dataSumRow);
                dtSUMMARY.Rows.Add(dataSumTitleRow);
                dtSUMMARY.Rows.Add(dataSumRow1);
                dtSUMMARY.Rows.Add(dataSumRow2);
                dtSUMMARY.Rows.Add(dataSumRow3);
                dtSUMMARY.Rows.Add(dataSumRow4);
                dtSUMMARY.Rows.Add(dataSumRow5);
                dtSUMMARY.Rows.Add(dataSumRow6);
                #endregion

                #region Two empty lines
                DataRow dataEmpty = dtSUMMARY.NewRow();
                dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                dtSUMMARY.Rows.Add(dataEmpty);
                dataEmpty = dtSUMMARY.NewRow();
                dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " "; dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                dtSUMMARY.Rows.Add(dataEmpty);
                #endregion

                string DECREASE1 = "0%"; string INCREASE1 = "0%"; string NEWLY1 = "0%"; string NOCHANGE1 = "0%"; string REMOVE1 = "0%";
                string DECREASE2 = "0%"; string INCREASE2 = "0%"; string NEWLY2 = "0%"; string NOCHANGE2 = "0%"; string REMOVE2 = "0%";
                string DECREASE3 = "0%"; string INCREASE3 = "0%"; string NEWLY3 = "0%"; string NOCHANGE3 = "0%"; string REMOVE3 = "0%";
                string DECREASE4 = "0%"; string INCREASE4 = "0%"; string NEWLY4 = "0%"; string NOCHANGE4 = "0%"; string REMOVE4 = "0%";
                string DECREASE5 = "0%"; string INCREASE5 = "0%"; string NEWLY5 = "0%"; string NOCHANGE5 = "0%"; string REMOVE5 = "0%";
                string CHICAGOTotal = "0%"; string LAXTotal = "0%"; string EASTCOASTTotal = "0%";
                string OTHERSTTotal = "0%"; string TotalTotal = "0%";

                #region Summary Table 2
                dataSumRow = dtSUMMARY.NewRow();
                dataSumTitleRow = dtSUMMARY.NewRow();
                dataSumRow1 = dtSUMMARY.NewRow();
                dataSumRow2 = dtSUMMARY.NewRow();
                dataSumRow3 = dtSUMMARY.NewRow();
                dataSumRow4 = dtSUMMARY.NewRow();
                dataSumRow5 = dtSUMMARY.NewRow();
                dataSumRow6 = dtSUMMARY.NewRow();

                dataSumRow[0] = "%"; dataSumRow[1] = ""; dataSumRow[2] = ""; dataSumRow[3] = ""; dataSumRow[4] = ""; dataSumRow[5] = "";
                dataSumTitleRow[0] = "CATEGORY"; dataSumTitleRow[1] = "CHICAGO"; dataSumTitleRow[2] = "LAX"; dataSumTitleRow[3] = "EASTCOAST"; dataSumTitleRow[4] = "OTHERS"; dataSumTitleRow[5] = "TOTAL";

                if (Total_Chicago > 0)
                {
                    DECREASE1 = Convert.ToDouble(dtDECREASE_CHICAGO * 100 / Total_Chicago).ToString("0.00") + "%";
                    INCREASE1 = Convert.ToDouble(dtINCREASE_CHICAGO * 100 / Total_Chicago).ToString("0.00") + "%";
                    NEWLY1 = Convert.ToDouble(dtNEWLYADDED_CHICAGO * 100 / Total_Chicago).ToString("0.00") + "%";
                    NOCHANGE1 = Convert.ToDouble(dtNoChange_CHICAGO * 100 / Total_Chicago).ToString("0.00") + "%";
                    REMOVE1 = Convert.ToDouble(dtREMOVE_CHICAGO * 100 / Total_Chicago).ToString("0.00") + "%";

                    CHICAGOTotal = "100%";
                }
                if (Total_Lax > 0)
                {
                    DECREASE2 = Convert.ToDouble(dtDECREASE_LAX * 100 / Total_Lax).ToString("0.00") + "%";
                    INCREASE2 = Convert.ToDouble(dtINCREASE_LAX * 100 / Total_Lax).ToString("0.00") + "%";
                    NEWLY2 = Convert.ToDouble(dtNEWLYADDED_LAX * 100 / Total_Lax).ToString("0.00") + "%";
                    NOCHANGE2 = Convert.ToDouble(dtNoChange_LAX * 100 / Total_Lax).ToString("0.00") + "%";
                    REMOVE2 = Convert.ToDouble(dtREMOVE_LAX * 100 / Total_Lax).ToString("0.00") + "%";

                    LAXTotal = "100%";
                }
                if (Total_East > 0)
                {
                    DECREASE3 = Convert.ToDouble(dtDECREASE_EASTCOAST * 100 / Total_East).ToString("0.00") + "%";
                    INCREASE3 = Convert.ToDouble(dtINCREASE_EASTCOAST * 100 / Total_East).ToString("0.00") + "%";
                    NEWLY3 = Convert.ToDouble(dtNEWLYADDED_EASTCOAST * 100 / Total_East).ToString("0.00") + "%";
                    NOCHANGE3 = Convert.ToDouble(dtNoChange_EASTCOAST * 100 / Total_East).ToString("0.00") + "%";
                    REMOVE3 = Convert.ToDouble(dtREMOVE_EASTCOAST * 100 / Total_East).ToString("0.00") + "%";

                    EASTCOASTTotal = "100%";
                }
                if (Total_Other > 0)
                {
                    DECREASE4 = Convert.ToDouble(dtDECREASE_OTHERS * 100 / Total_Other).ToString("0.00") + "%";
                    INCREASE4 = Convert.ToDouble(dtINCREASE_OTHERS * 100 / Total_Other).ToString("0.00") + "%";
                    NEWLY4 = Convert.ToDouble(dtNEWLYADDED_OTHERS * 100 / Total_Other).ToString("0.00") + "%";
                    NOCHANGE4 = Convert.ToDouble(dtNoChange_OTHERS * 100 / Total_Other).ToString("0.00") + "%";
                    REMOVE4 = Convert.ToDouble(dtREMOVE_OTHERS * 100 / Total_Other).ToString("0.00") + "%";

                    OTHERSTTotal = "100%";
                }
                if (Total_Total > 0)
                {
                    DECREASE5 = Convert.ToDouble(TotalDecrease * 100 / Total_Total).ToString("0.00") + "%";
                    INCREASE5 = Convert.ToDouble(TotalIncrease * 100 / Total_Total).ToString("0.00") + "%";
                    NEWLY5 = Convert.ToDouble(TotalNewly * 100 / Total_Total).ToString("0.00") + "%";
                    NOCHANGE5 = Convert.ToDouble(TotalOnChange * 100 / Total_Total).ToString("0.00") + "%";
                    REMOVE5 = Convert.ToDouble(TotalRemove * 100 / Total_Total).ToString("0.00") + "%";

                    TotalTotal = "100%";
                }

                dataSumRow1[0] = "DECREASE"; dataSumRow1[1] = (DECREASE1).ToString(); dataSumRow1[2] = (DECREASE2).ToString(); dataSumRow1[3] = (DECREASE3).ToString(); dataSumRow1[4] = (DECREASE4).ToString(); dataSumRow1[5] = (DECREASE5).ToString();
                dataSumRow2[0] = "INCREASE"; dataSumRow2[1] = (INCREASE1).ToString(); dataSumRow2[2] = (INCREASE2).ToString(); dataSumRow2[3] = (INCREASE3).ToString(); dataSumRow2[4] = (INCREASE4).ToString(); dataSumRow2[5] = (INCREASE5).ToString();
                dataSumRow3[0] = "NEWLY ADDED"; dataSumRow3[1] = (NEWLY1).ToString(); dataSumRow3[2] = (NEWLY2).ToString(); dataSumRow3[3] = (NEWLY3).ToString(); dataSumRow3[4] = (NEWLY4).ToString(); dataSumRow3[5] = (NEWLY5).ToString();
                dataSumRow4[0] = "NO CHANGE"; dataSumRow4[1] = (NOCHANGE1).ToString(); dataSumRow4[2] = (NOCHANGE2).ToString(); dataSumRow4[3] = (NOCHANGE3).ToString(); dataSumRow4[4] = (NOCHANGE4).ToString(); dataSumRow4[5] = (NOCHANGE5).ToString();
                dataSumRow5[0] = "REMOVE"; dataSumRow5[1] = (REMOVE1).ToString(); dataSumRow5[2] = (REMOVE2).ToString(); dataSumRow5[3] = (REMOVE3).ToString(); dataSumRow5[4] = (REMOVE4).ToString(); dataSumRow5[5] = (REMOVE5).ToString();
                dataSumRow6[0] = "TOTAL"; dataSumRow6[1] = CHICAGOTotal; dataSumRow6[2] = LAXTotal; dataSumRow6[3] = EASTCOASTTotal; dataSumRow6[4] = OTHERSTTotal; dataSumRow6[5] = TotalTotal;

                dtSUMMARY.Rows.Add(dataSumRow);
                dtSUMMARY.Rows.Add(dataSumTitleRow);
                dtSUMMARY.Rows.Add(dataSumRow1);
                dtSUMMARY.Rows.Add(dataSumRow2);
                dtSUMMARY.Rows.Add(dataSumRow3);
                dtSUMMARY.Rows.Add(dataSumRow4);
                dtSUMMARY.Rows.Add(dataSumRow5);
                dtSUMMARY.Rows.Add(dataSumRow6);
                #endregion

                #region Two empty lines
                dataEmpty = dtSUMMARY.NewRow();
                dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " ";
                dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                dtSUMMARY.Rows.Add(dataEmpty);
                dataEmpty = dtSUMMARY.NewRow();
                dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " ";
                dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                dtSUMMARY.Rows.Add(dataEmpty);
                #endregion

                string Dif_DECREASE1 = "0.00"; string Dif_INCREASE1 = "0.00"; string Dif_NEWLY1 = "0.00"; string Dif_NOCHANGE1 = "0.00"; string Dif_REMOVE1 = "0.00";
                string Dif_DECREASE2 = "0.00"; string Dif_INCREASE2 = "0.00"; string Dif_NEWLY2 = "0.00"; string Dif_NOCHANGE2 = "0.00"; string Dif_REMOVE2 = "0.00";
                string Dif_DECREASE3 = "0.00"; string Dif_INCREASE3 = "0.00"; string Dif_NEWLY3 = "0.00"; string Dif_NOCHANGE3 = "0.00"; string Dif_REMOVE3 = "0.00";
                string Dif_DECREASE4 = "0.00"; string Dif_INCREASE4 = "0.00"; string Dif_NEWLY4 = "0.00"; string Dif_NOCHANGE4 = "0.00"; string Dif_REMOVE4 = "0.00";
                string Dif_DECREASE5 = "0.00"; string Dif_INCREASE5 = "0.00"; string Dif_NEWLY5 = "0.00"; string Dif_NOCHANGE5 = "0.00"; string Dif_REMOVE5 = "0.00";

                #region Summary Table 3
                dataSumRow = dtSUMMARY.NewRow();
                dataSumTitleRow = dtSUMMARY.NewRow();
                dataSumRow1 = dtSUMMARY.NewRow();
                dataSumRow2 = dtSUMMARY.NewRow();
                dataSumRow3 = dtSUMMARY.NewRow();
                dataSumRow4 = dtSUMMARY.NewRow();
                dataSumRow5 = dtSUMMARY.NewRow();
                dataSumRow6 = dtSUMMARY.NewRow();
                
                dataSumRow[0] = "AVERAGE CHANGE"; dataSumRow[1] = ""; dataSumRow[2] = ""; dataSumRow[3] = ""; dataSumRow[4] = ""; dataSumRow[5] = "";
                dataSumTitleRow[0] = "CATEGORY"; dataSumTitleRow[1] = "CHICAGO"; dataSumTitleRow[2] = "LAX"; dataSumTitleRow[3] = "EASTCOAST"; dataSumTitleRow[4] = "OTHERS"; dataSumTitleRow[5] = "TOTAL";

                if (dtDECREASE_CHICAGO > 0)
                {
                    Dif_DECREASE1 = Convert.ToDouble(Total_Dif_DECREASE_CHICAGO / dtDECREASE_CHICAGO).ToString("0.00");
                }
                if (dtINCREASE_CHICAGO > 0)
                {
                    Dif_INCREASE1 = Convert.ToDouble(Total_Dif_INCREASE_CHICAGO / dtINCREASE_CHICAGO).ToString("0.00");
                }
                if (dtNEWLYADDED_CHICAGO > 0)
                {
                    Dif_NEWLY1 = Convert.ToDouble(Total_Dif_NEWLYADDED_CHICAGO / dtNEWLYADDED_CHICAGO).ToString("0.00");
                }
                if (dtNoChange_CHICAGO > 0)
                {
                    Dif_NOCHANGE1 = Convert.ToDouble(Total_Dif_NoChange_CHICAGO / dtNoChange_CHICAGO).ToString("0.00");
                }
                if (dtREMOVE_CHICAGO > 0)
                {
                    Dif_REMOVE1 = Convert.ToDouble(Total_Dif_REMOVE_CHICAGO / dtREMOVE_CHICAGO).ToString("0.00");
                }


                if (dtDECREASE_LAX > 0)
                {
                    Dif_DECREASE2 = Convert.ToDouble(Total_Dif_DECREASE_LAX / dtDECREASE_LAX).ToString("0.00");
                }
                if (dtINCREASE_LAX > 0)
                {
                    Dif_INCREASE2 = Convert.ToDouble(Total_Dif_INCREASE_LAX / dtINCREASE_LAX).ToString("0.00");
                }
                if (dtNEWLYADDED_LAX > 0)
                {
                    Dif_NEWLY2 = Convert.ToDouble(Total_Dif_NEWLYADDED_LAX / dtNEWLYADDED_LAX).ToString("0.00");
                }
                if (dtNoChange_LAX > 0)
                {
                    Dif_NOCHANGE2 = Convert.ToDouble(Total_Dif_NoChange_LAX / dtNoChange_LAX).ToString("0.00");
                }
                if (dtREMOVE_LAX > 0)
                {
                    Dif_REMOVE2 = Convert.ToDouble(Total_Dif_REMOVE_LAX / dtREMOVE_LAX).ToString("0.00");

                }

                if (dtDECREASE_EASTCOAST > 0)
                {
                    Dif_DECREASE3 = Convert.ToDouble(Total_Dif_DECREASE_EASTCOAST / dtDECREASE_EASTCOAST).ToString("0.00");
                }
                if (dtINCREASE_EASTCOAST > 0)
                {
                    Dif_INCREASE3 = Convert.ToDouble(Total_Dif_INCREASE_EASTCOAST / dtINCREASE_EASTCOAST).ToString("0.00");
                }
                if (dtNEWLYADDED_EASTCOAST > 0)
                {
                    Dif_NEWLY3 = Convert.ToDouble(Total_Dif_NEWLYADDED_EASTCOAST / dtNEWLYADDED_EASTCOAST).ToString("0.00");
                }
                if (dtNoChange_EASTCOAST > 0)
                {
                    Dif_NOCHANGE3 = Convert.ToDouble(Total_Dif_NoChange_EASTCOAST / dtNoChange_EASTCOAST).ToString("0.00");
                }
                if (dtREMOVE_EASTCOAST > 0)
                {
                    Dif_REMOVE3 = Convert.ToDouble(Total_Dif_REMOVE_EASTCOAST / dtREMOVE_EASTCOAST).ToString("0.00");
                }


                if (dtDECREASE_OTHERS > 0)
                {
                    Dif_DECREASE4 = Convert.ToDouble(Total_Dif_DECREASE_OTHERS / dtDECREASE_OTHERS).ToString("0.00");
                }
                if (dtINCREASE_OTHERS > 0)
                {
                    Dif_INCREASE4 = Convert.ToDouble(Total_Dif_INCREASE_OTHERS / dtINCREASE_OTHERS).ToString("0.00");
                }
                if (dtNEWLYADDED_OTHERS > 0)
                {
                    Dif_NEWLY4 = Convert.ToDouble(Total_Dif_NEWLYADDED_OTHERS / dtNEWLYADDED_OTHERS).ToString("0.00");
                }
                if (dtNoChange_OTHERS > 0)
                {
                    Dif_NOCHANGE4 = Convert.ToDouble(Total_Dif_NoChange_OTHERS / dtNoChange_OTHERS).ToString("0.00");
                }
                if (dtREMOVE_OTHERS > 0)
                {
                    Dif_REMOVE4 = Convert.ToDouble(Total_Dif_REMOVE_OTHERS / dtREMOVE_OTHERS).ToString("0.00");
                }

                if (TotalDecrease != 0)
                    Dif_DECREASE5 = Convert.ToDouble((Total_Dif_DECREASE_CHICAGO + Total_Dif_DECREASE_LAX + Total_Dif_DECREASE_EASTCOAST + Total_Dif_DECREASE_OTHERS)/ (dtDECREASE_CHICAGO+ dtDECREASE_LAX + dtDECREASE_EASTCOAST+dtDECREASE_OTHERS)).ToString("0.00");
                else
                    Dif_DECREASE5 = "0.00";
                if(TotalIncrease!=0)
                    Dif_INCREASE5 = Convert.ToDouble((Total_Dif_INCREASE_CHICAGO + Total_Dif_INCREASE_LAX + Total_Dif_INCREASE_EASTCOAST + Total_Dif_INCREASE_OTHERS) / (dtINCREASE_CHICAGO+dtINCREASE_EASTCOAST+dtINCREASE_LAX+dtINCREASE_OTHERS)).ToString("0.00");
                else
                    Dif_INCREASE5 = "0.00";
                if (TotalNewly != 0)
                    Dif_NEWLY5 = Convert.ToDouble((Total_Dif_NEWLYADDED_CHICAGO + Total_Dif_NEWLYADDED_LAX + Total_Dif_NEWLYADDED_EASTCOAST + Total_Dif_NEWLYADDED_OTHERS) / (dtNEWLYADDED_CHICAGO+dtNEWLYADDED_EASTCOAST+dtNEWLYADDED_LAX+dtNEWLYADDED_OTHERS)).ToString("0.00");
                else
                    Dif_NEWLY5 = "0.00";
                if (TotalOnChange != 0)
                    Dif_NOCHANGE5 = Convert.ToDouble((Total_Dif_NoChange_CHICAGO + Total_Dif_NoChange_LAX + Total_Dif_NoChange_EASTCOAST + Total_Dif_NoChange_OTHERS) / (dtNoChange_CHICAGO+dtNoChange_EASTCOAST+dtNoChange_LAX+dtNoChange_OTHERS)).ToString("0.00");
                else
                    Dif_NOCHANGE5 = "0.00";
                if (TotalRemove != 0)
                    Dif_REMOVE5 = Convert.ToDouble((Total_Dif_REMOVE_CHICAGO + Total_Dif_REMOVE_LAX + Total_Dif_REMOVE_EASTCOAST + Total_Dif_REMOVE_OTHERS) / (dtREMOVE_CHICAGO+dtREMOVE_EASTCOAST+dtREMOVE_LAX+dtREMOVE_OTHERS)).ToString("0.00");
                else
                    Dif_REMOVE5 = "0.00";

                dataSumRow1[0] = "DECREASE"; dataSumRow1[1] = "$ "+(Dif_DECREASE1).ToString(); dataSumRow1[2] = "$ " + (Dif_DECREASE2).ToString(); dataSumRow1[3] = "$ " + (Dif_DECREASE3).ToString(); dataSumRow1[4] = "$ " + (Dif_DECREASE4).ToString(); dataSumRow1[5] = "$ " + (Dif_DECREASE5).ToString();
                dataSumRow2[0] = "INCREASE"; dataSumRow2[1] = "$ " + (Dif_INCREASE1).ToString(); dataSumRow2[2] = "$ " + (Dif_INCREASE2).ToString(); dataSumRow2[3] = "$ " + (Dif_INCREASE3).ToString(); dataSumRow2[4] = "$ " + (Dif_INCREASE4).ToString(); dataSumRow2[5] = "$ " + (Dif_INCREASE5).ToString();
                dataSumRow3[0] = "NEWLY ADDED"; dataSumRow3[1] = "$ " + (Dif_NEWLY1).ToString(); dataSumRow3[2] = "$ " + (Dif_NEWLY2).ToString(); dataSumRow3[3] = "$ " + (Dif_NEWLY3).ToString(); dataSumRow3[4] = "$ " + (Dif_NEWLY4).ToString(); dataSumRow3[5] = "$ " + (Dif_NEWLY5).ToString();
                dataSumRow4[0] = "NO CHANGE"; dataSumRow4[1] = "$ " + (Dif_NOCHANGE1).ToString(); dataSumRow4[2] = "$ " + (Dif_NOCHANGE2).ToString(); dataSumRow4[3] = "$ " + (Dif_NOCHANGE3).ToString(); dataSumRow4[4] = "$ " + (Dif_NOCHANGE4).ToString(); dataSumRow4[5] = "$ " + (Dif_NOCHANGE5).ToString();
                dataSumRow5[0] = "REMOVE"; dataSumRow5[1] = "$ " + (Dif_REMOVE1).ToString(); dataSumRow5[2] = "$ " + (Dif_REMOVE2).ToString(); dataSumRow5[3] = "$ " + (Dif_REMOVE3).ToString(); dataSumRow5[4] = "$ " + (Dif_REMOVE4).ToString(); dataSumRow5[5] = "$ " + (Dif_REMOVE5).ToString();
                dataSumRow6[0] = ""; dataSumRow6[1] = ""; dataSumRow6[2] = ""; dataSumRow6[3] = ""; dataSumRow6[4] = ""; dataSumRow6[5] = "";

                dtSUMMARY.Rows.Add(dataSumRow);
                dtSUMMARY.Rows.Add(dataSumTitleRow);
                dtSUMMARY.Rows.Add(dataSumRow1);
                dtSUMMARY.Rows.Add(dataSumRow2);
                dtSUMMARY.Rows.Add(dataSumRow3);
                dtSUMMARY.Rows.Add(dataSumRow4);
                dtSUMMARY.Rows.Add(dataSumRow5);
                dtSUMMARY.Rows.Add(dataSumRow6);
                #endregion

                #region Two empty lines
                dataEmpty = dtSUMMARY.NewRow();
                dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " ";
                dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                dtSUMMARY.Rows.Add(dataEmpty);
                dataEmpty = dtSUMMARY.NewRow();
                dataEmpty[0] = " "; dataEmpty[1] = " "; dataEmpty[2] = " ";
                dataEmpty[3] = " "; dataEmpty[4] = " "; dataEmpty[5] = " ";
                dtSUMMARY.Rows.Add(dataEmpty);
                #endregion

                #region Summary Table 4
                dataSumRow = dtSUMMARY.NewRow();
                dataSumTitleRow = dtSUMMARY.NewRow();
                dataSumRow1 = dtSUMMARY.NewRow();
                dataSumRow2 = dtSUMMARY.NewRow();
                dataSumRow3 = dtSUMMARY.NewRow();
                dataSumRow4 = dtSUMMARY.NewRow();
                dataSumRow5 = dtSUMMARY.NewRow();
                dataSumRow6 = TotalTable.NewRow();

                double removeTotal = dtREMOVE_CHICAGO + dtREMOVE_LAX + dtREMOVE_EASTCOAST + dtREMOVE_OTHERS;

                dataSumRow[0] =  "REMOVED COUNT"; dataSumRow[1] = ""; dataSumRow[2] = ""; dataSumRow[3] = ""; dataSumRow[4] = ""; dataSumRow[5] = "";
                dataSumRow1[0] = "CHICAGO"; dataSumRow1[1] = (dtREMOVE_CHICAGO).ToString(); dataSumRow1[2] = ("").ToString(); dataSumRow1[3] = ("").ToString(); dataSumRow1[4] = ("").ToString(); dataSumRow1[5] = ("").ToString();
                dataSumRow2[0] = "LAX"; dataSumRow2[1] = (dtREMOVE_LAX).ToString(); dataSumRow2[2] = ("").ToString(); dataSumRow2[3] = ("").ToString(); dataSumRow2[4] = ("").ToString(); dataSumRow2[5] = ("").ToString();
                dataSumRow3[0] = "EASTCOAST"; dataSumRow3[1] = (dtREMOVE_EASTCOAST).ToString(); dataSumRow3[2] = ("").ToString(); dataSumRow3[3] = ("").ToString(); dataSumRow3[4] = ("").ToString(); dataSumRow3[5] = ("").ToString();
                dataSumRow4[0] = "OTHERS"; dataSumRow4[1] = (dtREMOVE_OTHERS).ToString(); dataSumRow4[2] = ("").ToString(); dataSumRow4[3] = ("").ToString(); dataSumRow4[4] = ("").ToString(); dataSumRow4[5] = ("").ToString();
                dataSumRow5[0] = "TOTAL"; dataSumRow5[1] = (removeTotal).ToString(); dataSumRow5[2] = ""; dataSumRow5[3] = ""; dataSumRow5[4] = ""; dataSumRow5[5] = "";

                dtSUMMARY.Rows.Add(dataSumRow);
                dtSUMMARY.Rows.Add(dataSumRow1);
                dtSUMMARY.Rows.Add(dataSumRow2);
                dtSUMMARY.Rows.Add(dataSumRow3);
                dtSUMMARY.Rows.Add(dataSumRow4);
                dtSUMMARY.Rows.Add(dataSumRow5);
                #endregion


                DataSet ds = new DataSet();

                ds.Tables.Add(dtSUMMARY);
                ds.Tables.Add(dtNOCHANGE);
                ds.Tables.Add(dtDECREASE);
                ds.Tables.Add(dtINCREASE);
                ds.Tables.Add(dtNEWLYADDED);
                ds.Tables.Add(dtREMOVE);
                ds.Tables.Add(TotalTable);

                ds.Tables[0].TableName = "SUMMARY" + "(" + newDate.Trim() + ")";
                ds.Tables[1].TableName = "NOCHANGE" +"(" + TotalOnChange.ToString() + ")";
                ds.Tables[2].TableName = "DECREASE" + "(" + TotalDecrease.ToString() + ")";
                ds.Tables[3].TableName = "INCREASE" + "(" + TotalIncrease.ToString() + ")";
                ds.Tables[4].TableName = "NEWLYADDED" + "(" + TotalNewly.ToString() + ")";
                ds.Tables[5].TableName = "REMOVE" + "("+ TotalRemove.ToString() + ")";
                ds.Tables[6].TableName = "TOTAL" + "(" + Total_Total.ToString() + ")";

                #endregion

                if (ds != null && ds.Tables.Count > 0)
                {
                    ExportDataToExcel2(ds, "Tracking_Rate_Report_" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond.ToString());
                }
                else
                {
                    MessageBox.Show("No Data.", "No Data", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion
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

                    #region Style
                    ICellStyle style_header = workbook.CreateCellStyle();
                    IFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 11;
                    font.FontName = "Arial";
                    font.IsBold = true;
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
                    font1.FontHeightInPoints = 11;
                    font1.FontName = "Arial";
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
                    #endregion

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

                            if (TableName.Columns[i].ColumnName.ToLower().Contains("column"))
                            {
                                cell.SetCellValue("          ");
                            }
                            else
                            {
                                cell.SetCellValue(TableName.Columns[i].ColumnName.ToUpper());
                            }

                            cell.CellStyle = style_header;

                            if(sheet.SheetName.Contains("SUMMARY") && i==0)
                            {
                                sheet.SetColumnWidth(i, 20 * 256);

                            }else if(sheet.SheetName.Contains("SUMMARY"))
                            {
                                sheet.SetColumnWidth(i, 15 * 256);
                            }else
                            {
                                sheet.AutoSizeColumn(i);
                            }
                        }

                        for (int i = 0; i < TableName.Rows.Count; i++)
                        {
                           
                            IRow rowData = sheet.CreateRow(i + 1);

                            for (int j = 0; j < TableName.Columns.Count; j++)
                            {
                                ICell cell = rowData.CreateCell(j);
                                string strValue = TableName.Rows[i][j].ToString();

                                if (!sheet.SheetName.Contains("SUMMARY"))
                                {
                                    if (j > 6 && j < 12)
                                    {
                                        if (strValue == "")
                                            strValue = "0";
                                        cell.SetCellValue(double.Parse(strValue));
                                    }
                                    else
                                    {
                                        cell.SetCellValue(strValue);
                                    }
                                }
                                else
                                {
                                    if(((i>1 && i<8 && j>0)||(i>30 && i<36 && j > 0)) && strValue.Trim()!="")
                                        cell.SetCellValue(double.Parse(strValue));
                                    else
                                        cell.SetCellValue(strValue);
                                }


                                if ((sheet.SheetName.Contains("SUMMARY") && ( i == 0 || i==1 || i == 10 || i == 11 || i == 20 || i == 21 || i == 30)) )
                                {
                                    cell.CellStyle = style_header;
                                }else
                                {
                                    cell.CellStyle = style_row;
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

       
    }
}
