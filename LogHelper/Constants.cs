using System;
using System.Collections.Generic;
using System.Net;

namespace System
{
    public abstract class Constants
    {
        public static class Configuration
        {
            public const string APPLICATION_NAME_PROPERTY = "AppName";

            public const string USER_NAME_PROPERTY = "UserDisplayName";

            public static class Logging
            {
                public const string PATH_PROPERTY_NAME = "LogPath";
            }
        }

        public static class Pluralization
        {
            public static class English
            {
                // [some] rules regarding pluralizing English words
                // http://www.csse.monash.edu.au/~damian/papers/HTML/Plurals.html

                public static readonly Dictionary<string, string> Suffixes = new Dictionary<string, string>()
                {
                    {"ss", "sses"},
                    {"y", "ies"},
                    {"is", "es"},
                    {"eus", "ei"},
                    {"ix", "ices"},
                    {"x", "xes"},
                    {"o", "oes"},
                    {"status", "statuses"},
                    {"ata", "ata"}
                };

                public static string Pluralize(string noun)
                {
                    if (noun == null) throw new ArgumentNullException("noun");

                    foreach (string key in Suffixes.Keys)
                        if (noun.EndsWith(key))
                            return noun.Substring(0, noun.Length - key.Length) + Suffixes[key];

                    return noun + "s";
                }
            }
        }
    }
}