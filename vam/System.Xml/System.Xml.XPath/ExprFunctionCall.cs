using System.Collections;
using System.Xml.Xsl;

namespace System.Xml.XPath;

internal class ExprFunctionCall : Expression
{
	protected readonly XmlQualifiedName _name;

	protected readonly bool resolvedName;

	protected readonly ArrayList _args = new ArrayList();

	public override XPathResultType ReturnType => XPathResultType.Any;

	internal override bool Peer => false;

	public ExprFunctionCall(XmlQualifiedName name, FunctionArguments args, IStaticXsltContext ctx)
	{
		if (ctx != null)
		{
			name = ctx.LookupQName(name.ToString());
			resolvedName = true;
		}
		_name = name;
		args?.ToArrayList(_args);
	}

	public static Expression Factory(XmlQualifiedName name, FunctionArguments args, IStaticXsltContext ctx)
	{
		if (name.Namespace != null && name.Namespace != string.Empty)
		{
			return new ExprFunctionCall(name, args, ctx);
		}
		return name.Name switch
		{
			"last" => new XPathFunctionLast(args), 
			"position" => new XPathFunctionPosition(args), 
			"count" => new XPathFunctionCount(args), 
			"id" => new XPathFunctionId(args), 
			"local-name" => new XPathFunctionLocalName(args), 
			"namespace-uri" => new XPathFunctionNamespaceUri(args), 
			"name" => new XPathFunctionName(args), 
			"string" => new XPathFunctionString(args), 
			"concat" => new XPathFunctionConcat(args), 
			"starts-with" => new XPathFunctionStartsWith(args), 
			"contains" => new XPathFunctionContains(args), 
			"substring-before" => new XPathFunctionSubstringBefore(args), 
			"substring-after" => new XPathFunctionSubstringAfter(args), 
			"substring" => new XPathFunctionSubstring(args), 
			"string-length" => new XPathFunctionStringLength(args), 
			"normalize-space" => new XPathFunctionNormalizeSpace(args), 
			"translate" => new XPathFunctionTranslate(args), 
			"boolean" => new XPathFunctionBoolean(args), 
			"not" => new XPathFunctionNot(args), 
			"true" => new XPathFunctionTrue(args), 
			"false" => new XPathFunctionFalse(args), 
			"lang" => new XPathFunctionLang(args), 
			"number" => new XPathFunctionNumber(args), 
			"sum" => new XPathFunctionSum(args), 
			"floor" => new XPathFunctionFloor(args), 
			"ceiling" => new XPathFunctionCeil(args), 
			"round" => new XPathFunctionRound(args), 
			_ => new ExprFunctionCall(name, args, ctx), 
		};
	}

	public override string ToString()
	{
		string text = string.Empty;
		for (int i = 0; i < _args.Count; i++)
		{
			Expression expression = (Expression)_args[i];
			if (text != string.Empty)
			{
				text += ", ";
			}
			text += expression.ToString();
		}
		return _name.ToString() + '(' + text + ')';
	}

	public override XPathResultType GetReturnType(BaseIterator iter)
	{
		return XPathResultType.Any;
	}

	private XPathResultType[] GetArgTypes(BaseIterator iter)
	{
		XPathResultType[] array = new XPathResultType[_args.Count];
		for (int i = 0; i < _args.Count; i++)
		{
			array[i] = ((Expression)_args[i]).GetReturnType(iter);
		}
		return array;
	}

	public override object Evaluate(BaseIterator iter)
	{
		XPathResultType[] argTypes = GetArgTypes(iter);
		IXsltContextFunction xsltContextFunction = null;
		XsltContext xsltContext = iter.NamespaceManager as XsltContext;
		if (xsltContext != null)
		{
			xsltContextFunction = ((!resolvedName) ? xsltContext.ResolveFunction(_name.Namespace, _name.Name, argTypes) : xsltContext.ResolveFunction(_name, argTypes));
		}
		if (xsltContextFunction == null)
		{
			throw new XPathException("function " + _name.ToString() + " not found");
		}
		object[] array = new object[_args.Count];
		if (xsltContextFunction.Maxargs != 0)
		{
			XPathResultType[] argTypes2 = xsltContextFunction.ArgTypes;
			for (int i = 0; i < _args.Count; i++)
			{
				XPathResultType type = ((argTypes2 != null) ? ((i >= argTypes2.Length) ? argTypes2[argTypes2.Length - 1] : argTypes2[i]) : XPathResultType.Any);
				Expression expression = (Expression)_args[i];
				object obj = expression.EvaluateAs(iter, type);
				array[i] = obj;
			}
		}
		return xsltContextFunction.Invoke(xsltContext, array, iter.Current);
	}
}
