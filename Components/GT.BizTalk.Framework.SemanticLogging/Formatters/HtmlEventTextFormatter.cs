using GT.BizTalk.Framework.SemanticLogging.Utility;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Web.UI;

namespace GT.BizTalk.Framework.SemanticLogging.Formatters
{
    /// <summary>
    /// A <see cref="IEventTextFormatter"/> implementation that writes out formatted html suitable for html email notifications.
    /// </summary>
    /// <remarks>This class is not thread-safe.</remarks>
    public class HtmlEventTextFormatter : IEventTextFormatter
    {
        #region Constants

        /// <summary>
        /// The default <see cref="VerbosityThreshold"/>.
        /// </summary>
        public const EventLevel DefaultVerbosityThreshold = EventLevel.Error;

        #endregion Constants

        #region Fields

        /// <summary>
        /// The datetime format.
        /// </summary>
        private string dateTimeFormat;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEventTextFormatter" /> class.
        /// </summary>
        /// <param name="verbosityThreshold">The verbosity threshold.</param>
        /// <param name="dateTimeFormat">The date time format used for timestamp value.</param>
        public HtmlEventTextFormatter(EventLevel verbosityThreshold = DefaultVerbosityThreshold, string dateTimeFormat = null)
        {
            this.VerbosityThreshold = verbosityThreshold;
            this.DateTimeFormat = dateTimeFormat;
        }

        #endregion Constructor

        #region Properties

        /// <summary>
        /// Gets or sets the lowest <see cref="System.Diagnostics.Tracing.EventLevel" /> value where the formatted output provides all the event entry information.
        /// Otherwise a summarized content of the event entry will be written.
        /// </summary>
        /// <value>The EventLevel.</value>
        public EventLevel VerbosityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the date time format used for timestamp value.
        /// </summary>
        /// <value>The date time format value.</value>
        public string DateTimeFormat
        {
            get
            {
                return this.dateTimeFormat;
            }

            set
            {
                Guard.ValidDateTimeFormat(value, "DateTimeFormat");
                this.dateTimeFormat = value;
            }
        }

        #endregion Properties

        #region IEventTextFormatter Implementation

        /// <summary>
        /// Writes the event.
        /// </summary>
        /// <param name="eventEntry">The <see cref="EventEntry" /> instance containing the event data.</param>
        /// <param name="writer">The writer.</param>
        public void WriteEvent(Microsoft.Practices.EnterpriseLibrary.SemanticLogging.EventEntry eventEntry, TextWriter writer)
        {
            Guard.ArgumentNotNull(eventEntry, "eventEntry");
            Guard.ArgumentNotNull(writer, "writer");

            // format the event entry using html
            using (HtmlTextWriter htmlWriter = new HtmlTextWriter(writer))
            {
                // write start of a table
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Border, "0");
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Table);

                if (eventEntry.Schema.Level <= this.VerbosityThreshold || this.VerbosityThreshold == EventLevel.LogAlways)
                {
                    // Write with verbosityThreshold format
                    this.WriteTwoCols(htmlWriter, PropertyNames.ProviderId, eventEntry.ProviderId);
                    this.WriteTwoCols(htmlWriter, PropertyNames.EventId, eventEntry.EventId);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Keywords, eventEntry.Schema.Keywords);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Level, eventEntry.Schema.Level);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Opcode, eventEntry.Schema.Opcode);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Task, eventEntry.Schema.Task);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Timestamp, eventEntry.GetFormattedTimestamp(this.DateTimeFormat));
                    this.WriteTwoCols(htmlWriter, PropertyNames.ProcessId, eventEntry.ProcessId);
                    this.WriteTwoCols(htmlWriter, PropertyNames.ThreadId, eventEntry.ThreadId);

                    if (eventEntry.ActivityId != Guid.Empty)
                    {
                        this.WriteTwoCols(htmlWriter, PropertyNames.ActivityId, eventEntry.ActivityId);
                    }

                    if (eventEntry.RelatedActivityId != Guid.Empty)
                    {
                        this.WriteTwoCols(htmlWriter, PropertyNames.RelatedActivityId, eventEntry.RelatedActivityId);
                    }

                    this.WriteTwoCols(htmlWriter, null, null); // leave an empty row
                    this.WriteTwoRows(htmlWriter, PropertyNames.Message, eventEntry.FormattedMessage);
                }
                else
                {
                    // Write with summary format
                    this.WriteTwoCols(htmlWriter, PropertyNames.EventId, eventEntry.EventId);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Level, eventEntry.Schema.Level);
                    this.WriteTwoCols(htmlWriter, PropertyNames.Timestamp, eventEntry.GetFormattedTimestamp(this.DateTimeFormat));
                    this.WriteTwoCols(htmlWriter, PropertyNames.ProcessId, eventEntry.ProcessId);
                    this.WriteTwoCols(htmlWriter, PropertyNames.ThreadId, eventEntry.ThreadId);

                    if (eventEntry.ActivityId != Guid.Empty)
                    {
                        this.WriteTwoCols(htmlWriter, PropertyNames.ActivityId, eventEntry.ActivityId);
                    }

                    if (eventEntry.RelatedActivityId != Guid.Empty)
                    {
                        this.WriteTwoCols(htmlWriter, PropertyNames.RelatedActivityId, eventEntry.RelatedActivityId);
                    }

                    this.WriteTwoCols(htmlWriter, null, null); // leave an empty row
                    this.WriteTwoRows(htmlWriter, PropertyNames.Message, eventEntry.FormattedMessage);
                }

                htmlWriter.RenderEndTag(); // </table>
            }
        }

        #endregion IEventTextFormatter Implementation

        #region Helpers

        private void WriteTwoCols(HtmlTextWriter htmlWriter, string name, object value)
        {
            // write a table row with two cells, one for the name and one for the value
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlWriter.WriteLine();

            // first cell
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "10px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "0");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "10px");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(this.HtmlEncodeNewLine(name));
            if (name != null)
                htmlWriter.Write(":");
            htmlWriter.RenderEndTag(); // </td>
            htmlWriter.WriteLine();

            // second cell
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "20px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "0");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "0");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Strong);
            htmlWriter.Write(this.HtmlEncodeNewLine(value));
            htmlWriter.RenderEndTag(); // </strong>
            htmlWriter.RenderEndTag(); // </td>
            htmlWriter.WriteLine();

            htmlWriter.RenderEndTag(); // </tr>
            htmlWriter.WriteLine();
        }

        private void WriteTwoRows(HtmlTextWriter htmlWriter, string name, object value)
        {
            // write two table rows, one for the name and one for the value

            // first row
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlWriter.WriteLine();
            // cell
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "10px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "0");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "10px");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(this.HtmlEncodeNewLine(name));
            if (name != null)
                htmlWriter.Write(":");
            htmlWriter.RenderEndTag(); // </td>
            htmlWriter.WriteLine();
            htmlWriter.RenderEndTag(); // </tr>
            htmlWriter.WriteLine();

            // second row
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Tr);
            htmlWriter.WriteLine();
            // cell
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "20px");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "0");
            htmlWriter.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "20px");
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Td);
            htmlWriter.Write(this.HtmlEncodeNewLine(value));
            htmlWriter.RenderEndTag(); // </td>
            htmlWriter.WriteLine();
            htmlWriter.RenderEndTag(); // </tr>
            htmlWriter.WriteLine();
        }

        private string HtmlEncodeNewLine(object value)
        {
            if (value != null)
                return value.ToString().Replace("\r", string.Empty).Replace("\n", "<br/>");
            else
                return "&nbsp;";
        }

        #endregion Helpers
    }
}