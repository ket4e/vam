using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdName : XsdToken
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.Name;

	public override Type ValueType => typeof(string);

	internal XsdName()
	{
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		if (!XmlChar.IsName(s))
		{
			throw new ArgumentException("'" + s + "' is an invalid name.");
		}
		return s;
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new StringValueType(ParseValue(s, nameTable, nsmgr) as string);
	}
}
