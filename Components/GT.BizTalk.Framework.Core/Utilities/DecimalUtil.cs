using GT.BizTalk.Framework.Core.Tracing;
using System;
using System.Linq;

namespace GT.BizTalk.Framework.Utilities
{
    public class DecimalUtil
    {
        public static decimal ParseDecimal(string input)
        {
            var decRegex = new System.Text.RegularExpressions.Regex("(?<LEADINGZERO>[0]*)(?<VALUE>[1-9]{1}[0-9]*)*");
            var match = decRegex.Match(input);
            if (match.Success
                && decRegex.GetGroupNames().Contains("VALUE")
                && match.Groups["VALUE"].Success)
            {
                return Convert.ToInt32(match.Groups["VALUE"].Value) / new Decimal(100);
            }
            if (match.Success
                && decRegex.GetGroupNames().Contains("LEADINGZERO")
                && match.Groups["LEADINGZERO"].Success)
            {
                return new Decimal(0.00);
            }

            TraceProvider.Logger.TraceError(string.Format("Unable to parse <{0}> as decimal.", input));

            throw new Exception(string.Format("Unable to parse <{0}> as decimal.", input));
        }

        public static string FormatDecimal(decimal input, string format)
        {
            return input.ToString(format);
        }
    }
}