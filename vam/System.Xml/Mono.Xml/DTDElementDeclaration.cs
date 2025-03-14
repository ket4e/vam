using System.Xml.Schema;

namespace Mono.Xml;

internal class DTDElementDeclaration : DTDNode
{
	private DTDObjectModel root;

	private DTDContentModel contentModel;

	private string name;

	private bool isEmpty;

	private bool isAny;

	private bool isMixedContent;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return isEmpty;
		}
		set
		{
			isEmpty = value;
		}
	}

	public bool IsAny
	{
		get
		{
			return isAny;
		}
		set
		{
			isAny = value;
		}
	}

	public bool IsMixedContent
	{
		get
		{
			return isMixedContent;
		}
		set
		{
			isMixedContent = value;
		}
	}

	public DTDContentModel ContentModel
	{
		get
		{
			if (contentModel == null)
			{
				contentModel = new DTDContentModel(root, Name);
			}
			return contentModel;
		}
	}

	public DTDAttListDeclaration Attributes => base.Root.AttListDecls[Name];

	internal DTDElementDeclaration(DTDObjectModel root)
	{
		this.root = root;
	}

	internal XmlSchemaElement CreateXsdElement()
	{
		XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
		SetLineInfo(xmlSchemaElement);
		xmlSchemaElement.Name = Name;
		XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)(xmlSchemaElement.SchemaType = new XmlSchemaComplexType());
		if (Attributes != null)
		{
			SetLineInfo(xmlSchemaComplexType);
			foreach (DTDAttributeDefinition definition in Attributes.Definitions)
			{
				xmlSchemaComplexType.Attributes.Add(definition.CreateXsdAttribute());
			}
		}
		if (!IsEmpty)
		{
			if (IsAny)
			{
				XmlSchemaAny xmlSchemaAny = new XmlSchemaAny();
				xmlSchemaAny.MinOccurs = 0m;
				xmlSchemaAny.MaxOccursString = "unbounded";
				xmlSchemaComplexType.Particle = xmlSchemaAny;
			}
			else
			{
				if (IsMixedContent)
				{
					xmlSchemaComplexType.IsMixed = true;
				}
				xmlSchemaComplexType.Particle = ContentModel.CreateXsdParticle();
			}
		}
		return xmlSchemaElement;
	}
}
