using System.Collections.Generic;

namespace System.Xml;

public class XmlBinaryReaderSession : IXmlDictionary
{
	private XmlDictionary dic = new XmlDictionary();

	private Dictionary<int, XmlDictionaryString> store = new Dictionary<int, XmlDictionaryString>();

	public XmlDictionaryString Add(int id, string value)
	{
		XmlDictionaryString xmlDictionaryString = dic.Add(value);
		store[id] = xmlDictionaryString;
		return xmlDictionaryString;
	}

	public void Clear()
	{
		store.Clear();
	}

	public bool TryLookup(int key, out XmlDictionaryString result)
	{
		return store.TryGetValue(key, out result);
	}

	public bool TryLookup(string value, out XmlDictionaryString result)
	{
		foreach (XmlDictionaryString value2 in store.Values)
		{
			if (value2.Value == value)
			{
				result = value2;
				return true;
			}
		}
		result = null;
		return false;
	}

	public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
	{
		foreach (XmlDictionaryString value2 in store.Values)
		{
			if (value2 == value)
			{
				result = value2;
				return true;
			}
		}
		result = null;
		return false;
	}
}
