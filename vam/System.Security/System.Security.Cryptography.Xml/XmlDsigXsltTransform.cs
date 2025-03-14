using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace System.Security.Cryptography.Xml;

public class XmlDsigXsltTransform : Transform
{
	private Type[] input;

	private Type[] output;

	private bool comments;

	private XmlNodeList xnl;

	private XmlDocument inputDoc;

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

	public XmlDsigXsltTransform()
		: this(includeComments: false)
	{
	}

	public XmlDsigXsltTransform(bool includeComments)
	{
		comments = includeComments;
		base.Algorithm = "http://www.w3.org/TR/1999/REC-xslt-19991116";
	}

	protected override XmlNodeList GetInnerXml()
	{
		return xnl;
	}

	public override object GetOutput()
	{
		if (xnl == null)
		{
			throw new ArgumentNullException("LoadInnerXml before transformation.");
		}
		XmlResolver resolver = GetResolver();
		XslTransform xslTransform = new XslTransform();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.XmlResolver = resolver;
		foreach (XmlNode item in xnl)
		{
			xmlDocument.AppendChild(xmlDocument.ImportNode(item, deep: true));
		}
		xslTransform.Load(xmlDocument, resolver);
		if (inputDoc == null)
		{
			throw new ArgumentNullException("LoadInput before transformation.");
		}
		MemoryStream memoryStream = new MemoryStream();
		xslTransform.XmlResolver = resolver;
		xslTransform.Transform(inputDoc, null, memoryStream);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return memoryStream;
	}

	public override object GetOutput(Type type)
	{
		if (type != typeof(Stream))
		{
			throw new ArgumentException("type");
		}
		return GetOutput();
	}

	public override void LoadInnerXml(XmlNodeList nodeList)
	{
		if (nodeList == null)
		{
			throw new CryptographicException("nodeList");
		}
		xnl = nodeList;
	}

	public override void LoadInput(object obj)
	{
		if (obj is Stream stream)
		{
			inputDoc = new XmlDocument();
			inputDoc.XmlResolver = GetResolver();
			inputDoc.Load(new XmlSignatureStreamReader(new StreamReader(stream)));
		}
		else if (obj is XmlDocument xmlDocument)
		{
			inputDoc = xmlDocument;
		}
		else if (obj is XmlNodeList xmlNodeList)
		{
			inputDoc = new XmlDocument();
			inputDoc.XmlResolver = GetResolver();
			for (int i = 0; i < xmlNodeList.Count; i++)
			{
				inputDoc.AppendChild(inputDoc.ImportNode(xmlNodeList[i], deep: true));
			}
		}
	}
}
