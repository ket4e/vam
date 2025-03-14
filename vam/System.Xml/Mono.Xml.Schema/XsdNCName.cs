using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdNCName : XsdName
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.NCName;

	public override XmlTypeCode TypeCode => XmlTypeCode.NCName;

	public override Type ValueType => typeof(string);

	internal XsdNCName()
	{
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		if (!XmlChar.IsNCName(s))
		{
			throw new ArgumentException("'" + s + "' is an invalid NCName.");
		}
		return s;
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new StringValueType(ParseValue(s, nameTable, nsmgr) as string);
	}
}
