using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace MVCwCMS.Helpers
{
    /// <summary>
    /// Represents a wildcard running on the <see cref="System.Text.RegularExpressions"/> engine
    /// </summary>
    public class WildcardHelper : Regex
    {
        /// <summary>
        /// Initializes a wildcard with the given search pattern.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        public WildcardHelper(string pattern)
            : base(WildcardToRegex(pattern))
        {
        }

        /// <summary>
        /// Initializes a wildcard with the given search pattern and options.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to match.</param>
        /// <param name="options">A combination of one or more
        /// <see cref="System.Text.RegexOptions"/>.</param>
        public WildcardHelper(string pattern, RegexOptions options)
            : base(WildcardToRegex(pattern), options)
        {
        }

        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
             Replace("\\*", ".*").
             Replace("\\?", ".") + "$";
        }

        /// <summary>
        /// Performs a wildcard (*,?) case insensitive search returning true if the text satisfies the provided pattern
        /// </summary>
        /// <param name="text">The source expression</param>
        /// <param name="pattern">The pattern containing the wildcard characters</param>
        /// <returns></returns>
        public static bool IsLike(string text, string pattern)
        {
            return IsLike(text, pattern, false);
        }

        /// <summary>
        /// Performs a wildcard (*,?) case insensitive search returning true if the text satisfies one of the provided pattern contained in the patternList
        /// </summary>
        /// <param name="text">The source expression</param>
        /// <param name="pattern">The pattern containing the wildcard characters</param>
        /// <returns></returns>
        public static bool IsLike(string text, string[] patternList)
        {
            return IsLike(text, patternList, false);
        }

        /// <summary>
        /// Performs a wildcard (*,?) search returning true if the text satisfies the provided pattern
        /// </summary>
        /// <param name="text">The source expression</param>
        /// <param name="pattern">The pattern containing the wildcard characters</param>
        /// <param name="caseSensitive">True to perform a case sensitive search</param>
        /// <returns></returns>
        public static bool IsLike(string text, string pattern, bool caseSensitive)
        {
            WildcardHelper wildcard = new WildcardHelper(pattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
            return wildcard.IsMatch(text);
        }

        /// <summary>
        /// Performs a wildcard (*,?) search returning true if the text satisfies one of the provided pattern contained in the patternList
        /// </summary>
        /// <param name="text"></param>
        /// <param name="patternList"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        public static bool IsLike(string text, string[] patternList, bool caseSensitive)
        {
            bool isItemMatched = false;
            foreach (string patternItem in patternList)
            {
                if (WildcardHelper.IsLike(text, patternItem))
                {
                    isItemMatched = true;
                    break;
                }
            }
            return isItemMatched;
        }
    }
}
