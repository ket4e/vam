using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Xml;

internal class XmlBinaryDictionaryWriter : XmlDictionaryWriter
{
	private class MyBinaryWriter : BinaryWriter
	{
		public MyBinaryWriter(Stream s)
			: base(s)
		{
		}

		public void WriteFlexibleInt(int value)
		{
			Write7BitEncodedInt(value);
		}
	}

	private enum SaveTarget
	{
		None,
		Namespaces,
		XmlLang,
		XmlSpace
	}

	private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

	private const string XmlnsNamespace = "http://www.w3.org/2000/xmlns/";

	private MyBinaryWriter original;

	private MyBinaryWriter writer;

	private MyBinaryWriter buffer_writer;

	private IXmlDictionary dict_ext;

	private XmlDictionary dict_int = new XmlDictionary();

	private XmlBinaryWriterSession session;

	private bool owns_stream;

	private Encoding utf8Enc = new UTF8Encoding();

	private MemoryStream buffer = new MemoryStream();

	private WriteState state;

	private bool open_start_element;

	private List<KeyValuePair<string, object>> namespaces = new List<KeyValuePair<string, object>>();

	private string xml_lang;

	private XmlSpace xml_space;

	private int ns_index;

	private Stack<int> ns_index_stack = new Stack<int>();

	private Stack<string> xml_lang_stack = new Stack<string>();

	private Stack<XmlSpace> xml_space_stack = new Stack<XmlSpace>();

	private Stack<string> element_ns_stack = new Stack<string>();

	private string element_ns = string.Empty;

	private int element_count;

	private string element_prefix;

	private string attr_value;

	private string current_attr_prefix;

	private object current_attr_name;

	private object current_attr_ns;

	private bool attr_typed_value;

	private SaveTarget save_target;

	public override WriteState WriteState => state;

	public override string XmlLang => xml_lang;

	public override XmlSpace XmlSpace => xml_space;

	public XmlBinaryDictionaryWriter(Stream stream, IXmlDictionary dictionary, XmlBinaryWriterSession session, bool ownsStream)
	{
		if (dictionary == null)
		{
			dictionary = new XmlDictionary();
		}
		if (session == null)
		{
			session = new XmlBinaryWriterSession();
		}
		original = new MyBinaryWriter(stream);
		writer = original;
		buffer_writer = new MyBinaryWriter(buffer);
		dict_ext = dictionary;
		this.session = session;
		owns_stream = ownsStream;
		AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");
		AddNamespace("xml", "http://www.w3.org/2000/xmlns/");
		ns_index = 2;
	}

	private void AddMissingElementXmlns()
	{
		for (int i = ns_index; i < namespaces.Count; i++)
		{
			KeyValuePair<string, object> keyValuePair = namespaces[i];
			string key = keyValuePair.Key;
			string text = keyValuePair.Value as string;
			XmlDictionaryString ds = keyValuePair.Value as XmlDictionaryString;
			if (text != null)
			{
				if (key.Length > 0)
				{
					writer.Write((byte)9);
					writer.Write(key);
				}
				else
				{
					writer.Write((byte)8);
				}
				writer.Write(text);
			}
			else
			{
				if (key.Length > 0)
				{
					writer.Write((byte)11);
					writer.Write(key);
				}
				else
				{
					writer.Write((byte)10);
				}
				WriteDictionaryIndex(ds);
			}
		}
		ns_index = namespaces.Count;
	}

	private void CheckState()
	{
		if (state == WriteState.Closed)
		{
			throw new InvalidOperationException("The Writer is closed.");
		}
	}

	private void ProcessStateForContent()
	{
		CheckState();
		if (state == WriteState.Element)
		{
			CloseStartElement();
		}
		ProcessPendingBuffer(last: false, endElement: false);
		if (state != WriteState.Attribute)
		{
			writer = buffer_writer;
		}
	}

	private void ProcessTypedValue()
	{
		ProcessStateForContent();
		if (state == WriteState.Attribute)
		{
			if (attr_typed_value)
			{
				throw new InvalidOperationException($"A typed value for the attribute '{current_attr_name}' in namespace '{current_attr_ns}' was already written");
			}
			attr_typed_value = true;
		}
	}

	private void ProcessPendingBuffer(bool last, bool endElement)
	{
		if (buffer.Position > 0)
		{
			byte[] array = buffer.GetBuffer();
			if (endElement)
			{
				array[0]++;
			}
			original.Write(array, 0, (int)buffer.Position);
			buffer.SetLength(0L);
		}
		if (last)
		{
			writer = original;
		}
	}

	public override void Close()
	{
		CloseOpenAttributeAndElements();
		if (owns_stream)
		{
			writer.Close();
		}
		else if (state != WriteState.Closed)
		{
			writer.Flush();
		}
		state = WriteState.Closed;
	}

	private void CloseOpenAttributeAndElements()
	{
		CloseStartElement();
		while (element_count > 0)
		{
			WriteEndElement();
		}
	}

	private void CloseStartElement()
	{
		if (open_start_element)
		{
			if (state == WriteState.Attribute)
			{
				WriteEndAttribute();
			}
			AddMissingElementXmlns();
			state = WriteState.Content;
			open_start_element = false;
		}
	}

	public override void Flush()
	{
		writer.Flush();
	}

	public override string LookupPrefix(string ns)
	{
		if (ns == null || ns == string.Empty)
		{
			throw new ArgumentException("The Namespace cannot be empty.");
		}
		return namespaces.LastOrDefault((KeyValuePair<string, object> i) => i.Value.ToString() == ns).Key;
	}

	public override void WriteBase64(byte[] buffer, int index, int count)
	{
		if (count < 0)
		{
			throw new IndexOutOfRangeException("Negative count");
		}
		ProcessStateForContent();
		if (count < 256)
		{
			writer.Write((byte)158);
			writer.Write((byte)count);
			writer.Write(buffer, index, count);
		}
		else if (count < 65536)
		{
			writer.Write((byte)158);
			writer.Write((ushort)count);
			writer.Write(buffer, index, count);
		}
		else
		{
			writer.Write((byte)162);
			writer.Write(count);
			writer.Write(buffer, index, count);
		}
	}

	public override void WriteCData(string text)
	{
		if (text.IndexOf("]]>") >= 0)
		{
			throw new ArgumentException("CDATA section cannot contain text \"]]>\".");
		}
		ProcessStateForContent();
		WriteTextBinary(text);
	}

	public override void WriteCharEntity(char ch)
	{
		WriteChars(new char[1] { ch }, 0, 1);
	}

	public override void WriteChars(char[] buffer, int index, int count)
	{
		ProcessStateForContent();
		int byteCount = Encoding.UTF8.GetByteCount(buffer, index, count);
		if (byteCount == 0)
		{
			writer.Write((byte)168);
		}
		else if (count == 1 && buffer[0] == '0')
		{
			writer.Write((byte)128);
		}
		else if (count == 1 && buffer[0] == '1')
		{
			writer.Write((byte)130);
		}
		else if (byteCount < 256)
		{
			writer.Write((byte)152);
			writer.Write((byte)byteCount);
			writer.Write(buffer, index, count);
		}
		else if (byteCount < 65536)
		{
			writer.Write((byte)154);
			writer.Write((ushort)byteCount);
			writer.Write(buffer, index, count);
		}
		else
		{
			writer.Write((byte)156);
			writer.Write(byteCount);
			writer.Write(buffer, index, count);
		}
	}

	public override void WriteComment(string text)
	{
		if (text.EndsWith("-"))
		{
			throw new ArgumentException("An XML comment cannot contain \"--\" inside.");
		}
		if (text.IndexOf("--") > 0)
		{
			throw new ArgumentException("An XML comment cannot end with \"-\".");
		}
		ProcessStateForContent();
		if (state == WriteState.Attribute)
		{
			throw new InvalidOperationException("Comment node is not allowed inside an attribute");
		}
		writer.Write((byte)2);
		writer.Write(text);
	}

	public override void WriteDocType(string name, string pubid, string sysid, string subset)
	{
		throw new NotSupportedException("This XmlWriter implementation does not support document type.");
	}

	public override void WriteEndAttribute()
	{
		if (state != WriteState.Attribute)
		{
			throw new InvalidOperationException("Token EndAttribute in state Start would result in an invalid XML document.");
		}
		CheckState();
		if (attr_value == null)
		{
			attr_value = string.Empty;
		}
		switch (save_target)
		{
		case SaveTarget.XmlLang:
			xml_lang = attr_value;
			goto default;
		case SaveTarget.XmlSpace:
			switch (attr_value)
			{
			default:
			{
				int num;
				if (num == 1)
				{
					xml_space = XmlSpace.Default;
					break;
				}
				throw new ArgumentException($"Invalid xml:space value: '{attr_value}'");
			}
			case "preserve":
				xml_space = XmlSpace.Preserve;
				break;
			}
			goto default;
		case SaveTarget.Namespaces:
			if (current_attr_name.ToString().Length > 0 && attr_value.Length == 0)
			{
				throw new ArgumentException("Cannot use prefix with an empty namespace.");
			}
			AddNamespaceChecked(current_attr_name.ToString(), attr_value);
			break;
		default:
			if (!attr_typed_value)
			{
				WriteTextBinary(attr_value);
			}
			break;
		}
		if (current_attr_prefix.Length > 0 && save_target != SaveTarget.Namespaces)
		{
			AddNamespaceChecked(current_attr_prefix, current_attr_ns);
		}
		state = WriteState.Element;
		current_attr_prefix = null;
		current_attr_name = null;
		current_attr_ns = null;
		attr_value = null;
		attr_typed_value = false;
	}

	public override void WriteEndDocument()
	{
		CloseOpenAttributeAndElements();
		switch (state)
		{
		case WriteState.Start:
			throw new InvalidOperationException("Document has not started.");
		case WriteState.Prolog:
			throw new ArgumentException("This document does not have a root element.");
		}
		state = WriteState.Start;
	}

	private bool SupportsCombinedEndElementSupport(byte operation)
	{
		byte b = operation;
		if (b == 2)
		{
			return false;
		}
		return true;
	}

	public override void WriteEndElement()
	{
		if (element_count-- == 0)
		{
			throw new InvalidOperationException("There was no XML start tag open.");
		}
		if (state == WriteState.Attribute)
		{
			WriteEndAttribute();
		}
		bool flag = buffer.Position == 0L || !SupportsCombinedEndElementSupport(buffer.GetBuffer()[0]);
		ProcessPendingBuffer(last: true, !flag);
		CheckState();
		AddMissingElementXmlns();
		if (flag)
		{
			writer.Write((byte)1);
		}
		element_ns = element_ns_stack.Pop();
		xml_lang = xml_lang_stack.Pop();
		xml_space = xml_space_stack.Pop();
		int count = namespaces.Count;
		ns_index = ns_index_stack.Pop();
		namespaces.RemoveRange(ns_index, count - ns_index);
		open_start_element = false;
		base.Depth--;
	}

	public override void WriteEntityRef(string name)
	{
		throw new NotSupportedException("This XmlWriter implementation does not support entity references.");
	}

	public override void WriteFullEndElement()
	{
		WriteEndElement();
	}

	public override void WriteProcessingInstruction(string name, string text)
	{
		if (name != "xml")
		{
			throw new ArgumentException("Processing instructions are not supported. ('xml' is allowed for XmlDeclaration; this is because of design problem of ECMA XmlWriter)");
		}
	}

	public override void WriteQualifiedName(XmlDictionaryString local, XmlDictionaryString ns)
	{
		string text = namespaces.LastOrDefault((KeyValuePair<string, object> i) => i.Value.ToString() == ns.ToString()).Key;
		bool flag = text != null;
		if (text == null)
		{
			text = LookupPrefix(ns.Value);
		}
		if (text == null)
		{
			throw new ArgumentException($"Namespace URI '{ns}' is not bound to any of the prefixes");
		}
		ProcessTypedValue();
		if (flag && text.Length == 1)
		{
			writer.Write((byte)188);
			writer.Write((byte)(text[0] - 97));
			WriteDictionaryIndex(local);
		}
		else
		{
			WriteString(text);
			WriteString(":");
			WriteString(local);
		}
	}

	public override void WriteRaw(string data)
	{
		WriteString(data);
	}

	public override void WriteRaw(char[] buffer, int index, int count)
	{
		WriteChars(buffer, index, count);
	}

	private void CheckStateForAttribute()
	{
		CheckState();
		if (state != WriteState.Element)
		{
			throw new InvalidOperationException(string.Concat("Token StartAttribute in state ", WriteState, " would result in an invalid XML document."));
		}
	}

	private string CreateNewPrefix()
	{
		return CreateNewPrefix(string.Empty);
	}

	private string CreateNewPrefix(string p)
	{
		for (char c = 'a'; c <= 'z'; c += '\u0001')
		{
			if (!namespaces.Any((KeyValuePair<string, object> iter) => iter.Key == p + c))
			{
				return p + c;
			}
		}
		for (char c2 = 'a'; c2 <= 'z'; c2 = (char)(c2 + 1))
		{
			string text = CreateNewPrefix(c2.ToString());
			if (text != null)
			{
				return text;
			}
		}
		throw new InvalidOperationException("too many prefix population");
	}

	private bool CollectionContains(ICollection col, string value)
	{
		foreach (string item in col)
		{
			if (item == value)
			{
				return true;
			}
		}
		return false;
	}

	private void ProcessStartAttributeCommon(ref string prefix, string localName, string ns, object nameObj, object nsObj)
	{
		if (prefix.Length == 0 && ns.Length > 0)
		{
			prefix = LookupPrefix(ns);
			if (string.IsNullOrEmpty(prefix))
			{
				prefix = CreateNewPrefix();
			}
		}
		else if (prefix.Length > 0 && ns.Length == 0)
		{
			switch (prefix)
			{
			default:
			{
				int num;
				if (num == 1)
				{
					nsObj = (ns = "http://www.w3.org/2000/xmlns/");
					break;
				}
				throw new ArgumentException("Cannot use prefix with an empty namespace.");
			}
			case "xml":
				nsObj = (ns = "http://www.w3.org/XML/1998/namespace");
				break;
			}
		}
		if (prefix == "xmlns" && ns != "http://www.w3.org/2000/xmlns/")
		{
			throw new ArgumentException(string.Format("The 'xmlns' attribute is bound to the reserved namespace '{0}'", "http://www.w3.org/2000/xmlns/"));
		}
		CheckStateForAttribute();
		state = WriteState.Attribute;
		save_target = SaveTarget.None;
		switch (prefix)
		{
		case "xml":
			ns = "http://www.w3.org/XML/1998/namespace";
			switch (localName)
			{
			case "lang":
				save_target = SaveTarget.XmlLang;
				break;
			case "space":
				save_target = SaveTarget.XmlSpace;
				break;
			}
			break;
		case "xmlns":
			save_target = SaveTarget.Namespaces;
			break;
		}
		current_attr_prefix = prefix;
		current_attr_name = nameObj;
		current_attr_ns = nsObj;
	}

	public override void WriteStartAttribute(string prefix, string localName, string ns)
	{
		if (prefix == null)
		{
			prefix = string.Empty;
		}
		if (ns == null)
		{
			ns = string.Empty;
		}
		if (localName == "xmlns" && prefix.Length == 0)
		{
			prefix = "xmlns";
			localName = string.Empty;
		}
		ProcessStartAttributeCommon(ref prefix, localName, ns, localName, ns);
		if (save_target == SaveTarget.Namespaces)
		{
			return;
		}
		byte b = (byte)((prefix.Length == 1 && 'a' <= prefix[0] && prefix[0] <= 'z') ? ((byte)(prefix[0] - 97 + 38)) : ((prefix.Length != 0) ? 5 : 4));
		if (38 <= b && b <= 63)
		{
			writer.Write(b);
			writer.Write(localName);
			return;
		}
		writer.Write(b);
		if (prefix.Length > 0)
		{
			writer.Write(prefix);
		}
		writer.Write(localName);
	}

	public override void WriteStartDocument()
	{
		WriteStartDocument(standalone: false);
	}

	public override void WriteStartDocument(bool standalone)
	{
		if (state != 0)
		{
			throw new InvalidOperationException("WriteStartDocument should be the first call.");
		}
		CheckState();
		state = WriteState.Prolog;
	}

	private void PrepareStartElement()
	{
		ProcessPendingBuffer(last: true, endElement: false);
		CheckState();
		CloseStartElement();
		base.Depth++;
		element_ns_stack.Push(element_ns);
		xml_lang_stack.Push(xml_lang);
		xml_space_stack.Push(xml_space);
		ns_index_stack.Push(ns_index);
	}

	public override void WriteStartElement(string prefix, string localName, string ns)
	{
		PrepareStartElement();
		if (prefix != null && prefix != string.Empty && (ns == null || ns == string.Empty))
		{
			throw new ArgumentException("Cannot use a prefix with an empty namespace.");
		}
		if (ns == null)
		{
			ns = string.Empty;
		}
		if (ns == string.Empty)
		{
			prefix = string.Empty;
		}
		if (prefix == null)
		{
			prefix = string.Empty;
		}
		byte b = (byte)((prefix.Length == 1 && 'a' <= prefix[0] && prefix[0] <= 'z') ? ((byte)(prefix[0] - 97 + 94)) : ((prefix.Length != 0) ? 65 : 64));
		if (94 <= b && b <= 119)
		{
			writer.Write(b);
			writer.Write(localName);
		}
		else
		{
			writer.Write(b);
			if (prefix.Length > 0)
			{
				writer.Write(prefix);
			}
			writer.Write(localName);
		}
		OpenElement(prefix, ns);
	}

	private void OpenElement(string prefix, object nsobj)
	{
		string text = nsobj.ToString();
		state = WriteState.Element;
		open_start_element = true;
		element_prefix = prefix;
		element_count++;
		element_ns = nsobj.ToString();
		if (element_ns != string.Empty && LookupPrefix(element_ns) != prefix)
		{
			AddNamespace(prefix, nsobj);
		}
	}

	private void AddNamespace(string prefix, object nsobj)
	{
		namespaces.Add(new KeyValuePair<string, object>(prefix, nsobj));
	}

	private void CheckIfTextAllowed()
	{
		WriteState writeState = state;
		if (writeState != 0 && writeState != WriteState.Prolog)
		{
			return;
		}
		throw new InvalidOperationException("Token content in state Prolog would result in an invalid XML document.");
	}

	public override void WriteString(string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		CheckIfTextAllowed();
		if (text == null)
		{
			text = string.Empty;
		}
		ProcessStateForContent();
		if (state == WriteState.Attribute)
		{
			attr_value += text;
		}
		else
		{
			WriteTextBinary(text);
		}
	}

	public override void WriteString(XmlDictionaryString text)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		CheckIfTextAllowed();
		if (text == null)
		{
			text = XmlDictionaryString.Empty;
		}
		ProcessStateForContent();
		if (state == WriteState.Attribute)
		{
			attr_value += text.Value;
			return;
		}
		if (text.Equals(XmlDictionary.Empty))
		{
			writer.Write((byte)168);
			return;
		}
		writer.Write((byte)170);
		WriteDictionaryIndex(text);
	}

	public override void WriteSurrogateCharEntity(char lowChar, char highChar)
	{
		WriteChars(new char[2] { highChar, lowChar }, 0, 2);
	}

	public override void WriteWhitespace(string ws)
	{
		for (int i = 0; i < ws.Length; i++)
		{
			switch (ws[i])
			{
			case '\t':
			case '\n':
			case '\r':
			case ' ':
				continue;
			}
			throw new ArgumentException("Invalid Whitespace");
		}
		ProcessStateForContent();
		WriteTextBinary(ws);
	}

	public override void WriteXmlnsAttribute(string prefix, string namespaceUri)
	{
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		if (string.IsNullOrEmpty(prefix))
		{
			prefix = CreateNewPrefix();
		}
		CheckStateForAttribute();
		AddNamespaceChecked(prefix, namespaceUri);
		state = WriteState.Element;
	}

	private void AddNamespaceChecked(string prefix, object ns)
	{
		switch (ns.ToString())
		{
		case "http://www.w3.org/2000/xmlns/":
		case "http://www.w3.org/XML/1998/namespace":
			return;
		}
		if (prefix == null)
		{
			throw new InvalidOperationException();
		}
		KeyValuePair<string, object> item = namespaces.LastOrDefault((KeyValuePair<string, object> i) => i.Key == prefix);
		if (item.Key != null)
		{
			if (item.Value.ToString() != ns.ToString())
			{
				if (namespaces.LastIndexOf(item) >= ns_index)
				{
					throw new ArgumentException(string.Format("The prefix '{0}' is already mapped to another namespace URI '{1}' in this element scope and cannot be mapped to '{2}'", prefix ?? "(null)", item.Value ?? "(null)", ns.ToString()));
				}
				AddNamespace(prefix, ns);
			}
		}
		else
		{
			AddNamespace(prefix, ns);
		}
	}

	private void WriteDictionaryIndex(XmlDictionaryString ds)
	{
		bool flag = false;
		int key = ds.Key;
		if (ds.Dictionary != dict_ext)
		{
			flag = true;
			if (dict_int.TryLookup(ds.Value, out var result))
			{
				ds = result;
			}
			if (!session.TryLookup(ds, out key))
			{
				session.TryAdd(dict_int.Add(ds.Value), out key);
			}
		}
		if (key >= 128)
		{
			writer.Write((byte)(128 + (key % 128 << 1) + (flag ? 1 : 0)));
			writer.Write((byte)((byte)(key / 128) << 1));
		}
		else
		{
			writer.Write((byte)((key % 128 << 1) + (flag ? 1 : 0)));
		}
	}

	public override void WriteStartElement(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		PrepareStartElement();
		if (prefix == null)
		{
			prefix = string.Empty;
		}
		byte b = (byte)((prefix.Length == 1 && 'a' <= prefix[0] && prefix[0] <= 'z') ? ((byte)(prefix[0] - 97 + 68)) : ((prefix.Length != 0) ? 67 : 66));
		if (68 <= b && b <= 93)
		{
			writer.Write(b);
			WriteDictionaryIndex(localName);
		}
		else
		{
			writer.Write(b);
			if (prefix.Length > 0)
			{
				writer.Write(prefix);
			}
			WriteDictionaryIndex(localName);
		}
		OpenElement(prefix, namespaceUri);
	}

	public override void WriteStartAttribute(string prefix, XmlDictionaryString localName, XmlDictionaryString ns)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (prefix == null)
		{
			prefix = string.Empty;
		}
		if (ns == null)
		{
			ns = XmlDictionaryString.Empty;
		}
		if (localName.Value == "xmlns" && prefix.Length == 0)
		{
			prefix = "xmlns";
			localName = XmlDictionaryString.Empty;
		}
		ProcessStartAttributeCommon(ref prefix, localName.Value, ns.Value, localName, ns);
		if (save_target == SaveTarget.Namespaces)
		{
			return;
		}
		if (prefix.Length == 1 && 'a' <= prefix[0] && prefix[0] <= 'z')
		{
			writer.Write((byte)(prefix[0] - 97 + 12));
			WriteDictionaryIndex(localName);
			return;
		}
		byte value = (byte)((ns.Value.Length != 0) ? 7 : 6);
		writer.Write(value);
		if (prefix.Length > 0)
		{
			writer.Write(prefix);
		}
		WriteDictionaryIndex(localName);
	}

	public override void WriteXmlnsAttribute(string prefix, XmlDictionaryString namespaceUri)
	{
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		if (string.IsNullOrEmpty(prefix))
		{
			prefix = CreateNewPrefix();
		}
		CheckStateForAttribute();
		AddNamespaceChecked(prefix, namespaceUri);
		state = WriteState.Element;
	}

	public override void WriteValue(bool value)
	{
		ProcessTypedValue();
		writer.Write((byte)((!value) ? 132 : 134));
	}

	public override void WriteValue(int value)
	{
		WriteValue((long)value);
	}

	public override void WriteValue(long value)
	{
		ProcessTypedValue();
		if (value == 0L)
		{
			writer.Write((byte)128);
		}
		else if (value == 1)
		{
			writer.Write((byte)130);
		}
		else if (value < 0 || value > uint.MaxValue)
		{
			writer.Write((byte)142);
			for (int i = 0; i < 8; i++)
			{
				writer.Write((byte)(value & 0xFF));
				value >>= 8;
			}
		}
		else if (value <= 255)
		{
			writer.Write((byte)136);
			writer.Write((byte)value);
		}
		else if (value <= 32767)
		{
			writer.Write((byte)138);
			writer.Write((byte)(value & 0xFF));
			writer.Write((byte)(value >> 8));
		}
		else if (value <= int.MaxValue)
		{
			writer.Write((byte)140);
			for (int j = 0; j < 4; j++)
			{
				writer.Write((byte)(value & 0xFF));
				value >>= 8;
			}
		}
	}

	public override void WriteValue(float value)
	{
		ProcessTypedValue();
		writer.Write((byte)144);
		WriteValueContent(value);
	}

	private void WriteValueContent(float value)
	{
		writer.Write(value);
	}

	public override void WriteValue(double value)
	{
		ProcessTypedValue();
		writer.Write((byte)146);
		WriteValueContent(value);
	}

	private void WriteValueContent(double value)
	{
		writer.Write(value);
	}

	public override void WriteValue(decimal value)
	{
		ProcessTypedValue();
		writer.Write((byte)148);
		WriteValueContent(value);
	}

	private void WriteValueContent(decimal value)
	{
		int[] bits = decimal.GetBits(value);
		writer.Write(bits[3]);
		writer.Write(bits[2]);
		writer.Write(bits[0]);
		writer.Write(bits[1]);
	}

	public override void WriteValue(DateTime value)
	{
		ProcessTypedValue();
		writer.Write((byte)150);
		WriteValueContent(value);
	}

	private void WriteValueContent(DateTime value)
	{
		writer.Write(value.Ticks);
	}

	public override void WriteValue(Guid value)
	{
		ProcessTypedValue();
		writer.Write((byte)176);
		WriteValueContent(value);
	}

	private void WriteValueContent(Guid value)
	{
		byte[] array = value.ToByteArray();
		writer.Write(array, 0, array.Length);
	}

	public override void WriteValue(UniqueId value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		if (value.TryGetGuid(out var guid))
		{
			ProcessTypedValue();
			writer.Write((byte)172);
			byte[] array = guid.ToByteArray();
			writer.Write(array, 0, array.Length);
		}
		else
		{
			WriteValue(value.ToString());
		}
	}

	public override void WriteValue(TimeSpan value)
	{
		ProcessTypedValue();
		writer.Write((byte)174);
		WriteValueContent(value);
	}

	private void WriteValueContent(TimeSpan value)
	{
		WriteBigEndian(value.Ticks, 8);
	}

	private void WriteBigEndian(long value, int digits)
	{
		long num = 0L;
		for (int i = 0; i < digits; i++)
		{
			num = (num << 8) + (value & 0xFF);
			value >>= 8;
		}
		for (int j = 0; j < digits; j++)
		{
			writer.Write((byte)(num & 0xFF));
			num >>= 8;
		}
	}

	private void WriteTextBinary(string text)
	{
		if (text.Length == 0)
		{
			writer.Write((byte)168);
			return;
		}
		char[] array = text.ToCharArray();
		WriteChars(array, 0, array.Length);
	}

	private void WriteValueContent(bool value)
	{
		writer.Write((byte)(value ? 1 : 0));
	}

	private void WriteValueContent(short value)
	{
		writer.Write(value);
	}

	private void WriteValueContent(int value)
	{
		writer.Write(value);
	}

	private void WriteValueContent(long value)
	{
		writer.Write(value);
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

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, bool[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(bool[] array, int offset, int length)
	{
		writer.Write((byte)181);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, DateTime[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(DateTime[] array, int offset, int length)
	{
		writer.Write((byte)151);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, decimal[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(decimal[] array, int offset, int length)
	{
		writer.Write((byte)149);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, double[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(double[] array, int offset, int length)
	{
		writer.Write((byte)147);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, Guid[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(Guid[] array, int offset, int length)
	{
		writer.Write((byte)177);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, short[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(short[] array, int offset, int length)
	{
		writer.Write((byte)139);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, int[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(int[] array, int offset, int length)
	{
		writer.Write((byte)141);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, long[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(long[] array, int offset, int length)
	{
		writer.Write((byte)143);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, float[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(float[] array, int offset, int length)
	{
		writer.Write((byte)145);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}

	public override void WriteArray(string prefix, XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	public override void WriteArray(string prefix, string localName, string namespaceUri, TimeSpan[] array, int offset, int length)
	{
		CheckWriteArrayArguments(array, offset, length);
		writer.Write((byte)3);
		WriteStartElement(prefix, localName, namespaceUri);
		WriteEndElement();
		WriteArrayRemaining(array, offset, length);
	}

	private void WriteArrayRemaining(TimeSpan[] array, int offset, int length)
	{
		writer.Write((byte)175);
		writer.WriteFlexibleInt(length);
		for (int i = offset; i < offset + length; i++)
		{
			WriteValueContent(array[i]);
		}
	}
}
