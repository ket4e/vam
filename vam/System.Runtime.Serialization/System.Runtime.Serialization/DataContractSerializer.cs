using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace System.Runtime.Serialization;

public sealed class DataContractSerializer : XmlObjectSerializer
{
	private const string xmlns = "http://www.w3.org/2000/xmlns/";

	private Type type;

	private bool ignore_ext;

	private bool preserve_refs;

	private StreamingContext context;

	private ReadOnlyCollection<Type> known_runtime_types;

	private KnownTypeCollection known_types;

	private IDataContractSurrogate surrogate;

	private int max_items = 65536;

	private bool names_filled;

	private XmlDictionaryString root_name;

	private XmlDictionaryString root_ns;

	public bool IgnoreExtensionDataObject => ignore_ext;

	public ReadOnlyCollection<Type> KnownTypes => known_runtime_types;

	public IDataContractSurrogate DataContractSurrogate => surrogate;

	public int MaxItemsInObjectGraph => max_items;

	public bool PreserveObjectReferences => preserve_refs;

	public DataContractSerializer(Type type)
		: this(type, Type.EmptyTypes)
	{
	}

	public DataContractSerializer(Type type, IEnumerable<Type> knownTypes)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		this.type = type;
		known_types = new KnownTypeCollection();
		PopulateTypes(knownTypes);
		known_types.TryRegister(type);
		XmlQualifiedName qName = known_types.GetQName(type);
		FillDictionaryString(qName.Name, qName.Namespace);
	}

	public DataContractSerializer(Type type, string rootName, string rootNamespace)
		: this(type, rootName, rootNamespace, Type.EmptyTypes)
	{
	}

	public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
		: this(type, rootName, rootNamespace, Type.EmptyTypes)
	{
	}

	public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (rootName == null)
		{
			throw new ArgumentNullException("rootName");
		}
		if (rootNamespace == null)
		{
			throw new ArgumentNullException("rootNamespace");
		}
		this.type = type;
		PopulateTypes(knownTypes);
		FillDictionaryString(rootName, rootNamespace);
	}

	public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (rootName == null)
		{
			throw new ArgumentNullException("rootName");
		}
		if (rootNamespace == null)
		{
			throw new ArgumentNullException("rootNamespace");
		}
		this.type = type;
		PopulateTypes(knownTypes);
		root_name = rootName;
		root_ns = rootNamespace;
	}

	public DataContractSerializer(Type type, IEnumerable<Type> knownTypes, int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		: this(type, knownTypes)
	{
		Initialize(maxObjectsInGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
	}

	public DataContractSerializer(Type type, string rootName, string rootNamespace, IEnumerable<Type> knownTypes, int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		: this(type, rootName, rootNamespace, knownTypes)
	{
		Initialize(maxObjectsInGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
	}

	public DataContractSerializer(Type type, XmlDictionaryString rootName, XmlDictionaryString rootNamespace, IEnumerable<Type> knownTypes, int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
		: this(type, rootName, rootNamespace, knownTypes)
	{
		Initialize(maxObjectsInGraph, ignoreExtensionDataObject, preserveObjectReferences, dataContractSurrogate);
	}

	private void PopulateTypes(IEnumerable<Type> knownTypes)
	{
		if (known_types == null)
		{
			known_types = new KnownTypeCollection();
		}
		if (knownTypes != null)
		{
			foreach (Type knownType in knownTypes)
			{
				known_types.TryRegister(knownType);
			}
		}
		Type elementType = type;
		if (type.HasElementType)
		{
			elementType = type.GetElementType();
		}
		object[] customAttributes = elementType.GetCustomAttributes(typeof(KnownTypeAttribute), inherit: true);
		for (int i = 0; i < customAttributes.Length; i++)
		{
			KnownTypeAttribute knownTypeAttribute = (KnownTypeAttribute)customAttributes[i];
			known_types.TryRegister(knownTypeAttribute.Type);
		}
	}

	private void FillDictionaryString(string name, string ns)
	{
		XmlDictionary xmlDictionary = new XmlDictionary();
		root_name = xmlDictionary.Add(name);
		root_ns = xmlDictionary.Add(ns);
		names_filled = true;
	}

	private void Initialize(int maxObjectsInGraph, bool ignoreExtensionDataObject, bool preserveObjectReferences, IDataContractSurrogate dataContractSurrogate)
	{
		if (maxObjectsInGraph < 0)
		{
			throw new ArgumentOutOfRangeException("maxObjectsInGraph must not be negative.");
		}
		max_items = maxObjectsInGraph;
		ignore_ext = ignoreExtensionDataObject;
		preserve_refs = preserveObjectReferences;
		surrogate = dataContractSurrogate;
		PopulateTypes(Type.EmptyTypes);
	}

	[System.MonoTODO]
	public override bool IsStartObject(XmlDictionaryReader reader)
	{
		throw new NotImplementedException();
	}

	public override bool IsStartObject(XmlReader reader)
	{
		return IsStartObject(XmlDictionaryReader.CreateDictionaryReader(reader));
	}

	public override object ReadObject(XmlReader reader)
	{
		return ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader));
	}

	public override object ReadObject(XmlReader reader, bool verifyObjectName)
	{
		return ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader), verifyObjectName);
	}

	[System.MonoTODO]
	public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
	{
		int count = known_types.Count;
		known_types.Add(type);
		bool isEmptyElement = reader.IsEmptyElement;
		object result = XmlFormatterDeserializer.Deserialize(reader, type, known_types, surrogate, root_name.Value, root_ns.Value, verifyObjectName);
		while (known_types.Count > count)
		{
			known_types.RemoveAt(count);
		}
		return result;
	}

	private void ReadRootStartElement(XmlReader reader, Type type)
	{
		SerializationMap serializationMap = known_types.FindUserMap(type);
		XmlQualifiedName xmlQualifiedName = ((serializationMap == null) ? KnownTypeCollection.GetPredefinedTypeName(type) : serializationMap.XmlName);
		reader.MoveToContent();
		reader.ReadStartElement(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
		reader.Read();
	}

	public override void WriteObject(XmlWriter writer, object graph)
	{
		XmlDictionaryWriter writer2 = XmlDictionaryWriter.CreateDictionaryWriter(writer);
		WriteObject(writer2, graph);
	}

	[System.MonoTODO("support arrays; support Serializable; support SharedType; use DataContractSurrogate")]
	public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
	{
		if (graph != null)
		{
			int count = known_types.Count;
			XmlFormatterSerializer.Serialize(writer, graph, known_types, ignore_ext, max_items, root_ns.Value);
			while (known_types.Count > count)
			{
				known_types.RemoveAt(count);
			}
		}
	}

	public override void WriteObjectContent(XmlWriter writer, object graph)
	{
		XmlDictionaryWriter writer2 = XmlDictionaryWriter.CreateDictionaryWriter(writer);
		WriteObjectContent(writer2, graph);
	}

	public override void WriteStartObject(XmlWriter writer, object graph)
	{
		WriteStartObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
	}

	public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
	{
		Type type = this.type;
		if (root_name.Value == string.Empty)
		{
			throw new InvalidDataContractException("Type '" + this.type.ToString() + "' cannot have a DataContract attribute Name set to null or empty string.");
		}
		if (graph == null)
		{
			if (names_filled)
			{
				writer.WriteStartElement(root_name.Value, root_ns.Value);
			}
			else
			{
				writer.WriteStartElement(root_name, root_ns);
			}
			writer.WriteAttributeString("i", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			return;
		}
		XmlQualifiedName xmlQualifiedName = null;
		XmlQualifiedName qName = known_types.GetQName(type);
		XmlQualifiedName qName2 = known_types.GetQName(graph.GetType());
		known_types.Add(graph.GetType());
		if (names_filled)
		{
			writer.WriteStartElement(root_name.Value, root_ns.Value);
		}
		else
		{
			writer.WriteStartElement(root_name, root_ns);
		}
		if (root_ns.Value != qName.Namespace && qName.Namespace != "http://schemas.microsoft.com/2003/10/Serialization/")
		{
			writer.WriteXmlnsAttribute(null, qName.Namespace);
		}
		if (qName == qName2)
		{
			if (qName.Namespace != "http://schemas.microsoft.com/2003/10/Serialization/" && !type.IsEnum)
			{
				writer.WriteXmlnsAttribute("i", "http://www.w3.org/2001/XMLSchema-instance");
			}
			return;
		}
		known_types.Add(type);
		xmlQualifiedName = KnownTypeCollection.GetPredefinedTypeName(graph.GetType());
		xmlQualifiedName = ((!(xmlQualifiedName == XmlQualifiedName.Empty)) ? new XmlQualifiedName(xmlQualifiedName.Name, "http://www.w3.org/2001/XMLSchema") : qName2);
		writer.WriteStartAttribute("i", "type", "http://www.w3.org/2001/XMLSchema-instance");
		writer.WriteQualifiedName(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
		writer.WriteEndAttribute();
	}

	public override void WriteEndObject(XmlDictionaryWriter writer)
	{
		writer.WriteEndElement();
	}

	public override void WriteEndObject(XmlWriter writer)
	{
		WriteEndObject(XmlDictionaryWriter.CreateDictionaryWriter(writer));
	}
}
