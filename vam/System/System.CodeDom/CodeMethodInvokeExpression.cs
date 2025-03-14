using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeMethodInvokeExpression : CodeExpression
{
	private CodeMethodReferenceExpression method;

	private CodeExpressionCollection parameters;

	public CodeMethodReferenceExpression Method
	{
		get
		{
			if (method == null)
			{
				method = new CodeMethodReferenceExpression();
			}
			return method;
		}
		set
		{
			method = value;
		}
	}

	public CodeExpressionCollection Parameters
	{
		get
		{
			if (parameters == null)
			{
				parameters = new CodeExpressionCollection();
			}
			return parameters;
		}
	}

	public CodeMethodInvokeExpression()
	{
	}

	public CodeMethodInvokeExpression(CodeMethodReferenceExpression method, params CodeExpression[] parameters)
	{
		this.method = method;
		Parameters.AddRange(parameters);
	}

	public CodeMethodInvokeExpression(CodeExpression targetObject, string methodName, params CodeExpression[] parameters)
	{
		method = new CodeMethodReferenceExpression(targetObject, methodName);
		Parameters.AddRange(parameters);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
