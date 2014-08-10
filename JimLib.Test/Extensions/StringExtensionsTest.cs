using System;
using FluentAssertions;
using JimBobBennett.JimLib.Extensions;
using NUnit.Framework;
using Org.BouncyCastle.Security;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {
        #region IsNullOrEmpty

        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("FooBar", false)]
        public void IsNullOrEmptyReturnsCorrectValue(string s, bool expected)
        {
            s.IsNullOrEmpty().Should().Be(expected);
        }

        #endregion IsNullOrEmpty

        #region IsNullOrWhiteSpace

        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase("  ", true)]
        [TestCase("\t", true)]
        [TestCase("\t\t", true)]
        [TestCase(" FooBar", false)]
        [TestCase("FooBar ", false)]
        [TestCase("\tFooBar", false)]
        [TestCase("FooBar\t", false)]
        public void IsNullOrWhiteSpaceReturnsCorrectValue(string s, bool expected)
        {
            s.IsNullOrWhiteSpace().Should().Be(expected);
        }

        #endregion IsNullOrWhiteSpace

        #region Encrypt/Decrypt

        [Test]
        public void TestEncryptedStringDoesNotMatchOriginalString()
        {
            const string s = "FooBar";
            s.Encrypt("Password123456").Should().NotBe(s);
        }

        [Test]
        public void TestEncryptedStringCanBeDecryptedWithTheSamePassword()
        {
            const string s = "FooBar";
            s.Encrypt("Password123456").Decrypt("Password123456").Should().Be(s);
        }

        [Test]
        [ExpectedException(typeof(PasswordException))]
        public void TestDecryptingWithTheWrongPasswordThrows()
        {
            const string s = "FooBar";
            s.Encrypt("Password123456").Decrypt("123456Password");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEncryptingANullStringThrows()
        {
            string s = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            s.Encrypt("Password123456");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEncryptingWithANullPasswordThrows()
        {
            const string s = "FooBar";
            s.Encrypt(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEncryptingWithAPasswordLessThan12CharactersThrows()
        {
            const string s = "FooBar";
            s.Encrypt("password");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDecryptingANullStringThrows()
        {
            string s = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            s.Decrypt("Password123456");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDecryptingWithANullPasswordThrows()
        {
            const string s = "FooBar";
            s.Encrypt("Password123456").Decrypt(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDecryptingWithAPasswordLessThan12ChanractersThrows()
        {
            const string s = "FooBar";
            s.Encrypt("Password123456").Decrypt("password");
        }

        #endregion Encrypt/Decrypt

        #region IsUpperCase

        [TestCase("ABCD", true)]
        [TestCase("Abcd", false)]
        [TestCase("abcd", false)]
        [TestCase("", false)]
        public void IsUpperCaseReturnsTheCorrectValue(string s, bool expected)
        {
            s.IsUpperCase().Should().Be(expected);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void IsUpperCaseThrowsOnNull()
        {
            string s = null;
// ReSharper disable once ExpressionIsAlwaysNull
            s.IsUpperCase();
        }

        #endregion IsUpperCase

        #region MakeInitialLowerCase

        [TestCase("Hello", "hello")]
        [TestCase("hello", "hello")]
        [TestCase("HELLO", "hELLO")]
        public void MakeInitialLowerCaseMakesTheFirstLetterLowerCase(string s, string expected)
        {
            s.MakeInitialLowerCase().Should().Be(expected);
        }

        #endregion MakeInitialLowerCase

        #region ToCamelCase

        [TestCase("Hello", "hello")]
        [TestCase("Hello world", "helloWorld")]
        [TestCase("hello world", "helloWorld")]
        [TestCase("HELLO WORLD", "helloWorld")]
        public void ToCamelCaseConvertsTheStringToCamelCase(string s, string expected)
        {
            s.ToCamelCase().Should().Be(expected);
        }

        #endregion ToCamelCase

        #region ToPascalCase

        [TestCase("Hello", "Hello")]
        [TestCase("Hello world", "HelloWorld")]
        [TestCase("hello world", "HelloWorld")]
        [TestCase("HELLO WORLD", "HelloWorld")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void ToPascalCaseConvertsTheStringToPascalCase(string s, string expected)
        {
            s.ToPascalCase().Should().Be(expected);
        }

        #endregion ToPascalCase

        #region RemoveUnderscoresAndDashes

        [TestCase("Hello", "Hello")]
        [TestCase("Hello_World", "HelloWorld")]
        [TestCase("Hello-World", "HelloWorld")]
        [TestCase("Hello-World_Foo-Bar", "HelloWorldFooBar")]
        public void RemoveUnderscoresAndDashesRemovesAllUnderscoresAndDashes(string s, string expected)
        {
            s.RemoveUnderscoresAndDashes().Should().Be(expected);
        }

        #endregion RemoveUnderscoresAndDashes
    }
}
