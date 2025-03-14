using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace Mono.Xml;

internal class XmlCanonicalizer
{
	private enum XmlCanonicalizerState
	{
		BeforeDocElement,
		InsideDocElement,
		AfterDocElement
	}

	private bool comments;

	private bool exclusive;

	private string inclusiveNamespacesPrefixList;

	private XmlNodeList xnl;

	private StringBuilder res;

	private XmlCanonicalizerState state;

	private ArrayList visibleNamespaces;

	private int prevVisibleNamespacesStart;

	private int prevVisibleNamespacesEnd;

	private Hashtable propagatedNss;

	public string InclusiveNamespacesPrefixList
	{
		get
		{
			return inclusiveNamespacesPrefixList;
		}
		set
		{
			inclusiveNamespacesPrefixList = value;
		}
	}

	public XmlCanonicalizer(bool withComments, bool excC14N, Hashtable propagatedNamespaces)
	{
		res = new StringBuilder();
		comments = withComments;
		exclusive = excC14N;
		propagatedNss = propagatedNamespaces;
	}

	private void Initialize()
	{
		state = XmlCanonicalizerState.BeforeDocElement;
		visibleNamespaces = new ArrayList();
		prevVisibleNamespacesStart = 0;
		prevVisibleNamespacesEnd = 0;
		res.Length = 0;
	}

	public Stream Canonicalize(XmlDocument doc)
	{
		if (doc == null)
		{
			throw new ArgumentNullException("doc");
		}
		Initialize();
		FillMissingPrefixes(doc, new XmlNamespaceManager(doc.NameTable), new ArrayList());
		WriteDocumentNode(doc);
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(res.ToString());
		return new MemoryStream(bytes);
	}

	public Stream Canonicalize(XmlNodeList nodes)
	{
		xnl = nodes;
		if (nodes == null || nodes.Count < 1)
		{
			return new MemoryStream();
		}
		XmlNode xmlNode = nodes[0];
		return Canonicalize((xmlNode.NodeType != XmlNodeType.Document) ? xmlNode.OwnerDocument : (xmlNode as XmlDocument));
	}

	private XmlAttribute CreateXmlns(XmlNode n)
	{
		XmlAttribute xmlAttribute = ((n.Prefix.Length != 0) ? n.OwnerDocument.CreateAttribute("xmlns", n.Prefix, "http://www.w3.org/2000/xmlns/") : n.OwnerDocument.CreateAttribute("xmlns", "http://www.w3.org/2000/xmlns/"));
		xmlAttribute.Value = n.NamespaceURI;
		return xmlAttribute;
	}

	private void FillMissingPrefixes(XmlNode n, XmlNamespaceManager nsmgr, ArrayList tmpList)
	{
		if (n.Prefix.Length == 0 && propagatedNss != null)
		{
			foreach (DictionaryEntry item in propagatedNss)
			{
				if ((string)item.Value == n.NamespaceURI)
				{
					n.Prefix = (string)item.Key;
					break;
				}
			}
		}
		if (n.NodeType == XmlNodeType.Element && ((XmlElement)n).HasAttributes)
		{
			foreach (XmlAttribute attribute in n.Attributes)
			{
				if (attribute.NamespaceURI == "http://www.w3.org/2000/xmlns/")
				{
					nsmgr.AddNamespace((attribute.Prefix.Length != 0) ? attribute.LocalName : string.Empty, attribute.Value);
				}
			}
			nsmgr.PushScope();
		}
		if (n.NamespaceURI.Length > 0 && nsmgr.LookupPrefix(n.NamespaceURI) == null)
		{
			tmpList.Add(CreateXmlns(n));
		}
		if (n.NodeType == XmlNodeType.Element && ((XmlElement)n).HasAttributes)
		{
			foreach (XmlAttribute attribute2 in n.Attributes)
			{
				if (attribute2.NamespaceURI.Length > 0 && nsmgr.LookupNamespace(attribute2.Prefix) == null)
				{
					tmpList.Add(CreateXmlns(attribute2));
				}
			}
		}
		foreach (XmlAttribute tmp in tmpList)
		{
			((XmlElement)n).SetAttributeNode(tmp);
		}
		tmpList.Clear();
		if (n.HasChildNodes)
		{
			for (XmlNode xmlNode = n.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					FillMissingPrefixes(xmlNode, nsmgr, tmpList);
				}
			}
		}
		nsmgr.PopScope();
	}

	private void WriteNode(XmlNode node)
	{
		bool visible = IsNodeVisible(node);
		switch (node.NodeType)
		{
		case XmlNodeType.Document:
		case XmlNodeType.DocumentFragment:
			WriteDocumentNode(node);
			break;
		case XmlNodeType.Element:
			WriteElementNode(node, visible);
			break;
		case XmlNodeType.Text:
		case XmlNodeType.CDATA:
		case XmlNodeType.SignificantWhitespace:
			WriteTextNode(node, visible);
			break;
		case XmlNodeType.Whitespace:
			if (state == XmlCanonicalizerState.InsideDocElement)
			{
				WriteTextNode(node, visible);
			}
			break;
		case XmlNodeType.Comment:
			WriteCommentNode(node, visible);
			break;
		case XmlNodeType.ProcessingInstruction:
			WriteProcessingInstructionNode(node, visible);
			break;
		case XmlNodeType.EntityReference:
		{
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				WriteNode(node.ChildNodes[i]);
			}
			break;
		}
		case XmlNodeType.Attribute:
			throw new XmlException("Attribute node is impossible here", null);
		case XmlNodeType.EndElement:
			throw new XmlException("EndElement node is impossible here", null);
		case XmlNodeType.EndEntity:
			throw new XmlException("EndEntity node is impossible here", null);
		case XmlNodeType.Entity:
		case XmlNodeType.DocumentType:
		case XmlNodeType.Notation:
		case XmlNodeType.XmlDeclaration:
			break;
		}
	}

	private void WriteDocumentNode(XmlNode node)
	{
		state = XmlCanonicalizerState.BeforeDocElement;
		for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			WriteNode(xmlNode);
		}
	}

	private void WriteElementNode(XmlNode node, bool visible)
	{
		int num = prevVisibleNamespacesStart;
		int num2 = prevVisibleNamespacesEnd;
		int count = visibleNamespaces.Count;
		XmlCanonicalizerState xmlCanonicalizerState = state;
		if (visible && state == XmlCanonicalizerState.BeforeDocElement)
		{
			state = XmlCanonicalizerState.InsideDocElement;
		}
		if (visible)
		{
			res.Append("<");
			res.Append(node.Name);
		}
		WriteNamespacesAxis(node, visible);
		WriteAttributesAxis(node);
		if (visible)
		{
			res.Append(">");
		}
		for (XmlNode xmlNode = node.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
		{
			WriteNode(xmlNode);
		}
		if (visible)
		{
			res.Append("</");
			res.Append(node.Name);
			res.Append(">");
		}
		if (visible && xmlCanonicalizerState == XmlCanonicalizerState.BeforeDocElement)
		{
			state = XmlCanonicalizerState.AfterDocElement;
		}
		prevVisibleNamespacesStart = num;
		prevVisibleNamespacesEnd = num2;
		if (visibleNamespaces.Count > count)
		{
			visibleNamespaces.RemoveRange(count, visibleNamespaces.Count - count);
		}
	}

	private void WriteNamespacesAxis(XmlNode node, bool visible)
	{
		XmlDocument ownerDocument = node.OwnerDocument;
		bool flag = false;
		ArrayList arrayList = new ArrayList();
		XmlNode xmlNode = node;
		while (xmlNode != null && xmlNode != ownerDocument)
		{
			foreach (XmlAttribute attribute in xmlNode.Attributes)
			{
				if (!IsNamespaceNode(attribute))
				{
					continue;
				}
				string text = string.Empty;
				if (attribute.Prefix == "xmlns")
				{
					text = attribute.LocalName;
				}
				if (text == "xml" && attribute.Value == "http://www.w3.org/XML/1998/namespace")
				{
					continue;
				}
				string namespaceOfPrefix = node.GetNamespaceOfPrefix(text);
				if (namespaceOfPrefix != attribute.Value || !IsNodeVisible(attribute))
				{
					continue;
				}
				bool flag2 = IsNamespaceRendered(text, attribute.Value);
				if (!exclusive || IsVisiblyUtilized(node as XmlElement, attribute))
				{
					if (visible)
					{
						visibleNamespaces.Add(attribute);
					}
					if (!flag2)
					{
						arrayList.Add(attribute);
					}
					if (text == string.Empty)
					{
						flag = true;
					}
				}
			}
			xmlNode = xmlNode.ParentNode;
		}
		if (visible && !flag && !IsNamespaceRendered(string.Empty, string.Empty) && node.NamespaceURI == string.Empty)
		{
			res.Append(" xmlns=\"\"");
		}
		arrayList.Sort(new XmlDsigC14NTransformNamespacesComparer());
		foreach (object item in arrayList)
		{
			if (item is XmlNode xmlNode2)
			{
				res.Append(" ");
				res.Append(xmlNode2.Name);
				res.Append("=\"");
				res.Append(xmlNode2.Value);
				res.Append("\"");
			}
		}
		if (visible)
		{
			prevVisibleNamespacesStart = prevVisibleNamespacesEnd;
			prevVisibleNamespacesEnd = visibleNamespaces.Count;
		}
	}

	private void WriteAttributesAxis(XmlNode node)
	{
		ArrayList arrayList = new ArrayList();
		foreach (XmlNode attribute in node.Attributes)
		{
			if (!IsNamespaceNode(attribute) && IsNodeVisible(attribute))
			{
				arrayList.Add(attribute);
			}
		}
		if (!exclusive && node.ParentNode != null && node.ParentNode.ParentNode != null && !IsNodeVisible(node.ParentNode.ParentNode))
		{
			for (XmlNode parentNode = node.ParentNode; parentNode != null; parentNode = parentNode.ParentNode)
			{
				if (parentNode.Attributes != null)
				{
					foreach (XmlNode attribute2 in parentNode.Attributes)
					{
						if (attribute2.Prefix != "xml" || node.Attributes.GetNamedItem(attribute2.LocalName, attribute2.NamespaceURI) != null)
						{
							continue;
						}
						bool flag = false;
						foreach (object item in arrayList)
						{
							XmlNode xmlNode3 = item as XmlNode;
							if (xmlNode3.Prefix == "xml" && xmlNode3.LocalName == attribute2.LocalName)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							arrayList.Add(attribute2);
						}
					}
				}
			}
		}
		arrayList.Sort(new XmlDsigC14NTransformAttributesComparer());
		foreach (object item2 in arrayList)
		{
			if (item2 is XmlNode xmlNode4)
			{
				res.Append(" ");
				res.Append(xmlNode4.Name);
				res.Append("=\"");
				res.Append(NormalizeString(xmlNode4.Value, XmlNodeType.Attribute));
				res.Append("\"");
			}
		}
	}

	private void WriteTextNode(XmlNode node, bool visible)
	{
		if (visible)
		{
			res.Append(NormalizeString(node.Value, node.NodeType));
		}
	}

	private void WriteCommentNode(XmlNode node, bool visible)
	{
		if (visible && comments)
		{
			if (state == XmlCanonicalizerState.AfterDocElement)
			{
				res.Append("\n<!--");
			}
			else
			{
				res.Append("<!--");
			}
			res.Append(NormalizeString(node.Value, XmlNodeType.Comment));
			if (state == XmlCanonicalizerState.BeforeDocElement)
			{
				res.Append("-->\n");
			}
			else
			{
				res.Append("-->");
			}
		}
	}

	private void WriteProcessingInstructionNode(XmlNode node, bool visible)
	{
		if (visible)
		{
			if (state == XmlCanonicalizerState.AfterDocElement)
			{
				res.Append("\n<?");
			}
			else
			{
				res.Append("<?");
			}
			res.Append(node.Name);
			if (node.Value.Length > 0)
			{
				res.Append(" ");
				res.Append(NormalizeString(node.Value, XmlNodeType.ProcessingInstruction));
			}
			if (state == XmlCanonicalizerState.BeforeDocElement)
			{
				res.Append("?>\n");
			}
			else
			{
				res.Append("?>");
			}
		}
	}

	private bool IsNodeVisible(XmlNode node)
	{
		if (xnl == null)
		{
			return true;
		}
		foreach (XmlNode item in xnl)
		{
			if (node.Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsVisiblyUtilized(XmlElement owner, XmlAttribute ns)
	{
		if (owner == null)
		{
			return false;
		}
		string text = ((!(ns.LocalName == "xmlns")) ? ns.LocalName : string.Empty);
		if (owner.Prefix == text && owner.NamespaceURI == ns.Value)
		{
			return true;
		}
		if (!owner.HasAttributes)
		{
			return false;
		}
		foreach (XmlAttribute attribute in owner.Attributes)
		{
			if (attribute.Prefix == string.Empty || attribute.Prefix != text || attribute.NamespaceURI != ns.Value || !IsNodeVisible(attribute))
			{
				continue;
			}
			return true;
		}
		return false;
	}

	private bool IsNamespaceRendered(string prefix, string uri)
	{
		bool flag = prefix == string.Empty && uri == string.Empty;
		int num = ((!flag) ? prevVisibleNamespacesStart : 0);
		for (int num2 = visibleNamespaces.Count - 1; num2 >= num; num2--)
		{
			if (visibleNamespaces[num2] is XmlNode xmlNode)
			{
				string text = string.Empty;
				if (xmlNode.Prefix == "xmlns")
				{
					text = xmlNode.LocalName;
				}
				if (text == prefix)
				{
					return xmlNode.Value == uri;
				}
			}
		}
		return flag;
	}

	private bool IsNamespaceNode(XmlNode node)
	{
		if (node == null || node.NodeType != XmlNodeType.Attribute)
		{
			return false;
		}
		return node.NamespaceURI == "http://www.w3.org/2000/xmlns/";
	}

	private bool IsTextNode(XmlNodeType type)
	{
		if (type == XmlNodeType.Text || type == XmlNodeType.CDATA || type == XmlNodeType.Whitespace || type == XmlNodeType.SignificantWhitespace)
		{
			return true;
		}
		return false;
	}

	private string NormalizeString(string input, XmlNodeType type)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in input)
		{
			if (c == '<' && (type == XmlNodeType.Attribute || IsTextNode(type)))
			{
				stringBuilder.Append("&lt;");
			}
			else if (c == '>' && IsTextNode(type))
			{
				stringBuilder.Append("&gt;");
			}
			else if (c == '&' && (type == XmlNodeType.Attribute || IsTextNode(type)))
			{
				stringBuilder.Append("&amp;");
			}
			else if (c == '"' && type == XmlNodeType.Attribute)
			{
				stringBuilder.Append("&quot;");
			}
			else if (c == '\t' && type == XmlNodeType.Attribute)
			{
				stringBuilder.Append("&#x9;");
			}
			else if (c == '\n' && type == XmlNodeType.Attribute)
			{
				stringBuilder.Append("&#xA;");
			}
			else if (c == '\r')
			{
				stringBuilder.Append("&#xD;");
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}
}
