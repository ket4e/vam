using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Xml;

namespace System.Runtime.Serialization;

public sealed class NetDataContractSerializer : XmlObjectSerializer, IFormatter
{
	private const string xmlns = "http://www.w3.org/2000/xmlns/";

	private const string default_ns = "http://schemas.datacontract.org/2004/07/";

	private StreamingContext context;

	private SerializationBinder binder;

	private ISurrogateSelector selector;

	private int max_items = 65536;

	private bool ignore_extensions;

	private FormatterAssemblyStyle ass_style;

	private XmlDictionaryString root_name;

	private XmlDictionaryString root_ns;

	public FormatterAssemblyStyle AssemblyFormat
	{
		get
		{
			return ass_style;
		}
		set
		{
			ass_style = value;
		}
	}

	public SerializationBinder Binder
	{
		get
		{
			return binder;
		}
		set
		{
			binder = value;
		}
	}

	public bool IgnoreExtensionDataObject => ignore_extensions;

	public ISurrogateSelector SurrogateSelector
	{
		get
		{
			return selector;
		}
		set
		{
			selector = value;
		}
	}

	public StreamingContext Context
	{
		get
		{
			return context;
		}
		set
		{
			context = value;
		}
	}

	public int MaxItemsInObjectGraph => max_items;

	public NetDataContractSerializer()
	{
	}

	public NetDataContractSerializer(StreamingContext context)
	{
		this.context = context;
	}

	public NetDataContractSerializer(string rootName, string rootNamespace)
	{
		FillDictionaryString(rootName, rootNamespace);
	}

	public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace)
	{
		if (rootName == null)
		{
			throw new ArgumentNullException("rootName");
		}
		if (rootNamespace == null)
		{
			throw new ArgumentNullException("rootNamespace");
		}
		root_name = rootName;
		root_ns = rootNamespace;
	}

	public NetDataContractSerializer(StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensibleDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
	{
		this.context = context;
		max_items = maxItemsInObjectGraph;
		ignore_extensions = ignoreExtensibleDataObject;
		ass_style = assemblyFormat;
		selector = surrogateSelector;
	}

	public NetDataContractSerializer(string rootName, string rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensibleDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		: this(context, maxItemsInObjectGraph, ignoreExtensibleDataObject, assemblyFormat, surrogateSelector)
	{
		FillDictionaryString(rootName, rootNamespace);
	}

	public NetDataContractSerializer(XmlDictionaryString rootName, XmlDictionaryString rootNamespace, StreamingContext context, int maxItemsInObjectGraph, bool ignoreExtensibleDataObject, FormatterAssemblyStyle assemblyFormat, ISurrogateSelector surrogateSelector)
		: this(context, maxItemsInObjectGraph, ignoreExtensibleDataObject, assemblyFormat, surrogateSelector)
	{
		if (rootName == null)
		{
			throw new ArgumentNullException("rootName");
		}
		if (rootNamespace == null)
		{
			throw new ArgumentNullException("rootNamespace");
		}
		root_name = rootName;
		root_ns = rootNamespace;
	}

	private void FillDictionaryString(string rootName, string rootNamespace)
	{
		if (rootName == null)
		{
			throw new ArgumentNullException("rootName");
		}
		if (rootNamespace == null)
		{
			throw new ArgumentNullException("rootNamespace");
		}
		XmlDictionary xmlDictionary = new XmlDictionary();
		root_name = xmlDictionary.Add(rootName);
		root_ns = xmlDictionary.Add(rootNamespace);
	}

	public object Deserialize(Stream stream)
	{
		return ReadObject(stream);
	}

	[System.MonoTODO]
	public override bool IsStartObject(XmlDictionaryReader reader)
	{
		throw new NotImplementedException();
	}

	public override object ReadObject(XmlDictionaryReader reader, bool readContentOnly)
	{
		throw new NotImplementedException();
	}

	public void Serialize(Stream stream, object graph)
	{
		using XmlWriter writer = XmlWriter.Create(stream);
		WriteObject(writer, graph);
	}

	[System.MonoTODO("support arrays; support Serializable; support SharedType; use DataContractSurrogate")]
	public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
	{
		throw new NotImplementedException();
	}

	public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
	{
		throw new NotImplementedException();
	}

	public override void WriteEndObject(XmlDictionaryWriter writer)
	{
		writer.WriteEndElement();
	}
}
