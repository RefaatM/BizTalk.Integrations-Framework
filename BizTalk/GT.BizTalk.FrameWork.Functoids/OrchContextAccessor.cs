using Microsoft.BizTalk.BaseFunctoids;
using Microsoft.XLANGs.BaseTypes;
using Microsoft.XLANGs.Core;
using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace GT.BizTalk.Framework.Functoids
{
    /// <summary>
    /// This is the main Orchestration Context Accesor functoid.
    /// </summary>
    /// <remarks>Originally developed by Carlos Medina.</remarks>
    /// <see cref="http://www.devdeo.com"/>
    public class OrchContextAccessor : BaseFunctoid
    {
        public OrchContextAccessor()
            : base()
        {
            // Specify an unique ID for this functoid
            this.ID = Constants.FID_ORCH_CONTEXT_ACCESSOR;

            // set up the resource assembly to our own assembly
            this.SetupResourceAssembly(Constants.RESOURCE_ASSEMBLY, Assembly.GetExecutingAssembly());

            // Set up the name, description and tooltip
            // (as resource entries)
            this.SetName("OCA_NAME");
            this.SetDescription("OCA_DESC");
            this.SetTooltip("OCA_TOOLTIP");
            // Bitmap = 16x16
            this.SetBitmap("OCA_BITMAP");

            // We are expecting four parameters, one optional
            // The first will be the message that contains the ctx prop
            // The second will be the context property name
            // The third the namespace the property is defined in
            // The fourth is optional and is the replacement value if the requested
            // context property could not be found
            this.HasVariableInputs = true;
            this.SetMinParams(3);
            this.SetMaxParams(4);

            // Set the Assembly, Class and Method specification for the functionality
            // that will be executed when this functoid is used
            Type type = this.GetType();
            this.SetExternalFunctionName(type.Assembly.FullName, type.FullName, "OrchAccessContext");

            // Bind to the Advanced category
            // This determines the location in the toolbox within Visual Studio
            // NOTE: use "None" to allow the functoid to show under the "Advanced" tab
            // and the output to be connected to all except records
            this.Category = FunctoidCategory.None;

            // Allow all except records
            this.OutputConnectionType = ConnectionType.AllExceptRecord;

            // We only allows fields or elements as input.
            this.AddInputConnectionType(ConnectionType.AllExceptRecord);
            this.AddInputConnectionType(ConnectionType.AllExceptRecord);
            this.AddInputConnectionType(ConnectionType.AllExceptRecord);
        }

        public string OrchAccessContext(string contextMessage, string contextItemName, string contextItemNamespace)
        {
            return this.OrchAccessContext(contextMessage, contextItemName, contextItemNamespace, "");
        }

        // This is the function that gets called when the Map is executed which has this functoid.
        public string OrchAccessContext(string contextMessage, string contextItemName, string contextItemNamespace, string replacementValue)
        {
            object retval = null;
            retval = GetMsgCtxProperty(contextMessage, contextItemName, contextItemNamespace);

            // if retval has not been filled at this point, either an exception occured or
            // the ItemName+ItemNamespaceUri is non existing or empty.
            // we will replace the value with the given replacement value.
            if (retval == null)
                retval = replacementValue;

            // return the value to the Xslt engine.
            return retval as string;
        }

        // This is the actuall method retrieving properties from the context.
        private object GetMsgCtxProperty(string contextMessage, string contextItemName, string contextItemNamespace)
        {
            object retval = null;

            try
            {
                // get the service parent of the context
                foreach (Microsoft.XLANGs.Core.Segment segment in Service.RootService._segments)
                {
                    // find the real name of the message
                    IDictionary fields = Context.FindFields(typeof(XLANGMessage), segment.ExceptionContext);
                    foreach (DictionaryEntry ctxfield in fields)
                    {
                        string field = ctxfield.Key.ToString();
                        if (field.EndsWith(contextMessage))
                        {
                            // get the XMessage object
                            XMessage xmsg = ctxfield.Value as XMessage;
                            // get the value of the property if the message was found
                            if (xmsg != null)
                            {
                                // create a XmlQName instance
                                XmlQName qName = new XmlQName(contextItemName, contextItemNamespace);
                                // find the message property in the message
                                if (xmsg.GetContextProperties().ContainsKey(qName))
                                {
                                    // get the property from GetContextProperties
                                    retval = xmsg.GetContextProperties()[qName];
                                }
                                else if (xmsg.GetContentProperties().ContainsKey(qName))
                                {
                                    // get the property from GetContentProperties
                                    retval = xmsg.GetContentProperties()[qName];
                                }
                            }
                            goto exit;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // If an exception occurs while retrieving the interface to the context or
                // accessing the properties with that interface, we ignore the failure and
                // force the retval to null.  In addition, the error is wrote in Trace
                Trace.WriteLine(string.Format("OrchContextAccessor functoid has failed, message: {0}... stack: {1}", e.Message, e.StackTrace));
                retval = null;
            }
        exit:
            // return the value
            return retval;
        }
    }
}