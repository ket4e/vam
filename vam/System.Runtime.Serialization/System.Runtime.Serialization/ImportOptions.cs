using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace System.Runtime.Serialization;

public class ImportOptions
{
	private IDataContractSurrogate surrogate;

	private ICollection<Type> referenced_collection_types = new List<Type>();

	private ICollection<Type> referenced_types = new List<Type>();

	private bool enable_data_binding;

	private bool generate_internal;

	private bool generate_serializable;

	private bool import_xml_type;

	private IDictionary<string, string> namespaces = new Dictionary<string, string>();

	private CodeDomProvider code_provider;

	public CodeDomProvider CodeProvider
	{
		get
		{
			return code_provider;
		}
		set
		{
			code_provider = value;
		}
	}

	public IDataContractSurrogate DataContractSurrogate
	{
		get
		{
			return surrogate;
		}
		set
		{
			surrogate = value;
		}
	}

	public bool EnableDataBinding
	{
		get
		{
			return enable_data_binding;
		}
		set
		{
			enable_data_binding = value;
		}
	}

	public bool GenerateInternal
	{
		get
		{
			return generate_internal;
		}
		set
		{
			generate_internal = value;
		}
	}

	public bool GenerateSerializable
	{
		get
		{
			return generate_serializable;
		}
		set
		{
			generate_serializable = value;
		}
	}

	public bool ImportXmlType
	{
		get
		{
			return import_xml_type;
		}
		set
		{
			import_xml_type = value;
		}
	}

	public IDictionary<string, string> Namespaces => namespaces;

	public ICollection<Type> ReferencedCollectionTypes => referenced_collection_types;

	public ICollection<Type> ReferencedTypes => referenced_types;
}
