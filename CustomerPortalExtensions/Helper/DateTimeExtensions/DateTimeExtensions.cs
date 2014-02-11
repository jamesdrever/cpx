using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerPortalExtensions.Helper.DateTimeExtensions
{
    public static class DateTimeExtensions
    {

        public static string ToStringOrDefault(this DateTime? source, string format, string defaultValue)
        {
            if (source != null)
            {
                return source.Value.ToString(format);
            }
            else
            {
                return String.IsNullOrEmpty(defaultValue) ? String.Empty : defaultValue;
            }
        }

        public static string ToStringOrDefault(this DateTime? source, string format)
        {
            return ToStringOrDefault(source, format, null);
        }

    }
}