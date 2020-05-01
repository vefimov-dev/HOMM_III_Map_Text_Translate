using System;

namespace Translator.Core.Utility
{
    public static class StringExtentions
    {
        public static bool IsOrdinalEqualsIgnoreCase(this string value, string other)
        {
            return string.Compare(value, other, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public static bool ContainsOrdinalIgnoreCase(this string value, string substring)
        {
            return value.IndexOf(substring, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool StartsWithOrdinalIgnoreCase(this string value, string substring)
        {
            return value.StartsWith(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithOrdinalIgnoreCase(this string value, string substring)
        {
            return value.EndsWith(substring, StringComparison.OrdinalIgnoreCase);
        }
    }
}