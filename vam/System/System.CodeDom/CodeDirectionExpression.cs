using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[ComVisible(true)]
public class CodeDirectionExpression : CodeExpression
{
	private FieldDirection direction;

	private CodeExpression expression;

	public FieldDirection Direction
	{
		get
		{
			return direction;
		}
		set
		{
			direction = value;
		}
	}

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

	public CodeDirectionExpression()
	{
	}

	public CodeDirectionExpression(FieldDirection direction, CodeExpression expression)
	{
		this.direction = direction;
		this.expression = expression;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
