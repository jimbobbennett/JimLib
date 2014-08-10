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
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Gets if the given string is null or whitespace
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>true if the string is null or whitespace, otherwise false</returns>
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
        public static string Decrypt(this string s, string password)
        {
            return Aesgcm.SimpleDecryptWithPassword(s, password);
        }
    }
}
