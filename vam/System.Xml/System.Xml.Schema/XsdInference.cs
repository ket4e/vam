using System.Collections;

namespace System.Xml.Schema;

internal class XsdInference
{
	public const string NamespaceXml = "http://www.w3.org/XML/1998/namespace";

	public const string NamespaceXmlns = "http://www.w3.org/2000/xmlns/";

	public const string XdtNamespace = "http://www.w3.org/2003/11/xpath-datatypes";

	private static readonly XmlQualifiedName QNameString = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameBoolean = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameAnyType = new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameByte = new XmlQualifiedName("byte", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameUByte = new XmlQualifiedName("unsignedByte", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameShort = new XmlQualifiedName("short", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameUShort = new XmlQualifiedName("unsignedShort", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameInt = new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameUInt = new XmlQualifiedName("unsignedInt", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameLong = new XmlQualifiedName("long", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameULong = new XmlQualifiedName("unsignedLong", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameDecimal = new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameUDecimal = new XmlQualifiedName("unsignedDecimal", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameDouble = new XmlQualifiedName("double", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameFloat = new XmlQualifiedName("float", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameDateTime = new XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");

	private static readonly XmlQualifiedName QNameDuration = new XmlQualifiedName("duration", "http://www.w3.org/2001/XMLSchema");

	private XmlReader source;

	private XmlSchemaSet schemas;

	private bool laxOccurrence;

	private bool laxTypeInference;

	private Hashtable newElements = new Hashtable();

	private Hashtable newAttributes = new Hashtable();

	private XsdInference(XmlReader xmlReader, XmlSchemaSet schemas, bool laxOccurrence, bool laxTypeInference)
	{
		source = xmlReader;
		this.schemas = schemas;
		this.laxOccurrence = laxOccurrence;
		this.laxTypeInference = laxTypeInference;
	}

	public static XmlSchemaSet Process(XmlReader xmlReader, XmlSchemaSet schemas, bool laxOccurrence, bool laxTypeInference)
	{
		XsdInference xsdInference = new XsdInference(xmlReader, schemas, laxOccurrence, laxTypeInference);
		xsdInference.Run();
		return xsdInference.schemas;
	}

	private void Run()
	{
		schemas.Compile();
		source.MoveToContent();
		if (source.NodeType != XmlNodeType.Element)
		{
			throw new ArgumentException("Argument XmlReader content is expected to be an element.");
		}
		XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(source.LocalName, source.NamespaceURI);
		XmlSchemaElement globalElement = GetGlobalElement(xmlQualifiedName);
		if (globalElement == null)
		{
			globalElement = CreateGlobalElement(xmlQualifiedName);
			InferElement(globalElement, xmlQualifiedName.Namespace, isNew: true);
		}
		else
		{
			InferElement(globalElement, xmlQualifiedName.Namespace, isNew: false);
		}
	}

	private void AddImport(string current, string import)
	{
		foreach (XmlSchema item in schemas.Schemas(current))
		{
			bool flag = false;
			foreach (XmlSchemaExternal include in item.Includes)
			{
				if (include is XmlSchemaImport xmlSchemaImport && xmlSchemaImport.Namespace == import)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				XmlSchemaImport xmlSchemaImport2 = new XmlSchemaImport();
				xmlSchemaImport2.Namespace = import;
				item.Includes.Add(xmlSchemaImport2);
			}
		}
	}

	private void IncludeXmlAttributes()
	{
		if (schemas.Schemas("http://www.w3.org/XML/1998/namespace").Count == 0)
		{
			schemas.Add("http://www.w3.org/XML/1998/namespace", "http://www.w3.org/2001/xml.xsd");
		}
	}

	private void InferElement(XmlSchemaElement el, string ns, bool isNew)
	{
		if (el.RefName != XmlQualifiedName.Empty)
		{
			XmlSchemaElement globalElement = GetGlobalElement(el.RefName);
			if (globalElement == null)
			{
				globalElement = CreateElement(el.RefName);
				InferElement(globalElement, ns, isNew: true);
			}
			else
			{
				InferElement(globalElement, ns, isNew);
			}
			return;
		}
		if (source.MoveToFirstAttribute())
		{
			InferAttributes(el, ns, isNew);
			source.MoveToElement();
		}
		if (source.IsEmptyElement)
		{
			InferAsEmptyElement(el, ns, isNew);
			source.Read();
			source.MoveToContent();
		}
		else
		{
			InferContent(el, ns, isNew);
			source.ReadEndElement();
		}
		if (el.SchemaType == null && el.SchemaTypeName == XmlQualifiedName.Empty)
		{
			el.SchemaTypeName = QNameString;
		}
	}

	private Hashtable CollectAttrTable(XmlSchemaObjectCollection attList)
	{
		Hashtable hashtable = new Hashtable();
		foreach (XmlSchemaObject att in attList)
		{
			if (!(att is XmlSchemaAttribute xmlSchemaAttribute))
			{
				throw Error(att, $"Attribute inference only supports direct attribute definition. {att.GetType()} is not supported.");
			}
			if (xmlSchemaAttribute.RefName != XmlQualifiedName.Empty)
			{
				hashtable.Add(xmlSchemaAttribute.RefName, xmlSchemaAttribute);
			}
			else
			{
				hashtable.Add(new XmlQualifiedName(xmlSchemaAttribute.Name, string.Empty), xmlSchemaAttribute);
			}
		}
		return hashtable;
	}

	private void InferAttributes(XmlSchemaElement el, string ns, bool isNew)
	{
		XmlSchemaComplexType xmlSchemaComplexType = null;
		XmlSchemaObjectCollection xmlSchemaObjectCollection = null;
		Hashtable hashtable = null;
		do
		{
			switch (source.NamespaceURI)
			{
			case "http://www.w3.org/XML/1998/namespace":
				if (schemas.Schemas("http://www.w3.org/XML/1998/namespace").Count == 0)
				{
					IncludeXmlAttributes();
				}
				break;
			case "http://www.w3.org/2001/XMLSchema-instance":
				if (source.LocalName == "nil")
				{
					el.IsNillable = true;
				}
				continue;
			case "http://www.w3.org/2000/xmlns/":
				continue;
			}
			if (xmlSchemaComplexType == null)
			{
				xmlSchemaComplexType = ToComplexType(el);
				xmlSchemaObjectCollection = GetAttributes(xmlSchemaComplexType);
				hashtable = CollectAttrTable(xmlSchemaObjectCollection);
			}
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(source.LocalName, source.NamespaceURI);
			if (!(hashtable[xmlQualifiedName] is XmlSchemaAttribute xmlSchemaAttribute))
			{
				xmlSchemaObjectCollection.Add(InferNewAttribute(xmlQualifiedName, isNew, ns));
				continue;
			}
			hashtable.Remove(xmlQualifiedName);
			if (!(xmlSchemaAttribute.RefName != null) || !(xmlSchemaAttribute.RefName != XmlQualifiedName.Empty))
			{
				InferMergedAttribute(xmlSchemaAttribute);
			}
		}
		while (source.MoveToNextAttribute());
		if (hashtable == null)
		{
			return;
		}
		foreach (XmlSchemaAttribute value in hashtable.Values)
		{
			value.Use = XmlSchemaUse.Optional;
		}
	}

	private XmlSchemaAttribute InferNewAttribute(XmlQualifiedName attrName, bool isNewTypeDefinition, string ns)
	{
		XmlSchemaAttribute xmlSchemaAttribute = null;
		bool flag = false;
		if (attrName.Namespace.Length > 0)
		{
			xmlSchemaAttribute = GetGlobalAttribute(attrName);
			if (xmlSchemaAttribute == null)
			{
				xmlSchemaAttribute = CreateGlobalAttribute(attrName);
				xmlSchemaAttribute.SchemaTypeName = InferSimpleType(source.Value);
			}
			else
			{
				InferMergedAttribute(xmlSchemaAttribute);
				flag = xmlSchemaAttribute.Use == XmlSchemaUse.Required;
			}
			xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.RefName = attrName;
			AddImport(ns, attrName.Namespace);
		}
		else
		{
			xmlSchemaAttribute = new XmlSchemaAttribute();
			xmlSchemaAttribute.Name = attrName.Name;
			xmlSchemaAttribute.SchemaTypeName = InferSimpleType(source.Value);
		}
		if (!laxOccurrence && (isNewTypeDefinition || flag))
		{
			xmlSchemaAttribute.Use = XmlSchemaUse.Required;
		}
		else
		{
			xmlSchemaAttribute.Use = XmlSchemaUse.Optional;
		}
		return xmlSchemaAttribute;
	}

	private void InferMergedAttribute(XmlSchemaAttribute attr)
	{
		attr.SchemaTypeName = InferMergedType(source.Value, attr.SchemaTypeName);
		attr.SchemaType = null;
	}

	private XmlQualifiedName InferMergedType(string value, XmlQualifiedName typeName)
	{
		XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaType.GetBuiltInSimpleType(typeName);
		if (xmlSchemaSimpleType == null)
		{
			return QNameString;
		}
		do
		{
			try
			{
				xmlSchemaSimpleType.Datatype.ParseValue(value, source.NameTable, source as IXmlNamespaceResolver);
				return typeName;
			}
			catch
			{
				xmlSchemaSimpleType = xmlSchemaSimpleType.BaseXmlSchemaType as XmlSchemaSimpleType;
				typeName = ((xmlSchemaSimpleType == null) ? XmlQualifiedName.Empty : xmlSchemaSimpleType.QualifiedName);
			}
		}
		while (typeName != XmlQualifiedName.Empty);
		return QNameString;
	}

	private XmlSchemaObjectCollection GetAttributes(XmlSchemaComplexType ct)
	{
		if (ct.ContentModel == null)
		{
			return ct.Attributes;
		}
		if (ct.ContentModel is XmlSchemaSimpleContent xmlSchemaSimpleContent)
		{
			if (xmlSchemaSimpleContent.Content is XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension)
			{
				return xmlSchemaSimpleContentExtension.Attributes;
			}
			if (xmlSchemaSimpleContent.Content is XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction)
			{
				return xmlSchemaSimpleContentRestriction.Attributes;
			}
			throw Error(xmlSchemaSimpleContent, "Invalid simple content model.");
		}
		XmlSchemaComplexContent xmlSchemaComplexContent = ct.ContentModel as XmlSchemaComplexContent;
		if (xmlSchemaComplexContent != null)
		{
			if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension)
			{
				return xmlSchemaComplexContentExtension.Attributes;
			}
			if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction)
			{
				return xmlSchemaComplexContentRestriction.Attributes;
			}
			throw Error(xmlSchemaComplexContent, "Invalid simple content model.");
		}
		throw Error(xmlSchemaComplexContent, "Invalid complexType. Should not happen.");
	}

	private XmlSchemaComplexType ToComplexType(XmlSchemaElement el)
	{
		XmlQualifiedName schemaTypeName = el.SchemaTypeName;
		XmlSchemaType schemaType = el.SchemaType;
		if (schemaType is XmlSchemaComplexType result)
		{
			return result;
		}
		XmlSchemaType xmlSchemaType = schemas.GlobalTypes[schemaTypeName] as XmlSchemaType;
		if (xmlSchemaType is XmlSchemaComplexType result2)
		{
			return result2;
		}
		XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)(el.SchemaType = new XmlSchemaComplexType());
		el.SchemaTypeName = XmlQualifiedName.Empty;
		if (schemaTypeName == QNameAnyType)
		{
			return xmlSchemaComplexType;
		}
		if (schemaType == null && schemaTypeName == XmlQualifiedName.Empty)
		{
			return xmlSchemaComplexType;
		}
		XmlSchemaSimpleContent xmlSchemaSimpleContent = (XmlSchemaSimpleContent)(xmlSchemaComplexType.ContentModel = new XmlSchemaSimpleContent());
		if (schemaType is XmlSchemaSimpleType baseType)
		{
			XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction = new XmlSchemaSimpleContentRestriction();
			xmlSchemaSimpleContentRestriction.BaseType = baseType;
			xmlSchemaSimpleContent.Content = xmlSchemaSimpleContentRestriction;
			return xmlSchemaComplexType;
		}
		XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension = (XmlSchemaSimpleContentExtension)(xmlSchemaSimpleContent.Content = new XmlSchemaSimpleContentExtension());
		XmlSchemaSimpleType builtInSimpleType = XmlSchemaType.GetBuiltInSimpleType(schemaTypeName);
		if (builtInSimpleType != null)
		{
			xmlSchemaSimpleContentExtension.BaseTypeName = schemaTypeName;
			return xmlSchemaComplexType;
		}
		if (xmlSchemaType is XmlSchemaSimpleType)
		{
			xmlSchemaSimpleContentExtension.BaseTypeName = schemaTypeName;
			return xmlSchemaComplexType;
		}
		throw Error(el, "Unexpected schema component that contains simpleTypeName that could not be resolved.");
	}

	private void InferAsEmptyElement(XmlSchemaElement el, string ns, bool isNew)
	{
		if (el.SchemaType is XmlSchemaComplexType xmlSchemaComplexType)
		{
			if (xmlSchemaComplexType.ContentModel is XmlSchemaSimpleContent sm)
			{
				ToEmptiableSimpleContent(sm, isNew);
			}
			else if (xmlSchemaComplexType.ContentModel is XmlSchemaComplexContent cm)
			{
				ToEmptiableComplexContent(cm, isNew);
			}
			else if (xmlSchemaComplexType.Particle != null)
			{
				xmlSchemaComplexType.Particle.MinOccurs = 0m;
			}
		}
		else if (el.SchemaType is XmlSchemaSimpleType st)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = MakeBaseTypeAsEmptiable(st);
			switch (xmlSchemaSimpleType.QualifiedName.Namespace)
			{
			case "http://www.w3.org/2001/XMLSchema":
			case "http://www.w3.org/2003/11/xpath-datatypes":
				el.SchemaTypeName = xmlSchemaSimpleType.QualifiedName;
				break;
			default:
				el.SchemaType = xmlSchemaSimpleType;
				break;
			}
		}
	}

	private XmlSchemaSimpleType MakeBaseTypeAsEmptiable(XmlSchemaSimpleType st)
	{
		switch (st.QualifiedName.Namespace)
		{
		case "http://www.w3.org/2001/XMLSchema":
		case "http://www.w3.org/2003/11/xpath-datatypes":
			return XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String);
		default:
			if (st.Content is XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction)
			{
				ArrayList arrayList = null;
				foreach (XmlSchemaFacet facet in xmlSchemaSimpleTypeRestriction.Facets)
				{
					if (facet is XmlSchemaLengthFacet || facet is XmlSchemaMinLengthFacet)
					{
						if (arrayList == null)
						{
							arrayList = new ArrayList();
						}
						arrayList.Add(facet);
					}
				}
				foreach (XmlSchemaFacet item in arrayList)
				{
					xmlSchemaSimpleTypeRestriction.Facets.Remove(item);
				}
				if (xmlSchemaSimpleTypeRestriction.BaseType != null)
				{
					xmlSchemaSimpleTypeRestriction.BaseType = MakeBaseTypeAsEmptiable(st);
				}
				else
				{
					xmlSchemaSimpleTypeRestriction.BaseTypeName = QNameString;
				}
			}
			return st;
		}
	}

	private void ToEmptiableSimpleContent(XmlSchemaSimpleContent sm, bool isNew)
	{
		if (sm.Content is XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension)
		{
			xmlSchemaSimpleContentExtension.BaseTypeName = QNameString;
			return;
		}
		if (!(sm.Content is XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction))
		{
			throw Error(sm, "Invalid simple content model was passed.");
		}
		xmlSchemaSimpleContentRestriction.BaseTypeName = QNameString;
		xmlSchemaSimpleContentRestriction.BaseType = null;
	}

	private void ToEmptiableComplexContent(XmlSchemaComplexContent cm, bool isNew)
	{
		if (cm.Content is XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension)
		{
			if (xmlSchemaComplexContentExtension.Particle != null)
			{
				xmlSchemaComplexContentExtension.Particle.MinOccurs = 0m;
			}
			else if (xmlSchemaComplexContentExtension.BaseTypeName != null && xmlSchemaComplexContentExtension.BaseTypeName != XmlQualifiedName.Empty && xmlSchemaComplexContentExtension.BaseTypeName != QNameAnyType)
			{
				throw Error(xmlSchemaComplexContentExtension, "Complex type content extension has a reference to an external component that is not supported.");
			}
			return;
		}
		if (!(cm.Content is XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction))
		{
			throw Error(cm, "Invalid complex content model was passed.");
		}
		if (xmlSchemaComplexContentRestriction.Particle != null)
		{
			xmlSchemaComplexContentRestriction.Particle.MinOccurs = 0m;
		}
		else if (xmlSchemaComplexContentRestriction.BaseTypeName != null && xmlSchemaComplexContentRestriction.BaseTypeName != XmlQualifiedName.Empty && xmlSchemaComplexContentRestriction.BaseTypeName != QNameAnyType)
		{
			throw Error(xmlSchemaComplexContentRestriction, "Complex type content extension has a reference to an external component that is not supported.");
		}
	}

	private void InferContent(XmlSchemaElement el, string ns, bool isNew)
	{
		source.Read();
		source.MoveToContent();
		switch (source.NodeType)
		{
		case XmlNodeType.EndElement:
			InferAsEmptyElement(el, ns, isNew);
			break;
		case XmlNodeType.Element:
			InferComplexContent(el, ns, isNew);
			break;
		case XmlNodeType.Text:
		case XmlNodeType.CDATA:
		case XmlNodeType.SignificantWhitespace:
			InferTextContent(el, isNew);
			source.MoveToContent();
			if (source.NodeType == XmlNodeType.Element)
			{
				goto case XmlNodeType.Element;
			}
			break;
		case XmlNodeType.Whitespace:
			InferContent(el, ns, isNew);
			break;
		}
	}

	private void InferComplexContent(XmlSchemaElement el, string ns, bool isNew)
	{
		XmlSchemaComplexType xmlSchemaComplexType = ToComplexType(el);
		ToComplexContentType(xmlSchemaComplexType);
		int position = 0;
		bool consumed = false;
		while (true)
		{
			switch (source.NodeType)
			{
			case XmlNodeType.Element:
			{
				XmlSchemaSequence xmlSchemaSequence = PopulateSequence(xmlSchemaComplexType);
				XmlSchemaChoice xmlSchemaChoice = ((xmlSchemaSequence.Items.Count <= 0) ? null : (xmlSchemaSequence.Items[0] as XmlSchemaChoice));
				if (xmlSchemaChoice != null)
				{
					ProcessLax(xmlSchemaChoice, ns);
				}
				else
				{
					ProcessSequence(xmlSchemaComplexType, xmlSchemaSequence, ns, ref position, ref consumed, isNew);
				}
				source.MoveToContent();
				break;
			}
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.SignificantWhitespace:
				MarkAsMixed(xmlSchemaComplexType);
				source.ReadString();
				source.MoveToContent();
				break;
			case XmlNodeType.EndElement:
				return;
			case XmlNodeType.None:
				throw new NotImplementedException("Internal Error: Should not happen.");
			}
		}
	}

	private void InferTextContent(XmlSchemaElement el, bool isNew)
	{
		string value = source.ReadString();
		if (el.SchemaType == null)
		{
			if (el.SchemaTypeName == XmlQualifiedName.Empty)
			{
				if (isNew)
				{
					el.SchemaTypeName = InferSimpleType(value);
				}
				else
				{
					el.SchemaTypeName = QNameString;
				}
				return;
			}
			switch (el.SchemaTypeName.Namespace)
			{
			case "http://www.w3.org/2001/XMLSchema":
			case "http://www.w3.org/2003/11/xpath-datatypes":
				el.SchemaTypeName = InferMergedType(value, el.SchemaTypeName);
				return;
			}
			if (schemas.GlobalTypes[el.SchemaTypeName] is XmlSchemaComplexType ct)
			{
				MarkAsMixed(ct);
			}
			else
			{
				el.SchemaTypeName = QNameString;
			}
			return;
		}
		if (el.SchemaType is XmlSchemaSimpleType)
		{
			el.SchemaType = null;
			el.SchemaTypeName = QNameString;
			return;
		}
		XmlSchemaComplexType xmlSchemaComplexType = el.SchemaType as XmlSchemaComplexType;
		if (!(xmlSchemaComplexType.ContentModel is XmlSchemaSimpleContent xmlSchemaSimpleContent))
		{
			MarkAsMixed(xmlSchemaComplexType);
			return;
		}
		if (xmlSchemaSimpleContent.Content is XmlSchemaSimpleContentExtension xmlSchemaSimpleContentExtension)
		{
			xmlSchemaSimpleContentExtension.BaseTypeName = InferMergedType(value, xmlSchemaSimpleContentExtension.BaseTypeName);
		}
		if (xmlSchemaSimpleContent.Content is XmlSchemaSimpleContentRestriction xmlSchemaSimpleContentRestriction)
		{
			xmlSchemaSimpleContentRestriction.BaseTypeName = InferMergedType(value, xmlSchemaSimpleContentRestriction.BaseTypeName);
			xmlSchemaSimpleContentRestriction.BaseType = null;
		}
	}

	private void MarkAsMixed(XmlSchemaComplexType ct)
	{
		if (ct.ContentModel is XmlSchemaComplexContent xmlSchemaComplexContent)
		{
			xmlSchemaComplexContent.IsMixed = true;
		}
		else
		{
			ct.IsMixed = true;
		}
	}

	private void ProcessLax(XmlSchemaChoice c, string ns)
	{
		foreach (XmlSchemaParticle item in c.Items)
		{
			if (!(item is XmlSchemaElement el))
			{
				throw Error(c, string.Format("Target schema item contains unacceptable particle {0}. Only element is allowed here."));
			}
			if (ElementMatches(el, ns))
			{
				InferElement(el, ns, isNew: false);
				return;
			}
		}
		XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
		if (source.NamespaceURI == ns)
		{
			xmlSchemaElement.Name = source.LocalName;
		}
		else
		{
			xmlSchemaElement.RefName = new XmlQualifiedName(source.LocalName, source.NamespaceURI);
			AddImport(ns, source.NamespaceURI);
		}
		InferElement(xmlSchemaElement, source.NamespaceURI, isNew: true);
		c.Items.Add(xmlSchemaElement);
	}

	private bool ElementMatches(XmlSchemaElement el, string ns)
	{
		bool result = false;
		if (el.RefName != XmlQualifiedName.Empty)
		{
			if (el.RefName.Name == source.LocalName && el.RefName.Namespace == source.NamespaceURI)
			{
				result = true;
			}
		}
		else if (el.Name == source.LocalName && ns == source.NamespaceURI)
		{
			result = true;
		}
		return result;
	}

	private void ProcessSequence(XmlSchemaComplexType ct, XmlSchemaSequence s, string ns, ref int position, ref bool consumed, bool isNew)
	{
		for (int i = 0; i < position; i++)
		{
			XmlSchemaElement el = s.Items[i] as XmlSchemaElement;
			if (ElementMatches(el, ns))
			{
				ProcessLax(ToSequenceOfChoice(s), ns);
				return;
			}
		}
		if (s.Items.Count <= position)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(source.LocalName, source.NamespaceURI);
			XmlSchemaElement xmlSchemaElement = CreateElement(xmlQualifiedName);
			if (laxOccurrence)
			{
				xmlSchemaElement.MinOccurs = 0m;
			}
			InferElement(xmlSchemaElement, ns, isNew: true);
			if (ns == xmlQualifiedName.Namespace)
			{
				s.Items.Add(xmlSchemaElement);
			}
			else
			{
				XmlSchemaElement xmlSchemaElement2 = new XmlSchemaElement();
				if (laxOccurrence)
				{
					xmlSchemaElement2.MinOccurs = 0m;
				}
				xmlSchemaElement2.RefName = xmlQualifiedName;
				AddImport(ns, xmlQualifiedName.Namespace);
				s.Items.Add(xmlSchemaElement2);
			}
			consumed = true;
			return;
		}
		if (!(s.Items[position] is XmlSchemaElement xmlSchemaElement3))
		{
			throw Error(s, $"Target complex type content sequence has an unacceptable type of particle {s.Items[position]}");
		}
		if (ElementMatches(xmlSchemaElement3, ns))
		{
			if (consumed)
			{
				xmlSchemaElement3.MaxOccursString = "unbounded";
			}
			InferElement(xmlSchemaElement3, source.NamespaceURI, isNew: false);
			source.MoveToContent();
			switch (source.NodeType)
			{
			case XmlNodeType.None:
				if (source.NodeType == XmlNodeType.Element)
				{
					goto case XmlNodeType.Element;
				}
				if (source.NodeType != XmlNodeType.EndElement)
				{
				}
				break;
			case XmlNodeType.Element:
				ProcessSequence(ct, s, ns, ref position, ref consumed, isNew);
				break;
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.SignificantWhitespace:
				MarkAsMixed(ct);
				source.ReadString();
				goto case XmlNodeType.None;
			case XmlNodeType.Whitespace:
				source.ReadString();
				goto case XmlNodeType.None;
			case XmlNodeType.EndElement:
				break;
			default:
				source.Read();
				break;
			}
		}
		else if (consumed)
		{
			position++;
			consumed = false;
			ProcessSequence(ct, s, ns, ref position, ref consumed, isNew);
		}
		else
		{
			ProcessLax(ToSequenceOfChoice(s), ns);
		}
	}

	private XmlSchemaChoice ToSequenceOfChoice(XmlSchemaSequence s)
	{
		XmlSchemaChoice xmlSchemaChoice = new XmlSchemaChoice();
		if (laxOccurrence)
		{
			xmlSchemaChoice.MinOccurs = 0m;
		}
		xmlSchemaChoice.MaxOccursString = "unbounded";
		foreach (XmlSchemaParticle item in s.Items)
		{
			xmlSchemaChoice.Items.Add(item);
		}
		s.Items.Clear();
		s.Items.Add(xmlSchemaChoice);
		return xmlSchemaChoice;
	}

	private void ToComplexContentType(XmlSchemaComplexType type)
	{
		if (!(type.ContentModel is XmlSchemaSimpleContent))
		{
			return;
		}
		XmlSchemaObjectCollection attributes = GetAttributes(type);
		foreach (XmlSchemaObject item in attributes)
		{
			type.Attributes.Add(item);
		}
		type.ContentModel = null;
		type.IsMixed = true;
	}

	private XmlSchemaSequence PopulateSequence(XmlSchemaComplexType ct)
	{
		XmlSchemaParticle xmlSchemaParticle = PopulateParticle(ct);
		if (xmlSchemaParticle is XmlSchemaSequence result)
		{
			return result;
		}
		throw Error(ct, $"Target complexType contains unacceptable type of particle {xmlSchemaParticle}");
	}

	private XmlSchemaSequence CreateSequence()
	{
		XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
		if (laxOccurrence)
		{
			xmlSchemaSequence.MinOccurs = 0m;
		}
		return xmlSchemaSequence;
	}

	private XmlSchemaParticle PopulateParticle(XmlSchemaComplexType ct)
	{
		if (ct.ContentModel == null)
		{
			if (ct.Particle == null)
			{
				ct.Particle = CreateSequence();
			}
			return ct.Particle;
		}
		if (ct.ContentModel is XmlSchemaComplexContent xmlSchemaComplexContent)
		{
			if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension)
			{
				if (xmlSchemaComplexContentExtension.Particle == null)
				{
					xmlSchemaComplexContentExtension.Particle = CreateSequence();
				}
				return xmlSchemaComplexContentExtension.Particle;
			}
			if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction)
			{
				if (xmlSchemaComplexContentRestriction.Particle == null)
				{
					xmlSchemaComplexContentRestriction.Particle = CreateSequence();
				}
				return xmlSchemaComplexContentRestriction.Particle;
			}
		}
		throw Error(ct, "Schema inference internal error. The complexType should have been converted to have a complex content.");
	}

	private XmlQualifiedName InferSimpleType(string value)
	{
		if (laxTypeInference)
		{
			return QNameString;
		}
		switch (value)
		{
		case "true":
		case "false":
			return QNameBoolean;
		default:
			try
			{
				long num = XmlConvert.ToInt64(value);
				if (0 <= num && num <= 255)
				{
					return QNameUByte;
				}
				if (-128 <= num && num <= 127)
				{
					return QNameByte;
				}
				if (0 <= num && num <= 65535)
				{
					return QNameUShort;
				}
				if (-32768 <= num && num <= 32767)
				{
					return QNameShort;
				}
				if (0 <= num && num <= uint.MaxValue)
				{
					return QNameUInt;
				}
				if (int.MinValue <= num && num <= int.MaxValue)
				{
					return QNameInt;
				}
				return QNameLong;
			}
			catch (Exception)
			{
			}
			try
			{
				XmlConvert.ToUInt64(value);
				return QNameULong;
			}
			catch (Exception)
			{
			}
			try
			{
				XmlConvert.ToDecimal(value);
				return QNameDecimal;
			}
			catch (Exception)
			{
			}
			try
			{
				double num2 = XmlConvert.ToDouble(value);
				if (-3.4028234663852886E+38 <= num2 && num2 <= 3.4028234663852886E+38)
				{
					return QNameFloat;
				}
				return QNameDouble;
			}
			catch (Exception)
			{
			}
			try
			{
				XmlConvert.ToDateTime(value);
				return QNameDateTime;
			}
			catch (Exception)
			{
			}
			try
			{
				XmlConvert.ToTimeSpan(value);
				return QNameDuration;
			}
			catch (Exception)
			{
			}
			return QNameString;
		}
	}

	private XmlSchemaElement GetGlobalElement(XmlQualifiedName name)
	{
		XmlSchemaElement xmlSchemaElement = newElements[name] as XmlSchemaElement;
		if (xmlSchemaElement == null)
		{
			xmlSchemaElement = schemas.GlobalElements[name] as XmlSchemaElement;
		}
		return xmlSchemaElement;
	}

	private XmlSchemaAttribute GetGlobalAttribute(XmlQualifiedName name)
	{
		XmlSchemaAttribute xmlSchemaAttribute = newElements[name] as XmlSchemaAttribute;
		if (xmlSchemaAttribute == null)
		{
			xmlSchemaAttribute = schemas.GlobalAttributes[name] as XmlSchemaAttribute;
		}
		return xmlSchemaAttribute;
	}

	private XmlSchemaElement CreateElement(XmlQualifiedName name)
	{
		XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
		xmlSchemaElement.Name = name.Name;
		return xmlSchemaElement;
	}

	private XmlSchemaElement CreateGlobalElement(XmlQualifiedName name)
	{
		XmlSchemaElement xmlSchemaElement = CreateElement(name);
		XmlSchema xmlSchema = PopulateSchema(name.Namespace);
		xmlSchema.Items.Add(xmlSchemaElement);
		newElements.Add(name, xmlSchemaElement);
		return xmlSchemaElement;
	}

	private XmlSchemaAttribute CreateGlobalAttribute(XmlQualifiedName name)
	{
		XmlSchemaAttribute xmlSchemaAttribute = new XmlSchemaAttribute();
		XmlSchema xmlSchema = PopulateSchema(name.Namespace);
		xmlSchemaAttribute.Name = name.Name;
		xmlSchema.Items.Add(xmlSchemaAttribute);
		newAttributes.Add(name, xmlSchemaAttribute);
		return xmlSchemaAttribute;
	}

	private XmlSchema PopulateSchema(string ns)
	{
		ICollection collection = schemas.Schemas(ns);
		if (collection.Count > 0)
		{
			IEnumerator enumerator = collection.GetEnumerator();
			enumerator.MoveNext();
			return (XmlSchema)enumerator.Current;
		}
		XmlSchema xmlSchema = new XmlSchema();
		if (ns != null && ns.Length > 0)
		{
			xmlSchema.TargetNamespace = ns;
		}
		xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
		xmlSchema.AttributeFormDefault = XmlSchemaForm.Unqualified;
		schemas.Add(xmlSchema);
		return xmlSchema;
	}

	private XmlSchemaInferenceException Error(XmlSchemaObject sourceObj, string message)
	{
		return Error(sourceObj, useReader: false, message);
	}

	private XmlSchemaInferenceException Error(XmlSchemaObject sourceObj, bool useReader, string message)
	{
		string message2 = message + ((sourceObj == null) ? string.Empty : string.Format(". Related schema component is {0}", sourceObj.SourceUri, sourceObj.LineNumber, sourceObj.LinePosition)) + ((!useReader) ? string.Empty : $". {source.BaseURI}");
		IXmlLineInfo xmlLineInfo = source as IXmlLineInfo;
		if (useReader && xmlLineInfo != null)
		{
			return new XmlSchemaInferenceException(message2, null, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition);
		}
		return new XmlSchemaInferenceException(message2);
	}
}
