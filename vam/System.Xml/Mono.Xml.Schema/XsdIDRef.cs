using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdIDRef : XsdName
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.IDREF;

	public override XmlTypeCode TypeCode => XmlTypeCode.Idref;

	public override Type ValueType => typeof(string);

	internal XsdIDRef()
	{
	}

	public override object ParseValue(string s, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
	{
		if (!XmlChar.IsNCName(s))
		{
			throw new ArgumentException("'" + s + "' is an invalid NCName.");
		}
		return s;
	}
}
