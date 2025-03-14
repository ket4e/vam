using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class KeyInfo : IEnumerable
{
	private ArrayList Info;

	private string id;

	public int Count => Info.Count;

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public KeyInfo()
	{
		Info = new ArrayList();
	}

	public void AddClause(KeyInfoClause clause)
	{
		Info.Add(clause);
	}

	public IEnumerator GetEnumerator()
	{
		return Info.GetEnumerator();
	}

	public IEnumerator GetEnumerator(Type requestedObjectType)
	{
		ArrayList arrayList = new ArrayList();
		IEnumerator enumerator = Info.GetEnumerator();
		do
		{
			if (enumerator.Current.GetType().Equals(requestedObjectType))
			{
				arrayList.Add(enumerator.Current);
			}
		}
		while (enumerator.MoveNext());
		return arrayList.GetEnumerator();
	}

	public XmlElement GetXml()
	{
		XmlDocument xmlDocument = new XmlDocument();
		XmlElement xmlElement = xmlDocument.CreateElement("KeyInfo", "http://www.w3.org/2000/09/xmldsig#");
		foreach (KeyInfoClause item in Info)
		{
			XmlNode xml = item.GetXml();
			XmlNode newChild = xmlDocument.ImportNode(xml, deep: true);
			xmlElement.AppendChild(newChild);
		}
		return xmlElement;
	}

	public void LoadXml(XmlElement value)
	{
		if (value == null)
		{
			throw new ArgumentNullException("value");
		}
		Id = ((value.Attributes["Id"] == null) ? null : value.GetAttribute("Id"));
		if (!(value.LocalName == "KeyInfo") || !(value.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#"))
		{
			return;
		}
		foreach (XmlNode childNode in value.ChildNodes)
		{
			if (childNode.NodeType != XmlNodeType.Element)
			{
				continue;
			}
			KeyInfoClause keyInfoClause = null;
			switch (childNode.LocalName)
			{
			case "KeyValue":
			{
				XmlNodeList childNodes = childNode.ChildNodes;
				if (childNodes.Count <= 0)
				{
					break;
				}
				foreach (XmlNode item in childNodes)
				{
					switch (item.LocalName)
					{
					case "DSAKeyValue":
						keyInfoClause = new DSAKeyValue();
						break;
					case "RSAKeyValue":
						keyInfoClause = new RSAKeyValue();
						break;
					}
				}
				break;
			}
			case "KeyName":
				keyInfoClause = new KeyInfoName();
				break;
			case "RetrievalMethod":
				keyInfoClause = new KeyInfoRetrievalMethod();
				break;
			case "X509Data":
				keyInfoClause = new KeyInfoX509Data();
				break;
			case "RSAKeyValue":
				keyInfoClause = new RSAKeyValue();
				break;
			case "EncryptedKey":
				keyInfoClause = new KeyInfoEncryptedKey();
				break;
			default:
				keyInfoClause = new KeyInfoNode();
				break;
			}
			if (keyInfoClause != null)
			{
				keyInfoClause.LoadXml((XmlElement)childNode);
				AddClause(keyInfoClause);
			}
		}
	}
}
