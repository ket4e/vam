using System.Runtime.InteropServices;

namespace System.CodeDom;

[Serializable]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
public class CodeBinaryOperatorExpression : CodeExpression
{
	private CodeExpression left;

	private CodeExpression right;

	private CodeBinaryOperatorType op;

	public CodeExpression Left
	{
		get
		{
			return left;
		}
		set
		{
			left = value;
		}
	}

	public CodeBinaryOperatorType Operator
	{
		get
		{
			return op;
		}
		set
		{
			op = value;
		}
	}

	public CodeExpression Right
	{
		get
		{
			return right;
		}
		set
		{
			right = value;
		}
	}

	public CodeBinaryOperatorExpression()
	{
	}

	public CodeBinaryOperatorExpression(CodeExpression left, CodeBinaryOperatorType op, CodeExpression right)
	{
		this.left = left;
		this.op = op;
		this.right = right;
	}

	internal override void Accept(System.CodeDom.ICodeDomVisitor visitor)
	{
		visitor.Visit(this);
	}
}
