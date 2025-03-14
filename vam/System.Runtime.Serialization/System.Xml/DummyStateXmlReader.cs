namespace System.Xml;

internal abstract class DummyStateXmlReader : XmlReader
{
	private string base_uri;

	private XmlNameTable name_table;

	private ReadState read_state;

	public override string BaseURI => base_uri;

	public override bool EOF => false;

	public override int AttributeCount => 0;

	public override bool IsEmptyElement => false;

	public override string LocalName => string.Empty;

	public override string NamespaceURI => string.Empty;

	public override XmlNameTable NameTable => name_table;

	public override string Prefix => string.Empty;

	public override ReadState ReadState => read_state;

	protected DummyStateXmlReader(string baseUri, XmlNameTable nameTable, ReadState readState)
	{
		base_uri = baseUri;
		name_table = nameTable;
		read_state = readState;
	}

	public override void Close()
	{
		throw new NotSupportedException();
	}

	public override bool Read()
	{
		throw new NotSupportedException();
	}

	public override bool MoveToElement()
	{
		return false;
	}

	public override string GetAttribute(int index)
	{
		return null;
	}

	public override string GetAttribute(string name)
	{
		return null;
	}

	public override string GetAttribute(string localName, string namespaceURI)
	{
		return null;
	}

	public override void MoveToAttribute(int index)
	{
		throw new ArgumentOutOfRangeException();
	}

	public override bool MoveToAttribute(string name)
	{
		return false;
	}

	public override bool MoveToAttribute(string localName, string namespaceURI)
	{
		return false;
	}

	public override bool MoveToFirstAttribute()
	{
		return false;
	}

	public override bool MoveToNextAttribute()
	{
		return false;
	}

	public override string LookupNamespace(string prefix)
	{
		return null;
	}

	public override bool ReadAttributeValue()
	{
		return false;
	}

	public override void ResolveEntity()
	{
		throw new InvalidOperationException();
	}
}
