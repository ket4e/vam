using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System.Xml;

internal class XmlBinaryDictionaryReader : XmlDictionaryReader, IXmlNamespaceResolver
{
	internal interface ISource
	{
		int Position { get; }

		BinaryReader Reader { get; }

		int ReadByte();

		int Read(byte[] data, int offset, int count);
	}

	internal class StreamSource : ISource
	{
		private BinaryReader reader;

		public int Position => (int)reader.BaseStream.Position;

		public BinaryReader Reader => reader;

		public StreamSource(Stream stream)
		{
			reader = new BinaryReader(stream);
		}

		public int ReadByte()
		{
			if (reader.PeekChar() < 0)
			{
				return -1;
			}
			return reader.ReadByte();
		}

		public int Read(byte[] data, int offset, int count)
		{
			return reader.Read(data, offset, count);
		}
	}

	private class NodeInfo
	{
		public bool IsAttributeValue;

		public int Position;

		public string Prefix;

		public XmlDictionaryString DictLocalName;

		public XmlDictionaryString DictNS;

		public XmlDictionaryString DictValue;

		public XmlNodeType NodeType;

		public object TypedValue;

		public byte ValueType;

		public int NSSlot;

		private string name = string.Empty;

		private string local_name = string.Empty;

		private string ns = string.Empty;

		private string value;

		public string LocalName
		{
			get
			{
				return (DictLocalName == null) ? local_name : DictLocalName.Value;
			}
			set
			{
				DictLocalName = null;
				local_name = value;
			}
		}

		public string NS
		{
			get
			{
				return (DictNS == null) ? ns : DictNS.Value;
			}
			set
			{
				DictNS = null;
				ns = value;
			}
		}

		public string Name
		{
			get
			{
				if (name.Length == 0)
				{
					name = ((Prefix.Length <= 0) ? LocalName : (Prefix + ":" + LocalName));
				}
				return name;
			}
		}

		public virtual string Value
		{
			get
			{
				switch (ValueType)
				{
				case 0:
				case 2:
				case 152:
				case 154:
				case 156:
				case 168:
				case 182:
				case 184:
				case 186:
					return value;
				case 170:
					return DictValue.Value;
				case 128:
					return "0";
				case 130:
					return "1";
				case 134:
					return "true";
				case 132:
					return "false";
				case 136:
					return XmlConvert.ToString((byte)TypedValue);
				case 138:
					return XmlConvert.ToString((short)TypedValue);
				case 140:
					return XmlConvert.ToString((int)TypedValue);
				case 142:
					return XmlConvert.ToString((long)TypedValue);
				case 144:
					return XmlConvert.ToString((float)TypedValue);
				case 146:
					return XmlConvert.ToString((double)TypedValue);
				case 150:
					return XmlConvert.ToString((DateTime)TypedValue, XmlDateTimeSerializationMode.RoundtripKind);
				case 174:
					return XmlConvert.ToString((TimeSpan)TypedValue);
				case 176:
					return XmlConvert.ToString((Guid)TypedValue);
				case 172:
					return TypedValue.ToString();
				case 158:
				case 160:
				case 162:
					return Convert.ToBase64String((byte[])TypedValue);
				default:
					throw new NotImplementedException("ValueType " + ValueType + " on node " + NodeType);
				}
			}
			set
			{
				this.value = value;
			}
		}

		public NodeInfo()
		{
		}

		public NodeInfo(bool isAttr)
		{
			IsAttributeValue = isAttr;
		}

		public virtual void Reset()
		{
			Position = 0;
			DictLocalName = (DictNS = null);
			string prefix = (Value = string.Empty);
			prefix = (NS = (Prefix = prefix));
			LocalName = prefix;
			NodeType = XmlNodeType.None;
			TypedValue = null;
			ValueType = 0;
			NSSlot = -1;
		}
	}

	private class AttrNodeInfo : NodeInfo
	{
		private XmlBinaryDictionaryReader owner;

		public int ValueIndex;

		public override string Value => owner.attr_values[ValueIndex].Value;

		public AttrNodeInfo(XmlBinaryDictionaryReader owner)
		{
			this.owner = owner;
		}

		public override void Reset()
		{
			base.Reset();
			ValueIndex = -1;
			NodeType = XmlNodeType.Attribute;
		}
	}

	private ISource source;

	private IXmlDictionary dictionary;

	private XmlDictionaryReaderQuotas quota;

	private XmlBinaryReaderSession session;

	private OnXmlDictionaryReaderClose on_close;

	private XmlParserContext context;

	private ReadState state;

	private NodeInfo node;

	private NodeInfo current;

	private List<AttrNodeInfo> attributes = new List<AttrNodeInfo>();

	private List<NodeInfo> attr_values = new List<NodeInfo>();

	private List<NodeInfo> node_stack = new List<NodeInfo>();

	private List<XmlQualifiedName> ns_store = new List<XmlQualifiedName>();

	private Dictionary<int, XmlDictionaryString> ns_dict_store = new Dictionary<int, XmlDictionaryString>();

	private int attr_count;

	private int attr_value_count;

	private int current_attr = -1;

	private int depth;

	private int ns_slot;

	private int next = -1;

	private bool is_next_end_element;

	private byte[] tmp_buffer = new byte[128];

	private UTF8Encoding utf8enc = new UTF8Encoding();

	private int array_item_remaining;

	private byte array_item_type;

	private XmlNodeType array_state;

	public override int AttributeCount => attr_count;

	public override string BaseURI => context.BaseURI;

	public override int Depth => (current == node) ? depth : ((NodeType != XmlNodeType.Attribute) ? (depth + 2) : (depth + 1));

	public override bool EOF => state == ReadState.EndOfFile || state == ReadState.Error;

	public override bool HasValue => Value.Length > 0;

	public override bool IsEmptyElement => false;

	public override XmlNodeType NodeType => current.NodeType;

	public override string Prefix => (current_attr < 0) ? current.Prefix : attributes[current_attr].Prefix;

	public override string LocalName => (current_attr < 0) ? current.LocalName : attributes[current_attr].LocalName;

	public override string Name => (current_attr < 0) ? current.Name : attributes[current_attr].Name;

	public override string NamespaceURI => (current_attr < 0) ? current.NS : attributes[current_attr].NS;

	public override XmlNameTable NameTable => context.NameTable;

	public override XmlDictionaryReaderQuotas Quotas => quota;

	public override ReadState ReadState => state;

	public override string Value => current.Value;

	public XmlBinaryDictionaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quota, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
	{
		source = new StreamSource(new MemoryStream(buffer, offset, count));
		Initialize(dictionary, quota, session, onClose);
	}

	public XmlBinaryDictionaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quota, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
	{
		source = new StreamSource(stream);
		Initialize(dictionary, quota, session, onClose);
	}

	private void Initialize(IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
	{
		if (quotas == null)
		{
			throw new ArgumentNullException("quotas");
		}
		if (dictionary == null)
		{
			dictionary = new XmlDictionary();
		}
		this.dictionary = dictionary;
		quota = quotas;
		if (session == null)
		{
			session = new XmlBinaryReaderSession();
		}
		this.session = session;
		on_close = onClose;
		NameTable nameTable = new NameTable();
		context = new XmlParserContext(nameTable, new XmlNamespaceManager(nameTable), null, XmlSpace.None);
		current = (node = new NodeInfo());
		current.Reset();
		node_stack.Add(node);
	}

	public override void Close()
	{
		if (on_close != null)
		{
			on_close(this);
		}
	}

	public override string GetAttribute(int i)
	{
		if (i >= attr_count)
		{
			throw new ArgumentOutOfRangeException($"Specified attribute index is {i} and should be less than {attr_count}");
		}
		return attributes[i].Value;
	}

	public override string GetAttribute(string name)
	{
		for (int i = 0; i < attr_count; i++)
		{
			if (attributes[i].Name == name)
			{
				return attributes[i].Value;
			}
		}
		return null;
	}

	public override string GetAttribute(string localName, string ns)
	{
		for (int i = 0; i < attr_count; i++)
		{
			if (attributes[i].LocalName == localName && attributes[i].NS == ns)
			{
				return attributes[i].Value;
			}
		}
		return null;
	}

	public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
	{
		return context.NamespaceManager.GetNamespacesInScope(scope);
	}

	public string LookupPrefix(string ns)
	{
		return context.NamespaceManager.LookupPrefix(NameTable.Get(ns));
	}

	public override string LookupNamespace(string prefix)
	{
		return context.NamespaceManager.LookupNamespace(NameTable.Get(prefix));
	}

	public override bool IsArray(out Type type)
	{
		if (array_state == XmlNodeType.Element)
		{
			type = GetArrayType(array_item_type);
			return true;
		}
		type = null;
		return false;
	}

	public override bool MoveToElement()
	{
		bool result = current_attr >= 0;
		current_attr = -1;
		current = node;
		return result;
	}

	public override bool MoveToFirstAttribute()
	{
		if (attr_count == 0)
		{
			return false;
		}
		current_attr = 0;
		current = attributes[current_attr];
		return true;
	}

	public override bool MoveToNextAttribute()
	{
		if (++current_attr < attr_count)
		{
			current = attributes[current_attr];
			return true;
		}
		current_attr--;
		return false;
	}

	public override void MoveToAttribute(int i)
	{
		if (i >= attr_count)
		{
			throw new ArgumentOutOfRangeException($"Specified attribute index is {i} and should be less than {attr_count}");
		}
		current_attr = i;
		current = attributes[i];
	}

	public override bool MoveToAttribute(string name)
	{
		for (int i = 0; i < attributes.Count; i++)
		{
			if (attributes[i].Name == name)
			{
				MoveToAttribute(i);
				return true;
			}
		}
		return false;
	}

	public override bool MoveToAttribute(string localName, string ns)
	{
		for (int i = 0; i < attributes.Count; i++)
		{
			if (attributes[i].LocalName == localName && attributes[i].NS == ns)
			{
				MoveToAttribute(i);
				return true;
			}
		}
		return false;
	}

	public override bool ReadAttributeValue()
	{
		if (current_attr < 0)
		{
			return false;
		}
		int valueIndex = attributes[current_attr].ValueIndex;
		int num = ((current_attr + 1 != attr_count) ? attributes[current_attr + 1].ValueIndex : attr_value_count);
		if (valueIndex == num)
		{
			return false;
		}
		if (!current.IsAttributeValue)
		{
			current = attr_values[valueIndex];
			return true;
		}
		return false;
	}

	public override bool Read()
	{
		switch (state)
		{
		case ReadState.Error:
		case ReadState.EndOfFile:
		case ReadState.Closed:
			return false;
		default:
		{
			state = ReadState.Interactive;
			MoveToElement();
			attr_count = 0;
			attr_value_count = 0;
			ns_slot = 0;
			if (node.NodeType == XmlNodeType.Element)
			{
				if (node_stack.Count <= ++depth)
				{
					if (depth == quota.MaxDepth)
					{
						throw new XmlException($"Binary XML stream quota exceeded. Depth must be less than {quota.MaxDepth}");
					}
					node = new NodeInfo();
					node_stack.Add(node);
				}
				else
				{
					node = node_stack[depth];
					node.Reset();
				}
			}
			current = node;
			if (is_next_end_element)
			{
				is_next_end_element = false;
				node.Reset();
				ProcessEndElement();
				return true;
			}
			switch (array_state)
			{
			case XmlNodeType.Element:
				ReadArrayItem();
				return true;
			case XmlNodeType.Text:
				ShiftToArrayItemEndElement();
				return true;
			case XmlNodeType.EndElement:
				if (--array_item_remaining == 0)
				{
					array_state = XmlNodeType.None;
					break;
				}
				ShiftToArrayItemElement();
				return true;
			}
			node.Reset();
			int num = ((next < 0) ? source.ReadByte() : next);
			next = -1;
			if (num < 0)
			{
				state = ReadState.EndOfFile;
				current.Reset();
				return false;
			}
			is_next_end_element = num > 128 && (num & 1) == 1;
			num -= (is_next_end_element ? 1 : 0);
			switch (num)
			{
			case 1:
				ProcessEndElement();
				break;
			case 2:
				node.Value = ReadUTF8();
				node.ValueType = 2;
				node.NodeType = XmlNodeType.Comment;
				break;
			case 64:
			case 65:
			case 66:
			case 67:
				ReadElementBinary((byte)num);
				break;
			case 3:
				num = ReadByteOrError();
				ReadElementBinary((byte)num);
				num = ReadByteOrError();
				if (num != 1)
				{
					throw new XmlException($"EndElement is expected after element in an array. The actual byte was {num:X} in hexadecimal");
				}
				num = ReadByteOrError() - 1;
				VerifyValidArrayItemType(num);
				if (num < 0)
				{
					throw new XmlException("The stream has ended where the array item type is expected");
				}
				array_item_type = (byte)num;
				array_item_remaining = ReadVariantSize();
				if (array_item_remaining > quota.MaxArrayLength)
				{
					throw new Exception($"Binary xml stream exceeded max array length quota. Items are {quota.MaxArrayLength} and should be less than quota.MaxArrayLength");
				}
				array_state = XmlNodeType.Element;
				break;
			default:
				if ((68 <= num && num <= 93) || (94 <= num && num <= 119))
				{
					goto case 64;
				}
				ReadTextOrValue((byte)num, node, canSkip: false);
				break;
			}
			return true;
		}
		}
	}

	private void ReadArrayItem()
	{
		ReadTextOrValue(array_item_type, node, canSkip: false);
		array_state = XmlNodeType.Text;
	}

	private void ShiftToArrayItemEndElement()
	{
		ProcessEndElement();
		array_state = XmlNodeType.EndElement;
	}

	private void ShiftToArrayItemElement()
	{
		node.NodeType = XmlNodeType.Element;
		context.NamespaceManager.PushScope();
		array_state = XmlNodeType.Element;
	}

	private void VerifyValidArrayItemType(int ident)
	{
		if (GetArrayType(ident) == null)
		{
			throw new XmlException($"Unexpected array item type {ident:X} in hexadecimal");
		}
	}

	private Type GetArrayType(int ident)
	{
		return ident switch
		{
			180 => typeof(bool), 
			138 => typeof(short), 
			140 => typeof(int), 
			142 => typeof(long), 
			144 => typeof(float), 
			146 => typeof(double), 
			148 => typeof(decimal), 
			150 => typeof(DateTime), 
			174 => typeof(TimeSpan), 
			176 => typeof(Guid), 
			_ => null, 
		};
	}

	private void ProcessEndElement()
	{
		if (depth == 0)
		{
			throw new XmlException("Unexpected end of element while there is no element started.");
		}
		current = (node = node_stack[--depth]);
		node.NodeType = XmlNodeType.EndElement;
		context.NamespaceManager.PopScope();
	}

	private void ReadElementBinary(int ident)
	{
		node.NodeType = XmlNodeType.Element;
		node.Prefix = string.Empty;
		context.NamespaceManager.PushScope();
		switch (ident)
		{
		case 64:
			node.LocalName = ReadUTF8();
			break;
		case 65:
			node.Prefix = ReadUTF8();
			node.NSSlot = ns_slot++;
			goto case 64;
		case 66:
			node.DictLocalName = ReadDictName();
			break;
		case 67:
			node.Prefix = ReadUTF8();
			node.NSSlot = ns_slot++;
			goto case 66;
		default:
			if (68 <= ident && ident <= 93)
			{
				node.Prefix = ((char)(ident - 68 + 97)).ToString();
				node.DictLocalName = ReadDictName();
				break;
			}
			if (94 <= ident && ident <= 119)
			{
				node.Prefix = ((char)(ident - 94 + 97)).ToString();
				node.LocalName = ReadUTF8();
				break;
			}
			throw new XmlException($"Invalid element node type {ident:X02} in hexadecimal");
		}
		bool flag = true;
		do
		{
			ident = ReadByteOrError();
			switch (ident)
			{
			case 4:
			case 5:
			case 6:
			case 7:
				ReadAttribute((byte)ident);
				continue;
			case 8:
			case 9:
			case 10:
			case 11:
				ReadNamespace((byte)ident);
				continue;
			}
			if ((38 <= ident && ident <= 63) || (12 <= ident && ident <= 37))
			{
				ReadAttribute((byte)ident);
				continue;
			}
			next = ident;
			flag = false;
		}
		while (flag);
		node.NS = context.NamespaceManager.LookupNamespace(node.Prefix) ?? string.Empty;
		foreach (AttrNodeInfo attribute in attributes)
		{
			if (attribute.Prefix.Length > 0)
			{
				attribute.NS = context.NamespaceManager.LookupNamespace(attribute.Prefix);
			}
		}
		ns_store.Clear();
		ns_dict_store.Clear();
	}

	private void ReadAttribute(byte ident)
	{
		if (attributes.Count == attr_count)
		{
			attributes.Add(new AttrNodeInfo(this));
		}
		AttrNodeInfo attrNodeInfo = attributes[attr_count++];
		attrNodeInfo.Reset();
		attrNodeInfo.Position = source.Position;
		switch (ident)
		{
		case 4:
			attrNodeInfo.LocalName = ReadUTF8();
			break;
		case 5:
			attrNodeInfo.Prefix = ReadUTF8();
			attrNodeInfo.NSSlot = ns_slot++;
			goto case 4;
		case 6:
			attrNodeInfo.DictLocalName = ReadDictName();
			break;
		case 7:
			attrNodeInfo.Prefix = ReadUTF8();
			attrNodeInfo.NSSlot = ns_slot++;
			goto case 6;
		default:
			if (38 <= ident && ident <= 63)
			{
				attrNodeInfo.Prefix = ((char)(97 + ident - 38)).ToString();
				attrNodeInfo.LocalName = ReadUTF8();
				break;
			}
			if (12 <= ident && ident <= 37)
			{
				attrNodeInfo.Prefix = ((char)(97 + ident - 12)).ToString();
				attrNodeInfo.DictLocalName = ReadDictName();
				break;
			}
			throw new XmlException($"Unexpected attribute node type: 0x{ident:X02}");
		}
		ReadAttributeValueBinary(attrNodeInfo);
	}

	private void ReadNamespace(byte ident)
	{
		if (attributes.Count == attr_count)
		{
			attributes.Add(new AttrNodeInfo(this));
		}
		AttrNodeInfo attrNodeInfo = attributes[attr_count++];
		attrNodeInfo.Reset();
		attrNodeInfo.Position = source.Position;
		string text = null;
		string text2 = null;
		XmlDictionaryString xmlDictionaryString = null;
		switch (ident)
		{
		case 8:
			text = string.Empty;
			text2 = ReadUTF8();
			break;
		case 9:
			text = ReadUTF8();
			text2 = ReadUTF8();
			break;
		case 10:
			text = string.Empty;
			xmlDictionaryString = ReadDictName();
			ns_dict_store.Add(ns_store.Count, xmlDictionaryString);
			text2 = xmlDictionaryString.Value;
			break;
		case 11:
			text = ReadUTF8();
			xmlDictionaryString = ReadDictName();
			ns_dict_store.Add(ns_store.Count, xmlDictionaryString);
			text2 = xmlDictionaryString.Value;
			break;
		}
		attrNodeInfo.Prefix = ((text.Length <= 0) ? string.Empty : "xmlns");
		attrNodeInfo.LocalName = ((text.Length <= 0) ? "xmlns" : text);
		attrNodeInfo.NS = "http://www.w3.org/2000/xmlns/";
		attrNodeInfo.ValueIndex = attr_value_count;
		if (attr_value_count == attr_values.Count)
		{
			attr_values.Add(new NodeInfo(isAttr: true));
		}
		NodeInfo nodeInfo = attr_values[attr_value_count++];
		nodeInfo.Reset();
		nodeInfo.Value = text2;
		nodeInfo.ValueType = 152;
		nodeInfo.NodeType = XmlNodeType.Text;
		ns_store.Add(new XmlQualifiedName(text, text2));
		context.NamespaceManager.AddNamespace(text, text2);
	}

	private void ReadAttributeValueBinary(AttrNodeInfo a)
	{
		a.ValueIndex = attr_value_count;
		if (attr_value_count == attr_values.Count)
		{
			attr_values.Add(new NodeInfo(isAttr: true));
		}
		NodeInfo nodeInfo = attr_values[attr_value_count++];
		nodeInfo.Reset();
		int num = ReadByteOrError();
		bool flag = num > 128 && (num & 1) == 1;
		num -= (flag ? 1 : 0);
		ReadTextOrValue((byte)num, nodeInfo, canSkip: true);
	}

	private bool ReadTextOrValue(byte ident, NodeInfo node, bool canSkip)
	{
		node.Value = null;
		node.ValueType = ident;
		node.NodeType = XmlNodeType.Text;
		switch (ident)
		{
		case 128:
			node.TypedValue = 0;
			break;
		case 130:
			node.TypedValue = 1;
			break;
		case 132:
			node.TypedValue = false;
			break;
		case 134:
			node.TypedValue = true;
			break;
		case 136:
			node.TypedValue = ReadByteOrError();
			break;
		case 138:
			node.TypedValue = source.Reader.ReadInt16();
			break;
		case 140:
			node.TypedValue = source.Reader.ReadInt32();
			break;
		case 142:
			node.TypedValue = source.Reader.ReadInt64();
			break;
		case 144:
			node.TypedValue = source.Reader.ReadSingle();
			break;
		case 146:
			node.TypedValue = source.Reader.ReadDouble();
			break;
		case 148:
		{
			int[] array4 = new int[4];
			array4[3] = source.Reader.ReadInt32();
			array4[2] = source.Reader.ReadInt32();
			array4[0] = source.Reader.ReadInt32();
			array4[1] = source.Reader.ReadInt32();
			node.TypedValue = new decimal(array4);
			break;
		}
		case 150:
			node.TypedValue = new DateTime(source.Reader.ReadInt64());
			break;
		case 158:
		case 160:
		case 162:
		{
			byte[] array3 = Alloc(ident switch
			{
				158 => source.Reader.ReadByte(), 
				160 => source.Reader.ReadUInt16(), 
				_ => source.Reader.ReadInt32(), 
			});
			source.Reader.Read(array3, 0, array3.Length);
			node.TypedValue = array3;
			break;
		}
		case 174:
			node.TypedValue = new TimeSpan(source.Reader.ReadInt64());
			break;
		case 172:
		{
			byte[] array2 = new byte[16];
			source.Reader.Read(array2, 0, array2.Length);
			node.TypedValue = new UniqueId(new Guid(array2));
			break;
		}
		case 176:
		{
			byte[] array2 = new byte[16];
			source.Reader.Read(array2, 0, array2.Length);
			node.TypedValue = new Guid(array2);
			break;
		}
		case 152:
		case 154:
		case 156:
		case 182:
		case 184:
		case 186:
		{
			Encoding encoding = ((ident > 156) ? Encoding.Unicode : Encoding.UTF8);
			int num;
			switch (ident)
			{
			case 152:
			case 182:
				num = source.Reader.ReadByte();
				break;
			case 154:
			case 184:
				num = source.Reader.ReadUInt16();
				break;
			default:
				num = source.Reader.ReadInt32();
				break;
			}
			int num2 = num;
			byte[] array = Alloc(num2);
			source.Reader.Read(array, 0, num2);
			node.Value = encoding.GetString(array, 0, num2);
			node.NodeType = XmlNodeType.Text;
			break;
		}
		case 168:
			node.Value = string.Empty;
			node.NodeType = XmlNodeType.Text;
			break;
		case 170:
			node.DictValue = ReadDictName();
			node.NodeType = XmlNodeType.Text;
			break;
		default:
			if (!canSkip)
			{
				throw new ArgumentException(string.Format("Unexpected binary XML data at position {1}: {0:X}", ident + (is_next_end_element ? 1 : 0), source.Position));
			}
			next = ident;
			return false;
		}
		return true;
	}

	private byte[] Alloc(int size)
	{
		if (size > quota.MaxStringContentLength || size < 0)
		{
			throw new XmlException(string.Format("Text content buffer exceeds the quota limitation at {2}. {0} bytes and should be less than {1} bytes", size, quota.MaxStringContentLength, source.Position));
		}
		return new byte[size];
	}

	private int ReadVariantSize()
	{
		int num = 0;
		int num2 = 0;
		byte b;
		do
		{
			b = ReadByteOrError();
			num += (b & 0x7F) << (num2 & 0x1F);
			num2 += 7;
		}
		while (b >= 128);
		return num;
	}

	private string ReadUTF8()
	{
		int num = ReadVariantSize();
		if (num == 0)
		{
			return string.Empty;
		}
		if (tmp_buffer.Length < num)
		{
			int num2 = tmp_buffer.Length * 2;
			tmp_buffer = Alloc((num >= num2) ? num : num2);
		}
		num = source.Read(tmp_buffer, 0, num);
		return utf8enc.GetString(tmp_buffer, 0, num);
	}

	private XmlDictionaryString ReadDictName()
	{
		int num = ReadVariantSize();
		XmlDictionaryString result;
		if ((num & 1) == 1)
		{
			if (session.TryLookup(num >> 1, out result))
			{
				return result;
			}
		}
		else if (dictionary.TryLookup(num >> 1, out result))
		{
			return result;
		}
		throw new XmlException($"Input XML binary stream is invalid. No matching XML dictionary string entry at {num}. Binary stream position at {source.Position}");
	}

	private byte ReadByteOrError()
	{
		if (next >= 0)
		{
			byte result = (byte)next;
			next = -1;
			return result;
		}
		int num = source.ReadByte();
		if (num < 0)
		{
			throw new XmlException($"Unexpected end of binary stream. Position is at {source.Position}");
		}
		return (byte)num;
	}

	public override void ResolveEntity()
	{
		throw new NotSupportedException("this XmlReader does not support ResolveEntity.");
	}

	public override bool TryGetBase64ContentLength(out int length)
	{
		length = 0;
		switch (current.ValueType)
		{
		case 158:
		case 160:
		case 162:
			length = ((byte[])current.TypedValue).Length;
			return true;
		default:
			return false;
		}
	}

	public override string ReadContentAsString()
	{
		string text = string.Empty;
		do
		{
			switch (NodeType)
			{
			case XmlNodeType.Element:
			case XmlNodeType.EndElement:
				return text;
			case XmlNodeType.Text:
				text += Value;
				break;
			}
		}
		while (Read());
		return text;
	}

	public override int ReadContentAsInt()
	{
		int intValue = GetIntValue();
		Read();
		return intValue;
	}

	private int GetIntValue()
	{
		return node.ValueType switch
		{
			128 => 0, 
			130 => 1, 
			136 => (byte)current.TypedValue, 
			138 => (short)current.TypedValue, 
			140 => (int)current.TypedValue, 
			_ => throw new InvalidOperationException($"Current content is not an integer. (Internal value type:{(int)node.ValueType:X02})"), 
		};
	}

	public override long ReadContentAsLong()
	{
		if (node.ValueType == 142)
		{
			long result = (long)current.TypedValue;
			Read();
			return result;
		}
		return ReadContentAsInt();
	}

	public override float ReadContentAsFloat()
	{
		if (node.ValueType != 144)
		{
			throw new InvalidOperationException("Current content is not a single");
		}
		float result = (float)current.TypedValue;
		Read();
		return result;
	}

	public override double ReadContentAsDouble()
	{
		if (node.ValueType != 146)
		{
			throw new InvalidOperationException("Current content is not a double");
		}
		double result = (double)current.TypedValue;
		Read();
		return result;
	}

	private bool IsBase64Node(byte b)
	{
		switch (b)
		{
		case 158:
		case 160:
		case 162:
			return true;
		default:
			return false;
		}
	}

	public override byte[] ReadContentAsBase64()
	{
		byte[] array = null;
		if (!IsBase64Node(node.ValueType))
		{
			throw new InvalidOperationException("Current content is not base64");
		}
		while (NodeType == XmlNodeType.Text && IsBase64Node(node.ValueType))
		{
			if (array == null)
			{
				array = (byte[])node.TypedValue;
			}
			else
			{
				byte[] array2 = (byte[])node.TypedValue;
				byte[] array3 = Alloc(array.Length + array2.Length);
				Array.Copy(array, array3, array.Length);
				Array.Copy(array2, 0, array3, array.Length, array2.Length);
				array = array3;
			}
			Read();
		}
		return array;
	}

	public override Guid ReadContentAsGuid()
	{
		if (node.ValueType != 176)
		{
			throw new InvalidOperationException("Current content is not a Guid");
		}
		Guid result = (Guid)node.TypedValue;
		Read();
		return result;
	}

	public override UniqueId ReadContentAsUniqueId()
	{
		switch (node.ValueType)
		{
		case 152:
		case 154:
		case 156:
		case 182:
		case 184:
		case 186:
		{
			UniqueId result = new UniqueId(node.Value);
			Read();
			return result;
		}
		case 172:
		{
			UniqueId result = (UniqueId)node.TypedValue;
			Read();
			return result;
		}
		default:
			throw new InvalidOperationException("Current content is not a UniqueId");
		}
	}
}
