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
    }
}
