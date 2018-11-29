using Microsoft.BizTalk.BaseFunctoids;
using System;
using System.Reflection;
using System.Text;

namespace GT.BizTalk.Framework.Functoids
{
    /// <summary>
    /// StringTrim functoid.
    /// </summary>
    public class StringTrim : BaseFunctoid
    {
        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public StringTrim()
            : base()
        {
            // Specify an unique ID for this functoid
            this.ID = Constants.FID_STRING_TRIM;

            // set up the resource assembly to our own assembly
            this.SetupResourceAssembly(Constants.RESOURCE_ASSEMBLY, Assembly.GetExecutingAssembly());

            // Set up the name, description and tooltip
            // (as resource entries)
            this.SetName("STRTRIM_NAME");
            this.SetDescription("STRTRIM_DESC");
            this.SetTooltip("STRTRIM_TOOLTIP");
            // Bitmap = 16x16
            this.SetBitmap("STRTRIM_BITMAP");

            // We are expecting 1 parameter
            // The first and only parameter is the string value to check
            this.HasVariableInputs = false;
            this.SetMinParams(1);
            this.SetMaxParams(1);

            // Set the Assembly, Class and Method specification for the functionality
            // that will be executed when this functoid is used
            Type type = this.GetType();
            this.SetExternalFunctionName(type.Assembly.FullName, type.FullName, "StringTrim");

            // Set CSharp buffer containing code
            this.SetScriptBuffer(ScriptType.CSharp, this.GetCSharpBuffer());

            // Bind to the String category
            // This determines the location in the toolbox within Visual Studio
            this.Category = FunctoidCategory.String;

            // Allow all except records
            this.OutputConnectionType = ConnectionType.AllExceptRecord;

            // We allow all inputs except records
            this.AddInputConnectionType(ConnectionType.AllExceptRecord);

            this.HasSideEffects = false;
        }

        #endregion Constructor

        #region Private Methods

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private string GetCSharpBuffer()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("public string StringTrim(string str)\n");
            builder.Append("{\n");
            builder.Append("\tif (str == null)\n");
            builder.Append("\t{\n");
            builder.Append("\t\treturn \"\";\n");
            builder.Append("\t}\n");
            builder.Append("\treturn str.Trim(null);\n");
            builder.Append("}\n");
            return builder.ToString();
        }

        #endregion Private Methods
    }
}