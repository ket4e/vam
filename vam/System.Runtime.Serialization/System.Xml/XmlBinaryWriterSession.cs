using System.Collections.Generic;

namespace System.Xml;

public class XmlBinaryWriterSession
{
	private Dictionary<int, XmlDictionaryString> dic = new Dictionary<int, XmlDictionaryString>();

	public void Reset()
	{
		dic.Clear();
	}

	public virtual bool TryAdd(XmlDictionaryString value, out int key)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (TryLookup(value, out key))
		{
			throw new InvalidOperationException("Argument XmlDictionaryString was already added to the writer session");
		}
		key = dic.Count;
		dic.Add(key, value);
		return true;
	}

	internal bool TryLookup(XmlDictionaryString value, out int key)
	{
		foreach (KeyValuePair<int, XmlDictionaryString> item in dic)
		{
			if (item.Value.Value == value.Value)
			{
				key = item.Key;
				return true;
			}
		}
		key = -1;
		return false;
	}
}
