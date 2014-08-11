using System;

namespace JimBobBennett.JimLib.Xml
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class XmlNameMappingAttribute : Attribute
    {
        public XmlNameMappingAttribute(string mappedName)
        {
            MappedName = mappedName;
        }

        public string MappedName { get; set; }
    }
}
