using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Date;

namespace WebApi.Edi.Topocean.EdiModels.Common
{
    public class BaseModel
    {
        protected DataTable baseDataTable { set; get; }

        public void SetModelValueTmp()
        {
            Type tt = this.GetType();
            PropertyInfo[] fields = this.GetType().GetProperties();
            //IList<T> ts = new List<T>();
            foreach (DataRow dr in baseDataTable.Rows)
            {
                foreach (DataColumn dc in baseDataTable.Columns)
                {
                    foreach (PropertyInfo t in fields)
                    {
                        if (t.CanWrite)
                        {
                            if (dc.ColumnName == t.Name)
                            {
                                object drValue = dr[dc.ColumnName];
                                if (drValue != null && dr[dc.ColumnName] != DBNull.Value)
                                {
                                    t.SetValue(this, drValue);
                                }
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public void SetModelValue()
        {
            SetModelValue(0);
        }
        
        public void SetModelValue(int index)
        {
            try
            {
                Type tt = this.GetType();
                PropertyInfo[] fields = this.GetType().GetProperties();
                //IList<T> ts = new List<T>();
                DataRow dr = baseDataTable.Rows[index];
                foreach (DataColumn dc in baseDataTable.Columns)
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
                                    switch (dr[dc.ColumnName].GetType().Name) {
                                        case "DateTime":
                                            DateTime tmpDateTime = (DateTime)Convert.ChangeType(dr[dc.ColumnName], dr[dc.ColumnName].GetType());
                                            if (tmpDateTime > (DateTime.MinValue.AddYears(1970)))
                                            {
                                                if (t.GetType().Name != "DateTime")
                                                {
                                                    t.SetValue(this, DateFormatHelper.DisplayDate(tmpDateTime));
                                                }
                                                else
                                                {
                                                    t.SetValue(this, Convert.ChangeType(drValue, t.GetType()));
                                                }
                                            }
                                            break;

                                        default:
                                            t.SetValue(this, drValue);
                                            break;
                                    }
                                    
                                }
                                continue;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                //CommonFunc.SaveLog(ex.ToString(), "E");
            }
           
        }

    }
}
