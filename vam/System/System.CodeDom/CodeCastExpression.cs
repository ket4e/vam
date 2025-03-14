using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeCastExpression : CodeExpression
{
	private CodeTypeReference targetType;

	private CodeExpression expression;

	public CodeExpression Expression
	{
		get
		{
			return expression;
		}
		set
		{
			expression = value;
		}
	}

	public CodeTypeReference TargetType
	{
		get
		{
			if (targetType == null)
			{
				targetType = new CodeTypeReference(string.Empty);
			}
			return targetType;
		}
		set
		{
			targetType = value;
		}
	}

	public CodeCastExpression()
	{
	}

	public CodeCastExpression(CodeTypeReference targetType, CodeExpression expression)
	{
		this.targetType = targetType;
		this.expression = expression;
	}

	public CodeCastExpression(string targetType, CodeExpression expression)
	{
		this.targetType = new CodeTypeReference(targetType);
		this.expression = expression;
	}

	public CodeCastExpression(Type targetType, CodeExpression expression)
	{
		this.targetType = new CodeTypeReference(targetType);
		this.expression = expression;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
