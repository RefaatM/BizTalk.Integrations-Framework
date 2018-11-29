using Microsoft.BizTalk.BaseFunctoids;
using System;
using System.Reflection;

namespace GT.BizTalk.Framework.Functoids
{
    /// <summary>
    /// Guid generator functoid.
    /// </summary>
    internal class GuidGenerator : BaseFunctoid
    {
        #region Constructor

        /// <summary>
        /// Default instance constructor.
        /// </summary>
        public GuidGenerator()
            : base()
        {
            // Specify an unique ID for this functoid
            this.ID = Constants.FID_GUID_GENERATOR;

            // set up the resource assembly to our own assembly
            this.SetupResourceAssembly(Constants.RESOURCE_ASSEMBLY, Assembly.GetExecutingAssembly());

            // Set up the name, description and tooltip
            // (as resource entries)
            this.SetName("GUIDGEN_NAME");
            this.SetTooltip("GUIDGEN_TOOLTIP");
            this.SetDescription("GUIDGEN_DESCRIPTION");
            // Bitmap = 16x16
            this.SetBitmap("GUIDGEN_BITMAP");

            // Bind to the Advanced category
            // This determines the location in the toolbox within Visual Studio
            // NOTE: use "None" to allow the functoid to show under the "Advanced" tab
            // and the output to be connected to all except records
            this.Category = FunctoidCategory.None;

            // Set the limits for the number of input parameters.
            this.SetMinParams(0);
            this.SetMaxParams(0);

            // The functoid output can go to any node type.
            this.OutputConnectionType = ConnectionType.All;

            // Set the function name that is to be called when invoking this functoid.
            // To test the map in Visual Studio, this functoid does not need to be in the GAC.
            // If using this functoid in a deployed BizTalk app. then it must be in the GAC
            this.SetExternalFunctionName(GetType().Assembly.FullName, GetType().FullName, "GenerateGuid");
        }

        #endregion Constructor

        #region Implementation

        /// <summary>
        /// Generates and returns a new guid as a string.
        /// </summary>
        /// <remarks>
        /// This is the function that gets called when the Map is executed which has this functoid.
        /// </remarks>
        /// <returns>New guid as a string.</returns>
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion Implementation
    }
}