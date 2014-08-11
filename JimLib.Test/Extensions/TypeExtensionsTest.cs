using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using JimBobBennett.JimLib.Extensions;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Extensions
{
    [TestFixture]
    public class TypeExtensionsTest
    {
        public class A
        {
            public string AString { get; set; }
            public int AInt { get; set; }
            public virtual string VirtualString { get; set; }
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

        public class D : A
        {
            public override string VirtualString { get; set; }
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

        [Test]
        public void GetAllPropertiesForOverriddenPropertiesDoesntHaveDuplicates()
        {
            var properties = typeof(D).GetAllProperties().ToList();
            var propDict = properties.ToDictionary(p => p.Name, p => p);
        }

        [Test]
        public void IsSubTypeOfRawGenericReturnsTrueForClass()
        {
            var s = new List<string>();
            s.GetType().IsSubclassOfRawGeneric(typeof(List<>)).Should().BeTrue();
        }

        [Test]
        public void IsSubTypeOfRawGenericReturnsTrueForDerivedClass()
        {
            var s = new ObservableCollectionEx<string>();
            s.GetType().IsSubclassOfRawGeneric(typeof(ObservableCollection<>)).Should().BeTrue();
        }

        [Test]
        public void IsSubTypeOfRawGenericReturnsFalseForDifferentClass()
        {
            var s = new ObservableCollectionEx<string>();
            s.GetType().IsSubclassOfRawGeneric(typeof(List<>)).Should().BeFalse();
        }
    }
}
