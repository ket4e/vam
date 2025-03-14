using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdNormalizedString : XsdString
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.NormalizedString;

	public override Type ValueType => typeof(string);

	internal XsdNormalizedString()
	{
		WhitespaceValue = XsdWhitespaceFacet.Replace;
	}
}
