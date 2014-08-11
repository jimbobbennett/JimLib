using System;

namespace JimBobBennett.JimLib.Xml
{
    public class XmlNameMappingAttribute : Attribute
    {
        public XmlNameMappingAttribute(string mappedName)
        {
            MappedName = mappedName;
        }

        public string MappedName { get; set; }
    }
}
