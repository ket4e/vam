using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdToken : XsdNormalizedString
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.Token;

	public override Type ValueType => typeof(string);

	internal XsdToken()
	{
		WhitespaceValue = XsdWhitespaceFacet.Collapse;
	}
}
