using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema;

internal class XsdString : XsdAnySimpleType
{
	internal override XmlSchemaFacet.Facet AllowedFacets => XsdAnySimpleType.stringAllowedFacets;

	public override XmlTokenizedType TokenizedType => XmlTokenizedType.CDATA;

	public override XmlTypeCode TypeCode => XmlTypeCode.String;

	public override Type ValueType => typeof(string);

	public override bool Bounded => false;

	public override bool Finite => false;

	public override bool Numeric => false;

	public override XsdOrderedFacet Ordered => XsdOrderedFacet.False;

	internal XsdString()
	{
	}
}
