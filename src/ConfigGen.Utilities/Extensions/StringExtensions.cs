using System;
using JetBrains.Annotations;

namespace ConfigGen.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty([CanBeNull] this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        [CanBeNull]
        public static string WithoutNewlines([CanBeNull] this string s)
        {
            return s?.Replace("\n", "");
        }
    }
}
