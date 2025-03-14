using System.Collections.Generic;

namespace System.Xml;

internal class XmlSimpleDictionaryReader : XmlDictionaryReader, IXmlLineInfo, IXmlNamespaceResolver
{
	private XmlDictionary dict;

	private XmlReader reader;

	private XmlDictionaryReader as_dict_reader;

	private IXmlLineInfo as_line_info;

	private OnXmlDictionaryReaderClose onClose;

	public int LineNumber => (as_line_info != null) ? as_line_info.LineNumber : 0;

	public int LinePosition => (as_line_info != null) ? as_line_info.LinePosition : 0;

	public override bool CanCanonicalize => as_dict_reader != null && as_dict_reader.CanCanonicalize;

	public override int AttributeCount => reader.AttributeCount;

	public override string BaseURI => reader.BaseURI;

	public override int Depth => reader.Depth;

	public override XmlNodeType NodeType => reader.NodeType;

	public override string Name => reader.Name;

	public override string LocalName => reader.LocalName;

	public override string NamespaceURI => reader.NamespaceURI;

	public override string Prefix => reader.Prefix;

	public override bool HasValue => reader.HasValue;

	public override string Value => reader.Value;

	public override bool IsEmptyElement => reader.IsEmptyElement;

	public override bool IsDefault => reader.IsDefault;

	public override char QuoteChar => reader.QuoteChar;

	public override string XmlLang => reader.XmlLang;

	public override XmlSpace XmlSpace => reader.XmlSpace;

	public override string this[int i] => reader[i];

	public override string this[string name] => reader[name];

	public override string this[string localName, string namespaceURI] => reader[localName, namespaceURI];

	public override bool EOF => reader.EOF;

	public override ReadState ReadState => reader.ReadState;

	public override XmlNameTable NameTable => reader.NameTable;

	public XmlSimpleDictionaryReader(XmlReader reader)
		: this(reader, null)
	{
	}

	public XmlSimpleDictionaryReader(XmlReader reader, XmlDictionary dictionary)
		: this(reader, dictionary, null)
	{
	}

	public XmlSimpleDictionaryReader(XmlReader reader, XmlDictionary dictionary, OnXmlDictionaryReaderClose onClose)
	{
		this.reader = reader;
		this.onClose = onClose;
		as_line_info = reader as IXmlLineInfo;
		as_dict_reader = reader as XmlDictionaryReader;
		if (dictionary == null)
		{
			dictionary = new XmlDictionary();
		}
		dict = dictionary;
	}

	public bool HasLineInfo()
	{
		return as_line_info != null && as_line_info.HasLineInfo();
	}

	public override void EndCanonicalization()
	{
		if (as_dict_reader != null)
		{
			as_dict_reader.EndCanonicalization();
			return;
		}
		throw new NotSupportedException();
	}

	public override bool TryGetLocalNameAsDictionaryString(out XmlDictionaryString localName)
	{
		localName = null;
		return false;
	}

	public override bool TryGetNamespaceUriAsDictionaryString(out XmlDictionaryString namespaceUri)
	{
		namespaceUri = null;
		return false;
	}

	public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
	{
		IXmlNamespaceResolver xmlNamespaceResolver = reader as IXmlNamespaceResolver;
		return xmlNamespaceResolver.GetNamespacesInScope(scope);
	}

	public string LookupPrefix(string ns)
	{
		IXmlNamespaceResolver xmlNamespaceResolver = reader as IXmlNamespaceResolver;
		return xmlNamespaceResolver.LookupPrefix(NameTable.Get(ns));
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
		reader.Close();
		if (onClose != null)
		{
			onClose(this);
		}
	}

	public override bool Read()
	{
		if (!reader.Read())
		{
			return false;
		}
		dict.Add(reader.Prefix);
		dict.Add(reader.LocalName);
		dict.Add(reader.NamespaceURI);
		if (reader.MoveToFirstAttribute())
		{
			do
			{
				dict.Add(reader.Prefix);
				dict.Add(reader.LocalName);
				dict.Add(reader.NamespaceURI);
				dict.Add(reader.Value);
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
		}
		return true;
	}

	public override string ReadString()
	{
		return reader.ReadString();
	}

	public override string ReadInnerXml()
	{
		return reader.ReadInnerXml();
	}

	public override string ReadOuterXml()
	{
		return reader.ReadOuterXml();
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
