using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace GT.BizTalk.Framework.Core.Caching
{
    /// <summary>
    /// Cache manager class.
    /// </summary>
    public static class CacheManager
    {
        #region Constants

        private const int DEFAULT_EXPIRATION_MINUTES = 30;

        #endregion Constants

        #region Fields

        private static readonly ObjectCache Cache = new MemoryCache(typeof(CacheManager).FullName);

        #endregion Fields

        #region Public methods

        /// <summary>
        /// Retrieve cached item.
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Name of cached item</param>
        /// <returns>Cached item as type</returns>
        public static T Get<T>(string key)
        {
            try
            {
                return (T)Cache[key];
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Insert value into the cache using appropriate name/value pairs.
        /// </summary>
        /// <typeparam name="T">Type of cached item.</typeparam>
        /// <param name="objectToCache">Item to be cached.</param>
        /// <param name="key">Name of item.</param>
        public static void Add<T>(string key, T objectToCache)
        {
            Cache.Add(key, objectToCache, DateTime.Now.AddMinutes(DEFAULT_EXPIRATION_MINUTES));
        }

        /// <summary>
        /// Insert value into the cache using appropriate name/value pairs.
        /// </summary>
        /// <typeparam name="T">Type of cached item.</typeparam>
        /// <param name="objectToCache">Item to be cached.</param>
        /// <param name="key">Name of item.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        public static void Add<T>(string key, T objectToCache, DateTimeOffset absoluteExpiration)
        {
            Cache.Add(key, objectToCache, absoluteExpiration);
        }

        /// <summary>
        /// Insert value into the cache using appropriate name/value pairs.
        /// </summary>
        /// <param name="objectToCache">Item to be cached.</param>
        /// <param name="key">Name of item.</param>
        public static void Add(string key, object objectToCache)
        {
            Cache.Add(key, objectToCache, DateTime.Now.AddMinutes(DEFAULT_EXPIRATION_MINUTES));
        }

        /// <summary>
        /// Insert value into the cache using appropriate name/value pairs.
        /// </summary>
        /// <param name="objectToCache">Item to be cached.</param>
        /// <param name="key">Name of item.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        public static void Add(string key, object objectToCache, DateTimeOffset absoluteExpiration)
        {
            Cache.Add(key, objectToCache, absoluteExpiration);
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">Name of cached item</param>
        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Checks for an item in the cache.
        /// </summary>
        /// <param name="key">Name of cached item</param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        /// <summary>
        /// Gets all the cached items as a list.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAll()
        {
            return Cache.Select(keyValuePair => keyValuePair.Key).ToList();
        }

        #endregion Public methods
    }
}