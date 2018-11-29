using Microsoft.BizTalk.BaseFunctoids;
using System;
using System.Globalization;
using System.Reflection;

namespace GT.BizTalk.Framework.Functoids
{
    /// <summary>
    /// Format Date Time functoid.
    /// </summary>
    public class FormatDateTime : BaseFunctoid
    {
        #region Fields

        private const string INVALID_DATE = "INVALID";

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public FormatDateTime()
            : base()
        {
            // Specify an unique ID for this functoid
            this.ID = Constants.FID_FORMAT_DATETIME;

            // set up the resource assembly to our own assembly
            this.SetupResourceAssembly(Constants.RESOURCE_ASSEMBLY, Assembly.GetExecutingAssembly());

            // Set up the name, description and tooltip
            // (as resource entries)
            this.SetName("FMTDT_NAME");
            this.SetDescription("FMTDT_DESC");
            this.SetTooltip("FMTDT_TOOLTIP");
            // Bitmap = 16x16
            this.SetBitmap("FMTDT_BITMAP");

            // We are expecting 4 parameters
            // The first will be the input date
            // The second will be the format of the input date
            // The third will be the format of the output date
            // The fourth will be the default value of the output date
            this.HasVariableInputs = true;
            this.SetMinParams(2);
            this.SetMaxParams(4);

            // Set the Assembly, Class and Method specification for the functionality
            // that will be executed when this functoid is used
            Type type = this.GetType();
            this.SetExternalFunctionName(type.Assembly.FullName, type.FullName, "DateFormatter");

            // Bind to the DateTime category
            // This determines the location in the toolbox within Visual Studio
            this.Category = FunctoidCategory.DateTime;

            // We output to all except records
            base.OutputConnectionType = ConnectionType.AllExceptRecord;

            // We allow all inputs except records
            this.AddInputConnectionType(ConnectionType.AllExceptRecord);
            this.AddInputConnectionType(ConnectionType.AllExceptRecord);
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Formats the input string representing a DateTime value using the specified output format.
        /// </summary>
        /// <param name="inDate">String representing a DateTime value.</param>
        /// <param name="outFormat">Formatting string use to format the output.</param>
        /// <remarks>
        /// The input string is parsed using the default DateTime parsing.
        /// </remarks>
        /// <returns>Formatted string representing a DateTime value; otherwise "INVALID" if the input string could not be parsed.</returns>
        public string DateFormatter(string inDate, string outFormat)
        {
            string outDate = String.Empty;
            DateTime date = DateTime.MinValue;
            if (DateTime.TryParse(inDate, out date) == true)
            {
                outDate = date.ToString(outFormat);
            }
            else
            {
                outDate = INVALID_DATE;
            }
            return outDate;
        }

        /// <summary>
        /// Formats the input string representing a DateTime value in the specified input format
        /// using the specified output format.
        /// </summary>
        /// <param name="inDate">String representing a DateTime value.</param>
        /// <param name="inFormat">Formatting string use to parsed the input string value.</param>
        /// <param name="outFormat">Formatting string use to format the output.</param>
        /// <remarks>
        /// The input string is parsed using the specified input format.
        /// </remarks>
        /// <returns>Formatted string representing a DateTime value; otherwise "INVALID" if the input string could not be parsed.</returns>
        public string DateFormatter(string inDate, string inFormat, string outFormat)
        {
            return DateFormatter(inDate, inFormat, outFormat, INVALID_DATE);
        }

        /// <summary>
        /// Formats the input string representing a DateTime value in the specified input format
        /// using the specified output format.
        /// </summary>
        /// <param name="inDate">String representing a DateTime value.</param>
        /// <param name="inFormat">Formatting string use to parsed the input string value.</param>
        /// <param name="outFormat">Formatting string use to format the output.</param>
        /// <param name="defaultValue">Default value, if parsing fails.</param>
        /// <remarks>
        /// The input string is parsed using the specified input format.
        /// </remarks>
        /// <returns>Formatted string representing a DateTime value; otherwise specified default value if the input string could not be parsed.</returns>
        public string DateFormatter(string inDate, string inFormat, string outFormat, string defaultValue)
        {
            string outDate = String.Empty;
            DateTime date = DateTime.MinValue;
            if (DateTime.TryParseExact(inDate, inFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date) == true)
            {
                outDate = date.ToString(outFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                outDate = defaultValue;
            }
            return outDate;
        }

        #endregion Public Methods
    }
}