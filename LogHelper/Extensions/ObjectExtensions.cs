using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace System
{
    public static class ObjectExtensions
    {
        public static void Log(this Object obj, LogLevel level, String message)
        {
            LogHelper.Log(obj, level, message, null);
        }

        public static void Log(this Object obj, LogLevel level, String format, params Object[] args)
        {
            LogHelper.Log(obj, level, format, args);
        }

        public static string ToNullSafeString(this Object obj)
        {
            return (obj == null || DBNull.Value.Equals(obj)) ? String.Empty : obj.ToString();
        }

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current <see cref="Object"/>
        /// </summary>
        /// <param name="obj">the object for which the string should be returned</param>
        /// <param name="replacement">the replacement text to return should the object be null.</param>
        /// <returns>a string which either represents the current object or its provided replacement value.</returns>
        /// <exception cref="ArgumentNullException">the replacement string is not specified.</exception>
        public static string ToNullSafeString(this Object obj, string replacement)
        {
            if (string.IsNullOrEmpty(replacement))
                throw new ArgumentNullException("replacement", "replacement string must be specified.");

            return (obj == null || DBNull.Value.Equals(obj)) ? replacement : obj.ToString();
        }    
    }
}