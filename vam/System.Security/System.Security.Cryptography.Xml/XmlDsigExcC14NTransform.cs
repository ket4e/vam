using System.IO;
using System.Xml;
using Mono.Xml;

namespace System.Security.Cryptography.Xml;

public class XmlDsigExcC14NTransform : Transform
{
	private Type[] input;

	private Type[] output;

	private XmlCanonicalizer canonicalizer;

	private Stream s;

	private string inclusiveNamespacesPrefixList;

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
				output = new Type[1];
				output[0] = typeof(Stream);
			}
			return output;
		}
	}

	public XmlDsigExcC14NTransform()
		: this(includeComments: false, null)
	{
	}

	public XmlDsigExcC14NTransform(bool includeComments)
		: this(includeComments, null)
	{
	}

	public XmlDsigExcC14NTransform(string inclusiveNamespacesPrefixList)
		: this(includeComments: false, inclusiveNamespacesPrefixList)
	{
	}

	public XmlDsigExcC14NTransform(bool includeComments, string inclusiveNamespacesPrefixList)
	{
		if (includeComments)
		{
			base.Algorithm = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";
		}
		else
		{
			base.Algorithm = "http://www.w3.org/2001/10/xml-exc-c14n#";
		}
		this.inclusiveNamespacesPrefixList = inclusiveNamespacesPrefixList;
		canonicalizer = new XmlCanonicalizer(includeComments, excC14N: true, base.PropagatedNamespaces);
	}

	protected override XmlNodeList GetInnerXml()
	{
		return null;
	}

	public override byte[] GetDigestedOutput(HashAlgorithm hash)
	{
		return hash.ComputeHash((Stream)GetOutput());
	}

	public override object GetOutput()
	{
		return s;
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
		canonicalizer.InclusiveNamespacesPrefixList = InclusiveNamespacesPrefixList;
		if (obj is Stream stream)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.PreserveWhitespace = true;
			xmlDocument.XmlResolver = GetResolver();
			xmlDocument.Load(new XmlSignatureStreamReader(new StreamReader(stream)));
			s = canonicalizer.Canonicalize(xmlDocument);
		}
		else if (obj is XmlDocument doc)
		{
			s = canonicalizer.Canonicalize(doc);
		}
		else
		{
			if (!(obj is XmlNodeList nodes))
			{
				throw new ArgumentException("obj");
			}
			s = canonicalizer.Canonicalize(nodes);
		}
	}
}
