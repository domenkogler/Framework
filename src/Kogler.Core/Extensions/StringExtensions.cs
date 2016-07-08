using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kogler.Framework
{
    public static class StringExtensions
    {
        #region << Public >>
        
        public static string CapitalizeWords(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
                result[i] = char.IsWhiteSpace(result[i - 1]) ? char.ToUpper(result[i]) : char.ToLower(result[i]);
            return result.ToString();
        }
        
        /// <summary>
        /// Converts string to int array
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static int[] ToIntArray(this string value, char separator)
        {
            return value.Split(separator).Select(int.Parse).ToArray();
        }

        /// <summary>Use this function like string.Split but instead of a character to split on,
        /// use a maximum line width size. This is similar to a Word Wrap where no words will be split.</summary>
        /// Note if the a word is longer than the maxcharactes it will be trimmed from the start.
        /// <param name="me">The string to parse.</param>
        /// <param name="maxCharacters">The maximum size.</param>
        /// <remarks>This function will remove some white space at the end of a line, but allow for a blank line.</remarks>
        /// http://omegacoder.com/?p=661
        /// <returns>An array of strings.</returns>
        public static List<string> SplitOn(this string me, int maxCharacters)
        {
            List<string> lines = new List<string>();
            if (string.IsNullOrEmpty(me) == false)
            {
                const string targetGroup = "Line";
                string theRegex = string.Format(@"(?<{0}>.{{1,{1}}})(?:\W|$)", targetGroup, maxCharacters);

                MatchCollection matches = Regex.Matches(me, theRegex, RegexOptions.IgnoreCase
                                                                      | RegexOptions.Multiline
                                                                      | RegexOptions.ExplicitCapture
                                                                      | RegexOptions.CultureInvariant);
                if (matches.Count > 0)
                    lines.AddRange(from Match m in matches select m.Groups[targetGroup].Value);
            }
            return lines;
        }

        public static string RemoveNonNumeric(this string me)
        {
            if (!string.IsNullOrEmpty(me))
            {
                char[] result = new char[me.Length];
                int resultIndex = 0;
                foreach (char c in me)
                {
                    if (char.IsNumber(c)) result[resultIndex++] = c;
                }
                if (0 == resultIndex)
                    me = string.Empty;
                else if (result.Length != resultIndex)
                    me = new string(result, 0, resultIndex);
            }
            return me;
        }

        private static readonly Regex EmailRegex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,6}$");

        /// <summary>
        /// true, if is valid email address
        /// from http://www.davidhayden.com/blog/dave/
        /// archive/2006/11/30/ExtensionMethodsCSharp.aspx
        /// </summary>
        /// <param name="me">email address to test</param>
        /// <returns>true, if is valid email address</returns>
        public static bool IsValidEmailAddress(this string me)
        {
            return EmailRegex.IsMatch(me);
        }

        private static readonly Regex UrlRegex =
            new Regex(
                "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                + "|" // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?" // port number- :80
                + "((/?)|" // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$"
                );

        /// <summary>
        /// Checks if url is valid. 
        /// from http://www.osix.net/modules/article/?id=586 and changed to match http://localhost
        /// 
        /// complete (not only http) url regex can be found 
        /// at http://internet.ls-la.net/folklore/url-regexpr.html
        /// </summary>
        /// <returns></returns>
        public static bool IsValidUrl(this string me)
        {
            return UrlRegex.IsMatch(me);
        }

        /// <summary>
        /// Reverse the string
        /// from http://en.wikipedia.org/wiki/Extension_method
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string Reverse(this string me)
        {
            char[] chars = me.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// Like LINQ Take - Takes the first x characters
        public static string Take(this string me, int count, bool ellipsis = false)
        {
            int lengthToTake = Math.Min(count, me.Length);
            var cutDownString = me.Substring(0, lengthToTake);

            if (ellipsis && lengthToTake < me.Length)
                cutDownString += "...";

            return cutDownString;
        }

        // Like LINQ Skip - Skips the first x characters and returns the remaining string
        public static string Skip(this string me, int count)
        {
            int startIndex = Math.Min(count, me.Length);
            var cutDownString = me.Substring(startIndex - 1);

            return cutDownString;
        }

        /// <summary>
        /// Reduce string to shorter preview which is optionally ended by some string (...).
        /// </summary>
        /// <param name="me">string to reduce</param>
        /// <param name="count">Length of returned string including endings.</param>
        /// <param name="endings">optional edings of reduced text</param>
        /// <param name="atLeft">if true, takes characters on left side insted of right side.</param>
        /// <example>
        /// string description = "This is very long description of something";
        /// string preview = description.Reduce(20,"...");
        /// produce -> "This is very long..."
        /// </example>
        /// <returns></returns>
        public static string Reduce(this string me, int count, string endings, bool atLeft = false)
        {
            if (me == null) return null;
            if (count < endings.Length)
                throw new Exception("Failed to reduce to less then endings length.");
            int len = me.Length + endings.Length;
            if (count > len) return me; //it's too short to reduce
            return atLeft
                       ? $"{endings}{me.Substring(len - count)}"
                : $"{me.Substring(0, len - count)}{endings}";
        }

        /// <summary>
        /// remove white space, not line end
        /// Useful when parsing user input such phone,
        /// price int.Parse("1 000 000".RemoveSpaces(),.....
        /// </summary>
        /// <param name="me"></param>
        public static string RemoveSpaces(this string me)
        {
            return me.Replace(" ", "");
        }

        /// <summary>
        /// true, if the string can be parse as Double respective Int32
        /// Spaces are not considred.
        /// </summary>
        /// <param name="me">input string</param>
        /// <param name="floatpoint">true, if Double is considered,
        /// otherwhise Int32 is considered.</param>
        /// <returns>true, if the string contains only digits or float-point</returns>
        public static bool IsNumber(this string me, bool floatpoint, IFormatProvider formatProvider)
        {
            int i;
            double d;
            string withoutWhiteSpace = me.RemoveSpaces();
            return floatpoint
                       ? double.TryParse(withoutWhiteSpace, NumberStyles.Any, formatProvider,
                                         out d)
                       : int.TryParse(withoutWhiteSpace, out i);
        }

        /// <summary>
        /// true, if the string contains only digits or float-point.
        /// Spaces are not considred.
        /// </summary>
        /// <param name="me">input string</param>
        /// <param name="floatpoint">true, if float-point is considered</param>
        /// <returns>true, if the string contains only digits or float-point</returns>
        public static bool IsNumberOnly(this string me, bool floatpoint)
        {
            me = me.Trim();
            if (me.Length == 0) return false;
            foreach (var c in me)
            {
                if (!char.IsDigit(c) && !floatpoint) return false;
                return c == '.' || c == ',';
            }
            return true;
        }

        /// <summary>
        /// Returns the given string truncated to the specified length, suffixed with the given value
        /// </summary>
        /// <param name="me"></param>
        /// <param name="length">Maximum length of return string</param>
        /// <param name="suffix">The value to suffix the return value with (if truncation is performed)</param>
        /// <returns></returns>
        public static string Truncate(this string me, int length, string suffix = "...")
        {
            if (me == null) return string.Empty;
            if (me.Length <= length) return me;
            if (suffix == null) suffix = "...";
            return me.Substring(0, length - suffix.Length) + suffix;
        }

        /// <summary>
        /// Splits a given string into an array based on character line breaks
        /// </summary>
        /// <param name="me"></param>
        /// <returns>String array, each containing one line</returns>
        public static string[] ToLineArray(this string me)
        {
            return me == null ? new string[] {} : Regex.Split(me, "\r\n");
        }

        /// <summary>
        /// Splits a given string into a strongly-typed list based on character line breaks
        /// </summary>
        /// <param name="me"></param>
        /// <returns>Strongly-typed string list, each containing one line</returns>
        public static List<string> ToLineList(this string me)
        {
            var output = new List<string>();
            output.AddRange(me.ToLineArray());
            return output;
        }

        /// <summary>
        /// Replaces line breaks with self-closing HTML 'br' tags
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        public static string ReplaceBreaksWithBr(this string me)
        {
            return string.Join("<br/>", me.ToLineArray());
        }

        /// <summary>
        /// Replaces any single apostrophes with two of the same
        /// </summary>
        /// <param name="me"></param>
        /// <returns>String</returns>
        public static string DoubleApostrophes(this string me)
        {
            return Regex.Replace(me, "'", "''");
        }

        private static readonly Regex StripHtmlRegex =
            new Regex(@"<(style|script)[^<>]*>.*?</\1>|</?[a-z][a-z0-9]*[^<>]*>|<!--.*?-->");

        /// <summary>
        /// Removes any HTML tags from the input string
        /// </summary>
        /// <param name="me"></param>
        /// <returns>String</returns>
        public static string StripHtml(this string me)
        {
            return StripHtmlRegex.Replace(me, "");
        }

        /// <summary>
        /// Return the leftmost number_of_characters characters from the string.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to return.</param>
        /// <returns>A string containing the leftmost characters in this string.</returns>
        public static string Left(this string me, int number_of_characters)
        {
            if (number_of_characters < 0) return me;
            return me.Length <= number_of_characters ? me : me.Substring(0, number_of_characters);
        }

        public static string LeftOf(this string me, string indexOf)
        {
            return me.Left(me.IndexOf(indexOf, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Return the rightmost number_of_characters characters from the string.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to return.</param>
        /// <returns>A string containing the rightmost characters in this string.</returns>
        public static string Right(this string me, int number_of_characters)
        {
            return me.Length <= number_of_characters ? me : me.Substring(me.Length - number_of_characters);
        }

        public static string Mid(this string me, int index, int count)
        {
            return me.Substring(index, count);
        }

        public static int ToInteger(this string me)
        {
            int integerValue;
            int.TryParse(me, out integerValue);
            return integerValue;
        }

        public static bool IsInteger(this string me)
        {
            Regex regularExpression = new Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(me).Success;
        }

        /// <summary>
        /// Return the string with the leftmost number_of_characters characters removed.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveLeft(this string me, int number_of_characters)
        {
            if (me.Length <= number_of_characters) return "";
            return me.Substring(number_of_characters);
        }

        /// <summary>
        /// Return the string with the rightmost number_of_characters characters removed.
        /// </summary>
        /// <param name="me">The string being extended</param>
        /// <param name="number_of_characters">The number of characters to remove.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string RemoveRight(this string me, int number_of_characters)
        {
            if (me.Length <= number_of_characters) return "";
            return me.Substring(0, me.Length - number_of_characters);
        }
        
        /// <summary>
        /// Convert this string containing hexadecimal into a byte array.
        /// </summary>
        /// <param name="me">The hexadecimal string to convert.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static byte[] HexStringToBytes(this string me)
        {
            me = me.Replace(" ", "");
            int max_byte = (me.Length/2) - 1;
            var bytes = new byte[max_byte + 1];
            for (int i = 0; i <= max_byte; i++)
            {
                bytes[i] = byte.Parse(me.Substring(2*i, 2), NumberStyles.AllowHexSpecifier);
            }

            return bytes;
        }

        // "a string".IsNullOrEmpty() beats string.IsNullOrEmpty("a string")
        public static bool IsNullOrEmpty(this string me)
        {
            return string.IsNullOrEmpty(me);
        }

        /// <summary>
        /// Checks if string is in "version" format
        /// </summary>
        /// <param name="me">The hexadecimal string to convert.</param>
        /// <returns></returns>
        public static bool IsValidSectionedNumber(this string me)
        {
            // SectionedNumbers may have up to 10 sections of 5-or-less-digit numbers.
            string[] sections = me.Split('.');
            if (sections.Length > 10)
            {
                return false;
            }
            foreach (string t in sections)
            {
                if (t.Length < 1 || t.Length > 5)
                    return false;
                int number;
                if (int.TryParse(t, out number) == false)
                    return false;
            }
            return true;
        }

        private static readonly Regex NewLineRegex = new Regex(@"\r(?!\n)|(?<!\r)\n");

        public static string NormalizeNewLines(this string value)
        {
            // From: 
            // http://stackoverflow.com/questions/3022571/how-to-deal-with-the-new-line-character-in-the-silverlight-textbox
            return value == null ? null : NewLineRegex.Replace(value, Environment.NewLine);
        }

        public static int CountOf(this string value, char ch)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            int count = 0;
            foreach (var c in value)
            {
                if (c == ch) count++;
            }
            return count;
        }

        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            return source.IndexOf(value, comparison) >= 0;
        }

        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool StartsWithIgnoreCase(this string source, string value)
        {
            return source.StartsWith(value, StringComparison.OrdinalIgnoreCase);
        }

        public static string PadCenter(this string me, int width, char c = ' ')
        {
            if (IsNullOrEmpty(me) || width <= me.Length) return me;
            int padding = width - me.Length;
            return me.PadLeft(me.Length + padding/2, c).PadRight(width, c);
        }

        #endregion
    }
}