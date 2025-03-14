using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl;

internal class XsltElementAvailable : XPathFunction
{
	private Expression arg0;

	private IStaticXsltContext ctx;

	public override XPathResultType ReturnType => XPathResultType.Boolean;

	internal override bool Peer => arg0.Peer;

	public XsltElementAvailable(FunctionArguments args, IStaticXsltContext ctx)
		: base(args)
	{
		if (args == null || args.Tail != null)
		{
			throw new XPathException("element-available takes 1 arg");
		}
		arg0 = args.Arg;
		this.ctx = ctx;
	}

	public override object Evaluate(BaseIterator iter)
	{
		XmlQualifiedName xmlQualifiedName = XslNameUtil.FromString(arg0.EvaluateString(iter), ctx);
		return xmlQualifiedName.Namespace == "http://www.w3.org/1999/XSL/Transform" && (xmlQualifiedName.Name == "apply-imports" || xmlQualifiedName.Name == "apply-templates" || xmlQualifiedName.Name == "call-template" || xmlQualifiedName.Name == "choose" || xmlQualifiedName.Name == "comment" || xmlQualifiedName.Name == "copy" || xmlQualifiedName.Name == "copy-of" || xmlQualifiedName.Name == "element" || xmlQualifiedName.Name == "fallback" || xmlQualifiedName.Name == "for-each" || xmlQualifiedName.Name == "message" || xmlQualifiedName.Name == "number" || xmlQualifiedName.Name == "processing-instruction" || xmlQualifiedName.Name == "text" || xmlQualifiedName.Name == "value-of" || xmlQualifiedName.Name == "variable");
	}
}
