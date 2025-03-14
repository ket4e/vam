using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml;

internal class DTDContentModel : DTDNode
{
	private DTDObjectModel root;

	private DTDAutomata compiledAutomata;

	private string ownerElementName;

	private string elementName;

	private DTDContentOrderType orderType;

	private DTDContentModelCollection childModels = new DTDContentModelCollection();

	private DTDOccurence occurence;

	public DTDContentModelCollection ChildModels
	{
		get
		{
			return childModels;
		}
		set
		{
			childModels = value;
		}
	}

	public DTDElementDeclaration ElementDecl => root.ElementDecls[ownerElementName];

	public string ElementName
	{
		get
		{
			return elementName;
		}
		set
		{
			elementName = value;
		}
	}

	public DTDOccurence Occurence
	{
		get
		{
			return occurence;
		}
		set
		{
			occurence = value;
		}
	}

	public DTDContentOrderType OrderType
	{
		get
		{
			return orderType;
		}
		set
		{
			orderType = value;
		}
	}

	internal DTDContentModel(DTDObjectModel root, string ownerElementName)
	{
		this.root = root;
		this.ownerElementName = ownerElementName;
	}

	public DTDAutomata GetAutomata()
	{
		if (compiledAutomata == null)
		{
			Compile();
		}
		return compiledAutomata;
	}

	public DTDAutomata Compile()
	{
		compiledAutomata = CompileInternal();
		return compiledAutomata;
	}

	internal XmlSchemaParticle CreateXsdParticle()
	{
		XmlSchemaParticle xmlSchemaParticle = CreateXsdParticleCore();
		if (xmlSchemaParticle == null)
		{
			return null;
		}
		switch (Occurence)
		{
		case DTDOccurence.Optional:
			xmlSchemaParticle.MinOccurs = 0m;
			break;
		case DTDOccurence.OneOrMore:
			xmlSchemaParticle.MaxOccursString = "unbounded";
			break;
		case DTDOccurence.ZeroOrMore:
			xmlSchemaParticle.MinOccurs = 0m;
			xmlSchemaParticle.MaxOccursString = "unbounded";
			break;
		}
		return xmlSchemaParticle;
	}

	private XmlSchemaParticle CreateXsdParticleCore()
	{
		XmlSchemaParticle xmlSchemaParticle = null;
		if (ElementName != null)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			SetLineInfo(xmlSchemaElement);
			xmlSchemaElement.RefName = new XmlQualifiedName(ElementName);
			return xmlSchemaElement;
		}
		if (ChildModels.Count == 0)
		{
			return null;
		}
		XmlSchemaGroupBase xmlSchemaGroupBase = ((OrderType != DTDContentOrderType.Seq) ? ((XmlSchemaGroupBase)new XmlSchemaChoice()) : ((XmlSchemaGroupBase)new XmlSchemaSequence()));
		SetLineInfo(xmlSchemaGroupBase);
		foreach (DTDContentModel item in ChildModels.Items)
		{
			XmlSchemaParticle xmlSchemaParticle2 = item.CreateXsdParticle();
			if (xmlSchemaParticle2 != null)
			{
				xmlSchemaGroupBase.Items.Add(xmlSchemaParticle2);
			}
		}
		return xmlSchemaGroupBase;
	}

	private DTDAutomata CompileInternal()
	{
		if (ElementDecl.IsAny)
		{
			return root.Any;
		}
		if (ElementDecl.IsEmpty)
		{
			return root.Empty;
		}
		DTDAutomata basicContentAutomata = GetBasicContentAutomata();
		return Occurence switch
		{
			DTDOccurence.One => basicContentAutomata, 
			DTDOccurence.Optional => Choice(root.Empty, basicContentAutomata), 
			DTDOccurence.OneOrMore => new DTDOneOrMoreAutomata(root, basicContentAutomata), 
			DTDOccurence.ZeroOrMore => Choice(root.Empty, new DTDOneOrMoreAutomata(root, basicContentAutomata)), 
			_ => throw new InvalidOperationException(), 
		};
	}

	private DTDAutomata GetBasicContentAutomata()
	{
		if (ElementName != null)
		{
			return new DTDElementAutomata(root, ElementName);
		}
		switch (ChildModels.Count)
		{
		case 0:
			return root.Empty;
		case 1:
			return ChildModels[0].GetAutomata();
		default:
		{
			DTDAutomata dTDAutomata = null;
			int count = ChildModels.Count;
			switch (OrderType)
			{
			case DTDContentOrderType.Seq:
			{
				dTDAutomata = Sequence(ChildModels[count - 2].GetAutomata(), ChildModels[count - 1].GetAutomata());
				for (int num2 = count - 2; num2 > 0; num2--)
				{
					dTDAutomata = Sequence(ChildModels[num2 - 1].GetAutomata(), dTDAutomata);
				}
				return dTDAutomata;
			}
			case DTDContentOrderType.Or:
			{
				dTDAutomata = Choice(ChildModels[count - 2].GetAutomata(), ChildModels[count - 1].GetAutomata());
				for (int num = count - 2; num > 0; num--)
				{
					dTDAutomata = Choice(ChildModels[num - 1].GetAutomata(), dTDAutomata);
				}
				return dTDAutomata;
			}
			default:
				throw new InvalidOperationException("Invalid pattern specification");
			}
		}
		}
	}

	private DTDAutomata Sequence(DTDAutomata l, DTDAutomata r)
	{
		return root.Factory.Sequence(l, r);
	}

	private DTDAutomata Choice(DTDAutomata l, DTDAutomata r)
	{
		return l.MakeChoice(r);
	}
}
