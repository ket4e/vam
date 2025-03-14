using System.IO;
using System.Xml;

namespace System.Runtime.Serialization;

public abstract class XmlObjectSerializer
{
	private IDataContractSurrogate surrogate;

	private SerializationBinder binder;

	private ISurrogateSelector selector;

	private int max_items = 65536;

	public virtual bool IsStartObject(XmlReader reader)
	{
		return IsStartObject(XmlDictionaryReader.CreateDictionaryReader(reader));
	}

	public abstract bool IsStartObject(XmlDictionaryReader reader);

	public virtual object ReadObject(Stream stream)
	{
		return ReadObject(XmlReader.Create(stream));
	}

	public virtual object ReadObject(XmlReader reader)
	{
		return ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader));
	}

	public virtual object ReadObject(XmlDictionaryReader reader)
	{
		return ReadObject(reader, readContentOnly: true);
	}

	public virtual object ReadObject(XmlReader reader, bool readContentOnly)
	{
		return ReadObject(XmlDictionaryReader.CreateDictionaryReader(reader), readContentOnly);
	}

	[System.MonoTODO]
	public abstract object ReadObject(XmlDictionaryReader reader, bool readContentOnly);

	public virtual void WriteObject(Stream stream, object graph)
	{
		using XmlWriter writer = XmlDictionaryWriter.CreateTextWriter(stream);
		WriteObject(writer, graph);
	}

	public virtual void WriteObject(XmlWriter writer, object graph)
	{
		WriteObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
	}

	public virtual void WriteStartObject(XmlWriter writer, object graph)
	{
		WriteStartObject(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
	}

	public virtual void WriteObject(XmlDictionaryWriter writer, object graph)
	{
		WriteStartObject(writer, graph);
		WriteObjectContent(writer, graph);
		WriteEndObject(writer);
	}

	public abstract void WriteStartObject(XmlDictionaryWriter writer, object graph);

	public virtual void WriteObjectContent(XmlWriter writer, object graph)
	{
		WriteObjectContent(XmlDictionaryWriter.CreateDictionaryWriter(writer), graph);
	}

	public abstract void WriteObjectContent(XmlDictionaryWriter writer, object graph);

	public virtual void WriteEndObject(XmlWriter writer)
	{
		WriteEndObject(XmlDictionaryWriter.CreateDictionaryWriter(writer));
	}

	public abstract void WriteEndObject(XmlDictionaryWriter writer);
}
