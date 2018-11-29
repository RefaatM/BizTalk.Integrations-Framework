using DE.DAXFSA.Framework.BizTalk;
using DE.DAXFSA.Framework.BizTalk.Serialization;
using DE.DAXFSA.Framework.Core;
using DE.DAXFSA.Framework.Core.Tracing;
using DE.DAXFSA.Framework.PipelineComponents.Properties;
using Microsoft.BizTalk.Component;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DE.DAXFSA.Framework.PipelineComponents
{
    /// <summary>
    /// Pipeline component which can be placed into any receive or send
    /// pipeline stage and do a property promotion / distinguished fields
    /// writing based on arbitrary XPath.
    /// </summary>
    [System.Runtime.InteropServices.Guid("B1B5A1D0-A829-4461-9BC0-995F06FE8347")]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [Microsoft.BizTalk.Component.Interop.ComponentCategory(CategoryTypes.CATID_Any)]
    public sealed class ContextPropertyPromoter : BasePipelineComponent
    {
        #region Constants

        private const string PROPERTIES_PROP_NAME = "ContextProperties";

        #endregion Constants

        #region ExpressionData class

        /// <summary>
        /// Class used to pass the message instance to the expression evaluator.
        /// </summary>
        public class ExpressionData
        {
            private IBaseMessage message;

            public ExpressionData(IBaseMessage message)
            {
                this.message = message;
            }

            public object ReadContextValue(string propName, string propNamespace)
            {
                return this.message.Context.Read(propName, propNamespace);
            }

            public string ReadContextString(string propName, string propNamespace)
            {
                return this.message.Context.Read<string>(propName, propNamespace, string.Empty);
            }

            public int ReadContextInt(string propName, string propNamespace)
            {
                return this.message.Context.Read<int>(propName, propNamespace, 0);
            }
        }

        #endregion ExpressionData class

        #region Fields

        private static Expressions.Import[] ExpressionImports = new[]
        {
            new Expressions.Import("Convert", typeof(Convert)),
		    new Expressions.Import("Guid", typeof(Guid)),
            new Expressions.Import("DateTime", typeof(DateTime)),
		    new Expressions.Import("Math", typeof(Math)),
            new Expressions.Import("String", typeof(String)),
		    new Expressions.Import("StringExtensions", typeof(StringExtensions)),
        };

        private Expressions.ExpressionContext ExpressionContext = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public ContextPropertyPromoter()
            : base(Resources.ResourceManager, Resources.ContextPropertyPromoterName, Resources.ContextPropertyPromoterDescription, Resources.ContextPropertyPromoterVersion, Resources.ContextPropertyPromoterIcon)
        {
            this.ContextProperties = new List<ContextProperty>();
        }

        #endregion Constructors

        #region Design-time Properties

        /// <summary>
        /// Gets o sets the context value collection.
        /// </summary>
        [BtsPropertyName("ContextPropertiesPropertyName")]
        [BtsDescription("ContextPropertiesPropertyDescription")]
        [Editor(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public List<ContextProperty> ContextProperties
        {
            get;
            set;
        }

        #endregion Design-time Properties

        #region BasePipelineComponent Members

        /// <summary>
        /// Load component properties from a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="errorLog">Error log level</param>
        protected override void LoadProperties(IPropertyBag propertyBag, int errorLog)
        {
            string xml = this.ReadPropertyValue<string>(propertyBag, PROPERTIES_PROP_NAME, null);
            if (string.IsNullOrEmpty(xml) == false)
            {
                ContextPropertySerializer serializer = new ContextPropertySerializer();
                serializer.Deserialize(xml);
                this.ContextProperties = serializer.Properties;
            }
        }

        /// <summary>
        /// Save component properties to a property bag.
        /// </summary>
        /// <param name="propertyBag">Property bag</param>
        /// <param name="clearDirty">Clear dirty flag</param>
        /// <param name="saveAllProperties">Save all properties flag</param>
        protected override void SaveProperties(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {
            string xml = null;
            if (this.ContextProperties != null)
            {
                ContextPropertySerializer serializer = new ContextPropertySerializer(this.ContextProperties);
                xml = serializer.Serialize();
            }
            this.WritePropertyValue(propertyBag, PROPERTIES_PROP_NAME, xml);
        }

        /// <summary>
        /// Validates the component properties.
        /// </summary>
        /// <returns>
        /// A list of error and/or warning messages encounter during validation
        /// of this component.
        /// </returns>
        protected override List<string> Validate()
        {
            return null;
        }

        /// <summary>
        /// Promotes/writes the specified set of properties into the message context.
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <returns>Original input message</returns>
        protected override IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage)
        {
            if (this.ContextProperties != null && this.ContextProperties.Count > 0)
            {
                // set the dynamic expression context required for ContextProperty properties of type "Expression"
                ExpressionData expressionData = new ExpressionData(inputMessage);
                this.ExpressionContext = new Expressions.ExpressionContext(ExpressionImports, expressionData);

                // iterate through all context properties promoting or writing
                foreach (ContextProperty contextProperty in this.ContextProperties)
                {
                    object value = this.GetPropertyValue(inputMessage, contextProperty);
                    if (contextProperty.IgnoreNullOrEmptyValue == false || this.IsNullOrEmptyValue(value) == false)
                    {
                        if (contextProperty.Promote == true)
                        {
                            TraceProvider.Logger.TraceInfo("Promoting property into context: {0} = {1}; Namespace = {2}, Source = {3}", contextProperty.Name, value, contextProperty.Namespace, contextProperty.Source.ToString());
                            inputMessage.Context.Promote(contextProperty.Name, contextProperty.Namespace, value);
                        }
                        else
                        {
                            TraceProvider.Logger.TraceInfo("Writing property into context: {0} = {1}; Namespace = {2}, Source = {3}", contextProperty.Name, value, contextProperty.Namespace, contextProperty.Source.ToString());
                            inputMessage.Context.Write(contextProperty.Name, contextProperty.Namespace, value);
                        }
                    }
                    else
                    {
                        TraceProvider.Logger.TraceInfo("Ignoring null or empty value: {0} = {1}; Namespace = {2}, Source = {3}", contextProperty.Name, value, contextProperty.Namespace, contextProperty.Source.ToString());
                    }
                }
            }
            return inputMessage;
        }

        #endregion BasePipelineComponent Members

        #region Helpers

        /// <summary>
        /// Gets the property value based on the context property source.
        /// </summary>
        /// <param name="inputMessage">Instance of the input message.</param>
        /// <param name="contextProperty">Instance of the context property.</param>
        /// <returns>Property value.</returns>
        private object GetPropertyValue(IBaseMessage inputMessage, ContextProperty contextProperty)
        {
            // evaluate property
            object value = null;
            switch (contextProperty.Source)
            {
                case ContextPropertySource.Literal:
                    // literal value specified in the Value field
                    value = contextProperty.Value;
                    break;

                case ContextPropertySource.Context:
                    // read the value from the message context
                    value = inputMessage.Context.Read(contextProperty.Name, contextProperty.Namespace);
                    break;

                case ContextPropertySource.Expression:
                    // evaluate the expression defined by the context property
                    var expression = new Expressions.DynamicExpression(contextProperty.Value, Expressions.ExpressionLanguage.Csharp);
                    value = expression.Invoke(this.ExpressionContext);
                    break;
            }
            return value;
        }

        /// <summary>
        /// Indicates whether the specified value is null or an System.String.Empty string.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns><b>true</b> if the value parameter is null or an empty string (""); otherwise, <b>false</b>.</returns>
        private bool IsNullOrEmptyValue(object value)
        {
            if (value is string)
                return string.IsNullOrEmpty((string)value);
            else
                return (value == null);
        }

        #endregion Helpers
    }
}