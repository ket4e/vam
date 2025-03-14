using System.IO;
using System.Xml;

namespace System.Security.Cryptography.Xml;

public class XmlDsigBase64Transform : Transform
{
	private CryptoStream cs;

	private Type[] input;

	private Type[] output;

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

	public XmlDsigBase64Transform()
	{
		base.Algorithm = "http://www.w3.org/2000/09/xmldsig#base64";
	}

	protected override XmlNodeList GetInnerXml()
	{
		return null;
	}

	public override object GetOutput()
	{
		return cs;
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
	}

	public override void LoadInput(object obj)
	{
		XmlNodeList xmlNodeList = null;
		Stream stream = null;
		if (obj is Stream)
		{
			stream = obj as Stream;
		}
		else if (obj is XmlDocument)
		{
			xmlNodeList = (obj as XmlDocument).SelectNodes("//.");
		}
		else if (obj is XmlNodeList)
		{
			xmlNodeList = (XmlNodeList)obj;
		}
		if (xmlNodeList != null)
		{
			stream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(stream);
			foreach (XmlNode item in xmlNodeList)
			{
				switch (item.NodeType)
				{
				case XmlNodeType.Attribute:
				case XmlNodeType.Text:
				case XmlNodeType.CDATA:
				case XmlNodeType.Whitespace:
				case XmlNodeType.SignificantWhitespace:
					streamWriter.Write(item.Value);
					break;
				}
			}
			streamWriter.Flush();
			stream.Position = 0L;
		}
		if (stream != null)
		{
			cs = new CryptoStream(stream, new FromBase64Transform(), CryptoStreamMode.Read);
		}
	}
}
