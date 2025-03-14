using System.Collections.Generic;

namespace System.Xml;

public class XmlDictionary : IXmlDictionary
{
	internal class EmptyDictionary : XmlDictionary
	{
		public static readonly EmptyDictionary Instance = new EmptyDictionary();

		public EmptyDictionary()
			: base(1)
		{
		}
	}

	private static XmlDictionary empty = new XmlDictionary(isReadOnly: true);

	private readonly bool is_readonly;

	private Dictionary<string, XmlDictionaryString> dict;

	private List<XmlDictionaryString> list;

	public static IXmlDictionary Empty => empty;

	public XmlDictionary()
	{
		dict = new Dictionary<string, XmlDictionaryString>();
		list = new List<XmlDictionaryString>();
	}

	public XmlDictionary(int capacity)
	{
		dict = new Dictionary<string, XmlDictionaryString>(capacity);
		list = new List<XmlDictionaryString>(capacity);
	}

	private XmlDictionary(bool isReadOnly)
		: this(1)
	{
		is_readonly = isReadOnly;
	}

	public virtual XmlDictionaryString Add(string value)
	{
		if (is_readonly)
		{
			throw new InvalidOperationException();
		}
		if (dict.TryGetValue(value, out var value2))
		{
			return value2;
		}
		value2 = new XmlDictionaryString(this, value, dict.Count);
		dict.Add(value, value2);
		list.Add(value2);
		return value2;
	}

	public virtual bool TryLookup(int key, out XmlDictionaryString result)
	{
		if (key < 0 || dict.Count <= key)
		{
			result = null;
			return false;
		}
		result = list[key];
		return true;
	}

	public virtual bool TryLookup(string value, out XmlDictionaryString result)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		return dict.TryGetValue(value, out result);
	}

	public virtual bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
	{
		if (value == null)
		{
			throw new ArgumentNullException();
		}
		if (value.Dictionary != this)
		{
			result = null;
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (object.ReferenceEquals(list[i], value))
			{
				result = value;
				return true;
			}
		}
		result = null;
		return false;
	}
}
