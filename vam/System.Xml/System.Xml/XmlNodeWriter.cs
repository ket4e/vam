namespace System.Xml;

internal class XmlNodeWriter : XmlWriter
{
	private XmlDocument doc;

	private bool isClosed;

	private XmlNode current;

	private XmlAttribute attribute;

	private bool isDocumentEntity;

	private XmlDocumentFragment fragment;

	private XmlNodeType state;

	public XmlNode Document => (!isDocumentEntity) ? ((XmlNode)fragment) : ((XmlNode)doc);

	public override WriteState WriteState
	{
		get
		{
			if (isClosed)
			{
				return WriteState.Closed;
			}
			if (attribute != null)
			{
				return WriteState.Attribute;
			}
			return state switch
			{
				XmlNodeType.None => WriteState.Start, 
				XmlNodeType.XmlDeclaration => WriteState.Prolog, 
				XmlNodeType.DocumentType => WriteState.Element, 
				_ => WriteState.Content, 
			};
		}
	}

	public override string XmlLang
	{
		get
		{
			for (XmlElement xmlElement = current as XmlElement; xmlElement != null; xmlElement = xmlElement.ParentNode as XmlElement)
			{
				if (xmlElement.HasAttribute("xml:lang"))
				{
					return xmlElement.GetAttribute("xml:lang");
				}
			}
			return string.Empty;
		}
	}

	public override XmlSpace XmlSpace
	{
		get
		{
			for (XmlElement xmlElement = current as XmlElement; xmlElement != null; xmlElement = xmlElement.ParentNode as XmlElement)
			{
				string text = xmlElement.GetAttribute("xml:space");
				switch (text)
				{
				case "preserve":
					return XmlSpace.Preserve;
				case "default":
					return XmlSpace.Default;
				default:
					throw new InvalidOperationException($"Invalid xml:space {text}.");
				case "":
					break;
				}
			}
			return XmlSpace.None;
		}
	}

	public XmlNodeWriter()
		: this(isDocumentEntity: true)
	{
	}

	public XmlNodeWriter(bool isDocumentEntity)
	{
		doc = new XmlDocument();
		state = XmlNodeType.None;
		this.isDocumentEntity = isDocumentEntity;
		if (!isDocumentEntity)
		{
			current = (fragment = doc.CreateDocumentFragment());
		}
	}

	private void CheckState()
	{
		if (isClosed)
		{
			throw new InvalidOperationException();
		}
	}

	private void WritePossiblyTopLevelNode(XmlNode n, bool possiblyAttribute)
	{
		CheckState();
		if (!possiblyAttribute && attribute != null)
		{
			throw new InvalidOperationException($"Current state is not acceptable for {n.NodeType}.");
		}
		if (state != XmlNodeType.Element)
		{
			Document.AppendChild(n);
		}
		else if (attribute != null)
		{
			attribute.AppendChild(n);
		}
		else
		{
			current.AppendChild(n);
		}
		if (state == XmlNodeType.None)
		{
			state = XmlNodeType.XmlDeclaration;
		}
	}

	public override void Close()
	{
		CheckState();
		isClosed = true;
	}

	public override void Flush()
	{
	}

	public override string LookupPrefix(string ns)
	{
		CheckState();
		if (current == null)
		{
			throw new InvalidOperationException();
		}
		return current.GetPrefixOfNamespace(ns);
	}

	public override void WriteStartDocument()
	{
		WriteStartDocument(null);
	}

	public override void WriteStartDocument(bool standalone)
	{
		WriteStartDocument((!standalone) ? "no" : "yes");
	}

	private void WriteStartDocument(string sddecl)
	{
		CheckState();
		if (state != 0)
		{
			throw new InvalidOperationException("Current state is not acceptable for xmldecl.");
		}
		doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, sddecl));
		state = XmlNodeType.XmlDeclaration;
	}

	public override void WriteEndDocument()
	{
		CheckState();
		isClosed = true;
	}

	public override void WriteDocType(string name, string publicId, string systemId, string internalSubset)
	{
		CheckState();
		XmlNodeType xmlNodeType = state;
		if (xmlNodeType == XmlNodeType.None || xmlNodeType == XmlNodeType.XmlDeclaration)
		{
			doc.AppendChild(doc.CreateDocumentType(name, publicId, systemId, internalSubset));
			state = XmlNodeType.DocumentType;
			return;
		}
		throw new InvalidOperationException("Current state is not acceptable for doctype.");
	}

	public override void WriteStartElement(string prefix, string name, string ns)
	{
		CheckState();
		if (isDocumentEntity && state == XmlNodeType.EndElement && doc.DocumentElement != null)
		{
			throw new InvalidOperationException("Current state is not acceptable for startElement.");
		}
		XmlElement newChild = doc.CreateElement(prefix, name, ns);
		if (current == null)
		{
			Document.AppendChild(newChild);
			state = XmlNodeType.Element;
		}
		else
		{
			current.AppendChild(newChild);
			state = XmlNodeType.Element;
		}
		current = newChild;
	}

	public override void WriteEndElement()
	{
		WriteEndElementInternal(forceFull: false);
	}

	public override void WriteFullEndElement()
	{
		WriteEndElementInternal(forceFull: true);
	}

	private void WriteEndElementInternal(bool forceFull)
	{
		CheckState();
		if (current == null)
		{
			throw new InvalidOperationException("Current state is not acceptable for endElement.");
		}
		if (!forceFull && current.FirstChild == null)
		{
			((XmlElement)current).IsEmpty = true;
		}
		if (isDocumentEntity && current.ParentNode == doc)
		{
			state = XmlNodeType.EndElement;
		}
		else
		{
			current = current.ParentNode;
		}
	}

	public override void WriteStartAttribute(string prefix, string name, string ns)
	{
		CheckState();
		if (attribute != null)
		{
			throw new InvalidOperationException("There is an open attribute.");
		}
		if (!(current is XmlElement))
		{
			throw new InvalidOperationException("Current state is not acceptable for startAttribute.");
		}
		attribute = doc.CreateAttribute(prefix, name, ns);
		((XmlElement)current).SetAttributeNode(attribute);
	}

	public override void WriteEndAttribute()
	{
		CheckState();
		if (attribute == null)
		{
			throw new InvalidOperationException("Current state is not acceptable for startAttribute.");
		}
		attribute = null;
	}

	public override void WriteCData(string data)
	{
		CheckState();
		if (current == null)
		{
			throw new InvalidOperationException("Current state is not acceptable for CDATAsection.");
		}
		current.AppendChild(doc.CreateCDataSection(data));
	}

	public override void WriteComment(string comment)
	{
		WritePossiblyTopLevelNode(doc.CreateComment(comment), possiblyAttribute: false);
	}

	public override void WriteProcessingInstruction(string name, string value)
	{
		WritePossiblyTopLevelNode(doc.CreateProcessingInstruction(name, value), possiblyAttribute: false);
	}

	public override void WriteEntityRef(string name)
	{
		WritePossiblyTopLevelNode(doc.CreateEntityReference(name), possiblyAttribute: true);
	}

	public override void WriteCharEntity(char c)
	{
		WritePossiblyTopLevelNode(doc.CreateTextNode(new string(new char[1] { c }, 0, 1)), possiblyAttribute: true);
	}

	public override void WriteWhitespace(string ws)
	{
		WritePossiblyTopLevelNode(doc.CreateWhitespace(ws), possiblyAttribute: true);
	}

	public override void WriteString(string data)
	{
		CheckState();
		if (current == null)
		{
			throw new InvalidOperationException("Current state is not acceptable for Text.");
		}
		if (attribute != null)
		{
			attribute.AppendChild(doc.CreateTextNode(data));
		}
		else if (!(current.LastChild is XmlText xmlText))
		{
			current.AppendChild(doc.CreateTextNode(data));
		}
		else
		{
			xmlText.AppendData(data);
		}
	}

	public override void WriteName(string name)
	{
		WriteString(name);
	}

	public override void WriteNmToken(string nmtoken)
	{
		WriteString(nmtoken);
	}

	public override void WriteQualifiedName(string name, string ns)
	{
		string text = LookupPrefix(ns);
		if (text == null)
		{
			throw new ArgumentException($"Invalid namespace {ns}");
		}
		if (text != string.Empty)
		{
			WriteString(name);
		}
		else
		{
			WriteString(text + ":" + name);
		}
	}

	public override void WriteChars(char[] chars, int start, int len)
	{
		WriteString(new string(chars, start, len));
	}

	public override void WriteRaw(string data)
	{
		WriteString(data);
	}

	public override void WriteRaw(char[] chars, int start, int len)
	{
		WriteChars(chars, start, len);
	}

	public override void WriteBase64(byte[] data, int start, int len)
	{
		WriteString(Convert.ToBase64String(data, start, len));
	}

	public override void WriteBinHex(byte[] data, int start, int len)
	{
		throw new NotImplementedException();
	}

	public override void WriteSurrogateCharEntity(char c1, char c2)
	{
		throw new NotImplementedException();
	}
}
