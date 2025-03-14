using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace System.Xml;

public abstract class XmlDictionaryReader : XmlReader
{
	private XmlDictionaryReaderQuotas quotas;

	private MethodInfo xmlconv_from_bin_hex = typeof(XmlConvert).GetMethod("FromBinHexString", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[1] { typeof(string) }, null);

	private static readonly char[] wsChars = new char[4] { ' ', '\t', '\n', '\r' };

	public virtual bool CanCanonicalize => false;

	public virtual XmlDictionaryReaderQuotas Quotas
	{
		get
		{
			if (quotas == null)
			{
				quotas = new XmlDictionaryReaderQuotas();
			}
			return quotas;
		}
	}

	public virtual void EndCanonicalization()
	{
		throw new NotSupportedException();
	}

	public virtual string GetAttribute(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		return GetAttribute(localName.Value, namespaceUri.Value);
	}

	public virtual int IndexOfLocalName(string[] localNames, string namespaceUri)
	{
		if (localNames == null)
		{
			throw new ArgumentNullException("localNames");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		if (NamespaceURI != namespaceUri)
		{
			return -1;
		}
		for (int i = 0; i < localNames.Length; i++)
		{
			if (localNames[i] == LocalName)
			{
				return i;
			}
		}
		return -1;
	}

	public virtual int IndexOfLocalName(XmlDictionaryString[] localNames, XmlDictionaryString namespaceUri)
	{
		if (localNames == null)
		{
			throw new ArgumentNullException("localNames");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		if (NamespaceURI != namespaceUri.Value)
		{
			return -1;
		}
		if (!TryGetLocalNameAsDictionaryString(out var localName))
		{
			return -1;
		}
		IXmlDictionary dictionary = localName.Dictionary;
		for (int i = 0; i < localNames.Length; i++)
		{
			if (dictionary.TryLookup(localNames[i], out var result) && object.ReferenceEquals(result, localName))
			{
				return i;
			}
		}
		return -1;
	}

	public virtual bool IsArray(out Type type)
	{
		type = null;
		return false;
	}

	public virtual bool IsLocalName(string localName)
	{
		return LocalName == localName;
	}

	public virtual bool IsLocalName(XmlDictionaryString localName)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		return LocalName == localName.Value;
	}

	public virtual bool IsNamespaceUri(string namespaceUri)
	{
		return NamespaceURI == namespaceUri;
	}

	public virtual bool IsNamespaceUri(XmlDictionaryString namespaceUri)
	{
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		return NamespaceURI == namespaceUri.Value;
	}

	public virtual bool IsStartArray(out Type type)
	{
		type = null;
		return false;
	}

	public virtual bool IsStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		return IsStartElement(localName.Value, namespaceUri.Value);
	}

	protected bool IsTextNode(XmlNodeType nodeType)
	{
		switch (nodeType)
		{
		case XmlNodeType.Attribute:
		case XmlNodeType.Text:
		case XmlNodeType.CDATA:
		case XmlNodeType.Whitespace:
		case XmlNodeType.SignificantWhitespace:
			return true;
		default:
			return false;
		}
	}

	private XmlException XmlError(string message)
	{
		if (!(this is IXmlLineInfo xmlLineInfo) || !xmlLineInfo.HasLineInfo())
		{
			return new XmlException(message);
		}
		return new XmlException($"{message} in {BaseURI} , at ({xmlLineInfo.LineNumber},{xmlLineInfo.LinePosition})");
	}

	public virtual void MoveToStartElement()
	{
		MoveToContent();
		if (NodeType != XmlNodeType.Element)
		{
			throw XmlError($"Element node is expected, but got {NodeType} node.");
		}
	}

	public virtual void MoveToStartElement(string name)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		MoveToStartElement();
		if (Name != name)
		{
			throw XmlError($"Element node '{name}' is expected, but got '{Name}' element.");
		}
	}

	public virtual void MoveToStartElement(string localName, string namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		MoveToStartElement();
		if (LocalName != localName || NamespaceURI != namespaceUri)
		{
			throw XmlError($"Element node '{localName}' in namespace '{namespaceUri}' is expected, but got '{LocalName}' in namespace '{NamespaceURI}' element.");
		}
	}

	public virtual void MoveToStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		MoveToStartElement(localName.Value, namespaceUri.Value);
	}

	public virtual void StartCanonicalization(Stream stream, bool includeComments, string[] inclusivePrefixes)
	{
		throw new NotSupportedException();
	}

	public virtual bool TryGetArrayLength(out int count)
	{
		count = -1;
		return false;
	}

	public virtual bool TryGetBase64ContentLength(out int count)
	{
		count = -1;
		return false;
	}

	public virtual bool TryGetLocalNameAsDictionaryString(out XmlDictionaryString localName)
	{
		localName = null;
		return false;
	}

	public virtual bool TryGetNamespaceUriAsDictionaryString(out XmlDictionaryString namespaceUri)
	{
		namespaceUri = null;
		return false;
	}

	public override object ReadContentAs(Type type, IXmlNamespaceResolver nsResolver)
	{
		return base.ReadContentAs(type, nsResolver);
	}

	public virtual byte[] ReadContentAsBase64()
	{
		if (!TryGetBase64ContentLength(out var count))
		{
			return Convert.FromBase64String(ReadContentAsString());
		}
		byte[] array = new byte[count];
		ReadContentAsBase64(array, 0, count);
		return array;
	}

	private byte[] FromBinHexString(string s)
	{
		return (byte[])xmlconv_from_bin_hex.Invoke(null, new object[1] { s });
	}

	public virtual byte[] ReadContentAsBinHex()
	{
		if (!TryGetArrayLength(out var count))
		{
			return FromBinHexString(ReadContentAsString());
		}
		return ReadContentAsBinHex(count);
	}

	protected byte[] ReadContentAsBinHex(int maxByteArrayContentLength)
	{
		byte[] array = new byte[maxByteArrayContentLength];
		ReadContentAsBinHex(array, 0, maxByteArrayContentLength);
		return array;
	}

	[System.MonoTODO]
	public virtual int ReadContentAsChars(char[] chars, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override decimal ReadContentAsDecimal()
	{
		return base.ReadContentAsDecimal();
	}

	public override float ReadContentAsFloat()
	{
		return base.ReadContentAsFloat();
	}

	public virtual Guid ReadContentAsGuid()
	{
		return XmlConvert.ToGuid(ReadContentAsString());
	}

	public virtual void ReadContentAsQualifiedName(out string localName, out string namespaceUri)
	{
		XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)ReadContentAs(typeof(XmlQualifiedName), this as IXmlNamespaceResolver);
		localName = xmlQualifiedName.Name;
		namespaceUri = xmlQualifiedName.Namespace;
	}

	public override string ReadContentAsString()
	{
		return ReadContentAsString(Quotas.MaxStringContentLength);
	}

	[System.MonoTODO]
	protected string ReadContentAsString(int maxStringContentLength)
	{
		return base.ReadContentAsString();
	}

	[System.MonoTODO("there is exactly no information on the web")]
	public virtual string ReadContentAsString(string[] strings, out int index)
	{
		throw new NotImplementedException();
	}

	[System.MonoTODO("there is exactly no information on the web")]
	public virtual string ReadContentAsString(XmlDictionaryString[] strings, out int index)
	{
		throw new NotImplementedException();
	}

	public virtual TimeSpan ReadContentAsTimeSpan()
	{
		return XmlConvert.ToTimeSpan(ReadContentAsString());
	}

	public virtual UniqueId ReadContentAsUniqueId()
	{
		return new UniqueId(ReadContentAsString());
	}

	public virtual byte[] ReadElementContentAsBase64()
	{
		ReadStartElement();
		byte[] result = ReadContentAsBase64();
		ReadEndElement();
		return result;
	}

	public virtual byte[] ReadElementContentAsBinHex()
	{
		ReadStartElement();
		byte[] result = ReadContentAsBinHex();
		ReadEndElement();
		return result;
	}

	public virtual Guid ReadElementContentAsGuid()
	{
		ReadStartElement();
		Guid result = ReadContentAsGuid();
		ReadEndElement();
		return result;
	}

	public virtual TimeSpan ReadElementContentAsTimeSpan()
	{
		ReadStartElement();
		TimeSpan result = ReadContentAsTimeSpan();
		ReadEndElement();
		return result;
	}

	public virtual UniqueId ReadElementContentAsUniqueId()
	{
		ReadStartElement();
		UniqueId result = ReadContentAsUniqueId();
		ReadEndElement();
		return result;
	}

	public override string ReadElementContentAsString()
	{
		if (IsEmptyElement)
		{
			Read();
			return string.Empty;
		}
		ReadStartElement();
		string result = ((NodeType != XmlNodeType.EndElement) ? ReadContentAsString() : string.Empty);
		ReadEndElement();
		return result;
	}

	public virtual void ReadFullStartElement()
	{
		if (!IsStartElement())
		{
			throw new XmlException("Current node is not a start element");
		}
		ReadStartElement();
	}

	public virtual void ReadFullStartElement(string name)
	{
		if (!IsStartElement(name))
		{
			throw new XmlException($"Current node is not a start element '{name}'");
		}
		ReadStartElement(name);
	}

	public virtual void ReadFullStartElement(string localName, string namespaceUri)
	{
		if (!IsStartElement(localName, namespaceUri))
		{
			throw new XmlException($"Current node is not a start element '{localName}' in namesapce '{namespaceUri}'");
		}
		ReadStartElement(localName, namespaceUri);
	}

	public virtual void ReadFullStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (!IsStartElement(localName, namespaceUri))
		{
			throw new XmlException($"Current node is not a start element '{localName}' in namesapce '{namespaceUri}'");
		}
		ReadStartElement(localName.Value, namespaceUri.Value);
	}

	public virtual void ReadStartElement(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		if (localName == null)
		{
			throw new ArgumentNullException("localName");
		}
		if (namespaceUri == null)
		{
			throw new ArgumentNullException("namespaceUri");
		}
		ReadStartElement(localName.Value, namespaceUri.Value);
	}

	public override string ReadString()
	{
		return ReadString(Quotas.MaxStringContentLength);
	}

	[System.MonoTODO]
	protected string ReadString(int maxStringContentLength)
	{
		return base.ReadString();
	}

	public virtual int ReadValueAsBase64(byte[] bytes, int start, int length)
	{
		throw new NotSupportedException();
	}

	public virtual bool TryGetValueAsDictionaryString(out XmlDictionaryString value)
	{
		throw new NotSupportedException();
	}

	public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
	{
		return CreateBinaryReader(buffer, 0, buffer.Length, quotas);
	}

	public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
	{
		return CreateBinaryReader(buffer, offset, count, new XmlDictionary(), quotas);
	}

	public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas)
	{
		return CreateBinaryReader(buffer, offset, count, dictionary, quotas, new XmlBinaryReaderSession(), null);
	}

	public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session)
	{
		return CreateBinaryReader(buffer, offset, count, dictionary, quotas, session, null);
	}

	public static XmlDictionaryReader CreateBinaryReader(byte[] buffer, int offset, int count, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
	{
		return new XmlBinaryDictionaryReader(buffer, offset, count, dictionary, quotas, session, onClose);
	}

	public static XmlDictionaryReader CreateBinaryReader(Stream stream, XmlDictionaryReaderQuotas quotas)
	{
		return CreateBinaryReader(stream, new XmlDictionary(), quotas);
	}

	public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas)
	{
		return CreateBinaryReader(stream, dictionary, quotas, new XmlBinaryReaderSession(), null);
	}

	public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session)
	{
		return CreateBinaryReader(stream, dictionary, quotas, session, null);
	}

	public static XmlDictionaryReader CreateBinaryReader(Stream stream, IXmlDictionary dictionary, XmlDictionaryReaderQuotas quotas, XmlBinaryReaderSession session, OnXmlDictionaryReaderClose onClose)
	{
		return new XmlBinaryDictionaryReader(stream, dictionary, quotas, session, onClose);
	}

	public static XmlDictionaryReader CreateDictionaryReader(XmlReader reader)
	{
		return new XmlSimpleDictionaryReader(reader);
	}

	public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas)
	{
		return new XmlMtomDictionaryReader(stream, encoding, quotas);
	}

	public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding[] encodings, XmlDictionaryReaderQuotas quotas)
	{
		return CreateMtomReader(stream, encodings, null, quotas);
	}

	public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas)
	{
		return CreateMtomReader(stream, encodings, contentType, quotas, int.MaxValue, null);
	}

	public static XmlDictionaryReader CreateMtomReader(Stream stream, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
	{
		return new XmlMtomDictionaryReader(stream, encodings, contentType, quotas, maxBufferSize, onClose);
	}

	public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas)
	{
		return CreateMtomReader(new MemoryStream(buffer, offset, count), encoding, quotas);
	}

	public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, XmlDictionaryReaderQuotas quotas)
	{
		return CreateMtomReader(new MemoryStream(buffer, offset, count), encodings, quotas);
	}

	public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas)
	{
		return CreateMtomReader(new MemoryStream(buffer, offset, count), encodings, contentType, quotas);
	}

	public static XmlDictionaryReader CreateMtomReader(byte[] buffer, int offset, int count, Encoding[] encodings, string contentType, XmlDictionaryReaderQuotas quotas, int maxBufferSize, OnXmlDictionaryReaderClose onClose)
	{
		return CreateMtomReader(new MemoryStream(buffer, offset, count), encodings, contentType, quotas, maxBufferSize, onClose);
	}

	public static XmlDictionaryReader CreateTextReader(byte[] buffer, XmlDictionaryReaderQuotas quotas)
	{
		return CreateTextReader(buffer, 0, buffer.Length, quotas);
	}

	public static XmlDictionaryReader CreateTextReader(byte[] buffer, int offset, int count, XmlDictionaryReaderQuotas quotas)
	{
		return CreateTextReader(buffer, offset, count, Encoding.UTF8, quotas, null);
	}

	public static XmlDictionaryReader CreateTextReader(byte[] buffer, int offset, int count, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
	{
		return CreateTextReader(new MemoryStream(buffer, offset, count), encoding, quotas, onClose);
	}

	public static XmlDictionaryReader CreateTextReader(Stream stream, XmlDictionaryReaderQuotas quotas)
	{
		return CreateTextReader(stream, Encoding.UTF8, quotas, null);
	}

	public static XmlDictionaryReader CreateTextReader(Stream stream, Encoding encoding, XmlDictionaryReaderQuotas quotas, OnXmlDictionaryReaderClose onClose)
	{
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		XmlNameTable xmlNameTable = new NameTable();
		XmlParserContext context = new XmlParserContext(xmlNameTable, new XmlNamespaceManager(xmlNameTable), string.Empty, XmlSpace.None, encoding);
		XmlDictionaryReader xmlDictionaryReader = new XmlSimpleDictionaryReader(XmlReader.Create(stream, xmlReaderSettings, context), null, onClose);
		xmlDictionaryReader.quotas = quotas;
		return xmlDictionaryReader;
	}

	private void CheckReadArrayArguments(Array array, int offset, int length)
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

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, bool[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, bool[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			array[offset + i] = XmlConvert.ToBoolean(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual bool[] ReadBooleanArray(string localName, string namespaceUri)
	{
		List<bool> list = new List<bool>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToBoolean(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual bool[] ReadBooleanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadBooleanArray(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, DateTime[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, DateTime[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			ref DateTime reference = ref array[offset + i];
			reference = XmlConvert.ToDateTime(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual DateTime[] ReadDateTimeArray(string localName, string namespaceUri)
	{
		List<DateTime> list = new List<DateTime>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToDateTime(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual DateTime[] ReadDateTimeArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadDateTimeArray(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, decimal[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, decimal[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			ref decimal reference = ref array[offset + i];
			reference = XmlConvert.ToDecimal(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual decimal[] ReadDecimalArray(string localName, string namespaceUri)
	{
		List<decimal> list = new List<decimal>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToDecimal(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual decimal[] ReadDecimalArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadDecimalArray(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, double[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, double[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			array[offset + i] = XmlConvert.ToDouble(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual double[] ReadDoubleArray(string localName, string namespaceUri)
	{
		List<double> list = new List<double>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToDouble(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual double[] ReadDoubleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadDoubleArray(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, Guid[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, Guid[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			ref Guid reference = ref array[offset + i];
			reference = XmlConvert.ToGuid(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual Guid[] ReadGuidArray(string localName, string namespaceUri)
	{
		List<Guid> list = new List<Guid>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToGuid(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual Guid[] ReadGuidArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadGuidArray(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, short[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, short[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			array[offset + i] = XmlConvert.ToInt16(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual short[] ReadInt16Array(string localName, string namespaceUri)
	{
		List<short> list = new List<short>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToInt16(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual short[] ReadInt16Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadInt16Array(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, int[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, int[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			array[offset + i] = XmlConvert.ToInt32(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual int[] ReadInt32Array(string localName, string namespaceUri)
	{
		List<int> list = new List<int>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToInt32(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual int[] ReadInt32Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadInt32Array(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, long[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, long[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			array[offset + i] = XmlConvert.ToInt64(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual long[] ReadInt64Array(string localName, string namespaceUri)
	{
		List<long> list = new List<long>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToInt64(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual long[] ReadInt64Array(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadInt64Array(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, float[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, float[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			array[offset + i] = XmlConvert.ToSingle(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual float[] ReadSingleArray(string localName, string namespaceUri)
	{
		List<float> list = new List<float>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToSingle(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual float[] ReadSingleArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadSingleArray(localName.Value, namespaceUri.Value);
	}

	public virtual int ReadArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri, TimeSpan[] array, int offset, int length)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadArray(localName.Value, namespaceUri.Value, array, offset, length);
	}

	public virtual int ReadArray(string localName, string namespaceUri, TimeSpan[] array, int offset, int length)
	{
		CheckReadArrayArguments(array, offset, length);
		for (int i = 0; i < length; i++)
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				return i;
			}
			ReadStartElement(localName, namespaceUri);
			ref TimeSpan reference = ref array[offset + i];
			reference = XmlConvert.ToTimeSpan(ReadContentAsString());
			ReadEndElement();
		}
		return length;
	}

	public virtual TimeSpan[] ReadTimeSpanArray(string localName, string namespaceUri)
	{
		List<TimeSpan> list = new List<TimeSpan>();
		do
		{
			MoveToContent();
			if (NodeType != XmlNodeType.Element)
			{
				break;
			}
			ReadStartElement(localName, namespaceUri);
			list.Add(XmlConvert.ToTimeSpan(ReadContentAsString()));
			ReadEndElement();
		}
		while (list.Count != Quotas.MaxArrayLength);
		return list.ToArray();
	}

	public virtual TimeSpan[] ReadTimeSpanArray(XmlDictionaryString localName, XmlDictionaryString namespaceUri)
	{
		CheckDictionaryStringArgs(localName, namespaceUri);
		return ReadTimeSpanArray(localName.Value, namespaceUri.Value);
	}

	public override bool ReadElementContentAsBoolean()
	{
		ReadStartElement(LocalName, NamespaceURI);
		bool result = ReadContentAsBoolean();
		ReadEndElement();
		return result;
	}

	public override DateTime ReadElementContentAsDateTime()
	{
		ReadStartElement(LocalName, NamespaceURI);
		DateTime result = ReadContentAsDateTime();
		ReadEndElement();
		return result;
	}

	public override decimal ReadElementContentAsDecimal()
	{
		ReadStartElement(LocalName, NamespaceURI);
		decimal result = ReadContentAsDecimal();
		ReadEndElement();
		return result;
	}

	public override double ReadElementContentAsDouble()
	{
		ReadStartElement(LocalName, NamespaceURI);
		double result = ReadContentAsDouble();
		ReadEndElement();
		return result;
	}

	public override float ReadElementContentAsFloat()
	{
		ReadStartElement(LocalName, NamespaceURI);
		float result = ReadContentAsFloat();
		ReadEndElement();
		return result;
	}

	public override int ReadElementContentAsInt()
	{
		ReadStartElement(LocalName, NamespaceURI);
		int result = ReadContentAsInt();
		ReadEndElement();
		return result;
	}

	public override long ReadElementContentAsLong()
	{
		ReadStartElement(LocalName, NamespaceURI);
		long result = ReadContentAsLong();
		ReadEndElement();
		return result;
	}
}
