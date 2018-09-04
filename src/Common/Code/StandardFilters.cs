using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DotLiquid.Util;
using System.Text;
using System.Reflection;

namespace DotLiquid
{
    /// <summary>
    /// Standard Liquid filters
    /// </summary>
    public static class StandardFilters
    {
        /// <summary>
        /// Return the size of an array or of an string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int size(object input)
        {
            if (input is string stringInput)
            {
                return stringInput.Length;
            }
            if (input is IEnumerable enumerableInput)
            {
                return enumerableInput.Cast<object>().Count();
            }
            return 0;
        }

        /// <summary>
        /// Return a Part of a String
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string slice(string input, int start, int len = 1)
        {
            if (input == null || start > input.Length)
                return null;

            if (start < 0)
            { 
                start += input.Length;
            }
            if (start + len > input.Length)
            { 
                len = input.Length - start;
            }
            return input.Substring(start, len);
        }

        /// <summary>
        /// convert a input string to DOWNCASE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string downcase(string input)
        {
            return input == null ? input : input.ToLower();
        }

        /// <summary>
        /// convert a input string to UPCASE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string upcase(string input)
        {
            return input == null
                ? input
                : input.ToUpper();
        }

        /// <summary>
        /// convert a input string to URLENCODE
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string url_encode(string input)
        {
            return input == null
                ? input
                : Uri.EscapeDataString(input);
        }

        /// <summary>
        /// capitalize words in the input sentence
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string capitalize(string input)
        {
            if (input.IsNullOrWhiteSpace())
                return input;

            return string.IsNullOrEmpty(input)
                ? input
#if CORE
                : Regex.Replace(input, @"\b(\w)", m => m.Value.ToUpper(), RegexOptions.None, Template.RegexTimeOut);
#else
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
#endif
        }

        /// <summary>
        /// Escape html chars
        /// </summary>
        /// <param name="input">String to escape</param>
        /// <returns>Escaped string</returns>
        /// <remarks>Alias of H</remarks>
        public static string escape(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                return WebUtility.HtmlEncode(input);
            }
            catch
            {
                return input;
            }
        }

        /// <summary>
        /// escape string values as defined in rfc4627 http://www.ietf.org/rfc/rfc4627.txt, chapter 2.5
        /// </summary>
        /// <param name="input">String to escape</param>
        /// <returns>Escaped string</returns>
        public static string json_escape(string input)
        {

            if (string.IsNullOrEmpty(input))
                return input;

            return cleanForJSON(input);

        }

        private static string cleanForJSON(string s)
        {

            if (s == null || s.Length == 0)
            {
                return "";
            }

            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            foreach(char c in s)
            {

                switch (c)
                {
                    case '\\':
                    case '"':
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;

                    case '\b':
                        sb.Append("\\b");
                        break;

                    case '\t':
                        sb.Append("\\t");
                        break;

                    case '\n':
                        sb.Append("\\n");
                        break;

                    case '\f':
                        sb.Append("\\f");
                        break;

                    case '\r':
                        sb.Append("\\r");
                        break;

                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;

                }
            }

            return sb.ToString();

        }

        /// <summary>
        /// Escape html chars
        /// </summary>
        /// <param name="input">String to escape</param>
        /// <returns>Escaped string</returns>
        /// <remarks>Alias of Escape</remarks>
        public static string h(string input)
        {
            return escape(input);
        }

        /// <summary>
        /// Truncates a string down to x characters
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <param name="truncateString"></param>
        /// <returns></returns>
        public static string truncate(string input, int length = 50, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int l = length - truncateString.Length;

            return input.Length > length
                ? input.Substring(0, l < 0 ? 0 : l) + truncateString
                : input;
        }

        /// <summary>
        /// Truncate a string down to x words
        /// </summary>
        /// <param name="input"></param>
        /// <param name="words"></param>
        /// <param name="truncateString"></param>
        /// <returns></returns>
        public static string truncate_words(string input, int words = 15, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var wordList = input.Split(' ').ToList();
            int l = words < 0 ? 0 : words;

            return wordList.Count > l
                ? string.Join(" ", wordList.Take(l).ToArray()) + truncateString
                : input;
        }

        /// <summary>
        /// Split input string into an array of substrings separated by given pattern.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] split(string input, string pattern)
        {
            return input.IsNullOrWhiteSpace()
                ? new[] { input }
                : input.Split(new[] { pattern }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Strip all html nodes from input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string strip_html(string input)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : Regex.Replace(input, @"<.*?>", string.Empty, RegexOptions.None, Template.RegexTimeOut);
        }

        /// <summary>
        /// Strip all whitespace from input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string strip(string input)
        {
            return input?.Trim();
        }

        /// <summary>
        /// Strip all leading whitespace from input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string lstrip(string input)
        {
            return input?.TrimStart();
        }

        /// <summary>
        /// Strip all trailing whitespace from input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string rstrip(string input)
        {
            return input?.TrimEnd();
        }

        /// <summary>
        /// Converts the input object into a formatted currency as specified by the culture info.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string currency(object input, string cultureInfo = null)
        {

            if (decimal.TryParse(input.ToString(), out decimal amount))
            {
                if (cultureInfo.IsNullOrWhiteSpace())
                {
                    cultureInfo = CultureInfo.CurrentCulture.Name;
                }

                var culture = new CultureInfo(cultureInfo);

                return amount.ToString("C", culture);
            }

            return input.ToString();
        }

        /// <summary>
        /// Remove all newlines from the string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string strip_newlines(string input)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : Regex.Replace(input, @"(\r?\n)", string.Empty, RegexOptions.None, Template.RegexTimeOut);
        }

        /// <summary>
        /// Join elements of the array with a certain character between them
        /// </summary>
        /// <param name="input"></param>
        /// <param name="glue"></param>
        /// <returns></returns>
        public static string join(IEnumerable input, string glue = " ")
        {
            if (input == null)
                return null;

            IEnumerable<object> castInput = input.Cast<object>();

            return string.Join(glue, castInput);
        }

        /// <summary>
        /// Sort elements of the array
        /// provide optional property with which to sort an array of hashes or drops
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable sort(object input, string property = null)
        {

            if (input == null)
                return null;

            List<object> ary;
            if (input is IEnumerable<Hash> enumerableHash && !string.IsNullOrEmpty(property))
            {
                ary = enumerableHash.Cast<object>().ToList();
            }
            else if (input is IEnumerable enumerableInput)
            {
                ary = enumerableInput.Flatten().Cast<object>().ToList();
            }
            else
            {
                ary = new List<object>(new[] { input });
            }

            if (!ary.Any())
                return ary;

            if (string.IsNullOrEmpty(property))
            { 
                ary.Sort();
            }
            else if ((ary.All(o => o is IDictionary)) && ((IDictionary)ary.First()).Contains(property))
            { 
                ary.Sort((a, b) => Comparer<object>.Default.Compare(((IDictionary)a)[property], ((IDictionary)b)[property]));
            }
            else if (ary.All(o => o.RespondTo(property)))
            { 
                ary.Sort((a, b) => Comparer<object>.Default.Compare(a.Send(property), b.Send(property)));
            }

            return ary;
        }

        /// <summary>
        /// Map/collect on a given property
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable map(IEnumerable input, string property)
        {

            if (input == null)
                return null;

            List<object> ary = input.Cast<object>().ToList();
            if (!ary.Any())
                return ary;

            if ((ary.All(o => o is IDictionary)) && ((IDictionary)ary.First()).Contains(property))
                return ary.Select(e => ((IDictionary)e)[property]);

            return ary.Select(e => {
                if (e == null)
                    return null;

                var drop = e as DropBase;
                if (drop == null)
                {
                    var type = e.GetType();
                    var safeTypeTransformer = Template.GetSafeTypeTransformer(type);
                    if (safeTypeTransformer != null)
                        drop = safeTypeTransformer(e) as DropBase;
                    else
                    {
                        var attr = type.GetTypeInfo().GetCustomAttributes(typeof(LiquidTypeAttribute), false).FirstOrDefault() as LiquidTypeAttribute;
                        if (attr != null)
                        {
                            drop = new DropProxy(e, attr.AllowedMembers);
                        }
                        else if (TypeUtility.IsAnonymousType(type))
                        {
                            return e.RespondTo(property) ? e.Send(property) : e;
                        }
                    }
                }
                return (drop?.ContainsKey(property) ?? false) ? drop[property] : null;
            });

        }

        /// <summary>
        /// Replace occurrences of a string with another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string replace(string input, string @string, string replacement = "")
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(@string))
                return input;

            return string.IsNullOrEmpty(input)
                ? input
                : Regex.Replace(input, @string, replacement, RegexOptions.None, Template.RegexTimeOut);
        }

        /// <summary>
        /// Replace the first occurence of a string with another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string replace_first(string input, string @string, string replacement = "")
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(@string))
                return input;

            bool doneReplacement = false;
            return Regex.Replace(input, @string, m =>
            {
                if (doneReplacement)
                    return m.Value;

                doneReplacement = true;
                return replacement;
            }, RegexOptions.None, Template.RegexTimeOut);
        }

        /// <summary>
        /// Remove a substring
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string remove(string input, string @string)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : input.Replace(@string, string.Empty);
        }

        /// <summary>
        /// Remove the first occurrence of a substring
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string remove_first(string input, string @string)
        {
            return input.IsNullOrWhiteSpace()
                ? input
                : replace_first(input, @string, string.Empty);
        }

        /// <summary>
        /// Add one string to another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string append(string input, string @string)
        {
            return input == null
                ? input
                : input + @string;
        }

        /// <summary>
        /// Prepend a string to another
        /// </summary>
        /// <param name="input"></param>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string prepend(string input, string @string)
        {
            return input == null
                ? input
                : @string + input;
        }

        /// <summary>
        /// Add <br /> tags in front of all newlines in input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string newline_to_br(string input)
        {
            return input.IsNullOrWhiteSpace()
                    ? input
                    : Regex.Replace(input, @"(\r?\n)", "<br />$1", RegexOptions.None, Template.RegexTimeOut);
        }

        /// <summary>
        /// Formats a date using a .NET date format string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string date(object input, string format)
        {

            if (input == null)
                return null;

            DateTime date;
            if (input is DateTime)
            {
                date = (DateTime)input;

                if (format.IsNullOrWhiteSpace())
                    return date.ToString();
            }
            else
            {
                string value = input.ToString();

                if (string.Equals(value, "now", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "today", StringComparison.OrdinalIgnoreCase))
                {
                    date = DateTime.Now;

                    if (format.IsNullOrWhiteSpace())
                        return date.ToString();
                }
                else if (!DateTime.TryParse(value, out date))
                {
                    return value;
                }

                if (format.IsNullOrWhiteSpace())
                    return value;
            }

            return Liquid.UseRubyDateFormat ? date.ToStrFTime(format) : date.ToString(format);

        }

        /// <summary>
        /// Get the first element of the passed in array
        ///
        /// Example:
        ///   {{ product.images | first | to_img }}
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object first(IEnumerable array)
        {
            if (array == null)
                return null;

            return array.Cast<object>().FirstOrDefault();
        }

        /// <summary>
        /// Get the last element of the passed in array
        ///
        /// Example:
        ///   {{ product.images | last | to_img }}
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object last(IEnumerable array)
        {
            if (array == null)
                return null;

            return array.Cast<object>().LastOrDefault();
        }

        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object plus(object input, object operand)
        {
            return input is string
                ? string.Concat(input, operand)
                : DoMathsOperation(input, operand, Expression.Add);
        }

        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object minus(object input, object operand)
        {
            return DoMathsOperation(input, operand, Expression.Subtract);
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object times(object input, object operand)
        {
            return input is string && operand is int
                ? Enumerable.Repeat((string)input, (int)operand)
                : DoMathsOperation(input, operand, Expression.Multiply);
        }

        /// <summary>
        /// Rounds a decimal value to the specified places
        /// </summary>
        /// <param name="input"></param>
        /// <param name="places"></param>
        /// <returns>The rounded value; null if an exception have occured</returns>
        public static object round(object input, object places = null)
        {
            try
            {
                var p = places == null ? 0 : Convert.ToInt32(places);
                var i = Convert.ToDecimal(input);
                return Math.Round(i, p);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Division
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object divided_by(object input, object operand)
        {
            return DoMathsOperation(input, operand, Expression.Divide);
        }

        /// <summary>
        /// Performs an arithmetic remainder operation on the input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static object modulo(object input, object operand)
        {
            return DoMathsOperation(input, operand, Expression.Modulo);
        }

        /// <summary>
        /// If a value isn't set for a variable in the template, allow the user to specify a default value for that variable
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Default(string input, string @defaultValue)
        {
            return !string.IsNullOrWhiteSpace(input) ? input : defaultValue;
        }

        private static bool IsReal(object o) => o is double || o is float;

        private static object DoMathsOperation(object input, object operand, Func<Expression, Expression, BinaryExpression> operation)
        {
            if (input == null || operand == null)
                return null;

            if (IsReal(input) || IsReal(operand))
            {
                input = Convert.ToDouble(input);
                operand = Convert.ToDouble(operand);
            }

            return ExpressionUtility.CreateExpression
                                    (body: operation
                                      , leftType: input.GetType()
                                      , rightType: operand.GetType())
                                    .DynamicInvoke(input, operand);
        }
    }

    internal static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrEmpty(s) || s.Trim().Length == 0;
        }
    }
}