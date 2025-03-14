using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdNMToken : XsdToken
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.NMTOKEN;

	public override XmlTypeCode TypeCode => XmlTypeCode.NmToken;

	public override Type ValueType => typeof(string);

	internal XsdNMToken()
	{
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		if (!XmlChar.IsNmToken(s))
		{
			throw new ArgumentException("'" + s + "' is an invalid NMTOKEN.");
		}
		return s;
	}

	internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return new StringValueType(ParseValue(s, nameTable, nsmgr) as string);
	}
}
