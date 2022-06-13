using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.CommonFun
{
    [Serializable]
    public class CacheData
    {
        /// <summary>
        /// 键
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public object value { get; set; }
        /// <summary>
        /// 缓存更新时间
        /// </summary>
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 过期时间(秒)，0表示永不过期
        /// </summary>
        public int expirationSeconds { get; set; }

        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        public CacheData(string key, object value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
