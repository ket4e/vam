using System.Collections;
using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class XmlDsigEnvelopedSignatureTransform : Transform
{
	private Type[] input;

	private Type[] output;

	private bool comments;

	private object inputObj;

	public override Type[] InputTypes
	{
		get
		{
			if (input == null)
			{
				input = new Type[3];
				input[0] = typeof(Stream);
				input[1] = typeof(XmlDocument);
				input[2] = typeof(XmlNodeList);
			}
			return input;
		}
	}

	public override Type[] OutputTypes
	{
		get
		{
			if (output == null)
			{
				output = new Type[2];
				output[0] = typeof(XmlDocument);
				output[1] = typeof(XmlNodeList);
			}
			return output;
		}
	}

	public XmlDsigEnvelopedSignatureTransform()
		: this(includeComments: false)
	{
	}

	public XmlDsigEnvelopedSignatureTransform(bool includeComments)
	{
		base.Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
		comments = includeComments;
	}

	protected override XmlNodeList GetInnerXml()
	{
		return null;
	}

	public override object GetOutput()
	{
		XmlDocument xmlDocument = null;
		if (inputObj is Stream)
		{
			xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.XmlResolver = GetResolver();
			xmlDocument.Load(new XmlSignatureStreamReader(new StreamReader(inputObj as Stream)));
			return GetOutputFromNode(xmlDocument, GetNamespaceManager(xmlDocument), remove: true);
		}
		if (inputObj is XmlDocument)
		{
			xmlDocument = inputObj as XmlDocument;
			return GetOutputFromNode(xmlDocument, GetNamespaceManager(xmlDocument), remove: true);
		}
		if (inputObj is XmlNodeList)
		{
			ArrayList arrayList = new ArrayList();
			XmlNodeList xmlNodeList = (XmlNodeList)inputObj;
			if (xmlNodeList.Count > 0)
			{
				XmlNamespaceManager namespaceManager = GetNamespaceManager(xmlNodeList.Item(0));
				ArrayList arrayList2 = new ArrayList();
				foreach (XmlNode item in xmlNodeList)
				{
					arrayList2.Add(item);
				}
				foreach (XmlNode item2 in arrayList2)
				{
					if (item2.SelectNodes("ancestor-or-self::dsig:Signature", namespaceManager).Count == 0)
					{
						arrayList.Add(GetOutputFromNode(item2, namespaceManager, remove: false));
					}
				}
			}
			return new XmlDsigNodeList(arrayList);
		}
		if (inputObj is XmlElement)
		{
			XmlElement xmlElement = inputObj as XmlElement;
			XmlNamespaceManager namespaceManager2 = GetNamespaceManager(xmlElement);
			if (xmlElement.SelectNodes("ancestor-or-self::dsig:Signature", namespaceManager2).Count == 0)
			{
				return GetOutputFromNode(xmlElement, namespaceManager2, remove: true);
			}
		}
		throw new NullReferenceException();
	}

	private XmlNamespaceManager GetNamespaceManager(XmlNode n)
	{
		XmlDocument xmlDocument = ((!(n is XmlDocument)) ? n.OwnerDocument : (n as XmlDocument));
		XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
		xmlNamespaceManager.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
		return xmlNamespaceManager;
	}

	private XmlNode GetOutputFromNode(XmlNode input, XmlNamespaceManager nsmgr, bool remove)
	{
		if (remove)
		{
			XmlNodeList xmlNodeList = input.SelectNodes("descendant-or-self::dsig:Signature", nsmgr);
			ArrayList arrayList = new ArrayList();
			foreach (XmlNode item in xmlNodeList)
			{
				arrayList.Add(item);
			}
			foreach (XmlNode item2 in arrayList)
			{
				item2.ParentNode.RemoveChild(item2);
			}
		}
		return input;
	}

	public override object GetOutput(Type type)
	{
		if (type == typeof(Stream))
		{
			return GetOutput();
		}
		throw new ArgumentException("type");
	}

	public override void LoadInnerXml(XmlNodeList nodeList)
	{
	}

	public override void LoadInput(object obj)
	{
		inputObj = obj;
	}
}
