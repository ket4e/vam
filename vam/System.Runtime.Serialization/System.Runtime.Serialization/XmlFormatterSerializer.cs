using System.Collections;
using System.Xml;

namespace System.Runtime.Serialization;

internal class XmlFormatterSerializer
{
	private XmlDictionaryWriter writer;

	private object graph;

	private KnownTypeCollection types;

	private bool save_id;

	private bool ignore_unknown;

	private IDataContractSurrogate surrogate;

	private int max_items;

	private ArrayList objects = new ArrayList();

	private Hashtable references = new Hashtable();

	public ArrayList SerializingObjects => objects;

	public IDictionary References => references;

	public XmlDictionaryWriter Writer => writer;

	public XmlFormatterSerializer(XmlDictionaryWriter writer, KnownTypeCollection types, bool ignoreUnknown, int maxItems, string root_ns)
	{
		this.writer = writer;
		this.types = types;
		ignore_unknown = ignoreUnknown;
		max_items = maxItems;
	}

	public static void Serialize(XmlDictionaryWriter writer, object graph, KnownTypeCollection types, bool ignoreUnknown, int maxItems, string root_ns)
	{
		new XmlFormatterSerializer(writer, types, ignoreUnknown, maxItems, root_ns).Serialize(graph?.GetType(), graph);
	}

	public void Serialize(Type type, object graph)
	{
		if (graph == null)
		{
			writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			return;
		}
		Type type2 = graph.GetType();
		SerializationMap serializationMap = types.FindUserMap(type2);
		if (serializationMap == null)
		{
			type2 = types.GetSerializedType(type2);
			serializationMap = types.FindUserMap(type2);
		}
		if (serializationMap == null)
		{
			types.Add(type2);
			serializationMap = types.FindUserMap(type2);
		}
		if (type2 != type && (serializationMap == null || serializationMap.OutputXsiType))
		{
			XmlQualifiedName xmlName = types.GetXmlName(type2);
			string localName = xmlName.Name;
			string text = xmlName.Namespace;
			if (xmlName == XmlQualifiedName.Empty)
			{
				localName = XmlConvert.EncodeLocalName(type2.Name);
				text = "http://schemas.datacontract.org/2004/07/" + type2.Namespace;
			}
			else if (xmlName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				text = "http://www.w3.org/2001/XMLSchema";
			}
			if (writer.LookupPrefix(text) == null)
			{
				writer.WriteXmlnsAttribute(null, text);
			}
			writer.WriteStartAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
			writer.WriteQualifiedName(localName, text);
			writer.WriteEndAttribute();
		}
		XmlQualifiedName predefinedTypeName = KnownTypeCollection.GetPredefinedTypeName(type2);
		if (predefinedTypeName != XmlQualifiedName.Empty)
		{
			SerializePrimitive(type, graph, predefinedTypeName);
		}
		else
		{
			serializationMap.Serialize(graph, this);
		}
	}

	public void SerializePrimitive(Type type, object graph, XmlQualifiedName qname)
	{
		writer.WriteString(KnownTypeCollection.PredefinedTypeObjectToString(graph));
	}

	public void WriteStartElement(string rootName, string rootNamespace, string currentNamespace)
	{
		writer.WriteStartElement(rootName, rootNamespace);
		if (!string.IsNullOrEmpty(currentNamespace) && currentNamespace != rootNamespace)
		{
			writer.WriteXmlnsAttribute(null, currentNamespace);
		}
	}

	public void WriteEndElement()
	{
		writer.WriteEndElement();
	}
}
