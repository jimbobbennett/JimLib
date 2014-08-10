using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;
using JimBobBennett.JimLib.Encryption;

namespace JimBobBennett.JimLib.Extensions
{
    /// <summary>
    /// Extension methods for string
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets if the given string is null or empty
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>true if the string is null or empty, otherwise false</returns>
        [Pure]
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Gets if the given string is null or whitespace
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>true if the string is null or whitespace, otherwise false</returns>
        [Pure]
        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Encrypts the given string using the given password
        /// </summary>
        /// <param name="s">The string to encrypt</param>
        /// <param name="password">The password to use to encrypt</param>
        /// <returns>The encrypted string</returns>
        [Pure]
        public static string Encrypt(this string s, string password)
        {
            return Aesgcm.SimpleEncryptWithPassword(s, password);
        }

        /// <summary>
        /// Decrypts the given string using the given password
        /// </summary>
        /// <param name="s">The string to encrypt</param>
        /// <param name="password">password key to used to encrypt the string</param>
        /// <returns>The decrypted string</returns>
        [Pure]
        public static string Decrypt(this string s, string password)
        {
            return Aesgcm.SimpleDecryptWithPassword(s, password);
        }

        /// <summary>
        /// Remove underscores from a string
        /// </summary>
        /// <param name="input">String to process</param>
        /// <returns>The string without any underscores or dashes (_ or -)</returns>
        [Pure]
        public static string RemoveUnderscoresAndDashes(this string input)
        {
            return input.Replace("_", "").Replace("-", "");
        }
        /// <summary>
        /// Converts a string to pascal case with the option to remove underscores
        /// </summary>
        /// <param name="text">String to convert</param>
        /// <param name="removeUnderscores">Option to remove underscores</param>
        /// <returns></returns>
        [Pure]
        public static string ToPascalCase(this string text, bool removeUnderscores = true)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = text.Replace("_", " ");

            var joinString = removeUnderscores ? string.Empty : "_";
            var words = text.Split(' ');

            if (words.Length <= 1 && !words[0].IsUpperCase())
                return string.Concat(words[0].Substring(0, 1).ToUpper(), words[0].Substring(1));

            for (var i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    var word = words[i];
                    var restOfWord = word.Substring(1);

                    if (restOfWord.IsUpperCase())
                        restOfWord = restOfWord.ToLower();

                    var firstChar = char.ToUpper(word[0]);
                    words[i] = string.Concat(firstChar, restOfWord);
                }
            }

            return string.Join(joinString, words);
        }

        /// <summary>
        /// Converts a string to camel case
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">String to convert</param>
        /// <returns>String</returns>
        [Pure]
        public static string ToCamelCase(this string lowercaseAndUnderscoredWord)
        {
            return MakeInitialLowerCase(ToPascalCase(lowercaseAndUnderscoredWord));
        }

        /// <summary>
        /// Convert the first letter of a string to lower case
        /// </summary>
        /// <param name="word">String to convert</param>
        /// <returns>string</returns>
        [Pure]
        public static string MakeInitialLowerCase(this string word)
        {
            return string.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
        }

        /// <summary>
        /// Checks to see if a string is all uppper case
        /// </summary>
        /// <param name="inputString">String to check</param>
        /// <returns>bool</returns>
        [Pure]
        public static bool IsUpperCase(this string inputString)
        {
            return Regex.IsMatch(inputString, @"^[A-Z]+$");
        }
    }
}
