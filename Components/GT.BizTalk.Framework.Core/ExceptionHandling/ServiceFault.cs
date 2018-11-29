using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace GT.BizTalk.Framework.Core.ExceptionHandling
{
    [Serializable]
    [DataContract(Namespace = "https://schemas.DirectEnergy.com/Unify/Common/services/2015")]
    [XmlType(Namespace = "https://schemas.DirectEnergy.com/Unify/Common/services/2015")]
    [XmlRoot(Namespace = "https://schemas.DirectEnergy.com/Unify/Common/services/2015", IsNullable = true)]
    public class ServiceFault
    {
        #region Constructors

        /// <summary>
        /// Default instance constructor. Constructs an empty instance.
        /// </summary>
        public ServiceFault()
        {
        }

        /// <summary>
        /// Instance constructor. Constructs and initializes an instance using the specified
        /// fault code, message and reason.
        /// </summary>
        /// <param name="faultCode">Fault code.</param>
        /// <param name="message">Message.</param>
        /// <param name="reason">Reason.</param>
        public ServiceFault(string faultCode, string message, string reason)
        {
            this.FaultCode = faultCode;
            this.Message = message;
            this.Reason = reason;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets/sets the exception fault code.
        /// </summary>
        [DataMember(Order = 1)]
        public string FaultCode { get; set; }

        /// <summary>
        /// Gets/sets the exception message.
        /// </summary>
        [DataMember(Order = 2)]
        public string Message { get; set; }

        /// <summary>
        /// Gets/sets the exception reason.
        /// </summary>
        [DataMember(Order = 3)]
        public string Reason { get; set; }

        #endregion Properties
    }
}