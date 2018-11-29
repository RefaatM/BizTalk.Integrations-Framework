using Microsoft.BizTalk.BaseFunctoids;
using System;
using System.Reflection;
using System.Text;

namespace GT.BizTalk.Framework.Functoids
{
    /// <summary>
    /// StringPadLef functoid.
    /// </summary>
    public class StringPadRight : BaseFunctoid
    {
        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public StringPadRight()
            : base()
        {
            // Specify an unique ID for this functoid
            this.ID = Constants.FID_STRING_PADRIGHT;

            // set up the resource assembly to our own assembly
            this.SetupResourceAssembly(Constants.RESOURCE_ASSEMBLY, Assembly.GetExecutingAssembly());

            // Set up the name, description and tooltip
            // (as resource entries)
            this.SetName("STRPADRIGHT_NAME");
            this.SetDescription("STRPADRIGHT_DESC");
            this.SetTooltip("STRPADRIGHT_TOOLTIP");
            // Bitmap = 16x16
            this.SetBitmap("STRPADRIGHT_BITMAP");

            // We are expecting 1 parameter
            // The first and only parameter is the string value to check
            this.HasVariableInputs = false;
            this.SetMinParams(3);
            this.SetMaxParams(3);

            // Set the Assembly, Class and Method specification for the functionality
            // that will be executed when this functoid is used
            Type type = this.GetType();
            this.SetExternalFunctionName(type.Assembly.FullName, type.FullName, "StringPadRight");

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

        private string GetCSharpBuffer()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("public string StringPadRight(string str, int totalWidth, string paddingChar)\n");
            builder.Append("{\n");
            builder.Append("\tif (str == null || string.IsNullOrEmpty(paddingChar) == true)\n");
            builder.Append("\t{\n");
            builder.Append("\t\treturn \"\";\n");
            builder.Append("\t}\n");
            builder.Append("\treturn str.PadRight(totalWidth, paddingChar[0]);\n");
            builder.Append("}\n");
            return builder.ToString();
        }

        #endregion Private Methods
    }
}