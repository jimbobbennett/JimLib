using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using FluentAssertions;
using JimBobBennett.JimLib.Collections;
using JimBobBennett.JimLib.Xml;
using NUnit.Framework;

namespace JimBobBennett.JimLib.Test.Xml
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable ClassNeverInstantiated.Global
    // ReSharper disable UnusedMember.Global
    // ReSharper disable InconsistentNaming

    public class InnerItem
    {
        public string First { get; set; }
        public string Second { get; set; }
    }

    public class MappedInnerItem
    {
        [XmlNameMapping("First")]
        public string FooBar { get; set; }
        public string Second { get; set; }
    }

    public class LowerCaseInnerItem
    {
        public string first { get; set; }
        public string second { get; set; }
    }

    public class MultiWordInnerItem
    {
        public string FirstProperty { get; set; }
        public string SecondProperty { get; set; }
    }

    public class ItemWithInners
    {
        public string Third { get; set; }
        public List<InnerItem> InnerItems { get; set; }
    }

    public class ItemWithObservableInners
    {
        public string Third { get; set; }
        public ObservableCollection<InnerItem> InnerItems { get; set; }
    }

    public class ItemWithObservableExInners
    {
        public string Third { get; set; }
        public ObservableCollectionEx<InnerItem> InnerItems { get; set; }
    }

    public class MyItemsList : List<InnerItem> { }

    public class ItemWithTypes
    {
        public int Integer { get; set; }
        public long Long { get; set; }
        public bool Bool { get; set; }
        public double Double { get; set; }
        public decimal Decimal { get; set; }

        public bool? NullableBool1 { get; set; }
        public bool? NullableBool2 { get; set; }
        public bool? NullableBool3 { get; set; }

        public MyEnum MyEnum { get; set; }

        public Uri Uri { get; set; }

        public DateTime Today { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
    }

    public enum MyEnum
    {
        One,
        Two,
        Three
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore UnusedMember.Global
    // ReSharper restore ClassNeverInstantiated.Global
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    [TestFixture]
    public class XmlDeserializerTest
    {
        private const string InnerItemXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                            "<InnerItem First=\"Hello\" Second=\"World\"/>";

        private const string InnerItemsXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                             "<InnerItems>" +
                                             "  <InnerItem First=\"Hello\" Second=\"World\"/>" +
                                             "</InnerItems>";

        private const string InnerItemInLowercaseXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                       "<InnerItem first=\"Hello\" second=\"World\"/>";

        private const string MultiWordInnerItemXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                     "<MultiWordInnerItem FirstProperty=\"Hello\" SecondProperty=\"World\"/>";

        private const string MultiWordInnerItemInCamelcaseXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                                "<MultiWordInnerItem firstProperty=\"Hello\" secondProperty=\"World\"/>";

        private const string LowerCaseInnerItemXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                     "<LowerCaseInnerItem first=\"Hello\" second=\"World\"/>";

        private const string ItemWithInnersXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                 "<ItemWithInners Third=\"FooBar\">" +
                                                 "  <InnerItem first=\"Hello\" second=\"World\"/>" +
                                                 "</ItemWithInners>";

        private static readonly string ItemWithTypesXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                                          "<ItemWithTypes Integer=\"1\" Long=\"3000\" " +
                                                          "  Bool=\"true\" NullableBool2=\"true\" NullableBool3=\"\"" +
                                                          "  MyEnum=\"Two\" Double=\"1.1\" Decimal=\"4.9\" " +
                                                          "  Uri=\"http://google.com\" Today=\"" + DateTime.Now + "\"" +
                                                          "  DateTimeOffset=\"" + DateTimeOffset.Now + "\"" +
                                                          "/>";
         
        [Test]
        public void ReadSimpleClassWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<InnerItem>(InnerItemXml);

            item.First.Should().Be("Hello");
            item.Second.Should().Be("World");
        }

        [Test]
        public void ReadSimpleMappedClassWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<MappedInnerItem>(InnerItemXml);

            item.FooBar.Should().Be("Hello");
            item.Second.Should().Be("World");
        }

        [Test]
        public void ReadSimpleClassWithLowerCaseAttribtuesWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<LowerCaseInnerItem>(LowerCaseInnerItemXml);

            item.first.Should().Be("Hello");
            item.second.Should().Be("World");
        }

        [Test]
        public void ReadClassWithInnersWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<ItemWithInners>(ItemWithInnersXml);

            item.Third.Should().Be("FooBar");
            item.InnerItems.Should().HaveCount(1);
            item.InnerItems.Should().OnlyContain(i => i.First == "Hello" && i.Second == "World");
        }

        [Test]
        public void ReadClassWithObservableCollectionOfInnersWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<ItemWithObservableInners>(ItemWithInnersXml);

            item.Third.Should().Be("FooBar");
            item.InnerItems.Should().HaveCount(1);
            item.InnerItems.Should().OnlyContain(i => i.First == "Hello" && i.Second == "World");
        }

        [Test]
        public void ReadClassWithObservableCollectionExOfInnersWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<ItemWithObservableExInners>(ItemWithInnersXml);

            item.Third.Should().Be("FooBar");
            item.InnerItems.Should().HaveCount(1);
            item.InnerItems.Should().OnlyContain(i => i.First == "Hello" && i.Second == "World");
        }

        [Test]
        public void ReadClassWithInnersUsingDerviedTypeWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<MyItemsList>(InnerItemsXml);

            item.Should().HaveCount(1);
            item.Should().OnlyContain(i => i.First == "Hello" && i.Second == "World");
        }

        [Test]
        public void ReadingWhenClassHasUpperCaseAndAttributesAreLowerMapsCorrectly()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<InnerItem>(InnerItemInLowercaseXml);

            item.First.Should().Be("Hello");
            item.Second.Should().Be("World");
        }

        [Test]
        public void ReadingMultiWordAttributesWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<MultiWordInnerItem>(MultiWordInnerItemXml);

            item.FirstProperty.Should().Be("Hello");
            item.SecondProperty.Should().Be("World");
        }

        [Test]
        public void ReadingMultiWordAttributesInCamelCaseWorks()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<MultiWordInnerItem>(MultiWordInnerItemInCamelcaseXml);

            item.FirstProperty.Should().Be("Hello");
            item.SecondProperty.Should().Be("World");
        }

        [Test]
        public void DeserializingNullReturnsDefault()
        {
            var deserializer = new XmlDeserializer();

            var item = deserializer.Deserialize<MultiWordInnerItem>(null);

            item.Should().BeNull();
        }

        [Test]
        public void TypesOtherThanStringAreReadCorrectly()
        {
            var now = DateTime.Now;

            var trimmedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            var trimmedNowOffSet = new DateTimeOffset(trimmedNow);

            var deserializer = new XmlDeserializer {Culture = CultureInfo.CurrentCulture};
            var item = deserializer.Deserialize<ItemWithTypes>(ItemWithTypesXml);

            item.Integer.Should().Be(1);
            item.Long.Should().Be(3000L);
            item.Bool.Should().BeTrue();
            item.NullableBool1.HasValue.Should().BeFalse();
            item.NullableBool2.Should().BeTrue();
            item.NullableBool3.HasValue.Should().BeFalse();
            item.MyEnum.Should().Be(MyEnum.Two);
            item.Double.Should().Be(1.1);
            item.Decimal.Should().Be(4.9m);
            item.Uri.AbsoluteUri.Should().Be("http://google.com/");
            item.Today.Should().BeOnOrAfter(trimmedNow).And.BeOnOrBefore(DateTime.Now);
            item.DateTimeOffset.Should().BeOnOrAfter(trimmedNowOffSet).And.BeOnOrBefore(DateTimeOffset.Now);
        }
    }
}
