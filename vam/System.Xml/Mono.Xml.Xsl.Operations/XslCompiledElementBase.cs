using System.Xml;
using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations;

internal abstract class XslCompiledElementBase : XslOperation
{
	private int lineNumber;

	private int linePosition;

	private XPathNavigator debugInput;

	public XPathNavigator DebugInput => debugInput;

	public int LineNumber => lineNumber;

	public int LinePosition => linePosition;

	public XslCompiledElementBase(Compiler c)
	{
		if (c.Input is IXmlLineInfo xmlLineInfo)
		{
			lineNumber = xmlLineInfo.LineNumber;
			linePosition = xmlLineInfo.LinePosition;
		}
		if (c.Debugger != null)
		{
			debugInput = c.Input.Clone();
		}
	}

	protected abstract void Compile(Compiler c);
}
