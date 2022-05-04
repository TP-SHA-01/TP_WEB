using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using TB_WEB.CommonLibrary.Date;

namespace WebApi.Edi.Topocean.Common
{
    public class ModelConvertHelper
    {
        public T SetModelValue<T>(T baseDao, DataTable dt)
        {
            Type tt = typeof(T);
            PropertyInfo[] fields = baseDao.GetType().GetProperties();
            //IList<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    foreach (PropertyInfo t in fields)
                    {
                        if (t.CanWrite)
                        {
                            if (dc.ColumnName == t.Name)
                            {
                                object drValue = dr[dc.ColumnName];
                                if (drValue != null && !Convert.IsDBNull(drValue))
                                {
                                    t.SetValue(baseDao, drValue);
                                }
                                continue;
                            }
                        }
                    }
                }
            }

            return baseDao;
        }


        public T SetModelValue<T>(T baseDao, DataTable dt, string type)
        {
            Type tt = typeof(T);
            PropertyInfo[] fields = baseDao.GetType().GetProperties();
            //IList<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    foreach (PropertyInfo t in fields)
                    {
                        if (t.CanWrite)
                        {
                            if (dc.ColumnName == type + "_" + t.Name)
                            {
                                object drValue = dr[dc.ColumnName];
                                if (drValue != null && !Convert.IsDBNull(drValue))
                                {
                                    t.SetValue(baseDao, drValue);
                                }
                                continue;
                            }
                        }
                    }
                }
            }

            return baseDao;
        }

        public string[] retDataTableCol<T>(T baseDao)
        {
            Type tt = typeof(T);
            PropertyInfo[] fields = baseDao.GetType().GetProperties();

            List<string> strList = new List<string>();
            foreach (PropertyInfo t in fields)
            {
                strList.Add(t.Name);
            }

            return strList.ToArray();
        }

        public string[] retDataTableCol<T>(T baseDao, string type)
        {
            Type tt = typeof(T);
            PropertyInfo[] fields = baseDao.GetType().GetProperties();

            List<string> strList = new List<string>();
            foreach (PropertyInfo t in fields)
            {
                strList.Add(type + "_" + t.Name);
            }

            return strList.ToArray();
        }

        /// <summary>
        /// Convert DataTable to Model
        /// </summary>
        /// <typeparam name="T">Model T </typeparam>
        /// <param name="table">datatable</param>
        /// <returns></returns>
        public static IList<T> DataTableToList<T>(DataTable table) where T : class
        {
            if (!IsHaveRows(table))
                return new List<T>();

            IList<T> list = new List<T>();
            T model = default(T);
            foreach (DataRow dr in table.Rows)
            {
                model = Activator.CreateInstance<T>();

                foreach (DataColumn dc in dr.Table.Columns)
                {
                    object drValue = dr[dc.ColumnName];
                    PropertyInfo pi = model.GetType().GetProperty(dc.ColumnName);

                    if (pi != null && pi.CanWrite && (drValue != null && !Convert.IsDBNull(drValue)))
                    {
                        switch (drValue.GetType().Name)
                        {
                            case "DateTime":
                                DateTime tmpDateTime = (DateTime)Convert.ChangeType(dr[dc.ColumnName], dr[dc.ColumnName].GetType());
                                if (tmpDateTime > (DateTime.MinValue.AddYears(1970)))
                                {
                                    if (pi.GetType().Name != "DateTime")
                                    {
                                        pi.SetValue(model, DateFormatHelper.DisplayDate(tmpDateTime));
                                    }
                                    else
                                    {
                                        pi.SetValue(model, Convert.ChangeType(drValue, pi.GetType()));
                                    }
                                }
                                break;

                            default:
                                pi.SetValue(model, drValue);
                                break;
                        }
                        //pi.SetValue(model, drValue, null);
                    }
                }

                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// Model List Convert to DataTable
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="list"> Model List</param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IList<T> list)
            where T : class
        {
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            DataTable dt = new DataTable(typeof(T).Name);
            DataColumn column;
            DataRow row;

            PropertyInfo[] myPropertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            int length = myPropertyInfo.Length;
            bool createColumn = true;

            foreach (T t in list)
            {
                if (t == null)
                {
                    continue;
                }

                row = dt.NewRow();
                for (int i = 0; i < length; i++)
                {
                    PropertyInfo pi = myPropertyInfo[i];
                    string name = pi.Name;
                    if (createColumn)
                    {
                        column = new DataColumn(name, pi.PropertyType);
                        dt.Columns.Add(column);
                    }

                    row[name] = pi.GetValue(t, null);
                }

                if (createColumn)
                {
                    createColumn = false;
                }

                dt.Rows.Add(row);
            }
            return dt;

        }

        /// <summary>
        /// Convert generic collection class to DataTable
        /// </summary>
        /// <typeparam name="T">Collection item type</typeparam>
        /// <param name="list">List</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(IList<T> list)
        {
            return ToDataTable<T>(list, null);
        }

        /// <summary>
        /// Convert generic collection class to DataTable
        /// </summary>
        /// <typeparam name="T">Collection item type</typeparam>
        /// <param name="list">List</param>
        /// <param name="propertyName">the RowNames should be return</param>
        /// <returns>DataTable</returns>
        public static DataTable ToDataTable<T>(IList<T> list, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);

            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                        {
                            result.Columns.Add(pi.Name, pi.PropertyType);
                        }
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        // <summary>
        /// Check DataTable has rows
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static bool IsHaveRows(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
                return true;

            return false;
        }
    }
}