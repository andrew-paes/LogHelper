using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static string ToNullSafeFormat(this string text, object arg0)
        {
            return ToNullSafeFormat(text, new object[] { arg0 });
        }

        public static string ToNullSafeFormat(this string text, object arg0, object arg1)
        {
            return ToNullSafeFormat(text, new object[] { arg0, arg1 });
        }

        public static string ToNullSafeFormat(this string text, object arg0, object arg1, object arg2)
        {
            return ToNullSafeFormat(text, new object[] { arg0, arg1, arg2 });
        }

        public static string ToNullSafeFormat(this string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text) || args == null || args.Length == 0) return text;

            return string.Format(text, args);
        }
    }
}