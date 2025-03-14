using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization;

public class XsdDataContractExporter
{
	private ExportOptions options;

	private KnownTypeCollection known_types;

	private XmlSchemaSet schemas;

	private Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types;

	private static XmlSchema mstypes_schema;

	public XmlSchemaSet Schemas
	{
		get
		{
			if (schemas == null)
			{
				schemas = new XmlSchemaSet();
				schemas.Add(MSTypesSchema);
			}
			return schemas;
		}
	}

	public ExportOptions Options
	{
		get
		{
			return options;
		}
		set
		{
			options = value;
		}
	}

	private KnownTypeCollection KnownTypes
	{
		get
		{
			if (known_types == null)
			{
				known_types = new KnownTypeCollection();
			}
			return known_types;
		}
	}

	private Dictionary<XmlQualifiedName, XmlSchemaType> GeneratedTypes
	{
		get
		{
			if (generated_schema_types == null)
			{
				generated_schema_types = new Dictionary<XmlQualifiedName, XmlSchemaType>();
			}
			return generated_schema_types;
		}
	}

	private static XmlSchema MSTypesSchema
	{
		get
		{
			if (mstypes_schema == null)
			{
				Assembly callingAssembly = Assembly.GetCallingAssembly();
				Stream manifestResourceStream = callingAssembly.GetManifestResourceStream("mstypes.schema");
				mstypes_schema = XmlSchema.Read(manifestResourceStream, null);
			}
			return mstypes_schema;
		}
	}

	public XsdDataContractExporter()
	{
	}

	public XsdDataContractExporter(XmlSchemaSet schemas)
	{
		this.schemas = schemas;
	}

	public bool CanExport(ICollection<Type> types)
	{
		foreach (Type type in types)
		{
			if (!CanExport(type))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanExport(ICollection<Assembly> assemblies)
	{
		foreach (Assembly assembly in assemblies)
		{
			Module[] modules = assembly.GetModules();
			foreach (Module module in modules)
			{
				Type[] types = module.GetTypes();
				foreach (Type type in types)
				{
					if (!CanExport(type))
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public bool CanExport(Type type)
	{
		return !KnownTypes.GetQName(type).IsEmpty;
	}

	public void Export(ICollection<Type> types)
	{
		foreach (Type type in types)
		{
			Export(type);
		}
	}

	public void Export(ICollection<Assembly> assemblies)
	{
		foreach (Assembly assembly in assemblies)
		{
			Module[] modules = assembly.GetModules();
			foreach (Module module in modules)
			{
				Type[] types = module.GetTypes();
				foreach (Type type in types)
				{
					Export(type);
				}
			}
		}
	}

	[System.MonoTODO]
	public void Export(Type type)
	{
		KnownTypes.Add(type);
		SerializationMap serializationMap = KnownTypes.FindUserMap(type);
		if (serializationMap != null)
		{
			serializationMap.GetSchemaType(Schemas, GeneratedTypes);
			Schemas.Compile();
		}
	}

	[System.MonoTODO]
	public XmlQualifiedName GetRootElementName(Type type)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO]
	public XmlSchemaType GetSchemaType(Type type)
	{
		return KnownTypes.FindUserMap(type)?.GetSchemaType(Schemas, GeneratedTypes);
	}

	public XmlQualifiedName GetSchemaTypeName(Type type)
	{
		XmlQualifiedName qName = KnownTypes.GetQName(type);
		if (qName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
		{
			return new XmlQualifiedName(qName.Name, "http://www.w3.org/2001/XMLSchema");
		}
		return qName;
	}
}
