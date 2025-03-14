using System.Collections;
using System.Collections.Specialized;
using System.Xml;

namespace System.Configuration;

internal class ConfigHelper
{
	private class CollectionWrapper
	{
		private IDictionary dict;

		private NameValueCollection collection;

		private bool isDict;

		public string this[string key]
		{
			set
			{
				if (isDict)
				{
					dict[key] = value;
				}
				else
				{
					collection[key] = value;
				}
			}
		}

		public CollectionWrapper(IDictionary dict)
		{
			this.dict = dict;
			isDict = true;
		}

		public CollectionWrapper(NameValueCollection collection)
		{
			this.collection = collection;
			isDict = false;
		}

		public void Remove(string s)
		{
			if (isDict)
			{
				dict.Remove(s);
			}
			else
			{
				collection.Remove(s);
			}
		}

		public void Clear()
		{
			if (isDict)
			{
				dict.Clear();
			}
			else
			{
				collection.Clear();
			}
		}

		public object UnWrap()
		{
			if (isDict)
			{
				return dict;
			}
			return collection;
		}
	}

	internal static IDictionary GetDictionary(IDictionary prev, XmlNode region, string nameAtt, string valueAtt)
	{
		Hashtable dict;
		if (prev == null)
		{
			dict = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		}
		else
		{
			Hashtable hashtable = (Hashtable)prev;
			dict = (Hashtable)hashtable.Clone();
		}
		CollectionWrapper result = new CollectionWrapper(dict);
		result = GoGetThem(result, region, nameAtt, valueAtt);
		if (result == null)
		{
			return null;
		}
		return result.UnWrap() as IDictionary;
	}

	internal static System.Configuration.ConfigNameValueCollection GetNameValueCollection(NameValueCollection prev, XmlNode region, string nameAtt, string valueAtt)
	{
		System.Configuration.ConfigNameValueCollection configNameValueCollection = new System.Configuration.ConfigNameValueCollection(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		if (prev != null)
		{
			configNameValueCollection.Add(prev);
		}
		CollectionWrapper result = new CollectionWrapper(configNameValueCollection);
		result = GoGetThem(result, region, nameAtt, valueAtt);
		if (result == null)
		{
			return null;
		}
		return result.UnWrap() as System.Configuration.ConfigNameValueCollection;
	}

	private static CollectionWrapper GoGetThem(CollectionWrapper result, XmlNode region, string nameAtt, string valueAtt)
	{
		if (region.Attributes != null && region.Attributes.Count != 0 && (region.Attributes.Count != 1 || region.Attributes[0].Name != "xmlns"))
		{
			throw new ConfigurationException("Unknown attribute", region);
		}
		XmlNodeList childNodes = region.ChildNodes;
		foreach (XmlNode item in childNodes)
		{
			switch (item.NodeType)
			{
			default:
				throw new ConfigurationException("Only XmlElement allowed", item);
			case XmlNodeType.Element:
				switch (item.Name)
				{
				case "clear":
					if (item.Attributes != null && item.Attributes.Count != 0)
					{
						throw new ConfigurationException("Unknown attribute", item);
					}
					result.Clear();
					break;
				case "remove":
				{
					XmlNode xmlNode2 = null;
					if (item.Attributes != null)
					{
						xmlNode2 = item.Attributes.RemoveNamedItem(nameAtt);
					}
					if (xmlNode2 == null)
					{
						throw new ConfigurationException("Required attribute not found", item);
					}
					if (xmlNode2.Value == string.Empty)
					{
						throw new ConfigurationException("Required attribute is empty", item);
					}
					if (item.Attributes.Count != 0)
					{
						throw new ConfigurationException("Unknown attribute", item);
					}
					result.Remove(xmlNode2.Value);
					break;
				}
				case "add":
				{
					XmlNode xmlNode2 = null;
					if (item.Attributes != null)
					{
						xmlNode2 = item.Attributes.RemoveNamedItem(nameAtt);
					}
					if (xmlNode2 == null)
					{
						throw new ConfigurationException("Required attribute not found", item);
					}
					if (xmlNode2.Value == string.Empty)
					{
						throw new ConfigurationException("Required attribute is empty", item);
					}
					XmlNode xmlNode3 = item.Attributes.RemoveNamedItem(valueAtt);
					if (xmlNode3 == null)
					{
						throw new ConfigurationException("Required attribute not found", item);
					}
					if (item.Attributes.Count != 0)
					{
						throw new ConfigurationException("Unknown attribute", item);
					}
					result[xmlNode2.Value] = xmlNode3.Value;
					break;
				}
				default:
					throw new ConfigurationException("Unknown element", item);
				}
				break;
			case XmlNodeType.Comment:
			case XmlNodeType.Whitespace:
				break;
			}
		}
		return result;
	}
}
