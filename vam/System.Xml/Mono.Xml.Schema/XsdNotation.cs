using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdNotation : XsdAnySimpleType
{
	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.stringAllowedFacets;

	public override XmlTokenizedType TokenizedType => XmlTokenizedType.NOTATION;

	public override XmlTypeCode TypeCode => XmlTypeCode.Notation;

	public override Type ValueType => typeof(string);

	public override bool Bounded => false;

	public override bool Finite => false;

	public override bool Numeric => false;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.False;

	internal XsdNotation()
	{
	}

	public override object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
	{
		return Normalize(s);
	}
}
