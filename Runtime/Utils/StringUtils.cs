#region

using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace BioluminescentGames.Utils
{
    public static class StringUtils
    {
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in str)
            {
                if (char.IsUpper(c) && stringBuilder.Length > 0)
                {
                    stringBuilder.Append('_');
                }
                stringBuilder.Append(char.ToLower(c));
            }

            return stringBuilder.ToString();
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            StringBuilder stringBuilder = new StringBuilder();
            bool capitalizeNext = true;

            foreach (char c in str)
            {
                if (c == '_')
                {
                    capitalizeNext = true;
                }
                else
                {
                    stringBuilder.Append(capitalizeNext ? char.ToUpper(c) : c);
                    capitalizeNext = false;
                }
            }

            return stringBuilder.ToString();
        }

        public static bool MatchesRegex(this string str, string regex, RegexOptions options = RegexOptions.IgnoreCase)
        {
            Regex dbgProfileArgRegex = new Regex(regex, options);
            return str.MatchesRegex(dbgProfileArgRegex);
        }

        public static bool MatchesRegex(this string str, Regex regex)
        {
            return regex.IsMatch(str);
        }

        public static byte[] ToByteArray(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
