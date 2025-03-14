using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdID : XsdName
{
	public override XmlTokenizedType TokenizedType => XmlTokenizedType.ID;

	public override XmlTypeCode TypeCode => XmlTypeCode.Id;

	public override Type ValueType => typeof(string);

	internal XsdID()
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
