namespace System.Xml;

internal class MultiPartedXmlReader : DummyStateXmlReader
{
	private XmlReader owner;

	private string value;

	public override int Depth => owner.Depth;

	public override bool HasValue => true;

	public override string Value => value;

	public override XmlNodeType NodeType => XmlNodeType.Text;

	public MultiPartedXmlReader(XmlReader reader, MimeEncodedStream value)
		: base(reader.BaseURI, reader.NameTable, reader.ReadState)
	{
		owner = reader;
		this.value = value.CreateTextReader().ReadToEnd();
	}
}
