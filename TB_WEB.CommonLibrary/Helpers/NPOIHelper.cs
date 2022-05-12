using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Helpers
{
    /// <summary>
    /// office 导入导出
    /// </summary>
    public static class NPOIHelper
    {
        public static MemoryStream RenderToExcel(DataTable table, string sheetName = null)
        {
            MemoryStream ms = new MemoryStream();

            try
            {
                using (table)
                {
                    IWorkbook workbook = new HSSFWorkbook();
                    //IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet();
                    IDataFormat format = workbook.CreateDataFormat();
                    ICellStyle dateStyle = workbook.CreateCellStyle();
                    dateStyle.Alignment = HorizontalAlignment.Center;
                    IFont font = workbook.CreateFont();
                    font.Boldweight = 700;
                    dateStyle.SetFont(font);

                    dateStyle.DataFormat = format.GetFormat("MM/dd/yyyy HH:mm:ss");
                    IRow headerRow = sheet.CreateRow(0);
                    if (!String.IsNullOrEmpty(sheetName))
                    {
                        workbook.SetSheetName(0, sheetName);
                    }
                    // handling header.
                    foreach (DataColumn column in table.Columns)
                        headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value

                    sheet.ForceFormulaRecalculation = true;

                    int[] arrColWidth = new int[table.Columns.Count];
                    foreach (DataColumn item in table.Columns)
                    {
                        if (arrColWidth[item.Ordinal] >= 255)
                        {
                            arrColWidth[item.Ordinal] = 99;
                        }
                        else
                        {
                            arrColWidth[item.Ordinal] = Encoding.GetEncoding("UTF-8").GetBytes(item.ColumnName.ToString()).Length;
                        }
                    }
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        for (int j = 0; j < table.Columns.Count; j++)
                        {
                            int intTemp = Encoding.GetEncoding("UTF-8").GetBytes(table.Rows[i][j].ToString()).Length;
                            if (intTemp > arrColWidth[j])
                            {
                                if (arrColWidth[j] >= 255)
                                {
                                    arrColWidth[j] = 99;
                                }
                                else
                                {
                                    arrColWidth[j] = intTemp;
                                }

                            }
                        }
                    }

                    // handling value.
                    int rowIndex = 1;

                    foreach (DataRow row in table.Rows)
                    {
                        IRow dataRow = sheet.CreateRow(rowIndex);

                        foreach (DataColumn column in table.Columns)
                        {
                            ICell newCell = dataRow.CreateCell(column.Ordinal);
                            string drValue = row[column].ToString();
                            switch (column.DataType.ToString())
                            {
                                case "System.String"://字符串类型
                                    newCell.SetCellValue(drValue);
                                    break;
                                case "System.DateTime"://日期类型
                                    DateTime dateV;
                                    DateTime.TryParse(drValue, out dateV);
                                    newCell.SetCellValue(dateV);
                                    newCell.CellStyle = dateStyle;//格式化显示
                                    break;
                                case "System.Boolean"://布尔型
                                    bool boolV = false;
                                    bool.TryParse(drValue, out boolV);
                                    newCell.SetCellValue(boolV);
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    int intV = 0;
                                    int.TryParse(drValue, out intV);
                                    newCell.SetCellValue(intV);
                                    break;
                                case "System.Decimal":
                                case "System.Double":
                                    double doubV = 0;
                                    double.TryParse(drValue, out doubV);
                                    newCell.SetCellValue(doubV);
                                    break;
                                case "System.DBNull"://空值处理
                                    newCell.SetCellValue("");
                                    break;
                                default:
                                    newCell.SetCellValue("");
                                    break;
                            }

                            if (arrColWidth[column.Ordinal] >= 255)
                            {
                                sheet.SetColumnWidth(column.Ordinal, 99);
                            }
                            else
                            {
                                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 199);
                            }

                        }

                        rowIndex++;
                    }

                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return ms;
        }

        /// <summary>
        /// DataTable 导出到 Excel 的 MemoryStream
        /// </summary>
        /// <param name="dtSource">源 DataTable</param>
        /// <param name="strHeaderText">表头文本 空值未不要表头标题</param>
        /// <returns></returns>
        public static MemoryStream ExportExcel(DataTable dtSource, string strHeaderText)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            #region 文件属性
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "topocean.com.cn";
            workbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "topocean.com.cn";
            si.ApplicationName = "topocean.com.cn";
            si.LastAuthor = "topocean.com.cn";
            si.Comments = "";
            si.Title = "";
            si.Subject = "";
            si.CreateDateTime = DateTime.Now;
            workbook.SummaryInformation = si;
            #endregion
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding("UTF-8").GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding("UTF-8").GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            int intTop = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表、填充表头、填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }
                    intTop = 0;
                    #region 表头及样式
                    {
                        if (strHeaderText.Length > 0)
                        {
                            IRow headerRow = sheet.CreateRow(intTop);
                            intTop += 1;
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(strHeaderText);
                            ICellStyle headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = workbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));

                        }
                    }
                    #endregion
                    #region  列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(intTop);
                        intTop += 1;
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                        }


                    }
                    #endregion
                    rowIndex = intTop;
                }
                #endregion
                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);
                            newCell.CellStyle = dateStyle;//格式化显示
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        /// <summary>
        /// DaataTable 导出到 Excel 文件
        /// </summary>
        /// <param name="dtSource">源 DataaTable</param>
        /// <param name="strHeaderText">表头文本</param>
        /// <param name="strFileName">保存位置(文件名及路径)</param>
        public static void ExportExcel(DataTable dtSource, string strHeaderText, string strFileName)
        {
            using (MemoryStream ms = RenderToExcel(dtSource, strHeaderText))
            {
                using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }

        /// <summary>
        /// 读取 excel
        /// 默认第一行为标头
        /// </summary>
        /// <param name="strFileName">excel 文档路径</param>
        /// <returns></returns>
        public static DataTable ImportExcel(string strFileName, int startLine, int lastLine)
        {
            int ii = strFileName.LastIndexOf(".");
            string filetype = strFileName.Substring(ii + 1, strFileName.Length - ii - 1);
            DataTable dt = new DataTable();
            ISheet sheet;
            if ("xlsx" == filetype)
            {
                XSSFWorkbook xssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                    xssfworkbook = new XSSFWorkbook(file);
                }
                sheet = xssfworkbook.GetSheetAt(0);
            }
            else
            {
                HSSFWorkbook hssfworkbook;
                using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                sheet = hssfworkbook.GetSheetAt(0);
            }
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            IRow headerRow = sheet.GetRow(startLine);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                ICell cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString());
            }
            for (int i = (sheet.FirstRowNum + startLine); i <= sheet.LastRowNum - lastLine; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                {
                    DataRow dataRow = dt.NewRow();
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
            return dt;

        }


        /// <summary>
        /// DataSet 导出到 Excel 的 MemoryStream
        /// </summary>
        /// <param name="dsSource">源 DataSet</param>
        /// <param name="strHeaderText">表头文本 空值未不要表头标题(多个表对应多个表头以英文逗号(,)分开，个数应与表相同)</param>
        /// <returns></returns>
        public static MemoryStream ExportExcel(DataSet dsSource, string strHeaderText)
        {

            HSSFWorkbook workbook = new HSSFWorkbook();

            #region 文件属性
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "yuebon.com";
            workbook.DocumentSummaryInformation = dsi;
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Author = "yuebon.com";
            si.ApplicationName = "yuebon.com";
            si.LastAuthor = "yuebon.com";
            si.Comments = "";
            si.Title = "";
            si.Subject = "";
            si.CreateDateTime = DateTime.Now;
            workbook.SummaryInformation = si;
            #endregion

            #region 注释


            //ICellStyle dateStyle = workbook.CreateCellStyle();
            //IDataFormat format = workbook.CreateDataFormat();
            //dateStyle.DataFormat = format.GetFormat("yyyy-mm-dd");

            //ISheet sheet = workbook.CreateSheet();
            //int[] arrColWidth = new int[dtSource.Columns.Count];
            //foreach (DataColumn item in dtSource.Columns)
            //{
            //    arrColWidth[item.Ordinal] = Encoding.GetEncoding("gb2312").GetBytes(item.ColumnName.ToString()).Length;
            //}
            //for (int i = 0; i < dtSource.Rows.Count; i++)
            //{
            //    for (int j = 0; j < dtSource.Columns.Count; j++)
            //    {
            //        int intTemp = Encoding.GetEncoding("gb2312").GetBytes(dtSource.Rows[i][j].ToString()).Length;
            //        if (intTemp > arrColWidth[j])
            //        {
            //            arrColWidth[j] = intTemp;
            //        }
            //    }
            //}
            //int rowIndex = 0;
            //int intTop = 0;
            //foreach (DataRow row in dtSource.Rows)
            //{
            //    #region 新建表、填充表头、填充列头，样式
            //    if (rowIndex == 65535 || rowIndex == 0)
            //    {
            //        if (rowIndex != 0)
            //        {
            //            sheet = workbook.CreateSheet();
            //        }
            //        intTop = 0;
            //        #region 表头及样式
            //        {
            //            if (strHeaderText.Length > 0)
            //            {
            //                IRow headerRow = sheet.CreateRow(intTop);
            //                intTop += 1;
            //                headerRow.HeightInPoints = 25;
            //                headerRow.CreateCell(0).SetCellValue(strHeaderText);
            //                ICellStyle headStyle = workbook.CreateCellStyle();
            //                headStyle.Alignment = HorizontalAlignment.CENTER;
            //                IFont font = workbook.CreateFont();
            //                font.FontHeightInPoints = 20;
            //                font.Boldweight = 700;
            //                headStyle.SetFont(font);
            //                headerRow.GetCell(0).CellStyle = headStyle;
            //                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));

            //            }
            //        }
            //        #endregion
            //        #region  列头及样式
            //        {
            //            IRow headerRow = sheet.CreateRow(intTop);
            //            intTop += 1;
            //            ICellStyle headStyle = workbook.CreateCellStyle();
            //            headStyle.Alignment = HorizontalAlignment.CENTER;
            //            IFont font = workbook.CreateFont();
            //            font.Boldweight = 700;
            //            headStyle.SetFont(font);
            //            foreach (DataColumn column in dtSource.Columns)
            //            {
            //                headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
            //                headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
            //                //设置列宽
            //                sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
            //            }


            //        }
            //        #endregion
            //        rowIndex = intTop;
            //    }
            //    #endregion
            //    #region 填充内容
            //    IRow dataRow = sheet.CreateRow(rowIndex);
            //    foreach (DataColumn column in dtSource.Columns)
            //    {
            //        ICell newCell = dataRow.CreateCell(column.Ordinal);
            //        string drValue = row[column].ToString();
            //        switch (column.DataType.ToString())
            //        {
            //            case "System.String"://字符串类型
            //                newCell.SetCellValue(drValue);
            //                break;
            //            case "System.DateTime"://日期类型
            //                DateTime dateV;
            //                DateTime.TryParse(drValue, out dateV);
            //                newCell.SetCellValue(dateV);
            //                newCell.CellStyle = dateStyle;//格式化显示
            //                break;
            //            case "System.Boolean"://布尔型
            //                bool boolV = false;
            //                bool.TryParse(drValue, out boolV);
            //                newCell.SetCellValue(boolV);
            //                break;
            //            case "System.Int16":
            //            case "System.Int32":
            //            case "System.Int64":
            //            case "System.Byte":
            //                int intV = 0;
            //                int.TryParse(drValue, out intV);
            //                newCell.SetCellValue(intV);
            //                break;
            //            case "System.Decimal":
            //            case "System.Double":
            //                double doubV = 0;
            //                double.TryParse(drValue, out doubV);
            //                newCell.SetCellValue(doubV);
            //                break;
            //            case "System.DBNull"://空值处理
            //                newCell.SetCellValue("");
            //                break;
            //            default:
            //                newCell.SetCellValue("");
            //                break;
            //        }
            //    }
            //    #endregion
            //    rowIndex++;
            //}
            #endregion

            string[] strNewText = strHeaderText.Split(Convert.ToChar(","));
            if (dsSource.Tables.Count == strNewText.Length)
            {
                for (int i = 0; i < dsSource.Tables.Count; i++)
                {
                    ExportFromDSExcel(workbook, dsSource.Tables[i], strNewText[i]);
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
                return ms;
            }
        }
        /// <summary>
        /// DataTable 导出到 Excel 的 MemoryStream
        /// </summary>
        /// <param name="workbook">源 workbook</param>
        /// <param name="dtSource">源 DataTable</param>
        /// <param name="strHeaderText">表头文本 空值未不要表头标题(多个表对应多个表头以英文逗号(,)分开，个数应与表相同)</param>
        /// <returns></returns>
        public static void ExportFromDSExcel(HSSFWorkbook workbook, DataTable dtSource, string strHeaderText)
        {
            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd HH:mm:ss");
            ISheet sheet = workbook.CreateSheet(strHeaderText);

            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                arrColWidth[item.Ordinal] = Encoding.GetEncoding("gb2312").GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding("gb2312").GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            int rowIndex = 0;
            int intTop = 0;
            foreach (DataRow row in dtSource.Rows)
            {
                #region 新建表、填充表头、填充列头，样式
                if (rowIndex == 65535 || rowIndex == 0)
                {
                    if (rowIndex != 0)
                    {
                        sheet = workbook.CreateSheet();
                    }
                    intTop = 0;
                    #region 表头及样式
                    {
                        if (strHeaderText.Length > 0)
                        {
                            IRow headerRow = sheet.CreateRow(intTop);
                            intTop += 1;
                            headerRow.HeightInPoints = 25;
                            headerRow.CreateCell(0).SetCellValue(strHeaderText);
                            ICellStyle headStyle = workbook.CreateCellStyle();
                            headStyle.Alignment = HorizontalAlignment.Center;
                            IFont font = workbook.CreateFont();
                            font.FontHeightInPoints = 20;
                            font.Boldweight = 700;
                            headStyle.SetFont(font);
                            headerRow.GetCell(0).CellStyle = headStyle;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));

                        }
                    }
                    #endregion
                    #region  列头及样式
                    {
                        IRow headerRow = sheet.CreateRow(intTop);
                        intTop += 1;
                        ICellStyle headStyle = workbook.CreateCellStyle();
                        headStyle.Alignment = HorizontalAlignment.Center;
                        IFont font = workbook.CreateFont();
                        font.Boldweight = 700;
                        headStyle.SetFont(font);
                        foreach (DataColumn column in dtSource.Columns)
                        {
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);
                            headerRow.GetCell(column.Ordinal).CellStyle = headStyle;
                            //设置列宽
                            // sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256); // 设置设置列宽 太长会报错 修改2014 年9月22日
                            int dd = (arrColWidth[column.Ordinal] + 1) * 256;

                            if (dd > 200 * 256)
                            {
                                dd = 100 * 256;
                            }


                            sheet.SetColumnWidth(column.Ordinal, dd);
                        }


                    }
                    #endregion
                    rowIndex = intTop;
                }
                #endregion
                #region 填充内容
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    string drValue = row[column].ToString();
                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型
                            if (drValue.Length > 0)
                            {
                                DateTime dateV;
                                DateTime.TryParse(drValue, out dateV);
                                newCell.SetCellValue(dateV);
                                newCell.CellStyle = dateStyle;//格式化显示
                            }
                            else { newCell.SetCellValue(drValue); }
                            break;
                        case "System.Boolean"://布尔型
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal":
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue("");
                            break;
                    }
                }
                #endregion
                rowIndex++;
            }
        }

        /// <summary>
        /// 按指定长度创建列并带入样式
        /// </summary>
        /// <param name="hssfrow"></param>
        /// <param name="len"></param>
        /// <param name="cellstyle"></param>
        /// <returns></returns>
        public static void CreateCellsWithLength(XSSFRow hssfrow, int len, XSSFCellStyle cellstyle)
        {
            try
            {
                for (int i = 0; i < len; i++)
                {
                    hssfrow.CreateCell(i);
                    hssfrow.Cells[i].CellStyle = cellstyle;
                }
            }
            catch (Exception ce)
            {
                throw new Exception("CreateCellsWithLength:" + ce.Message);
            }
        }

        public static void ImportExcelByFileList(Dictionary<string, FileInfo> dict, string reportType, out string exportfilePath)
        {
            DataSet ds = new DataSet();
            DataTable tempDt = InitTemplateTable();
            Dictionary<string, DataTable> dicList = new Dictionary<string, DataTable>();
            string filePath = String.Empty;
            foreach (KeyValuePair<string, FileInfo> kvp in dict)
            {
                filePath = kvp.Value.DirectoryName;
                DataTable dt = new DataTable();
                ISheet sheet;
                string strFileName = kvp.Value.FullName;
                int ii = strFileName.LastIndexOf(".");
                string filetype = strFileName.Substring(ii + 1, strFileName.Length - ii - 1);
                string strOffice = kvp.Key.Split('.')[0].ToUpper();

                if ("xlsx" == filetype)
                {
                    XSSFWorkbook xssfworkbook;
                    using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        xssfworkbook = new XSSFWorkbook(file);
                    }

                    sheet = xssfworkbook.GetSheetAt(0);

                    if (xssfworkbook.Count > 3)
                    {
                        if (strOffice == "INDONESIA" && reportType == "NONTP")
                        {
                            sheet = xssfworkbook.GetSheet("COMPILATION");
                        }
                        else
                        {
                            sheet = xssfworkbook.GetSheet("Sheet1");
                        }
                    }
                }
                else
                {
                    HSSFWorkbook hssfworkbook;
                    using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        hssfworkbook = new HSSFWorkbook(file);
                    }

                    sheet = hssfworkbook.GetSheetAt(0);

                    if (hssfworkbook.Count > 3)
                    {
                        if (strOffice == "INDONESIA" && reportType == "NONTP")
                        {
                            sheet = hssfworkbook.GetSheet("COMPILATION");
                        }
                        else
                        {
                            sheet = hssfworkbook.GetSheet("Sheet1");
                        }
                    }
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

                        if (!String.IsNullOrEmpty(_cell.ToString()))
                        {
                            if (_cell.ToString().ToUpper().Trim() == "WEEK")
                            {
                                headStartNum = i;
                                break;
                            }
                        }
                    }
                }

                IRow headerRow = sheet.GetRow(headStartNum);
                int cellCount = headerRow.LastCellNum;

                for (int j = 0; j < cellCount; j++)
                {
                    ICell cell = headerRow.GetCell(j);
                    dt.Columns.Add(cell.ToString().ToUpper().Trim().Replace(" ", ""));
                }


                for (int i = (sheet.FirstRowNum + 1 + headStartNum); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row != null && row.FirstCellNum >= 0)
                    {
                        if (row.GetCell(row.FirstCellNum) != null && row.GetCell(row.FirstCellNum).ToString().Length > 0)
                        {
                            DataRow dataRow = dt.NewRow();
                            if (row.GetCell(1) != null)
                            {
                                if (!String.IsNullOrEmpty(row.GetCell(1).ToString()) && row.GetCell(1).ToString().ToUpper() != "WEEK")
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


                sheet.ForceFormulaRecalculation = true;

                
                DataTable datNew = new DataTable();

                switch (reportType)
                {
                    case "LoadingReport":
                        RenderLoadingReport(dt, strOffice);
                        datNew = dt.DefaultView.ToTable(false, ReturnOriginTemplateColumn(strOffice, reportType));
                        break;
                    case "NONTP":
                        RenderNONTPReport(dt, strOffice);
                        datNew = dt.DefaultView.ToTable(false, ReturnOriginTemplateColumn(strOffice, reportType));
                        break;
                    default:
                        dt.DefaultView.ToTable(false, ReturnOriginTemplateColumn(strOffice, reportType));
                        break;
                }

                ds.Tables.Add(datNew);
            }

            DataTable dataTable = GetAllDataTable(ds);
            tempDt.Merge(dataTable, true);

            ReturnTempDataTable(tempDt);

            string originPath = filePath + "\\" + reportType + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            ExportExcel(tempDt, "", originPath);

            exportfilePath = originPath;
        }
        private static void RenderNONTPReport(DataTable dt, string strOffice)
        {
            if (strOffice == "KOREA")
            {
                dt.Columns.Add("CONTRACT#");
            }
        }

        private static void RenderLoadingReport(DataTable dt, string strOffice)
        {

            if (strOffice == "MAL")
            {
                dt.Columns["CONTRACTNO"].ColumnName = "CONTRACT#";
            }
            else if (strOffice == "SIN")
            {
                dt.Columns.Add("MONTH");
                dt.Columns.Add("SONO");
                dt.Columns.Add("BY");
                dt.Columns.Add("SHIPPER");
                dt.Columns.Add("CONSOL");
                dt.Columns.Add("CBM");
                dt.Columns.Add("TYPE");
                dt.Columns.Add("MBLBOOKTO");

                dt.Columns["BOOKINGID"].ColumnName = "TRAFFIC";
                dt.Columns["BOOKINGSTATUS"].ColumnName = "AGENT";
                dt.Columns["DestOffice"].ColumnName = "NOMINATION";
                dt.Columns["Booked20"].ColumnName = "20";
                dt.Columns["Booked40"].ColumnName = "40";
                dt.Columns["Booked45"].ColumnName = "45";
                dt.Columns["BookedHQ"].ColumnName = "HQ";
                dt.Columns["Booked53"].ColumnName = "53";
                dt.Columns["BookedFEU"].ColumnName = "FEUS";
                dt.Columns["ContractNo"].ColumnName = "CONTRACT#";
                dt.Columns["CurrentETD"].ColumnName = "ETD";
                dt.Columns["CurrentETA"].ColumnName = "ETA";
                dt.Columns["FinalDest"].ColumnName = "DEST";
                dt.Columns["HBL"].ColumnName = "HOUSEBL#";
                dt.Columns["MBL"].ColumnName = "MASTERBL#";
                dt.Columns["ContainerList"].ColumnName = "CONTAINER#";

                foreach (DataRow dr in dt.Rows)
                {
                    dr["MONTH"] = Convert.ToDateTime(dr["BookingDate"]).ToString("MMM", CultureInfo.CreateSpecificCulture("en-GB")).ToUpper();
                    dr["TRAFFIC"] = "USA";
                    dr["AGENT"] = "641";
                    dr["BRANCH"] = "SINGAPORE";
                }
            }


            //DataTable datNew = dt.DefaultView.ToTable(false, ReturnOriginTemplateColumn(strOffice,));
        }
        private static DataTable ReturnTempDataTable(DataTable tempDt)
        {

            foreach (DataRow dr in tempDt.Rows)
            {
                double _40 = CheckNumber(dr["40"]);
                double _45 = CheckNumber(dr["45"]);
                double _HQ = CheckNumber(dr["HQ"]);
                double _20 = CheckNumber(dr["20"]);
                double _CONSOL = CheckNumber(dr["CONSOL"]);

                double _feus = (_40 + _45 + _HQ) + (_20 / 2) + (_CONSOL * -1);
                double _ttl = _feus + _CONSOL;

                dr["FEUS"] = String.Empty;
                if (_feus > 0)
                {
                    dr["FEUS"] = _feus;
                }

                dr["TTL"] = String.Empty;
                if (_ttl > 0)
                {
                    dr["TTL"] = _ttl;
                }

            }

            tempDt.Columns["PRINCIPAL"].ColumnName = "Principal";
            tempDt.Columns["NOMINATIONSALES"].ColumnName = "NOMINATION SALES";
            tempDt.Columns["CONTRACT#"].ColumnName = "CONTRACT #";
            tempDt.Columns["SERVICESTRING"].ColumnName = "SERVICE STRING";
            tempDt.Columns["SONO"].ColumnName = "SO NO";
            tempDt.Columns["MBLBOOKTO"].ColumnName = "MBL BOOK TO";
            tempDt.Columns["HOUSEBL#"].ColumnName = "HOUSEBL #";
            tempDt.Columns["MASTERBL#"].ColumnName = "MASTERBL #";
            tempDt.Columns["CONTAINER#"].ColumnName = "CONTAINER #";
            tempDt.Columns["TRADE"].ColumnName = "Trade";
            tempDt.Columns["ACCOUNT TYPE"].ColumnName = "Account Type";
            tempDt.Columns["LENTHOFHBL"].ColumnName = "LENTH OF HBL";
            tempDt.Columns["SHIPMENTCOUNT"].ColumnName = "SHIPMENT COUNT";
            tempDt.Columns["TOTALFEU"].ColumnName = "TOTAL FEU";

            return tempDt;
        }

        private static string[] ReturnOriginTemplateColumn(string strOffice, string reportType)
        {
            // TTL, 53
            string[] list = null;
            strOffice = strOffice.ToUpper();

            if ((strOffice == "INDIA" || strOffice == "KOREA" || strOffice == "PHI") && reportType == "LoadingReport")
            {
                list = new string[] {"MONTH","WEEK","BRANCH"
                                    ,"TRAFFIC","BY","AGENT","SHIPPER"
                                    ,"PRINCIPAL","CONSIGNEE" ,"NOMINATION","NOMINATIONSALES","20","40","45","HQ" ,"FEUS","CONSOL","CBM","TYPE","CARRIER" ,"SERVICESTRING"
                                    ,"SONO","VESSEL","VOYAGE","ETD","ETA","POL","DEST","MBLBOOKTO","HOUSEBL#","MASTERBL#","CONTAINER#"};
            }
            else
            {
                list = new string[] {"MONTH","WEEK","BRANCH"
                                    ,"TRAFFIC","BY","AGENT","SHIPPER"
                                    ,"PRINCIPAL","CONSIGNEE" ,"NOMINATION","NOMINATIONSALES","20","40","45","HQ" ,"FEUS","CONSOL","CBM","TYPE","CARRIER"  ,"CONTRACT#" ,"SERVICESTRING"
                                    ,"SONO","VESSEL","VOYAGE","ETD","ETA","POL","DEST","MBLBOOKTO","HOUSEBL#","MASTERBL#","CONTAINER#"};
            }

            return list;
        }

        private static string[] ReturnTemplateColumn()
        {
            string[] list = new string[] {"MONTH","WEEK","BRANCH"
            ,"TRAFFIC","BY","AGENT","SHIPPER"
            ,"PRINCIPAL","CONSIGNEE" ,"NOMINATION","NOMINATIONSALES","20","40","45","HQ","53" ,"FEUS","CONSOL","TTL","CBM","TYPE","CARRIER"  ,"CONTRACT#" ,"SERVICESTRING"
            ,"SONO","VESSEL","VOYAGE","ETD","ETA","POL","DEST","MBLBOOKTO","HOUSEBL#","MASTERBL#","CONTAINER#","TRADE","ACCOUNT TYPE","LENTHOFHBL","SHIPMENTCOUNT","TOTALFEU"};
            return list;
        }

        private static DataTable InitTemplateTable()
        {
            DataTable dt = new DataTable();

            string[] list = ReturnTemplateColumn();
            for (int i = 0; i < list.Length; i++)
            {
                dt.Columns.Add(list[i]);
            }

            return dt;
        }

        private static DataTable GetAllDataTable(DataSet ds)
        {
            DataTable newDataTable = ds.Tables[0].Clone();                //创建新表 克隆以有表的架构。
            object[] objArray = new object[newDataTable.Columns.Count];   //定义与表列数相同的对象数组 存放表的一行的值。

            try
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {
                        ds.Tables[i].Rows[j].ItemArray.CopyTo(objArray, 0);    //将表的一行的值存放数组中。
                        newDataTable.Rows.Add(objArray);                       //将数组的值添加到新表中。
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return newDataTable;                                           //返回新表。
        }

        private static DataTable UniteDataTable(DataTable dt1, DataTable dt2, string DTName = "Template")
        {
            DataTable dt3 = dt1.Clone();
            for (int i = 0; i < dt2.Columns.Count; i++)
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            object[] obj = new object[dt3.Columns.Count];
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }
            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                }
            }
            dt3.TableName = DTName; //设置DT的名字
            return dt3;
        }

        private static bool IsEmpty(Object str)
        {
            return str != null && !"".Equals(str) && !Convert.IsDBNull(str);
        }

        private static bool IsNotEmpty(string str)
        {
            return str != null && !String.IsNullOrEmpty(str);
        }

        private static string ReplaceWrap(Object str)
        {
            return CheckEmpty(str).Replace("\x0A", "<br/>").Replace("\x0D", "<br/>").Replace(" ", "&nbsp;");
        }

        private static double CheckNumber(Object str)
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

        private static string CheckEmpty(Object str)
        {
            string ret = String.Empty;

            if (IsEmpty(str))
            {
                ret = str.ToString().Trim();
            }

            return ret;
        }
    }
}
