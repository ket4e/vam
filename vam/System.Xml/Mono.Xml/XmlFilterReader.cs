using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml;

internal class XmlFilterReader : XmlReader, IXmlLineInfo
{
	private XmlReader reader;

	private XmlReaderSettings settings;

	private IXmlLineInfo lineInfo;

	public override bool CanReadBinaryContent => reader.CanReadBinaryContent;

	public override bool CanReadValueChunk => reader.CanReadValueChunk;

	public XmlReader Reader => reader;

	public int LineNumber => (lineInfo != null) ? lineInfo.LineNumber : 0;

	public int LinePosition => (lineInfo != null) ? lineInfo.LinePosition : 0;

	public override XmlNodeType NodeType => reader.NodeType;

	public override string Name => reader.Name;

	public override string LocalName => reader.LocalName;

	public override string NamespaceURI => reader.NamespaceURI;

	public override string Prefix => reader.Prefix;

	public override bool HasValue => reader.HasValue;

	public override int Depth => reader.Depth;

	public override string Value => reader.Value;

	public override string BaseURI => reader.BaseURI;

	public override bool IsEmptyElement => reader.IsEmptyElement;

	public override bool IsDefault => reader.IsDefault;

	public override char QuoteChar => reader.QuoteChar;

	public override string XmlLang => reader.XmlLang;

	public override XmlSpace XmlSpace => reader.XmlSpace;

	public override int AttributeCount => reader.AttributeCount;

	public override string this[int i] => reader[i];

	public override string this[string name] => reader[name];

	public override string this[string localName, string namespaceURI] => reader[localName, namespaceURI];

	public override bool EOF => reader.EOF;

	public override ReadState ReadState => reader.ReadState;

	public override XmlNameTable NameTable => reader.NameTable;

	public override IXmlSchemaInfo SchemaInfo => reader.SchemaInfo;

	public override XmlReaderSettings Settings => settings;

	public XmlFilterReader(XmlReader reader, XmlReaderSettings settings)
	{
		this.reader = reader;
		this.settings = settings.Clone();
		lineInfo = reader as IXmlLineInfo;
	}

	public override string GetAttribute(string name)
	{
		return reader.GetAttribute(name);
	}

	public override string GetAttribute(string localName, string namespaceURI)
	{
		return reader.GetAttribute(localName, namespaceURI);
	}

	public override string GetAttribute(int i)
	{
		return reader.GetAttribute(i);
	}

	public bool HasLineInfo()
	{
		return lineInfo != null && lineInfo.HasLineInfo();
	}

	public override bool MoveToAttribute(string name)
	{
		return reader.MoveToAttribute(name);
	}

	public override bool MoveToAttribute(string localName, string namespaceURI)
	{
		return reader.MoveToAttribute(localName, namespaceURI);
	}

	public override void MoveToAttribute(int i)
	{
		reader.MoveToAttribute(i);
	}

	public override bool MoveToFirstAttribute()
	{
		return reader.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return reader.MoveToNextAttribute();
	}

	public override bool MoveToElement()
	{
		return reader.MoveToElement();
	}

	public override void Close()
	{
		if (settings.CloseInput)
		{
			reader.Close();
		}
	}

	public override bool Read()
	{
		if (!reader.Read())
		{
			return false;
		}
		if (reader.NodeType == XmlNodeType.DocumentType && settings.ProhibitDtd)
		{
			throw new XmlException("Document Type Definition (DTD) is prohibited in this XML reader.");
		}
		if (reader.NodeType == XmlNodeType.Whitespace && settings.IgnoreWhitespace)
		{
			return Read();
		}
		if (reader.NodeType == XmlNodeType.ProcessingInstruction && settings.IgnoreProcessingInstructions)
		{
			return Read();
		}
		if (reader.NodeType == XmlNodeType.Comment && settings.IgnoreComments)
		{
			return Read();
		}
		return true;
	}

	public override string ReadString()
	{
		return reader.ReadString();
	}

	public override string LookupNamespace(string prefix)
	{
		return reader.LookupNamespace(prefix);
	}

	public override void ResolveEntity()
	{
		reader.ResolveEntity();
	}

	public override bool ReadAttributeValue()
	{
		return reader.ReadAttributeValue();
	}
}
