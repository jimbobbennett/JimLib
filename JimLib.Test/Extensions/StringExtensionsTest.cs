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
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase("FooBar", false)]
        public void IsNullOrEmptyReturnsCorrectValue(string s, bool expected)
        {
            s.IsNullOrEmpty().Should().Be(expected);
        }

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
    }
}
