using System.IO;
using System.Net.Mime;
using System.Text;

namespace System.Xml;

internal class XmlMtomDictionaryWriter : XmlDictionaryWriter
{
	private TextWriter writer;

	private XmlWriterSettings xml_writer_settings;

	private Encoding encoding;

	private int max_bytes;

	private bool write_headers;

	private bool owns_stream;

	private ContentType content_type;

	private XmlWriter w;

	private int depth;

	private int section_count;

	public override WriteState WriteState => w.WriteState;

	public override string XmlLang => w.XmlLang;

	public override XmlSpace XmlSpace => w.XmlSpace;

	public XmlMtomDictionaryWriter(Stream stream, Encoding encoding, int maxSizeInBytes, string startInfo, string boundary, string startUri, bool writeMessageHeaders, bool ownsStream)
	{
		writer = new StreamWriter(stream, encoding);
		max_bytes = maxSizeInBytes;
		write_headers = writeMessageHeaders;
		owns_stream = ownsStream;
		xml_writer_settings = new XmlWriterSettings
		{
			Encoding = encoding,
			OmitXmlDeclaration = true
		};
		ContentType contentType = new ContentType("multipart/related");
		contentType.Parameters["type"] = "application/xop+xml";
		contentType.Boundary = boundary;
		contentType.Parameters["start"] = "<" + startUri + ">";
		contentType.Parameters["start-info"] = startInfo;
		content_type = contentType;
	}

	private XmlWriter CreateWriter()
	{
		return XmlWriter.Create(writer, xml_writer_settings);
	}

	public override void Close()
	{
		w.Close();
		if (owns_stream)
		{
			writer.Close();
		}
	}

	public override void Flush()
	{
		w.Flush();
	}

	public override string LookupPrefix(string namespaceUri)
	{
		return w.LookupPrefix(namespaceUri);
	}

	public override void WriteBase64(byte[] bytes, int start, int length)
	{
		CheckState();
		w.WriteBase64(bytes, start, length);
	}

	public override void WriteCData(string text)
	{
		CheckState();
		w.WriteCData(text);
	}

	public override void WriteCharEntity(char c)
	{
		CheckState();
		w.WriteCharEntity(c);
	}

	public override void WriteChars(char[] buffer, int index, int count)
	{
		CheckState();
		w.WriteChars(buffer, index, count);
	}

	public override void WriteComment(string comment)
	{
		CheckState();
		w.WriteComment(comment);
	}

	public override void WriteDocType(string name, string pubid, string sysid, string intSubset)
	{
		throw new NotSupportedException();
	}

	public override void WriteEndAttribute()
	{
		w.WriteEndAttribute();
	}

	public override void WriteEndDocument()
	{
		w.WriteEndDocument();
	}

	public override void WriteEndElement()
	{
		w.WriteEndElement();
		if (--depth == 0)
		{
			WriteEndOfMimeSection();
		}
	}

	public override void WriteEntityRef(string name)
	{
		w.WriteEntityRef(name);
	}

	public override void WriteFullEndElement()
	{
		w.WriteFullEndElement();
		if (--depth == 0)
		{
			WriteEndOfMimeSection();
		}
	}

	public override void WriteProcessingInstruction(string name, string data)
	{
		throw new NotSupportedException();
	}

	public override void WriteRaw(string raw)
	{
		CheckState();
		w.WriteRaw(raw);
	}

	public override void WriteRaw(char[] chars, int index, int count)
	{
		CheckState();
		w.WriteRaw(chars, index, count);
	}

	public override void WriteStartAttribute(string prefix, string localName, string namespaceURI)
	{
		CheckState();
		w.WriteStartAttribute(prefix, localName, namespaceURI);
	}

	public override void WriteStartDocument()
	{
		CheckState();
		w.WriteStartDocument();
	}

	public override void WriteStartDocument(bool standalone)
	{
		CheckState();
		w.WriteStartDocument(standalone);
	}

	public override void WriteStartElement(string prefix, string localName, string namespaceURI)
	{
		CheckState();
		if (depth == 0)
		{
			WriteStartOfMimeSection();
		}
		w.WriteStartElement(prefix, localName, namespaceURI);
		depth++;
	}

	public override void WriteString(string text)
	{
		CheckState();
		int num = 0;
		while (true)
		{
			int num2 = text.IndexOf('\r', num);
			if (num2 < 0)
			{
				break;
			}
			w.WriteString(text.Substring(num, num2 - num));
			WriteCharEntity('\r');
			num = num2 + 1;
		}
		w.WriteString(text.Substring(num));
	}

	public override void WriteSurrogateCharEntity(char low, char high)
	{
		CheckState();
		w.WriteSurrogateCharEntity(low, high);
	}

	public override void WriteWhitespace(string text)
	{
		CheckState();
		w.WriteWhitespace(text);
	}

	private void CheckState()
	{
		if (w == null && write_headers)
		{
			WriteMimeHeaders();
		}
		if (w == null || w.WriteState == WriteState.Closed || w.WriteState == WriteState.Error)
		{
			w = CreateWriter();
		}
	}

	private void WriteMimeHeaders()
	{
		writer.Write("MIME-Version: 1.0\r\n");
		writer.Write("Content-Type: ");
		writer.Write(content_type.ToString());
		writer.Write("\r\n\r\n\r\n");
	}

	private void WriteStartOfMimeSection()
	{
		section_count++;
		if (section_count <= 1)
		{
			writer.Write("\r\n");
			writer.Write("--");
			writer.Write(content_type.Boundary);
			writer.Write("\r\n");
			writer.Write("Content-ID: ");
			writer.Write(content_type.Parameters["start"]);
			writer.Write("\r\n");
			writer.Write("Content-Transfer-Encoding: 8bit\r\n");
			writer.Write("Content-Type: application/xop+xml;charset=");
			writer.Write(xml_writer_settings.Encoding.HeaderName);
			writer.Write(";type=\"");
			writer.Write(content_type.Parameters["start-info"].Replace("\"", "\\\""));
			writer.Write("\"\r\n\r\n");
		}
	}

	private void WriteEndOfMimeSection()
	{
		if (section_count <= 1)
		{
			writer.Write("\r\n");
			writer.Write("--");
			writer.Write(content_type.Boundary);
			writer.Write("--\r\n");
		}
	}
}
