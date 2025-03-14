using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public abstract class Transform
{
	private string algo;

	private XmlResolver xmlResolver;

	private Hashtable propagated_namespaces = new Hashtable();

	public string Algorithm
	{
		get
		{
			return algo;
		}
		set
		{
			algo = value;
		}
	}

	public abstract Type[] InputTypes { get; }

	public abstract Type[] OutputTypes { get; }

	[ComVisible(false)]
	public XmlResolver Resolver
	{
		set
		{
			xmlResolver = value;
		}
	}

	[ComVisible(false)]
	[System.MonoTODO]
	public XmlElement Context
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	[ComVisible(false)]
	public Hashtable PropagatedNamespaces => propagated_namespaces;

	protected Transform()
	{
		if (SecurityManager.SecurityEnabled)
		{
			xmlResolver = new XmlSecureResolver(new XmlUrlResolver(), new Evidence());
		}
		else
		{
			xmlResolver = new XmlUrlResolver();
		}
	}

	[ComVisible(false)]
	public virtual byte[] GetDigestedOutput(HashAlgorithm hash)
	{
		return hash.ComputeHash((Stream)GetOutput(typeof(Stream)));
	}

	protected abstract XmlNodeList GetInnerXml();

	public abstract object GetOutput();

	public abstract object GetOutput(Type type);

	public XmlElement GetXml()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.XmlResolver = GetResolver();
		XmlElement xmlElement = xmlDocument.CreateElement("Transform", "http://www.w3.org/2000/09/xmldsig#");
		xmlElement.SetAttribute("Algorithm", algo);
		XmlNodeList innerXml = GetInnerXml();
		if (innerXml != null)
		{
			foreach (XmlNode item in innerXml)
			{
				XmlNode newChild = xmlDocument.ImportNode(item, deep: true);
				xmlElement.AppendChild(newChild);
			}
		}
		return xmlElement;
	}

	public abstract void LoadInnerXml(XmlNodeList nodeList);

	public abstract void LoadInput(object obj);

	internal XmlResolver GetResolver()
	{
		return xmlResolver;
	}
}
