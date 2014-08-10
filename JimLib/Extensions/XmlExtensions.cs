using System.Xml.Linq;

namespace JimBobBennett.JimLib.Extensions
{
	/// <summary>
	/// XML Extension Methods
	/// </summary>
	internal static class XmlExtensions
	{
		/// <summary>
		/// Returns the name of an element with the namespace if specified
		/// </summary>
		/// <param name="name">Element name</param>
		/// <param name="namespace">XML Namespace</param>
		/// <returns>The namespaced XName</returns>
        internal static XName AsNamespaced(this string name, string @namespace)
        {
			XName xName = name;

			if (!@namespace.IsNullOrEmpty())
				xName = XName.Get(name, @namespace);

			return xName;
		}
	}
}
