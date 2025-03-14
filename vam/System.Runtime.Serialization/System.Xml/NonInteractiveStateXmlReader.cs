namespace System.Xml;

internal class NonInteractiveStateXmlReader : DummyStateXmlReader
{
	public override int Depth => 0;

	public override bool HasValue => false;

	public override string Value => string.Empty;

	public override XmlNodeType NodeType => XmlNodeType.None;

	public NonInteractiveStateXmlReader(string baseUri, XmlNameTable nameTable, ReadState readState)
		: base(baseUri, nameTable, readState)
	{
	}
}
