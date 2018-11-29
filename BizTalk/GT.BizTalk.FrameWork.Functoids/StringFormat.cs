using Microsoft.BizTalk.BaseFunctoids;
using System;
using System.Reflection;
using System.Text;

namespace GT.BizTalk.Framework.Functoids
{
    /// <summary>
    /// StringFormat functoid.,
    /// </summary>
    public class StringFormat : BaseFunctoid
    {
        #region Constants

        private const int MIN_INPUT_PARAMS = 2;
        private const int MAX_INPUT_PARAMS = 100;

        #endregion Constants

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public StringFormat()
            : base()
        {
            // Specify an unique ID for this functoid
            this.ID = Constants.FID_STRING_FORMAT;

            // set up the resource assembly to our own assembly
            this.SetupResourceAssembly(Constants.RESOURCE_ASSEMBLY, Assembly.GetExecutingAssembly());

            // Set up the name, description and tooltip
            // (as resource entries)
            this.SetName("STRFORMAT_NAME");
            this.SetDescription("STRFORMAT_DESC");
            this.SetTooltip("STRFORMAT_TOOLTIP");
            // Bitmap = 16x16
            this.SetBitmap("STRFORMAT_BITMAP");

            // Set the Assembly, Class and Method specification for the functionality
            // that will be executed when this functoid is used
            Type type = typeof(StringFormat.FunctoidScripts);
            this.SetExternalFunctionName(type.Assembly.FullName, type.FullName, "StringFormat");

            // Set the script type support
            this.AddScriptTypeSupport(ScriptType.CSharp);

            // We are expecting at leat 2 parameter and up to 100
            // The first parameter is the formatting string and at least one additional parameter with a value to be formatted
            this.HasVariableInputs = false;
            this.SetMinParams(MIN_INPUT_PARAMS);
            this.SetMaxParams(MAX_INPUT_PARAMS);

            // Bind to the String category
            // This determines the location in the toolbox within Visual Studio
            this.Category = FunctoidCategory.String;

            // Allow all except records
            this.OutputConnectionType = ConnectionType.AllExceptRecord;

            // We allow all inputs except records
            for (int index = 0; index < MAX_INPUT_PARAMS; ++index)
                this.AddInputConnectionType(ConnectionType.AllExceptRecord);

            this.HasSideEffects = false;
        }

        #endregion Constructor

        #region GetInlineScriptBuffer

        /// <summary>
        /// Generates the script required for this instance of the functoid based on the number of input parameters.
        /// </summary>
        /// <param name="scriptType">Type of script.</param>
        /// <param name="numParams">Number of input parameters.</param>
        /// <param name="functionNumber">Not used.</param>
        /// <returns></returns>
        protected override string GetInlineScriptBuffer(ScriptType scriptType, int numParams, int functionNumber)
        {
            if (ScriptType.CSharp != scriptType)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();

            // function declaration
            stringBuilder.Append(this.BuildDynamicScriptHeader("StringFormat", "string", "string", numParams));
            // build the function body
            stringBuilder.Append("{\n");
            // add "String.Format instruction with first parameter (param0) corresponding to the formatting string
            stringBuilder.Append("    return ");
            if (numParams >= MIN_INPUT_PARAMS)
            {
                stringBuilder.Append("string.Format(param0");
                // add parameters to be formatted to the "String.Format" expression
                // start at the first value parameter (skip formatting string -> param0)
                for (int index = 1; index < numParams; ++index)
                {
                    stringBuilder.AppendFormat(", param{0}", index);
                }
                stringBuilder.Append(");\n");
            }
            else
            {
                stringBuilder.Append("string.Empty;\n");
            }
            stringBuilder.Append("}\n");

            // return script
            return stringBuilder.ToString();
        }

        #endregion GetInlineScriptBuffer

        #region Formatter Class

        /// <summary>
        /// Formatter class.
        /// </summary>
        public class FunctoidScripts
        {
            /// <summary>
            /// Formats a text using the specified format and parameter.
            /// </summary>
            /// <param name="format">Formatting string.</param>
            /// <param name="param1">Parameter.</param>
            /// <returns>Formatted text.</returns>
            public string StringFormat(string format, string param1)
            {
                return string.Format(format, param1);
            }

            /// <summary>
            /// Formats a text using the specified format and parameters.
            /// </summary>
            /// <param name="format">Formatting string.</param>
            /// <param name="param1">Parameter 1.</param>
            /// <param name="param2">Parameter 2.</param>
            /// <returns>Formatted text.</returns>
            public string StringFormat(string format, string param1, string param2)
            {
                return string.Format(format, param1, param2);
            }

            /// <summary>
            /// Formats a text using the specified format and parameters.
            /// </summary>
            /// <param name="format">Formatting string.</param>
            /// <param name="param1">Parameter 1.</param>
            /// <param name="param2">Parameter 2.</param>
            /// <param name="param3">Parameter 3.</param>
            /// <returns>Formatted text.</returns>
            public string StringFormat(string format, string param1, string param2, string param3)
            {
                return string.Format(format, param1, param2, param3);
            }
        }

        #endregion Formatter Class
    }
}