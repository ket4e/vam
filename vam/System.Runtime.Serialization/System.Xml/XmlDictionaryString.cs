namespace System.Xml;

public class XmlDictionaryString
{
	private static XmlDictionaryString empty = new XmlDictionaryString(XmlDictionary.EmptyDictionary.Instance, string.Empty, 0);

	private readonly IXmlDictionary dict;

	private readonly string value;

	private readonly int key;

	public static XmlDictionaryString Empty => empty;

	public IXmlDictionary Dictionary => dict;

	public int Key => key;

	public string Value => value;

	public XmlDictionaryString(IXmlDictionary dictionary, string value, int key)
	{
		if (dictionary == null)
		{
			throw new ArgumentNullException("dictionary");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (key < 0 || key > 536870911)
		{
			throw new ArgumentOutOfRangeException("key");
		}
		dict = dictionary;
		this.value = value;
		this.key = key;
	}

	public override string ToString()
	{
		return value;
	}
}
