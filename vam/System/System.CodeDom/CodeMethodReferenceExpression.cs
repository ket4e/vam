using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeMethodReferenceExpression : CodeExpression
{
	private string methodName;

	private CodeExpression targetObject;

	private CodeTypeReferenceCollection typeArguments;

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

	[ComVisible(false)]
	public CodeTypeReferenceCollection TypeArguments
	{
		get
		{
			if (typeArguments == null)
			{
				typeArguments = new CodeTypeReferenceCollection();
			}
			return typeArguments;
		}
	}

	public CodeMethodReferenceExpression()
	{
	}

	public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName)
	{
		this.targetObject = targetObject;
		this.methodName = methodName;
	}

	public CodeMethodReferenceExpression(CodeExpression targetObject, string methodName, params CodeTypeReference[] typeParameters)
		: this(targetObject, methodName)
	{
		if (typeParameters != null && typeParameters.Length > 0)
		{
			TypeArguments.AddRange(typeParameters);
		}
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
