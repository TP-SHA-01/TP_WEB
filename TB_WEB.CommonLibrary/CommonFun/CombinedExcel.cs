using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Helpers;
using TB_WEB.CommonLibrary.Log;
using TB_WEB.CommonLibrary.Model;

namespace TB_WEB.CommonLibrary.CommonFun
{
    public static class CombinedExcel
    {
        public static void Render(RenderModel renderModel, out string outFilePath)
        {
            try
            {
                FileInfo fileInfo = renderModel.file_info;
                string filePath = fileInfo.DirectoryName;
                string fileType = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf(".") + 1, fileInfo.FullName.Length - fileInfo.FullName.LastIndexOf(".") - 1);
                string originPath = filePath + "\\" + renderModel.report_type + " Carrier Lifting Report" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileInfo.Extension;
                renderModel.file_path = originPath;
                IWorkbook workBook;

                DataSet tempds = new DataSet();
                DataSet ds = new DataSet();

                if ("XLSX" == fileType.ToUpper())
                {
                    XSSFWorkbook xssfworkbook;
                    using (FileStream file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        xssfworkbook = new XSSFWorkbook(file);
                    }

                    workBook = xssfworkbook;
                }
                else
                {
                    HSSFWorkbook hssfworkbook;
                    using (FileStream file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        hssfworkbook = new HSSFWorkbook(file);
                    }

                    workBook = hssfworkbook;
                }

                renderModel.work_book = workBook;

                ds = InitExcelData(renderModel);
                if (ds.Tables.Count > 0)
                {
                    if (renderModel.report_type != "BY WEEK")
                    {
                        tempds = GenerateExcelData(ds);
                    }
                    else
                    {
                        tempds = GenerateExcelDataByWeek(ds);
                    }

                    if (tempds.Tables.Count > 0)
                    {
                        List<string> listSheetName = new List<string>();
                        List<string> tempListSheetName = new List<string>();
                        for (int i = 0; i < tempds.Tables.Count; i++)
                        {
                            listSheetName.Add(tempds.Tables[i].TableName);
                        }
                        //listSheetName.Sort();

                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            tempListSheetName.Add(ds.Tables[i].TableName);
                        }
                        //tempListSheetName.Sort();

                        ExportModel exportModel = new ExportModel();
                        exportModel.dt_source = tempds;
                        exportModel.report_type = renderModel.report_type;
                        exportModel.file_name = renderModel.file_path;
                        exportModel.sheet_name_list = listSheetName;
                        exportModel.temp_sheet_name_list = tempListSheetName;
                        exportModel.temp_work_book = workBook;
                        exportModel.temp_work_tpye = fileType;
                        NPOIHelper.ExportExcel_USReport(exportModel);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(" CombinedExcel => Render Error Message: " + ex.Message + " , StackTrace: " + ex.StackTrace);
            }
            outFilePath = renderModel.file_path;
        }

        public static DataSet InitExcelData(RenderModel renderModel)
        {
            ISheet tempsheet;

            bool isSheetHidden = false;
            DataSet ds = new DataSet();
            IWorkbook workBook = renderModel.work_book;
            try
            {
                for (int sheetCount = 0; sheetCount < workBook.NumberOfSheets; sheetCount++)
                {
                    isSheetHidden = workBook.IsSheetHidden(sheetCount);
                    tempsheet = workBook.GetSheetAt(sheetCount);

                    if (!isSheetHidden)
                    {
                        string sheetName = tempsheet.SheetName;
                        IEnumerator rows = tempsheet.GetRowEnumerator();
                        int sheetRowCount = tempsheet.LastRowNum;
                        int headStartNum = 0;
                        DataTable dt = new DataTable(sheetName);

                        for (int i = 0; i < sheetRowCount; i++)
                        {
                            IRow _headerRow = tempsheet.GetRow(i);

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

                        IRow headerRow = tempsheet.GetRow(headStartNum);
                        int cellCount = headerRow.LastCellNum;

                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = headerRow.GetCell(j);
                            dt.Columns.Add(cell.ToString().ToUpper().Trim().Replace(" ", ""));
                        }


                        for (int i = (tempsheet.FirstRowNum + 1 + headStartNum); i <= tempsheet.LastRowNum; i++)
                        {
                            IRow row = tempsheet.GetRow(i);
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


                        foreach (DataRow dr in dt.Rows)
                        {
                            double _40 = CheckNumber(dr["40"]);
                            double _45 = CheckNumber(dr["45"]);
                            double _HQ = CheckNumber(dr["HQ"]);
                            double _20 = CheckNumber(dr["20"]);
                            double _CONSOL = CheckNumber(dr["CONSOL"]);

                            double _feus = (_40 + _45 + _HQ) + (_20 / 2) + (_CONSOL * -1);
                            double _ttl = _feus + _CONSOL;

                            if (dt.Columns.Contains("FEUS"))
                            {
                                dr["FEUS"] = String.Empty;
                                if (_feus >= 0)
                                {
                                    dr["FEUS"] = _feus;
                                }

                            }

                            if (dt.Columns.Contains("TTLFEU"))
                            {
                                dr["TTLFEU"] = String.Empty;
                                if (_ttl >= 0)
                                {
                                    dr["TTLFEU"] = _ttl;
                                }
                            }

                            if (dt.Columns.Contains("TTL"))
                            {
                                dr["TTL"] = String.Empty;
                                if (_ttl >= 0)
                                {
                                    dr["TTL"] = _ttl;
                                }
                            }
                        }

                        ds.Tables.Add(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(" CombinedExcel => InitExcelData Error Message: " + ex.Message + " , StackTrace: " + ex.StackTrace);
            }


            return ds;
        }

        public static DataSet GenerateExcelData(DataSet ds)
        {
            DataSet tempds = new DataSet();

            try
            {
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        List<RetModel> retList = new List<RetModel>();
                        DataTable tempDT = ds.Tables[i];
                        DataTable retDT = tempDT.Clone();
                        string tbName = ds.Tables[i].TableName;

                        var query = tempDT.AsEnumerable().GroupBy(t => new
                        {
                            BRANCH = t.Field<string>("BRANCH"),
                            CARRIER = t.Field<string>("CARRIER")
                        }).Select(t => new
                        {
                            BRANCH = t.Key.BRANCH,
                            CARRIER = t.Key.CARRIER,
                            TTLFEU = t.Sum(x => Convert.ToDouble(x.Field<string>("TTLFEU")))
                        });

                        if (query.ToList().Count > 0)
                        {
                            List<Dictionary<string, RetModel>> dictList = new List<Dictionary<string, RetModel>>();
                            Dictionary<string, List<RetModel>> dictModelList = new Dictionary<string, List<RetModel>>();
                            query.ToList().ForEach(q =>
                            {

                                RetModel retModel = new RetModel();
                                retModel.sort_id = SortByBranch(q.BRANCH);
                                retModel.branch = q.BRANCH;
                                retModel.carrier = q.CARRIER;
                                retModel.lb_carrier = ConvertCarrierName(q.CARRIER);
                                retModel.ttl_feu = q.TTLFEU;

                                retList.Add(retModel);
                            });

                            var sortList = retList.OrderBy(o => o.sort_id).ToList();

                            double sum_qty = 0;
                            Dictionary<string, double> dictSumList = new Dictionary<string, double>();
                            foreach (var item in sortList)
                            {
                                if (!dictModelList.ContainsKey(item.branch))
                                {
                                    var query_list = from branchModellist in retList where branchModellist.branch == item.branch select branchModellist;
                                    sum_qty = query_list.Sum(value => value.ttl_feu);


                                    dictSumList.Add(item.branch, sum_qty);
                                    dictModelList.Add(item.branch, query_list.ToList());
                                }
                            }

                            DataTable dataTable = RenderDataTable(dictModelList, dictSumList, tbName);

                            DataTable cloneDT = dataTable.Clone();

                            foreach (DataColumn col in cloneDT.Columns)
                            {
                                if (col.ColumnName == "Row Labels")
                                {
                                    col.DataType = typeof(String);
                                }
                                else
                                {
                                    col.DataType = typeof(Double);
                                }
                            }

                            foreach (DataRow row in dataTable.Rows)
                            {
                                DataRow dataRow = cloneDT.NewRow();
                                foreach (DataColumn col in dataTable.Columns)
                                {
                                    if (col.ColumnName == "Row Labels")
                                    {
                                        dataRow[col.ColumnName] = row[col.ColumnName];
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(CheckEmpty(row[col.ColumnName])))
                                        {
                                            dataRow[col.ColumnName] = Convert.ToDouble(row[col.ColumnName]);
                                        }
                                    }
                                }

                                cloneDT.Rows.Add(dataRow);
                            }

                            tempds.Tables.Add(cloneDT);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(" CombinedExcel => GenerateExcelData Error Message: " + ex.Message + " , StackTrace: " + ex.StackTrace);
            }


            return tempds;
        }

        public static DataSet GenerateExcelDataByWeek(DataSet ds)
        {
            DataSet tempds = new DataSet();

            try
            {
                if (ds.Tables.Count > 0)
                {
                    List<RetModel> retList = new List<RetModel>();
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable tempDT = ds.Tables[i];
                        DataTable retDT = tempDT.Clone();


                        var query_WEEK = tempDT.AsEnumerable().GroupBy(t => new
                        {
                            WEEK = Convert.ToInt32(t.Field<string>("WEEK"))
                        }).Select(t => new
                        {
                            WEEK = t.Key.WEEK
                        });

                        List<int> weekList = new List<int>();
                        if (query_WEEK.ToList().Count > 0)
                        {
                            query_WEEK.ToList().ForEach(q =>
                            {
                                weekList.Add(q.WEEK);
                            });
                        }
                        weekList.Sort();

                        foreach (var weekNum in weekList)
                        {
                            string tbName = "WEEK" + weekNum;
                            var query = tempDT.AsEnumerable()
                                .Where(t => t.Field<string>("WEEK").Equals(weekNum.ToString()))
                                .GroupBy(t => new
                                {
                                    BRANCH = t.Field<string>("BRANCH"),
                                    CARRIER = t.Field<string>("CARRIER")
                                })
                                .Select(t => new
                                {
                                    BRANCH = t.Key.BRANCH,
                                    CARRIER = t.Key.CARRIER,
                                    TTL = t.Sum(x => Convert.ToDouble(x.Field<string>("TTL")))
                                });

                            if (query.ToList().Count > 0)
                            {
                                List<Dictionary<string, RetModel>> dictList = new List<Dictionary<string, RetModel>>();
                                Dictionary<string, List<RetModel>> dictModelList = new Dictionary<string, List<RetModel>>();
                                query.ToList().ForEach(q =>
                                {

                                    RetModel retModel = new RetModel();
                                    retModel.sort_id = SortByBranch(q.BRANCH);
                                    retModel.branch = q.BRANCH;
                                    retModel.carrier = q.CARRIER;
                                    retModel.lb_carrier = ConvertCarrierName(q.CARRIER);
                                    retModel.ttl_feu = q.TTL;

                                    retList.Add(retModel);
                                });

                                var sortList = retList.OrderBy(o => o.sort_id).ToList();

                                double sum_qty = 0;
                                Dictionary<string, double> dictSumList = new Dictionary<string, double>();
                                foreach (var item in sortList)
                                {
                                    if (!dictModelList.ContainsKey(item.branch))
                                    {
                                        var query_list = from branchModellist in retList where branchModellist.branch == item.branch select branchModellist;
                                        sum_qty = query_list.Sum(value => value.ttl_feu);


                                        dictSumList.Add(item.branch, sum_qty);
                                        dictModelList.Add(item.branch, query_list.ToList());
                                    }
                                }

                                DataTable dataTable = RenderDataTable(dictModelList, dictSumList, tbName);

                                DataTable cloneDT = dataTable.Clone();

                                foreach (DataColumn col in cloneDT.Columns)
                                {
                                    if (col.ColumnName == "Row Labels")
                                    {
                                        col.DataType = typeof(String);
                                    }
                                    else
                                    {
                                        col.DataType = typeof(Double);
                                    }
                                }

                                foreach (DataRow row in dataTable.Rows)
                                {
                                    DataRow dataRow = cloneDT.NewRow();
                                    foreach (DataColumn col in dataTable.Columns)
                                    {
                                        if (col.ColumnName == "Row Labels")
                                        {
                                            dataRow[col.ColumnName] = row[col.ColumnName];
                                        }
                                        else
                                        {
                                            if (!String.IsNullOrEmpty(CheckEmpty(row[col.ColumnName])))
                                            {
                                                dataRow[col.ColumnName] = Convert.ToDouble(row[col.ColumnName]);
                                            }
                                        }
                                    }

                                    cloneDT.Rows.Add(dataRow);
                                }

                                tempds.Tables.Add(cloneDT);
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(" CombinedExcel => GenerateExcelData Error Message: " + ex.Message + " , StackTrace: " + ex.StackTrace);
            }

            return tempds;
        }

        public static void RenderXSSF(XSSFWorkbook xssfworkbook, string filePath, out string retFilePath)
        {

            DataSet ds = new DataSet();
            DataSet tempds = new DataSet();
            try
            {
                for (int sheetCount = 0; sheetCount < xssfworkbook.Count; sheetCount++)
                {

                    bool isSheetHidden = xssfworkbook.IsSheetHidden(sheetCount);
                    ISheet tempsheet = xssfworkbook.GetSheetAt(sheetCount);

                    if (!isSheetHidden)
                    {
                        string sheetName = tempsheet.SheetName;
                        IEnumerator rows = tempsheet.GetRowEnumerator();
                        int sheetRowCount = tempsheet.LastRowNum;
                        int headStartNum = 0;
                        DataTable dt = new DataTable(sheetName);

                        for (int i = 0; i < sheetRowCount; i++)
                        {
                            IRow _headerRow = tempsheet.GetRow(i);

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

                        IRow headerRow = tempsheet.GetRow(headStartNum);
                        int cellCount = headerRow.LastCellNum;

                        for (int j = 0; j < cellCount; j++)
                        {
                            ICell cell = headerRow.GetCell(j);
                            dt.Columns.Add(cell.ToString().ToUpper().Trim().Replace(" ", ""));
                        }


                        for (int i = (tempsheet.FirstRowNum + 1 + headStartNum); i <= tempsheet.LastRowNum; i++)
                        {
                            IRow row = tempsheet.GetRow(i);
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


                        foreach (DataRow dr in dt.Rows)
                        {
                            double _40 = CheckNumber(dr["40"]);
                            double _45 = CheckNumber(dr["45"]);
                            double _HQ = CheckNumber(dr["HQ"]);
                            double _20 = CheckNumber(dr["20"]);
                            double _CONSOL = CheckNumber(dr["CONSOL"]);

                            double _feus = (_40 + _45 + _HQ) + (_20 / 2) + (_CONSOL * -1);
                            double _ttl = _feus + _CONSOL;

                            dr["FEUS"] = String.Empty;
                            if (_feus >= 0)
                            {
                                dr["FEUS"] = _feus;
                            }

                            dr["TTLFEU"] = String.Empty;
                            if (_ttl >= 0)
                            {
                                dr["TTLFEU"] = _ttl;
                            }
                        }

                        ds.Tables.Add(dt);
                    }
                }

                if (ds.Tables.Count > 0)
                {
                    List<RetModel> retList = new List<RetModel>();
                    for (int i = 0; i < ds.Tables.Count; i++)
                    {
                        DataTable tempDT = ds.Tables[i];
                        DataTable retDT = tempDT.Clone();
                        string tbName = ds.Tables[i].TableName;

                        var query = tempDT.AsEnumerable().GroupBy(t => new
                        {
                            BRANCH = t.Field<string>("BRANCH"),
                            CARRIER = t.Field<string>("CARRIER")
                        }).Select(t => new
                        {
                            BRANCH = t.Key.BRANCH,
                            CARRIER = t.Key.CARRIER,
                            TTLFEU = t.Sum(x => Convert.ToDouble(x.Field<string>("TTLFEU")))
                        });

                        if (query.ToList().Count > 0)
                        {
                            List<Dictionary<string, RetModel>> dictList = new List<Dictionary<string, RetModel>>();
                            Dictionary<string, List<RetModel>> dictModelList = new Dictionary<string, List<RetModel>>();
                            query.ToList().ForEach(q =>
                            {

                                RetModel retModel = new RetModel();
                                retModel.sort_id = SortByBranch(q.BRANCH);
                                retModel.branch = q.BRANCH;
                                retModel.carrier = q.CARRIER;
                                retModel.lb_carrier = ConvertCarrierName(q.CARRIER);
                                retModel.ttl_feu = q.TTLFEU;

                                retList.Add(retModel);
                            });

                            var sortList = retList.OrderBy(o => o.sort_id).ToList();

                            double sum_qty = 0;
                            Dictionary<string, double> dictSumList = new Dictionary<string, double>();
                            foreach (var item in sortList)
                            {
                                if (!dictModelList.ContainsKey(item.branch))
                                {
                                    var query_list = from branchModellist in retList where branchModellist.branch == item.branch select branchModellist;
                                    sum_qty = query_list.Sum(value => value.ttl_feu);


                                    dictSumList.Add(item.branch, sum_qty);
                                    dictModelList.Add(item.branch, query_list.ToList());
                                }
                            }

                            DataTable dataTable = RenderDataTable(dictModelList, dictSumList, tbName);

                            DataTable cloneDT = dataTable.Clone();

                            foreach (DataColumn col in cloneDT.Columns)
                            {
                                if (col.ColumnName == "Row Labels")
                                {
                                    col.DataType = typeof(String);
                                }
                                else
                                {
                                    col.DataType = typeof(Double);
                                }
                            }

                            foreach (DataRow row in dataTable.Rows)
                            {
                                DataRow dataRow = cloneDT.NewRow();
                                foreach (DataColumn col in dataTable.Columns)
                                {
                                    if (col.ColumnName == "Row Labels")
                                    {
                                        dataRow[col.ColumnName] = row[col.ColumnName];
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(CheckEmpty(row[col.ColumnName])))
                                        {
                                            dataRow[col.ColumnName] = Convert.ToDouble(row[col.ColumnName]);
                                        }
                                    }
                                }

                                cloneDT.Rows.Add(dataRow);
                            }

                            tempds.Tables.Add(cloneDT);
                        }
                    }
                }

                if (tempds.Tables.Count > 0)
                {
                    List<string> listSheetName = new List<string>();

                    for (int i = 0; i < tempds.Tables.Count; i++)
                    {
                        listSheetName.Add(tempds.Tables[i].TableName);
                    }
                    //NPOIHelper.ExportExcel_USReport(tempds, listSheetName, xssfworkbook, filePath);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("CombineExcel => RenderXSSF :" + ex.Message + " StackTrace: " + ex.StackTrace);
            }
            retFilePath = filePath;
        }

        private static int SortByBranch(string pBranch)
        {
            int ret = 0;
            switch (pBranch.ToUpper())
            {
                case "DALIAN":
                    ret = 0;
                    break;
                case "FUZHOU":
                    ret = 1;
                    break;
                case "HKG/YANTIAN":
                    ret = 2;
                    break;
                case "INDIA":
                    ret = 3;
                    break;
                case "INDONESIA":
                    ret = 4;
                    break;
                case "KOREA":
                    ret = 5;
                    break;
                case "NINGBO":
                    ret = 6;
                    break;
                case "MALAYSIA":
                    ret = 7;
                    break;
                case "QINGDAO":
                    ret = 8;
                    break;
                case "SHANGHAI":
                    ret = 9;
                    break;
                case "SINGAPORE":
                    ret = 10;
                    break;
                case "THAILAND":
                    ret = 11;
                    break;
                case "TIANJIN":
                    ret = 12;
                    break;
                case "TAIWAN":
                    ret = 13;
                    break;
                case "VIETNAM":
                    ret = 14;
                    break;
                case "XIAMEN":
                    ret = 15;
                    break;
                case "PHILIPPINE":
                    ret = 16;
                    break;
            }

            return ret;

        }


        private static double CheckNumber(Object str)
        {
            double ret = 0;

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

        private static bool IsEmpty(Object str)
        {
            return str != null && !"".Equals(str) && !Convert.IsDBNull(str);
        }

        private static string ConvertCarrierName(string pCarrierName)
        {
            string retName = String.Empty;

            switch (pCarrierName.Trim().ToUpper())
            {
                case "CULV":
                    retName = "CU Line";
                    break;
                case "CMDU":
                    retName = "CMA";
                    break;
                case "COSU":
                    retName = "COSCO";
                    break;
                case "EGLV":
                    retName = "EMC";
                    break;
                case "HDMU":
                    retName = "Hyundai";
                    break;
                case "HLCU":
                    retName = "Hapag";
                    break;
                case "MAEU":
                    retName = "Maersk";
                    break;
                case "MATS":
                    retName = "Matson";
                    break;
                case "MSCU":
                    retName = "MSC";
                    break;
                case "ONEY":
                    retName = "ONE";
                    break;
                case "OOLU":
                    retName = "OOCL";
                    break;
                case "WHLC":
                    retName = "WHL";
                    break;
                case "SMLM":
                    retName = "SML";
                    break;
                case "YMLU":
                    retName = "YML";
                    break;
                case "ZIMU":
                    retName = "ZIM";
                    break;
                case "COLOADER":
                    retName = "COLOADER";
                    break;
                case "SQQY":
                    retName = "SEA-LEAD";
                    break;
                case "TJFH":
                    retName = "Transfer";
                    break;
                default:
                    break;
            }

            return retName;
        }

        private static DataTable RenderDataTable(Dictionary<string, List<RetModel>> dict, Dictionary<string, double> dictSumList, string tableName)
        {
            string[] list = new string[] { "Row Labels", "CU Line", "CMA", "COSCO", "EMC", "Hyundai", "Hapag", "Maersk", "Matson", "MSC", "ONE", "OOCL", "WHL", "SML", "YML", "ZIM", "COLOADER", "SEA-LEAD", "Transfer", "Grand Total" };

            DataTable dt = new DataTable(tableName);

            for (int i = 0; i < list.Length; i++)
            {
                dt.Columns.Add(list[i]);
            }

            foreach (var item in dict)
            {
                DataRow row = dt.NewRow();

                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] == "Row Labels")
                    {
                        row[list[i]] = item.Key;
                    }
                    else if (list[i] == "Grand Total")
                    {
                        row[list[i]] = dictSumList[item.Key];
                    }
                    else
                    {
                        foreach (var listModel in item.Value)
                        {
                            if (listModel.lb_carrier == list[i])
                            {
                                row[list[i]] = listModel.ttl_feu;
                            }
                        }
                    }
                }


                dt.Rows.Add(row);

            }

            DataRow row_sum = dt.NewRow();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == "Row Labels")
                {
                    row_sum[list[i]] = "總計";
                }
                else
                {
                    double total = dt.AsEnumerable().Select(d => Convert.ToDouble(d.Field<string>(list[i]))).Sum();

                    row_sum[list[i]] = total <= 0 ? "" : CheckEmpty(total);
                }

            }
            dt.Rows.Add(row_sum);

            return dt;
        }

        private static void RenderXSSF(XSSFWorkbook workbook, string sheetName, DataTable table, int sheetIndex)
        {
            ISheet sheet = workbook.CreateSheet();
            IDataFormat format = workbook.CreateDataFormat();
            ICellStyle dateStyle = workbook.CreateCellStyle();
            dateStyle.Alignment = HorizontalAlignment.Center;
            IFont font = workbook.CreateFont();
            font.Boldweight = 700;
            dateStyle.SetFont(font);

            dateStyle.DataFormat = format.GetFormat("MM/dd/yyyy HH:mm:ss");

            //sheet.SetAutoFilter(new CellRangeAddress(3, 0, 0, 26)); //首行筛选
            //sheet.CreateFreezePane(40, 1); //首行冻结

            IRow headerRow = sheet.CreateRow(0);
            if (!String.IsNullOrEmpty(sheetName))
            {
                workbook.SetSheetName(sheetIndex, sheetName);
            }

            ICellStyle headStyle = workbook.CreateCellStyle();
            headStyle.FillPattern = FillPattern.SolidForeground;
            headStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey40Percent.Index;

            // handling header.
            foreach (DataColumn column in table.Columns)
            {
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value
            }

            for (int i = 0; i < table.Columns.Count; i++)
            {
                headerRow.GetCell(i).CellStyle = headStyle;
            }


            sheet.ForceFormulaRecalculation = true;

            int[] arrColWidth = new int[table.Columns.Count];
            foreach (DataColumn item in table.Columns)
            {
                if (arrColWidth[item.Ordinal] >= 255)
                {
                    arrColWidth[item.Ordinal] = 120;
                }
                else
                {
                    arrColWidth[item.Ordinal] = Encoding.GetEncoding("UTF-8").GetBytes(item.ColumnName.ToString().Trim()).Length;
                }
            }
            for (int i = 0; i < table.Rows.Count; i++)
            {
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding("UTF-8").GetBytes(table.Rows[i][j].ToString().Trim()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        if (arrColWidth[j] >= 255)
                        {
                            arrColWidth[j] = 120;
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
                            if (String.IsNullOrEmpty(drValue))
                            {
                                newCell.SetCellValue("");
                            }
                            else
                            {
                                newCell.SetCellValue(drValue);
                            }
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
                        sheet.SetColumnWidth(column.Ordinal, 120);
                    }
                    else
                    {
                        sheet.SetColumnWidth(column.Ordinal, (arrColWidth[column.Ordinal] + 1) * 256);
                    }

                }

                rowIndex++;
            }
        }

    }
}
