using DE.DAXFSA.Framework.BizTalk.Serialization;
using DE.DAXFSA.Framework.Core.Tracing;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.BizTalk.BRE
{
    /// <summary>
    /// Class used in the BRE Context Resolver pipeline component, BRE Resolver Provider,
    /// and Business Rules Composer to create,  update and read BizTalk Message Context Property
    /// name and value information.
    /// </summary>
    public class MessageContextAccessorFact
    {
        #region Fields

        private Dictionary<string, ContextProperty> contextProperties = new Dictionary<string, ContextProperty>();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes the Context Value collection with Message Context values being passed in.
        /// </summary>
        /// <param name="contextProperties">Collection of BizTalk Message Context values</param>
        public MessageContextAccessorFact(Dictionary<string, object> contextProperties)
        {
            foreach (var keyValuePair in contextProperties)
            {
                string[] names = keyValuePair.Key.Split('#');
                ContextProperty prop = new ContextProperty()
                {
                    Name = names[1],
                    Namespace = names[0],
                    Value = keyValuePair.Value.ToString()
                };
                this.contextProperties.Add(keyValuePair.Key, prop);
            }
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets/sets a value indicating whether to update the message context with the current values.
        /// </summary>
        public bool UpdateMessageContext
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets a value indicating whether to auto-set the document spec name when updating the message context.
        /// </summary>
        public bool AutoSetDocumentSpecName
        {
            get;
            set;
        }

        #endregion Properties

        #region GetProperty Overloads

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        [Obsolete("Maintained for backward compatibility purposes only. Use one of the GetPropertyXXX overloads or the generic version instead.")]
        public string GetPropertyValue(string propName, string propNamespace)
        {
            return this.GetProperty<string>(propName, propNamespace, string.Empty);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public string GetPropertyString(string propName, string propNamespace, string defaultValue)
        {
            return this.GetProperty<string>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public int GetPropertyInt32(string propName, string propNamespace, int defaultValue)
        {
            return this.GetProperty<int>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public long GetPropertyInt64(string propName, string propNamespace, long defaultValue)
        {
            return this.GetProperty<long>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public float GetPropertyFloat(string propName, string propNamespace, float defaultValue)
        {
            return this.GetProperty<float>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public double GetPropertyDouble(string propName, string propNamespace, double defaultValue)
        {
            return this.GetProperty<double>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public decimal GetPropertyDecimal(string propName, string propNamespace, decimal defaultValue)
        {
            return this.GetProperty<decimal>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public DateTime GetPropertyDateTime(string propName, string propNamespace, DateTime defaultValue)
        {
            return this.GetProperty<DateTime>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public bool GetPropertyBoolean(string propName, string propNamespace, bool defaultValue)
        {
            return this.GetProperty<bool>(propName, propNamespace, defaultValue);
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns the default value.
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <param name="defaultValue">Default value returned if the conversion cannot be performed.</param>
        /// <returns>Value of the property.</returns>
        public T GetProperty<T>(string propName, string propNamespace, T defaultValue)
        {
            if (string.IsNullOrEmpty(propName) == true || string.IsNullOrEmpty(propNamespace) == true)
                return defaultValue;

            string key = string.Format("{0}#{1}", propNamespace, propName);
            try
            {
                ContextProperty prop = null;
                if (this.contextProperties.TryGetValue(key, out prop) == true)
                {
                    if (prop.Value != null)
                    {
                        if (typeof(T).IsEnum == true)
                        {
                            return (T)Enum.Parse(typeof(T), prop.Value.ToString());
                        }
                        else
                        {
                            return (T)Convert.ChangeType(prop.Value, typeof(T));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceProvider.Logger.TraceInfo("Error retrieving context property {0}: {1}", key, ex.Message);
            }
            // return default value, if property cannot be read
            return defaultValue;
        }

        #endregion GetProperty Overloads

        #region SetProperty Overloads

        /// <summary>
        /// Method which updates, or adds a new String value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">String value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        [Obsolete("Maintained for backward compatibility purposes only. Use one of the SetPropertyXXX overloads or the generic version instead.")]
        public void SetPropertyValue(string propName, string propNamespace, string value, bool promote)
        {
            this.SetProperty<string>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new String value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">String value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyString(string propName, string propNamespace, string value, bool promote)
        {
            this.SetProperty<string>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new Int32 value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Int32 value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyInt32(string propName, string propNamespace, int value, bool promote)
        {
            this.SetProperty<int>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new Int64 value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Int64 value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyInt64(string propName, string propNamespace, long value, bool promote)
        {
            this.SetProperty<long>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new Float value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Float value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyFloat(string propName, string propNamespace, float value, bool promote)
        {
            this.SetProperty<float>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new Double value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Double value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyDouble(string propName, string propNamespace, double value, bool promote)
        {
            this.SetProperty<double>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new Decimal value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Decimal value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyDecimal(string propName, string propNamespace, decimal value, bool promote)
        {
            this.SetProperty<decimal>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new DateTime value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">DateTime value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyDateTime(string propName, string propNamespace, DateTime value, bool promote)
        {
            this.SetProperty<DateTime>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new Boolean value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Boolean value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyBoolean(string propName, string propNamespace, bool value, bool promote)
        {
            this.SetProperty<bool>(propName, propNamespace, value, promote);
        }

        /// <summary>
        /// Method which updates, or adds a new name value property into the Message Context value collection
        /// </summary>
        /// <typeparam name="T">Type of property value.</typeparam>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetProperty<T>(string propName, string propNamespace, T value, bool promote)
        {
            if (string.IsNullOrEmpty(propName) == true || string.IsNullOrEmpty(propNamespace) == true)
                return;

            string key = string.Format("{0}#{1}", propNamespace, propName);
            try
            {
                // create a context property
                ContextProperty prop = new ContextProperty()
                {
                    Name = propName,
                    Namespace = propNamespace,
                    Value = value.ToString(),
                    Promote = promote,
                    Dirty = true
                };
                // add or update property in the dictionary
                if (contextProperties.ContainsKey(key) == false)
                {
                    contextProperties.Add(key, prop);
                }
                else
                {
                    contextProperties[key] = prop;
                }
            }
            catch (Exception ex)
            {
                TraceProvider.Logger.TraceInfo("Error setting context property {0}: {1}", key, ex.Message);
            }
        }

        #endregion SetProperty Overloads

        #region Methods

        /// <summary>
        /// Gets a dictionary collection of the context properties that have been changed.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> GetChangedValues()
        {
            Dictionary<string, object> changedContextProp = new Dictionary<string, object>();
            foreach (var keyValuePair in this.contextProperties)
            {
                ContextProperty prop = keyValuePair.Value;
                if (prop.Dirty == true)
                {
                    changedContextProp.Add(keyValuePair.Key, prop.Value);
                }
            }
            return changedContextProp;
        }

        /// <summary>
        /// Updates the BizTalk BaseMessage and Message Context with any new or modified values from the executed BRE Policies.
        /// </summary>
        /// <param name="pipelineContext">PipelineContext</param>
        /// <param name="baseMessage">BizTalk BaseMessage to update</param>
        /// <param name="allowRepromotion">Value indicating whether to allow to re-promote properties.</param>
        public void ApplyMessageContextUpdates(IPipelineContext pipelineContext, IBaseMessage baseMessage, bool allowRepromotion)
        {
            try
            {
                if (baseMessage == null || this.UpdateMessageContext == false)
                    return;

                foreach (var keyValuePair in this.contextProperties)
                {
                    ContextProperty prop = keyValuePair.Value;

                    // update the message context with the context properties that
                    // have been changed (dirty)
                    if (prop.Dirty == true)
                    {
                        // check to determine if we should promote
                        if (prop.Promote == true)
                        {
                            // however check to see if already promoted or not
                            bool isAlreadyPromoted = false;
                            var ovalue = baseMessage.Context.Read(prop.Name, prop.Namespace);
                            if (ovalue != null)
                            {
                                isAlreadyPromoted = baseMessage.Context.IsPromoted(prop.Name, prop.Namespace);
                            }

                            if (isAlreadyPromoted == true)
                            {
                                // we need to remove and re - promote
                                baseMessage.Context.Write(prop.Name, prop.Namespace, null);
                                baseMessage.Context.Promote(prop.Name, prop.Namespace, null);
                                baseMessage.Context.Promote(prop.Name, prop.Namespace, prop.Value);
                            }
                            else
                            {
                                // it's not already promoted and we should promote if we can,
                                // this assumes there is a valid property schema, name, and data type associated with it for promotion validation...
                                // dangerous operation which could cause cyclic loop by re-promoting a property that was slated to be demoted *wasPromote*...
                                if (allowRepromotion == true)
                                {
                                    try
                                    {
                                        baseMessage.Context.Write(prop.Name, prop.Namespace, null);
                                        baseMessage.Context.Promote(prop.Name, prop.Namespace, null);
                                        baseMessage.Context.Promote(prop.Name, prop.Namespace, prop.Value);
                                    }
                                    catch (Exception ex)
                                    {
                                        TraceProvider.Logger.TraceError("Context property: {0}#{1} caused an exception:\n{2}\nThis property was not promoted.",
                                            prop.Namespace, prop.Name, ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // we don't need to promote it, only write it (Distinguished)
                            // we need to remove and re-write it
                            baseMessage.Context.Write(prop.Name, prop.Namespace, null);
                            baseMessage.Context.Write(prop.Name, prop.Namespace, prop.Value);
                        }
                    }
                }

                if (this.AutoSetDocumentSpecName == true)
                {
                    // set document spec name based on the message type
                    string messageType = baseMessage.Context.Read<string>(BtsProperties.MessageType.Name, BtsProperties.MessageType.Namespace, null);
                    if (string.IsNullOrEmpty(messageType) == false)
                    {
                        IDocumentSpec documentSpec = pipelineContext.GetDocumentSpecByType(messageType);
                        TraceProvider.Logger.TraceInfo("Using document specification: {0}", documentSpec.DocSpecName);
                        // write document spec type name to the message context so that the Flat File disassembler could access this
                        // property and do the message processing for a schema that has the document spec type name we've discovered
                        baseMessage.Context.Write(BtsProperties.DocumentSpecName.Name, BtsProperties.DocumentSpecName.Namespace, documentSpec.DocSpecStrongName);
                    }
                }

                pipelineContext.ResourceTracker.AddResource(baseMessage.Context);
            }
            catch (Exception ex)
            {
                TraceProvider.Logger.TraceError(ex);
                throw;
            }
        }

        #endregion Methods
    }
}