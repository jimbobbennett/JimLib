using System;
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

            public virtual void AMethod() { }

            public event EventHandler AEvent;
        }

        public class B : A
        {
            public string BString { get; set; }
            public int BInt { get; set; }

            public void BMethod() { }

            public event EventHandler BEvent;
        }

        public class C : B
        {
            public string CString { get; set; }
            public int CInt { get; set; }

            public void CMethod() { }

            public event EventHandler CEvent;
        }

        public class D : A
        {
            public override string VirtualString { get; set; }

            public override void AMethod() { }

            public event EventHandler DEvent;
        }

        [Test]
        public void BaseClassReturnsItsOwnProperties()
        {
            var properties = typeof(A).GetAllProperties().ToList();

            properties.Should().HaveCount(3);
            properties.Should().Contain(p => p.Name == "AString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "AInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "VirtualString" && p.PropertyType == typeof(string));
        }

        [Test]
        public void DerivedClassReturnsItsOwnPropertiesAndThoseOfTheBase()
        {
            var properties = typeof(B).GetAllProperties().ToList();

            properties.Should().HaveCount(5);
            properties.Should().Contain(p => p.Name == "AString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "AInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "VirtualString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "BString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "BInt" && p.PropertyType == typeof(int));
        }

        [Test]
        public void DerivedClassReturnsItsOwnPropertiesAndThoseOfAllBaseClasses()
        {
            var properties = typeof(C).GetAllProperties().ToList();

            properties.Should().HaveCount(7);
            properties.Should().Contain(p => p.Name == "AString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "AInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "VirtualString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "BString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "BInt" && p.PropertyType == typeof(int));
            properties.Should().Contain(p => p.Name == "CString" && p.PropertyType == typeof(string));
            properties.Should().Contain(p => p.Name == "CInt" && p.PropertyType == typeof(int));
        }

        [Test]
        public void GetAllPropertiesForOverriddenPropertiesDoesntHaveDuplicates()
        {
            var properties = typeof(D).GetAllProperties().ToList();
            var dictionary = properties.ToDictionary(p => p.Name, p => p);
        }

        [Test]
        public void BaseClassReturnsItsOwnMethods()
        {
            var methods = typeof(A).GetAllMethods().ToList();

            methods.Should().Contain(p => p.Name == "AMethod");
        }

        [Test]
        public void DerivedClassReturnsItsOwnMethodsAndThoseOfTheBase()
        {
            var methods = typeof(B).GetAllMethods().ToList();

            methods.Should().Contain(p => p.Name == "AMethod");
            methods.Should().Contain(p => p.Name == "BMethod");
        }

        [Test]
        public void DerivedClassReturnsItsOwnMethodsAndThoseOfAllBaseClasses()
        {
            var methods = typeof(C).GetAllMethods().ToList();

            methods.Should().Contain(p => p.Name == "AMethod");
            methods.Should().Contain(p => p.Name == "BMethod");
            methods.Should().Contain(p => p.Name == "CMethod");
        }

        [Test]
        public void GetAllMethodsForOverriddenMethodsDoesntHaveDuplicates()
        {
            var methods = typeof(D).GetAllMethods().ToList();
            var dictionary = methods.ToDictionary(m => m.Name, p => p);
        }

        [Test]
        public void BaseClassReturnsItsOwnEvents()
        {
            var events = typeof(A).GetAllEvents().ToList();

            events.Should().HaveCount(1);
            events.Should().Contain(p => p.Name == "AEvent");
        }

        [Test]
        public void DerivedClassReturnsItsOwnEventsAndThoseOfTheBase()
        {
            var events = typeof(B).GetAllEvents().ToList();

            events.Should().HaveCount(2);
            events.Should().Contain(p => p.Name == "AEvent");
            events.Should().Contain(p => p.Name == "BEvent");
        }

        [Test]
        public void DerivedClassReturnsItsOwnEventsAndThoseOfAllBaseClasses()
        {
            var events = typeof(C).GetAllEvents().ToList();

            events.Should().HaveCount(3);
            events.Should().Contain(p => p.Name == "AEvent");
            events.Should().Contain(p => p.Name == "BEvent");
            events.Should().Contain(p => p.Name == "CEvent");
        }

        [Test]
        public void GetAllEventsForOverriddenEventsDoesntHaveDuplicates()
        {
            var events = typeof(D).GetAllEvents().ToList();
            var dictionary = events.ToDictionary(m => m.Name, p => p);
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
