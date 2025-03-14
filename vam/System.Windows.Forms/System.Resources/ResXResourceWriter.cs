using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;

namespace System.Resources;

public class ResXResourceWriter : IDisposable, IResourceWriter
{
	private string filename;

	private Stream stream;

	private TextWriter textwriter;

	private XmlTextWriter writer;

	private bool written;

	private string base_path;

	public static readonly string BinSerializedObjectMimeType = "application/x-microsoft.net.object.binary.base64";

	public static readonly string ByteArraySerializedObjectMimeType = "application/x-microsoft.net.object.bytearray.base64";

	public static readonly string DefaultSerializedObjectMimeType = BinSerializedObjectMimeType;

	public static readonly string ResMimeType = "text/microsoft-resx";

	public static readonly string ResourceSchema = schema;

	public static readonly string SoapSerializedObjectMimeType = "application/x-microsoft.net.object.soap.base64";

	public static readonly string Version = "2.0";

	private static string schema = "\n  <xsd:schema id='root' xmlns='' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>\n    <xsd:element name='root' msdata:IsDataSet='true'>\n      <xsd:complexType>\n        <xsd:choice maxOccurs='unbounded'>\n          <xsd:element name='data'>\n            <xsd:complexType>\n              <xsd:sequence>\n                <xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />\n                <xsd:element name='comment' type='xsd:string' minOccurs='0' msdata:Ordinal='2' />\n              </xsd:sequence>\n              <xsd:attribute name='name' type='xsd:string' msdata:Ordinal='1' />\n              <xsd:attribute name='type' type='xsd:string' msdata:Ordinal='3' />\n              <xsd:attribute name='mimetype' type='xsd:string' msdata:Ordinal='4' />\n            </xsd:complexType>\n          </xsd:element>\n          <xsd:element name='resheader'>\n            <xsd:complexType>\n              <xsd:sequence>\n                <xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />\n              </xsd:sequence>\n              <xsd:attribute name='name' type='xsd:string' use='required' />\n            </xsd:complexType>\n          </xsd:element>\n        </xsd:choice>\n      </xsd:complexType>\n    </xsd:element>\n  </xsd:schema>\n".Replace("'", "\"");

	public string BasePath
	{
		get
		{
			return base_path;
		}
		set
		{
			base_path = value;
		}
	}

	public ResXResourceWriter(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (!stream.CanWrite)
		{
			throw new ArgumentException("stream is not writable.", "stream");
		}
		this.stream = stream;
	}

	public ResXResourceWriter(TextWriter textWriter)
	{
		if (textWriter == null)
		{
			throw new ArgumentNullException("textWriter");
		}
		textwriter = textWriter;
	}

	public ResXResourceWriter(string fileName)
	{
		if (fileName == null)
		{
			throw new ArgumentNullException("fileName");
		}
		filename = fileName;
	}

	~ResXResourceWriter()
	{
		Dispose(disposing: false);
	}

	private void InitWriter()
	{
		if (filename != null)
		{
			stream = File.OpenWrite(filename);
		}
		if (textwriter == null)
		{
			textwriter = new StreamWriter(stream, Encoding.UTF8);
		}
		writer = new XmlTextWriter(textwriter);
		writer.Formatting = Formatting.Indented;
		writer.WriteStartDocument();
		writer.WriteStartElement("root");
		writer.WriteRaw(schema);
		WriteHeader("resmimetype", "text/microsoft-resx");
		WriteHeader("version", "1.3");
		WriteHeader("reader", typeof(ResXResourceReader).AssemblyQualifiedName);
		WriteHeader("writer", typeof(ResXResourceWriter).AssemblyQualifiedName);
	}

	private void WriteHeader(string name, string value)
	{
		writer.WriteStartElement("resheader");
		writer.WriteAttributeString("name", name);
		writer.WriteStartElement("value");
		writer.WriteString(value);
		writer.WriteEndElement();
		writer.WriteEndElement();
	}

	private void WriteNiceBase64(byte[] value, int offset, int length)
	{
		string text = Convert.ToBase64String(value, offset, length);
		StringBuilder stringBuilder = new StringBuilder(text, text.Length + (text.Length + 160) / 80 * 3);
		int i = 0;
		int num = 80 + Environment.NewLine.Length + 1;
		string value2 = Environment.NewLine + "\t";
		for (; i < stringBuilder.Length; i += num)
		{
			stringBuilder.Insert(i, value2);
		}
		stringBuilder.Insert(stringBuilder.Length, Environment.NewLine);
		writer.WriteString(stringBuilder.ToString());
	}

	private void WriteBytes(string name, Type type, byte[] value, int offset, int length)
	{
		WriteBytes(name, type, value, offset, length, string.Empty);
	}

	private void WriteBytes(string name, Type type, byte[] value, int offset, int length, string comment)
	{
		writer.WriteStartElement("data");
		writer.WriteAttributeString("name", name);
		if (type != null)
		{
			writer.WriteAttributeString("type", type.AssemblyQualifiedName);
			if (type != typeof(byte[]))
			{
				writer.WriteAttributeString("mimetype", ByteArraySerializedObjectMimeType);
			}
			writer.WriteStartElement("value");
			WriteNiceBase64(value, offset, length);
		}
		else
		{
			writer.WriteAttributeString("mimetype", BinSerializedObjectMimeType);
			writer.WriteStartElement("value");
			writer.WriteBase64(value, offset, length);
		}
		writer.WriteEndElement();
		if (comment != null && !comment.Equals(string.Empty))
		{
			writer.WriteStartElement("comment");
			writer.WriteString(comment);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
	}

	private void WriteBytes(string name, Type type, byte[] value)
	{
		WriteBytes(name, type, value, 0, value.Length);
	}

	private void WriteString(string name, string value)
	{
		WriteString(name, value, null);
	}

	private void WriteString(string name, string value, Type type)
	{
		WriteString(name, value, type, string.Empty);
	}

	private void WriteString(string name, string value, Type type, string comment)
	{
		writer.WriteStartElement("data");
		writer.WriteAttributeString("name", name);
		if (type != null)
		{
			writer.WriteAttributeString("type", type.AssemblyQualifiedName);
		}
		writer.WriteStartElement("value");
		writer.WriteString(value);
		writer.WriteEndElement();
		if (comment != null && !comment.Equals(string.Empty))
		{
			writer.WriteStartElement("comment");
			writer.WriteString(comment);
			writer.WriteEndElement();
		}
		writer.WriteEndElement();
		writer.WriteWhitespace("\n  ");
	}

	public void AddResource(string name, byte[] value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		if (writer == null)
		{
			InitWriter();
		}
		WriteBytes(name, value.GetType(), value);
	}

	public void AddResource(string name, object value)
	{
		AddResource(name, value, string.Empty);
	}

	private void AddResource(string name, object value, string comment)
	{
		if (value is string)
		{
			AddResource(name, (string)value);
			return;
		}
		if (value is byte[])
		{
			AddResource(name, (byte[])value);
			return;
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (!value.GetType().IsSerializable)
		{
			throw new InvalidOperationException($"The element '{name}' of type '{value.GetType().Name}' is not serializable.");
		}
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		if (writer == null)
		{
			InitWriter();
		}
		TypeConverter converter = TypeDescriptor.GetConverter(value);
		if (converter != null && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
		{
			string value2 = converter.ConvertToInvariantString(value);
			WriteString(name, value2, value.GetType());
			return;
		}
		if (converter != null && converter.CanConvertTo(typeof(byte[])) && converter.CanConvertFrom(typeof(byte[])))
		{
			byte[] value3 = (byte[])converter.ConvertTo(value, typeof(byte[]));
			WriteBytes(name, value.GetType(), value3);
			return;
		}
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		try
		{
			binaryFormatter.Serialize(memoryStream, value);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(string.Concat("Cannot add a ", value.GetType(), "because it cannot be serialized: ", ex.Message));
		}
		WriteBytes(name, null, memoryStream.GetBuffer(), 0, (int)memoryStream.Length, comment);
		memoryStream.Close();
	}

	public void AddResource(string name, string value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		if (writer == null)
		{
			InitWriter();
		}
		WriteString(name, value);
	}

	[System.MonoTODO("Stub, not implemented")]
	public virtual void AddAlias(string aliasName, AssemblyName assemblyName)
	{
	}

	public void AddResource(ResXDataNode node)
	{
		AddResource(node.Name, node.Value, node.Comment);
	}

	public void AddMetadata(string name, string value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		if (writer == null)
		{
			InitWriter();
		}
		writer.WriteStartElement("metadata");
		writer.WriteAttributeString("name", name);
		writer.WriteAttributeString("xml:space", "preserve");
		writer.WriteElementString("value", value);
		writer.WriteEndElement();
	}

	public void AddMetadata(string name, byte[] value)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		if (writer == null)
		{
			InitWriter();
		}
		writer.WriteStartElement("metadata");
		writer.WriteAttributeString("name", name);
		writer.WriteAttributeString("type", value.GetType().AssemblyQualifiedName);
		writer.WriteStartElement("value");
		WriteNiceBase64(value, 0, value.Length);
		writer.WriteEndElement();
		writer.WriteEndElement();
	}

	public void AddMetadata(string name, object value)
	{
		if (value is string)
		{
			AddMetadata(name, (string)value);
			return;
		}
		if (value is byte[])
		{
			AddMetadata(name, (byte[])value);
			return;
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (!value.GetType().IsSerializable)
		{
			throw new InvalidOperationException($"The element '{name}' of type '{value.GetType().Name}' is not serializable.");
		}
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		if (writer == null)
		{
			InitWriter();
		}
		Type type = value.GetType();
		TypeConverter converter = TypeDescriptor.GetConverter(value);
		if (converter != null && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
		{
			string text = converter.ConvertToInvariantString(value);
			writer.WriteStartElement("metadata");
			writer.WriteAttributeString("name", name);
			if (type != null)
			{
				writer.WriteAttributeString("type", type.AssemblyQualifiedName);
			}
			writer.WriteStartElement("value");
			writer.WriteString(text);
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteWhitespace("\n  ");
			return;
		}
		if (converter != null && converter.CanConvertTo(typeof(byte[])) && converter.CanConvertFrom(typeof(byte[])))
		{
			byte[] array = (byte[])converter.ConvertTo(value, typeof(byte[]));
			writer.WriteStartElement("metadata");
			writer.WriteAttributeString("name", name);
			if (type != null)
			{
				writer.WriteAttributeString("type", type.AssemblyQualifiedName);
				writer.WriteAttributeString("mimetype", ByteArraySerializedObjectMimeType);
				writer.WriteStartElement("value");
				WriteNiceBase64(array, 0, array.Length);
			}
			else
			{
				writer.WriteAttributeString("mimetype", BinSerializedObjectMimeType);
				writer.WriteStartElement("value");
				writer.WriteBase64(array, 0, array.Length);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
			return;
		}
		MemoryStream memoryStream = new MemoryStream();
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		try
		{
			binaryFormatter.Serialize(memoryStream, value);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException(string.Concat("Cannot add a ", value.GetType(), "because it cannot be serialized: ", ex.Message));
		}
		writer.WriteStartElement("metadata");
		writer.WriteAttributeString("name", name);
		if (type != null)
		{
			writer.WriteAttributeString("type", type.AssemblyQualifiedName);
			writer.WriteAttributeString("mimetype", ByteArraySerializedObjectMimeType);
			writer.WriteStartElement("value");
			WriteNiceBase64(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
		}
		else
		{
			writer.WriteAttributeString("mimetype", BinSerializedObjectMimeType);
			writer.WriteStartElement("value");
			writer.WriteBase64(memoryStream.GetBuffer(), 0, memoryStream.GetBuffer().Length);
		}
		writer.WriteEndElement();
		writer.WriteEndElement();
		memoryStream.Close();
	}

	public void Close()
	{
		if (!written)
		{
			Generate();
		}
		if (writer != null)
		{
			writer.Close();
			stream = null;
			filename = null;
			textwriter = null;
		}
	}

	public virtual void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Generate()
	{
		if (written)
		{
			throw new InvalidOperationException("The resource is already generated.");
		}
		written = true;
		writer.WriteEndElement();
		writer.Flush();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Close();
		}
	}
}
