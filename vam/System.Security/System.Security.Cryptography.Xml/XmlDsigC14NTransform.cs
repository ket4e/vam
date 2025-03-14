using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Mono.Xml;

namespace System.Security.Cryptography.Xml;

public class XmlDsigC14NTransform : Transform
{
	private Type[] input;

	private Type[] output;

	private XmlCanonicalizer canonicalizer;

	private Stream s;

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

	public XmlDsigC14NTransform()
		: this(includeComments: false)
	{
	}

	public XmlDsigC14NTransform(bool includeComments)
	{
		if (includeComments)
		{
			base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";
		}
		else
		{
			base.Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
		}
		canonicalizer = new XmlCanonicalizer(includeComments, excC14N: false, base.PropagatedNamespaces);
	}

	protected override XmlNodeList GetInnerXml()
	{
		return null;
	}

	[ComVisible(false)]
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
