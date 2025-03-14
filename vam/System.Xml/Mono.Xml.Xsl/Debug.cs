using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl;

internal class Debug
{
	[Conditional("_DEBUG")]
	internal static void TraceContext(XPathNavigator context)
	{
		string text = "(null)";
		if (context == null)
		{
			return;
		}
		context = context.Clone();
		XPathNodeType nodeType = context.NodeType;
		if (nodeType == XPathNodeType.Element)
		{
			text = $"<{context.Prefix}:{context.LocalName}";
			bool flag = context.MoveToFirstAttribute();
			while (flag)
			{
				text += string.Format(CultureInfo.InvariantCulture, " {0}:{1}={2}", context.Prefix, context.LocalName, context.Value);
				flag = context.MoveToNextAttribute();
			}
			text += ">";
		}
	}

	[Conditional("DEBUG")]
	internal static void Assert(bool condition, string message)
	{
		if (!condition)
		{
			throw new XsltException(message, null);
		}
	}

	[Conditional("_DEBUG")]
	internal static void WriteLine(object value)
	{
		Console.Error.WriteLine(value);
	}

	[Conditional("_DEBUG")]
	internal static void WriteLine(string message)
	{
		Console.Error.WriteLine(message);
	}

	[Conditional("DEBUG")]
	internal static void EnterNavigator(Compiler c)
	{
	}

	[Conditional("DEBUG")]
	internal static void ExitNavigator(Compiler c)
	{
	}
}
