using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeDelegateInvokeExpression : CodeExpression
{
	private CodeExpressionCollection parameters;

	private CodeExpression targetObject;

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

	public CodeExpression TargetObject
	{
		get
		{
			return targetObject;
		}
		set
		{
			targetObject = value;
		}
	}

	public CodeDelegateInvokeExpression()
	{
	}

	public CodeDelegateInvokeExpression(CodeExpression targetObject)
	{
		this.targetObject = targetObject;
	}

	public CodeDelegateInvokeExpression(CodeExpression targetObject, params CodeExpression[] parameters)
	{
		this.targetObject = targetObject;
		Parameters.AddRange(parameters);
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
