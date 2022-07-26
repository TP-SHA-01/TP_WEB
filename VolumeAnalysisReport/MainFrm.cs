using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TB_WEB.CommonLibrary.CommonFun;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;

namespace VolumeAnalysisReport
{
    public partial class MainFrm : Form
    {
        DBHelper dbHelper = new DBHelper();
        public MainFrm()
        {
            InitializeComponent();
        }

        private void btn_TitanUpload_Click(object sender, EventArgs e)
        {
            string pPathFileName = String.Empty;

            try
            {
                if (showDialogGetSaveFolder(out pPathFileName))
                {
                    txt_TitanFileName.Text = pPathFileName.Trim();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btn_RemarkUpload_Click(object sender, EventArgs e)
        {
            string pPathFileName = String.Empty;

            try
            {
                if (showDialogGetSaveFolder(out pPathFileName))
                {
                    txt_RemarkFIleName.Text = pPathFileName.Trim();

                    FileInfo _folder = new FileInfo(txt_RemarkFIleName.Text);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            try
            {
                btn_Create.Enabled = false;
                // For Titan
                if (!String.IsNullOrEmpty(txt_TitanFileName.Text) && !String.IsNullOrEmpty(txt_RemarkFIleName.Text))
                {
                    FileInfo _folder = new FileInfo(txt_TitanFileName.Text);
                    FileInfo _folderRemark = new FileInfo(txt_RemarkFIleName.Text);

                    string filePath = _folder.DirectoryName;
                    string fileType = _folder.FullName.Substring(_folder.FullName.LastIndexOf(".") + 1, _folder.FullName.Length - _folder.FullName.LastIndexOf(".") - 1);
                    string originPath = filePath + "\\" + "VolumeAnalysisReport_" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond.ToString() + ".xlsx";


                    DataSet ds = GenerateTitanData(_folder);
                    var qMBL = ds.Tables["ALL BILL"].AsEnumerable().Select(p => p.Field<string>("MASTER BL"));

                    string arrMBL = String.Join("','", qMBL.ToArray());

                    DataTable dt_Remark = GenerateRemarkData(_folderRemark);
                    DataTable dt_TBS = CommonFun.GenerateTBSData(arrMBL);


                    foreach (DataRow dr in dt_TBS.Rows)
                    {
                        if (IsEmpty(dr["ETD"]))
                        {
                            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                            dtFormat.ShortDatePattern = "MM/dd/yy";
                            dr["ETD"] = Convert.ToDateTime(CheckEmpty(dr["ETD"]), dtFormat).ToString();
                        }
                    }

                    DataTable dt_First = new DataTable();
                    DataTable dt_Second = new DataTable();
                    DataTable dt_All = new DataTable();

                    DataSet tempDS = new DataSet();
                    foreach (DataTable tempDT in ds.Tables)
                    {
                        string tableName = tempDT.TableName;
                        var query = from p in tempDT.AsEnumerable()
                                    join c in dt_Remark.AsEnumerable() on p.Field<string>("MASTER BL") equals c.Field<string>("MASTER BL") into temp
                                    from remark in temp.DefaultIfEmpty()
                                    join tbs in dt_TBS.AsEnumerable() on p.Field<string>("MASTER BL") equals tbs.Field<string>("MBL") into temp_TBS
                                    from tbs_data in temp_TBS.DefaultIfEmpty()
                                    select new
                                    {
                                        AC_CODE = p.Field<string>("A/C CODE"),
                                        CUSTOMER = p.Field<string>("CUSTOMER"),
                                        CONSIGNEE = p.Field<string>("CONSIGNEE"),
                                        TEL = p.Field<string>("TEL"),
                                        BILLNO = p.Field<string>("中國發票號碼."),
                                        INVOICE_NO = p.Field<string>("INVOICE NO."),
                                        HBL = p.Field<string>("HOUSE BL"),
                                        MBL = p.Field<string>("MASTER BL"),
                                        ISSUE_DATE = p.Field<string>("ISSUE DATE"),
                                        ETA = p.Field<string>("ETA"),
                                        CURRENCY = p.Field<string>("CURRENCY"),
                                        ORIGINAL_AMOUNT = p.Field<string>("ORIGINAL AMOUNT"),
                                        EQUIVANLENT_AMOUNT = p.Field<string>("EQUIVANLENT AMOUNT"),
                                        EXCHANGE_RATE = p.Field<string>("EXCHANGE RATE"),
                                        OUTSTANDING_AMOUNT = p.Field<string>("OUTSTANDING AMOUNT"),
                                        PAYMENT_TERMS = p.Field<string>("PAYMENT TERMS"),
                                        USER = p.Field<string>("USER"),
                                        WEEK = p.Field<string>("WEEK"),
                                        MONTH = p.Field<string>("MONTH"),
                                        REMARK = remark == null ? "#N/A" : remark.Field<string>("REMARK"),
                                        NOMINATION_OFFICE = tbs_data == null ? "#N/A" : tbs_data.Field<string>("Nomination Office"),
                                        PCI_SALES = tbs_data == null ? "#N/A" : tbs_data.Field<string>("PIC(销售）"),
                                        ETD = tbs_data == null ? "" : tbs_data.Field<string>("ETD"),
                                        CS = tbs_data == null ? "#N/A" : tbs_data.Field<string>("Created By"),
                                        OP = tbs_data == null ? "#N/A" : tbs_data.Field<string>("Handling User Email"),
                                    };

                        switch (tableName.ToUpper())
                        {
                            case "BILL":
                                dt_First = GetDistinctTable(LINQToDataTable(query));
                                break;
                            case "NO BILL":
                                dt_Second = GetDistinctTable(LINQToDataTable(query));
                                break;
                            case "ALL BILL":
                                dt_All = GetDistinctTable(LINQToDataTable(query));
                                break;
                        }
                    }

                    try
                    {
                        CommonFun.RenderColumn(dt_All);
                        CommonFun.RenderColumn(dt_First);
                        CommonFun.RenderColumn(dt_Second);
                    }
                    catch (Exception)
                    {
                        btn_Create.Enabled = true;
                        throw;
                    }
                    

                    dt_All.TableName = "ALL DATA";
                    dt_First.TableName = "INVOICED NOT PAID";
                    dt_Second.TableName = "NOT INVOICED";
                    

                    tempDS.Tables.Add(dt_All);
                    tempDS.Tables.Add(dt_First);
                    tempDS.Tables.Add(dt_Second);

                    NPOIHelper.ExportExcel_VolumeAnalysisReport(tempDS, originPath);

                    if (MessageBox.Show("Export Success，Open the File？", "Tips", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(originPath);
                    }

                    
                }
                else
                {
                    MessageBox.Show("Error Msg: Should Upload the Titan Original File And Remark File ");
                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Message: " + ex.Message + ",StackTrace: " + ex.StackTrace);
                MessageBox.Show("Error Msg:" + ex.Message + " , StackTrace:" + ex.StackTrace, "Error");
                return;
            }
            finally
            {
                btn_Create.Enabled = true;
            }
        }


        

        public bool showDialogGetSaveFolder(out string folderPath)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Files|*.xls;*.xlsx";
            //dlg.Filter = "Files|*.xlsx";
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            bool dialogResult = dlg.ShowDialog() == DialogResult.OK;
            folderPath = dlg.FileName;

            return dialogResult;
        }

        private DataSet GenerateTitanData(FileInfo _folder)
        {
            string strFileName = String.Empty;
            string filePath = String.Empty;
            string filetype = String.Empty;
            ISheet sheet;

            strFileName = _folder.FullName;
            filePath = _folder.DirectoryName;
            int ii = strFileName.LastIndexOf(".");
            filetype = strFileName.Substring(ii + 1, strFileName.Length - ii - 1);

            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    xssfworkbook = new XSSFWorkbook(file);
                }

                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }

                sheet = hssfworkbook.GetSheetAt(0);
            }

            IEnumerator rows = sheet.GetRowEnumerator();
            int sheetRowCount = sheet.LastRowNum;
            int headStartNum = 0;
            int EndNum = 0;
            bool firstPart = true;

            for (int i = 0; i < sheetRowCount; i++)
            {
                IRow _headerRow = sheet.GetRow(i);

                if (_headerRow != null)
                {
                    ICell _cell = _headerRow.GetCell(1);

                    if (_cell != null)
                    {
                        if (!String.IsNullOrEmpty(_cell.ToString()))
                        {
                            if (firstPart)
                            {
                                if (_cell.ToString().ToUpper().Trim() == "CUSTOMER")
                                {
                                    headStartNum = i;
                                    firstPart = false;
                                    continue;
                                }
                            }
                            else
                            {
                                if (_cell.ToString().ToUpper().Trim() == "CUSTOMER")
                                {
                                    EndNum = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            int firstPartStartNum = headStartNum;
            int firstPartEndNum = EndNum;

            int secondPartStartNum = EndNum;
            int secondPartEndNum = sheetRowCount;

            DataTable dt_first = InitExcelData(sheet, firstPartStartNum, firstPartEndNum, "BILL");

            // Part Two
            DataTable dt_second = InitExcelData(sheet, secondPartStartNum, secondPartEndNum, "NO BILL");

            DataTable dt_all = InitExcelData(sheet, firstPartStartNum, secondPartEndNum, "ALL BILL");

            DataSet ds = new DataSet();

            if (!dt_first.Columns.Contains("MONTH"))
            {
                dt_first.Columns.Add("MONTH").SetOrdinal(dt_first.Columns.Count - 2);
            }

            if (!dt_second.Columns.Contains("MONTH"))
            {
                dt_second.Columns.Add("MONTH").SetOrdinal(dt_second.Columns.Count - 2);
            }

            if (!dt_all.Columns.Contains("MONTH"))
            {
                dt_all.Columns.Add("MONTH").SetOrdinal(dt_all.Columns.Count - 2);
            }

            foreach (DataRow dr in dt_first.Rows)
            {
                if (IsEmpty(dr["ISSUE DATE"]))
                {
                    DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

                    dtFormat.ShortDatePattern = "MM/dd/yy";

                    DateTime issueDate = Convert.ToDateTime(CheckEmpty(dr["ISSUE DATE"]), dtFormat);
                    
                    dr["MONTH"] = issueDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
                    dr["ISSUE DATE"] = issueDate.ToString();
                }

                if (IsEmpty(dr["ETA"]))
                {
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                    dtFormat.ShortDatePattern = "MM/dd/yy";
                    dr["ETA"] = Convert.ToDateTime(CheckEmpty(dr["ETA"]), dtFormat).ToString();
                }

            }

            foreach (DataRow dr in dt_second.Rows)
            {
                if (IsEmpty(dr["ISSUE DATE"]))
                {
                    DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

                    dtFormat.ShortDatePattern = "MM/dd/yy";

                    DateTime issueDate = Convert.ToDateTime(CheckEmpty(dr["ISSUE DATE"]), dtFormat);
                    dr["MONTH"] = issueDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
                    dr["ISSUE DATE"] = issueDate.ToString();
                }

                if (IsEmpty(dr["ETA"]))
                {
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                    dtFormat.ShortDatePattern = "MM/dd/yy";
                    dr["ETA"] = Convert.ToDateTime(CheckEmpty(dr["ETA"]), dtFormat).ToString();
                }
            }

            foreach (DataRow dr in dt_all.Rows)
            {
                if (IsEmpty(dr["ISSUE DATE"]))
                {
                    DateTimeFormatInfo dtFormat = new System.Globalization.DateTimeFormatInfo();

                    dtFormat.ShortDatePattern = "MM/dd/yy";

                    DateTime issueDate = Convert.ToDateTime(CheckEmpty(dr["ISSUE DATE"]), dtFormat);
                    dr["MONTH"] = issueDate.ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB"));
                    dr["ISSUE DATE"] = issueDate.ToString();
                }

                if (IsEmpty(dr["ETA"]))
                {
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                    dtFormat.ShortDatePattern = "MM/dd/yy";
                    dr["ETA"] = Convert.ToDateTime(CheckEmpty(dr["ETA"]), dtFormat).ToString();
                }
            }

            ds.Tables.Add(dt_first);
            ds.Tables.Add(dt_second);
            ds.Tables.Add(dt_all);

            return ds;
        }

        private DataTable GenerateRemarkData(FileInfo _folder)
        {
            DataTable dt = new DataTable();

            string strFileName = String.Empty;
            string filePath = String.Empty;
            string filetype = String.Empty;
            ISheet sheet;

            strFileName = _folder.FullName;
            filePath = _folder.DirectoryName;
            int ii = strFileName.LastIndexOf(".");
            filetype = strFileName.Substring(ii + 1, strFileName.Length - ii - 1);

            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    xssfworkbook = new XSSFWorkbook(file);
                }

                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }

                sheet = hssfworkbook.GetSheetAt(0);
            }

            IEnumerator rows = sheet.GetRowEnumerator();
            int sheetRowCount = sheet.LastRowNum;
            int headStartNum = 0;

            for (int i = 0; i < sheetRowCount; i++)
            {
                IRow _headerRow = sheet.GetRow(i);

                if (_headerRow != null)
                {
                    ICell _cell = _headerRow.GetCell(1);

                    if (_cell != null)
                    {
                        if (!String.IsNullOrEmpty(_cell.ToString()))
                        {
                            if (_cell.ToString().ToUpper().Trim() == "MASTER BL")
                            {
                                headStartNum = i;
                                break;
                            }
                        }
                    }
                }
            }

            IRow headerRow = sheet.GetRow(headStartNum);
            int cellCount = headerRow.LastCellNum;


            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }


            for (int i = (sheet.FirstRowNum + 1 + headStartNum); i <= sheetRowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null && row.FirstCellNum >= 0)
                {
                    if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                    {
                        DataRow dataRow = dt.NewRow();
                        if (row.GetCell(1) != null)
                        {
                            if (!String.IsNullOrEmpty(row.GetCell(1).ToString()) && row.GetCell(1).ToString().ToUpper() != "MASTER BL")
                            {
                                for (int j = row.FirstCellNum; j < cellCount; j++)
                                {
                                    if (row.GetCell(j) != null)
                                    {

                                        dataRow[j] = row.GetCell(j).ToString();

                                    }
                                }
                                dt.Rows.Add(dataRow);
                            }
                        }
                    }
                }

            }

            return dt;
        }

        

        private DataTable InitExcelData(ISheet sheet, int startNum, int endNum, string TableName)
        {
            DataTable dt = new DataTable(TableName);

            IRow headerRow = sheet.GetRow(startNum);
            int cellCount = headerRow.LastCellNum;

            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }

            for (int i = (sheet.FirstRowNum + 1 + startNum); i <= endNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row != null && row.FirstCellNum >= 0)
                {
                    if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                    {
                        DataRow dataRow = dt.NewRow();
                        if (row.GetCell(1) != null)
                        {
                            if (!String.IsNullOrEmpty(row.GetCell(1).ToString()) && row.GetCell(1).ToString().ToUpper() != "CUSTOMER")
                            {
                                for (int j = row.FirstCellNum; j < cellCount; j++)
                                {
                                    if (row.GetCell(j) != null)
                                    {

                                        dataRow[j] = row.GetCell(j).ToString();

                                    }
                                }
                                dt.Rows.Add(dataRow);
                            }
                        }
                    }
                }

            }


            return dt;
        }


        private  bool IsEmpty(Object str)
        {
            return str != null && !"".Equals(str) && !Convert.IsDBNull(str);
        }

        private  bool IsNotEmpty(string str)
        {
            return str != null && !String.IsNullOrEmpty(str);
        }

        private  string ReplaceWrap(Object str)
        {
            return CheckEmpty(str).Replace("\x0A", "<br/>").Replace("\x0D", "<br/>").Replace(" ", "&nbsp;");
        }

        private  double CheckNumber(Object str)
        {
            double ret = 0;
            //try
            //{
            //    ret = double.Parse(CheckEmpty(str));
            //}
            //catch (Exception)
            //{
            //    ret = 0;
            //}

            if (double.TryParse(CheckEmpty(str), out ret))
            {
                return ret;
            }

            return ret;
        }

        private string CheckEmpty(Object str)
        {
            string ret = String.Empty;

            if (IsEmpty(str))
            {
                ret = str.ToString().Trim();
            }

            return ret;
        }


        private DataTable LINQToDataTable<T>(IEnumerable<T> varlist)

        {
            DataTable dtReturn = new DataTable();
            // column names 
            PropertyInfo[] oProps = null;



            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        /// <summary>
        /// datatable去重
        /// </summary>
        /// <param name="dtSource">需要去重的datatable</param>
        /// <returns></returns>
        public DataTable GetDistinctTable(DataTable dtSource)
        {
            DataTable distinctTable = null;
            try
            {
                if (dtSource != null && dtSource.Rows.Count > 0)
                {
                    string[] columnNames = GetTableColumnName(dtSource);
                    DataView dv = new DataView(dtSource);
                    distinctTable = dv.ToTable(true, columnNames);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            return distinctTable;
        }

        #region 获取表中所有列名
        public string[] GetTableColumnName(DataTable dt)
        {
            string cols = string.Empty;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                cols += (dt.Columns[i].ColumnName + ",");
            }
            cols = cols.TrimEnd(',');
            return cols.Split(',');
        }
        #endregion
    }
}
