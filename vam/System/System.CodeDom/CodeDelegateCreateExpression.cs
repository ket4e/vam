using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeDelegateCreateExpression : CodeExpression
{
	private CodeTypeReference delegateType;

	private string methodName;

	private CodeExpression targetObject;

	public CodeTypeReference DelegateType
	{
		get
		{
			if (delegateType == null)
			{
				delegateType = new CodeTypeReference(string.Empty);
			}
			return delegateType;
		}
		set
		{
			delegateType = value;
		}
	}

	public string MethodName
	{
		get
		{
			if (methodName == null)
			{
				return string.Empty;
			}
			return methodName;
		}
		set
		{
			methodName = value;
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

	public CodeDelegateCreateExpression()
	{
	}

	public CodeDelegateCreateExpression(CodeTypeReference delegateType, CodeExpression targetObject, string methodName)
	{
		this.delegateType = delegateType;
		this.targetObject = targetObject;
		this.methodName = methodName;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
