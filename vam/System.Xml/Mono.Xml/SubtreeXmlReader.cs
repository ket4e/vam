using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml;

internal class SubtreeXmlReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
{
	private int startDepth;

	private bool eof;

	private bool initial;

	private bool read;

	private XmlReader Reader;

	private IXmlLineInfo li;

	private IXmlNamespaceResolver nsResolver;

	public override int AttributeCount => (!initial) ? Reader.AttributeCount : 0;

	public override bool CanReadBinaryContent => Reader.CanReadBinaryContent;

	public override bool CanReadValueChunk => Reader.CanReadValueChunk;

	public override int Depth => Reader.Depth - startDepth;

	public override string BaseURI => Reader.BaseURI;

	public override bool EOF => eof || Reader.EOF;

	public override bool IsEmptyElement => Reader.IsEmptyElement;

	public int LineNumber => (!initial) ? ((li != null) ? li.LineNumber : 0) : 0;

	public int LinePosition => (!initial) ? ((li != null) ? li.LinePosition : 0) : 0;

	public override bool HasValue => !initial && !eof && Reader.HasValue;

	public override string LocalName => (!initial && !eof) ? Reader.LocalName : string.Empty;

	public override string Name => (!initial && !eof) ? Reader.Name : string.Empty;

	public override XmlNameTable NameTable => Reader.NameTable;

	public override string NamespaceURI => (!initial && !eof) ? Reader.NamespaceURI : string.Empty;

	public override XmlNodeType NodeType => (!initial && !eof) ? Reader.NodeType : XmlNodeType.None;

	public override string Prefix => (!initial && !eof) ? Reader.Prefix : string.Empty;

	public override ReadState ReadState => (!initial) ? ((!eof) ? Reader.ReadState : ReadState.EndOfFile) : ReadState.Initial;

	public override IXmlSchemaInfo SchemaInfo => Reader.SchemaInfo;

	public override XmlReaderSettings Settings => Reader.Settings;

	public override string Value => (!initial) ? Reader.Value : string.Empty;

	public SubtreeXmlReader(XmlReader reader)
	{
		Reader = reader;
		li = reader as IXmlLineInfo;
		nsResolver = reader as IXmlNamespaceResolver;
		initial = true;
		startDepth = reader.Depth;
		if (reader.ReadState == ReadState.Initial)
		{
			startDepth = -1;
		}
	}

	IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
	{
		object result;
		if (nsResolver != null)
		{
			IDictionary<string, string> namespacesInScope = nsResolver.GetNamespacesInScope(scope);
			result = namespacesInScope;
		}
		else
		{
			result = new Dictionary<string, string>();
		}
		return (IDictionary<string, string>)result;
	}

	string IXmlNamespaceResolver.LookupPrefix(string ns)
	{
		return (nsResolver == null) ? string.Empty : nsResolver.LookupPrefix(ns);
	}

	public override void Close()
	{
		while (Read())
		{
		}
	}

	public override string GetAttribute(int i)
	{
		return (!initial) ? Reader.GetAttribute(i) : null;
	}

	public override string GetAttribute(string name)
	{
		return (!initial) ? Reader.GetAttribute(name) : null;
	}

	public override string GetAttribute(string local, string ns)
	{
		return (!initial) ? Reader.GetAttribute(local, ns) : null;
	}

	public bool HasLineInfo()
	{
		return li != null && li.HasLineInfo();
	}

	public override string LookupNamespace(string prefix)
	{
		return Reader.LookupNamespace(prefix);
	}

	public override bool MoveToFirstAttribute()
	{
		return !initial && Reader.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return !initial && Reader.MoveToNextAttribute();
	}

	public override void MoveToAttribute(int i)
	{
		if (!initial)
		{
			Reader.MoveToAttribute(i);
		}
	}

	public override bool MoveToAttribute(string name)
	{
		return !initial && Reader.MoveToAttribute(name);
	}

	public override bool MoveToAttribute(string local, string ns)
	{
		return !initial && Reader.MoveToAttribute(local, ns);
	}

	public override bool MoveToElement()
	{
		return !initial && Reader.MoveToElement();
	}

	public override bool Read()
	{
		if (initial)
		{
			initial = false;
			return true;
		}
		if (!read)
		{
			read = true;
			Reader.MoveToElement();
			bool flag = !Reader.IsEmptyElement && Reader.Read();
			if (!flag)
			{
				eof = true;
			}
			return flag;
		}
		Reader.MoveToElement();
		if (Reader.Depth > startDepth && Reader.Read())
		{
			return true;
		}
		eof = true;
		return false;
	}

	public override bool ReadAttributeValue()
	{
		if (initial || eof)
		{
			return false;
		}
		return Reader.ReadAttributeValue();
	}

	public override void ResolveEntity()
	{
		if (!initial && !eof)
		{
			Reader.ResolveEntity();
		}
	}
}
