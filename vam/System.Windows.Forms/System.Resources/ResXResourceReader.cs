using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace System.Resources;

public class ResXResourceReader : IDisposable, IResourceReader, IEnumerable
{
	private class ResXHeader
	{
		private string resMimeType;

		private string reader;

		private string version;

		private string writer;

		public string ResMimeType
		{
			get
			{
				return resMimeType;
			}
			set
			{
				resMimeType = value;
			}
		}

		public string Reader
		{
			get
			{
				return reader;
			}
			set
			{
				reader = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public string Writer
		{
			get
			{
				return writer;
			}
			set
			{
				writer = value;
			}
		}

		public bool IsValid
		{
			get
			{
				if (string.Compare(ResMimeType, ResXResourceWriter.ResMimeType) != 0)
				{
					return false;
				}
				if (Reader == null || Writer == null)
				{
					return false;
				}
				string text = Reader.Split(',')[0].Trim();
				if (text != typeof(ResXResourceReader).FullName)
				{
					return false;
				}
				string text2 = Writer.Split(',')[0].Trim();
				if (text2 != typeof(ResXResourceWriter).FullName)
				{
					return false;
				}
				return true;
			}
		}

		public void Verify()
		{
			if (!IsValid)
			{
				throw new ArgumentException("Invalid ResX input.  Could not find valid \"resheader\" tags for the ResX reader & writer type names.");
			}
		}
	}

	private string fileName;

	private Stream stream;

	private TextReader reader;

	private Hashtable hasht;

	private ITypeResolutionService typeresolver;

	private XmlTextReader xmlReader;

	private string basepath;

	private bool useResXDataNodes;

	private AssemblyName[] assemblyNames;

	private Hashtable hashtm;

	public string BasePath
	{
		get
		{
			return basepath;
		}
		set
		{
			basepath = value;
		}
	}

	public bool UseResXDataNodes
	{
		get
		{
			return useResXDataNodes;
		}
		set
		{
			if (xmlReader != null)
			{
				throw new InvalidOperationException();
			}
			useResXDataNodes = value;
		}
	}

	public ResXResourceReader(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (!stream.CanRead)
		{
			throw new ArgumentException("Stream was not readable.");
		}
		this.stream = stream;
	}

	public ResXResourceReader(Stream stream, ITypeResolutionService typeResolver)
		: this(stream)
	{
		typeresolver = typeResolver;
	}

	public ResXResourceReader(string fileName)
	{
		this.fileName = fileName;
	}

	public ResXResourceReader(string fileName, ITypeResolutionService typeResolver)
		: this(fileName)
	{
		typeresolver = typeResolver;
	}

	public ResXResourceReader(TextReader reader)
	{
		this.reader = reader;
	}

	public ResXResourceReader(TextReader reader, ITypeResolutionService typeResolver)
		: this(reader)
	{
		typeresolver = typeResolver;
	}

	public ResXResourceReader(Stream stream, AssemblyName[] assemblyNames)
		: this(stream)
	{
		this.assemblyNames = assemblyNames;
	}

	public ResXResourceReader(string fileName, AssemblyName[] assemblyNames)
		: this(fileName)
	{
		this.assemblyNames = assemblyNames;
	}

	public ResXResourceReader(TextReader reader, AssemblyName[] assemblyNames)
		: this(reader)
	{
		this.assemblyNames = assemblyNames;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IResourceReader)this).GetEnumerator();
	}

	void IDisposable.Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	~ResXResourceReader()
	{
		Dispose(disposing: false);
	}

	private void LoadData()
	{
		hasht = new Hashtable();
		hashtm = new Hashtable();
		if (fileName != null)
		{
			stream = File.OpenRead(fileName);
		}
		try
		{
			xmlReader = null;
			if (stream != null)
			{
				xmlReader = new XmlTextReader(stream);
			}
			else if (reader != null)
			{
				xmlReader = new XmlTextReader(reader);
			}
			if (xmlReader == null)
			{
				throw new InvalidOperationException("ResourceReader is closed.");
			}
			xmlReader.WhitespaceHandling = WhitespaceHandling.None;
			ResXHeader resXHeader = new ResXHeader();
			try
			{
				while (xmlReader.Read())
				{
					if (xmlReader.NodeType == XmlNodeType.Element)
					{
						switch (xmlReader.LocalName)
						{
						case "resheader":
							ParseHeaderNode(resXHeader);
							break;
						case "data":
							ParseDataNode(meta: false);
							break;
						case "metadata":
							ParseDataNode(meta: true);
							break;
						}
					}
				}
			}
			catch (XmlException innerException)
			{
				throw new ArgumentException("Invalid ResX input.", innerException);
			}
			catch (Exception ex)
			{
				XmlException innerException2 = new XmlException(ex.Message, ex, xmlReader.LineNumber, xmlReader.LinePosition);
				throw new ArgumentException("Invalid ResX input.", innerException2);
			}
			resXHeader.Verify();
		}
		finally
		{
			if (fileName != null)
			{
				stream.Close();
				stream = null;
			}
			xmlReader = null;
		}
	}

	private void ParseHeaderNode(ResXHeader header)
	{
		string attribute = GetAttribute("name");
		if (attribute != null)
		{
			if (string.Compare(attribute, "resmimetype", ignoreCase: true) == 0)
			{
				header.ResMimeType = GetHeaderValue();
			}
			else if (string.Compare(attribute, "reader", ignoreCase: true) == 0)
			{
				header.Reader = GetHeaderValue();
			}
			else if (string.Compare(attribute, "version", ignoreCase: true) == 0)
			{
				header.Version = GetHeaderValue();
			}
			else if (string.Compare(attribute, "writer", ignoreCase: true) == 0)
			{
				header.Writer = GetHeaderValue();
			}
		}
	}

	private string GetHeaderValue()
	{
		string text = null;
		xmlReader.ReadStartElement();
		if (xmlReader.NodeType == XmlNodeType.Element)
		{
			return xmlReader.ReadElementString();
		}
		return xmlReader.Value.Trim();
	}

	private string GetAttribute(string name)
	{
		if (!xmlReader.HasAttributes)
		{
			return null;
		}
		for (int i = 0; i < xmlReader.AttributeCount; i++)
		{
			xmlReader.MoveToAttribute(i);
			if (string.Compare(xmlReader.Name, name, ignoreCase: true) == 0)
			{
				string value = xmlReader.Value;
				xmlReader.MoveToElement();
				return value;
			}
		}
		xmlReader.MoveToElement();
		return null;
	}

	private string GetDataValue(bool meta, out string comment)
	{
		string result = null;
		comment = null;
		while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !(xmlReader.LocalName == ((!meta) ? "data" : "metadata"))))
		{
			if (xmlReader.NodeType == XmlNodeType.Element)
			{
				if (xmlReader.Name.Equals("value"))
				{
					xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
					result = xmlReader.ReadString();
					xmlReader.WhitespaceHandling = WhitespaceHandling.None;
				}
				else if (xmlReader.Name.Equals("comment"))
				{
					xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
					comment = xmlReader.ReadString();
					xmlReader.WhitespaceHandling = WhitespaceHandling.None;
					if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.LocalName == ((!meta) ? "data" : "metadata"))
					{
						break;
					}
				}
			}
			else
			{
				result = xmlReader.Value.Trim();
			}
		}
		return result;
	}

	private void ParseDataNode(bool meta)
	{
		Hashtable hashtable = ((!meta || useResXDataNodes) ? hasht : hashtm);
		Point position = new Point(xmlReader.LineNumber, xmlReader.LinePosition);
		string attribute = GetAttribute("name");
		string attribute2 = GetAttribute("type");
		string attribute3 = GetAttribute("mimetype");
		Type type = ((attribute2 != null) ? ResolveType(attribute2) : null);
		if (attribute2 != null && type == null)
		{
			throw new ArgumentException($"The type '{attribute2}' of the element '{attribute}' could not be resolved.");
		}
		if (type == typeof(ResXNullRef))
		{
			if (useResXDataNodes)
			{
				hashtable[attribute] = new ResXDataNode(attribute, null, position);
			}
			else
			{
				hashtable[attribute] = null;
			}
			return;
		}
		string comment = null;
		string dataValue = GetDataValue(meta, out comment);
		object obj = null;
		if (attribute3 != null && attribute3.Length > 0)
		{
			if (attribute3 == ResXResourceWriter.BinSerializedObjectMimeType)
			{
				byte[] buffer = Convert.FromBase64String(dataValue);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				using MemoryStream serializationStream = new MemoryStream(buffer);
				obj = binaryFormatter.Deserialize(serializationStream);
			}
			else if (attribute3 == ResXResourceWriter.ByteArraySerializedObjectMimeType && type != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if (converter.CanConvertFrom(typeof(byte[])))
				{
					obj = converter.ConvertFrom(Convert.FromBase64String(dataValue));
				}
			}
		}
		else if (type != null)
		{
			if (type == typeof(byte[]))
			{
				obj = Convert.FromBase64String(dataValue);
			}
			else
			{
				TypeConverter converter2 = TypeDescriptor.GetConverter(type);
				if (converter2.CanConvertFrom(typeof(string)))
				{
					if (BasePath != null && type == typeof(ResXFileRef))
					{
						string[] array = ResXFileRef.Parse(dataValue);
						array[0] = Path.Combine(BasePath, array[0]);
						obj = converter2.ConvertFromInvariantString(string.Join(";", array));
					}
					else
					{
						obj = converter2.ConvertFromInvariantString(dataValue);
					}
				}
			}
		}
		else
		{
			obj = dataValue;
		}
		if (attribute == null)
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Could not find a name for a resource. The resource value was '{0}'.", obj));
		}
		if (useResXDataNodes)
		{
			ResXDataNode resXDataNode = new ResXDataNode(attribute, obj, position);
			resXDataNode.Comment = comment;
			hashtable[attribute] = resXDataNode;
		}
		else
		{
			hashtable[attribute] = obj;
		}
	}

	private Type ResolveType(string type)
	{
		if (typeresolver != null)
		{
			return typeresolver.GetType(type);
		}
		if (assemblyNames != null)
		{
			AssemblyName[] array = assemblyNames;
			foreach (AssemblyName assemblyRef in array)
			{
				Assembly assembly = Assembly.Load(assemblyRef);
				Type type2 = assembly.GetType(type, throwOnError: false);
				if (type2 != null)
				{
					return type2;
				}
			}
		}
		return Type.GetType(type);
	}

	public void Close()
	{
		if (reader != null)
		{
			reader.Close();
			reader = null;
		}
	}

	public IDictionaryEnumerator GetEnumerator()
	{
		if (hasht == null)
		{
			LoadData();
		}
		return hasht.GetEnumerator();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			Close();
		}
	}

	public static ResXResourceReader FromFileContents(string fileContents)
	{
		return new ResXResourceReader(new StringReader(fileContents));
	}

	public static ResXResourceReader FromFileContents(string fileContents, ITypeResolutionService typeResolver)
	{
		return new ResXResourceReader(new StringReader(fileContents), typeResolver);
	}

	public static ResXResourceReader FromFileContents(string fileContents, AssemblyName[] assemblyNames)
	{
		return new ResXResourceReader(new StringReader(fileContents), assemblyNames);
	}

	public IDictionaryEnumerator GetMetadataEnumerator()
	{
		if (hashtm == null)
		{
			LoadData();
		}
		return hashtm.GetEnumerator();
	}
}
