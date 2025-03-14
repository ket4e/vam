namespace System.Xml.Schema;

internal class XmlSchemaReader : XmlReader, IXmlLineInfo
{
	private XmlReader reader;

	private ValidationEventHandler handler;

	private bool hasLineInfo;

	public string FullName => NamespaceURI + ":" + LocalName;

	public XmlReader Reader => reader;

	public int LineNumber => hasLineInfo ? ((IXmlLineInfo)reader).LineNumber : 0;

	public int LinePosition => hasLineInfo ? ((IXmlLineInfo)reader).LinePosition : 0;

	public override int AttributeCount => reader.AttributeCount;

	public override string BaseURI => reader.BaseURI;

	public override bool CanResolveEntity => reader.CanResolveEntity;

	public override int Depth => reader.Depth;

	public override bool EOF => reader.EOF;

	public override bool HasAttributes => reader.HasAttributes;

	public override bool HasValue => reader.HasValue;

	public override bool IsDefault => reader.IsDefault;

	public override bool IsEmptyElement => reader.IsEmptyElement;

	public override string this[int i] => reader[i];

	public override string this[string name] => reader[name];

	public override string this[string name, string namespaceURI] => reader[name, namespaceURI];

	public override string LocalName => reader.LocalName;

	public override string Name => reader.Name;

	public override string NamespaceURI => reader.NamespaceURI;

	public override XmlNameTable NameTable => reader.NameTable;

	public override XmlNodeType NodeType => reader.NodeType;

	public override string Prefix => reader.Prefix;

	public override char QuoteChar => reader.QuoteChar;

	public override ReadState ReadState => reader.ReadState;

	public override string Value => reader.Value;

	public override string XmlLang => reader.XmlLang;

	public override XmlSpace XmlSpace => reader.XmlSpace;

	public XmlSchemaReader(XmlReader reader, ValidationEventHandler handler)
	{
		this.reader = reader;
		this.handler = handler;
		if (reader is IXmlLineInfo)
		{
			IXmlLineInfo xmlLineInfo = (IXmlLineInfo)reader;
			hasLineInfo = xmlLineInfo.HasLineInfo();
		}
	}

	public void RaiseInvalidElementError()
	{
		string text = "Element " + FullName + " is invalid in this context.\n";
		if (hasLineInfo)
		{
			string text2 = text;
			text = text2 + "The error occured on (" + ((IXmlLineInfo)reader).LineNumber + "," + ((IXmlLineInfo)reader).LinePosition + ")";
		}
		XmlSchemaObject.error(handler, text, null);
		SkipToEnd();
	}

	public bool ReadNextElement()
	{
		MoveToElement();
		while (Read())
		{
			if (NodeType == XmlNodeType.Element || NodeType == XmlNodeType.EndElement)
			{
				if (!(reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema"))
				{
					return true;
				}
				RaiseInvalidElementError();
			}
		}
		return false;
	}

	public void SkipToEnd()
	{
		MoveToElement();
		if (!IsEmptyElement && NodeType == XmlNodeType.Element && NodeType == XmlNodeType.Element)
		{
			int depth = Depth;
			while (Read() && Depth != depth)
			{
			}
		}
	}

	public bool HasLineInfo()
	{
		return hasLineInfo;
	}

	public override void Close()
	{
		reader.Close();
	}

	public override bool Equals(object obj)
	{
		return reader.Equals(obj);
	}

	public override string GetAttribute(int i)
	{
		return reader.GetAttribute(i);
	}

	public override string GetAttribute(string name)
	{
		return reader.GetAttribute(name);
	}

	public override string GetAttribute(string name, string namespaceURI)
	{
		return reader.GetAttribute(name, namespaceURI);
	}

	public override int GetHashCode()
	{
		return reader.GetHashCode();
	}

	public override bool IsStartElement()
	{
		return reader.IsStartElement();
	}

	public override bool IsStartElement(string localname, string ns)
	{
		return reader.IsStartElement(localname, ns);
	}

	public override bool IsStartElement(string name)
	{
		return reader.IsStartElement(name);
	}

	public override string LookupNamespace(string prefix)
	{
		return reader.LookupNamespace(prefix);
	}

	public override void MoveToAttribute(int i)
	{
		reader.MoveToAttribute(i);
	}

	public override bool MoveToAttribute(string name)
	{
		return reader.MoveToAttribute(name);
	}

	public override bool MoveToAttribute(string name, string ns)
	{
		return reader.MoveToAttribute(name, ns);
	}

	public override XmlNodeType MoveToContent()
	{
		return reader.MoveToContent();
	}

	public override bool MoveToElement()
	{
		return reader.MoveToElement();
	}

	public override bool MoveToFirstAttribute()
	{
		return reader.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return reader.MoveToNextAttribute();
	}

	public override bool Read()
	{
		return reader.Read();
	}

	public override bool ReadAttributeValue()
	{
		return reader.ReadAttributeValue();
	}

	public override string ReadElementString()
	{
		return reader.ReadElementString();
	}

	public override string ReadElementString(string localname, string ns)
	{
		return reader.ReadElementString(localname, ns);
	}

	public override string ReadElementString(string name)
	{
		return reader.ReadElementString(name);
	}

	public override void ReadEndElement()
	{
		reader.ReadEndElement();
	}

	public override string ReadInnerXml()
	{
		return reader.ReadInnerXml();
	}

	public override string ReadOuterXml()
	{
		return reader.ReadOuterXml();
	}

	public override void ReadStartElement()
	{
		reader.ReadStartElement();
	}

	public override void ReadStartElement(string localname, string ns)
	{
		reader.ReadStartElement(localname, ns);
	}

	public override void ReadStartElement(string name)
	{
		reader.ReadStartElement(name);
	}

	public override string ReadString()
	{
		return reader.ReadString();
	}

	public override void ResolveEntity()
	{
		reader.ResolveEntity();
	}

	public override void Skip()
	{
		reader.Skip();
	}

	public override string ToString()
	{
		return reader.ToString();
	}
}
