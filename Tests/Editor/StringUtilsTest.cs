using BioluminescentGames.Utils.Utilities;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace BioluminescentGames.Utils.Tests.Editor
{
    public class StringUtilsTest
    {
        [Test]
        public void ToSnakeCase_ConvertsPascalCaseToSnakeCase()
        {
            const string input = "PascalCaseText";
            const string expected = "pascal_case_text";

            string result = input.ToSnakeCase();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToSnakeCase_HandlesEmptyString()
        {
            const string input = "";
            const string expected = "";

            string result = input.ToSnakeCase();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToSnakeCase_HandlesNullString()
        {
            string input = null;

            string result = input.ToSnakeCase();

            Assert.IsNull(result);
        }

        [Test]
        public void ToSnakeCase_HandlesSingleUpperCaseLetters()
        {
            const string input = "ABC";
            const string expected = "a_b_c";

            string result = input.ToSnakeCase();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToPascalCase_ConvertsSnakeCaseToPascalCase()
        {
            const string input = "snake_case_text";
            const string expected = "SnakeCaseText";

            string result = input.ToPascalCase();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToPascalCase_HandlesEmptyString()
        {
            const string input = "";
            const string expected = "";

            string result = input.ToPascalCase();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToPascalCase_HandlesNullString()
        {
            string input = null;

            string result = input.ToPascalCase();

            Assert.IsNull(result);
        }

        [Test]
        public void ToPascalCase_HandlesMultipleUnderscores()
        {
            const string input = "___snake_case___text___";
            const string expected = "SnakeCaseText";

            string result = input.ToPascalCase();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void MatchesRegex_WithStringPattern_ReturnsTrueForMatchingText()
        {
            const string input = "hello world";
            const string pattern = "hello";

            bool result = input.MatchesRegex(pattern);

            Assert.IsTrue(result);
        }

        [Test]
        public void MatchesRegex_WithStringPattern_ReturnsFalseForNonMatchingText()
        {
            const string input = "hello world";
            const string pattern = "worlds";

            bool result = input.MatchesRegex(pattern);

            Assert.IsFalse(result);
        }

        [Test]
        public void MatchesRegex_WithRegexInstance_ReturnsTrueForMatchingText()
        {
            const string input = "hello world";
            Regex regex = new("hello");

            bool result = input.MatchesRegex(regex);

            Assert.IsTrue(result);
        }

        [Test]
        public void MatchesRegex_WithRegexInstance_ReturnsFalseForNonMatchingText()
        {
            const string input = "hello world";
            Regex regex = new("worlds");

            bool result = input.MatchesRegex(regex);

            Assert.IsFalse(result);
        }

        [Test]
        public void MatchesRegex_HandlesEmptyString()
        {
            const string input = "";
            const string pattern = ".*";

            bool result = input.MatchesRegex(pattern);

            Assert.IsTrue(result);
        }

        [Test]
        public void ToByteArray_ConvertsStringToUTF8ByteArray()
        {
            const string input = "Hello";
            byte[] expected = { 72, 101, 108, 108, 111 };

            byte[] result = input.ToByteArray();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToByteArray_HandlesEmptyString()
        {
            const string input = "";
            byte[] expected = { };

            byte[] result = input.ToByteArray();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToByteArray_HandlesNullString()
        {
            string input = null;

            byte[] result = input.ToByteArray();

            Assert.IsNull(result);
        }
    }
}
