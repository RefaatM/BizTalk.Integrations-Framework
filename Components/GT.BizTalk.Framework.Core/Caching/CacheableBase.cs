using System;

namespace GT.BizTalk.Framework.Core.Caching
{
    /// <summary>
    /// Base class for cacheable lists.
    /// </summary>
    /// <typeparam name="T">Object class type.</typeparam>
    [Serializable]
    public abstract class CacheableBase<T>
    {
        #region Fields

        private volatile object SyncObj = new object();

        #endregion Fields

        #region Overridable Methods

        /// <summary>
        /// Base classes must override this method to load the data from the underlying repository.
        /// </summary>
        /// <param name="key">Key value used to identify the object.</param>
        /// <returns>Reference to the data loaded from the repository.</returns>
        protected abstract T Load(string key);

        #endregion Overridable Methods

        #region Protected Methods

        /// <summary>
        /// Gets the object from the cache. If the object is not in the cache, then it loads it from
        /// the underlying repository.
        /// </summary>
        /// <returns>The reference to the cached object.</returns>
        protected T Get()
        {
            return this.Get(null);
        }

        /// <summary>
        /// Gets the object from the cache. If the object is not in the cache, then it loads it from
        /// the underlying repository.
        /// </summary>
        /// <param name="key">Key name of the cache entry holding the object.</param>
        /// <returns>The reference to the cached object.</returns>
        protected T Get(string key)
        {
            // check if object is in the cache
            string cacheKey = this.GetCacheKey(key);
            if (CacheManager.Contains(cacheKey) == false)
            {
                // not in the cache, get a lock and load it
                lock (this.SyncObj)
                {
                    // check again in case the object was loaded in another
                    // request while we were waiting for the lock
                    if (CacheManager.Contains(cacheKey) == false)
                    {
                        // load the object from the underlying repository
                        T obj = this.Load(key);
                        // add list to the cache
                        CacheManager.Add(cacheKey, obj);
                    }
                }
            }
            // return the object instance
            return CacheManager.Get<T>(cacheKey);
        }

        /// <summary>
        /// Removes a object from the cache.
        /// </summary>
        /// <param name="key">Key name of the cache entry holding the object.</param>
        protected void Remove(string key)
        {
            // the object is in the cache, get a lock and remove it
            string cacheKey = this.GetCacheKey(key);
            lock (this.SyncObj)
            {
                // remove it
                CacheManager.Remove(cacheKey);
            }
        }

        #endregion Protected Methods

        #region Helpers

        private string GetCacheKey(string key)
        {
            Type type = typeof(T);
            return (string.IsNullOrEmpty(key) == true ? type.FullName : string.Format("{0}.{1}", type.FullName, key));
        }

        #endregion Helpers
    }
}