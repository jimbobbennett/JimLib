using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JimLib.Extensions;
using NUnit.Framework;

namespace JimLib.Test.Extensions
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        public class A
        {
            public string AString { get; set; }
            public int AInt { get; set; }
        }

        public class B : A
        {
            public string BString { get; set; }
            public int BInt { get; set; }
        }

        public class C : B
        {
            public string CString { get; set; }
            public int CInt { get; set; }
        }

        [Test]
        public void BaseClassReturnsItsOwnProperties()
        {
            var properties = typeof(A).GetAllProperties().ToList();

            properties.Should().HaveCount(2);
            properties.Should().Contain(p => p.Name == "AString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "AInt" && p.PropertyType == typeof(int));
        }

        [Test]
        public void DerivedClassReturnsItsOwnPropertiesAndThoseOfTheBase()
        {
            var properties = typeof(B).GetAllProperties().ToList();

            properties.Should().HaveCount(4);
            properties.Should().Contain(p => p.Name == "AString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "AInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "BString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "BInt" && p.PropertyType == typeof(int));
        }

        [Test]
        public void DerivedClassReturnsItsOwnPropertiesAndThoseOfAllBaseClasses()
        {
            var properties = typeof(C).GetAllProperties().ToList();

            properties.Should().HaveCount(6);
            properties.Should().Contain(p => p.Name == "AString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "AInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "BString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "BInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "CString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "CInt" && p.PropertyType == typeof(int));
        }
    }
}
