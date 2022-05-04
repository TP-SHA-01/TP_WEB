using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Edi.Common
{
    public class Paging<T>
    {
        public List<T> DataSource = new List<T>();

        public int PageIndex { get; set; }
        public int AllPage { get; set; }
        public int EachCount { get; set; }

        public Paging(List<T> dataSource, int eachCount)
        {
            this.DataSource = dataSource;
            this.AllPage = (int)Math.Ceiling((double)this.DataSource.Count / eachCount);
            this.EachCount = eachCount;
            this.PageIndex = 1;
        }

        public List<T> Next()
        {
            PageIndex++;
            if (this.PageIndex > AllPage)
                PageIndex--;
            return GetPage();
        }

        public List<T> Provious()
        {
            PageIndex--;
            if (this.PageIndex == 0)
                PageIndex++;
            return GetPage();
        }

        public List<T> Fist()
        {
            this.PageIndex = 1;
            return GetPage();
        }

        public List<T> End()
        {
            this.PageIndex = this.AllPage;
            return GetPage();
        }

        public List<T> GoTo(int index)
        {
            if (index <= 0)
                index = 1;
            if (index >= AllPage)
                index = AllPage;
            this.PageIndex = index;
            return GetPage();
        }

        private List<T> GetPage()
        {
            return this.DataSource.Skip((PageIndex - 1) * EachCount).Take(EachCount).ToList();
        }

        public List<T> GetPerPage(int PageIndex)
        {
            return this.DataSource.Skip(PageIndex * EachCount).Take(EachCount).ToList();
        }
    }
}
