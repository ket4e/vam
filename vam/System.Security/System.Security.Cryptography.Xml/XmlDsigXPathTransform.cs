using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace System.Security.Cryptography.Xml;

public class XmlDsigXPathTransform : Transform
{
	internal class XmlDsigXPathContext : XsltContext
	{
		private XmlDsigXPathFunctionHere here;

		public override bool Whitespace => true;

		public XmlDsigXPathContext(XmlNode node)
		{
			here = new XmlDsigXPathFunctionHere(node);
		}

		public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argType)
		{
			if (name == "here" && prefix == string.Empty && argType.Length == 0)
			{
				return here;
			}
			return null;
		}

		public override bool PreserveWhitespace(XPathNavigator node)
		{
			return true;
		}

		public override int CompareDocument(string s1, string s2)
		{
			return string.Compare(s1, s2);
		}

		public override IXsltContextVariable ResolveVariable(string prefix, string name)
		{
			throw new InvalidOperationException();
		}
	}

	internal class XmlDsigXPathFunctionHere : IXsltContextFunction
	{
		private static XPathResultType[] types;

		private XPathNodeIterator xpathNode;

		public XPathResultType[] ArgTypes => types;

		public int Maxargs => 0;

		public int Minargs => 0;

		public XPathResultType ReturnType => XPathResultType.NodeSet;

		public XmlDsigXPathFunctionHere(XmlNode node)
		{
			xpathNode = node.CreateNavigator().Select(".");
		}

		static XmlDsigXPathFunctionHere()
		{
			types = new XPathResultType[0];
		}

		public object Invoke(XsltContext ctx, object[] args, XPathNavigator docContext)
		{
			if (args.Length != 0)
			{
				throw new ArgumentException("Not allowed arguments for function here().", "args");
			}
			return xpathNode.Clone();
		}
	}

	private Type[] input;

	private Type[] output;

	private XmlNodeList xpath;

	private XmlDocument doc;

	private XsltContext ctx;

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
				output[0] = typeof(XmlNodeList);
			}
			return output;
		}
	}

	public XmlDsigXPathTransform()
	{
		base.Algorithm = "http://www.w3.org/TR/1999/REC-xpath-19991116";
	}

	protected override XmlNodeList GetInnerXml()
	{
		if (xpath == null)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml("<XPath xmlns=\"http://www.w3.org/2000/09/xmldsig#\"></XPath>");
			xpath = xmlDocument.ChildNodes;
		}
		return xpath;
	}

	[System.MonoTODO("Evaluation of extension function here() results in different from MS.NET (is MS.NET really correct??).")]
	public override object GetOutput()
	{
		if (xpath == null || doc == null)
		{
			return new XmlDsigNodeList(new ArrayList());
		}
		string text = null;
		for (int i = 0; i < xpath.Count; i++)
		{
			switch (xpath[i].NodeType)
			{
			case XmlNodeType.Element:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				text += xpath[i].InnerText;
				break;
			}
		}
		ctx = new XmlDsigXPathContext(doc);
		foreach (XmlNode item in xpath)
		{
			XPathNavigator xPathNavigator = item.CreateNavigator();
			XPathNodeIterator xPathNodeIterator = xPathNavigator.Select("namespace::*");
			while (xPathNodeIterator.MoveNext())
			{
				if (xPathNodeIterator.Current.LocalName != "xml")
				{
					ctx.AddNamespace(xPathNodeIterator.Current.LocalName, xPathNodeIterator.Current.Value);
				}
			}
		}
		return EvaluateMatch(doc, text);
	}

	public override object GetOutput(Type type)
	{
		if (type != typeof(XmlNodeList))
		{
			throw new ArgumentException("type");
		}
		return GetOutput();
	}

	private XmlDsigNodeList EvaluateMatch(XmlNode n, string xpath)
	{
		ArrayList arrayList = new ArrayList();
		XPathNavigator xPathNavigator = n.CreateNavigator();
		XPathExpression xPathExpression = xPathNavigator.Compile(xpath);
		xPathExpression.SetContext(ctx);
		EvaluateMatch(n, xPathExpression, arrayList);
		return new XmlDsigNodeList(arrayList);
	}

	private void EvaluateMatch(XmlNode n, XPathExpression exp, ArrayList al)
	{
		if (NodeMatches(n, exp))
		{
			al.Add(n);
		}
		if (n.Attributes != null)
		{
			for (int i = 0; i < n.Attributes.Count; i++)
			{
				if (NodeMatches(n.Attributes[i], exp))
				{
					al.Add(n.Attributes[i]);
				}
			}
		}
		for (int j = 0; j < n.ChildNodes.Count; j++)
		{
			EvaluateMatch(n.ChildNodes[j], exp, al);
		}
	}

	private bool NodeMatches(XmlNode n, XPathExpression exp)
	{
		object obj = n.CreateNavigator().Evaluate(exp);
		if (obj is bool)
		{
			return (bool)obj;
		}
		if (obj is double num)
		{
			return num != 0.0 && !double.IsNaN(num);
		}
		if (obj is string)
		{
			return ((string)obj).Length > 0;
		}
		if (obj is XPathNodeIterator)
		{
			XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)obj;
			return xPathNodeIterator.Count > 0;
		}
		return false;
	}

	public override void LoadInnerXml(XmlNodeList nodeList)
	{
		if (nodeList == null)
		{
			throw new CryptographicException("nodeList");
		}
		xpath = nodeList;
	}

	public override void LoadInput(object obj)
	{
		if (obj is Stream)
		{
			doc = new XmlDocument();
			doc.PreserveWhitespace = true;
			doc.XmlResolver = GetResolver();
			doc.Load(new XmlSignatureStreamReader(new StreamReader((Stream)obj)));
		}
		else if (obj is XmlDocument)
		{
			doc = obj as XmlDocument;
		}
		else
		{
			if (!(obj is XmlNodeList))
			{
				return;
			}
			doc = new XmlDocument();
			doc.XmlResolver = GetResolver();
			foreach (XmlNode item in obj as XmlNodeList)
			{
				XmlNode newChild = doc.ImportNode(item, deep: true);
				doc.AppendChild(newChild);
			}
		}
	}
}
