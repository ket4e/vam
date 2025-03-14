using System.IO;
using System.Text;

namespace System.Xml;

public abstract class XmlDictionaryWriter : XmlWriter
{
	private static readonly Encoding utf8_unmarked = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

	private int depth;

	internal int Depth
	{
		get
		{
			return depth;
		}
		set
		{
			depth = value;
		}
	}

	public virtual bool CanCanonicalize => false;

	public static XmlDictionaryWriter CreateBinaryWriter(Stream stream)
	{
		return CreateBinaryWriter(stream, null, null, ownsStream: false);
	}

	public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary)
	{
		return CreateBinaryWriter(stream, dictionary, null, ownsStream: false);
	}

	public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session)
	{
		return CreateBinaryWriter(stream, dictionary, session, ownsStream: false);
	}

	public static XmlDictionaryWriter CreateBinaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
	{
		return new XmlBinaryDictionaryWriter(stream, dictionary, session, ownsStream);
	}

	public static XmlDictionaryWriter CreateDictionaryWriter(XmlWriter writer)
	{
		return new XmlSimpleDictionaryWriter(writer);
	}

	public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo)
	{
		return CreateMtomWriter(stream, encoding, maxSizeInBytes, startInfo, string.Concat(Guid.NewGuid(), "id=1"), "http://tempuri.org/0/" + DateTime.Now.Ticks, writeMessageHeaders: true, ownsStream: false);
	}

	public static XmlDictionaryWriter CreateMtomWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
	{
		return new XmlMtomDictionaryWriter(stream, encoding, maxSizeInBytes, startInfo, boundary, startUri, writeMessageHeaders, ownsStream);
	}

	public static XmlDictionaryWriter CreateTextWriter(Stream stream)
	{
		return CreateTextWriter(stream, Encoding.UTF8);
	}

	public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding)
	{
		return CreateTextWriter(stream, encoding, ownsStream: false);
	}

	public static XmlDictionaryWriter CreateTextWriter(Stream stream, Encoding encoding, bool ownsStream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (encoding == null)
		{
			throw new ArgumentNullException("encoding");
		}
		int codePage = encoding.CodePage;
		if (codePage == 1200 || codePage == 1201 || codePage == 65001)
		{
			encoding = utf8_unmarked;
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Encoding = encoding;
			xmlWriterSettings.CloseOutput = ownsStream;
			xmlWriterSettings.OmitXmlDeclaration = true;
			return CreateDictionaryWriter(XmlWriter.Create(stream, xmlWriterSettings));
		}
		throw new XmlException($"XML declaration is required for encoding code page {encoding.CodePage} but this XmlWriter does not support XML declaration.");
	}

	public virtual void EndCanonicalization()
	{
		throw new NotSupportedException();
	}

	public virtual void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
	{
		throw new NotSupportedException();
	}

	public void WriteAttributeString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
	{
		WriteAttributeString(null, localName, namespaceUri, value);
	}

	public void WriteAttributeString(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
	{
		WriteStartAttribute(prefix, localName, namespaceUri);
		WriteString(value);
		WriteEndAttribute();
	}

	public void WriteElementString(XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
	{
		WriteElementString(null, localName, namespaceUri, value);
	}

	public void WriteElementString(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, string value)
	{
		WriteStartElement(prefix, localName, namespaceUri);
		WriteString(value);
		WriteEndElement();
	}

	public virtual void WriteNode(XmlDictionaryReader reader, bool defattr)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		switch (reader.NodeType)
		{
		case XmlNodeType.Element:
		{
			if (reader.TryGetLocalNameAsDictionaryString(out var localName) && reader.TryGetLocalNameAsDictionaryString(out var localName2))
			{
				WriteStartElement(reader.Prefix, localName, localName2);
			}
			else
			{
				WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
			}
			if (reader.HasAttributes)
			{
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					WriteAttribute(reader, defattr);
				}
				reader.MoveToElement();
			}
			if (reader.IsEmptyElement)
			{
				WriteEndElement();
			}
			else
			{
				int num = reader.Depth;
				reader.Read();
				if (reader.NodeType != XmlNodeType.EndElement)
				{
					do
					{
						WriteNode(reader, defattr);
					}
					while (num < reader.Depth);
				}
				WriteFullEndElement();
			}
			reader.Read();
			break;
		}
		case XmlNodeType.Attribute:
		case XmlNodeType.Text:
			WriteTextNode(reader, defattr);
			break;
		default:
			base.WriteNode(reader, defattr);
			break;
		}
	}

	private void WriteAttribute(XmlDictionaryReader reader, bool defattr)
	{
		if (!defattr && reader.IsDefault)
		{
			return;
		}
		if (reader.TryGetLocalNameAsDictionaryString(out var localName) && reader.TryGetLocalNameAsDictionaryString(out var localName2))
		{
			WriteStartAttribute(reader.Prefix, localName, localName2);
		}
		else
		{
			WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
		}
		while (reader.ReadAttributeValue())
		{
			switch (reader.NodeType)
			{
			case XmlNodeType.Text:
				WriteTextNode(reader, isAttribute: true);
				break;
			case XmlNodeType.EntityReference:
				WriteEntityRef(reader.Name);
				break;
			}
		}
		WriteEndAttribute();
	}

	public override void WriteNode(XmlReader reader, bool defattr)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		if (reader is XmlDictionaryReader reader2)
		{
			WriteNode(reader2, defattr);
		}
		else
		{
			base.WriteNode(reader, defattr);
		}
	}

	public virtual void WriteQualifiedName(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		WriteQualifiedName(localName.Value, namespaceUri.Value);
	}

	public void WriteStartAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		WriteStartAttribute(localName.Value, namespaceUri.Value);
	}

	public virtual void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		WriteStartAttribute(prefix, localName.Value, namespaceUri.Value);
	}

	public void WriteStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		WriteStartElement(null, localName, namespaceUri);
	}

	public virtual void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentException("localName must not be null.", "localName");
		}
		WriteStartElement(prefix, localName.Value, namespaceUri?.Value);
	}

	public virtual void WriteString(XmlDictionaryString value)
	{
		WriteString(value.Value);
	}

	protected virtual void WriteTextNode(XmlDictionaryReader reader, bool isAttribute)
	{
		WriteString(reader.Value);
		if (!isAttribute)
		{
			reader.Read();
		}
	}

	public virtual void WriteValue(Guid guid)
	{
		WriteString(guid.ToString());
	}

	public virtual void WriteValue(IStreamProvider value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		Stream stream = value.GetStream();
		byte[] array = new byte[Math.Min(2048L, (!stream.CanSeek) ? 2048 : stream.Length)];
		int count;
		while ((count = stream.Read(array, 0, array.Length)) > 0)
		{
			WriteBase64(array, 0, count);
		}
		value.ReleaseStream(stream);
	}

	public virtual void WriteValue(TimeSpan duration)
	{
		WriteString(XmlConvert.ToString(duration));
	}

	public virtual void WriteValue(UniqueId id)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		WriteString(id.ToString());
	}

	public virtual void WriteValue(XmlDictionaryString value)
	{
		WriteValue(value.Value);
	}

	public virtual void WriteXmlAttribute(string localName, string value)
	{
		WriteAttributeString("xml", localName, "http://www.w3.org/XML/1998/namespace", value);
	}

	public virtual void WriteXmlAttribute(XmlDictionaryString localName, XmlDictionaryString value)
	{
		WriteXmlAttribute(localName.Value, value.Value);
	}

	public virtual void WriteXmlnsAttribute(string prefix, string namespaceUri)
	{
		if (prefix == null)
		{
			prefix = "d" + Depth + "p1";
		}
		if (prefix == string.Empty)
		{
			WriteAttributeString("xmlns", namespaceUri);
		}
		else
		{
			WriteAttributeString("xmlns", prefix, "http://www.w3.org/2000/xmlns/", namespaceUri);
		}
	}

	public virtual void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
	{
		WriteXmlnsAttribute(prefix, namespaceUri.Value);
	}

	private void CheckWriteArrayArguments(Array array, int offset, int length)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (offset < 0)
		{
			throw new ArgumentOutOfRangeException("offset is negative");
		}
		if (offset > array.Length)
		{
			throw new ArgumentOutOfRangeException("offset exceeds the length of the destination array");
		}
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length is negative");
		}
		if (length > array.Length - offset)
		{
			throw new ArgumentOutOfRangeException("length + offset exceeds the length of the destination array");
		}
	}

	private void CheckDictionaryStringArgs(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}

	public virtual void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		WriteArray(prefix, localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			WriteStartElement(prefix, localName, namespaceUri);
			WriteValue(array[offset + i]);
			WriteEndElement();
		}
	}
}
