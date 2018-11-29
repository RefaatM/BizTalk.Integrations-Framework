using DE.DAXFSA.Framework.BizTalk.Serialization;
using DE.DAXFSA.Framework.Core.Tracing;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;

namespace DE.DAXFSA.Framework.BizTalk.BRE
{
    public class MessageContextAccessorResolver
    {
        #region Fields

        private Dictionary<string, ContextProperty> contextProperties = new Dictionary<string, ContextProperty>();
        private Dictionary<string, string> resolverDictionary = new Dictionary<string, string>();

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes the Context Value collection with Message Context values being passed in.
        /// </summary>
        /// <param name="contextProperties">Collection of BizTalk Message Context values</param>
        public MessageContextAccessorResolver(Dictionary<string, object> _contextProperties)
        {
            this.contextProperties = ParseContextProperties(_contextProperties);
        }

        /// <summary>
        /// Initializes the Context Value collection with Message Context
        /// </summary>
        /// <param name="message">the IBaseMessage</param>
        public MessageContextAccessorResolver(IBaseMessage message)
        {
            // extract context properties
            Dictionary<string, object> _contextProperties = new Dictionary<string, object>();
            message.Context.ReadAll(_contextProperties);
            for (int p = 0; p < message.PartCount; p++)
            {
                string partName = string.Empty;
                var part = message.GetPartByIndex(p, out partName);
                part.PartProperties.ReadAll(_contextProperties);
            }

            this.contextProperties = ParseContextProperties(_contextProperties);
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

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sample properties:
        ///     Resolver.Successful = true|false
        ///     Resolver.ExecuteESBDispatcher = true|false
        ///     Resolver.UpdateMessageContext = true|false
        ///     Resolver.UpdateMessageContextBeforeDispatcher = true|false
        ///     Resolver.UpdateMessageContextAfterDispatcher = true|false
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        public void AddResolverProperty(string propName, string propValue)
        {
            this.resolverDictionary.Add(propName, propValue);
        }

        public Dictionary<string, string> GetResolverDictionary()
        {
            return this.resolverDictionary;
        }

        public string GetResolverProperty(string propName)
        {
            if (this.resolverDictionary.ContainsKey(propName))
                return this.resolverDictionary[propName];
            else
                return string.Empty;
        }

        private static Dictionary<string, ContextProperty> ParseContextProperties(Dictionary<string, object> _contextProperties)
        {
            var _contextPropertiesParsed = new Dictionary<string, ContextProperty>();
            foreach (var keyValuePair in _contextProperties)
            {
                string[] names = keyValuePair.Key.Split('#');
                ContextProperty prop = new ContextProperty()
                {
                    Name = names[1],
                    Namespace = names[0],
                    Value = keyValuePair.Value.ToString(),
                    DataType = keyValuePair.Value.GetType().FullName,
                };
                _contextPropertiesParsed.Add(keyValuePair.Key, prop);
            }

            return _contextPropertiesParsed;
        }

        /// <summary>
        /// Method which updates, or adds a new name value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        public void SetPropertyValue(string propName, string propNamespace, string value, bool promote)
        {
            SetPropertyValue(propName, propNamespace, value, promote, null);
        }

        /// <summary>
        /// Method which updates, or adds a new name value property into the Message Context value collection
        /// </summary>
        /// <param name="propName">Name of the property to create or update</param>
        /// <param name="propNamespace">Namespace of the property to create or update</param>
        /// <param name="value">Value of the property - name and namespace to be created or updated with</param>
        /// <param name="promote">Boolean value to determine if the property should be Property Promoted (which follows all the BizTalk Proper rules for property promotion), or distinguished. True means to property promote, which also means the namespace, name and value must adhere to the BizTalk proper rules for property promotion. This includes, an existing property schema, matching schema namespace, and 255 character limitation of a string data type, and other BizTalk Proper validation procedures.</param>
        /// <param name="dataType">.net data type (i.e. System.Boolean, System.String)</param>
        public void SetPropertyValue(string propName, string propNamespace, string value, bool promote, string dataType)
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
                    Value = value,
                    Promote = promote,
                    Dirty = true,
                };
                // add or update property in the dictionary
                if (contextProperties.ContainsKey(key) == false)
                {
                    if (!string.IsNullOrEmpty(dataType)) prop.DataType = dataType;
                    contextProperties.Add(key, prop);
                }
                else
                {
                    if (!string.IsNullOrEmpty(contextProperties[key].DataType))
                    {
                        prop.DataType = contextProperties[key].DataType;
                    }
                    else if (!string.IsNullOrEmpty(dataType)) { prop.DataType = dataType; }
                    contextProperties[key] = prop;
                }
            }
            catch (Exception ex)
            {
                TraceProvider.Logger.TraceInfo("Error setting context property {0}: {1}", key, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the current entry in the Message Context Value collection if present, otherwise it returns an empty string
        /// </summary>
        /// <param name="propName">The Name of the property to retrieve</param>
        /// <param name="propNamespace">The namespace of the property name to retrieve</param>
        /// <returns>String representation of the value</returns>
        public string GetPropertyValue(string propName, string propNamespace)
        {
            if (string.IsNullOrEmpty(propName) == true || string.IsNullOrEmpty(propNamespace) == true)
                return string.Empty;

            string key = string.Format("{0}#{1}", propNamespace, propName);
            string value = string.Empty;
            try
            {
                ContextProperty prop = null;
                if (this.contextProperties.TryGetValue(key, out prop) == true)
                {
                    value = (prop.Value != null ? prop.Value.ToString() : string.Empty);
                }
            }
            catch (Exception ex)
            {
                TraceProvider.Logger.TraceInfo("Error retrieving context property {0}: {1}", key, ex.Message);
            }
            return value;
        }

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
                if (baseMessage == null &&
                        !this.GetResolverProperty("Resolver.UpdateMessageContext")
                            .Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    return;

                foreach (var keyValuePair in this.contextProperties)
                {
                    ContextProperty prop = keyValuePair.Value;

                    // update the message context with the context properties that
                    // have been changed (dirty)
                    if (prop.Dirty == true)
                    {
                        var typedValue = (object)(string.IsNullOrEmpty(prop.DataType)
                                                    ? prop.Value
                                                    : Convert.ChangeType(prop.Value, Type.GetType(prop.DataType)));
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
                                baseMessage.Context.Promote(prop.Name, prop.Namespace, typedValue);
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
                                        baseMessage.Context.Promote(prop.Name, prop.Namespace, typedValue);
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
                            baseMessage.Context.Write(prop.Name, prop.Namespace, typedValue);
                        }
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