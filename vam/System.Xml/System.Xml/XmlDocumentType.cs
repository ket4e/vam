using System.IO;
using Mono.Xml;
using Mono.Xml2;

namespace System.Xml;

public class XmlDocumentType : XmlLinkedNode
{
	internal XmlNamedNodeMap entities;

	internal XmlNamedNodeMap notations;

	private DTDObjectModel dtd;

	internal DTDObjectModel DTD => dtd;

	public XmlNamedNodeMap Entities => entities;

	public string InternalSubset => dtd.InternalSubset;

	public override bool IsReadOnly => true;

	public override string LocalName => dtd.Name;

	public override string Name => dtd.Name;

	public override XmlNodeType NodeType => XmlNodeType.DocumentType;

	public XmlNamedNodeMap Notations => notations;

	public string PublicId => dtd.PublicId;

	public string SystemId => dtd.SystemId;

	protected internal XmlDocumentType(string name, string publicId, string systemId, string internalSubset, XmlDocument doc)
		: base(doc)
	{
		Mono.Xml2.XmlTextReader xmlTextReader = new Mono.Xml2.XmlTextReader(BaseURI, new StringReader(string.Empty), doc.NameTable);
		xmlTextReader.XmlResolver = doc.Resolver;
		xmlTextReader.GenerateDTDObjectModel(name, publicId, systemId, internalSubset);
		dtd = xmlTextReader.DTD;
		ImportFromDTD();
	}

	internal XmlDocumentType(DTDObjectModel dtd, XmlDocument doc)
		: base(doc)
	{
		this.dtd = dtd;
		ImportFromDTD();
	}

	private void ImportFromDTD()
	{
		entities = new XmlNamedNodeMap(this);
		notations = new XmlNamedNodeMap(this);
		foreach (DTDEntityDeclaration value in DTD.EntityDecls.Values)
		{
			XmlNode namedItem = new XmlEntity(value.Name, value.NotationName, value.PublicId, value.SystemId, OwnerDocument);
			entities.SetNamedItem(namedItem);
		}
		foreach (DTDNotationDeclaration value2 in DTD.NotationDecls.Values)
		{
			XmlNode namedItem2 = new XmlNotation(value2.LocalName, value2.Prefix, value2.PublicId, value2.SystemId, OwnerDocument);
			notations.SetNamedItem(namedItem2);
		}
	}

	public override XmlNode CloneNode(bool deep)
	{
		return new XmlDocumentType(dtd, OwnerDocument);
	}

	public override void WriteContentTo(XmlWriter w)
	{
	}

	public override void WriteTo(XmlWriter w)
	{
		w.WriteDocType(Name, PublicId, SystemId, InternalSubset);
	}
}
