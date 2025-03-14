using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Mono.Xml2;

namespace Mono.Xml;

internal class DTDObjectModel
{
	public const int AllowedExternalEntitiesMax = 256;

	private DTDAutomataFactory factory;

	private DTDElementAutomata rootAutomata;

	private DTDEmptyAutomata emptyAutomata;

	private DTDAnyAutomata anyAutomata;

	private DTDInvalidAutomata invalidAutomata;

	private DTDElementDeclarationCollection elementDecls;

	private DTDAttListDeclarationCollection attListDecls;

	private DTDParameterEntityDeclarationCollection peDecls;

	private DTDEntityDeclarationCollection entityDecls;

	private DTDNotationDeclarationCollection notationDecls;

	private ArrayList validationErrors;

	private XmlResolver resolver;

	private XmlNameTable nameTable;

	private Hashtable externalResources;

	private string baseURI;

	private string name;

	private string publicId;

	private string systemId;

	private string intSubset;

	private bool intSubsetHasPERef;

	private bool isStandalone;

	private int lineNumber;

	private int linePosition;

	public string BaseURI
	{
		get
		{
			return baseURI;
		}
		set
		{
			baseURI = value;
		}
	}

	public bool IsStandalone
	{
		get
		{
			return isStandalone;
		}
		set
		{
			isStandalone = value;
		}
	}

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

	public XmlNameTable NameTable => nameTable;

	public string PublicId
	{
		get
		{
			return publicId;
		}
		set
		{
			publicId = value;
		}
	}

	public string SystemId
	{
		get
		{
			return systemId;
		}
		set
		{
			systemId = value;
		}
	}

	public string InternalSubset
	{
		get
		{
			return intSubset;
		}
		set
		{
			intSubset = value;
		}
	}

	public bool InternalSubsetHasPEReference
	{
		get
		{
			return intSubsetHasPERef;
		}
		set
		{
			intSubsetHasPERef = value;
		}
	}

	public int LineNumber
	{
		get
		{
			return lineNumber;
		}
		set
		{
			lineNumber = value;
		}
	}

	public int LinePosition
	{
		get
		{
			return linePosition;
		}
		set
		{
			linePosition = value;
		}
	}

	internal XmlResolver Resolver => resolver;

	public XmlResolver XmlResolver
	{
		set
		{
			resolver = value;
		}
	}

	internal Hashtable ExternalResources => externalResources;

	public DTDAutomataFactory Factory => factory;

	public DTDElementDeclaration RootElement => ElementDecls[Name];

	public DTDElementDeclarationCollection ElementDecls => elementDecls;

	public DTDAttListDeclarationCollection AttListDecls => attListDecls;

	public DTDEntityDeclarationCollection EntityDecls => entityDecls;

	public DTDParameterEntityDeclarationCollection PEDecls => peDecls;

	public DTDNotationDeclarationCollection NotationDecls => notationDecls;

	public DTDAutomata RootAutomata
	{
		get
		{
			if (rootAutomata == null)
			{
				rootAutomata = new DTDElementAutomata(this, Name);
			}
			return rootAutomata;
		}
	}

	public DTDEmptyAutomata Empty
	{
		get
		{
			if (emptyAutomata == null)
			{
				emptyAutomata = new DTDEmptyAutomata(this);
			}
			return emptyAutomata;
		}
	}

	public DTDAnyAutomata Any
	{
		get
		{
			if (anyAutomata == null)
			{
				anyAutomata = new DTDAnyAutomata(this);
			}
			return anyAutomata;
		}
	}

	public DTDInvalidAutomata Invalid
	{
		get
		{
			if (invalidAutomata == null)
			{
				invalidAutomata = new DTDInvalidAutomata(this);
			}
			return invalidAutomata;
		}
	}

	public XmlSchemaException[] Errors => validationErrors.ToArray(typeof(XmlSchemaException)) as XmlSchemaException[];

	public DTDObjectModel(XmlNameTable nameTable)
	{
		this.nameTable = nameTable;
		elementDecls = new DTDElementDeclarationCollection(this);
		attListDecls = new DTDAttListDeclarationCollection(this);
		entityDecls = new DTDEntityDeclarationCollection(this);
		peDecls = new DTDParameterEntityDeclarationCollection(this);
		notationDecls = new DTDNotationDeclarationCollection(this);
		factory = new DTDAutomataFactory(this);
		validationErrors = new ArrayList();
		externalResources = new Hashtable();
	}

	internal XmlSchema CreateXsdSchema()
	{
		XmlSchema xmlSchema = new XmlSchema();
		xmlSchema.SourceUri = BaseURI;
		xmlSchema.LineNumber = LineNumber;
		xmlSchema.LinePosition = LinePosition;
		foreach (DTDElementDeclaration value in ElementDecls.Values)
		{
			xmlSchema.Items.Add(value.CreateXsdElement());
		}
		return xmlSchema;
	}

	public string ResolveEntity(string name)
	{
		DTDEntityDeclaration dTDEntityDeclaration = EntityDecls[name];
		if (dTDEntityDeclaration == null)
		{
			AddError(new XmlSchemaException("Required entity was not found.", LineNumber, LinePosition, null, BaseURI, null));
			return " ";
		}
		return dTDEntityDeclaration.EntityValue;
	}

	public void AddError(XmlSchemaException ex)
	{
		validationErrors.Add(ex);
	}

	internal string GenerateEntityAttributeText(string entityName)
	{
		return EntityDecls[entityName]?.EntityValue;
	}

	internal Mono.Xml2.XmlTextReader GenerateEntityContentReader(string entityName, XmlParserContext context)
	{
		DTDEntityDeclaration dTDEntityDeclaration = EntityDecls[entityName];
		if (dTDEntityDeclaration == null)
		{
			return null;
		}
		if (dTDEntityDeclaration.SystemId != null)
		{
			Uri baseUri = ((!(dTDEntityDeclaration.BaseURI == string.Empty)) ? new Uri(dTDEntityDeclaration.BaseURI) : null);
			Stream xmlFragment = resolver.GetEntity(resolver.ResolveUri(baseUri, dTDEntityDeclaration.SystemId), null, typeof(Stream)) as Stream;
			return new Mono.Xml2.XmlTextReader(xmlFragment, XmlNodeType.Element, context);
		}
		return new Mono.Xml2.XmlTextReader(dTDEntityDeclaration.EntityValue, XmlNodeType.Element, context);
	}
}
