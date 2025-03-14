using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations;

internal class XslCopyOf : XslCompiledElement
{
	private XPathExpression select;

	public XslCopyOf(Compiler c)
		: base(c)
	{
	}

	protected override void Compile(Compiler c)
	{
		if (c.Debugger != null)
		{
			c.Debugger.DebugCompile(c.Input);
		}
		c.CheckExtraAttributes("copy-of", "select");
		c.AssertAttribute("select");
		select = c.CompileExpression(c.GetAttribute("select"));
	}

	private void CopyNode(XslTransformProcessor p, XPathNavigator nav)
	{
		Outputter @out = p.Out;
		switch (nav.NodeType)
		{
		case XPathNodeType.Root:
		{
			XPathNodeIterator xPathNodeIterator = nav.SelectChildren(XPathNodeType.All);
			while (xPathNodeIterator.MoveNext())
			{
				CopyNode(p, xPathNodeIterator.Current);
			}
			break;
		}
		case XPathNodeType.Element:
		{
			bool insideCDataElement = p.InsideCDataElement;
			string prefix = nav.Prefix;
			string namespaceURI = nav.NamespaceURI;
			p.PushElementState(prefix, nav.LocalName, namespaceURI, preserveWhitespace: false);
			@out.WriteStartElement(prefix, nav.LocalName, namespaceURI);
			if (nav.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
			{
				do
				{
					if (!(prefix == nav.Name) && (nav.Name.Length != 0 || namespaceURI.Length != 0))
					{
						@out.WriteNamespaceDecl(nav.Name, nav.Value);
					}
				}
				while (nav.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml));
				nav.MoveToParent();
			}
			if (nav.MoveToFirstAttribute())
			{
				do
				{
					@out.WriteAttributeString(nav.Prefix, nav.LocalName, nav.NamespaceURI, nav.Value);
				}
				while (nav.MoveToNextAttribute());
				nav.MoveToParent();
			}
			if (nav.MoveToFirstChild())
			{
				do
				{
					CopyNode(p, nav);
				}
				while (nav.MoveToNext());
				nav.MoveToParent();
			}
			if (nav.IsEmptyElement)
			{
				@out.WriteEndElement();
			}
			else
			{
				@out.WriteFullEndElement();
			}
			p.PopCDataState(insideCDataElement);
			break;
		}
		case XPathNodeType.Namespace:
			if (nav.Name != p.XPathContext.ElementPrefix && (p.XPathContext.ElementNamespace.Length > 0 || nav.Name.Length > 0))
			{
				@out.WriteNamespaceDecl(nav.Name, nav.Value);
			}
			break;
		case XPathNodeType.Attribute:
			@out.WriteAttributeString(nav.Prefix, nav.LocalName, nav.NamespaceURI, nav.Value);
			break;
		case XPathNodeType.SignificantWhitespace:
		case XPathNodeType.Whitespace:
		{
			bool insideCDataSection = @out.InsideCDataSection;
			@out.InsideCDataSection = false;
			@out.WriteString(nav.Value);
			@out.InsideCDataSection = insideCDataSection;
			break;
		}
		case XPathNodeType.Text:
			@out.WriteString(nav.Value);
			break;
		case XPathNodeType.ProcessingInstruction:
			@out.WriteProcessingInstruction(nav.Name, nav.Value);
			break;
		case XPathNodeType.Comment:
			@out.WriteComment(nav.Value);
			break;
		}
	}

	public override void Evaluate(XslTransformProcessor p)
	{
		if (p.Debugger != null)
		{
			p.Debugger.DebugExecute(p, base.DebugInput);
		}
		object obj = p.Evaluate(select);
		if (obj is XPathNodeIterator xPathNodeIterator)
		{
			while (xPathNodeIterator.MoveNext())
			{
				CopyNode(p, xPathNodeIterator.Current);
			}
		}
		else if (obj is XPathNavigator nav)
		{
			CopyNode(p, nav);
		}
		else
		{
			p.Out.WriteString(XPathFunctions.ToString(obj));
		}
	}
}
