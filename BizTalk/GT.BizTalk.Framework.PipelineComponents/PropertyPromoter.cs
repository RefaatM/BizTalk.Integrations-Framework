using GT.BizTalk.Framework.BizTalk;
using GT.BizTalk.Framework.BizTalk.Pipeline;
using GT.BizTalk.Framework.BizTalk.Serialization;
using GT.BizTalk.Framework.Core;
using GT.BizTalk.Framework.Core.Tracing;
using GT.BizTalk.Framework.Core.Utilities;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.Streaming;
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace GT.BizTalk.Framework.PipelineComponents
{
    /// <summary>
    ///
    /// </summary>
    public class PropertyPromoter : IIBaseMessageProcessor
    {
        #region private properties

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

        #endregion private properties

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public PropertyPromoter()
        {
        }

        #endregion Constructor

        #region Implementations

        public string Name
        {
            get { return "PropertyPromoter"; }
        }

        public string Description
        {
            get { return "Context Property Prmoter"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        /// <summary>
        /// Gets the property value based on the context property source.
        /// </summary>
        /// <param name="inputMessage">Instance of the input message.</param>
        /// <param name="contextProperty">Instance of the context property.</param>
        /// <returns>Property value.</returns>
        private object GetPropertyValue(IPipelineContext pipelineContext, IBaseMessage inputMessage, GT.BizTalk.Framework.BizTalk.Serialization.ContextProperty contextProperty)
        {
            // evaluate property
            object value = null;
            switch (contextProperty.Source)
            {
                case GT.BizTalk.Framework.BizTalk.Serialization.ContextPropertySource.Literal:
                    // literal value specified in the Value field
                    value = Convert.ChangeType(contextProperty.Value, Type.GetType(contextProperty.DataType));
                    break;

                case GT.BizTalk.Framework.BizTalk.Serialization.ContextPropertySource.Context:
                    // read the value from the message context
                    //value = inputMessage.Context.Read(contextProperty.Name, contextProperty.Namespace);
                    var nns = contextProperty.Value.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    value = inputMessage.Context.Read(nns[1], nns[0]);
                    break;

                case GT.BizTalk.Framework.BizTalk.Serialization.ContextPropertySource.Expression:
                    // evaluate the expression defined by the context property
                    var expression = new Expressions.DynamicExpression(contextProperty.Value, Expressions.ExpressionLanguage.Csharp);
                    value = expression.Invoke(this.ExpressionContext);
                    break;

                case GT.BizTalk.Framework.BizTalk.Serialization.ContextPropertySource.XPath:
                    // read the value from the message context
                    value = GetXPathValue(pipelineContext, inputMessage, contextProperty.Value);
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

        public IBaseMessage Execute(IPipelineContext pipelineContext, IBaseMessage inputMessage, string objectArguement)
        {
            var callToken = TraceProvider.Logger.TraceIn(this.Name);
            try
            {
                Guard.ArgumentNotNull(objectArguement, "objectArguement");

                // set the dynamic expression context required for ContextProperty properties of type "Expression"
                ExpressionData expressionData = new ExpressionData(inputMessage);
                this.ExpressionContext = new Expressions.ExpressionContext(ExpressionImports, expressionData);
                ContextPropertySerializer ser = new ContextPropertySerializer();
                ser.Deserialize(objectArguement);

                // iterate through all context properties promoting or writing
                foreach (ContextProperty contextProperty in ser.Properties)
                {
                    object value = this.GetPropertyValue(pipelineContext, inputMessage, contextProperty);
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

                return inputMessage;
            }
            catch (Exception ex)
            {
                // put component name as a source information in this exception,
                // so the event log in message could reflect this
                ex.Source = this.Name;
                TraceProvider.Logger.TraceError(ex);
                throw ex;
            }
            finally
            {
                TraceProvider.Logger.TraceOut(callToken, this.Name);
            }
        }

        #endregion Implementations

        #region xpath evaluation

        /// <summary>
        /// retrieve xpath value for a list of xpath query
        /// </summary>
        /// <param name="pipelineContext">Pipeline context</param>
        /// <param name="inputMessage">Input message</param>
        /// <param name="primaryXPathEventSources">a list of xpath for belonging to the primary BAM activity</param>
        /// <param name="relatedXPathEventSources">a list of xpath for belonging to the related BAM activity</param>
        /// <returns></returns>
        private object GetXPathValue(IPipelineContext pipelineContext, IBaseMessage inputMessage, string xpath)
        {
            var result = (object)null;
            // create seekable stream to work on the message
            Stream messageStream = new ReadOnlySeekableStream(inputMessage.BodyPart.GetOriginalDataStream());
            inputMessage.BodyPart.Data = messageStream;
            // save the stream position
            long position = messageStream.Position;

            // create an xml reader to prevent the xpath document from closing it
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = false;
            settings.CloseInput = false;
            using (XmlReader reader = XmlReader.Create(messageStream, settings))
            {
                XPathDocument xpathDoc = new XPathDocument(reader);
                XPathNavigator xpathNav = xpathDoc.CreateNavigator();

                result = EvaluateXPath(xpath, xpathNav);
            }

            // restore the stream position
            messageStream.Position = position;

            // prevent the message stream from being GC collected
            pipelineContext.ResourceTracker.AddResource(messageStream);

            return result;
        }

        /// <summary>
        /// evaluate xpath using provided XPath navigator and xpath property list
        /// </summary>
        /// <param name="xpathProperty">xpath property list</param>
        /// <param name="navigator">xpath navigator</param>
        /// <returns>primitive types or clone of sub-xpath-navigator</returns>
        private object EvaluateXPath(string xpath, XPathNavigator navigator)
        {
            object result = null;
            // create an xpath expression
            XPathExpression expression = XPathExpression.Compile(xpath);

            switch (expression.ReturnType)
            {
                case XPathResultType.String:
                case XPathResultType.Number:
                    result = navigator.Evaluate(expression);
                    break;

                case XPathResultType.NodeSet:
                    XPathNodeIterator ni = navigator.Select(expression);
                    if (ni.Count >= 1)
                    {
                        if (ni.MoveNext() == true)
                        {
                            if (ni.Current.NodeType.Equals(XPathNodeType.Text)
                                || ni.Current.NodeType.Equals(XPathNodeType.Attribute))
                            {
                                result = ni.Current.ToString();
                            }
                            else
                            {
                                result = ni.Current.Clone();
                            }
                        }
                    }
                    break;

                case XPathResultType.Boolean:
                    result = (bool)navigator.Evaluate(expression);
                    break;
            }
            return result;
        }

        #endregion xpath evaluation
    }

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
}