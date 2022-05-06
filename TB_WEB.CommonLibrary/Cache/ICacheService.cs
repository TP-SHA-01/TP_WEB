using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TB_WEB.CommonLibrary.Cache
{
    public interface ICacheService
    {
        #region Verify cache entry exists
        /// <summary>
        /// Verify that the cache item exists
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        bool Exists(string key);


        #endregion

        #region add cache
        /// <summary>
        /// Add cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">Cache Value</param>
        /// <returns></returns>
        bool Add(string key, object value);


        /// <summary>
        /// Add cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">Cache Value</param>
        /// <param name="expiresSliding">Sliding expiration time (if there is an operation within the expiration time, the expiration time will be extended with the current time point)</param>
        /// <param name="expiressAbsoulte">absolute expiration time</param>
        /// <returns></returns>
        bool Add(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte);


        /// <summary>
        /// Add cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">Cache Value</param>
        /// <param name="expiresIn">cache duration</param>
        /// <param name="isSliding">Whether sliding expires (if there is an operation within the expiration time, the expiration time will be extended with the current time point)</param>
        /// <returns></returns>
        bool Add(string key, object value, TimeSpan expiresIn, bool isSliding = false);

        #endregion

        #region delete cache
        /// <summary>
        /// delete the cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// Batch delete cache
        /// </summary>
        /// <param name="keys">Cache Key Collection</param>
        /// <returns></returns>
        void RemoveAll(IEnumerable<string> keys);


        /// <summary>
        /// Use wildcards to find all keys and delete them one by one
        /// </summary>
        /// <param name="pattern">wildcard</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// delete all caches
        /// </summary>
        void RemoveCacheAll();
        #endregion

        #region get cache
        /// <summary>
        /// Get the cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;


        /// <summary>
        /// Get the cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <returns></returns>
        object Get(string key);


        /// <summary>
        /// Get the cache collection
        /// </summary>
        /// <param name="keys">Cache Key Collection</param>
        /// <returns></returns>
        IDictionary<string, object> GetAll(IEnumerable<string> keys);

        #endregion

        #region modify cache
        /// <summary>
        /// Modify the cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">New cached Value</param>
        /// <returns></returns>
        bool Replace(string key, object value);


        /// <summary>
        /// Modify the cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">New cached Value</param>
        /// <param name="expiresSliding">Sliding expiration time (if there is an operation within the expiration time, the expiration time will be extended with the current time point)</param>
        /// <param name="expiressAbsoulte">absolute expiration time</param>
        /// <returns></returns>
        bool Replace(string key, object value, TimeSpan expiresSliding, TimeSpan expiressAbsoulte);


        /// <summary>
        /// Modify the cache
        /// </summary>
        /// <param name="key">Cache Key</param>
        /// <param name="value">New cached Value</param>
        /// <param name="expiresIn">cache duration</param>
        /// <param name="isSliding">Whether sliding expires (if there is an operation within the expiration time, the expiration time will be extended with the current time point)</param>
        /// <returns></returns>
        bool Replace(string key, object value, TimeSpan expiresIn, bool isSliding = false);

        #endregion

    }
}
