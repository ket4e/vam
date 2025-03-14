using System.Xml;
using System.Xml.XPath;
using Mono.Xml.XPath;

namespace Mono.Xml.Xsl;

internal class XslKey
{
	private XmlQualifiedName name;

	private CompiledExpression useExpr;

	private Pattern matchPattern;

	public XmlQualifiedName Name => name;

	internal CompiledExpression Use => useExpr;

	internal Pattern Match => matchPattern;

	public XslKey(Compiler c)
	{
		name = c.ParseQNameAttribute("name");
		c.KeyCompilationMode = true;
		useExpr = c.CompileExpression(c.GetAttribute("use"));
		if (useExpr == null)
		{
			useExpr = c.CompileExpression(".");
		}
		c.AssertAttribute("match");
		string attribute = c.GetAttribute("match");
		matchPattern = c.CompilePattern(attribute, c.Input);
		c.KeyCompilationMode = false;
	}
}
