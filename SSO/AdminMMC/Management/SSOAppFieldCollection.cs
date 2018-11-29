using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

using Microsoft.EnterpriseSingleSignOn.Interop;

namespace GT.BizTalk.SSO.AdminMMC.Management
{
    /// <summary>
    /// Represents a list of fields (with their values) of an SSO application.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = Constants.Namespace)]
    public class SSOAppFieldCollection : IPropertyBag, IEnumerable, IEnumerable<SSOAppField>, ICollection<SSOAppField>
    {
        #region Fields
        /// <summary>
        /// The standard generic dictionary, in most cases, offers better performance than a HybridDictionary.
        /// </summary>
        private Dictionary<string, SSOAppField> fields = new Dictionary<string, SSOAppField>();
        #endregion

        #region Methods
        /// <summary>
        /// Adds the specified field to the collection.
        /// </summary>
        /// <param name="field">Field to be added.</param>
        public void Add(SSOAppField field)
        {
            this.fields.Add(field.Name, field);
        }

        /// <summary>
        /// Removes the field with the specified name from the field collection.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        public bool Remove(string fieldName)
        {
            return this.fields.Remove(fieldName);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified field.
        /// </summary>
        /// <param name="fieldName">Field name to get or set.</param>
        /// <returns>The value associated with the specified field name.</returns>
        public SSOAppField this[string fieldName]
        {
            get { return this.fields[fieldName]; }
            set { this.fields[fieldName] = value; }
        }

        /// <summary>
        /// Gets the number of fields stored in the field collection.
        /// </summary>
        public int Count
        {
            get { return this.fields.Count; }
        }

        /// <summary>
        /// Determines whether the field conllection contains the specified field.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <returns>True if the field collection contains a field with the specified name, false otherwise.</returns>
        public bool Contains(string fieldName)
        {
            return this.fields.ContainsKey(fieldName);
        }

        /// <summary>
        /// Removes all the fields from the collection.
        /// </summary>
        public void Clear()
        {
            this.fields.Clear();
        }
        #endregion

        #region IPropertyBag Members
        /// <summary>
        /// Reads a property value from the configuration property bag.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        /// <param name="errFlag">Error flag.</param>
        void IPropertyBag.Read(string propName, out object ptrVar, int errorLog)
        {
            // initialize the output value
            ptrVar = null;
            // check if the field is in the collection to return its value
            if (this.fields.ContainsKey(propName) == true)
            {
                ptrVar = this.fields[propName].Value;
            }
        }

        /// <summary>
        /// Writes a property value.
        /// </summary>
        /// <param name="propName">Property name.</param>
        /// <param name="propValue">Property value.</param>
        void IPropertyBag.Write(string propName, ref object ptrVar)
        {
            // check if the field is in the collection
            if (this.fields.ContainsKey(propName) == true)
            {
                // the field is in the collection; update its value.
                this.fields[propName].Value = ptrVar.ToString();
            }
            else
            {
                // the field is not in the collection; add it with
                // default metadata and the specified value
                this.fields.Add(propName, new SSOAppField()
                    {
                        Ordinal = this.fields.Count,
                        Name = propName,
                        Value = ptrVar.ToString(),
                        Masked = false,
                        Identifier = string.Empty
                    });
            }
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Gets the enumerator associated to this collection.
        /// </summary>
        /// <returns>Property names enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.fields.GetEnumerator();
        }
        #endregion

        #region IEnumerable<T> Members
        IEnumerator<SSOAppField> IEnumerable<SSOAppField>.GetEnumerator()
        {
            return this.fields.Values.GetEnumerator();
        }
        #endregion

        #region ICollection Methods
        void ICollection<SSOAppField>.Add(SSOAppField item)
        {
            this.Add(item);
        }

        bool ICollection<SSOAppField>.Remove(SSOAppField item)
        {
            return this.Remove(item.Name);
        }

        void ICollection<SSOAppField>.Clear()
        {
            this.Clear();
        }

        bool ICollection<SSOAppField>.Contains(SSOAppField item)
        {
            return this.Contains(item.Name);
        }

        void ICollection<SSOAppField>.CopyTo(SSOAppField[] array, int arrayIndex)
        {
            this.fields.Values.CopyTo(array, arrayIndex);
        }

        int ICollection<SSOAppField>.Count
        {
            get { return this.Count; }
        }

        bool ICollection<SSOAppField>.IsReadOnly
        {
            get { return false; }
        }
        #endregion
    }
}
