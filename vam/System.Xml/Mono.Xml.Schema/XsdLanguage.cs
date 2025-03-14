using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdLanguage : XsdToken
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.Language;

	public override Type ValueType => typeof(string);

	internal XsdLanguage()
	{
	}
}
