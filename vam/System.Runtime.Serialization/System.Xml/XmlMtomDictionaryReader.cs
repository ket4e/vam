using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Xml;

internal class XmlMtomDictionaryReader : XmlDictionaryReader
{
	private Stream stream;

	private Encoding encoding;

	private Encoding[] encodings;

	private ContentType content_type;

	private XmlDictionaryReaderQuotas quotas;

	private int max_buffer_size;

	private OnXmlDictionaryReaderClose on_close;

	private Dictionary<string, MimeEncodedStream> readers = new Dictionary<string, MimeEncodedStream>();

	private XmlReader xml_reader;

	private XmlReader initial_reader;

	private XmlReader eof_reader;

	private XmlReader part_reader;

	private int buffer_length;

	private byte[] buffer;

	private int peek_char;

	private ContentType current_content_type;

	private int content_index;

	private string current_content_id;

	private string current_content_encoding;

	private XmlReader Reader => part_reader ?? xml_reader;

	public override bool EOF => Reader == eof_reader;

	public override int AttributeCount => Reader.AttributeCount;

	public override string BaseURI => Reader.BaseURI;

	public override int Depth => Reader.Depth;

	public override bool HasValue => Reader.HasValue;

	public override bool IsEmptyElement => Reader.IsEmptyElement;

	public override string LocalName => Reader.LocalName;

	public override string NamespaceURI => Reader.NamespaceURI;

	public override XmlNameTable NameTable => Reader.NameTable;

	public override XmlNodeType NodeType => Reader.NodeType;

	public override string Prefix => Reader.Prefix;

	public override ReadState ReadState => Reader.ReadState;

	public override string Value => Reader.Value;

	public XmlMtomDictionaryReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas)
	{
		this.stream = stream;
		this.encoding = encoding;
		this.quotas = quotas;
		Initialize();
	}

	public XmlMtomDictionaryReader(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
	{
		this.stream = stream;
		this.encodings = encodings;
		content_type = ((contentType == null) ? null : CreateContentType(contentType));
		this.quotas = quotas;
		max_buffer_size = maxBufferSize;
		on_close = onClose;
		Initialize();
	}

	private void Initialize()
	{
		NameTable nameTable = new NameTable();
		initial_reader = new NonInteractiveStateXmlReader(string.Empty, nameTable, ReadState.Initial);
		eof_reader = new NonInteractiveStateXmlReader(string.Empty, nameTable, ReadState.EndOfFile);
		xml_reader = initial_reader;
	}

	private ContentType CreateContentType(string contentTypeString)
	{
		ContentType contentType = null;
		string[] array = contentTypeString.Split(';');
		foreach (string text in array)
		{
			string text2 = text.Trim();
			if (contentType == null)
			{
				contentType = new ContentType(text2);
				continue;
			}
			int num = text2.IndexOf('=');
			if (num < 0)
			{
				throw new XmlException("Invalid content type header");
			}
			string value = StripBraces(text2.Substring(num + 1));
			contentType.Parameters[text2.Substring(0, num)] = value;
		}
		return contentType;
	}

	public override void Close()
	{
		if (!EOF && on_close != null)
		{
			on_close(this);
		}
		xml_reader = eof_reader;
	}

	public override bool Read()
	{
		if (EOF)
		{
			return false;
		}
		if (Reader == initial_reader)
		{
			SetupPrimaryReader();
		}
		if (part_reader != null)
		{
			part_reader = null;
		}
		if (!Reader.Read())
		{
			xml_reader = eof_reader;
			return false;
		}
		if (Reader.LocalName == "Include" && Reader.NamespaceURI == "http://www.w3.org/2004/08/xop/include")
		{
			string attribute = Reader.GetAttribute("href");
			if (!attribute.StartsWith("cid:"))
			{
				throw new XmlException("Cannot resolve non-cid href attribute value in XOP Include element");
			}
			attribute = attribute.Substring(4);
			if (!readers.ContainsKey(attribute))
			{
				ReadToIdentifiedStream(attribute);
			}
			part_reader = new MultiPartedXmlReader(Reader, readers[attribute]);
		}
		return true;
	}

	private void SetupPrimaryReader()
	{
		ReadOptionalMimeHeaders();
		if (current_content_type != null)
		{
			content_type = current_content_type;
		}
		if (content_type == null)
		{
			throw new XmlException("Content-Type header for the MTOM message was not found");
		}
		if (content_type.Boundary == null)
		{
			throw new XmlException("Content-Type header for the MTOM message must contain 'boundary' parameter");
		}
		if (encoding == null && content_type.CharSet != null)
		{
			encoding = Encoding.GetEncoding(content_type.CharSet);
		}
		if (encoding == null && encodings == null)
		{
			throw new XmlException("Encoding specification is required either in the constructor argument or the content-type header");
		}
		string value = "--" + content_type.Boundary;
		string text;
		do
		{
			text = ReadAsciiLine().Trim();
			if (text == null)
			{
				return;
			}
		}
		while (text.Length == 0);
		if (!text.StartsWith(value, StringComparison.Ordinal))
		{
			throw new XmlException($"Unexpected boundary line was found. Expected boundary is '{content_type.Boundary}' but it was '{text}'");
		}
		string text2 = content_type.Parameters["start"];
		ReadToIdentifiedStream(text2);
		xml_reader = XmlReader.Create(readers[text2].CreateTextReader());
	}

	private void ReadToIdentifiedStream(string id)
	{
		do
		{
			if (!ReadNextStream())
			{
				throw new XmlException($"The stream '{id}' did not appear");
			}
		}
		while (!(current_content_id == id) && id != null);
	}

	private bool ReadNextStream()
	{
		ReadOptionalMimeHeaders();
		string value = "--" + content_type.Boundary;
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			string text = ReadAsciiLine();
			if (text == null && stringBuilder.Length == 0)
			{
				return false;
			}
			if (text == null || text.StartsWith(value, StringComparison.Ordinal))
			{
				break;
			}
			stringBuilder.Append(text);
		}
		readers.Add(current_content_id, new MimeEncodedStream(current_content_id, current_content_encoding, stringBuilder.ToString()));
		return true;
	}

	private void ReadOptionalMimeHeaders()
	{
		peek_char = stream.ReadByte();
		if (peek_char != 45)
		{
			ReadMimeHeaders();
		}
	}

	private string ReadAllHeaderLines()
	{
		string text = string.Empty;
		while (true)
		{
			string text2 = ReadAsciiLine();
			if (text2.Length == 0)
			{
				break;
			}
			text2 = text2.TrimEnd();
			text += text2;
			if (text2[text2.Length - 1] != ';')
			{
				text += '\n';
			}
		}
		return text;
	}

	private void ReadMimeHeaders()
	{
		string[] array = ReadAllHeaderLines().Split('\n');
		foreach (string text in array)
		{
			if (text.Length != 0)
			{
				int num = text.IndexOf(':');
				if (num < 0)
				{
					throw new XmlException($"Unexpected header string: {text}");
				}
				string contentTypeString = StripBraces(text.Substring(num + 1).Trim());
				switch (text.Substring(0, num).ToLower())
				{
				case "content-type":
					current_content_type = CreateContentType(contentTypeString);
					break;
				case "content-id":
					current_content_id = contentTypeString;
					break;
				case "content-transfer-encoding":
					current_content_encoding = contentTypeString;
					break;
				}
			}
		}
	}

	private string StripBraces(string s)
	{
		if (s.Length >= 2 && s[0] == '"' && s[s.Length - 1] == '"')
		{
			s = s.Substring(1, s.Length - 2);
		}
		if (s.Length >= 2 && s[0] == '<' && s[s.Length - 1] == '>')
		{
			s = s.Substring(1, s.Length - 2);
		}
		return s;
	}

	private string ReadAsciiLine()
	{
		if (buffer == null)
		{
			buffer = new byte[1024];
		}
		int num = 0;
		int num2 = peek_char;
		bool flag = num2 >= 0;
		peek_char = -1;
		while (true)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				num2 = stream.ReadByte();
			}
			if (num2 < 0)
			{
				if (num > 0)
				{
					throw new XmlException("The stream ends without end of line");
				}
				return null;
			}
			if (num2 == 13)
			{
				num2 = stream.ReadByte();
				if (num2 < 0)
				{
					buffer[num++] = 13;
					break;
				}
				if (num2 == 10)
				{
					break;
				}
				buffer[num++] = 13;
				flag = true;
			}
			else
			{
				buffer[num++] = (byte)num2;
			}
			if (num == buffer.Length)
			{
				byte[] destinationArray = new byte[buffer.Length << 1];
				Array.Copy(buffer, 0, destinationArray, 0, buffer.Length);
				buffer = destinationArray;
			}
		}
		return Encoding.ASCII.GetString(buffer, 0, num);
	}

	public override bool MoveToElement()
	{
		return Reader.MoveToElement();
	}

	public override string GetAttribute(int index)
	{
		return Reader.GetAttribute(index);
	}

	public override string GetAttribute(string name)
	{
		return Reader.GetAttribute(name);
	}

	public override string GetAttribute(string localName, string namespaceURI)
	{
		return Reader.GetAttribute(localName, namespaceURI);
	}

	public override void MoveToAttribute(int index)
	{
		Reader.MoveToAttribute(index);
	}

	public override bool MoveToAttribute(string name)
	{
		return Reader.MoveToAttribute(name);
	}

	public override bool MoveToAttribute(string localName, string namespaceURI)
	{
		return Reader.MoveToAttribute(localName, namespaceURI);
	}

	public override bool MoveToFirstAttribute()
	{
		return Reader.MoveToFirstAttribute();
	}

	public override bool MoveToNextAttribute()
	{
		return Reader.MoveToNextAttribute();
	}

	public override string LookupNamespace(string prefix)
	{
		return Reader.LookupNamespace(prefix);
	}

	public override bool ReadAttributeValue()
	{
		return Reader.ReadAttributeValue();
	}

	public override void ResolveEntity()
	{
		Reader.ResolveEntity();
	}
}
