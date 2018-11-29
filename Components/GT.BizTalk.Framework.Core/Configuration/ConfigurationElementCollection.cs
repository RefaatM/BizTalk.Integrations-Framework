using System;
using System.Configuration;

namespace GT.BizTalk.Framework.Core.Configuration
{
    /// <summary>
    /// Generic implementation of the <see cref="ConfigurationElementCollection"/> that provides
    /// a strongly-typed collection of configuration elements.
    /// </summary>
    /// <typeparam name="K">Type of the collection key.</typeparam>
    /// <typeparam name="V">Type of the elements controlled by the collection.</typeparam>
    [Serializable]
    public abstract class ConfigurationElementCollection<K, V> : ConfigurationElementCollection
        where V : ConfigurationElement, new()
    {
        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public ConfigurationElementCollection()
        {
        }

        #endregion Constructor

        #region Overridable Methods

        /// <summary>
        /// Gets a flag indicating an exception should be thrown if a duplicate element
        /// is added to the collection.
        /// </summary>
        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new element of the type specified for this collection.
        /// </summary>
        /// <returns>A new element of type V.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new V();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The <see cref="System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>An <see cref="System.Object"/> that acts as the key for the specified <see cref="System.Configuration.ConfigurationElement"/>.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return this.GetElementKey((V)element);
        }

        /// <summary>
        /// Gets the strongly-typed element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The configuration element of type V to return the key for.</param>
        /// <returns>The strongly-typed key for the specified configuration element.</returns>
        protected abstract K GetElementKey(V element);

        #endregion Overridable Methods

        #region Public Methods

        /// <summary>
        /// Adds a configuration element to the collection.
        /// </summary>
        /// <param name="element">Element to add.</param>
        public void Add(V element)
        {
            this.BaseAdd(element);
        }

        /// <summary>
        /// Removes a configuration element from the collection.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        public void Remove(K key)
        {
            this.BaseRemove(key);
        }

        /// <summary>
        /// Returns the index of an item with the specified key in the collection.
        /// </summary>
        /// <param name="key">The key of the item to find.</param>
        /// <returns>Returns the index of the item with the key.</returns>
        public int IndexOf(K key)
        {
            return this.IndexOf((V)this.BaseGet(key));
        }

        /// <summary>
        /// Returns the index of a specified item in the collection.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>Returns the index of the item.</returns>
        public int IndexOf(V item)
        {
            if (item != null)
                return this.BaseIndexOf(item);
            else
                return -1;
        }

        /// <summary>
        /// Returns the configuration element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to return.</param>
        /// <returns>The configuration element with the specified key; otherwise null.</returns>
        public V this[K key]
        {
            get { return (V)this.BaseGet(key); }
        }

        /// <summary>
        /// Gets the configuration element at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the configuration element to return.</param>
        /// <returns>The configuration element at the specified index.</returns>
        public V this[int index]
        {
            get { return (V)this.BaseGet(index); }
        }

        /// <summary>
        /// Gets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <typeparam name="T">The type of the property to access.</typeparam>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <returns>The specified property, attribute, or child element.</returns>
        protected T GetPropertyValue<T>(string propertyName)
        {
            return (T)base[propertyName];
        }

        /// <summary>
        /// Sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <typeparam name="T">The type of the property to access.</typeparam>
        /// <param name="propertyName">The name of the property to access.</param>
        /// <param name="propertyValue">The value of the property.</param>
        protected void SetPropertyValue<T>(string propertyName, T propertyValue)
        {
            base[propertyName] = propertyValue;
        }

        #endregion Public Methods
    }
}