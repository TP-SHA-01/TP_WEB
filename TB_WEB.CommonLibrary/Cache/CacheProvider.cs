using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Cache
{
    /// <summary>
    /// Cache providing mode, use Redis or MemoryCache
    /// </summary>
    public class CacheProvider
    {
        private bool _isUseRedis;

        private string _connectionString;
        private string _instanceName;
        /// <summary>
        /// Whether to use Redis
        /// </summary>
        public bool IsUseRedis { get => _isUseRedis; set => _isUseRedis = value; }
        /// <summary>
        /// Redis connection
        /// </summary>
        public string ConnectionString { get => _connectionString; set => _connectionString = value; }
        /// <summary>
        /// Redis instance name
        /// </summary>
        public string InstanceName { get => _instanceName; set => _instanceName = value; }
    }
}
