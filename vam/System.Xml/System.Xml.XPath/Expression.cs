using System.Globalization;

namespace System.Xml.XPath;

internal abstract class Expression
{
	public abstract XPathResultType ReturnType { get; }

	public virtual bool HasStaticValue => false;

	public virtual object StaticValue => ReturnType switch
	{
		XPathResultType.String => StaticValueAsString, 
		XPathResultType.Number => StaticValueAsNumber, 
		XPathResultType.Boolean => StaticValueAsBoolean, 
		_ => null, 
	};

	public virtual string StaticValueAsString => (!HasStaticValue) ? null : XPathFunctions.ToString(StaticValue);

	public virtual double StaticValueAsNumber => (!HasStaticValue) ? 0.0 : XPathFunctions.ToNumber(StaticValue);

	public virtual bool StaticValueAsBoolean => HasStaticValue && XPathFunctions.ToBoolean(StaticValue);

	public virtual XPathNavigator StaticValueAsNavigator => StaticValue as XPathNavigator;

	internal virtual XPathNodeType EvaluatedNodeType => XPathNodeType.All;

	internal virtual bool IsPositional => false;

	internal virtual bool Peer => false;

	public virtual bool RequireSorting => false;

	public Expression()
	{
	}

	public virtual XPathResultType GetReturnType(BaseIterator iter)
	{
		return ReturnType;
	}

	public virtual Expression Optimize()
	{
		return this;
	}

	public abstract object Evaluate(BaseIterator iter);

	public virtual BaseIterator EvaluateNodeSet(BaseIterator iter)
	{
		XPathResultType returnType = GetReturnType(iter);
		switch (returnType)
		{
		case XPathResultType.NodeSet:
		case XPathResultType.Navigator:
		case XPathResultType.Any:
		{
			object obj = Evaluate(iter);
			XPathNodeIterator xPathNodeIterator = obj as XPathNodeIterator;
			BaseIterator baseIterator = null;
			if (xPathNodeIterator != null)
			{
				baseIterator = xPathNodeIterator as BaseIterator;
				if (baseIterator == null)
				{
					baseIterator = new WrapperIterator(xPathNodeIterator, iter.NamespaceManager);
				}
				return baseIterator;
			}
			if (obj is XPathNavigator xPathNavigator)
			{
				XPathNodeIterator xPathNodeIterator2 = xPathNavigator.SelectChildren(XPathNodeType.All);
				baseIterator = xPathNodeIterator2 as BaseIterator;
				if (baseIterator == null && xPathNodeIterator2 != null)
				{
					baseIterator = new WrapperIterator(xPathNodeIterator2, iter.NamespaceManager);
				}
			}
			if (baseIterator != null)
			{
				return baseIterator;
			}
			if (obj == null)
			{
				return new NullIterator(iter);
			}
			returnType = GetReturnType(obj);
			break;
		}
		}
		throw new XPathException(string.Format("expected nodeset but was {1}: {0}", ToString(), returnType));
	}

	protected static XPathResultType GetReturnType(object obj)
	{
		if (obj is string)
		{
			return XPathResultType.String;
		}
		if (obj is bool)
		{
			return XPathResultType.Boolean;
		}
		if (obj is XPathNodeIterator)
		{
			return XPathResultType.NodeSet;
		}
		if (obj is double || obj is int)
		{
			return XPathResultType.Number;
		}
		if (obj is XPathNavigator)
		{
			return XPathResultType.Navigator;
		}
		throw new XPathException("invalid node type: " + obj.GetType().ToString());
	}

	public virtual double EvaluateNumber(BaseIterator iter)
	{
		XPathResultType xPathResultType = GetReturnType(iter);
		object obj;
		if (xPathResultType == XPathResultType.NodeSet)
		{
			obj = EvaluateString(iter);
			xPathResultType = XPathResultType.String;
		}
		else
		{
			obj = Evaluate(iter);
		}
		if (xPathResultType == XPathResultType.Any)
		{
			xPathResultType = GetReturnType(obj);
		}
		switch (xPathResultType)
		{
		case XPathResultType.Number:
			if (obj is double)
			{
				return (double)obj;
			}
			if (obj is IConvertible)
			{
				return ((IConvertible)obj).ToDouble(CultureInfo.InvariantCulture);
			}
			return (double)obj;
		case XPathResultType.Boolean:
			return (!(bool)obj) ? 0.0 : 1.0;
		case XPathResultType.NodeSet:
			return XPathFunctions.ToNumber(EvaluateString(iter));
		case XPathResultType.String:
			return XPathFunctions.ToNumber((string)obj);
		case XPathResultType.Navigator:
			return XPathFunctions.ToNumber(((XPathNavigator)obj).Value);
		default:
			throw new XPathException("invalid node type");
		}
	}

	public virtual string EvaluateString(BaseIterator iter)
	{
		object obj = Evaluate(iter);
		XPathResultType returnType = GetReturnType(iter);
		if (returnType == XPathResultType.Any)
		{
			returnType = GetReturnType(obj);
		}
		switch (returnType)
		{
		case XPathResultType.Number:
		{
			double num = (double)obj;
			return XPathFunctions.ToString(num);
		}
		case XPathResultType.Boolean:
			return (!(bool)obj) ? "false" : "true";
		case XPathResultType.String:
			return (string)obj;
		case XPathResultType.NodeSet:
		{
			BaseIterator baseIterator = (BaseIterator)obj;
			if (baseIterator == null || !baseIterator.MoveNext())
			{
				return string.Empty;
			}
			return baseIterator.Current.Value;
		}
		case XPathResultType.Navigator:
			return ((XPathNavigator)obj).Value;
		default:
			throw new XPathException("invalid node type");
		}
	}

	public virtual bool EvaluateBoolean(BaseIterator iter)
	{
		object obj = Evaluate(iter);
		XPathResultType returnType = GetReturnType(iter);
		if (returnType == XPathResultType.Any)
		{
			returnType = GetReturnType(obj);
		}
		switch (returnType)
		{
		case XPathResultType.Number:
		{
			double num = Convert.ToDouble(obj);
			return num != 0.0 && num != -0.0 && !double.IsNaN(num);
		}
		case XPathResultType.Boolean:
			return (bool)obj;
		case XPathResultType.String:
			return ((string)obj).Length != 0;
		case XPathResultType.NodeSet:
			return ((BaseIterator)obj)?.MoveNext() ?? false;
		case XPathResultType.Navigator:
			return ((XPathNavigator)obj).HasChildren;
		default:
			throw new XPathException("invalid node type");
		}
	}

	public object EvaluateAs(BaseIterator iter, XPathResultType type)
	{
		return type switch
		{
			XPathResultType.Boolean => EvaluateBoolean(iter), 
			XPathResultType.NodeSet => EvaluateNodeSet(iter), 
			XPathResultType.String => EvaluateString(iter), 
			XPathResultType.Number => EvaluateNumber(iter), 
			_ => Evaluate(iter), 
		};
	}
}
