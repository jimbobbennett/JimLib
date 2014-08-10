using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using JimBobBennett.JimLib.Extensions;

namespace JimBobBennett.JimLib.Xml
{
    public sealed class XmlDeserializer
	{
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public CultureInfo Culture { get; set; }

		public XmlDeserializer()
		{
			Culture = CultureInfo.InvariantCulture;
		}

        [Pure]
		public T Deserialize<T>(string response)
		{
			if (string.IsNullOrEmpty( response ))
				return default(T);

			var doc = XDocument.Parse(response);
			var root = doc.Root;
			if (!RootElement.IsNullOrEmpty() && doc.Root != null)
				root = doc.Root.Element(RootElement.AsNamespaced(Namespace));

			// autodetect xml namespace
            if (Namespace.IsNullOrEmpty())
				RemoveNamespace(doc);

			var x = Activator.CreateInstance<T>();
			var objType = x.GetType();

			if (objType.IsSubclassOfRawGeneric(typeof(List<>)))
				x = (T)HandleListDerivative(root, objType.Name, objType);
			else
				Map(x, root);

			return x;
		}

		private static void RemoveNamespace(XDocument xdoc)
		{
			foreach (var e in xdoc.Root.DescendantsAndSelf())
			{
				if (e.Name.Namespace != XNamespace.None)
					e.Name = XNamespace.None.GetName(e.Name.LocalName);
				
				if (e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
					e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
			}
		}

	    private void Map(object x, XElement root)
		{
			var objType = x.GetType().GetTypeInfo();
			var props = objType.DeclaredProperties.Union(objType.BaseType.GetTypeInfo().DeclaredProperties);

			foreach (var prop in props)
			{
				var type = prop.PropertyType;

				if (!type.GetTypeInfo().IsPublic || !prop.CanWrite)
					continue;

				var name = prop.Name.AsNamespaced(Namespace);
				var value = GetValueFromXml(root, name);

				if (value == null)
				{
					// special case for inline list items
					if (type.GetTypeInfo().IsGenericType)
					{
                        var genericType = type.GenericTypeArguments[0];
						var first = GetElementByName(root, genericType.Name);
						var list = (IList)Activator.CreateInstance(type);

						if (first != null)
						{
							var elements = root.Elements(first.Name);
							PopulateListFromElements(genericType, elements, list);
						}

						prop.SetValue(x, list, null);
					}
					continue;
				}

				// check for nullable and extract underlying type
				if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					// if the value is empty, set the property to null...
					if (string.IsNullOrEmpty(value.ToString()))
					{
						prop.SetValue(x, null, null);
						continue;
					}
                    type = type.GenericTypeArguments[0];
				}

				if (type == typeof(bool))
				{
					var toConvert = value.ToString().ToLower();
					prop.SetValue(x, XmlConvert.ToBoolean(toConvert), null);
				}
                else if (type.GetTypeInfo().IsPrimitive)
					prop.SetValue(x, Convert.ChangeType(value, type, null));
                else if (type.GetTypeInfo().IsEnum)
				{
					var converted = Enum.Parse(type, value.ToString(), true);
					prop.SetValue(x, converted, null);
				}
				else if (type == typeof(Uri))
				{
					var uri = new Uri(value.ToString(), UriKind.RelativeOrAbsolute);
					prop.SetValue(x, uri, null);
				}
				else if (type == typeof(string))
					prop.SetValue(x, value, null);
				else if (type == typeof(DateTime))
				{
				    value = !DateFormat.IsNullOrEmpty() ? DateTime.ParseExact(value.ToString(), DateFormat, Culture) : DateTime.Parse(value.ToString(), Culture);
				    prop.SetValue(x, value, null);
				}
				else if (type == typeof(DateTimeOffset))
				{
					var toConvert = value.ToString();
					if (!string.IsNullOrEmpty(toConvert))
					{
						DateTimeOffset deserialisedValue;
					    try
					    {
					        deserialisedValue = XmlConvert.ToDateTimeOffset(toConvert);
					        prop.SetValue(x, deserialisedValue, null);
					    }
					    catch (Exception)
					    {
					        //fallback to parse
					        deserialisedValue = DateTimeOffset.Parse(toConvert);
					        prop.SetValue(x, deserialisedValue, null);
					    }
					}
				}
				else if (type == typeof(Decimal))
				{
					value = Decimal.Parse(value.ToString(), Culture);
					prop.SetValue(x, value, null);
				}
				else if (type == typeof(Guid))
				{
					var raw = value.ToString();
					value = string.IsNullOrEmpty(raw) ? Guid.Empty : new Guid(value.ToString());
					prop.SetValue(x, value, null);
				}
				else if (type == typeof(TimeSpan))
				{
					var timeSpan = XmlConvert.ToTimeSpan(value.ToString());
					prop.SetValue(x, timeSpan, null);
				}
                else if (type.GetTypeInfo().IsGenericType)
				{
                    var t = type.GenericTypeArguments[0];
					var list = (IList)Activator.CreateInstance(type);

					var container = GetElementByName(root, prop.Name.AsNamespaced(Namespace));

					if (container.HasElements)
					{
						var first = container.Elements().FirstOrDefault();
						var elements = container.Elements(first.Name);
						PopulateListFromElements(t, elements, list);
					}

					prop.SetValue(x, list, null);
				}
				else if (type.IsSubclassOfRawGeneric(typeof (List<>)))
				{
				    // handles classes that derive from List<T>
				    // e.g. a collection that also has attributes
				    var list = HandleListDerivative(root, prop.Name, type);
				    prop.SetValue(x, list, null);
				}
				else
				{
				    // nested property classes
				    if (root != null)
				    {
				        var element = GetElementByName(root, name);
				        if (element != null)
				        {
				            var item = CreateAndMap(type, element);
				            prop.SetValue(x, item, null);
				        }
				    }
				}
			}
		}
        
        private void PopulateListFromElements(Type t, IEnumerable<XElement> elements, IList list)
        {
            foreach (var item in elements.Select(element => CreateAndMap(t, element)))
                list.Add(item);
        }

        private object HandleListDerivative(XElement root, string propName, Type type)
		{
            var t = type.GetTypeInfo().IsGenericType ? type.GetTypeInfo().GenericTypeArguments[0] : type.GetTypeInfo().BaseType.GenericTypeArguments[0];
            
			var list = (IList)Activator.CreateInstance(type);

			var elements = root.Descendants(t.Name.AsNamespaced(Namespace)).ToList();
			
			var name = t.Name;

			if (!elements.Any())
			{
				var lowerName = name.ToLower().AsNamespaced(Namespace);
                elements = root.Descendants(lowerName).ToList();
			}

			if (!elements.Any())
			{
				var camelName = name.ToCamelCase().AsNamespaced(Namespace);
                elements = root.Descendants(camelName).ToList();
			}

			if (!elements.Any())
                elements = root.Descendants().Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == name).ToList();

			if (!elements.Any())
			{
				var lowerName = name.ToLower().AsNamespaced(Namespace);
                elements = root.Descendants().Where(e => e.Name.LocalName.RemoveUnderscoresAndDashes() == lowerName).ToList();
			}

			PopulateListFromElements(t, elements, list);

			// get properties too, not just list items
			// only if this isn't a generic type
            if (!type.GetTypeInfo().IsGenericType)
				Map(list, root.Element(propName.AsNamespaced(Namespace)) ?? root); // when using RootElement, the heirarchy is different

			return list;
		}

	    private object CreateAndMap(Type t, XElement element)
		{
			object item;
			if (t == typeof(string))
				item = element.Value;
            else if (t.GetTypeInfo().IsPrimitive)
                item = Convert.ChangeType(element.Value, t, null);
			else
			{
				item = Activator.CreateInstance(t);
				Map(item, element);
			}

			return item;
		}

	    private static object GetValueFromXml(XElement root, XName name)
		{
			object val = null;

			if (root != null)
			{
				var element = GetElementByName(root, name);
				if (element == null)
				{
					var attribute = GetAttributeByName(root, name);
					if (attribute != null)
						val = attribute.Value;
				}
				else
				{
					if (!element.IsEmpty || element.HasElements || element.HasAttributes)
						val = element.Value;
				}
			}

			return val;
		}

	    private static XElement GetElementByName(XElement root, XName name)
		{
			var lowerName = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
			var camelName = name.LocalName.ToCamelCase().AsNamespaced(name.NamespaceName);

			if (root.Element(name) != null)
				return root.Element(name);

			if (root.Element(lowerName) != null)
				return root.Element(lowerName);

			if (root.Element(camelName) != null)
				return root.Element(camelName);

			if (name == "Value".AsNamespaced(name.NamespaceName))
				return root;

			// try looking for element that matches sanitized property name (Order by depth)
			var element = root.Descendants()
				.OrderBy(d => d.Ancestors().Count())
				.FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName) 
				?? root.Descendants()
				.OrderBy(d => d.Ancestors().Count())
				.FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName.ToLower());

			return element;
		}

	    private static XAttribute GetAttributeByName(XElement root, XName name)
		{
			var lowerName = name.LocalName.ToLower().AsNamespaced(name.NamespaceName);
			var camelName = name.LocalName.ToCamelCase().AsNamespaced(name.NamespaceName);

			if (root.Attribute(name) != null)
				return root.Attribute(name);

			if (root.Attribute(lowerName) != null)
				return root.Attribute(lowerName);

			if (root.Attribute(camelName) != null)
				return root.Attribute(camelName);

			// try looking for element that matches sanitized property name
	        return root.Attributes().FirstOrDefault(d => d.Name.LocalName.RemoveUnderscoresAndDashes() == name.LocalName);
		}
	}
}
